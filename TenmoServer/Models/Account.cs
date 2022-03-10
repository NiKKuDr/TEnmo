using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        public int Account_id { get; set; }
        public int User_id { get; set; }
        public decimal Balance { get; set; }

        public Account(int account_id, int user_id, decimal balance)
        {
            Account_id = account_id;
            User_id = user_id;
            Balance = balance;
        }
    }
}
