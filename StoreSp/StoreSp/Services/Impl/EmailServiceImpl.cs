using System.Net;
using System.Net.Mail;
using StoreSp.Dtos.response;

namespace StoreSp.Services.Impl;


public class EmailServiceImpl : IEmailService
{
    public void SendEmail(EmailDto dto)
    {
        var pw = "ejmmbowgwhkugsob";
        var fromAddress = new MailAddress("speed.project.ecommerce@gmail.com");
        var toAddress = new MailAddress(dto.Email);

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = fromAddress;
        mailMessage.Subject = dto.Subject;
        mailMessage.To.Add(toAddress);
        mailMessage.Body = dto.Message;
        mailMessage.IsBodyHtml = true;

        var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(fromAddress.Address, pw)
        };

        smtp.Send(mailMessage);
    }
}