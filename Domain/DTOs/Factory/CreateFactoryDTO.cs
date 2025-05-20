using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Factory
{
    public class CreateFactoryDTO
    {
        public string Name { get; set; } = "Empty";
        public string Address { get; set; } = "Empty";
        public Guid FilialeId { get; set; }
    }
}
