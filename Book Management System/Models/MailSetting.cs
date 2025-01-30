namespace Book_Management_System.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }  
        public string Subject { get; set; }  
        public string Body { get; set; }     
        public List<IFormFile> Attachments { get; set; } 
    }

    public class MailSettings
    {
        public string Sender { get; set; }
        public Dictionary<string, string> Accounts { get; set; } // 存放不同發件人
        public string DisplayName { get; set; }   
        public string Password { get; set; }      
        public string Host { get; set; }         
        public int Port { get; set; }             
    }

}
