using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao AccountSqlDao;
        public AccountController(IAccountDao accountDao)
        {
            AccountSqlDao = accountDao;
        }

        [HttpGet("{id}")]
        public Account GetAccount(int id)
        {
            return AccountSqlDao.GetAccount(id);
        }
    }
}
