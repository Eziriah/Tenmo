using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountSqlDao
    {
        AccountSqlDao GetBalance(string username);


        //below is the example from the IUserDao, we will need something similar
        //User GetUser(string username);
        //User AddUser(string username, string password);
        //List<User> GetUsers();
    }
}
