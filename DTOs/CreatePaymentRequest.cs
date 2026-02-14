namespace PayByLink.DTOs.Payments;

public record CreatePaymentRequest(
    decimal Amount,
    string Currency,
    string CustomerContact,
    string? Channel // "auto" | "whatsapp" | "sms" | "email"
);
