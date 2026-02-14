using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PayByLink.Infrastructure.Messaging;

public class TwilioMessagingService
{
    private readonly IConfiguration _config;

    public TwilioMessagingService(IConfiguration config)
    {
        _config = config;

        TwilioClient.Init(
            _config["Twilio:AccountSid"],
            _config["Twilio:AuthToken"]
        );
    }

    public async Task<string> SendSmsAsync(string toE164, string body)
    {
        var from = _config["Twilio:SmsFrom"];
        var msg = await MessageResource.CreateAsync(
            to: toE164,
            from: from,
            body: body
        );
        return msg.Sid;
    }

    public async Task<string> SendWhatsappAsync(string toE164, string body)
    {
        var from = _config["Twilio:WhatsappFrom"]; // ex: "whatsapp:+14155238886"
        var msg = await MessageResource.CreateAsync(
            to: $"whatsapp:{toE164}",
            from: from,
            body: body
        );
        return msg.Sid;
    }
}
