using Google.Cloud.Firestore;
using StoreSp.Endpoints;
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
        UserEndpoint.userFireStore = new UserFireStore(db);
        RoleEndpoint.roleFireStore = new RoleFireStore(db);
    }
}
