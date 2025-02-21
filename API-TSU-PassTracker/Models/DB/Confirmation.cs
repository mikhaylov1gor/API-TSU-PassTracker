using System;
using System.ComponentModel.DataAnnotations;

namespace API_TSU_PassTracker.Models.DB
{
    public class Confirmation
    {
        public Guid Id { get; set; }

        [MaxLength(255, ErrorMessage = "Название файла не должно превышать 255 символов.")]
        public string? FileName { get; set; }

        [MaxLength(50, ErrorMessage = "MIME-тип не должен превышать 50 символов.")]
        public string? FileType { get; set; }

        public byte[]? FileContent { get; set; }

        public long? FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Guid RequestId { get; set; }

        public Request Request { get; set; }
    }
}