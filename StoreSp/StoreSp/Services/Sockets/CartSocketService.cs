using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using StoreSp.Commonds;
using StoreSp.Stores;

namespace StoreSp.Services.Sockets;

public class CartSocketService
{
    public static CartFireStore? CartFireStore { get; set; }
    public async Task GetCartByUserNameSocket(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var ws = await context.WebSockets.AcceptWebSocketAsync();
            var userName = context.Request.Query["userName"];
            Dictionary<string, string> values = new Dictionary<string, string>();
            while (ws.State == WebSocketState.Open)
            {
                bool canSend = true;
                var cartDtos = CartFireStore!.GetCartByUser(userName!).Result;
                var result = new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "success",
                    data = cartDtos
                };
                var jsonString = JsonSerializer.Serialize(result);
                if (values.ContainsKey(userName!) && values.Count != 0)
                {
                    if (values[userName!].Equals(jsonString))
                    {
                        canSend = false;
                    }
                }

                // Convert the string message to a byte array
                if (canSend == true)
                {
                    var buffer = Encoding.UTF8.GetBytes(jsonString);
                    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    if (values.ContainsKey(userName!))
                    {
                        values[userName!] = jsonString;
                    }
                    else
                    {
                        values.Add(userName!, jsonString);
                    }
                }
                Thread.Sleep(1000);
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
