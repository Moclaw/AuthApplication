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

public class UserServices
{
    private List<User> _users;
    private readonly string _filePath;
    private readonly IConfiguration _configuration;

    public UserServices(IConfiguration configuration)
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        LoadUsers();
        _configuration = configuration;
    }
    private void LoadUsers()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var usersWrapper = JsonSerializer.Deserialize<UsersWrapper>(json);
                _users = usersWrapper?.Users ?? new List<User>();
            }
            catch (Exception)
            {
                _users = new List<User>();
                throw;
            }
        }
        else
        {
            _users = new List<User>();
        }
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }

    private void SaveUsers()
    {
        var usersWrapper = new UsersWrapper { Users = _users };
        var json = JsonSerializer.Serialize(usersWrapper);
        File.WriteAllText(_filePath, json);
    }

    public void RegisterUser(UserPostModel user)
    {
        var encryptedPassword = Crypto.Encrypt(user?.Password);
        var id = _users?.Count ?? 0;
        _users.Add(new User
        {
            Id = id++,
            Email = user.Email,
            UserName = user.Username,
            Password = encryptedPassword,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        });
        SaveUsers();
    }

    public LoginResponse? LoginUser(LoginModel model)
    {
        if (model.Password is null || model.UserName is null) return null;

        var encryptedPassword = Crypto.Encrypt(model.Password);
        var user = _users.FirstOrDefault(x => x.UserName == model.UserName && x.Password == encryptedPassword);

        if (user == null)
        {
            return null;
        }
		var token = GetToken(user);
        return new LoginResponse
		{
			Token = token,
			User = new User
			{
				Email = user.Email,
				UserName = user.UserName,
				PhoneNumber = user.PhoneNumber,
				Address = user.Address
			}
		};
    }

    public RegisterStatus CheckUser(UserPostModel user)
    {
        if (_users.Any(x => x.UserName == user.Username))
        {
            return RegisterStatus.UserIsExist;
        }

        if (!Regex.IsMatch(user?.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
        {
            return RegisterStatus.EmailIsNotValid;
        }

        if (!Regex.IsMatch(user?.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$"))
        {
            return RegisterStatus.PasswordIsNotValid;
        }

        if (user?.Password != user?.PasswordConfirm)
        {
            return RegisterStatus.PasswordConfirmIsNotMatch;
        }

        if (!Regex.IsMatch(user?.PhoneNumber, @"^(0[1-9])+([0-9]{8})$"))
        {
            return RegisterStatus.PhoneIsNotValid;
        }

        if (!Regex.IsMatch(user?.Username, @"^[a-zA-Z0-9]{5,}$"))
        {
            return RegisterStatus.UserNameIsNotValid;
        }

        return RegisterStatus.Success;
    }

    public string GetToken(User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, user?.UserName),
				new Claim(ClaimTypes.Email, user?.Email),
				new Claim(ClaimTypes.MobilePhone, user?.PhoneNumber)
			}),
			Expires = DateTime.UtcNow.AddDays(7),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

    public class UsersWrapper
    {
        public List<User> Users { get; set; }
    }

    public static class Crypto
    {
        public static string Encrypt(string password)
        {
            if (password == null)
                return string.Empty;
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
				StringBuilder builder = new StringBuilder();
				foreach (byte b in bytes)
				{
					builder.Append(b.ToString("x2"));
				}
				password = builder.ToString();
			}
            return password;
        }
    }
}
