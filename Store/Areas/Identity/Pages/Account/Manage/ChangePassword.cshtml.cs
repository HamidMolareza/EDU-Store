#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Store.Areas.Identity.Pages.Account.Manage;

public class ChangePasswordModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<ChangePasswordModel> _logger;

    public ChangePasswordModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<ChangePasswordModel> logger) {
        _userManager   = userManager;
        _signInManager = signInManager;
        _logger        = logger;
    }

    [BindProperty] public InputModel Input { get; set; }

    [TempData] public string StatusMessage { get; set; }

    public class InputModel {
        [Required(ErrorMessage = "{0} ضروری است.")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز فعلی")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "{0} ضروری است.")]
        [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} کاراکتر باشه.",
            MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز جدید")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز جدید")]
        [Compare("NewPassword", ErrorMessage = "رمز جدید با تکرار آن برابر نیست.")]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync() {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword) {
            return RedirectToPage("./SetPassword");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid) {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        var changePasswordResult =
            await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded) {
            foreach (var error in changePasswordResult.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("User changed their password successfully");

        StatusMessage = "رمز جدید ثبت شد";
        return RedirectToPage();
    }
}