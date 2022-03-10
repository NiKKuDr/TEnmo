using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
            Account account = null;

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM account WHERE user_id = @user_id", conn);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        
                        int accountID = Convert.ToInt32(reader["account_id"]);
                        int userID = Convert.ToInt32(reader["user_id"]);
                        decimal balance = Convert.ToDecimal(reader["balance"]);
                        account = new Account(accountID, userID, balance);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return account;
        }
    }
}
