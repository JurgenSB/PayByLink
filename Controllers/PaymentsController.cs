using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using PayByLink.Domain.Entities;
using PayByLink.Domain.Enums;
using PayByLink.DTOs.Payments;
using PayByLink.Infrastructure.Persistence;
using PayByLink.Infrastructure.Messaging;

namespace PayByLink.Controllers;

[ApiController]
[Route("payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TwilioMessagingService _twilio;
    private readonly NotificationRouter _notify;
    public PaymentsController(AppDbContext db,NotificationRouter notify, TwilioMessagingService twilio)
    {
        _db = db;
        _notify = notify;
        _twilio = twilio;
    }

    [HttpPost("create")]
    public async Task<ActionResult<CreatePaymentResponse>> Create(
        CreatePaymentRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest("Invalid amount");

        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

        var session = await CreateStripeSession(request);

        var payment = new PaymentRequest
        {
            UserId = userId,
            Amount = request.Amount,
            Currency = request.Currency.ToUpper(),
            CustomerContact = request.CustomerContact,
            PaymentLink = session.Url,
            StripeSessionId = session.Id,
            Status = PaymentStatus.Pending
        };

        _db.PaymentRequests.Add(payment);

        var sendResults = await _notify.SendPaymentLinkAsync(
            request.CustomerContact,
            session.Url,
            request.Channel
        );

        foreach (var r in sendResults)
        {
            _db.NotificationAttempts.Add(new NotificationAttempt
            {
                PaymentRequestId = payment.Id,
                Channel = r.Channel,
                Status = r.Status,
                Provider = r.Provider,
                ProviderMessageId = r.ProviderMessageId,
                To = r.To,
                Error = r.Error
            });
        }

        await _db.SaveChangesAsync();
        return new CreatePaymentResponse(payment.Id, session.Url);
    }

    private static async Task<Session> CreateStripeSession(
        CreatePaymentRequest request)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency.ToLower(),
                        UnitAmount = (long)(request.Amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Payment Request"
                        }
                    }
                }
            },
            SuccessUrl = "https://yourapp.com/success",
            CancelUrl = "https://yourapp.com/cancel"
        };

        var service = new SessionService();
        return await service.CreateAsync(options);
    }
}
