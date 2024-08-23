using Microsoft.EntityFrameworkCore;
using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;
using StoreSp.Services.Impl;

namespace StoreSp.Stores;

public class UserFireStore
{
    private readonly AppDbContext? _appDbContext = null;

    public UserFireStore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    //Property
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    public readonly IBaseConverter<Address, AddressDto> addressConverter = new AddressConverter();
    private readonly IBaseConverter<User, RegisterUserDto> registerUserConverter = new RegisterUserConverter();
    private readonly IBaseConverter<User, GoogleRegisterDto> googleRegisterConverter = new GoogleRegisterConverter();
    private readonly IBaseConverter<Address, CreateAddressDto> createAddressConverter = new CreateAddressConverter();
    public readonly LogFireStore logFireStore = new LogFireStore();
    public readonly NotificationFireStore notificationFireStore = new NotificationFireStore();

    //Method it su dung
    public Task<List<UserDto>> GetAllUser()
    {
        var user = _appDbContext!.Users.ToList();
        List<UserDto> result = new List<UserDto>();
        foreach (var item in user)
        {
            var role = _appDbContext!.Roles.SingleOrDefault(r => r.Id == item.RoleId);
            var userDto = userConverter.ToDto(item);
            userDto.RoleCode = role?.Code;
            result.Add(userDto);
        }
        return Task.FromResult(result);
    }

    public Task<UserDto> GetUser(string id)
    {
        var user = _appDbContext!.Users.SingleOrDefault(r => r.Id == Convert.ToInt32(id));
        var role = _appDbContext!.Roles.SingleOrDefault(r => r.Id == user!.RoleId);
        var userDto = userConverter.ToDto(user!);
        userDto.RoleCode = role?.Code;
        return Task.FromResult(userDto);
    }

    public Task Add(CreateUserDto userDto)
    {
        return null!;
    }

    //Method chinh
    public Task<List<UserDto>> GetUserByRole(string roleCode)
    {
        var role = _appDbContext!.Roles.SingleOrDefault(r => r.Code == roleCode);

        if (role == null)
        {
            return null!;
        }

        var users = _appDbContext!.Users.Where(u => u.RoleId == role.Id).ToList();
        List<UserDto> userDtos = new List<UserDto>();
        foreach (var item in users)
        {
            var dto = userConverter.ToDto(item);
            dto.RoleCode = role.Code;
            userDtos.Add(dto);
        }
        return Task.FromResult(userDtos);
    }
    public async Task<User> Register(RegisterUserDto userDto)
    {
        if (userDto.Email == null)
        {
            var phoneExist = _appDbContext!.Users.SingleOrDefault(p => p.Phone == userDto.Phone)!;
            if (phoneExist != null)
            {
                return null!;
            }
        }

        if (userDto.Phone == null)
        {
            var emailExist = _appDbContext!.Users.SingleOrDefault(p => p.Email == userDto.Email)!;
            if (emailExist != null)
            {
                return null!;
            }
        }

        var user = registerUserConverter.ToEntity(userDto);
        var role = _appDbContext!.Roles.SingleOrDefault(u => u.Code == userDto.RoleCode);
        if (role == null)
        {
            return null!;
        }
        user.RoleId = role.Id;
        user.Role = role;
        if (userDto.Email != null && userDto.Email != "")
        {
            user.VerificationToken = AuthServiceImpl.CreateRandomToken(user);
        }
        user.DeviceToken = user.DeviceToken is null ? null : user.DeviceToken;
        await _appDbContext!.Users.AddAsync(user);
        await _appDbContext!.SaveChangesAsync();
        CreateCartForUser(user);

        // //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "dang-ky");
        await notificationFireStore.AddNotificationForUser(user, "Chào mừng bạn đến với ứng dụng", 0);
        return user;
    }

    public async Task<User> Login(LoginUserDto loginUserDto)
    {
        User user;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == loginUserDto.Username) == null)
        {
            if (_appDbContext!.Users.SingleOrDefault(r => r.Phone == loginUserDto.Username) == null)
            {
                return null!;
            }
            else
            {
                user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == loginUserDto.Username)!;
            }
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == loginUserDto.Username)!;
        }

        if (user.IsGoogleAccount == 1)
        {
            return user;
        }
        else
        {
            if (!BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash))
            {
                return null!;
            }

            var role = _appDbContext!.Roles.SingleOrDefault(r => r.Id == user!.RoleId);
            user.Role = role;

            //tao log cho user dang ky
            await logFireStore.AddLogForUser(user, "dang-nhap");

            //tao refresh token
            var token = AuthServiceImpl.CreateRefreshToken(user);
            user.RefreshToken = token;
            _appDbContext!.Users.Update(user);
            await _appDbContext!.SaveChangesAsync();
            return user;
        }
    }

    public User GetUserByUsername(string username)
    {
        User user;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) != null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == username)!;
        }
        if (user == null)
        {
            return null!;
        }
        var role = _appDbContext.Roles.SingleOrDefault(r => r.Id == user.RoleId);
        user.Role = role;
        return user;
    }

    public async Task<User> VerifyUser(string username)
    {
        User user;
        if (_appDbContext!.Users.SingleOrDefault(p => p.Email == username)! != null)
        {
            user = _appDbContext!.Users.SingleOrDefault(p => p.Email == username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(p => p.Phone == username)!;
        }

        user.VerifiedAt = DateTime.Now;
        var token = AuthServiceImpl.CreateRefreshToken(user);
        user.RefreshToken = token;
        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();

        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "da-xac-thuc");
        return user;
    }

    public async Task<User> ForgetPaswordByEmail(string email, string randomCode)
    {
        var user = _appDbContext!.Users.SingleOrDefault(r => r.Email == email);
        if (user == null || user.IsGoogleAccount == 1)
        {
            return null!;
        }

        user.PasswordReestToken = BCrypt.Net.BCrypt.HashPassword(randomCode);
        user.ResetTokenExpires = DateTime.Now.AddDays(1);

        _appDbContext!.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "quen-mat-khau");
        return user;
    }

    public async Task<User> CheckResetCode(ResetCodeDto resetCodeDto)
    {
        var userList = _appDbContext!.Users.ToList();
        User user = null!;
        foreach (var u in userList)
        {
            if (u.PasswordReestToken != null)
            {
                if (BCrypt.Net.BCrypt.Verify(resetCodeDto.Code, u.PasswordReestToken))
                {
                    user = u;
                    break;
                }
            }
        }

        if (user == null)
        {
            return null!;
        }

        user.IsUpdated = 1;
        _appDbContext!.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
        await logFireStore.AddLogForUser(user, "da-nhap-ma-otp");
        return user;
    }

    public async Task<User> ResetPasswordOfEmail(ResetPasswordDto resetPasswordDto)
    {
        var userList = _appDbContext!.Users.ToList();
        User user = null!;
        foreach (var u in userList)
        {
            if (u.PasswordReestToken != null)
            {
                if (BCrypt.Net.BCrypt.Verify(resetPasswordDto.Code, u.PasswordReestToken))
                {
                    user = u;
                    break;
                }
            }
        }

        if (user == null || user.IsUpdated == 0)
        {
            return null!;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
        user.PasswordReestToken = null!;
        user.ResetTokenExpires = new DateTime(1111, 11, 11, 11, 11, 11);
        user.IsUpdated = 0;
        _appDbContext!.Users.Update(user);
        await _appDbContext.SaveChangesAsync();

        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "thay-doi-mat-khau");
        return user;
    }

    public async Task<User> GoogleRegister(GoogleRegisterDto dto)
    {
        var emailExist = _appDbContext!.Users.SingleOrDefault(r => r.Email == dto.Email);
        if (emailExist != null)
        {
            return null!;
        }

        var user = googleRegisterConverter.ToEntity(dto);
        var role = _appDbContext!.Roles.SingleOrDefault(u => u.Code == dto.RoleCode);
        if (role == null)
        {
            return null!;
        }
        user.RoleId = role.Id;
        user.Role = role;
        user.IsGoogleAccount = 1;
        user.VerifiedAt = DateTime.Now;
        user.DeviceToken = user.DeviceToken is null ? null : user.DeviceToken;
        var token = AuthServiceImpl.CreateRefreshToken(user);
        user.RefreshToken = token;
        await _appDbContext!.Users.AddAsync(user);
        await _appDbContext!.SaveChangesAsync();
        CreateCartForUser(user);

        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "dang-ky-bang-google");
        await notificationFireStore.AddNotificationForUser(user, "Chào mừng bạn đến với ứng dụng", 0);
        return user;
    }

    public async Task<User> GoogleLogin(GoogleLoginDto dto)
    {
        var user = _appDbContext!.Users.SingleOrDefault(r => r.Email == dto.Email);
        if (user == null || user.IsGoogleAccount == 0)
        {
            return null!;
        }
        var role = _appDbContext!.Roles.SingleOrDefault(u => u.Id == user.Id);
        user.Role = role;
        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "dang-nhap");


        //tao refresh token
        user.DeviceToken = user.DeviceToken is null ? null : user.DeviceToken;
        var token = AuthServiceImpl.CreateRefreshToken(user);
        user.RefreshToken = token;
        _appDbContext!.Users.Update(user);
        await _appDbContext!.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateStatus(UpdateStatusUserDto dto)
    {
        User user = null!;
        if (dto.Email == null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == dto.Phone)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == dto.Email)!;
        }

        user.Status = dto.Status;
        _appDbContext!.Users.Update(user);
        await _appDbContext.SaveChangesAsync();

        await logFireStore.AddLogForUser(user, "cap-nhat-trang-thai");
        return user;
    }

    //method address
    public async Task<string> AddAdress(CreateAddressDto dto)
    {
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == dto.Username) == null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == dto.Username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == dto.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var address = createAddressConverter.ToEntity(dto);
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (_appDbContext!.Addresses.SingleOrDefault(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        address.Code = randomCode;

        if (address.Status == "1")
        {
            var addExits = _appDbContext!.Addresses.SingleOrDefault(r => r.Status.Contains("1"));
            if (addExits != null)
            {
                addExits.Status = "0";
                _appDbContext!.Addresses.Update(addExits);
                await _appDbContext!.SaveChangesAsync();
            }
        }

        List<User> users = new List<User>();
        users.Add(user);
        address.Users = users;
        _appDbContext!.Addresses.Update(address);
        await _appDbContext!.SaveChangesAsync();
        await logFireStore.AddLogForUser(user, "them-dia-chi");
        return "";
    }
    public Task<List<AddressDto>> GetAddress(string username)
    {
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            user = _appDbContext!.Users.Include(u => u.Addresses).SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            user = _appDbContext!.Users.Include(u => u.Addresses).SingleOrDefault(r => r.Email == username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var addresses = user.Addresses;
        var addressesDto = new List<AddressDto>();
        foreach (var add in addresses!)
        {
            var address = _appDbContext.Addresses.SingleOrDefault(r => r.Id == add.Id);
            addressesDto.Add(addressConverter.ToDto(address!));
        }
        return Task.FromResult(addressesDto);
    }

    //method ho tro
    public bool CheckIsVerified(string email)
    {
        var user = _appDbContext!.Users.SingleOrDefault(p => p.Email == email);
        if (user == null) return false;
        return user.VerifiedAt.Year != 1111;
    }

    public bool CheckValidToken(string email, string token)
    {
        var user = _appDbContext!.Users.SingleOrDefault(p => p.Email == email);
        return user!.VerificationToken == token;
    }

    private async void CreateCartForUser(User user)
    {
        User u = null!;
        if (user.Email == null)
        {
            u = _appDbContext!.Users.SingleOrDefault(p => p.Phone == user.Phone)!;
        }
        else
        {
            u = _appDbContext!.Users.SingleOrDefault(p => p.Email == user.Email)!;
        }

        var cart = new Cart
        {
            UserId = u.Id,
            TotalPrice = 0,
            Items = new List<CartItem>()
        };
        user.Cart = cart;
        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<string> UpdateUser(UpdateUserDto updateUserDto, string username)
    {
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == username)!;
        }

        if (user == null)
        {
            return null!;
        }

        user.Name = updateUserDto.Name;
        user.Avatar = updateUserDto.Image;
        _appDbContext!.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
        return "";
    }
}
