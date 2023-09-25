using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Utilities;

namespace Shop.Areas.Admin.Pages.Categories;

public class IndexModel : PaginationModel<IndexModel.CategoryModel> {
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context) {
        _context = context;
    }

    public class CategoryModel {
        public int Id { get; set; }

        [Display(Name = "نام دسته")] public string Name { get; set; }
    }

    public async Task OnGetAsync(int? p, int? limit) {
        var query = _context.Categories.AsNoTracking()
            .OrderByDescending(category => category.Id)
            .Select(item => new CategoryModel {
                Id   = item.Id,
                Name = item.Name
            });
        await LoadItemsAsync(query, p, limit ?? 10);
    }
}