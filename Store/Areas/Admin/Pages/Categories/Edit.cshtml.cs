using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Categories;

public class EditModel : PageModel {
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context) {
        _context = context;
    }

    [BindProperty] public CategoryModel Category { get; set; } = default!;

    public class CategoryModel {
        public int Id { get; set; }

        [Display(Name = "نام دسته")]
        [Required(ErrorMessage = "پر کردن {0} ضروری است.")]
        public string Name { get; set; }
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

    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid)
            return Page();

        var editedCategory = new Category {
            Id   = Category.Id,
            Name = Category.Name
        };
        _context.Attach(editedCategory).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex) {
            var exceptionEntry = ex.Entries.Single();
            var databaseEntry  = await exceptionEntry.GetDatabaseValuesAsync();
            if (databaseEntry is null) {
                ModelState.AddModelError(string.Empty, "این آیتم حذف شده است!");
                return Page();
            }

            throw;
        }

        return RedirectToPage("./Index");
    }
}