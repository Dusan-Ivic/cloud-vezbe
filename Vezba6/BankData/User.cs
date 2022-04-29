using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankData
{
    public class User : TableEntity
    {
        public double Amount { get; set; }

        public User(string userID)
        {
            PartitionKey = "User";
            RowKey = userID;
        }

        public User()
        {

        }

        public override string ToString()
        {
            return $"UserID: {RowKey}; Amount: {Amount}";
        }
    }
}
