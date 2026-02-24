using CryptoProj.Domain.Services.Users;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CryptoProj.API.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;


    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromRoute] int id)
    {
        var user = await _usersService.GetById(id);
        return Ok(user);
    }
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] string IdToken)
    {
        var jwt = await _usersService.AuthenticateWithGoogle(IdToken);
        if (jwt == null)
        {
            return Unauthorized();
        }
        return Ok(new { Token = jwt });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var user = await _usersService.Register(request);
        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var user = await _usersService.Login(request);
        return Ok(user);
    }
}