using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class DashboardStatisticsDTO
    {
        public int AdminCount { get; set; }
        public int FactoryMCount { get; set; }
        public int QualityMCount { get; set; }
        public int AuditorCount { get; set; }
        public int UsineCount { get; set; }
        public int FilialeCount { get; set; }
        public int PlanActionCount { get; set; }
        public int RapportCount { get; set; }
        public double GlobalAverage5s { get; set; }
        public double AverageS1 { get; set; }
        public double AverageS2 { get; set; }
        public double AverageS3 { get; set; }
        public double AverageS4 { get; set; }
        public double AverageS5 { get; set; }
    }
}
