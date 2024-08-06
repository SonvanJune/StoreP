using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;
using StoreSp.Services.Impl;

namespace StoreSp.Stores;

public class UserFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    //Property
    public static string _collectionUser = "Users";
    public static string _collectionRole = "Roles";
    public static string _collectionAddress = "Addresses";
    public static string _collectionAddress_User = "Address_User";
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    public readonly IBaseConverter<Address, AddressDto> addressConverter = new AddressConverter();
    private readonly IBaseConverter<User, CreateUserDto> createUserConverter = new CreateUserConverter();
    private readonly IBaseConverter<User, RegisterUserDto> registerUserConverter = new RegisterUserConverter();
    private readonly IBaseConverter<User, GoogleRegisterDto> googleRegisterConverter = new GoogleRegisterConverter();
    private readonly IBaseConverter<Address, CreateAddressDto> createAddressConverter = new CreateAddressConverter();
    public readonly LogFireStore logFireStore = new LogFireStore(firestoreDb);

    //Method it su dung
    public Task<List<UserDto>> GetAllUser()
    {
        var snapshot = base.GetSnapshots(_collectionUser);
        var user = snapshot.Documents.Select(s => s.ConvertTo<User>()).ToList();
        return Task.FromResult(user.Select(userConverter.ToDto).ToList());
    }

    public Task<UserDto> GetUser(string id)
    {
        var snapshot = base.GetSnapshots(_collectionUser);
        var user = snapshot.Documents.Select(s => s.ConvertTo<User>()).ToList().Find(u => u.Id == id);
        return Task.FromResult(userConverter.ToDto(user!));
    }

    public Task Add(CreateUserDto userDto)
    {
        var userDb = _firestoreDb.Collection(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        var user = createUserConverter.ToEntity(userDto);
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == userDto.RoleId);
        if (role != null)
        {
            user.RoleId = role.Id;
            user.Role = role;
        }
        return userDb.AddAsync(user);
    }

    //Method chinh
    public Task<List<UserDto>> GetUserByRole(string roleCode)
    {
        var roleDb = base.GetSnapshots(_collectionRole);
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Code == roleCode);

        if (role == null)
        {
            return null!;
        }

        var snapshot = base.GetSnapshots(_collectionUser);
        var users = snapshot.Documents.Select(s => s.ConvertTo<User>()).ToList().FindAll(u => u.RoleId == role.Id).ToList();
        List<UserDto> userDtos = new List<UserDto>();
        foreach (var item in users)
        {
            var dto = userConverter.ToDto(item);
            dto.RoleCode = role.Code;
            dto.Id = item.Id;
            userDtos.Add(dto);
        }
        return Task.FromResult(userDtos);
    }
    public async Task<User> Register(RegisterUserDto userDto)
    {
        var userExistDb = base.GetSnapshots(_collectionUser);
        if (userDto.Email == null)
        {
            var phoneExist = userExistDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == userDto.Phone);
            if (phoneExist != null)
            {
                return null!;
            }
        }
        else
        {
            var emailExist = userExistDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == userDto.Email);
            if (emailExist != null)
            {
                return null!;
            }
        }

        var userDb = _firestoreDb.Collection(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);

        var user = registerUserConverter.ToEntity(userDto);
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Code == userDto.RoleCode);
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
        await userDb.AddAsync(user);
        CreateCartForUser(user);

        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "dang-ky");

        return user;
    }

    public async Task<User> Login(LoginUserDto loginUserDto)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        User user;

        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == loginUserDto.Username) == null)
        {
            if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == loginUserDto.Username) == null)
            {
                return null!;
            }
            else
            {
                user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == loginUserDto.Username)!;
            }
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == loginUserDto.Username)!;
        }

        if (user.IsGoogleAccount)
        {
            return user;
        }
        else
        {
            if (!BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash))
            {
                return null!;
            }

            var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == user!.RoleId);
            user.Role = role;

            //tao log cho user dang ky
            await logFireStore.AddLogForUser(user, "dang-nhap");

            //tao refresh token
            DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
            Dictionary<string, object> data = new Dictionary<string, object>{
            {"DeviceToken" , loginUserDto.DeviceToken}
            };

            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                await docref.UpdateAsync(data);
            }
            return user;
        }
    }

    public User GetUserByUsername(string username)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        User user;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) != null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }
        if (user == null)
        {
            return null!;
        }
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == user.RoleId);
        user.Role = role;
        return user;
    }

    public async Task<User> VerifyUser(string username)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        User user;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) != null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }

        var unspecified = DateTime.UtcNow;
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"VerifiedAt" , specified}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "da-xac-thuc");
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == user.RoleId);
        user.Role = role;
        return user;
    }

    public async Task<User> ForgetPaswordByEmail(string email, string randomCode)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        if (user == null || user.IsGoogleAccount == true)
        {
            return null!;
        }

        var unspecified = DateTime.UtcNow.AddDays(1);
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"PasswordReestToken" , BCrypt.Net.BCrypt.HashPassword(randomCode)},
            {"ResetTokenExpires" , specified}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "quen-mat-khau");
        return user;
    }

    public async Task<User> CheckResetCode(ResetCodeDto resetCodeDto)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var userList = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList();
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

        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"IsUpdated", true}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        return user;
    }

    public async Task<User> ResetPasswordOfEmail(ResetPasswordDto resetPasswordDto)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var userList = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList();
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

        if (user == null || user.IsUpdated == false)
        {
            return null!;
        }

        var unspecified = new DateTime(1111, 11, 11, 11, 11, 11, DateTimeKind.Unspecified);
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"PasswordHash" , BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password)},
            {"PasswordReestToken" , null!},
            {"ResetTokenExpires" , Timestamp.FromDateTime(specified)},
            {"IsUpdated", false}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }

        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "thay-doi-mat-khau");
        return user;
    }

    public async Task<User> GoogleRegister(GoogleRegisterDto dto)
    {
        var userExistDb = base.GetSnapshots(_collectionUser);
        var emailExist = userExistDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == dto.Email);
        if (emailExist != null)
        {
            return null!;
        }

        var userDb = _firestoreDb.Collection(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        var user = googleRegisterConverter.ToEntity(dto);
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Code == dto.RoleCode);
        if (role == null)
        {
            return null!;
        }
        user.RoleId = role.Id;
        user.Role = role;
        user.IsGoogleAccount = true;

        var unspecified = DateTime.UtcNow;
        var specified = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        user.VerifiedAt = Timestamp.FromDateTime(specified);
        await userDb.AddAsync(user);
        CreateCartForUser(user);
        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "dang-ky-bang-google");
        return user;
    }

    public async Task<User> GoogleLogin(GoogleLoginDto dto)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var roleDb = base.GetSnapshots(_collectionRole);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == dto.Email);
        if (user == null || user.IsGoogleAccount == false)
        {
            return null!;
        }
        var role = roleDb.Documents.Select(r => r.ConvertTo<Role>()).ToList().Find(r => r.Id == user!.RoleId);
        user.Role = role;
        //tao log cho user dang ky
        await logFireStore.AddLogForUser(user, "dang-nhap");
        return user;
    }

    public async Task<User> UpdateStatus(UpdateStatusUserDto dto)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        User user = null!;
        if (dto.Email == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == dto.Phone)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == dto.Email)!;
        }

        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"Status" , dto.Status}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        return user;

    }

    //method address
    public async Task<string> AddAdress(CreateAddressDto dto)
    {
        var db = _firestoreDb.Collection(_collectionAddress);
        var userDb = base.GetSnapshots(_collectionUser);
        var addressDb = base.GetSnapshots(_collectionAddress);
        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == dto.Username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == dto.Username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == dto.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var address = createAddressConverter.ToEntity(dto);
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (addressDb.Documents.Select(r => r.ConvertTo<Address>()).ToList().Find(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        address.Code = randomCode;
        await db.AddAsync(address);
        await AddAddressUser(randomCode, user.Id!);
        return "";
    }
    public Task<List<AddressDto>> GetAddress(string username)
    {
        var address_UserDb = base.GetSnapshots(_collectionAddress_User);
        var addressDb = base.GetSnapshots(_collectionAddress);
        var userDb = base.GetSnapshots(_collectionUser);
        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var addressesUser = address_UserDb.Documents.Select(r => r.ConvertTo<Address_User>()).ToList().FindAll(r => r.UserId == user.Id);
        var addressesDto = new List<AddressDto>();
        foreach (var address_User in addressesUser)
        {
            var address = addressDb.Documents.Select(r => r.ConvertTo<Address>()).ToList().Find(r => r.Id == address_User.AddressId);
            addressesDto.Add(addressConverter.ToDto(address!));
        }
        return Task.FromResult(addressesDto);
    }

    //method ho tro
    public bool CheckIsVerified(string email)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        if (user == null) return false;
        return user.VerifiedAt.ToDateTime().Year != 1111;
    }

    public bool CheckValidToken(string email, string token)
    {
        var userDb = base.GetSnapshots(_collectionUser);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == email);
        return user!.VerificationToken == token;
    }

    private async void CreateCartForUser(User user)
    {
        var cartDb = _firestoreDb.Collection(CartFireStore._collectionCart);
        var userDb = base.GetSnapshots(_collectionUser);
        User u = null!;
        if (user.Email == null)
        {
            u = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == user.Phone)!;
        }
        else
        {
            u = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == user.Email)!;
        }

        var cart = new Cart
        {
            UserId = u.Id,
            TotalPrice = 0,
            Items = new List<CartItem>()
        };
        await cartDb.AddAsync(cart);
    }

    private async Task AddAddressUser(string code, string userId)
    {
        var address_UserDb = _firestoreDb.Collection(_collectionAddress_User);
        var addressDb = base.GetSnapshots(_collectionAddress);

        var addressExists = addressDb.Documents.Select(r => r.ConvertTo<Address>()).ToList().Find(r => r.Code == code)!;
        var address_User = new Address_User
        {
            UserId = userId,
            AddressId = addressExists.Id
        };
        await address_UserDb.AddAsync(address_User);
    }

    public async Task<User> GenRefreshToken(string username)
    {
        User user = GetUserByUsername(username);

        //tao refresh token
        DocumentReference docref = _firestoreDb.Collection(_collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"RefreshToken" , AuthServiceImpl.CreateRefreshToken(user)}
        };

        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }
        return user;
    }
}
