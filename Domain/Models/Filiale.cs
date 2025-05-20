using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Filiale
    {
        [Key]
        public Guid IdFiliale { get; set; }
        public string Name { get; set; } = "none";
        public ICollection<Factory> Factories { get; set; } = new List<Factory>();
    }
}
