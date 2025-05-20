using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Audit
{
    public class AuditDTO
    {
        public Guid IdAudit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateOnly PlanDate { get; set; }
        public string Status { get; set; } = "Underway";
        public string? Filiale {  get; set; }
        public string? Factory { get; set; }
    }
}
