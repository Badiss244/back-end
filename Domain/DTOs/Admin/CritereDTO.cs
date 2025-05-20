using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class CritereDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
        public Guid? SxId { get; set; }
        public string? NameS {  get; set; }
    }
}
