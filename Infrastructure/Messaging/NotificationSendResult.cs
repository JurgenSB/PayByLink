using PayByLink.Domain.Enum;

namespace PayByLink.Infrastructure.Messaging;

public record NotificationSendResult(
    NotificationChannel Channel,
    NotificationStatus Status,
    string? Provider,
    string? ProviderMessageId,
    string To,
    string? Error
);
