using System.Collections.Generic;
using System;
using TenmoServer.Models;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Reflection.PortableExecutable;
using System.Linq.Expressions;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public string GetNameById(int accountId)
        {
            string ReceivedName = "";

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlGetNameById = "SELECT username FROM tenmo_user " +
                                           "JOIN tenmo_account ON tenmo_user.user_id = tenmo_account.user_id " +
                                           "WHERE tenmo_account.account_id = @account_id;";

                    SqlCommand sqlCmd = new SqlCommand(sqlGetNameById, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@account_id", accountId);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ReceivedName = reader.GetString("username");
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return ReceivedName;
        }

        public int GetIdByName(string username)
        {
            int ReceivedId = 0;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlGetIdByName = "SELECT user_id FROM tenmo_user " +
                                            "WHERE username = @username;";

                    SqlCommand sqlCmd = new SqlCommand(sqlGetIdByName, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ReceivedId = reader.GetInt32("username");
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return ReceivedId;
        }

        public bool IsValidAccountId(int userId, int accountId)
        {
            bool success = false;
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlGetIdByName = "SELECT user_id FROM account " +
                                            "WHERE user_id = @user_id AND account_id = @account_id;";

                    SqlCommand sqlCmd = new SqlCommand(sqlGetIdByName, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@user_id", userId);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    if (userId != 0)
                    {
                        success = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return success;
        }


        public double GetBalance(int accountId)
        {
            double balance = 0;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlGetBalance = "SELECT balance FROM tenmo_account WHERE account_id = @account_id;";

                    SqlCommand sqlCmd = new SqlCommand(sqlGetBalance, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@account_id", accountId);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        balance = reader.GetDouble("balance");
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return balance;
        }

        public List<int> GetTransferToAccounts()
        {
            List<int> availableAccounts = new List<int>();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlGetTransferToAccts = "SELECT account_id FROM tenmo_account;";


                    SqlCommand sqlCmd = new SqlCommand(sqlGetTransferToAccts, sqlConn);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        availableAccounts.Add(reader.GetInt32("account_id"));
                    }
                }
            }

            catch (SqlException)
            {
                throw;
            }
            return availableAccounts;
        }
        /*
            @Override
            public double executeTransfer(double transferAmount, int accountIdA, int accountIdB) {

                String sql = "BEGIN TRANSACTION;" +
                        "UPDATE tenmo_account" +
                        "SET balance = balance - ?" +
                        "WHERE account_id = ?;" +
                        "UPDATE tenmo_account" +
                        "SET balance = balance + ?" +
                        "WHERE account_id = ?;" +
                        "COMMIT;";
                try {
                SqlRowSet results = jdbcTemplate.queryForRowSet(sql, transferAmount, accountIdA, transferAmount, accountIdB);

                } catch (UserNotFoundException ex){
                    System.out.println("User not found.");
                }
            }

         */



        public bool RejectTransferWithSameId(int accountFrom, int accountTo, double amount)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlRejectTransfer = "INSERT INTO tenmo_transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                           "VALUES (2, 3, @account_from, @account_to, @amount);";

                    SqlCommand sqlCmd = new SqlCommand(sqlRejectTransfer, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@account_from", accountFrom);
                    sqlCmd.Parameters.AddWithValue("@account_to", accountTo);
                    sqlCmd.Parameters.AddWithValue("@amount", amount);
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            if (rowsAffected > 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        public bool SendTransfer(int accountFrom, int accountTo, double amount)
        {
            int rowsAffected = 0;
            int rowsAffectedA = 0;
            int rowsAffectedB = 0;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlSendTransfer = "INSERT INTO tenmo_transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                           "VALUES (2, 2, @account_from, @account_to, @amount);";

                    SqlCommand sqlCmd = new SqlCommand(sqlSendTransfer, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@account_from", accountFrom);
                    sqlCmd.Parameters.AddWithValue("@account_to", accountTo);
                    sqlCmd.Parameters.AddWithValue("@amount", amount);
                    rowsAffected = sqlCmd.ExecuteNonQuery();


                    if (amount <= GetBalance(accountFrom))
                    {

                        string sqlTransferFrom = "UPDATE tenmo_account " +
                                                 "SET balance = balance - @amount " +
                                                 "WHERE account_id = @account_from;";

                        SqlCommand sqlCmdA = new SqlCommand(sqlTransferFrom, sqlConn);
                        sqlCmd.Parameters.AddWithValue("@amount", amount);
                        sqlCmd.Parameters.AddWithValue("@account_from", accountFrom);
                        rowsAffectedA = sqlCmd.ExecuteNonQuery();


                        string sqlTransferTo = "UPDATE tenmo_account " +
                                               "SET balance = balance + @amount " +
                                               "WHERE account_id = @account_to;";

                        SqlCommand sqlCmdB = new SqlCommand(sqlTransferTo, sqlConn);
                        sqlCmd.Parameters.AddWithValue("@amount", amount);
                        sqlCmd.Parameters.AddWithValue("@account_to", accountTo);
                        rowsAffectedB = sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            if ((rowsAffected > 0) && (rowsAffectedA > 0) && (rowsAffectedB > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetUserId(int accountId)
        {
            int userId = 0;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlUserId = "SELECT user_id FROM tenmo_account WHERE account_id = @account_id;";
                    SqlCommand sqlCmd = new SqlCommand(sqlUserId, sqlConn);
                    userId = Convert.ToInt32(sqlCmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return userId;
        }

        public List<Transfer> ListTransfers(int userId)
        {

            List<Transfer> transfers = new List<Transfer>();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlListTransfers = "SELECT * " +
                                              "FROM tenmo_transfer " +
                                              "JOIN tenmo_account ON tenmo_transfer.account_from = tenmo_account.account_id " +
                                              "WHERE user_id = @user_id;";

                    SqlCommand sqlCmd = new SqlCommand(sqlListTransfers, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@user_id", userId);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transfers.Add(MapRowToTransfer(reader));
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return transfers;
        }

        public Transfer TransfersFromId(int userId, int transferId)
        {
            Transfer transfer = new Transfer();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    string sqlTransfersFromId = "SELECT * " +
                                                "FROM tenmo_transfer JOIN tenmo_account ON tenmo_transfer.account_from = tenmo_account.account_id " +
                                                "WHERE user_id = @user_id AND transfer_id = @transfer_id; ";

                    SqlCommand sqlCmd = new SqlCommand(sqlTransfersFromId, sqlConn);
                    sqlCmd.Parameters.AddWithValue("@user_id", userId);
                    sqlCmd.Parameters.AddWithValue("transfer_id", transferId);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transfer = MapRowToTransfer(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfer;
        }


        public Transfer MapRowToTransfer(SqlDataReader reader)
        {

            Transfer transfer = new Transfer();

            transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
            transfer.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
            transfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
            transfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
            transfer.AccountTo = Convert.ToInt32(reader["account_to"]);
            transfer.Amount = Convert.ToDouble(reader["amount"]);
            transfer.AccountId = Convert.ToInt32(reader["account_id"]);
            transfer.UserId = Convert.ToInt32(reader["user_id"]);
            transfer.Balance = Convert.ToDouble(reader["balance"]);

            return transfer;
        }


    }
}
