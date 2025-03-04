using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using testprojekt.Data;
using testprojekt.DTO;

namespace testprojekt.Pages
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        private readonly AppDbContext _context;

        public UserDto UserProfile { get; set; }

        public UserProfileModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            // »звлекаем email из токенов текущего пользовател€
            var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                // ≈сли email не найден, можно добавить перенаправление или сообщение об ошибке
                return;
            }

            // ѕолучаем данные пользовател€ из базы данных
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                UserProfile = new UserDto
                {
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                    // Ќе передаем пароль
                };
            }
        }
    }
}
