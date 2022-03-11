using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;
        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public Account GetAccount(int id)
        {
            Account returnAccount = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM account WHERE user_id = @id;", conn);
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
        public List<Account> GetAllAccountsExceptUser(int id)
        {
            List<Account> accounts = new List<Account>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT a.account_id, a.user_id, a.balance, tu.username FROM account a " +
                        "JOIN tenmo_user tu " +
                        "ON a.user_id = tu.user_id " +
                        "WHERE a.user_id != @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Account account = new Account();
                        account.AccountId = Convert.ToInt32(reader["account_id"]);
                        account.UserId = Convert.ToInt32(reader["user_id"]);
                        account.Balance = Convert.ToDecimal(reader["balance"]);
                        account.UserName = Convert.ToString(reader["username"]);
                        accounts.Add(account);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return accounts;
        }
        
        public bool TransferFunds(int senderId, decimal transferAmount)
        {
            return false;
        }
    }
}
