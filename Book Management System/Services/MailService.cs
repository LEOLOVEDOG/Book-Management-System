using Book_Management_System.Interfaces;
using Book_Management_System.Models;
using Microsoft.Extensions.Options;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace Book_Management_System.Services
{
    public class MailService : IEmailSenderService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailiAsync(MailRequest mailRequest, string emailType)
        {
            // 寄/發送人的資訊
            var email = new MimeMessage();
            // 根據 emailType 設定不同發件人
            string fromEmail = _mailSettings.Accounts.ContainsKey(emailType)
                               ? _mailSettings.Accounts[emailType]
                               : _mailSettings.Sender; // 預設使用 Sender
            email.From.Add(MailboxAddress.Parse(fromEmail));
            email.Sender = MailboxAddress.Parse(_mailSettings.Sender);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject; // 主題
                                                 //=============================================================
                                                 //發送內容
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null) // 事處理檔案的部分
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body; // 郵件訊息內容
            email.Body = builder.ToMessageBody();
            //=============================================================
            //smtp的寄送方式(使用appsetting.json的資訊)
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Sender, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

    }
}
