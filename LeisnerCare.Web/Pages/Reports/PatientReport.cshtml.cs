using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Pages.Reports;

[Authorize(Roles = "Staff")]
public class PatientReportModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly SymptomService _symptomService;
    private readonly MedicationService _medicationService;

    public PatientReportModel(ApplicationDbContext context, SymptomService symptomService, MedicationService medicationService)
    {
        _context = context;
        _symptomService = symptomService;
        _medicationService = medicationService;
    }

    public List<SelectListItem> Patients { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? SelectedPatientId { get; set; }

    public Patient? Patient { get; set; }
    public List<Symptom> Symptoms { get; set; } = new();
    public List<Medication> Medications { get; set; } = new();
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;

    public void OnGet()
    {
        LoadPatients();

        if (SelectedPatientId.HasValue)
        {
            LoadReportData(SelectedPatientId.Value);
        }
    }

    private void LoadPatients()
    {
        Patients = _context.Patients
            .Include(p => p.User)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.User!.FirstName} {p.User.LastName} ({p.CprNumber})"
            })
            .ToList();
    }

    private void LoadReportData(int patientId)
    {
        Patient = _context.Patients
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == patientId);

        Symptoms = _context.Symptoms
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.RecordedAt)
            .Take(30)
            .ToList();

        Medications = _context.Medications
            .Where(m => m.PatientId == patientId)
            .Include(m => m.Logs)
            .ToList();
    }
}