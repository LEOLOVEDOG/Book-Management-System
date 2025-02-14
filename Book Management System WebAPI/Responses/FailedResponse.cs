namespace Book_Management_System_WebAPI.Responses
{
    // 登入 註冊 返回錯誤訊息
    public class FailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
