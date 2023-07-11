using AuthApplication.Models;
using AuthApplication.Models.Entity;
using AuthApplication.Models.PostModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AuthApplication.Models.Respones;
using AuthApplication.Services;

public class UserServices : BaseServices<User>
{
    private readonly List<User> _users = new List<User>();
    private readonly string _screctKey = string.Empty;
    private readonly string _encryptKey = string.Empty;
    public UserServices(ApplicationDbContext context, IConfiguration configuration, ILogger<User> logger) : base(context, configuration, logger)
    {
        _users = db.Users.ToList();
        _screctKey = configuration["Jwt:SignKey"] ?? "";
        _encryptKey = configuration["SecrectKey"] ?? string.Empty;
    }
    public async Task<LoginResponse?> RegisterUser(UserPostModel user)
    {
        var password = CryptoHelper.Encrypt(user?.Password ?? "", _encryptKey);

        var newUser = new User
        {
            UserName = user?.Username,
            Email = user?.Email,
            PhoneNumber = user?.PhoneNumber,
            Password = password,
            Address = user?.Address,
            CreatedDate = DateTime.UtcNow,
        };
        if (await InsertAsync(newUser))
        {
            var userId = _users?.FirstOrDefault(c => c.UserName == user?.Username && c.Password == password)?.Id;
            var token = GetToken(userId?.ToString() ?? "");

            return new LoginResponse
            {
                Token = token,
                User = newUser
            };
        }
        return null;
    }

    public LoginResponse? LoginUser(LoginModel model)
    {
        var user = _users.SingleOrDefault(x => x.UserName == model?.UserName);
        var password = CryptoHelper.Decrypt(user?.Password ?? "", _encryptKey) == model.Password;
        if (user != null)
        {
            if (!password)
                return null;
            var token = GetToken(user?.Id.ToString() ?? "");
            return new LoginResponse
            {
                Token = token,
                User = user
            };
        }
        return null;
    }

    private string GetToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = null,            // Not required as no third-party is involved
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.AddDays(5),
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new Claim("userId", userId),
                new Claim("role", "User")
            }),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(System.Convert.FromBase64String(_screctKey)), SecurityAlgorithms.HmacSha256Signature)
        };
        var createdToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(createdToken);
    }

    public RegisterStatus CheckUser(UserPostModel user)
    {
        if (_users.Any(x => x.UserName == user.Username))
        {
            return RegisterStatus.UserIsExist;
        }

        if (!Regex.IsMatch(user?.Email ?? "", @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
        {
            return RegisterStatus.EmailIsNotValid;
        }

        if (!Regex.IsMatch(user?.Password ?? "", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$"))
        {
            return RegisterStatus.PasswordIsNotValid;
        }

        if (user?.Password != user?.PasswordConfirm)
        {
            return RegisterStatus.PasswordConfirmIsNotMatch;
        }

        if (!Regex.IsMatch(user?.PhoneNumber ?? "", @"^(0[1-9])+([0-9]{8})$"))
        {
            return RegisterStatus.PhoneIsNotValid;
        }

        if (!Regex.IsMatch(user?.Username ?? "", @"^[a-zA-Z0-9]{5,}$"))
            return RegisterStatus.UserNameIsNotValid;

        return RegisterStatus.Success;
    }


}
