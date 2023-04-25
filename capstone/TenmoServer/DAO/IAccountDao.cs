using System.Collections.Generic;
using System;
using TenmoServer.Models;
using System.Data.SqlClient;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {

        string GetNameById(int accountId);

        int GetIdByName(string username);

        double GetBalance(int accountId);

        List<int> GetTransferToAccounts();

        bool SendTransfer(int accountFrom, int accountTo, double amount);

        bool RejectTransferWithSameId(int accountFrom, int accountTo, double amount);

        List<Transfer> ListTransfers(int userId);

        int GetUserId(int accountId);

        Transfer MapRowToTransfer(SqlDataReader reader);

        Transfer TransfersFromId(int userId, int transferId);


    }
}
