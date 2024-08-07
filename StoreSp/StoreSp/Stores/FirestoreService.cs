using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using StoreSp.Services.Impl;
using StoreSp.Services.Sockets;
using StoreSp.Stores.Stores;

namespace StoreSp.Stores;

public abstract class FirestoreService(FirestoreDb firestoreDb)
{
    public readonly FirestoreDb _firestoreDb = firestoreDb;
    private static StorageClient _storageClient = null!;
    public static FcmService _fmcService = null!;


    public QuerySnapshot GetSnapshots(string collectionName)
    {
        try
        {
            var collection = _firestoreDb.Collection(collectionName);
            var snapshot = collection.GetSnapshotAsync();
            return snapshot.Result;
        }
        catch (Exception)
        {
            throw new InvalidOperationException("loi db");
        }
    }

    public static void Run(FirestoreDb db , string credentialPath , string projectId)
    {
        FirebaseStorageHelper(credentialPath);
        FmcSendNotificaton(projectId , credentialPath);
        if (db != null)
        {
            UserServiceImpl.userFireStore = new UserFireStore(db);
            RoleServiceImpl.roleFireStore = new RoleFireStore(db);
            CategoryServiceImpl.CategoryFireStore = new CategoryFireStore(db);
            ProductServiceImpl.ProductFireStore = new ProductFireStore(db);
            CartServiceImpl.CartFireStore = new CartFireStore(db);
            CartSocketService.CartFireStore = new CartFireStore(db);
            BillServiceImpl.BillFirestore = new BillFirestore(db);
            LogServiceImpl.LogFireStore = new LogFireStore(db);
            NotificationServiceImpl.NotificationFireStore = new NotificationFireStore(db);
            ShippingMethodServiceImpl.ShippingMethodFirestore =  new ShippingMethodFirestore(db);
        }
    }

    public static void FirebaseStorageHelper(string credentialPath){
        GoogleCredential credential;
        using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream);
        }
        _storageClient = StorageClient.Create(credential);
    }

    public static void UploadFile(string localFilePath, string bucketName, string objectName)
    {
        using (var fileStream = new FileStream(localFilePath, FileMode.Open))
        {
            _storageClient.UploadObject(bucketName, objectName, null, fileStream);
        }
    }

    public static void FmcSendNotificaton(string credentialPath , string projectId){
        _fmcService = new FcmService(credentialPath, projectId);
    }
}
