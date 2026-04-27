using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Patients;

[Authorize(Roles = "Staff")]
public class CreateModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(PatientService patientService, UserManager<ApplicationUser> userManager)
    {
        _patientService = patientService;
        _userManager = userManager;
    }

    [BindProperty]
    public PatientInput Input { get; set; } = new();

    public class PatientInput
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CprNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string? ContactPhone { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // Opret bruger først
        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Role = UserRole.Patient
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _userManager.AddToRoleAsync(user, "Patient");

        // Opret patientprofil
        var patient = new Patient
        {
            UserId = user.Id,
            CprNumber = Input.CprNumber,
            DateOfBirth = Input.DateOfBirth,
            DiagnosisDate = Input.DiagnosisDate,
            ContactPhone = Input.ContactPhone,
            EmergencyContactName = Input.EmergencyContactName,
            EmergencyContactPhone = Input.EmergencyContactPhone
        };

        await _patientService.CreateAsync(patient);

        return RedirectToPage("./Index");
    }
}