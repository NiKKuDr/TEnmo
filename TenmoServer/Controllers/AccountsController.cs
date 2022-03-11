using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using System.Collections.Generic;

namespace TenmoServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountDao AccountSqlDao;
        public AccountsController(IAccountDao accountDao)
        {
            AccountSqlDao = accountDao;
        }

        [HttpGet]
        public Account GetAccount()
        {
            int userId = GetUserIdFromToken();
            return AccountSqlDao.GetAccount(userId);
        }
        [HttpGet("all")]
        public List<Account> GetAllAccountsExceptUser()
        {
            int userId = GetUserIdFromToken();
            return AccountSqlDao.GetAllAccountsExceptUser(userId);
        }

        public int GetUserIdFromToken()
        {
            int userId = -1;
            try
            {
                userId = int.Parse(User.FindFirst("sub")?.Value);
            }
            catch
            {

            }
            return userId;
        }
    }
}
