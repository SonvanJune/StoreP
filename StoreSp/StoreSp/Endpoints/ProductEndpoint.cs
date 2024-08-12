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
            string[] str= authorization.Split(' ');
            var username = authService.GetFirstByToken(str[1]);
            return ProductService.GetProductsByCategory(code , username);
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
            string[] str= authorization.Split(' ');
            var username = authService.GetFirstByToken(str[1]);
            return ProductService.GetProductByCode(pCode , username);
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
            string[] str= authorization.Split(' ');
            var username = authService.GetFirstByToken(str[1]);
            return ProductService.GetProductsBySearch(name , username);
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
            string[] str= authorization.Split(' ');
            var username = authService.GetFirstByToken(str[1]);
            return ProductService.GetProductsNew(dto , username);
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
            string[] str= authorization.Split(' ');
            var username = authService.GetFirstByToken(str[1]);
            return ProductService.GetProductsHot(dto , username);
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
      }).WithParameterValidation().RequireAuthorization("nguoi-mua");
      return group;
   }
}
