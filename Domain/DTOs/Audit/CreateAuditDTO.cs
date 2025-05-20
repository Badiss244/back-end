using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Audit
{
    public class CreateAuditDTO
    {
        public DateOnly PlanDate { get; set; }
        public Guid FKfactory { get; set; }
    }
}
