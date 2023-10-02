using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Areas.Admin.Pages.Members;

public class DetailsModel : PageModel {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public DetailsModel(UserManager<IdentityUser> userManager, ApplicationDbContext context) {
        _userManager = userManager;
        _context     = context;
    }

    public User UserModel { get; set; }

    public class User {
        [Display(Name = "آی‌دی")] public string Id { get; set; } = default!;
        [Display(Name = "نام کاربری")] public string Username { get; set; } = default!;
        [Display(Name = "نقش‌ها")] public string Roles { get; set; } = default!;
        [Display(Name = "ایمیل")] public string? Email { get; set; }
        [Display(Name = "قفل شده؟")] public bool IsLockout { get; set; }
        [Display(Name = "تلفن")] public string? Phone { get; set; }

        [Display(Name = "تعداد محصولات خریداری شده")]
        public int ProductsBought { get; set; }
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
            IsLockout = await _userManager.IsLockedOutAsync(user),
            ProductsBought = await _context.Carts.Where(cart => cart.IsArchived && cart.UserId == id)
                                 .SumAsync(cart => cart.Quantity)
        };
        return Page();
    }
}