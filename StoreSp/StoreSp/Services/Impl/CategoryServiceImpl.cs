using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class CategoryServiceImpl : ICategoryService
{
    public static CategoryFireStore? CategoryFireStore { get; set; }

    IResult ICategoryService.AddCategory(CreateCategoryDto createCategoryDto)
    {
        if (CategoryFireStore is null)
        {
            return Results.NoContent();
        }
        int status = CategoryFireStore!.Add(createCategoryDto);
        if (status == 0)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Khong tim thay danh muc cha",
                data = null
            });
        }

        if (status == -1)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Ten danh muc nay da ton tai",
                data = null
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Created Success",
            data = null
        });
    }

    IResult ICategoryService.GetAllCategories()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Success",
            data = CategoryFireStore!.GetAllCategories()
        });
    }
}
