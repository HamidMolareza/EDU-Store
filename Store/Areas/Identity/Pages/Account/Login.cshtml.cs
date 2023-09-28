#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Store.Areas.Identity.Pages.Account;

public class LoginModel : PageModel {
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger) {
        _signInManager = signInManager;
        _logger        = logger;
    }

    [BindProperty] public InputModel Input { get; set; }
    public string ReturnUrl { get; set; }

    [TempData] public string ErrorMessage { get; set; }

    public class InputModel {
        [Required(ErrorMessage = "وارد کردن {0} ضروری است")]
        [EmailAddress(ErrorMessage = "{0} معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "وارد کردن {0} ضروری است")]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "بخاطر سپردن؟")] public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null) {
        if (!string.IsNullOrEmpty(ErrorMessage)) {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe,
                         lockoutOnFailure: true);
        if (result.Succeeded) {
            _logger.LogInformation("User logged in");
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut) {
            _logger.LogWarning("User account locked out");
            return RedirectToPage("./Lockout");
        }

        ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه است");
        return Page();
    }
}