#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IndexModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager) {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    [Display(Name = "نام کاربری")] public string Username { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel {
        [Phone(ErrorMessage = "{0} درست نیست.")]
        [Display(Name = "شماره تلفن")]
        public string PhoneNumber { get; set; }
    }

    private async Task LoadAsync(IdentityUser user) {
        var userName    = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        Username = userName;

        Input = new InputModel {
            PhoneNumber = phoneNumber
        };
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