using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using testprojekt.Data;
using testprojekt.Models;

namespace testprojekt.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginDto Login { get; set; } = new LoginDto();

        public string Message { get; set; }

        public void OnGet()
        {
            // Если вы хотите, чтобы страница входа всегда отображалась, 
            // удалите или закомментируйте следующий код перенаправления:
            // if (User.Identity != null && User.Identity.IsAuthenticated)
            // {
            //     Response.Redirect("/UserProfile");
            // }
        }   // 

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Поиск пользователя без учета регистра
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == Login.Email.ToLower());
            if (user == null || !BCrypt.Net.BCrypt.Verify(Login.Password, user.PasswordHash))
            {
                ModelState.AddModelError("Login", "Неверный Email или пароль.");
                return Page();
            }

            // Формируем список claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Параметры аутентификации (куки сохраняются между сессиями)
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            // Создаем куки для аутентификации
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // После успешного входа перенаправляем пользователя на страницу профиля
            return RedirectToPage("/UserProfile");
        }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
