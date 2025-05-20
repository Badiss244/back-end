using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Tache;
using Domain.Models;

namespace Domain.DTOs.PlanAction
{
    public class PlanActionDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string factory {  get; set; }
        public bool IsDone { get; set; }
        public List<TacheDTO> taches { get; set; } = new List<TacheDTO>();
    }
}
