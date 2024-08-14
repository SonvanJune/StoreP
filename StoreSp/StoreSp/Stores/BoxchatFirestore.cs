using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class BoxchatFirestore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionBoxchat = "Boxchats";
    public static string _collectionMessage = "Messages";


    //method chinh
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

        //neu boxchat da ton tai
        Boxchat? boxChatExist = boxChatDbExist.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => (r.SenderId == userSender.Id && r.ReceiverId == userReceiver.Id) || (r.SenderId == userReceiver.Id && r.ReceiverId == userSender.Id));
        if (boxChatExist != null)
        {
            return null!;
        }

        //create boxchat
        //box chat of 
        Random rnd = new Random();
        string randomCode1 = rnd.Next(1, 100000).ToString();
        string randomCode2 = rnd.Next(1, 100000).ToString();
        while (boxChatDbExist.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == randomCode1) != null)
        {
            randomCode1 = rnd.Next(1, 100000).ToString();
        }
        while (boxChatDbExist.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == randomCode2) != null)
        {
            randomCode2 = rnd.Next(1, 100000).ToString();
        }

        var boxchatSender = new Boxchat
        {
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            Code = randomCode1,
            Status = 1,
            SenderId = userReceiver.Id,
            ReceiverId = userSender.Id
        };

        var boxchatReceiver = new Boxchat
        {
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            Code = randomCode2,
            Status = 1,
            SenderId = userSender.Id,
            ReceiverId = userReceiver.Id
        };

        await boxChatDb.AddAsync(boxchatSender);
        await boxChatDb.AddAsync(boxchatReceiver);
        return "";
    }

    public List<BoxchatDto> GetBoxchats(string username)
    {
        var boxChatDb = GetSnapshots(_collectionBoxchat);
        var userDb = GetSnapshots(UserFireStore._collectionUser);
        var messages = GetSnapshots(_collectionMessage);

        List<BoxchatDto> boxchatDtos = new List<BoxchatDto>();

        User user;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }

        var boxchats = boxChatDb.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Where(r => r.ReceiverId == user.Id);

        foreach (var item in boxchats)
        {
            var sender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item.SenderId);
            var userInBoxChatDto = new UserInBoxChatDto
            {
                Name = sender!.Name,
                Avatar = sender!.Avatar,
                Username = sender!.Email ?? sender!.Phone,
            };

            string LastMessage = "";
            int countMessNotRead = 0;
            var countMessNotReadList = messages.Documents.Select(r => r.ConvertTo<Message>()).ToList().FindAll(r => r.BoxchatId == item.Id && r.Status == 0);
            if (countMessNotReadList.Count > 0 && countMessNotReadList != null)
            {
                countMessNotRead = countMessNotReadList.Count();
                LastMessage = countMessNotReadList[0].Text;
            }

            var boxchatDto = new BoxchatDto
            {
                Code = item.Code!,
                LastMessage = LastMessage,
                CountMessNotRead = countMessNotRead,
                Sender = userInBoxChatDto
            };

            boxchatDtos.Add(boxchatDto);
        }

        return boxchatDtos;
    }

    public async Task<string> CreateMessage(CreateMessageDto createMessageDto, string receiver)
    {
        var messageDb = _firestoreDb.Collection(_collectionMessage);
        var boxChatDb = GetSnapshots(_collectionBoxchat);
        var userDb = GetSnapshots(UserFireStore._collectionUser);

        //find user
        User userSender = null!;
        User userReceiver = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == receiver) == null)
        {
            userSender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == receiver)!;
        }
        else
        {
            userSender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == receiver)!;
        }

        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createMessageDto.Sender) == null)
        {
            userReceiver = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == createMessageDto.Sender)!;
        }
        else
        {
            userReceiver = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createMessageDto.Sender)!;
        }

        if (userSender == null || userReceiver == null)
        {
            return null!;
        }

        var boxchatReceiver = boxChatDb.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.ReceiverId == userReceiver.Id && r.SenderId == userSender.Id);
        var boxchatSender = boxChatDb.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.ReceiverId == userSender.Id && r.SenderId == userReceiver.Id);
        if (boxchatReceiver != null && boxchatSender != null)
        {
            var MessageReceiver = new Message
            {
                Text = createMessageDto.Message,
                SenderId = userSender.Id,
                ReceiverId = userReceiver.Id,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                BoxchatId = boxchatSender.Id,
                Status = 0
            };

            var MessageSender = new Message
            {
                Text = createMessageDto.Message,
                SenderId = userReceiver.Id,
                ReceiverId = userSender.Id,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                BoxchatId = boxchatReceiver.Id,
                Status = 0
            };

            await messageDb.AddAsync(MessageReceiver);
            await messageDb.AddAsync(MessageSender);
        }

        return "";
    }

    public async Task<List<MessageDto>> GetMessages(string boxchatCode)
    {
        var userDb = GetSnapshots(UserFireStore._collectionUser);
        var messageDb = GetSnapshots(_collectionMessage);
        var boxChatDb = GetSnapshots(_collectionBoxchat);

        var boxchat = boxChatDb.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == boxchatCode);
        var result = new List<MessageDto>();

        var messages = messageDb.Documents.Select(r => r.ConvertTo<Message>()).ToList().FindAll(r => r.BoxchatId == boxchat!.Id && r.ReceiverId == boxchat.ReceiverId && r.SenderId == boxchat.SenderId);
        var messagesDecending = messages.OrderByDescending(item => item.CreatedAt).ToList();

        foreach (var item in messagesDecending)
        {
            var sender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item.SenderId);
            var senderInBoxChatDto = new UserInBoxChatDto
            {
                Name = sender!.Name,
                Avatar = sender!.Avatar,
                Username = sender!.Email ?? sender!.Phone,
            };

            var receiver = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item.ReceiverId);
            var receiverInBoxChatDto = new UserInBoxChatDto
            {
                Name = receiver!.Name,
                Avatar = receiver!.Avatar,
                Username = receiver!.Email ?? sender!.Phone,
            };

            var dto = new MessageDto
            {
                Id = item.Id!,
                Text = item.Text,
                Sender = senderInBoxChatDto,
                Receiver = receiverInBoxChatDto,
                CreatedAt = item.CreatedAt.ToDateTime().AddHours(7).ToString(),
                Status = item.Status
            };

            result.Add(dto);

            //update status message
            DocumentReference docref = _firestoreDb.Collection(_collectionMessage).Document(item.Id);
            Dictionary<string, object> data = new Dictionary<string, object>{
               {"Status" , 1}
            };
            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                await docref.UpdateAsync(data);
            }
        }
        return result;
    }
}
