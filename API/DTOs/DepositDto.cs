using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class DepositDto
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
    }
}