using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Audit;

namespace Domain.DTOs.Auditor
{
    public class AuditStatsDTO
    {
        public int TotalAudits { get; set; }
        public int CompletedAudits { get; set; }
        public int CanceledAudits { get; set; }
        public int InProgressAudits { get; set; }
        public int TotalEvidence { get; set; }
        public int Rapports { get; set; }
        public List<DatePlanifierDTO> Dates { get; set; } = new List<DatePlanifierDTO>();
    }
}
