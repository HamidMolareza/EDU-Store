using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Utilities;

namespace Store.Pages.Shop;

public class Index : PaginationModel<Index.Product> {
    private readonly ApplicationDbContext _context;

    public Index(ApplicationDbContext context) {
        _context = context;
    }

    public List<Category> Categories { get; set; } = new();

    public class Category {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int ProductCount { get; set; }
        public bool Active { get; set; }
    }

    public class Product {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public string Image { get; set; } = default!;
    }

    public async Task OnGetAsync(int? categoryId, int? p = 1, int? limit = 6) {
        categoryId ??= -1;
        Categories = new List<Category> {
            new() {
                Id           = -1,
                Name         = "همه",
                ProductCount = await _context.Products.CountAsync(),
                Active       = categoryId < 0
            }
        };
        Categories.AddRange(
            await _context.Categories.AsNoTracking()
                .Include(c => c.ProductCategories)
                .OrderBy(c => c.Id)
                .Select(c => new Category {
                    Id           = c.Id,
                    Name         = c.Name,
                    ProductCount = c.ProductCategories.Count(pc => pc.CategoryId == c.Id),
                    Active       = c.Id == categoryId
                }).Where(c => c.ProductCount > 0)
                .ToListAsync()
        );

        var query = _context.Products.AsNoTracking()
            .Where(product => product.StockQuantity > 0)
            .Include(product => product.ProductCategories).AsQueryable();

        if (categoryId > 0) {
            query = query
                .Where(product => product.ProductCategories.Any(pc => pc.CategoryId == categoryId));
        }

        var productQuery = query.OrderBy(product => product.Id)
            .Select(product => new Product {
                Id            = product.Id,
                Name          = product.Name,
                Description   = product.Description,
                Image         = product.Image,
                StockQuantity = product.StockQuantity,
                Price         = product.Price
            });

        await LoadItemsAsync(productQuery, p, limit);
    }
}