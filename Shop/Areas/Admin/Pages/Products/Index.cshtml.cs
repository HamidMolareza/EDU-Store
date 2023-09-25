using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Utilities;

namespace Shop.Areas.Admin.Pages.Products;

public class IndexModel : PaginationModel<IndexModel.ProductModel> {
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context) {
        _context = context;
    }

    public class ProductModel {
        public int Id { get; set; }
        [Display(Name = "عکس")] public string ImageUrl { get; set; } = default!;
        [Display(Name = "نام")] public string Name { get; set; } = default!;
        [Display(Name = "دسته‌بندی‌ها")] public string Categories { get; set; } = default!;

        [Display(Name = "قیمت (تومان)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [Display(Name = "تعداد")] public int StockQuantity { get; set; }
        [Display(Name = "ویژه؟")] public bool IsFeatured { get; set; }
    }

    public async Task OnGetAsync(int? p, int? limit) {
        var query = _context.Products.AsNoTracking()
            .OrderByDescending(product => product.Id)
            .Include(product => product.ProductCategories)
            .ThenInclude(productCategory => productCategory.Category)
            .Select(item => new ProductModel {
                Id            = item.Id,
                ImageUrl      = item.Image,
                Name          = item.Name,
                Categories    = string.Join(", ", item.ProductCategories.Select(pc => pc.Category.Name)),
                Price         = item.Price,
                StockQuantity = item.StockQuantity,
                IsFeatured    = item.IsFeatured
            });
        await LoadItemsAsync(query, p, limit ?? 5);
    }
}