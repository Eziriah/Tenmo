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
        
        public decimal GetBalance(string userId)
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
        public bool TransferTEBucks(string userIdToSend, int userIdToReceive, decimal amountToTransfer)
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
            catch (Exception)
            {

                throw;
            }
        }
        //our sql command needs to be a transaction (all or nothing)
        //make sure transfer amt <= balance

    }
}
