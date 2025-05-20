using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Tache
{
    public class UpdateTacheDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? NameS { get; set; }
    }
}
