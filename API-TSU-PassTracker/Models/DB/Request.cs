// Request.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API_TSU_PassTracker.Models.DTO;

namespace API_TSU_PassTracker.Models.DB
{
    public class Request
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        
        [Required]
        public ConfirmationType ConfirmationType { get; set; }
        public byte[][]? Files { get; set; }
    }
}

