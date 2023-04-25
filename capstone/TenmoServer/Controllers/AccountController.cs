using System.Collections.Generic;
using System.IO;
using System;
using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountSqlDao accountSqlDao;
        private readonly UserSqlDao userSqlDao;
        private readonly IUserDao userDao;


        public AccountController(AccountSqlDao accountSqlDao, UserSqlDao userSqlDao, IUserDao userDao)
        {
            this.accountSqlDao = accountSqlDao;
            this.userSqlDao = userSqlDao;
            this.userDao = userDao;
        }

        [HttpGet("/balance")]
        public double GetBalance(LoginUser userParam, int accountId)
        {
            double balance = 0;
            string username = userSqlDao.GetUser(userParam.Username).ToString();
            int usernameId = accountSqlDao.GetIdByName(username);
            if (accountSqlDao.IsValidAccountId(usernameId, accountId))
            {
                balance = accountSqlDao.GetBalance(accountId);
            }
            return balance;
        }

        [HttpGet("/account")]
        public List<int> ListAvailableAccounts(LoginUser userParam)
        {
            string username = userDao.GetUser(userParam.Username).ToString();
            List<int> available = accountSqlDao.GetTransferToAccounts();
            return available;
        }


        [HttpPut("transfer/{accountIdA}/{accountIdB}/{transferAmount}")]
        public string ExecuteTransfer(int accountIdA, int accountIdB, double transferAmount, LoginUser userParam)
        {
            string username = userDao.GetUser(userParam.Username).ToString();
            double balance = accountSqlDao.GetBalance(accountIdA);
            string validName = accountSqlDao.GetNameById(accountIdA);
            if (!username.Equals(validName))
            {
                return "You must use your own Account ID.";
            }
            else if (accountIdA == accountIdB)
            {
            }
            else if ((transferAmount > 0) && (transferAmount < balance))
            {
                accountSqlDao.SendTransfer(accountIdA, accountIdB, transferAmount);
                balance -= transferAmount;
                int userIdA = accountSqlDao.GetUserId(accountIdA);
                int userIdB = accountSqlDao.GetUserId(accountIdB);
                return "User " + userIdA + " sent $" + transferAmount + " to User " + userIdB + ".\n" + userIdA + ", your new balance is: $" + balance;
            }
            else
            {
                return "Insufficient funds or invalid amount";
            }
            return "breh...you know what you did... you can't send money to yourself";
        }

        [HttpGet("transfer")]
        public List<Transfer> GetTransfers(LoginUser userParam)
        {
            string username = userDao.GetUser(userParam.Username).ToString();
            int usernameId = accountSqlDao.GetIdByName(username);
            List<Transfer> transfers = accountSqlDao.ListTransfers(usernameId);
            return transfers;
        }

        [HttpGet("transfer/{transferId}")]
        public Transfer GetSelectedTransfer(int transferId, LoginUser userParam)
        {
            string username = userDao.GetUser(userParam.Username).ToString();
            int usernameId = accountSqlDao.GetIdByName(username);
            Transfer transfer = accountSqlDao.TransfersFromId(usernameId, transferId);
            return transfer;
        }
    }

}
