using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Areas.Admin.Pages;

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
        public int NumOfCategories { get; set; }
        public int NumOfProducts { get; set; }
        public int TotalSales { get; set; }
        public int TotalProductsInCart { get; set; }
    }

    public async Task OnGetAsync() {
        InfoModel.NumOfMessages   = await _context.Messages.CountAsync();
        InfoModel.NumOfMembers    = await _userManager.Users.CountAsync();
        InfoModel.NumOfCategories = await _context.Categories.CountAsync();
        InfoModel.NumOfProducts   = await _context.Products.CountAsync();
        InfoModel.TotalSales      = await _context.Carts.Where(cart => cart.IsArchived).SumAsync(cart => cart.Quantity);
        InfoModel.TotalProductsInCart =
            await _context.Carts.Where(cart => !cart.IsArchived).SumAsync(cart => cart.Quantity);
    }
}