﻿#nullable disable

namespace Authentication_Authorization.Entities
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Role Role { get; set; }
    }
}
