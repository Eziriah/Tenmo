using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountSqlDao accountDao;
        public AccountController(IAccountSqlDao _accountDao)
        {
            accountDao = _accountDao;
        }

        [HttpGet("/{}")]
        public decimal AccountBalance(string username)
        {

            Balance userBalance = accountDao.GetBalance(username);
            return 
        }
    }
}
