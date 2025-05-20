using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Tache;
using Domain.Models;

namespace Domain.DTOs.PlanAction
{
    public class CreatePlanActionDTO
    {
        public string Name { get; set; }
        public Guid IdFactory { get; set; }
        public Guid IdRapport { get; set; }
        public List<CreateTacheDTO>? Taches { get; set; }
    }
}
