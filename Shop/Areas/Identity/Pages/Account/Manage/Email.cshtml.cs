#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop.Areas.Identity.Pages.Account.Manage;

public class EmailModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public EmailModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager) {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    public string Email { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel {
        [Required(ErrorMessage = "{0} ضروری است")]
        [EmailAddress(ErrorMessage = "{0} معتبر نیست")]
        [Display(Name = "ایمیل جدید")]
        public string NewEmail { get; set; }
    }

    private async Task LoadAsync(IdentityUser user) {
        var email = await _userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel {
            NewEmail = email,
        };
    }

    public async Task<IActionResult> OnGetAsync() {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync() {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        if (!ModelState.IsValid) {
            await LoadAsync(user);
            return Page();
        }

        var email = await _userManager.GetEmailAsync(user);
        if (Input.NewEmail != email) {
            var setEmailResult = await _userManager.SetEmailAsync(user, Input.NewEmail);
            if (!setEmailResult.Succeeded) {
                StatusMessage = "در هنگام به روز رسانی ایمیل خطایی رخ داده است";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "پروفایل به روز رسانی شد.";
        return RedirectToPage();
    }
}