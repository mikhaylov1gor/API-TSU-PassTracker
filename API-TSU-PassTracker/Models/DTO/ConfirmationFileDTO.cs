using System;
using System.Buffers.Text;

namespace API_TSU_PassTracker.Models.DTO;

public class ConfirmationFileDTO
{
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileBase64{ get; set; }
        
}
