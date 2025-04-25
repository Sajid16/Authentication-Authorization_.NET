namespace Authentication_Authorization.Services
{
    public interface IRoleAccessPolicyService
    {
        Task<(List<string> roles, bool isValid)> GetAllowedRolesAsync(string policyKey);
    }

}
