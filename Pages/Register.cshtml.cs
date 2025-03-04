using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using testprojekt.DTO;

namespace testprojekt.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public UserDto User { get; set; } = new UserDto();

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            using (var client = new HttpClient())
            {
                var json = JsonSerializer.Serialize(User);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{baseUrl}/api/auth/register", content);
                if (response.IsSuccessStatusCode)
                {
                    Message = "Регистрация успешна!";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Ошибка регистрации: {errorContent}";
                }
            }
            return Page();
        }
    }
}
