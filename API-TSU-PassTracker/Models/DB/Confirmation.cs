using System;
using System.ComponentModel.DataAnnotations;
using API_TSU_PassTracker.Models.DTO;

namespace API_TSU_PassTracker.Models.DB
{
    public class Confirmation
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        [Required]
        public ConfirmationType ConfirmationType { get; set; }
        public List<ConfirmationFile>? Files { get; set; } = new List<ConfirmationFile>(); 
    }

    public class ConfirmationFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] FileData { get; set; }
        public Guid ConfirmationId { get; set; }
        public Confirmation Confirmation { get; set; }
    }
}