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
        public DateTime DateTo { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        
        [Required]
        public ConfirmationType ConfirmationType { get; set; }
        public List<RequestFile> Files { get; set; } = new();
    }

    public class RequestFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] FileData { get; set; }
        public Guid RequestId { get; set; }
        public Request Request { get; set; }
    }
}

