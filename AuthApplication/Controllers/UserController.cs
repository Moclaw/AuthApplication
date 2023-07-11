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
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
        _userServices = new UserServices(configuration);
    }

    [HttpPost("register")]
    public IActionResult Register(UserPostModel model)
    {
		var checkUser = _userServices.CheckUser(model);
        if (checkUser != AuthApplication.Models.RegisterStatus.Success)
			return BadRequest(new DefaultRespones  { Message = $"{checkUser.ToString()}" });

        _userServices.RegisterUser(model);
        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginModel user)
    {
        var result = _userServices.LoginUser(user);

        if(result is null)
            return BadRequest(new DefaultRespones { Data = string.Empty, Message = "Somethingwentwrong" });

        return Ok(new DefaultRespones { Data = result, Message = "Login successfully" });
    }
}
