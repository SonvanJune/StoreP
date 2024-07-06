using Google.Cloud.Firestore;
using StoreSp;
using StoreSp.Endpoints;
using StoreSp.Stores;

var builder = WebApplication.CreateBuilder(args);

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\storep-d7a1c-firebase-adminsdk-2a02r-2037f0e85e.json");
FirestoreDb db = FirestoreDb.Create(builder.Configuration.GetConnectionString("ProjectId"));
FirestoreService.Run(db);

var app = builder.Build();
app.MapUserEndpoints();
app.MapRoleEndpoints();

app.Run();
