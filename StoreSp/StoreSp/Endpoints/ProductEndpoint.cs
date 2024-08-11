using System.Net;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class ProductEndpoint
{
   public static IProductService? ProductService { get; set; }
   public static IAuthService? authService { get; set; }

   public static RouteGroupBuilder MapProductEndpoints(this WebApplication app)
   {
      var group = app.MapGroup("api/products");
      ProductService = new ProductServiceImpl();
      authService = new AuthServiceImpl();

      group.MapGet("/categories", ([FromQuery] string code, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.GetProductsByCategory(code);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            });
         }

      }).RequireAuthorization();

      group.MapGet("", ([FromQuery] string pCode, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.GetProductByCode(pCode);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            });
         }
      }).RequireAuthorization();

      group.MapGet("/search", ([FromQuery] string name, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.GetProductsBySearch(name);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            });
         }
      }).RequireAuthorization();

      group.MapPost("/new", (GetNewProductDto dto, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.GetProductsNew(dto);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            });
         }
      }).RequireAuthorization();

      group.MapPost("/like/get", (GetProductLikeDto dto, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.GetProductsLike(dto);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            });
         }
      }).RequireAuthorization();

      group.MapPost("/hot", (GetProductHot dto, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.GetProductsHot(dto);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            });
         }
      }).RequireAuthorization();

      group.MapPost("/", (CreateProductDto createProductDto, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.AddProduct(createProductDto);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            });
         }
      }).WithParameterValidation().RequireAuthorization("nguoi-ban");

      group.MapPost("/like", (LikeProductDto likeProductDto, [FromHeader] string authorization) =>
      {
         if (authService.GetResult(authorization) == 1)
         {
            return ProductService.LikeProduct(likeProductDto);
         }
         else
         {
            return Results.BadRequest(new HttpStatusConfig
            {
               status = HttpStatusCode.BadRequest,
               message = "Token has expired",
               data = null
            }); 
         }
      }).WithParameterValidation().RequireAuthorization("nguoi-ban");
      return group;
   }
}
