using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Authorize]
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private IAccountDao accountDao;

        public AccountController(IAccountDao accountDao)
        {
            this.accountDao = accountDao;
        }

        [HttpGet("{id}")]
        public ActionResult<decimal> GetBalance(int id)
        {
            Account account = accountDao.GetAccount(id);

            if(account != null)
            {
                return account.Balance;
            }
            else
            {
                return NotFound();
            }
        }
        
    }
}
