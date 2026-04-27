using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Patients;

[Authorize(Roles = "Staff")]
public class DetailsModel : PageModel
{
    private readonly PatientService _patientService;

    public DetailsModel(PatientService patientService)
    {
        _patientService = patientService;
    }

    public Patient Patient { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null) return NotFound();

        Patient = patient;
        return Page();
    }
}