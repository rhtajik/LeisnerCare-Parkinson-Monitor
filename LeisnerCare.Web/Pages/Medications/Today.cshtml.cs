using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Medications;

[Authorize(Roles = "Patient,Relative")]
public class TodayModel : PageModel
{
    private readonly MedicationService _medicationService;
    private readonly ApplicationDbContext _context;

    public TodayModel(MedicationService medicationService, ApplicationDbContext context)
    {
        _medicationService = medicationService;
        _context = context;
    }

    public List<MedicationViewModel> Medications { get; set; } = new();

    public class MedicationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public bool IsTakenToday { get; set; }
        public DateTime? TakenAt { get; set; }
        public int? Effectiveness { get; set; }
        public int? LogId { get; set; }
    }

    public async Task OnGetAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

        if (patient != null)
        {
            var meds = await _medicationService.GetPatientMedicationsAsync(patient.Id);

            Medications = meds.Select(m =>
            {
                var todayLog = m.Logs.FirstOrDefault(l => l.TakenAt.Date == DateTime.Today);
                return new MedicationViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    IsTakenToday = todayLog != null,
                    TakenAt = todayLog?.TakenAt,
                    Effectiveness = todayLog?.Effectiveness,
                    LogId = todayLog?.Id
                };
            }).ToList();
        }
    }

    public async Task<IActionResult> OnPostTakeAsync(int medicationId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

        if (patient == null) return RedirectToPage();

        var log = new MedicationLog
        {
            MedicationId = medicationId,
            TakenAt = DateTime.UtcNow
        };

        await _medicationService.LogMedicationAsync(log);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRateAsync(int logId, int effectiveness)
    {
        var log = await _context.MedicationLogs.FindAsync(logId);
        if (log != null)
        {
            log.Effectiveness = effectiveness;
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}