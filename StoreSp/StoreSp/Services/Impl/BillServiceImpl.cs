using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class BillServiceImpl : IBillService
{
    public static BillFirestore? BillFirestore { get; set; }
    IResult IBillService.Checkout(CreateBillDto createBillDto)
    {
        var item = BillFirestore!.Checkout(createBillDto).Result;
        if (item == 0)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Gio hang rong khong the thanh toan",
                data = null
            });
        }

        if (item == -1)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "So du tai khoan khong du",
                data = null
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Thanh toan thanh cong",
            data = null
        });
    }

    IResult IBillService.GetBillByUser(GetBillOfUserDto getBillOfUserDto)
    {
        var data = BillFirestore!.GetBillByUser(getBillOfUserDto);
        if(data == null){
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Khong tim thay nguoi dung",
                data = null
            });
        }
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "success",
            data = data
        });
    }

    IResult IBillService.ReOrderProducts(string code)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "success",
            data = BillFirestore!.ReOrderProducts(code)
        });
    }
}
