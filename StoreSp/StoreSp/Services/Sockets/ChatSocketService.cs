using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using StoreSp.Commonds;
using StoreSp.Stores;

namespace StoreSp.Services.Sockets;

public class ChatSocketService
{
    public static BoxchatFirestore? BoxchatFirestore { get; set; }
    
    public async Task GetMessageByUserNameSocket(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var ws = await context.WebSockets.AcceptWebSocketAsync();
            var boxchatCode = context.Request.Query["boxchatCode"];
            Dictionary<string, string> values = new Dictionary<string, string>();
            while (ws.State == WebSocketState.Open)
            {
                bool canSend = true;
                var boxchatDtos = BoxchatFirestore!.GetMessages(boxchatCode!).Result;
                var result = new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "success",
                    data = boxchatDtos
                };
                var jsonString = JsonSerializer.Serialize(result);
                if (values.ContainsKey(boxchatCode!) && values.Count != 0)
                {
                    if (values[boxchatCode!].Equals(jsonString))
                    {
                        canSend = false;
                    }
                }

                // Convert the string message to a byte array
                if (canSend == true)
                {
                    var buffer = Encoding.UTF8.GetBytes(jsonString);
                    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    if (values.ContainsKey(boxchatCode!))
                    {
                        values[boxchatCode!] = jsonString;
                    }
                    else
                    {
                        values.Add(boxchatCode!, jsonString);
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
