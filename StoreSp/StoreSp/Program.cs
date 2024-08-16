using System.Net.WebSockets;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Logging;
using StoreSp.Configs;
using StoreSp.Endpoints;
using StoreSp.Endpoints.SocketEndpoint;
using StoreSp.Stores;

var builder = WebApplication.CreateBuilder(args);
builder.RunConfig();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Cấu hình Kestrel để lắng nghe trên địa chỉ IP cụ thể
    serverOptions.Listen(System.Net.IPAddress.Parse("192.168.1.9"), 5000); // IP và cổng
    serverOptions.Listen(System.Net.IPAddress.Parse("127.0.0.1"), 5181); // IP và cổng
});

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\speed-5046d-firebase-adminsdk-i9v5v-a8e81bb926.json");
FirestoreDb db = FirestoreDb.Create(builder.Configuration.GetConnectionString("ProjectId"));
FirestoreService.Run(db, @"D:\speed-5046d-firebase-adminsdk-i9v5v-a8e81bb926.json", builder.Configuration.GetConnectionString("ProjectId")!);


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
app.UseCors("AllowSpecificOrigins");

app.MapUserEndpoints();
app.MapRoleEndpoints();
app.MapCategoryEndpoints();
app.MapProductEndpoints();
app.MapCartEndpoints();
app.MapBillEndpoints();
app.MapLogEndpoints();
app.MapShippingMehodEndpoints();
app.MapBannerEndpoints();
app.MapBoxchatEndpoints();
app.MapMessageEndpoints();

// app.MapCartSocketEndpoint();
app.MapChatSocketEndpoint();
app.MapPaymentEndpoints();
app.MapUploadEndpoints();

//await FirestoreService._fmcService.SendNotificationAsync("eopJfBtBRlysVuuaY1AwL2:APA91bFagkLLw0OqbwkJZPTBsDGtDfBO8KaxTRbmu8BwflZy6pJRjqgD70p1-Edfbn6t8CSaJJibxsbMSqU_9T1itjeXIQ2k8lEKOunoRUd1BE-eLMXLV-cHvHhJOj5mAj9Kq-l9k1Uj","Son dau buoi", "This is a test notification");

app.Run();
