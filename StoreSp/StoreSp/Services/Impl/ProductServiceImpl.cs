using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class ProductServiceImpl : IProductService
{
    public static ProductFireStore? ProductFireStore { get; set; }
    IResult IProductService.AddProduct(CreateProductDto createProductDto)
    {
        var product = ProductFireStore!.AddProduct(createProductDto);
        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Created Success",
            data = null
        });
    }
}
