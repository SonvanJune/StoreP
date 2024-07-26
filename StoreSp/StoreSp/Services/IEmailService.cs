using StoreSp.Dtos.response;

namespace StoreSp.Services;

public interface IEmailService
{
    public void SendEmail(EmailDto dto);
}
