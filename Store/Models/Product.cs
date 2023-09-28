using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models;

public class Product {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [MinLength(3)] public string Name { get; set; } = default!; // Product name
    [MaxLength(80)] public string? Description { get; set; }               // Product description
    public List<ProductCategory> ProductCategories { get; set; } = new();  // Product category
    public decimal Price { get; set; }                                     // Product price
    [Range(0, int.MaxValue)] public int StockQuantity { get; set; }        // Current stock quantity
    public string Image { get; set; }
    [Range(0, double.MaxValue)] public double ProductWeight { get; set; } // Product weight
}