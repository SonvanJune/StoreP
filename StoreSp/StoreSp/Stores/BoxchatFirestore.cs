using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using StoreSp.Context;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class BoxchatFirestore
{
    private readonly AppDbContext? _appDbContext = null;

    public BoxchatFirestore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    //method chinh
    public async Task<string> CreateBoxchat(string usernameSender, string usernameReceiver)
    {
        AppDbContext appDbContext = AppDbContext.GetInstance();
        //find user
        User userSender = null!;
        User userReceiver = null!;
        if (appDbContext!.Users.SingleOrDefault(r => r.Email == usernameSender) == null)
        {
            userSender = appDbContext!.Users.Include(r => r.Boxchats).SingleOrDefault(r => r.Phone == usernameSender)!;
        }
        else
        {
            userSender = appDbContext!.Users.Include(r => r.Boxchats).SingleOrDefault(r => r.Email == usernameSender)!;
        }

        if (appDbContext!.Users.SingleOrDefault(r => r.Email == usernameReceiver) == null)
        {
            userReceiver = appDbContext!.Users.Include(r => r.Boxchats).SingleOrDefault(r => r.Phone == usernameReceiver)!;
        }
        else
        {
            userReceiver = appDbContext!.Users.Include(r => r.Boxchats).SingleOrDefault(r => r.Email == usernameReceiver)!;
        }

        if (userSender == null || userReceiver == null)
        {
            return null!;
        }

        //create boxchat
        string boxchatCodeExist = "";

        foreach (var item in userSender.Boxchats!)
        {
            foreach (var i in userReceiver.Boxchats!)
            {
                if(item.Code == item.Code){
                    boxchatCodeExist = item.Code!;
                }
            }
        }

        if (boxchatCodeExist == "")
        {
            Random rnd = new Random();
            string randomCode = rnd.Next(1, 100000).ToString();
            while (appDbContext!.Boxchats.SingleOrDefault(r => r.Code == randomCode) != null)
            {
                randomCode = rnd.Next(1, 100000).ToString();
            }

            var boxchat = new Boxchat
            {
                CreatedAt = DateTime.Now,
                Code = randomCode,
                Status = 1
            };
            
            // boxchat.Users!.Add(userSender);
            boxchat.Users!.Add(userReceiver);
            appDbContext!.Boxchats.Add(boxchat);
            await appDbContext!.SaveChangesAsync();
        }

        return boxchatCodeExist;
    }

    public List<BoxchatDto> GetBoxchats(string username)
    {
        List<BoxchatDto> boxchatDtos = new List<BoxchatDto>();

        User user;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            user = _appDbContext!.Users.Include(r => r.Boxchats).SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            user = _appDbContext!.Users.Include(r => r.Boxchats).SingleOrDefault(r => r.Email == username)!;
        }

        foreach (var item in user.Boxchats!)
        {
            var boxchat = _appDbContext!.Boxchats.Include(r => r.Messages).Include(r => r.Users).SingleOrDefault(r => r.Id == item.Id);

            var sender = boxchat!.Users!.ToList().Find(r => r.Id != user.Id);
            var userInBoxChatDto = new UserInBoxChatDto
            {
                Name = sender!.Name,
                Avatar = sender!.Avatar!,
                Username = sender!.Email ?? sender!.Phone!,
            };

            string LastMessage = "";
            var messagesDecending = boxchat.Messages!.OrderByDescending(item => item.CreatedAt).ToList();
            if (messagesDecending.Count > 0 && messagesDecending != null)
            {
                LastMessage = messagesDecending[0].Text;

            }


            int countMessNotRead = 0;
            var countMessNotReadList = boxchat.Messages!.ToList().FindAll(r => r.BoxchatId == boxchat.Id && r.Status == 0 && r.SenderId != user.Id);
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

    public async Task<string> CreateMessage(CreateMessageDto createMessageDto, string sender)
    {
        //find user
        User userSender = null!;
        User userReceiver = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == sender) == null)
        {
            userSender = _appDbContext!.Users.SingleOrDefault(r => r.Phone == sender)!;
        }
        else
        {
            userSender = _appDbContext!.Users.SingleOrDefault(r => r.Email == sender)!;
        }

        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == createMessageDto.Receiver) == null)
        {
            userReceiver = _appDbContext!.Users.SingleOrDefault(r => r.Phone == createMessageDto.Receiver)!;
        }
        else
        {
            userReceiver = _appDbContext!.Users.SingleOrDefault(r => r.Email == createMessageDto.Receiver)!;
        }

        if (userSender == null || userReceiver == null)
        {
            return null!;
        }

        var boxchat = _appDbContext.Boxchats.SingleOrDefault(r => r.Code == createMessageDto.BoxchatCode);
        if (boxchat != null)
        {
            var Message = new Message
            {
                Text = createMessageDto.Message,
                SenderId = userSender.Id,
                ReceiverId = userReceiver.Id,
                CreatedAt = DateTime.Now,
                BoxchatId = boxchat.Id,
                Status = 0
            };

            _appDbContext.Messages.Add(Message);
            await _appDbContext.SaveChangesAsync();
        }
        return "";
    }

    public async Task<List<MessageDto>> GetMessages(string boxchatCode, string username)
    {

        var result = new List<MessageDto>();

        var boxchat = _appDbContext!.Boxchats.Include(r => r.Messages).SingleOrDefault(r => r.Code == boxchatCode);
        var messagesDecending = boxchat!.Messages!.OrderByDescending(item => item.CreatedAt).ToList();

        List<Message> messSenderNotMe = new List<Message>();
        User me = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            me = _appDbContext!.Users.SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            me = _appDbContext!.Users.SingleOrDefault(r => r.Email == username)!;
        }

        foreach (var item in messagesDecending)
        {
            var sender = _appDbContext.Users.SingleOrDefault(r => r.Id == item.SenderId);
            var senderInBoxChatDto = new UserInBoxChatDto
            {
                Name = sender!.Name,
                Avatar = sender!.Avatar!,
                Username = sender!.Email ?? sender!.Phone!,
            };

            var receiver = _appDbContext.Users.SingleOrDefault(r => r.Id == item.ReceiverId);
            var receiverInBoxChatDto = new UserInBoxChatDto
            {
                Name = receiver!.Name,
                Avatar = receiver!.Avatar!,
                Username = receiver!.Email ?? sender!.Phone!,
            };

            var dto = new MessageDto
            {
                Id = item.Id!,
                Text = item.Text,
                Sender = senderInBoxChatDto,
                Receiver = receiverInBoxChatDto,
                CreatedAt = item.CreatedAt.ToString(),
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
            item.Status = 1;
            _appDbContext.Messages.Update(item);
            await _appDbContext.SaveChangesAsync();
        }
        return result;
    }
}
