namespace PayByLink.DTOs.Payments;

public record CreatePaymentResponse(
    Guid PaymentId,
    string PaymentLink
);
