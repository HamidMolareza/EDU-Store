using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Pages.Cart;

[Authorize]
public class Index : PageModel {
    private readonly ApplicationDbContext _context;

    public Index(ApplicationDbContext context) {
        _context = context;
    }

    public IndexModel PageModel { get; set; }

    public class IndexModel {
        public List<Cart> Carts { get; set; } = new();

        [Display(Name = "جمع سبد خرید")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalPrice { get; set; }
    }

    public class Cart {
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = new();

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalPrice { get; set; }
    }

    public class Product {
        public int Id { get; set; }
        [Display(Name = "نام محصول")] public string Name { get; set; } = default!;
        [Display(Name = "توضیحات")] public string? Description { get; set; }

        [Display(Name = "قیمت (تومان)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public string Image { get; set; } = default!;
    }

    public async Task OnGetAsync() {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync() {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) throw new Exception($"Expected {nameof(userId)} be not null.");

        var carts = await _context.Carts.AsNoTracking()
                        .OrderBy(cart => cart.Id)
                        .Where(cart => cart.UserId == userId && !cart.IsArchived)
                        .Include(cart => cart.Product)
                        .Select(cart => new Cart {
                            CartId   = cart.Id,
                            Quantity = cart.Quantity,
                            Product = new Product {
                                Id            = cart.Product.Id,
                                Name          = cart.Product.Name,
                                Description   = cart.Product.Description,
                                Price         = cart.Product.Price,
                                StockQuantity = cart.Product.StockQuantity,
                                Image         = cart.Product.Image,
                            },
                            TotalPrice = cart.Quantity * cart.Product.Price
                        }).ToListAsync();

        PageModel = new IndexModel {
            Carts      = carts,
            TotalPrice = carts.Sum(cart => cart.Quantity * cart.Product.Price)
        };

        for (var i = 0; i < carts.Count; i++) {
            var cart = carts[i];
            if (cart.Quantity > cart.Product.StockQuantity)
                ModelState.AddModelError($"PageModel.Carts[{i}].Quantity",
                    cart.Product.StockQuantity < 1
                        ? "متاسفانه این مورد تمام شده است."
                        : $"فقط {cart.Product.StockQuantity} عدد موجود است.");
        }
    }

    public async Task<IActionResult> OnPost() {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) throw new Exception($"Expected {nameof(userId)} be not null.");

        var hasError = _context.Carts
            .Include(cart => cart.Product)
            .Where(cart => !cart.IsArchived && cart.UserId == userId)
            .Any(cart => cart.Quantity > cart.Product.StockQuantity);
        if (hasError) {
            await LoadDataAsync();
            return Page();
        }

        var carts = await _context.Carts
                        .Where(cart => !cart.IsArchived && cart.UserId == userId)
                        .Include(cart => cart.Product)
                        .ToListAsync();

        foreach (var cart in carts) {
            cart.IsArchived            =  true;
            cart.Product.StockQuantity -= cart.Quantity;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Success");
    }

    public async Task<IActionResult> OnPostDeleteCart(int? cartId) {
        if (cartId is null) return RedirectToPage("./Index");

        var exists = await _context.Carts.AnyAsync(cart => !cart.IsArchived && cart.Id == cartId);
        if (!exists) return RedirectToPage("./Index");

        _context.Carts.Remove(new Models.Cart {
            Id = (int)cartId
        });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostIncreaseCart(int? cartId) {
        if (cartId is null) return RedirectToPage("./Index");

        var cart = await _context.Carts
                       .Include(cart => cart.Product)
                       .FirstOrDefaultAsync(cart => cart.Id == cartId && !cart.IsArchived);
        if (cart is null) return RedirectToPage("./Index");

        cart.Quantity++;
        if (cart.Quantity > cart.Product.StockQuantity) {
            return RedirectToPage("./Index");
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostDecreaseCart(int? cartId) {
        if (cartId is null) return RedirectToPage("./Index");

        var cart = await _context.Carts
                       .Include(cart => cart.Product)
                       .FirstOrDefaultAsync(cart => cart.Id == cartId && !cart.IsArchived);
        if (cart is null) return RedirectToPage("./Index");

        cart.Quantity--;
        if (cart.Quantity < 1)
            _context.Carts.Remove(cart);

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}