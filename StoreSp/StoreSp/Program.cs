using Google.Cloud.Firestore;
using StoreSp.Endpoints;
using StoreSp.Stores;

var builder = WebApplication.CreateBuilder(args);

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\project\StoreSp\StoreSp\storep-d7a1c-firebase-adminsdk-2a02r-aeb7215ed2.json");
FirestoreDb db = FirestoreDb.Create(builder.Configuration.GetConnectionString("ProjectId"));
FirestoreService.Run(db);

var app = builder.Build();
app.MapUserEndpoints();

app.Run();
