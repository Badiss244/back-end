using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Rapport
{
    public class RapportDTO
    {
        public Guid Id { get; set; }
        public string? AuditorName { get; set; }
        public string? Factory { get; set; }
        public string Description { get; set; } = "Empty Description";
        public DateTime? CreatedDate { get; set; }
        public List<string>? Pictures { get; set; }
        public List<ScoreCritereDTO> Scores { get; set; } = new List<ScoreCritereDTO>();
    }
}
