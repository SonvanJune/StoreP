using Google.Cloud.Firestore;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class TokenFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    private const string _collectionToken = "Tokens";
}
