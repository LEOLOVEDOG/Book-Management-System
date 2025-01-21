using Microsoft.AspNetCore.Identity;

namespace Book_Management_System.Models
{
    public class User : IdentityUser
    {
        public int OverdueCount { get; set; }

        public bool RememberMe { get; set; }

    }
}
