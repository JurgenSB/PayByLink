using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PayByLink.Domain.Enum;
using PayByLink.Domain.Entities;

namespace PayByLink.Domain.Entities;

public class NotificationAttempt
{
    public Guid Id { get; set; }

    [Required]
    public Guid PaymentRequestId { get; set; }

    [ForeignKey(nameof(PaymentRequestId))]
    public PaymentRequest PaymentRequest { get; set; } = null!;

    [Required]
    public NotificationChannel Channel { get; set; }

    [Required]
    public NotificationStatus Status { get; set; }

    [MaxLength(50)]
    public string? Provider { get; set; } // "Twilio", "SendGrid", etc.

    [MaxLength(120)]
    public string? ProviderMessageId { get; set; } // Twilio SID

    [MaxLength(200)]
    public string? To { get; set; } // telefone/email (pode mascarar no frontend)

    [MaxLength(500)]
    public string? Error { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
