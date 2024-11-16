using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class AccountDto
    {
        public long Id { get; set; }
        public string OwnerName { get; set; }
        public decimal Balance { get; set; }
    }
}