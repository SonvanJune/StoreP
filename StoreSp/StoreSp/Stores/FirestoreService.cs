using Google.Cloud.Firestore;

namespace StoreSp.Stores;

public abstract class FirestoreService
{
    public static FcmService _fmcService = null!;

    public static void Run(FirestoreDb db, string projectId)
    {
        FmcSendNotificaton();
    }

    public static void FmcSendNotificaton()
    {
        _fmcService = new FcmService();
    }
}
