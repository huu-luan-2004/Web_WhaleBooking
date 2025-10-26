using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web_WhaleBooking.Models;

namespace Web_WhaleBooking.Services;

public class JwtOptions
{
    public string Issuer { get; set; } = "whalebooking";
    public string Audience { get; set; } = "whalebooking-clients";
    public string Key { get; set; } = "SUPER_SECRET_KEY_CHANGE_ME";
    public int ExpiresMinutes { get; set; } = 120;
}

public interface ITokenService
{
    string CreateToken(User user);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly byte[] _keyBytes;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _keyBytes = Encoding.UTF8.GetBytes(_options.Key);
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email ?? user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.VaiTro.ToString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(_keyBytes), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
