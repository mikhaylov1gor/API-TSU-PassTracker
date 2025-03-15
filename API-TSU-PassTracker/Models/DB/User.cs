using System;
using System.ComponentModel.DataAnnotations;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace API_TSU_PassTracker.Models.DB
{
    public class User
    {
        public Guid Id { get; set; }

        public bool IsConfirmed { get; set; }

        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        [MinLength(1, ErrorMessage = "Имя должно содержать хотя бы один символ.")]
        [MaxLength(100, ErrorMessage = "Имя не должно превышать 100 символов.")]
        public string Name { get; set; }
        public string Group { get; set; }

        [Required(ErrorMessage = "Роль обязательна для заполнения.")]
        public List<Role>? Roles { get; set; }

        [Required(ErrorMessage = "Логин обязателен для заполнения.")]
        [MinLength(6, ErrorMessage = "Минимальная длина логина 6 символов")]

        public string Login { get; set; }
        public string PasswordHash { get; set; }

        public string Salt {  get; set; }

        public List<Request>? Requests { get; set; }
    }
}