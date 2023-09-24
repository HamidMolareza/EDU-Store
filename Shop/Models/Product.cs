using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models;

public class Product {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }                             // Product name
    public string Description { get; set; }                      // Product description
    public List<ProductCategory> Category { get; set; } = new(); // Product category
    public decimal Price { get; set; }                           // Product price
    public int StockQuantity { get; set; }                       // Current stock quantity
    public string Image { get; set; }
    public double ProductWeight { get; set; } // Product weight
    public bool IsAvailable { get; set; }     // Product availability
    public bool IsFeatured { get; set; }      // Flag for featured products
}