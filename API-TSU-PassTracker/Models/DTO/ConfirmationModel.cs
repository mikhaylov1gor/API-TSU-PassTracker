using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace API_TSU_PassTracker.Models.DTO;

public class ConfirmationModel
{
        
        public ConfirmationType ConfirmationType { get; set; } 
        public List<IFormFile>? Files { get; set; }
}


