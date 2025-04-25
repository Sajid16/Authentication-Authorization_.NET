namespace Authentication_Authorization.Entities
{
    public class RoleAccessPolicy
    {
        public string PolicyKey { get; set; }
        public string AllowedRoles { get; set; }
        public DateTime? SwitchTimeUtc { get; set; }
        public string PostSwitchAllowedRoles { get; set; }
    }
}
