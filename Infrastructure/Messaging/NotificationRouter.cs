namespace PayByLink.Api.Infrastructure.Messaging;

public class NotificationRouter
{
    private readonly TwilioMessagingService _twilio;

    public NotificationRouter(TwilioMessagingService twilio)
    {
        _twilio = twilio;
    }

    public async Task SendPaymentLinkAsync(string contact, string paymentLink, string? channel = "auto")
    {
        channel = (channel ?? "auto").Trim().ToLowerInvariant();

        var msg = $"Olá! Aqui está seu link para pagamento: {paymentLink}";

        if (channel == "auto")
        {
            if (ContactParser.IsPhoneE164(contact))
            {
                // tenta WhatsApp e cai pra SMS se der erro
                try
                {
                    await _twilio.SendWhatsappAsync(contact, msg);
                    return;
                }
                catch
                {
                    await _twilio.SendSmsAsync(contact, msg);
                    return;
                }
            }

            // se não for telefone, tenta email (placeholder)
            if (ContactParser.IsEmail(contact))
            {
                // TODO: integrar provider de email (SendGrid/SMTP)
                // Por enquanto: não falha o fluxo de cobrança
                return;
            }

            // contato inválido
            throw new ArgumentException("CustomerContact inválido. Use telefone E.164 (+...) ou email.");
        }

        if (channel == "whatsapp")
        {
            if (!ContactParser.IsPhoneE164(contact)) throw new ArgumentException("WhatsApp requer telefone E.164 (+...)");
            await _twilio.SendWhatsappAsync(contact, msg);
            return;
        }

        if (channel == "sms")
        {
            if (!ContactParser.IsPhoneE164(contact)) throw new ArgumentException("SMS requer telefone E.164 (+...)");
            await _twilio.SendSmsAsync(contact, msg);
            return;
        }

        if (channel == "email")
        {
            if (!ContactParser.IsEmail(contact)) throw new ArgumentException("Email inválido");
            // TODO: integrar provider de email (SendGrid/SMTP)
            return;
        }

        throw new ArgumentException("Channel inválido. Use: auto | whatsapp | sms | email.");
    }
}
