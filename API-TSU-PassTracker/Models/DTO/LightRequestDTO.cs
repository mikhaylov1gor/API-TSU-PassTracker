using System;

namespace API_TSU_PassTracker.Models.DTO;

public class LightRequestDTO
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string UserName { get; set; } 
    public ConfirmationType? ConfirmationType {get; set;}
    public RequestStatus Status { get; set; }
}
