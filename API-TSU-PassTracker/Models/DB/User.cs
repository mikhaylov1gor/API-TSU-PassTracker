using System;
using System.ComponentModel.DataAnnotations;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace API_TSU_PassTracker.Models.DB
{
    public class User
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        [MinLength(1, ErrorMessage = "Имя должно содержать хотя бы один символ.")]
        [MaxLength(100, ErrorMessage = "Имя не должно превышать 100 символов.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Роль обязательна для заполнения.")]
        public Role Roles { get; set; }

        [Required(ErrorMessage = "Логин обязателен для заполнения.")]
        [MinLength(1, ErrorMessage = "Логин должен содержать хотя бы один символ.")]
        [MaxLength(50, ErrorMessage = "Логин не должен превышать 50 символов.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$",
            ErrorMessage = "Логин может содержать только буквы, цифры, подчеркивания и дефисы.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Хэш пароля обязателен для заполнения.")]
        public string PasswordHash { get; set; }

        public List<Request>? Requests { get; set; }
    }
}