namespace Shop.Models;

public class Pagination {
    public int PageIndex { get; set; }
    public int PageLimit { get; set; }
    public int TotalItems { get; set; }
    public bool IsValidPage { get; set; }
    public string MainPageLink { get; set; } = default!;
}