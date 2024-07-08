using Google.Cloud.Firestore;
using StoreSp.Configs;
using StoreSp.Endpoints;
using StoreSp.Stores;

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

app.Run();
