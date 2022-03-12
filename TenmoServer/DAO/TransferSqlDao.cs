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
        public StringifiedTransfer SendFunds(decimal transferAmount, int senderId, int recipientId)
        {
            StringifiedTransfer transfer = new StringifiedTransfer();
            int transferId = -1;
            if (transferAmount <= 0 || GetAccountFromUserId(senderId).Balance < transferAmount)
            {
                return null;
            }
            else
            {
                Account fromAccount = GetAccountFromUserId(senderId);
                Account toAccount = GetAccountFromUserId(recipientId);
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
                                    "OUTPUT inserted.transfer_id " +
                                    "VALUES (2, 2, @senderAccountId, @recipientAccountId, @transferAmount);", conn);
                                cmd.Parameters.AddWithValue("@senderAccountId", fromAccount.AccountId);
                                cmd.Parameters.AddWithValue("@recipientAccountId", toAccount.AccountId);
                                cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                                transferId = Convert.ToInt32(cmd.ExecuteScalar());

                                
                                transaction.Complete();
                            }
                            catch (Exception exception2)
                            {
                                Console.WriteLine("Commit Exception Type: {0}", exception2.GetType());
                                Console.WriteLine("  Message: {0}", exception2.Message);
                                transaction.Dispose();

                                return null;
                            }

                        }
                    }
                }
                catch (SqlException exception)
                {
                    Console.WriteLine(exception.Message);
                    return null;
                }
            }
            transfer = GetTransferById(transferId);
            return transfer;
        }
    
        public Account GetAccountFromUserId(int userId)
        {
            Account returnAccount = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM account WHERE user_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", userId);
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
        public List<StringifiedTransfer> GetTransfers(int userId)
        {
            List<StringifiedTransfer> transfers = new List<StringifiedTransfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string cmdString = "SELECT t.transfer_id, t.amount, tu_from.username AS from_user, tu_to.username AS to_user, tt.transfer_type_desc, ts.transfer_status_desc " +
                        "from transfer t " +
                        "join account a_from " +
                        "on a_from.account_id = t.account_from " +
                        "join account a_to " +
                        "on a_to.account_id = t.account_to " +
                        "join tenmo_user tu_from " +
                        "on tu_from.user_id = a_from.user_id " +
                        "join tenmo_user tu_to " +
                        "on tu_to.user_id = a_to.user_id " +
                        "join transfer_status ts " +
                        "on ts.transfer_status_id = t.transfer_status_id " +
                        "join transfer_type tt " +
                        "on tt.transfer_type_id = t.transfer_type_id " +
                        "WHERE tu_from.user_id = @userId OR tu_to.user_id = @userId;";
                    SqlCommand cmd = new SqlCommand(cmdString, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transfers.Add(CreateTransferFromReader(reader));
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfers;
        }
        public StringifiedTransfer GetTransferById(int transferId)
        {
            StringifiedTransfer transfer = new StringifiedTransfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.amount, tu_from.username AS from_user, tu_to.username AS to_user, tt.transfer_type_desc, ts.transfer_status_desc " +
                        "from transfer t " +
                        "join account a_from " +
                        "on a_from.account_id = t.account_from " +
                        "join account a_to " +
                        "on a_to.account_id = t.account_to " +
                        "join tenmo_user tu_from " +
                        "on tu_from.user_id = a_from.user_id " +
                        "join tenmo_user tu_to " +
                        "on tu_to.user_id = a_to.user_id " +
                        "join transfer_status ts " +
                        "on ts.transfer_status_id = t.transfer_status_id " +
                        "join transfer_type tt " +
                        "on tt.transfer_type_id = t.transfer_type_id " +
                        "WHERE t.transfer_id = @transferId", conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        transfer = CreateTransferFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfer;
        }
        public StringifiedTransfer CreateTransferFromReader(SqlDataReader reader)
        {
            StringifiedTransfer transfer = new StringifiedTransfer();
            transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
            transfer.TransferType = Convert.ToString(reader["transfer_type_desc"]);
            transfer.TransferStatus = Convert.ToString(reader["transfer_status_desc"]);
            transfer.UserTo = Convert.ToString(reader["to_user"]);
            transfer.UserFrom = Convert.ToString(reader["from_user"]);
            transfer.Amount = Convert.ToDecimal(reader["amount"]);
            
            return transfer;
        }
        public List<StringifiedTransfer> GetPendingTransfers(int userId)
        {
            List<StringifiedTransfer> transfers = new List<StringifiedTransfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.amount, tu_from.username AS from_user, tu_to.username AS to_user, tt.transfer_type_desc, ts.transfer_status_desc " +
                        "from transfer t " +
                        "join account a_from " +
                        "on a_from.account_id = t.account_from " +
                        "join account a_to " +
                        "on a_to.account_id = t.account_to " +
                        "join tenmo_user tu_from " +
                        "on tu_from.user_id = a_from.user_id " +
                        "join tenmo_user tu_to " +
                        "on tu_to.user_id = a_to.user_id " +
                        "join transfer_status ts " +
                        "on ts.transfer_status_id = t.transfer_status_id " +
                        "join transfer_type tt " +
                        "on tt.transfer_type_id = t.transfer_type_id " +
                        "WHERE a_from.user_id = @id AND ts.transfer_status_desc = 'Pending' AND tt.transfer_type_desc = 'Request'", conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transfers.Add(CreateTransferFromReader(reader));
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfers;
        }
        public StringifiedTransfer RequestFunds(decimal transferAmount, int requesterId, int requestFromAccount)
        {
            StringifiedTransfer transfer = new StringifiedTransfer();
            int transferId = -1;
            if (transferAmount <= 0)
            {
                return null;
            }
            else
            {
                Account toAccount = GetAccountFromUserId(requesterId);
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        {
                            try
                            {
                                SqlCommand cmd = new SqlCommand("INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                    "OUTPUT inserted.transfer_id " +
                                    "VALUES (1, 1, @senderAccountId, @recipientAccountId, @transferAmount);", conn);
                                cmd.Parameters.AddWithValue("@senderAccountId", requestFromAccount);
                                cmd.Parameters.AddWithValue("@recipientAccountId", toAccount.AccountId);
                                cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                                transferId = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            catch (Exception exception2)
                            {
                                Console.WriteLine("Commit Exception Type: {0}", exception2.GetType());
                                Console.WriteLine("  Message: {0}", exception2.Message);

                                return null;
                            }

                        }
                    }
                }
                catch (SqlException exception)
                {
                    Console.WriteLine(exception.Message);
                    return null;
                }
            }
            transfer = GetTransferById(transferId);
            return transfer;
        }
    }
}
