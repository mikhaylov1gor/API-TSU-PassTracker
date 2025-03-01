using System.ComponentModel.DataAnnotations;

namespace API_TSU_PassTracker.Models.DTO
{
    public class TokenResponseModel
    {
        [MinLength(1)]
        public string token { get; set; }
    }
}
