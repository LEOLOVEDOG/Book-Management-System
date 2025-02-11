using System.ComponentModel.DataAnnotations;

namespace Book_Management_System_WebAPI.Models
{
    public class Role
    {
        public Guid RoleId { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }

        // 導航屬性
        public virtual ICollection<User>? Users { get; set; }
    }
}
