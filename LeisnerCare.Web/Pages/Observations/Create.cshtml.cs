using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeisnerCare.Web.Pages.Observations;

[Authorize(Roles = "Relative,Staff")]
public class CreateModel : PageModel
{
    private readonly ObservationService _observationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public CreateModel(ObservationService observationService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _observationService = observationService;
        _userManager = userManager;
        _context = context;
    }

    [BindProperty]
    public ObservationInput Input { get; set; } = new();

    public List<SelectListItem> Patients { get; set; } = new();
    public bool IsStaff { get; set; } = false;

    public class ObservationInput
    {
        public int PatientId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsClinical { get; set; } = false;
    }

    public void OnGet()
    {
        IsStaff = User.IsInRole("Staff");
        LoadPatients();
    }

    private void LoadPatients()
    {
        Patients = _context.Patients
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.CprNumber
            })
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            IsStaff = User.IsInRole("Staff");
            LoadPatients();
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage();

        var observation = new Observation
        {
            PatientId = Input.PatientId,
            AuthorId = user.Id,
            AuthorName = $"{user.FirstName} {user.LastName}",
            AuthorRole = user.Role.ToString(),
            Content = Input.Content,
            IsClinical = IsStaff && Input.IsClinical
        };

        await _observationService.CreateAsync(observation);
        return RedirectToPage("./Index");
    }
}