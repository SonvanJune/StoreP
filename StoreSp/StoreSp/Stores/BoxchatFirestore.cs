using Google.Cloud.Firestore;

namespace StoreSp.Stores;

public class BoxchatFirestore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionBoxchat = "Boxchats";
    public static string _collectionBoxchat_User = "Boxchat_User";
    public static string _collectionMessage = "Messages";

    public async Task<string> CreateBoxchat(string usernameSender , string usernameReceiver){
        
        return "";
    }
}
