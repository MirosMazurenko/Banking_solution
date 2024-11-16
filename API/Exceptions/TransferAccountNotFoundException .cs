using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Exceptions
{
    public class TransferAccountNotFoundException : Exception
    {
        public TransferAccountNotFoundException(string message) : base(message) { }
    }
}