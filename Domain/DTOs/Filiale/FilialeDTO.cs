using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Factory;

namespace Domain.DTOs.Filiale
{
    public class FilialeDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? FactoryCount { get; set; }
        public List<FactoryDTO> Factories { get; set; } = new List<FactoryDTO>();
    }
}
