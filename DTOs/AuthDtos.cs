using Web_WhaleBooking.Models;

namespace Web_WhaleBooking.DTOs;

public class LoginRequest
{
    public string? IdToken { get; set; }
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public LoginData? Data { get; set; }
}

public class LoginData
{
    public UserDto User { get; set; } = new();
    public string[] Roles { get; set; } = Array.Empty<string>();
    public string Token { get; set; } = string.Empty; // JWT_BACKEND
    public string AccessToken { get; set; } = string.Empty; // alias of Token
    public string[] Permissions { get; set; } = Array.Empty<string>();
}

public class UserDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? HoTen { get; set; }
    public string VaiTro { get; set; } = UserRole.KhachHang.ToString();
}

public class UpdateRoleRequest
{
    public string VaiTro { get; set; } = string.Empty;
}
