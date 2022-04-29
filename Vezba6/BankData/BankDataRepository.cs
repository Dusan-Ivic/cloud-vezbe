using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankData
{
    public class BankDataRepository
    {
        private CloudTable _table;
        private CloudStorageAccount _storageAccount;

        private string _userID;
        private double _amount;

        public BankDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BankDataConnectionString"));
            CloudTableClient tableClient
                = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable");

            if (!_table.Exists())
            {
                _table.CreateIfNotExists();

                TableOperation insertOperation;

                User user1 = new User("U1");
                user1.Amount = 1200;
                insertOperation = TableOperation.Insert(user1);
                _table.Execute(insertOperation);

                User user2 = new User("U2");
                user2.Amount = 600;
                insertOperation = TableOperation.Insert(user2);
                _table.Execute(insertOperation);
            }
        }

        public IQueryable<User> ListClients()
        {
            IQueryable<User> clients = from g in _table.CreateQuery<User>()
                                       where g.PartitionKey == "User"
                                       select g;
            return clients;
        }

        public void EnlistMoneyTransfer(string userID, double amount)
        {
            _userID = userID;
            _amount = amount;
        }

        public bool Prepare()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<User>("User", _userID);
            TableResult tableResult = _table.Execute(retrieveOperation);
            User user = tableResult.Result as User;

            if (user != null)
            {
                if (user.Amount >= _amount)
                {
                    User newUser = new User($"{_userID}prep");
                    newUser.Amount = user.Amount - _amount;

                    TableOperation insertOperation = TableOperation.Insert(newUser);
                    _table.Execute(insertOperation);

                    return true;
                }
            }

            return false;
        }

        public void Commit()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<User>("User", $"{_userID}prep");
            TableResult tableResult = _table.Execute(retrieveOperation);
            User sourceUser = tableResult.Result as User;

            retrieveOperation = TableOperation.Retrieve<User>("User", _userID);
            tableResult = _table.Execute(retrieveOperation);
            User destinationUser = tableResult.Result as User;

            if (sourceUser != null & destinationUser != null)
            {
                destinationUser.Amount = sourceUser.Amount;
                TableOperation replaceOperation = TableOperation.Replace(destinationUser);
                _table.Execute(replaceOperation);
            }

            TableOperation deleteOperation = TableOperation.Delete(sourceUser);
            _table.Execute(deleteOperation);
        }

        public void Rollback()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<User>("User", $"{_userID}prep");
            TableResult tableResult = _table.Execute(retrieveOperation);
            User user = tableResult.Result as User;

            if (user != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(user);
                _table.Execute(deleteOperation);
            }
        }
    }
}
