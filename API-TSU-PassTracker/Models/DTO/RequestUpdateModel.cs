using System;
using API_TSU_PassTracker.Models.DB;

namespace API_TSU_PassTracker.Models.DTO;

public class RequestUpdateModel
{
    public Guid Id { get; set; }
    public DateTime DateTo { get; set; }
    public List<Confirmation>? Confirmations { get; set; }  
}
