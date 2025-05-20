using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Parametres
    {
        [Key]
        public Guid Id { get; set; }
        public bool Maintenance { get; set; } = false;
        //public bool DarkMode { get; set; }    //future idea 
    }
}
