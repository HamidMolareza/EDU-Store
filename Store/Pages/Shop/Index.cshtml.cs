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

    public async Task OnGetAsync(int? p = 1, int? limit = 6) {
        Categories = new List<Category> {
            new() {
                Id           = -1,
                Name         = "همه",
                ProductCount = await _context.Categories.CountAsync()
            }
        };
        Categories.AddRange(
            await _context.Categories.AsNoTracking()
                .Include(c => c.ProductCategories)
                .OrderBy(c => c.Id)
                .Select(c => new Category {
                    Id           = c.Id,
                    Name         = c.Name,
                    ProductCount = c.ProductCategories.Count(pc => pc.CategoryId == c.Id)
                }).Where(c => c.ProductCount > 0)
                .ToListAsync()
        );

        var productsQuery = _context.Products.AsNoTracking()
            .OrderBy(product => product.Id)
            .Select(product => new Product {
                Id            = product.Id,
                Name          = product.Name,
                Description   = product.Description,
                Image         = product.Image,
                StockQuantity = product.StockQuantity,
                Price         = product.Price
            });
        await LoadItemsAsync(productsQuery, p, limit);
    }
}