using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TSU_PassTracker.Models.DTO
{
    public class UserRoleUpdateModel
    {
        public Guid Id { get; set; }
        public List<Role> Roles { get; set; }
    }
}