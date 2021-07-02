using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        public decimal Balance { get; set; }
    }



    public class Transfer
    {
        public int UserIdToReceive { get; set; }
        public decimal AmountToTransfer { get; set; }

    }
}
