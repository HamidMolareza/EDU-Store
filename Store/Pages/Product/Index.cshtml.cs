using System.ComponentModel.DataAnnotations;
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

    public class Product {
        public int Id { get; set; }
        [Display(Name = "نام")] public string Name { get; set; } = default!;
        [Display(Name = "توضیحات")] public string? Description { get; set; }
        [Display(Name = "دسته‌بندی‌ها")] public List<Category> Categories { get; set; } = new();
        [Display(Name = "قیمت (تومان)")] public decimal Price { get; set; }
        [Display(Name = "تعداد باقی‌مانده")] public int StockQuantity { get; set; }
        [Display(Name = "عکس")] public string ImageUrl { get; set; } = default!;
        [Display(Name = "وزن (کیلوگرم)")] public double ProductWeight { get; set; }
    }

    public class Category {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public Product ProductModel { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id) {
        if (id is null) return NotFound();

        var product = await _context.Products.AsNoTracking()
                          .FirstOrDefaultAsync(product => product.Id == id);
        if (product is null) return NotFound();

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
            Categories    = categories
        };
        return Page();
    }
}