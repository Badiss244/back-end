using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Rapport
{
    public class ScoreCritereDTO
    {
        public Guid CritereId { get; set; }
        public string? Name { get; set; }
        public float? Score { get; set; }
        public string? SName { get; set; }
    }
}
