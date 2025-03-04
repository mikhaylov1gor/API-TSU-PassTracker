using API_TSU_PassTracker.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API_TSU_PassTracker.Models.DTO
{
    public class UserRegisterModel
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        [MinLength(1, ErrorMessage = "Имя должно содержать хотя бы один символ.")]
        [MaxLength(100, ErrorMessage = "Имя не должно превышать 100 символов.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Логин обязателен для заполнения.")]
        [MinLength(1, ErrorMessage = "Логин должен содержать хотя бы один символ.")]
        [MaxLength(50, ErrorMessage = "Логин не должен превышать 50 символов.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$",
            ErrorMessage = "Логин может содержать только буквы, цифры, подчеркивания и дефисы.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "пароль обязателен для заполнения."), MinLength(6, ErrorMessage = "минимальная длина пароля 6 символов")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Номер группы обязателен для заполнения")]
        public string Group { get; set; }
    }
}
