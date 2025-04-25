using Authentication_Authorization.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Authentication_Authorization.Utilities;
using System.IdentityModel.Tokens.Jwt;

namespace Authentication_Authorization.Attributes
{
    public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _policyKey;

        public CustomAuthorizeAttribute(string policyKey)
        {
            _policyKey = policyKey;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var policyService = context.HttpContext.RequestServices.GetService<IRoleAccessPolicyService>();
            var (roles, found) = await policyService.GetAllowedRolesAsync(_policyKey);

            if (!found)
            {
                context.Result = new ForbidResult(); // Or maybe return NotFound?
                return;
            }
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authorizationHeader is not null && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                // Validate token
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);

                    // Check expiration
                    if (jwtToken.ValidTo.AddHours(6) < DateTime.Now)
                    {
                        // Token is expired
                        context.Result = new UnauthorizedResult();
                    }

                    // Token is valid; you can add further claims validation here if needed
                    var userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                    //var isAuthorized = roles.Any(role => userRole(role));
                    bool isAuthorized = roles.Any(r => string.Equals(r, userRole, StringComparison.OrdinalIgnoreCase));
                    //var isAuthorized = true;

                    if (!isAuthorized)
                    {
                        context.Result = new ObjectResult(new ApiReturnObj<object>()
                        {
                            IsSuccess = false,
                            Message = "Don't have necessary permission."
                        })
                        {
                            StatusCode = StatusCodes.Status403Forbidden // Custom Forbidden status code
                        };
                    }
                    //if (_allowedRoles.Any() && !_allowedRoles.Contains(userRole))
                    //{
                    //    context.Result = new ObjectResult(new ApiReturnObj<object>()
                    //    {
                    //        IsSuccess = false,
                    //        Message = "Don't have necessary permission."
                    //    })
                    //    {
                    //        StatusCode = StatusCodes.Status403Forbidden // Custom Forbidden status code
                    //    };
                    //}
                }
                else
                {
                    // Invalid token
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                // No token provided
                context.Result = new UnauthorizedResult();
            }

            await Task.CompletedTask;
            
        }
    }

}
