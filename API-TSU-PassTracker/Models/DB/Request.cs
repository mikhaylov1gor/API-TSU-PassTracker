using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TSU_PassTracker.Models.DB
{
    public class Request
{
    public Guid Id { get; set; }

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public List<Confirmation>? Confirmations { get; set; }
}
}