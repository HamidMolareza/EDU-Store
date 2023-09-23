using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Areas.Admin.Pages.Members;

public class DeleteModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;

    public DeleteModel(UserManager<IdentityUser> userManager) {
        _userManager = userManager;
    }

    public User UserModel { get; set; }

    public class User {
        [Display(Name = "آی‌دی")] public string Id { get; set; } = default!;
        [Display(Name = "نام کاربری")] public string Username { get; set; } = default!;
        [Display(Name = "نقش‌ها")] public string Roles { get; set; } = default!;
        [Display(Name = "ایمیل")] public string? Email { get; set; }
        [Display(Name = "قفل شده؟")] public bool IsLockout { get; set; }
        [Display(Name = "تلفن")] public string? Phone { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string? id) {
        if (id is null)
            return NotFound();

        var user = await _userManager.Users.AsNoTracking()
                       .FirstOrDefaultAsync(user => user.Id == id);
        if (user is null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        UserModel = new User {
            Id        = user.Id,
            Username  = user.UserName!,
            Roles     = string.Join(", ", roles),
            Phone     = user.PhoneNumber,
            Email     = user.Email,
            IsLockout = await _userManager.IsLockedOutAsync(user)
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? id) {
        if (id is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        await _userManager.DeleteAsync(user);
        return RedirectToPage("./Index");
    }
}