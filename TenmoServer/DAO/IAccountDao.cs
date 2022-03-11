using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        Account GetAccount(int id);

        List<Account> GetAllAccountsExceptUser(int id);
    }
}
