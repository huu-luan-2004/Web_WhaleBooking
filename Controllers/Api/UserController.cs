using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_WhaleBooking.DTOs;
using Web_WhaleBooking.Models;
using Web_WhaleBooking.Services;

namespace Web_WhaleBooking.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserStore _users;

    public UserController(IUserStore users)
    {
        _users = users;
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var id))
        {
            return Unauthorized(new { success = false, message = "Invalid token" });
        }

        var user = _users.GetById(id);
        if (user == null) return NotFound(new { success = false, message = "User not found" });

        return Ok(new
        {
            success = true,
            data = new
            {
                id = user.Id,
                email = user.Email,
                hoTen = user.HoTen,
                vaiTro = user.VaiTro.ToString(),
                roles = new[] { user.VaiTro.ToString() }
            }
        });
    }

    [HttpPatch("{id:int}/role")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public ActionResult<object> UpdateRole([FromRoute] int id, [FromBody] UpdateRoleRequest body)
    {
        if (!Enum.TryParse<UserRole>(body.VaiTro, ignoreCase: true, out var role))
        {
            return BadRequest(new { success = false, message = "vaiTro invalid. Use KhachHang | ChuCoSo | Admin" });
        }

        var ok = _users.TryUpdateRole(id, role);
        if (!ok) return NotFound(new { success = false, message = "User not found" });

        return Ok(new { success = true });
    }
}
