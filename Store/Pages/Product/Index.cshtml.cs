using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Pages.Product;

public class Index : PageModel {
    private readonly ApplicationDbContext _context;

    public Index(ApplicationDbContext context) {
        _context = context;
    }

    public Product ProductModel { get; set; }

    public class Product {
        public int Id { get; set; }
        [Display(Name = "نام")] public string Name { get; set; } = default!;
        [Display(Name = "توضیحات")] public string? Description { get; set; }
        [Display(Name = "دسته‌بندی‌ها")] public List<Category> Categories { get; set; } = new();
        [Display(Name = "قیمت (تومان)")] public decimal Price { get; set; }
        [Display(Name = "تعداد باقی‌مانده")] public int StockQuantity { get; set; }
        [Display(Name = "عکس")] public string ImageUrl { get; set; } = default!;
        [Display(Name = "وزن (کیلوگرم)")] public double ProductWeight { get; set; }
        public bool IsInCart { get; set; }
    }

    public class Category {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public async Task<IActionResult> OnGetAsync(int? id) {
        if (id is null) return NotFound();

        await LoadDataAsync((int)id);
        if (ProductModel is null)
            return NotFound();

        return Page();
    }

    private async Task LoadDataAsync(int id) {
        var product = await _context.Products.AsNoTracking()
                          .FirstOrDefaultAsync(product => product.Id == id);
        if (product is null) {
            ProductModel = null!;
            return;
        }

        var categories = await _context.ProductCategories.AsNoTracking()
                             .Include(pc => pc.Category)
                             .Where(pc => pc.Product.Id == product.Id)
                             .Select(pc => new Category {
                                 Id   = pc.Category.Id,
                                 Name = pc.Category.Name
                             }).ToListAsync();

        ProductModel = new Product {
            Id            = product.Id,
            Name          = product.Name,
            Description   = product.Description,
            Price         = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl      = product.Image,
            ProductWeight = product.ProductWeight,
            Categories    = categories,
        };

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null) {
            ProductModel.IsInCart =
                await _context.Carts.AnyAsync(cart =>
                    !cart.IsArchived && cart.UserId == userId && cart.ProductId == id);
        }
    }

    public async Task<IActionResult> OnPostAddToCart(int? productId) {
        if (productId is null) return RedirectToPage("./Index");
        if (HttpContext.User.Identity?.IsAuthenticated == false)
            return Redirect($"/Identity/Account/Login?ReturnUrl={Url.Page("./Index")}");

        var product = await _context.Products.AsNoTracking()
                          .FirstOrDefaultAsync(product => product.Id == productId);
        if (product is null) return NotFound();
        if (product.StockQuantity < 1) return RedirectToPage("./Index");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            throw new Exception($"Expected {nameof(userId)} not to be null.");

        var cart = await _context.Carts.AsNoTracking()
                       .FirstOrDefaultAsync(cart =>
                           cart.UserId == userId && cart.ProductId == productId && !cart.IsArchived);
        if (cart is not null) return RedirectToPage("./Index");

        cart = new Models.Cart {
            ProductId = (int)productId,
            UserId    = userId,
            Quantity  = 1
        };
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        await LoadDataAsync((int)productId);
        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostRemoveCart(int? productId) {
        if (productId is null) return NotFound();
        if (HttpContext.User.Identity?.IsAuthenticated == false)
            return Redirect($"/Identity/Account/Login?ReturnUrl={Url.Page("./Index")}");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            throw new Exception($"Expected {nameof(userId)} not to be null.");

        var cart = await _context.Carts
                       .AsNoTracking()
                       .FirstOrDefaultAsync(cart =>
                           cart.UserId == userId && cart.ProductId == productId && !cart.IsArchived);
        if (cart is null) return RedirectToPage("./Index");

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();

        await LoadDataAsync((int)productId);
        return RedirectToPage("./Index");
    }
}