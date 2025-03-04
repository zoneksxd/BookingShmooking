using System.ComponentModel.DataAnnotations;

namespace testprojekt.DTO
{
    public class UserDto
    {
        [Required(ErrorMessage = "Имя обязательно.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Укажите роль.")]
        public string Role { get; set; }
    }
}
