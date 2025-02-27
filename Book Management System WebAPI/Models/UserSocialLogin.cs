namespace Book_Management_System_WebAPI.Models
{
    public class UserSocialLogin
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; }  // Google, Facebook, GitHub
        public string ProviderId { get; set; }  // 第三方提供的 ID

        // 導航屬性
        public User User { get; set; }  
    }
}
