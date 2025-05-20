using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class UpdateSDefinitionDTO
    {
        public Guid Id { get; set; }
        public string? NameEnglish { get; set; }
        public string? NameJaponaise { get; set; }
    }
}
