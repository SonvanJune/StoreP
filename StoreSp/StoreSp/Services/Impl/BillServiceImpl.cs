using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class BillServiceImpl : IBillService
{
    public static BillFirestore BillFirestore = new BillFirestore();
    IResult IBillService.Checkout(CreateBillDto createBillDto)
    {
        var item = BillFirestore!.Checkout(createBillDto).Result;
        if (item == 0)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Giỏ hàng rỗng không thể thanh toán",
                data = null
            });
        }

        if (item == -1)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Số dư tài khoản không đủ",
                data = null
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Thanh toán thành công",
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
                message = "Không tìm thấy người dùng",
                data = null
            });
        }
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = data
        });
    }

    IResult IBillService.GetBills()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = BillFirestore!.GetBills()
        });
    }

    IResult IBillService.ReOrderProducts(string code)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = BillFirestore!.ReOrderProducts(code)
        });
    }

    IResult IBillService.UpdateBillStatus(UpdateBillDto updateBillDto)
    {
        var data = BillFirestore!.UpdateStatusBill(updateBillDto).Result;
        if(data == null){
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy hóa đơn",
                data = null
            });
        }
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = data
        });
    }
}
