using Google.Cloud.Firestore;
using StoreSp.Configs;
using StoreSp.Endpoints;
using StoreSp.Services;
using StoreSp.Services.Impl;
using StoreSp.Stores;
using Vonage;
using Vonage.Request;

var builder = WebApplication.CreateBuilder(args);
builder.RunConfig();

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\storep-d7a1c-firebase-adminsdk-2a02r-2037f0e85e.json");
FirestoreDb db = FirestoreDb.Create(builder.Configuration.GetConnectionString("ProjectId"));
FirestoreService.Run(db);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthCore API V1");
    });
}
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.MapUserEndpoints();
app.MapRoleEndpoints();
app.MapCategoryEndpoints();

// var credentials = Credentials.FromApiKeyAndSecret(
//     "46dde3ba",
//     "To44NpkdcWv9du8S"
//     );

// var VonageClient = new VonageClient(credentials);

// app.MapPost("/users/send-otp", async () =>
// {
//     var otp = "234578";
//     var response = await VonageClient.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest()
//     {
//         To = "84377465180",
//         From = "84898129787",
//         Text = " Ma cua ban la : " + otp + " "
//     });

// });

app.Run();
