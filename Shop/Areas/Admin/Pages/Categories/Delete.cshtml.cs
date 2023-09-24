using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Areas.Admin.Pages.Categories;

public class DeleteModel : PageModel {
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context) {
        _context = context;
    }

    public CategoryModel Category { get; set; } = default!;

    public class CategoryModel {
        public int Id { get; set; }

        [Display(Name = "نام دسته")] public string Name { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int? id) {
        if (id == null)
            return NotFound();

        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound();

        Category = new CategoryModel {
            Id   = category.Id,
            Name = category.Name
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id) {
        if (id == null)
            return NotFound();

        var category = await _context.Categories.FindAsync(id);

        if (category != null) {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}