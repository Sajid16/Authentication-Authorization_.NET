using Authentication_Authorization.Entities;
using Authentication_Authorization.Utilities;
using Authentication_Authorization.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication_Authorization.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly AuthECAPIContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationServices(AuthECAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiReturnObj<List<UserVM>>> GetUsers()
        {
            ApiReturnObj<List<UserVM>> apiReturnObj = new ApiReturnObj<List<UserVM>>();
            try
            {
                var users = await _context.Users.Select(users=> new UserVM {
                    UserName = users.UserName,
                    CreatedAt = users.CreatedAt,
                    Email = users.Email,
                    Id = users.Id,
                    Password = users.Password,
                    PhoneNumber = users.PhoneNumber,
                    Role = new RoleDetails 
                    {
                        RoleId = users.RoleId,
                        RoleName = users.Role.RoleName
                    }
                }).ToListAsync();
                if (users == null)
                {
                    apiReturnObj.Message = "Users does not exist.";
                    apiReturnObj.IsSuccess = false;
                    return apiReturnObj;
                }

                apiReturnObj.Message = "successfully retrieved.";
                apiReturnObj.IsSuccess = true;
                apiReturnObj.Response = users;
                return apiReturnObj;
            }
            catch (Exception ex)
            {
                apiReturnObj.Message = ex.Message.ToString() + "-" + ex.InnerException?.Message.ToString();
                apiReturnObj.IsSuccess = false;
                return apiReturnObj;
            }
        }

        public async Task<ApiReturnObj<LogInResponseVM>> LogIn(LogInRequestVM logInRequestVM)
        {
            ApiReturnObj<LogInResponseVM> apiReturnObj = new ApiReturnObj<LogInResponseVM>();
            try
            {
                var user = await _context.Users.Include(user=>user.Role).FirstOrDefaultAsync(user=> user.UserName == logInRequestVM.UserName);
                if(user is null)
                {
                    apiReturnObj.Message = "User not found.";
                    apiReturnObj.IsSuccess = false;
                    return apiReturnObj;
                }

                bool isPasswordVerified = BCrypt.Net.BCrypt.Verify(logInRequestVM.Password, user.Password);
                if (!isPasswordVerified)
                {
                    apiReturnObj.Message = "Invalid credentials.";
                    apiReturnObj.IsSuccess = false;
                    return apiReturnObj;
                }

                byte[] secretKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWTKey:SecretKey"));
                var securityKey = new SymmetricSecurityKey(secretKey);
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.Aes128CbcHmacSha256);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("username", user.UserName),
                        new Claim("role", user.Role.RoleName)
                    }),
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = signingCredentials,

                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                LogInResponseVM logInResponseVM = new LogInResponseVM
                {
                    Token = tokenHandler.WriteToken(token)
                };

                apiReturnObj.Message = "successfully retrieved.";
                apiReturnObj.IsSuccess = true;
                apiReturnObj.Response = logInResponseVM;
                return apiReturnObj;
            }
            catch (Exception ex)
            {
                apiReturnObj.Message = ex.Message.ToString() + "-" + ex.InnerException?.Message.ToString();
                apiReturnObj.IsSuccess = false;
                return apiReturnObj;
            }
        }

        public async Task<ApiReturnObj<object>> RegisterUsers(RegisteruserVM registeruserVM)
        {
			try
			{
                ApiReturnObj<object> apiReturnObj = new ApiReturnObj<object>();

                var getUser = await _context.Users.FirstOrDefaultAsync(user=> user.Email == registeruserVM.Email);
                if (getUser != null)
                {
                    apiReturnObj.Message = "User exists.";
                    apiReturnObj.IsSuccess = false;
                    return apiReturnObj;
                }

                User user = new User
                {
                    UserName = registeruserVM.UserName,
                    Email = registeruserVM.Email,
                    PhoneNumber = registeruserVM.PhoneNumber,
                    Password = BCrypt.Net.BCrypt.HashPassword(registeruserVM.Password),
                    RoleId = registeruserVM.RoleId,
                    CreatedAt = DateTime.Now,
                };

                await _context.AddAsync(user);
                await _context.SaveChangesAsync();

                apiReturnObj.Message = "successfully registered.";
                apiReturnObj.IsSuccess = true;
                return apiReturnObj;
			}
			catch (Exception ex)
			{
                ApiReturnObj<object> apiReturnObj = new ApiReturnObj<object>();
                apiReturnObj.Message = ex.Message.ToString()+"-"+ex.InnerException?.Message.ToString();
                apiReturnObj.IsSuccess = false;
                return apiReturnObj;
            }
        }
    }
}
