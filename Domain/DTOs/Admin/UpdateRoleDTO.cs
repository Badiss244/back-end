using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class UpdateRoleDTO
    {
        public Guid Userid {  get; set; }
        public string NewRole { get; set; }

        public Guid? FactoryId { get; set; }
    }
}
