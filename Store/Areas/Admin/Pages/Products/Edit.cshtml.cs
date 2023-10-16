using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;
using Store.Utilities;

namespace Store.Areas.Admin.Pages.Products;

public class EditModel : PageModel {
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EditModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) {
        _context            = context;
        _webHostEnvironment = webHostEnvironment;
    }

    private string? _returnUrl;

    public string? ReturnUrl {
        get => _returnUrl;
        set => _returnUrl = Utility.SafeReturnUrl(value, Url);
    }

    [BindProperty] public ProductModel Product { get; set; } = default!;
    public List<CategoryModel> Categories { get; set; }

    public class CategoryModel {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public class ProductModel {
        public int Id { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "{0} ضروری است.")]
        [MinLength(3, ErrorMessage = "حداقل {1} کاراکتر وارد کنید.")]
        public string Name { get; set; } = default!;

        [Display(Name = "دسته‌بندی‌ها")] public List<int> Categories { get; set; } = new();

        [Display(Name = "قیمت (تومان)")]
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

        [Display(Name = "عکس")] public IFormFile? Image { get; set; }
        public string ImageUrl { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(80, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد.")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGet(int? id, string? returnUrl) {
        if (id is null)
            return NotFound();

        var product = await _context.Products.AsNoTracking()
                          .Include(product => product.ProductCategories)
                          .FirstOrDefaultAsync(product => product.Id == id);
        if (product is null)
            return NotFound();

        Product = new ProductModel {
            Id            = product.Id,
            Description   = product.Description,
            Name          = product.Name,
            Price         = product.Price,
            StockQuantity = product.StockQuantity,
            ProductWeight = product.ProductWeight,
            Categories    = product.ProductCategories.Select(pc => pc.CategoryId).ToList(),
            ImageUrl      = product.Image
        };
        await LoadCategoriesAsync();

        ReturnUrl = returnUrl;
        return Page();
    }

    private async Task LoadCategoriesAsync() {
        Categories = await _context.Categories.AsNoTracking()
                         .Select(item => new CategoryModel {
                             Id   = item.Id,
                             Name = item.Name
                         }).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl) {
        if (!ModelState.IsValid) {
            await LoadCategoriesAsync();
            return Page();
        }

        var dbProduct = await _context.Products
                            .Include(product => product.ProductCategories)
                            .FirstOrDefaultAsync(product => product.Id == Product.Id);
        if (dbProduct is null) {
            await LoadCategoriesAsync();
            ModelState.AddModelError(string.Empty, "این آیتم حذف شده!");
            return Page();
        }

        var isNewImageUploaded = Product.Image is not null;
        if (isNewImageUploaded && Product.Image!.Length < 100) {
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

        if (isNewImageUploaded) {
            // Generate a unique file name for the image
            var uniqueFileName = $"{Guid.NewGuid():N}_{Product.Image!.FileName}";

            var imageUrl  = $"img/products/{uniqueFileName}";
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);

            // Save the image to the server
            await using var stream = new FileStream(imagePath, FileMode.Create);
            await Product.Image.CopyToAsync(stream);

            //Remove old image
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, dbProduct.Image[1..]);
            System.IO.File.Delete(oldImagePath);

            //Set new image path
            dbProduct.Image = $"/{imageUrl}";
        }

        dbProduct.Name          = Product.Name;
        dbProduct.Description   = Product.Description;
        dbProduct.Price         = (decimal)Product.Price!;
        dbProduct.StockQuantity = (int)Product.StockQuantity!;
        dbProduct.ProductWeight = (double)Product.ProductWeight!;

        var productCategories = Product.Categories
            .Select(categoryId => new ProductCategory { CategoryId = categoryId, ProductId = dbProduct.Id })
            .ToList();
        dbProduct.ProductCategories = productCategories;

        await _context.SaveChangesAsync();
        return Utility.RedirectToReturnUrl(ReturnUrl, RedirectToPage("./Index"));
    }
}