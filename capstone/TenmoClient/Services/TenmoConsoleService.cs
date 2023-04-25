using RestSharp;
using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }
        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        // Add application-specific UI methods here...

        private readonly RestClient client = new RestClient();
        private static readonly string API_BASE_URL = ("https://localhost:5001");

        private Account currentUser;


        private string authToken = null;

        public void SetAuthToken(string authToken) { this.authToken = authToken; }

        public double GetCurrentBalance()
        {

            double balance = 0;

            try
            {
                RestRequest request = new RestRequest(API_BASE_URL);

                IRestResponse<Account> response = client.Get<Account>(request);

                Account user = response.Data;

                if (user != null)
                {
                    balance = user.GetBalance();
                    Console.WriteLine("Current balance is: $ " + balance);
                }
                if (response.IsSuccessful)
                {
                    Console.WriteLine((int)response.StatusCode);
                    Console.WriteLine(response.Content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return balance;
        }

        public Account[] ListTransferToAccounts()
        {

            Account[] availableAccounts = null;

            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "/account");

                IRestResponse<Account[]> response = client.Get<Account[]>(request);

                availableAccounts = response.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return availableAccounts;
        }
    }
}
