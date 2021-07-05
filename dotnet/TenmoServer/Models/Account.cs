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

    public class Transaction
    {
        //transfer_type_id, transfer_status_id, account_from, account_to, amount
        public int TransferType { get; set; }
        public string TypeString
        {
            get
            {
                if (TransferType == 1)
                {
                    return "Request";
                }
                else
                {
                    return "Send";
                }
            }
        }
        public int TransferStatus { get; set; }
        public string StatusString
        {
            get
            {
                if(TransferStatus == 1)
                {
                    return "Pending";
                }
                if(TransferStatus == 2)
                {
                    return "Approved";
                }
                else
                {
                    return "Rejected";
                }
            }
        }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal AmountTransfered { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public int TransferId { get; set; }


    }
}
