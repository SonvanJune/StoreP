using Google.Cloud.Firestore;
using StoreSp.Services.Impl;
using StoreSp.Services.Sockets;
using StoreSp.Stores.Stores;

namespace StoreSp.Stores;

public abstract class FirestoreService(FirestoreDb firestoreDb)
{
    public readonly FirestoreDb _firestoreDb = firestoreDb;

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

    public static void Run(FirestoreDb db)
    {
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
            ShippingMethodServiceImpl.ShippingMethodFirestore =  new ShippingMethodFirestore(db);
        }
    }
}
