namespace Shop.Models;

public class ProductCategory {
    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;
}