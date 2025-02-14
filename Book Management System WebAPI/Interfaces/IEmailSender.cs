using Book_Management_System_WebAPI.Models;

namespace Book_Management_System_WebAPI.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailiAsync(MailRequest mailRequest);
    }
}
