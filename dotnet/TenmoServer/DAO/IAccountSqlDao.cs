using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountSqlDao
    {
        decimal GetBalance(string userId);

        bool TransferTEBucks(string userIdToSend, int userIdToReceive, decimal amountToTransfer);
    }
}
