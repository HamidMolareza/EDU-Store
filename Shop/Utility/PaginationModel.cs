using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Shop.Utility;

public class PaginationModel<T> : PageModel {
    public int PageIndex { get; set; }
    public int PageLimit { get; set; }
    public int TotalItems { get; set; }
    public List<T> Items { get; set; } = new();
    public bool ValidPage { get; set; }

    public async Task LoadItemsAsync(IQueryable<T> source, int? page, int? limit) {
        if (page < 1 || limit < 1) {
            ValidPage = false;
            return;
        }

        PageIndex  = page  ?? 1;
        PageLimit  = limit ?? 10;
        TotalItems = await source.CountAsync();
        var totalPages = (int)Math.Ceiling(TotalItems / (double)PageLimit);
        if (PageIndex > totalPages) {
            ValidPage = false;
            return;
        }

        Items = await source
                    .Skip((PageIndex - 1) * PageLimit)
                    .Take(PageLimit).ToListAsync();
        ValidPage = true;
    }
}