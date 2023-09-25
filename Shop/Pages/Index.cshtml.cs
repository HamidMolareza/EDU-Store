using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Pages;

public class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context) {
        _logger  = logger;
        _context = context;
    }

    public List<Category> Categories { get; set; }

    public class Category {
        public int CategoryId { get; set; }
        public string Title { get; set; } = default!;
        public List<Product> Products { get; set; } = new();
    }

    public class Product {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public async Task OnGet() {
        Categories = await _context.Categories.AsNoTracking()
                         .Include(category => category.ProductCategories)
                         .ThenInclude(pc => pc.Product)
                         .Select(category => new Category {
                             CategoryId = category.Id,
                             Title      = category.Name,
                             Products = category.ProductCategories
                                 .Where(pc => pc.Product.StockQuantity > 0)
                                 .Select(pc => new Product {
                                     ProductId     = pc.Product.Id,
                                     Name          = pc.Product.Name,
                                     Description   = pc.Product.Description,
                                     ImageUrl      = pc.Product.Image,
                                     StockQuantity = pc.Product.StockQuantity,
                                     Price         = pc.Product.Price
                                 }).Take(4).ToList()
                         }).Where(category => category.Products.Count > 0)
                         .ToListAsync();
    }
}