using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class AccountService
    {
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
