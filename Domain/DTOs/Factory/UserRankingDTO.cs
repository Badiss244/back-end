using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Factory
{
    public class UserRankingDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FactoryName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}
