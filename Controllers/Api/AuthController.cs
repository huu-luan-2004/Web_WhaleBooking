using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Web_WhaleBooking.DTOs;
using Web_WhaleBooking.Models;
using Web_WhaleBooking.Services;

namespace Web_WhaleBooking.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IFirebaseVerifier _firebase;
    private readonly IUserStore _users;
    private readonly ITokenService _tokens;
    private readonly IConfiguration _config;

    public AuthController(IFirebaseVerifier firebase, IUserStore users, ITokenService tokens, IConfiguration config)
    {
        _firebase = firebase;
        _users = users;
        _tokens = tokens;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest? body)
    {
        string? idToken = body?.IdToken;

        // Allow token in Authorization header: Bearer <FIREBASE_ID_TOKEN>
        if (string.IsNullOrWhiteSpace(idToken))
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                idToken = authHeader.Substring("Bearer ".Length).Trim();
        }

        if (string.IsNullOrWhiteSpace(idToken))
            return BadRequest(new LoginResponse { Success = false, Message = "Missing idToken" });

        try
        {
            var decoded = await _firebase.VerifyAsync(idToken);
            var email = decoded.Claims.TryGetValue("email", out var em) ? em?.ToString() : null;
            var name = decoded.Claims.TryGetValue("name", out var nm) ? nm?.ToString() : null;
            var uid = decoded.Uid;

            var user = _users.GetOrCreateByProvider(uid, email, name);

            // Auto-assign Admin for configured emails in Development/testing
            var adminEmails = _config.GetSection("Auth:AdminEmails").Get<string[]>() ?? Array.Empty<string>();
            if (!string.IsNullOrWhiteSpace(user.Email) && adminEmails.Contains(user.Email, StringComparer.OrdinalIgnoreCase))
            {
                user.VaiTro = UserRole.Admin;
            }
            var jwt = _tokens.CreateToken(user);

            var data = new LoginData
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    HoTen = user.HoTen,
                    VaiTro = user.VaiTro.ToString()
                },
                Roles = new[] { user.VaiTro.ToString() },
                Token = jwt,
                AccessToken = jwt,
                Permissions = Array.Empty<string>()
            };

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                Data = data
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new LoginResponse { Success = false, Message = $"Invalid Firebase token: {ex.Message}" });
        }
    }
}
