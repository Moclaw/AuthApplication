using AuthApplication.Models;
using AuthApplication.Models.Entity;
using AuthApplication.Models.PostModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserServices _userServices;
    private readonly ILogger<UserController> _logger;
    public UserController(ILogger<UserController> logger, UserServices userServices)
    {
        _userServices = userServices;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserPostModel model)
    {
        try
        {
            var checkUser = _userServices.CheckUser(model);
            if (checkUser != RegisterStatus.Success)
                return BadRequest(new DefaultRespones { Message = $"{checkUser.ToString()}" });

            var resutl = await _userServices.RegisterUser(model);
            if (resutl is null)
            {
                return BadRequest(new DefaultRespones { Message = "Something went wrong" });
            }
            return Ok(new DefaultRespones { Message = "Register Successfully", Data = resutl});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
			return BadRequest(new DefaultRespones { Message = "Something went wrong" });
        }
		
    }

    [HttpPost("login")]
    public IActionResult Login(LoginModel user)
    {
        var result = _userServices.LoginUser(user);

        if(result is null)
			return BadRequest(new DefaultRespones { Message = "Login failure" });

        return Ok(new DefaultRespones { Data = result, Message = "Login successfully" });
    }
	[Authorize]
	[HttpGet("getall")]
	public IActionResult GetAll()
	{
		var users = _userServices.GetAll();
		return Ok(new DefaultRespones { Data = users, Message = "Get all users successfully" });
	}

	[Authorize]
	[HttpGet("logout")]
	public IActionResult Logout()
	{
        _logger.LogError(null, string.Empty);
        return Ok(new DefaultRespones { Message = "Logout successfully" });
	}

}
