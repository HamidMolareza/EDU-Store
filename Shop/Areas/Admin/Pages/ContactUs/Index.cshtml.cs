using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Utility;

namespace Shop.Areas.Admin.Pages.ContactUs;

public class IndexModel : PaginationModel<IndexModel.MessageModel> {
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context) {
        _context = context;
    }
    public class MessageModel {
        [Display(Name = "آی‌دی")] public int Id { get; set; }
        [Display(Name = "نام")] public string? Name { get; set; }

        [Display(Name = "ایمیل")] public string? Email { get; set; }

        [Display(Name = "عنوان")] public string? Title { get; set; }

        [Display(Name = "پیام")] public string Message { get; set; } = default!;
    }

    public async Task OnGetAsync(int? p, int? limit) {
        var query = _context.Messages
            .OrderByDescending(item => item.Id)
            .AsNoTracking()
            .Select(item => new MessageModel {
                Id      = item.Id,
                Title   = item.Title,
                Name    = item.Name,
                Email   = item.Email,
                Message = item.Message.Length > 30 ? $"{item.Message.Substring(0, 30)}..." : item.Message
            });
        await LoadItemsAsync(query, p, limit ?? 5);
    }

    public async Task<IActionResult> OnPostAsync(int? id) {
        if (id is null)
            return NotFound();

        _context.Messages.Remove(new ContactUsMessage { Id = (int)id });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}