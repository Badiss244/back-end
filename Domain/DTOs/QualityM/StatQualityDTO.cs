using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.QualityM
{
    public class StatQualityDTO
    {
        public int TotalPlanActions  { get; set; }
        public int TotalTasks  { get; set; }
        public int DoneTasks  { get; set; }
        public int NotDoneTasks  { get; set; }
        public int TotalRapports  { get; set; }
        public int TotalAuditors  { get; set; }
        public int TotalFactories  { get; set; }
    }
}
