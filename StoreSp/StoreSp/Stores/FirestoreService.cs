using Google.Cloud.Firestore;
using StoreSp.Endpoints;

namespace StoreSp.Stores;

public abstract class FirestoreService
{
    public readonly FirestoreDb _firestoreDb;

    public FirestoreService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public QuerySnapshot GetSnapshots(string collectionName){
        var collection = _firestoreDb.Collection(collectionName);
        var snapshot = collection.GetSnapshotAsync();
        return snapshot.Result;
    }

    public static void Run(FirestoreDb db){
        UserEndpoint.userFireStore = new UserFireStore(db);
    }
}
