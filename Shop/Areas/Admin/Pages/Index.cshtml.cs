using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Areas.Admin.Pages;

public class IndexModel : PageModel {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
        _context     = context;
        _userManager = userManager;
    }

    public Info InfoModel { get; set; } = new();

    public class Info {
        public int NumOfMessages { get; set; }
        public int NumOfMembers { get; set; }
    }

    public async Task OnGetAsync() {
        InfoModel.NumOfMessages = await _context.Messages.CountAsync();
        InfoModel.NumOfMembers  = await _userManager.Users.CountAsync();
    }
}