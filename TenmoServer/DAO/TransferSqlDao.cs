using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TenmoServer.Models;
using System.Transactions;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;
        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public bool SendFunds(decimal transferAmount, int senderId, int recipientId)
        {
            if (transferAmount <= 0 || GetAccount(senderId).Balance < transferAmount)
            {
                return false;
            }
            else
            {
                Account fromAccount = GetAccount(senderId);
                Account toAccount = GetAccount(recipientId);
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            try
                            {
                                SqlCommand cmd = new SqlCommand("UPDATE account SET balance = balance - @transferAmount WHERE user_id = @senderId;", conn);
                                cmd.Parameters.AddWithValue("@senderId", senderId);
                                cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                                cmd.ExecuteNonQuery();

                                cmd = new SqlCommand("UPDATE account SET balance = balance + @transferAmount WHERE user_id = @recipientId;", conn);
                                cmd.Parameters.AddWithValue("@recipientId", recipientId);
                                cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                                cmd.ExecuteNonQuery();

                                cmd = new SqlCommand("INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                    "VALUES (2, 2, @senderAccountId, @recipientAccountId, @transferAmount);", conn);
                                cmd.Parameters.AddWithValue("@senderAccountId", fromAccount.AccountId);
                                cmd.Parameters.AddWithValue("@recipientAccountId", toAccount.AccountId);
                                cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                                cmd.ExecuteNonQuery();

                                transaction.Complete();
                            }
                            catch (Exception exception2)
                            {
                                Console.WriteLine("Commit Exception Type: {0}", exception2.GetType());
                                Console.WriteLine("  Message: {0}", exception2.Message);
                                transaction.Dispose();

                                return false;
                            }

                        }
                    }
                }
                catch (SqlException exception)
                {
                    Console.WriteLine(exception.Message);
                    return false;
                }
            }
            return true;
        }
    
        public Account GetAccount(int id)
        {
            Account returnAccount = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM account WHERE user_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnAccount.AccountId = Convert.ToInt32(reader["account_id"]);
                        returnAccount.UserId = Convert.ToInt32(reader["user_id"]);
                        returnAccount.Balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnAccount;
        }
        public List<Transfer> GetTransfers(int userId)
        {
            int accountId = GetAccount(userId).AccountId;
            List<Transfer> transfers = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_type_id, t.transfer_status_id, t.transfer_id, t.account_to, t.account_from, t.amount " +
                        "FROM transfer t " +
                        "JOIN account a " +
                        "ON t.account_to = a.account_id OR t.account_from = a.account_id " +
                        "WHERE (t.account_to = @accountId OR t.account_from = @accountID) AND a.user_id != @userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Transfer transfer = new Transfer();
                        transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
                        transfer.TransferType = Convert.ToInt32(reader["transfer_type_id"]);
                        transfer.TransferStatus = Convert.ToInt32(reader["transfer_status_id"]);
                        transfer.AccountTo = Convert.ToInt32(reader["account_to"]);
                        transfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
                        transfer.Amount = Convert.ToDecimal(reader["amount"]);
                        transfers.Add(transfer);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfers;
        }
    }
}
