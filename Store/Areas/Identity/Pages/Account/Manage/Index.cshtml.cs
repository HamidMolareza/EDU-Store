#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Store.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IndexModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager) {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    [TempData] public string StatusMessage { get; set; }

    [Display(Name = "نام کاربری")] public string Username { get; set; }
    [BindProperty] public InputModel Input { get; set; }

    public class InputModel {
        [Phone(ErrorMessage = "{0} درست نیست.")]
        [Display(Name = "شماره تلفن")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "{0} معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
    }

    public async Task<IActionResult> OnGetAsync() {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        await LoadAsync(user);
        return Page();
    }

    private async Task LoadAsync(IdentityUser user) {
        Username = await _userManager.GetUserNameAsync(user);

        Input = new InputModel {
            PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
            Email       = await _userManager.GetEmailAsync(user)
        };
    }

    public async Task<IActionResult> OnPostAsync() {
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
        if (Input.Email != email) {
            var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
            if (!setEmailResult.Succeeded) {
                StatusMessage = "در هنگام به روز رسانی ایمیل خطایی رخ داده است";
                return RedirectToPage();
            }
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber) {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded) {
                StatusMessage = "در هنگام به روز رسانی شماره تلفن خطایی رخ داده است";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "پروفایل به روز رسانی شد.";
        return RedirectToPage();
    }
}