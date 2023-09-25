using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Utilities;

namespace Shop.Pages.Category;

public class Index : PaginationModel<Index.Product> {
    private readonly ApplicationDbContext _context;

    public Index(ApplicationDbContext context) {
        _context = context;
    }

    public string CategoryName { get; set; }

    public class Product {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int? id, int? p, int? limit) {
        if (id is null) return NotFound();

        var category = await _context.Categories.AsNoTracking()
                           .FirstOrDefaultAsync(category => category.Id == id);
        if (category is null) return NotFound();

        var query = _context.ProductCategories.AsNoTracking()
            .Include(pc => pc.Product)
            .Where(pc => pc.CategoryId == category.Id)
            .Select(pc => new Product {
                ProductId     = pc.Product.Id,
                ImageUrl      = pc.Product.Image,
                Name          = pc.Product.Name,
                Description   = pc.Product.Description,
                Price         = pc.Product.Price,
                StockQuantity = pc.Product.StockQuantity,
            });

        await LoadItemsAsync(query, p, limit ?? 8);

        CategoryName = category.Name;

        return Page();
    }
}