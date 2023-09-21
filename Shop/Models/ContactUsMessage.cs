using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models;

public class ContactUsMessage {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Name { get; set; }

    [EmailAddress] public string? Email { get; set; }

    public string? Title { get; set; }

    [MinLength(10)] public string Message { get; set; } = default!;
}