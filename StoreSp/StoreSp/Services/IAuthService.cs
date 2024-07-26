using StoreSp.Entities;

namespace StoreSp.Services;

public interface IAuthService
{
    public bool ValidateToken(string token);
    public string GenerateToken(User user);
    public string GetFirstByToken(string token);
}
