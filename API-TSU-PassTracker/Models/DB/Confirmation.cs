using System;
using System.ComponentModel.DataAnnotations;

namespace API_TSU_PassTracker.Models.DB
{
    public class Confirmation
    {
        public Guid Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public byte[] FileData { get; set; }
    }
}