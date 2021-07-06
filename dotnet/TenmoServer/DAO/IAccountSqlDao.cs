using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountSqlDao
    {
        decimal GetBalance(int userId);

        bool TransferTEBucks(int userIdToSend, int userIdToReceive, decimal amountToTransfer, int transferType);

        List<Transaction> DisplayTransactions(int userId);
        bool RequestTEBucks(int userIdToSend, int userIdToReceive, decimal amountToTransfer);
        List<Transaction> PendingTransactions(int userId);
        void AddTransaction(int userIdToSend, int userIdToReceive, decimal amountToTransfer, int transferType, int transferStatus);
        bool UpdateTransaction(int transferId, int userIdToSend, int userIdToReceive, decimal amountToTransfer, int transferType, int transferStatus);
    }
}
