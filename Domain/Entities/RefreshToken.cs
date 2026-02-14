using System.ComponentModel.DataAnnotations;

namespace PayByLink.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
