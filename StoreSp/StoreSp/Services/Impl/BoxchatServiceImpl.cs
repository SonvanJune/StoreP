
using System.Net;
using StoreSp.Commonds;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class BoxchatServiceImpl : IBoxchatService
{
    public static BoxchatFirestore? BoxchatFirestore { get; set; }
    IResult IBoxchatService.CreateBoxChat(string usernameSender, string usernameReceiver)
    {
        var item = BoxchatFirestore!.CreateBoxchat(usernameSender, usernameReceiver).Result;
        if (item == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Không tìm thấy người dùng để tạo hội thoại",
                data = null
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Thêm hội thoại thành công",
            data = null
        });
    }

    IResult IBoxchatService.GetBoxchats(string username)
    {
        var data = BoxchatFirestore!.GetBoxchats(username);
        if (data == null)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Không tìm thấy người dùng để lấy hội thoại",
                data = null
            });
        }

        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Lấy thành công",
            data = data
        });
    }
}
