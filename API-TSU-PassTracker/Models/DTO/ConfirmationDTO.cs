using System;

namespace API_TSU_PassTracker.Models.DTO;

public class ConfirmationDTO
{
    public Guid Id { get; set; }
    public ConfirmationType ConfirmationType {get; set;}
    public List<ConfirmationFileDTO> Files {get; set;}

}
