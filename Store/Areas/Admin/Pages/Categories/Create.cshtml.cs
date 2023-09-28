using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Categories;

public class CreateModel : PageModel {
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context) {
        _context = context;
    }

    public IActionResult OnGet() {
        return Page();
    }

    [BindProperty] public CategoryModel Category { get; set; } = default!;

    public class CategoryModel {
        [Display(Name = "نام دسته")]
        [Required(ErrorMessage = "{0} ضروری است.")]
        [MinLength(3, ErrorMessage = "{0} باید حداقل {1} کاراکتر باشد.")]
        public string Name { get; set; }
    }


    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid)
            return Page();

        _context.Categories.Add(new Category {
            Name = Category.Name
        });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}