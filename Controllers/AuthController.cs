using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using testprojekt.Data;
using testprojekt.DTO;
using testprojekt.Models;
using testprojekt.Services;

namespace testprojekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Проверка минимальной длины пароля
            if (string.IsNullOrWhiteSpace(userDto.Password) || userDto.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Пароль должен содержать минимум 6 символов.");
                return BadRequest(ModelState);
            }

            // Проверка уникальности email
            if (_context.Users.Any(u => u.Email.ToLower() == userDto.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "Пользователь с таким Email уже существует.");
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Role = userDto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Регистрация успешна.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == loginDto.Email.ToLower());
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                ModelState.AddModelError("Login", "Неверный Email или пароль.");
                return Unauthorized(ModelState);
            }

            // Генерация JWT токена
            var token = _jwtService.GenerateToken(user.Email, user.Role);

            // Настройка Claims для куки-аутентификации
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Выполняем вход пользователя через куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { token });
        }
    }

    // DTO для входа
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

