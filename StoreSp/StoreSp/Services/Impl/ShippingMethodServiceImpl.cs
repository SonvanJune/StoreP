using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class ShippingMethodServiceImpl : IShippingMethodService
{
    public static ShippingMethodFirestore? ShippingMethodFirestore { get; set; }

    IResult IShippingMethodService.AddShippingUnit(AddShippingMethodDto createShippingMethodDto)
    {
        ShippingMethodFirestore!.Add(createShippingMethodDto);
        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Created Success",
            data = null
        });
    }

    IResult IShippingMethodService.GetAllShippingMethods()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = ShippingMethodFirestore!.GetAllShippingMethods().Result
        });
    }
}
