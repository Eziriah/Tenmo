using Microsoft.AspNetCore.Authorization;
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
    [Route("[controller]")]
    [ApiController]
    //TODO: make whole class authorize
    public class AccountController : ControllerBase
    {
        private readonly IAccountSqlDao accountDao;
        private readonly IUserDao userDao;
        public AccountController(IAccountSqlDao _accountDao, IUserDao _userDao)
        {
            accountDao = _accountDao;
            userDao = _userDao;
        }
        [Authorize]
        //getting account balance
        [HttpGet]
        public decimal AccountBalance()
        {
            int userId = int.Parse(User.FindFirst("sub")?.Value);
            decimal userBalance = accountDao.GetBalance(userId);
            return userBalance;
            
        }

        //getting a list of users
        [Authorize]
        [HttpGet("userslist")]
        public List<User> ListUsers()
        {
            string username = User.Identity.Name;
            return userDao.GetUsersNameAndId(username);
        }
        
        //sending TE Bucks
        [Authorize]
        [HttpPut("transfer")]
        public bool TransferTEBucks(Transfer transfer)
        {
            int userId = int.Parse(User.FindFirst("sub")?.Value);
            bool result = accountDao.TransferTEBucks(userId, transfer.UserIdToReceive, transfer.AmountToTransfer);
            return result;
        }

        //requesting TE Bucks
        [Authorize]
        [HttpPost("request")]
        public bool RequestTEBucks(Transfer transfer)
        {
            int userId = int.Parse(User.FindFirst("sub")?.Value);

            bool result = accountDao.RequestTEBucks(transfer.UserIdToReceive, userId, transfer.AmountToTransfer);
            return result;
        }

        //creating a list of transactions
        [Authorize]
        [HttpGet("transfer")]
        public List<Transaction> DisplayTransactions()
        {
            int userId = int.Parse(User.FindFirst("sub")?.Value);
            return accountDao.DisplayTransactions(userId);
        }

        //display pending transactions
        [Authorize]
        [HttpGet("transfer/pending")]
        public List<Transaction> DisplayPendingTransactions()
        {
            int userId = int.Parse(User.FindFirst("sub")?.Value);
            return accountDao.PendingTransactions(userId);
        }
    }
}
