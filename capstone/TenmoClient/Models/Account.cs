using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class Account
    {
        double Balance { get; set; }
        int AccountId { get; set; }

        internal double GetBalance()
        {
            throw new NotImplementedException();
        }
    }
}
