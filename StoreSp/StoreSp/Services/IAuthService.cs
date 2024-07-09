using StoreSp.Entities;

namespace StoreSp.Services;

public interface IAuthService
{
    public string GenerateToken(User user);
    public string GetEmailByToken(string token);
    public bool ValidateToken(string token);
}
