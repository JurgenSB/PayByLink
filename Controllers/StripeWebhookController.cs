using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using PayByLink.Domain.Enums;
using PayByLink.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace PayByLink.Controllers;

[ApiController]
[Route("webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public StripeWebhookController(IConfiguration config, AppDbContext db)
    {
        _config = config;
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _config["Stripe:WebhookSecret"]
            );
        }
        catch (Exception)
        {
            return BadRequest();
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            if (session != null)
            {
                var payment = await _db.PaymentRequests
                    .Where(p => p.StripeSessionId == session.Id)
                    .FirstOrDefaultAsync();

                if (payment != null)
                {
                    payment.Status = PaymentStatus.Paid;
                    payment.PaidAt = DateTime.UtcNow;

                    await _db.SaveChangesAsync();
                }
            }
        }

        return Ok();
    }
}
