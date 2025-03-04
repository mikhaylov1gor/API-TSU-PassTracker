using System;
using API_TSU_PassTracker.Models.DB;

namespace API_TSU_PassTracker.Models.DTO;

public class RequestUpdateModel
    {
        public DateTime? DateFrom { get; set; } 
        public DateTime? DateTo { get; set; }  
        public List<IFormFile>? Files { get; set; }
    }
