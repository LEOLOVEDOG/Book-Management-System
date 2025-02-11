using Book_Management_System_WebAPI.Models;

namespace Book_Management_System_WebAPI.Interfaces
{
    public interface IUserService
    {
        Task <bool> RegisterAsync(string username, string password, string email);
        Task <User?> LoginAsync(string username, string password, bool remeberme);
    }
}
