#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Store.Areas.Identity.Pages.Account.Manage;

public class DeletePersonalDataModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<DeletePersonalDataModel> _logger;

    public DeletePersonalDataModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<DeletePersonalDataModel> logger) {
        _userManager   = userManager;
        _signInManager = signInManager;
        _logger        = logger;
    }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel {
        [Required(ErrorMessage = "{0} ضروری است.")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGet() {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        RequirePassword = await _userManager.HasPasswordAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync() {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        RequirePassword = await _userManager.HasPasswordAsync(user);
        if (RequirePassword) {
            if (!await _userManager.CheckPasswordAsync(user, Input.Password)) {
                ModelState.AddModelError(string.Empty, "رمز عبور اشتباه است.");
                return Page();
            }
        }

        var result = await _userManager.DeleteAsync(user);
        var userId = await _userManager.GetUserIdAsync(user);
        if (!result.Succeeded) {
            throw new InvalidOperationException($"Unexpected error occurred deleting user.");
        }

        await _signInManager.SignOutAsync();

        _logger.LogInformation("User with ID '{UserId}' deleted themselves", userId);

        return Redirect("~/");
    }
}