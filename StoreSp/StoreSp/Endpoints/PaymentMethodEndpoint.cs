using Microsoft.AspNetCore.Mvc;
using StoreSp.Services.checkout;
using System.Text.Json.Nodes;

namespace StoreSp.Endpoints;

public static class PaymentMethodEndpoint
{
    private static PayPalService _payPalService = new PayPalService();

    public static RouteGroupBuilder MapPaymentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/payment");
        group.MapPost("/paypal/create-order", ([FromBody] JsonObject data) =>
        {
            return _payPalService.CreateOrder(data?["amount"]?.ToString()!);
        });

        group.MapGet("/token", () =>
        {
            return _payPalService.GetPayPalAccessToken();
        });

        return group;
    }
}
