using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Sockets;

public class ChatSocketService
{
    public static BoxchatFirestore? BoxchatFirestore { get; set; }
    private static readonly ConcurrentBag<WebSocket> _connectedSockets = new ConcurrentBag<WebSocket>();

    public async Task GetMessageByUserNameSocket(HttpContext context)
    {
        if (context.Request.Headers["Upgrade"] == "websocket")
        {
            var boxchatCode = context.Request.RouteValues["boxchatCode"]!.ToString();
            if (context.Request.Path == $"/ws/chat/{boxchatCode}")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    _connectedSockets.Add(webSocket);

                    await HandleWebSocketAsync(webSocket, boxchatCode!);

                    // Xóa kết nối khỏi danh sách khi kết thúc
                    _connectedSockets.TryTake(out _);
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

    }

    private async Task HandleWebSocketAsync(WebSocket webSocket, string boxchatCode)
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
            string receiver = root.GetProperty("receiver").GetString()!;
            string message = root.GetProperty("message").GetString()!;
            string username = root.GetProperty("username").GetString()!;
            if (receiver != "" && message != "" && username != "")
            {
                var dto = new CreateMessageDto
                {
                    Receiver = receiver,
                    Message = message,
                    BoxchatCode = boxchatCode
                };
                await BoxchatFirestore!.CreateMessage(dto, username);
            }
            List<WebSocket> listClient = _connectedSockets.ToList();
            // Gửi dữ liệu đến tất cả các kết nối
            for (int i = 0; i < listClient.Count; i++)
            {
                var boxchatDtos = BoxchatFirestore!.GetMessages(boxchatCode, username).Result;
                var re = new HttpStatusConfig
                {
                    status = HttpStatusCode.OK,
                    message = "success",
                    data = boxchatDtos
                };
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // hoặc null để giữ nguyên kiểu chữ
                };
                var jsonString = JsonSerializer.Serialize(re,options);
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