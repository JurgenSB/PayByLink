using PayByLink.Domain.Enum;

namespace PayByLink.Infrastructure.Messaging;

public class NotificationRouter
{
    private readonly TwilioMessagingService _twilio;

    public NotificationRouter(TwilioMessagingService twilio)
    {
        _twilio = twilio;
    }

    public async Task<List<NotificationSendResult>> SendPaymentLinkAsync(
        string contact,
        string paymentLink,
        string? channel = "auto")
    {
        channel = (channel ?? "auto").Trim().ToLowerInvariant();
        var results = new List<NotificationSendResult>();

        var msg = $"Olá! Aqui está seu link para pagamento: {paymentLink}";
        var trimmed = contact.Trim();

        if (channel == "auto")
        {
            if (ContactParser.IsPhoneE164(trimmed))
            {
                // 1) tenta WhatsApp
                try
                {
                    var sid = await _twilio.SendWhatsappAsync(trimmed, msg);
                    results.Add(new NotificationSendResult(
                        NotificationChannel.Whatsapp, NotificationStatus.Sent,
                        "Twilio", sid, trimmed, null));
                    return results;
                }
                catch (Exception ex)
                {
                    results.Add(new NotificationSendResult(
                        NotificationChannel.Whatsapp, NotificationStatus.Failed,
                        "Twilio", null, trimmed, ex.Message));
                }

                // 2) fallback SMS
                try
                {
                    var sid = await _twilio.SendSmsAsync(trimmed, msg);
                    results.Add(new NotificationSendResult(
                        NotificationChannel.Sms, NotificationStatus.Sent,
                        "Twilio", sid, trimmed, null));
                    return results;
                }
                catch (Exception ex)
                {
                    results.Add(new NotificationSendResult(
                        NotificationChannel.Sms, NotificationStatus.Failed,
                        "Twilio", null, trimmed, ex.Message));
                    return results; // ambas falharam
                }
            }

            // email (placeholder)
            if (ContactParser.IsEmail(trimmed))
            {
                results.Add(new NotificationSendResult(
                    NotificationChannel.Email, NotificationStatus.Skipped,
                    "EmailProvider", null, trimmed, "Email provider not configured yet"));
                return results;
            }

            throw new ArgumentException("CustomerContact inválido. Use telefone E.164 (+...) ou email.");
        }

        if (channel == "whatsapp")
        {
            if (!ContactParser.IsPhoneE164(trimmed)) throw new ArgumentException("WhatsApp requer telefone E.164 (+...)");
            try
            {
                var sid = await _twilio.SendWhatsappAsync(trimmed, msg);
                results.Add(new NotificationSendResult(NotificationChannel.Whatsapp, NotificationStatus.Sent, "Twilio", sid, trimmed, null));
            }
            catch (Exception ex)
            {
                results.Add(new NotificationSendResult(NotificationChannel.Whatsapp, NotificationStatus.Failed, "Twilio", null, trimmed, ex.Message));
            }
            return results;
        }

        if (channel == "sms")
        {
            if (!ContactParser.IsPhoneE164(trimmed)) throw new ArgumentException("SMS requer telefone E.164 (+...)");
            try
            {
                var sid = await _twilio.SendSmsAsync(trimmed, msg);
                results.Add(new NotificationSendResult(NotificationChannel.Sms, NotificationStatus.Sent, "Twilio", sid, trimmed, null));
            }
            catch (Exception ex)
            {
                results.Add(new NotificationSendResult(NotificationChannel.Sms, NotificationStatus.Failed, "Twilio", null, trimmed, ex.Message));
            }
            return results;
        }

        if (channel == "email")
        {
            if (!ContactParser.IsEmail(trimmed)) throw new ArgumentException("Email inválido");
            results.Add(new NotificationSendResult(
                NotificationChannel.Email, NotificationStatus.Skipped,
                "EmailProvider", null, trimmed, "Email provider not configured yet"));
            return results;
        }

        throw new ArgumentException("Channel inválido. Use: auto | whatsapp | sms | email.");
    }
}
