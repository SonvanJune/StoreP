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

await FirestoreService._fmcService.SendNotificationAsync("cCpHlMnYS2O745lGEJ52wS:APA91bHJlWsLSRX5e6XomhJaHiJxUTxKJN_41gFMRDYGc6aDfcPRr0svGbLsIg8-OodlX57XtJ09ZAunYu-IaXsF6btdr4-Td6E1QOOBC2miT4h7Uy3e3r2AFsoJ6yeJYVHqngRLzNZC","Son dau buoi", "This is a test notification");
app.Run();
