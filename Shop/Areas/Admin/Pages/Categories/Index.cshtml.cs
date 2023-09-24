using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Areas.Admin.Pages.Categories;

public class IndexModel : PageModel {
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context) {
        _context = context;
    }

    public IList<CategoryModel> Category { get; set; } = default!;

    public class CategoryModel {
        public int Id { get; set; }

        [Display(Name = "نام دسته")] public string Name { get; set; }
    }

    public async Task OnGetAsync() {
        Category = await _context.Categories.AsNoTracking()
                       .Select(item => new CategoryModel {
                           Id   = item.Id,
                           Name = item.Name
                       }).ToListAsync();
    }
}