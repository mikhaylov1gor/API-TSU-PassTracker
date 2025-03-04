using Microsoft.AspNetCore.SignalR;
using System;

namespace API_TSU_PassTracker.Models.DTO;

public class RequestDTO
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public RequestStatus Status { get; set; }
    public ConfirmationType ConfirmationType { get; set; }
    public List<string> Files { get; set; }
}


    
