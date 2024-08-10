namespace StoreSp.Services.checkout;

using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

public class PayPalService
{
    private string PaypalClientId { get; set; } = "";
    private string PaypalSecret { get; set; } = "";
    private string PaypalUrl { get; set; } = "";

    public PayPalService()
    {
        PaypalClientId = "AWTvt6vimYcopjpCVZvQK1yQu_MF1DyYr4A253i9JUAQgf7xODeedoGJbkDCcJsRoG9kapx5C8LR0xfj";
        PaypalSecret = "EAzY-Bjq5IZjGVx-HjnGY1TpN30pwvx0ikWw9Cb5xdY-v6kEzcW8kLlmEXM7h6z0IyOMfTFeXWkeqWU1";
        PaypalUrl = "https://api-m.sandbox.paypal.com";
    }

    public async Task<string> GetPayPalAccessToken()
    {
        string accessToken = "";

        string url = PaypalUrl + "/v1/oauth2/token";

        using (var client = new HttpClient())
        {
            string cridentials64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(PaypalClientId + ":" + PaypalSecret));
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + cridentials64);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

            var httpResponse = await client.SendAsync(requestMessage);

            if (httpResponse.IsSuccessStatusCode)
            {
                var strResponse = await httpResponse.Content.ReadAsStringAsync();

                var jsonResponse = JsonNode.Parse(strResponse);
                if (jsonResponse != null)
                {
                    accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                }
            }
        }
        return accessToken;
    }

    public async Task<JsonResult> CreateOrder(string total)
    {
        if (total == null)
        {
            return new JsonResult(new { Id = "" });
        }

        JsonObject createOrderRequest = new JsonObject();
        createOrderRequest.Add("intent", "CAPTURE");

        JsonObject amount = new JsonObject();
        amount.Add("currency_code", "USD");
        amount.Add("value", total);

        JsonObject purchaseUnit1 = new JsonObject();
        purchaseUnit1.Add("amount", amount);

        JsonArray purchaseUnits = new JsonArray();
        purchaseUnits.Add(purchaseUnit1);

        createOrderRequest.Add("purchase_units", purchaseUnits);

        string accessToken = await GetPayPalAccessToken();

        string url = PaypalUrl + "/v2/checkout/orders";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");

            var httpResponse = await client.SendAsync(requestMessage);

            if (httpResponse.IsSuccessStatusCode)
            {
                var strResponse = await httpResponse.Content.ReadAsStringAsync();
                var jsonResponse = JsonNode.Parse(strResponse);

                if (jsonResponse != null)
                {
                    string paypalOrderId = jsonResponse["id"]?.ToString() ?? "";

                    return new JsonResult(new { Id = paypalOrderId });
                }
            }
        }
        return new JsonResult(new { Id = total });
    }
}


