using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API_TSU_PassTracker.Models.DB;

namespace API_TSU_PassTracker.Models.DTO
{
    public class RequestModel
    {
        [Required]
        public DateTime DateFrom { get; set; }
        
        [Required]
        public DateTime DateTo { get; set; }
        
        [Required]
        public ConfirmationType ConfirmationType { get; set; }
        
        public List<IFormFile>? Files { get; set; }
    }


}
