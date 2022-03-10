using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao AccountSqlDao;
        public AccountController(AccountSqlDao accountSqlDao)
        {
            AccountSqlDao = accountSqlDao;
        }
        [HttpGet("{id}")]
        public Account GetAccount(int id)
        {
            return AccountSqlDao.GetAccount(id);
        }
    }
}
