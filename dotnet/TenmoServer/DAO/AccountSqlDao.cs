using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountSqlDao
    {
        private readonly string connectionString;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public decimal GetBalance(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts a JOIN users u ON u.user_id = a.user_id WHERE u.user_id = @userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    decimal balance = Convert.ToDecimal(cmd.ExecuteScalar());
                    return balance;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //creating method for transferring money
        public bool TransferTEBucks(int userIdToSend, int userIdToReceive, decimal amountToTransfer, int transferType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    decimal currentBalance = GetBalance(userIdToSend);
                    if (currentBalance >= amountToTransfer)
                    {
                        SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION; " +
                                                        "UPDATE accounts SET balance = balance - @amountToTransfer " +
                                                        "WHERE user_id = @userIdToSend; " +
                                                        "UPDATE accounts SET balance = balance + @amountToTransfer " +
                                                        "WHERE user_id = @userIdToReceive; " +
                                                        "COMMIT;", conn);
                        cmd.Parameters.AddWithValue("@userIdToSend", userIdToSend);
                        cmd.Parameters.AddWithValue("@userIdToReceive", userIdToReceive);
                        cmd.Parameters.AddWithValue("@amountToTransfer", amountToTransfer);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        return false;// "You don't have enough TE Bucks to make this transfer!";
                    }
                }
                return true;//"Successful Transfer"; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        //requesting money method
        public bool RequestTEBucks(int userIdToSend, int userIdToReceive, decimal amountToTransfer)
        {
            int transferType = 1; //1 is request
            int transferStatus = 1; //1 is pending
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    AddTransaction(userIdToSend, userIdToReceive, amountToTransfer, transferType, transferStatus);
                }
                return true;//"Successful Transfer"; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void AddTransaction(int userIdToSend, int userIdToReceive, decimal amountToTransfer, int transferType, int transferStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                        SqlCommand cmd = new SqlCommand("INSERT INTO transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                                 "VALUES(@transferType, @transferStatus, (SELECT account_id FROM accounts WHERE user_id = @userIdToSend), " +
                                                 "(SELECT account_id FROM accounts WHERE user_id = @userIdToReceive), @amountToTransfer); ", conn);
                        cmd.Parameters.AddWithValue("@userIdToSend", userIdToSend);
                        cmd.Parameters.AddWithValue("@userIdToReceive", userIdToReceive);
                        cmd.Parameters.AddWithValue("@amountToTransfer", amountToTransfer);
                        cmd.Parameters.AddWithValue("@transferType", transferType);
                        cmd.Parameters.AddWithValue("@transferStatus", transferStatus);
                        cmd.ExecuteNonQuery();
         
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateTransaction(int transferId, int userIdToSend, int userIdToReceive, decimal amountToTransfer, int transferType, int transferStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_type_id = @transferType, transfer_status_id = @transferStatus, " +
                        "account_from = @userIdToSend, " +
                        "account_to =  (SELECT account_id FROM accounts WHERE user_id = @userIdToReceive), amount = @amountToTransfer " +
                        "WHERE transfer_id = @transferId", conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    cmd.Parameters.AddWithValue("@userIdToSend", userIdToSend);
                    cmd.Parameters.AddWithValue("@userIdToReceive", userIdToReceive);
                    cmd.Parameters.AddWithValue("@amountToTransfer", amountToTransfer);
                    cmd.Parameters.AddWithValue("@transferType", transferType);
                    cmd.Parameters.AddWithValue("@transferStatus", transferStatus);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    //    SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_type_id = @transferType, transfer_status_id = @transferStatus, " +
    //"account_from = (SELECT account_id FROM accounts WHERE user_id = @userIdToSend), " +
    //"account_to = (SELECT account_id FROM accounts WHERE user_id = @userIdToReceive), amount = @amountToTransfer " +
    //"WHERE transfer_id = @transferId", conn);

        public List<Transaction> DisplayTransactions(int userId)// we could possibly use this list to pull our pending transactions as well with transfer type id 
        {
            List<Transaction> allTransactions = new List<Transaction>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username AS sender_username, us.username AS recipient_username " +
                                                    "FROM transfers t " +
                                                    "JOIN accounts a ON t.account_from = a.account_id " +
                                                    "JOIN accounts ac ON t.account_to = ac.account_id " +
                                                    "JOIN users u ON a.user_id = u.user_id " +
                                                    "JOIN users us ON ac.user_id = us.user_id " +
                                                    "WHERE account_from = (SELECT account_id FROM accounts WHERE user_id = @userId) OR account_to = (SELECT account_id FROM accounts WHERE user_id = @userId); ", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        {
                            Transaction transaction = new Transaction()
                            {
                                TransferType = Convert.ToInt32(reader["transfer_type_id"]),
                                TransferStatus = Convert.ToInt32(reader["transfer_status_id"]),
                                AccountFrom = Convert.ToInt32(reader["account_from"]),
                                AccountTo = Convert.ToInt32(reader["account_to"]),
                                AmountTransfered = Convert.ToInt32(reader["amount"]),
                                SenderName = Convert.ToString(reader["sender_username"]),
                                RecipientName = Convert.ToString(reader["recipient_username"]),
                                TransferId = Convert.ToInt32(reader["transfer_id"])
                            };
                            allTransactions.Add(transaction);
                        }
                    }
                    return allTransactions;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public List<Transaction> PendingTransactions(int userId)//based on transfer type id 
        {
            List<Transaction> pendingTransactions = new List<Transaction>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username AS sender_username, us.username AS recipient_username " +
                                                    "FROM transfers t " +
                                                    "JOIN accounts a ON t.account_from = a.account_id " +
                                                    "JOIN accounts ac ON t.account_to = ac.account_id " +
                                                    "JOIN users u ON a.user_id = u.user_id " +
                                                    "JOIN users us ON ac.user_id = us.user_id " +
                                                    "WHERE account_from = (SELECT account_id FROM accounts WHERE user_id = @userId) " +
                                                    "AND t.transfer_status_id = 1; ", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        {
                            Transaction transaction = new Transaction()
                            {
                                TransferType = Convert.ToInt32(reader["transfer_type_id"]),
                                TransferStatus = Convert.ToInt32(reader["transfer_status_id"]),
                                AccountFrom = Convert.ToInt32(reader["account_from"]),
                                AccountTo = Convert.ToInt32(reader["account_to"]),
                                AmountTransfered = Convert.ToInt32(reader["amount"]),
                                SenderName = Convert.ToString(reader["sender_username"]),
                                RecipientName = Convert.ToString(reader["recipient_username"]),
                                TransferId = Convert.ToInt32(reader["transfer_id"])
                            };
                            pendingTransactions.Add(transaction);
                        }
                    }
                    return pendingTransactions;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }
    }
}

