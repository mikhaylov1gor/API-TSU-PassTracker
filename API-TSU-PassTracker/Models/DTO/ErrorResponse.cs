using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TSU_PassTracker.Models.DTO
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }

        public ErrorResponse(int status, string message, string? details = null)
        {
            Status = status;
            Message = message;
            Details = details;
        }
    }
}