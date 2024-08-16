using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Primitives;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;
using Vonage.ProactiveConnect.Lists;

namespace StoreSp.Services.Sockets;

public class ChatSocketService
{
    public static BoxchatFirestore? BoxchatFirestore { get; set; }
    private static readonly ConcurrentBag<WebSocket> _connectedSockets = new ConcurrentBag<WebSocket>();
    private static readonly List<StringValues> _boxchatCodes = new List<StringValues>();

    public async Task GetMessageByUserNameSocket(HttpContext context)
    {
        if (context.Request.Path == "/ws/chat" && context.Request.Headers["Upgrade"] == "websocket")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var requestParams = context.Request.Query;
                var boxchatCode = requestParams["boxchatCode"];

                // Thêm kết nối vào danh sách
                _connectedSockets.Add(webSocket);
                _boxchatCodes.Add(boxchatCode);

                await HandleWebSocketAsync(webSocket);

                // Xóa kết nối khỏi danh sách khi kết thúc
                _connectedSockets.TryTake(out _);
                _boxchatCodes.Remove(boxchatCode);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await context.Response.WriteAsync("WebSocket endpoint");
        }
    }

    private async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        //parse tu jso sang mang gia tri
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        while (!result.CloseStatus.HasValue)
        {
            var jsons = Encoding.UTF8.GetString(buffer, 0, result.Count);
            JsonDocument doc = JsonDocument.Parse(jsons);
            JsonElement root = doc.RootElement;
            //lay cac gia tri gui ve
            string sender = root.GetProperty("sender").GetString()!;
            string message = root.GetProperty("message").GetString()!;
            string username = root.GetProperty("username").GetString()!;
            if (sender != "" && message != "" && username != "")
            {
                var dto = new CreateMessageDto
                {
                    Sender = sender,
                    Message = message
                };
                await BoxchatFirestore!.CreateMessage(dto, username);
            }
            List<WebSocket> listClient = _connectedSockets.ToList();
            // Gửi dữ liệu đến tất cả các kết nối
            for (int i = 0; i < listClient.Count; i++)
            {
                var boxchatDtos = BoxchatFirestore!.GetMessages(_boxchatCodes[i]!).Result;
                var re = new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "success",
                    data = boxchatDtos
                };
                var jsonString = JsonSerializer.Serialize(re);
                var buff = Encoding.UTF8.GetBytes(jsonString);
                if (listClient[i].State == WebSocketState.Open)
                {
                    // var responseMessage = $"Other people: {message}";
                    // var responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                    await listClient[i].SendAsync(new ArraySegment<byte>(buff), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}