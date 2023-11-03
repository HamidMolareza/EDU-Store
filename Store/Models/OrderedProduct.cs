using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Store.Models;

public class OrderedProduct {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Image { get; set; } = default!;

    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public string UserId { get; set; } = default!;
    public IdentityUser User { get; set; } = default!;
}