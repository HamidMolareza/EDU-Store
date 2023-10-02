using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Store.Areas.Identity.Pages.Account.Manage;

public class PersonalDataModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<PersonalDataModel> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;

    public PersonalDataModel(
        UserManager<IdentityUser> userManager,
        ILogger<PersonalDataModel> logger,
        SignInManager<IdentityUser> signInManager) {
        _userManager   = userManager;
        _logger        = logger;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnGet() {
        var user = await _userManager.GetUserAsync(User);
        if (user is not null) return Page();

        await _signInManager.SignOutAsync();
        return RedirectToPage();
    }
}