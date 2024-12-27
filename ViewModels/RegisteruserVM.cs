namespace Authentication_Authorization.ViewModels
{
    public class RegisteruserVM
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required int RoleId { get; set; }
    }
}