#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel {
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger) {
        _userManager   = userManager;
        _userStore     = userStore;
        _signInManager = signInManager;
        _logger        = logger;
    }

    [BindProperty] public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel {
        [Required(ErrorMessage = "وارد کردن {0} ضروری است")]
        [EmailAddress(ErrorMessage = "{0} معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد کردن {0} ضروری است")]
        [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} کاراکتر باشد.",
            MinimumLength = 3)]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [Compare("Password", ErrorMessage = "رمز عبور با تکرار آن مطابق نیست.")]
        public string ConfirmPassword { get; set; }
    }


    public void OnGetAsync(string returnUrl = null) {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
        returnUrl ??= Url.Content("~/");
        if (!ModelState.IsValid) return Page();

        var user = CreateUser();

        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        var _      = await _userManager.SetEmailAsync(user, Input.Email); //ignore error if set email failed
        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded) {
            var userId = await _userManager.GetUserIdAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        foreach (var error in result.Errors) {
            if (error.Code == "DuplicateUserName") {
                ModelState.AddModelError("Input.Email", "نام کاربری تکراری است.");
            }
            else {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }

    private IdentityUser CreateUser() {
        try {
            return Activator.CreateInstance<IdentityUser>();
        }
        catch {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                                                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
}