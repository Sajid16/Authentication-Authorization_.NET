
using Authentication_Authorization.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication_Authorization.Services
{
    public class RoleAccessPolicyService(AuthECAPIContext context) : IRoleAccessPolicyService
    {
        public async Task<(List<string> roles, bool isValid)> GetAllowedRolesAsync(string policyKey)
        {
            RoleAccessPolicy policyKeyDetails = await context.RoleAccessPolicies.FirstOrDefaultAsync();
            
            var roles = string.Concat(policyKeyDetails?.AllowedRoles + "," + policyKeyDetails?.PostSwitchAllowedRoles);
            var parsedRoles = roles?.Split(',').Select(r => r.Trim()).ToList() ?? new List<string>();

            return (parsedRoles, true);
        }
    }
}
