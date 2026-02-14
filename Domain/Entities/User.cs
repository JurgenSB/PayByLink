using System.ComponentModel.DataAnnotations;

namespace PayByLink.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(2)]
    public string Country { get; set; } = "DE";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
}
