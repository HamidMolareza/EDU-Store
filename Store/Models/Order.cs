using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Store.Models;

public class Order {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public List<OrderedProduct> Products { get; set; } = new();
    public int Status { get; set; } = OrderStatus.Processing;

    public string UserId { get; set; } = default!;
    public IdentityUser User { get; set; } = default!;
}