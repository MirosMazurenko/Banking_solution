using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateAccountDto
    {
        public string OwnerName { get; set; }
        public decimal InitialBalance { get; set; }
    }
}