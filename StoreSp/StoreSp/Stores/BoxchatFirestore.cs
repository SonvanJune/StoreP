using Google.Cloud.Firestore;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class BoxchatFirestore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionBoxchat = "Boxchats";
    public static string _collectionBoxchat_User = "Boxchat_User";
    public static string _collectionMessage = "Messages";

    public async Task<string> CreateBoxchat(string usernameSender, string usernameReceiver)
    {
        var boxChatDb = _firestoreDb.Collection(_collectionBoxchat);
        var boxChatDbExist = base.GetSnapshots(_collectionBoxchat);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);

        //find user
        User userSender = null!;
        User userReceiver = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == usernameSender) == null)
        {
            userSender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == usernameSender)!;
        }
        else
        {
            userSender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == usernameSender)!;
        }

        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == usernameReceiver) == null)
        {
            userReceiver = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == usernameReceiver)!;
        }
        else
        {
            userReceiver = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == usernameReceiver)!;
        }

        if (userSender == null || userReceiver == null)
        {
            return null!;
        }

        //create boxchat
        //box chat of 
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (boxChatDbExist.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }

        var boxchat = new Boxchat
        {
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            Code = randomCode,
            Status = 1,
        };

        await boxChatDb.AddAsync(boxchat);
        await CreateBoxchatUser(randomCode, userSender.Id!, userReceiver.Id!);
        return "";
    }

    private async Task CreateBoxchatUser(string code, string senderId, string receiveId)
    {
        var boxChatUserDb = _firestoreDb.Collection(_collectionBoxchat_User);
        var boxChatDbExist = base.GetSnapshots(_collectionBoxchat);
        var boxchat = boxChatDbExist.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == code);
        var boxchatUser1 = new Boxchat_User
        {
            BoxchatId = boxchat!.Id,
            UserId = senderId,
        };

        var boxchatUser2 = new Boxchat_User
        {
            BoxchatId = boxchat!.Id,
            UserId = receiveId,
        };
        await boxChatUserDb.AddAsync(boxchatUser1);
        await boxChatUserDb.AddAsync(boxchatUser2);
    }
}
