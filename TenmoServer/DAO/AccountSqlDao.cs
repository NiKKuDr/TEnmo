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
    }
}
