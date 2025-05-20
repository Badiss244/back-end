using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Factory
{
    public class FactoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Guid FilialeId { get; set; }
        public string FilialeName { get; set; }
        public string ManagerFactory { get; set; } = "Empty";
    }
}
