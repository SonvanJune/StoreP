using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class BoxchatFirestore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionBoxchat = "Boxchats";
    public static string _collectionBoxchat_User = "Boxchat_User";
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
            Status = 1
        };

        await boxChatDb.AddAsync(boxchat);
        await AddBoxchatUserAsync(randomCode, userSender, userReceiver);
        return "";
    }

    private async Task AddBoxchatUserAsync(string code, User userSender, User userReceiver)
    {
        var boxChatUserDb = _firestoreDb.Collection(_collectionBoxchat_User);
        var boxChatDbExist = base.GetSnapshots(_collectionBoxchat);
        var boxchatAdded = boxChatDbExist.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == code);

        //add box chat user
        await boxChatUserDb.AddAsync(new Boxchat_User
        {
            BoxchatId = boxchatAdded!.Id,
            UserId = userSender.Id
        });

        await boxChatUserDb.AddAsync(new Boxchat_User
        {
            BoxchatId = boxchatAdded!.Id,
            UserId = userReceiver.Id
        });
    }

    public List<BoxchatDto> GetBoxchats(string username)
    {
        var boxChatDb = GetSnapshots(_collectionBoxchat);
        var userDb = GetSnapshots(UserFireStore._collectionUser);
        var messages = GetSnapshots(_collectionMessage);
        var boxChatUserDb = base.GetSnapshots(_collectionBoxchat_User);

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

        var boxchat_user = boxChatUserDb.Documents.Select(r => r.ConvertTo<Boxchat_User>()).ToList().FindAll(r => r.UserId == user.Id);

        foreach (var item in boxchat_user)
        {
            var boxchat = boxChatDb.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Id == item.BoxchatId);

            var sender = GetSender(boxchat!.Id!, user.Id!);
            var userInBoxChatDto = new UserInBoxChatDto
            {
                Name = sender!.Name,
                Avatar = sender!.Avatar,
                Username = sender!.Email ?? sender!.Phone,
            };

            string LastMessage = "";
            var messageList = messages.Documents.Select(r => r.ConvertTo<Message>()).ToList().FindAll(r => r.BoxchatId == boxchat!.Id);
            var messagesDecending = messageList.OrderByDescending(item => item.CreatedAt).ToList();
            if (messagesDecending.Count > 0 && messagesDecending != null)
            {
                LastMessage = messagesDecending[0].Text;

            }


            int countMessNotRead = 0;
            var countMessNotReadList = messages.Documents.Select(r => r.ConvertTo<Message>()).ToList().FindAll(r => r.BoxchatId == boxchat.Id && r.Status == 0 && r.SenderId != user.Id);
            if (countMessNotReadList.Count > 0 && countMessNotReadList != null)
            {
                countMessNotRead = countMessNotReadList.Count();
            }


            var boxchatDto = new BoxchatDto
            {
                Code = boxchat.Code!,
                LastMessage = LastMessage,
                CountMessNotRead = countMessNotRead,
                Sender = userInBoxChatDto
            };

            boxchatDtos.Add(boxchatDto);
        }

        return boxchatDtos;
    }

    public User GetSender(string boxchatId, string receiverId)
    {
        var boxChatUserDb = base.GetSnapshots(_collectionBoxchat_User);
        var usersInBoxchats = boxChatUserDb.Documents.Select(r => r.ConvertTo<Boxchat_User>()).ToList().FindAll(r => r.BoxchatId == boxchatId);
        var userDb = GetSnapshots(UserFireStore._collectionUser);
        foreach (var item in usersInBoxchats)
        {
            if (item.UserId != receiverId)
            {
                return userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item.UserId)!;
            }
        }
        return null!;
    }

    public async Task<string> CreateMessage(CreateMessageDto createMessageDto, string sender)
    {
        var messageDb = _firestoreDb.Collection(_collectionMessage);
        var boxChatDb = GetSnapshots(_collectionBoxchat);
        var userDb = GetSnapshots(UserFireStore._collectionUser);

        //find user
        User userSender = null!;
        User userReceiver = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == sender) == null)
        {
            userSender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == sender)!;
        }
        else
        {
            userSender = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == sender)!;
        }

        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createMessageDto.Receiver) == null)
        {
            userReceiver = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == createMessageDto.Receiver)!;
        }
        else
        {
            userReceiver = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createMessageDto.Receiver)!;
        }

        if (userSender == null || userReceiver == null)
        {
            return null!;
        }


        var boxchat = boxChatDb.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == createMessageDto.BoxchatCode);
        if (boxchat != null)
        {
            var Message = new Message
            {
                Text = createMessageDto.Message,
                SenderId = userSender.Id,
                ReceiverId = userReceiver.Id,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                BoxchatId = boxchat.Id,
                Status = 0
            };

            await messageDb.AddAsync(Message);
        }
        return "";
    }

    public async Task<List<MessageDto>> GetMessages(string boxchatCode, string username)
    {
        var userDb = GetSnapshots(UserFireStore._collectionUser);
        var messageDb = GetSnapshots(_collectionMessage);
        var boxChatDb = GetSnapshots(_collectionBoxchat);

        var boxchat = boxChatDb.Documents.Select(r => r.ConvertTo<Boxchat>()).ToList().Find(r => r.Code == boxchatCode);
        var result = new List<MessageDto>();

        var messages = messageDb.Documents.Select(r => r.ConvertTo<Message>()).ToList().FindAll(r => r.BoxchatId == boxchat!.Id);
        var messagesDecending = messages.OrderByDescending(item => item.CreatedAt).ToList();

        List<Message> messSenderNotMe = new List<Message>();
        User me = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) == null)
        {
            me = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }
        else
        {
            me = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }

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

            if (sender.Id != me.Id)
            {
                messSenderNotMe.Add(item);
            }
            result.Add(dto);
        }

        foreach (var item in messSenderNotMe)
        {
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
