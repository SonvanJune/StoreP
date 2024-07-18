using Google.Cloud.Firestore;
using StoreSp.Services.Impl;
using StoreSp.Stores.Stores;

namespace StoreSp.Stores;

public abstract class FirestoreService(FirestoreDb firestoreDb)
{
    public readonly FirestoreDb _firestoreDb = firestoreDb;

    public QuerySnapshot GetSnapshots(string collectionName){
        var collection = _firestoreDb.Collection(collectionName);
        var snapshot = collection.GetSnapshotAsync();
        return snapshot.Result;
    }

    public static void Run(FirestoreDb db){
        UserServiceImpl.userFireStore = new UserFireStore(db);
        RoleServiceImpl.roleFireStore = new RoleFireStore(db);
        CategoryServiceImpl.CategoryFireStore = new CategoryFireStore(db);
        ProductServiceImpl.ProductFireStore = new ProductFireStore(db);
        CartServiceImpl.CartFireStore = new CartFireStore (db);
    }
}
