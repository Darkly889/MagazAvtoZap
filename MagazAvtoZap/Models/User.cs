using System.ComponentModel.DataAnnotations;

namespace MagazAvtoZap.Models
{
    public class User
    {
        [Required(ErrorMessage = "Логин обязателен для заполнения")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        public string Password { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }
}
