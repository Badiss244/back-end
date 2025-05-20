using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Tache;
using Domain.Models;

namespace Domain.DTOs.PlanAction
{
    public class PutPlanActionDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
