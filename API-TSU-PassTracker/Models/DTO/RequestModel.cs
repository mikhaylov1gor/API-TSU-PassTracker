using API_TSU_PassTracker.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_TSU_PassTracker.Models.DTO
{
    public class RequestModel
    {
        public Guid Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public List<Confirmation>? Confirmations { get; set; }
    }
}
