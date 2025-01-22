using Microsoft.AspNetCore.Identity.UI.Services;

namespace Book_Management_System
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // 模擬電子郵件發送（實際上不執行任何操作）
            Console.WriteLine($"SendEmailAsync called: To={email}, Subject={subject}");
            return Task.CompletedTask;
        }
    }
}