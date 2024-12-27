using Authentication_Authorization.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Authentication_Authorization.Attributes
{
    public class CustomAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly List<string> _allowedRoles;

        public CustomAuthorizationAttribute(params string[] allowedRoles)
        {
            _allowedRoles = new List<string>(allowedRoles);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
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
                    if (_allowedRoles.Any() && !_allowedRoles.Contains(userRole))
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
