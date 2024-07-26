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

    IResult IProductService.GetProductByCode(string code)
    {
        if(ProductFireStore!.GetProductByProductCode(code) == null){
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Khong tim thay san pham",
                data = null
            });
        }
        
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.GetProductByProductCode(code)
        });
    }

    IResult IProductService.GetProductsByCategory(string code)
    {
        if(ProductFireStore!.GetProductsByCategory(code) == null){
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Khong tim thay danh muc",
                data = null
            });
        }
        
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.GetProductsByCategory(code)
        });
    }

    IResult IProductService.LikeProduct(LikeProductDto likeProductDto)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.LikeProduct(likeProductDto)
        });
    }
}
