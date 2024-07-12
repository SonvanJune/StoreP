using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class CategoryServiceImpl : ICategoryService
{
    public static CategoryFireStore? categoryFireStore { get; set; }
    
    IResult ICategoryService.AddCategory(CreateCategoryDto createCategoryDto)
    {
        if (categoryFireStore is null)
        {
            return Results.NoContent();
        }

        categoryFireStore!.Add(createCategoryDto);

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
            data = categoryFireStore!.GetAllCategories().Result
        });
    }
}
