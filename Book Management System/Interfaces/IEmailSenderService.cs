using Book_Management_System.Models;

namespace Book_Management_System.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailiAsync(MailRequest mailRequest, string emailType);
    }
}
