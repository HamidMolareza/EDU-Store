using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Utilities;

namespace Store.Areas.Admin.Pages.Members;

public class IndexModel : PaginationModel<IndexModel.User> {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IndexModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    public class User {
        public string Id { get; set; } = default!;
        [Display(Name = "آی‌دی")] public string DisplayId { get; set; } = default!;
        [Display(Name = "نام کاربری")] public string Username { get; set; } = default!;
        [Display(Name = "نقش‌ها")] public string Roles { get; set; } = default!;
        public bool CanDelete { get; set; }
    }

    public async Task OnGetAsync(int? p, int? limit) {
        var query = _userManager.Users.AsNoTracking()
            .Select(user => new User {
                Id        = user.Id,
                DisplayId = user.Id,
                Username  = user.UserName!,
                Roles     = string.Join(", ", _userManager.GetRolesAsync(user).Result.ToArray()),
                CanDelete = user.Id != HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            });

        await LoadItemsAsync(query, p, limit ?? 5);

        foreach (var item in Items)
            item.DisplayId = Utility.ConvertGuidToViewModel(item.DisplayId) ?? "Invalid-Id";
    }
}