using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LeisnerCare.Web.Services;

namespace LeisnerCare.Web.Pages.Dashboard;

[Authorize(Roles = "Patient")]
public class IndexModel : PageModel
{
    private readonly DashboardService _dashboardService;
    private readonly ApplicationDbContext _context;

    public IndexModel(DashboardService dashboardService, ApplicationDbContext context)
    {
        _dashboardService = dashboardService;
        _context = context;
    }

    public List<Symptom> TremorData { get; set; } = new();
    public Dictionary<string, int> OnOffData { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

        if (patient != null)
        {
            var symptoms = await _dashboardService.GetSymptomsLast30DaysAsync(patient.Id);
            TremorData = symptoms.Where(s => s.Type == SymptomType.Tremor).ToList();
            OnOffData = await _dashboardService.GetOnOffDistributionAsync(patient.Id);
        }
    }
}