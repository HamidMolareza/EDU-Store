using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Store.Data;
using Store.Models;

namespace Store.Pages.ContactUs;

public class Index : PageModel {
    private readonly ApplicationDbContext _context;

    public Index(ApplicationDbContext context) {
        _context = context;
    }

    [BindProperty] public InputModel Input { get; set; }


    public class InputModel {
        [Display(Name = "نام")] public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string? Email { get; set; }

        [Display(Name = "عنوان")] public string? Title { get; set; }

        [MinLength(10, ErrorMessage = "حداقل {1} کاراکتر واراد کنید.")]
        [Display(Name = "پیام")]
        [Required(ErrorMessage = "پر کردن {0} ضروری است.")]
        public string Message { get; set; } = default!;
    }

    public void OnGet() {
    }

    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid) {
            return Page();
        }

        _context.Messages.Add(new ContactUsMessage {
            Name    = Input.Name,
            Email   = Input.Email,
            Title   = Input.Title,
            Message = Input.Message,
        });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Success");
    }
}