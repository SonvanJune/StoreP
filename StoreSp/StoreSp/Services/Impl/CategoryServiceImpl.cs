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
                message = "Không tìm thấy danh mục cha",
                data = null
            });
        }

        if (status == -1)
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Tên danh mục này đã tồn tại",
                data = null
            });
        }

        return Results.Created("", new HttpStatusConfig
        {
            status = HttpStatusCode.Created,
            message = "Tạo thành công",
            data = null
        });
    }

    IResult ICategoryService.GetAllCategories(bool isMobile)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = CategoryFireStore!.GetAllCategories(isMobile)
        });
    }

    IResult ICategoryService.UpdateCategory(UpdateCategoryDto updateCategoryDto)
    {
        return Results.Created("" , new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Cập nhật thành công",
            data = CategoryFireStore!.UpdateCategory(updateCategoryDto)
        });
    }
}
