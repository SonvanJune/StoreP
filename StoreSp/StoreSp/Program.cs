using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Logging;
using StoreSp.Configs;
using StoreSp.Endpoints;
using StoreSp.Endpoints.SocketEndpoint;
using StoreSp.Stores;

var builder = WebApplication.CreateBuilder(args);
builder.RunConfig();

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\storep-d7a1c-firebase-adminsdk-2a02r-bfa325ae0a.json");
FirestoreDb db = FirestoreDb.Create(builder.Configuration.GetConnectionString("ProjectId"));
FirestoreService.Run(db , @"D:\fir-84aea-firebase-adminsdk-5fzab-9504e21114.json" , builder.Configuration.GetConnectionString("ProjectId")!);


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseHsts();
    IdentityModelEventSource.ShowPII = true;    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthCore API V1");
    });
}
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.UseCors();

app.MapUserEndpoints();
app.MapRoleEndpoints();
app.MapCategoryEndpoints();
app.MapProductEndpoints();
app.MapCartEndpoints();
app.MapBillEndpoints();
app.MapLogEndpoints();
app.MapShippingMehodEndpoints();
app.MapBannerEndpoints();

app.MapCartSocketEndpoint();
app.MapUploadEndpoints();

await FirestoreService._fmcService.SendNotificationAsync("eopJfBtBRlysVuuaY1AwL2:APA91bFagkLLw0OqbwkJZPTBsDGtDfBO8KaxTRbmu8BwflZy6pJRjqgD70p1-Edfbn6t8CSaJJibxsbMSqU_9T1itjeXIQ2k8lEKOunoRUd1BE-eLMXLV-cHvHhJOj5mAj9Kq-l9k1Uj","Son dau buoi", "This is a test notification");
app.Run();
