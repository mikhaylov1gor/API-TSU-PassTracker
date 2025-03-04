using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_TSU_PassTracker.Models.DTO
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public bool IsConfirmed { get; set; } = false;

        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        [MinLength(1, ErrorMessage = "Имя должно содержать хотя бы один символ.")]
        [MaxLength(100, ErrorMessage = "Имя не должно превышать 100 символов.")]
        public string Name { get; set; }
        public string Group { get; set; }

        [Required(ErrorMessage = "Роль обязательна для заполнения.")]
        public List<Role>? Roles { get; set; }
    }
}