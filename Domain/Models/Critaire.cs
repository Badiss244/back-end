using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Critaire
    {
        [Key]
        public Guid IdCritaire { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
        [ForeignKey("Sx")]
        public Guid? FKsx { get; set; }
        public Sx? Sx { get; set; }
        public Guid? Key { get; set; }

    }
}
