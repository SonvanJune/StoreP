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

    IResult IProductService.GetProductByCode(string code , string username)
    {
        var data = ProductFireStore!.GetProductByProductCode(code , username);
        if( data == null){
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
            data = data
        });
    }

    IResult IProductService.GetProductsByCategory(string code , string username)
    {
        var data = ProductFireStore!.GetProductsByCategory(code , username);
        if( data == null){
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
            data = data
        });
    }

    IResult IProductService.GetProductsBySearch(string name , string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.GetProductsBySearch(name , username)
        });
    }

    IResult IProductService.GetProductsByShop(string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.GetProductsByShop(username)
        });
    }

    IResult IProductService.GetProductsHot(GetProductHot getProductHot , string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.GetProductsHot(getProductHot , username)
        });
    }

    IResult IProductService.GetProductsLike(GetProductLikeDto getProductLikeDto)
    {
        var data = ProductFireStore!.GetProductsLike(getProductLikeDto);
        if(data == null){
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Khong tim thay user",
                data = null
            });
        }
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = data
        });
    }

    IResult IProductService.GetProductsNew(GetNewProductDto getNewProductDto , string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.GetProductsNew(getNewProductDto, username)
        });
    }

    IResult IProductService.LikeProduct(LikeProductDto likeProductDto)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Success",
            data = ProductFireStore!.LikeProduct(likeProductDto).Result
        });
    }
}
