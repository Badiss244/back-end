using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Tache
{
    public class TacheDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? NameS { get; set; }
        public bool IsDone { get; set; }
        public List<string> Pictures { get; set; } =new List<string>();
        public string Commantaire { get; set; } = string.Empty;
    }
}
