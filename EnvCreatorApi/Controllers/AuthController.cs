using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EnvCreatorApi.Models;

namespace EnvCreatorApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var user = new User
        {
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
            return Ok("User registered successfully");

        return BadRequest(result.Errors);
    }
}
