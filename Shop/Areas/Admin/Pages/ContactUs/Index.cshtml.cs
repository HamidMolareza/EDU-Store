using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Areas.Admin.Pages.ContactUs;

public class IndexModel : PageModel {
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context) {
        _context = context;
    }

    public IList<MessageModel> Messages { get; set; } = default!;

    public class MessageModel {
        [Display(Name = "آی‌دی")] public int Id { get; set; }
        [Display(Name = "نام")] public string? Name { get; set; }

        [Display(Name = "ایمیل")] public string? Email { get; set; }

        [Display(Name = "عنوان")] public string? Title { get; set; }

        [Display(Name = "پیام")] public string Message { get; set; } = default!;
    }

    public async Task OnGetAsync() {
        Messages = (await _context.Messages.OrderByDescending(item => item.Id)
                        .AsNoTracking()
                        .ToListAsync())
            .Select(item => new MessageModel {
                Id      = item.Id,
                Title   = item.Title,
                Name    = item.Name,
                Email   = item.Email,
                Message = item.Message.Length > 30 ? $"{item.Message[..30]}..." : item.Message
            }).ToList();
    }

    public async Task<IActionResult> OnPostAsync(int? id) {
        if (id is null)
            return NotFound();

        _context.Messages.Remove(new ContactUsMessage { Id = (int)id });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}