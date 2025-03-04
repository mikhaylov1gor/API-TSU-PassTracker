using System.ComponentModel.DataAnnotations;

namespace API_TSU_PassTracker.Models.DTO
{
    public class LoginCredentialsModel
    {
        [MinLength(1)]
        public string login { get; set; }
        [MinLength(1)]
        public string password { get; set; }
    }
}
