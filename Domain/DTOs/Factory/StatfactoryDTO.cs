using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Factory
{
    public class StatfactoryDTO
    {
        public string FactoryName { get; set; }
        public string FactoryAddress { get; set; }
        public int TotalPlanActions { get; set; }
        public double AverageScore { get; set; }
        public int Rapports { get; set; }
        public int TotalTasksToDo { get; set; }
        public int TotalTasksCompleted { get; set; }
        public int TotalTasksNotCompleted { get; set; }
    }
}
