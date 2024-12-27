using Authentication_Authorization.Entities;

namespace Authentication_Authorization.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public RoleDetails? Role { get; set; }
    }

    public class RoleDetails 
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
