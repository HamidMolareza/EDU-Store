using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Products;

public class CreateModel : PageModel {
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CreateModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) {
        _context            = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [BindProperty] public ProductModel Product { get; set; } = new();
    public List<CategoryModel> Categories { get; set; }

    public class CategoryModel {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public class ProductModel {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "{0} ضروری است.")]
        [MinLength(3, ErrorMessage = "حداقل {1} کاراکتر وارد کنید.")]
        public string Name { get; set; } = default!;

        [Display(Name = "دسته‌بندی‌ها")] public List<int> Categories { get; set; } = new();

        [Display(Name = "قیمت (تومان)")]
        // [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        [Range(0, double.MaxValue, ErrorMessage = "{0} باید بین {1} تا {2} باشد.")]
        [Required(ErrorMessage = "{0} ضروری است.")]
        public decimal? Price { get; set; }

        [Display(Name = "تعداد")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} باید بین {1} تا {2} باشد.")]
        [Required(ErrorMessage = "{0} ضروری است.")]
        public int? StockQuantity { get; set; }

        [Display(Name = "وزن محصول (کیلوگرم)")]
        [Range(0, double.MaxValue, ErrorMessage = "{0} باید بین {1} تا {2} باشد.")]
        [Required(ErrorMessage = "{0} ضروری است.")]
        public double? ProductWeight { get; set; }

        [Required(ErrorMessage = "{0} ضروری است.")]
        [Display(Name = "عکس")]
        public IFormFile Image { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(80, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد.")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGet() {
        await LoadCategoriesAsync();
        return Page();
    }

    private async Task LoadCategoriesAsync() {
        Categories = await _context.Categories.AsNoTracking()
                         .Select(item => new CategoryModel {
                             Id   = item.Id,
                             Name = item.Name
                         }).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid) {
            await LoadCategoriesAsync();
            return Page();
        }

        if (Product.Image.Length < 100) {
            ModelState.AddModelError("Product.Image", "حجم عکس بیش از اندازه کم است!");
            await LoadCategoriesAsync();
            return Page();
        }

        if (!Product.Categories.Any()) {
            ModelState.AddModelError("Product.Categories", "حداقل یک دسته‌بندی مشخص کنید.");
            await LoadCategoriesAsync();
            return Page();
        }

        if (!await _context.AllEntitiesExistAsync(Product.Categories)) {
            ModelState.AddModelError(string.Empty,
                "دسته‌بندی‌ها معتبر نیستن. ممکنه دسته‌بندی‌ای حذف شده باشه. صفحه رو رفرش کنید و دوباره تلاش کنید.");
            await LoadCategoriesAsync();
            return Page();
        }

        // Generate a unique file name for the image
        var uniqueFileName = $"{Guid.NewGuid():N}_{Product.Image.FileName}";

        var imageUrl  = $"img/products/{uniqueFileName}";
        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);

        // Save the image to the server
        await using (var stream = new FileStream(imagePath, FileMode.Create))
            await Product.Image.CopyToAsync(stream);

        var product = new Product {
            Name          = Product.Name,
            Description   = Product.Description,
            Price         = (decimal)Product.Price!,
            StockQuantity = (int)Product.StockQuantity!,
            Image         = $"/{imageUrl}",
            ProductWeight = (double)Product.ProductWeight!,
        };

        var productCategories = Product.Categories
            .Select(categoryId => new ProductCategory { CategoryId = categoryId })
            .ToList();
        product.ProductCategories.AddRange(productCategories);

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        foreach (var productCategory in productCategories)
            productCategory.ProductId = product.Id;

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}