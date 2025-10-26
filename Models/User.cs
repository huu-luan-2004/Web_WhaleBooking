using System;

namespace Web_WhaleBooking.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? HoTen { get; set; }
    public UserRole VaiTro { get; set; } = UserRole.KhachHang;

    // Firebase UID (subject)
    public string ProviderId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
