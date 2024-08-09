using Microsoft.AspNetCore.Mvc;
using StoreSp.Dtos.request;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Endpoints;

public static class UploadEndpoint
{
    public static IUploadService? UploadService { get; set; }
    public static IAuthService? authService { get; set; }

    public static RouteGroupBuilder MapUploadEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api");
        UploadService = new UploadServiceImpl();
        authService = new AuthServiceImpl();

        group.MapPost("/uploads", (HttpRequest request , [FromHeader] string authorization) =>
        {
            UploadFilesDto uploadFiles = new UploadFilesDto{
                Files = request.Form.Files
            };
            return authService.GetResult(authorization, UploadService.UploadFiles(uploadFiles));
        });

        group.MapGet("/get/image/{imageName}", (string imageName) =>
        {
            
            return UploadService.GetImage(imageName).Result;
        });

        group.MapGet("/get/image/phone/{imageName}", (string imageName) =>
        {
            
            return UploadService.GetImagePhone(imageName).Result;
        });
        return group;
    }
}
