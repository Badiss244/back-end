using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Rapport
{
    public class CreateRapportDTO
    {
        public string Description { get; set; } = "Empty Description";
        public Guid IdFactory { get; set; }
        public List<string> Pictures { get; set; } = new List<string>(); // Initialize here
        public List<ScoreCritereDTO> Scores { get; set; } = new List<ScoreCritereDTO>();
    }
}
