using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Login and password are required.");
        }

        var user = await _userService.AuthenticateAsync(request.Login.Trim(), request.Password.Trim(), cancellationToken);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid login or password." });
        }

        return Ok(new LoginResponse(user.UserId, user.FullName, user.RoleName));
    }
}
