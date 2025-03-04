using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using testprojekt.Data;

namespace testprojekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            // Получаем email из токенов
            var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized();
            }
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }
            // Формируем профиль без пароля
            var profile = new
            {
                user.Name,
                user.Email,
                user.Role
            };
            return Ok(profile);
        }
    }
}
