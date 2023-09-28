using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Store.Utilities;

public class PaginationModel<T> : PageModel {
    public int PageIndex { get; set; }
    public int PageLimit { get; set; }
    public int TotalItems { get; set; }
    public List<T> Items { get; set; } = new();
    public bool IsValidPage { get; set; }

    public async Task LoadItemsAsync(IQueryable<T> source, int? page, int? limit) {
        if (page < 1 || limit < 1) {
            IsValidPage = false;
            return;
        }

        PageIndex = page  ?? 1;
        PageLimit = limit ?? 10;

        TotalItems = await source.CountAsync();
        if (TotalItems == 0) {
            IsValidPage = true;
            return;
        }

        var totalPages = (int)Math.Ceiling(TotalItems / (double)PageLimit);
        if (PageIndex > totalPages) {
            IsValidPage = false;
            return;
        }

        Items = await source
                    .Skip((PageIndex - 1) * PageLimit)
                    .Take(PageLimit).ToListAsync();
        IsValidPage = true;
    }
}