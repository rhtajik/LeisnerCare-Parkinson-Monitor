using LeisnerCare.Application.Services;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Pages.Medications;

[Authorize(Roles = "Patient,Relative,Staff")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Core.Entities.Medication> Medications { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

        if (patient != null)
        {
            Medications = await _context.Medications
                .Where(m => m.PatientId == patient.Id)
                .Include(m => m.Logs)
                .ToListAsync();
        }
    }
}