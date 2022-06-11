using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchService_Data
{
    public class SwitchDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public SwitchDataRepository()
        {
            _storageAccount
                = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));

            CloudTableClient tableClient
                = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);

            _table = tableClient.GetTableReference("SwitchTable");

            _table.CreateIfNotExists();
        }

        public IQueryable<Switch> RetrieveAllSwitches()
        {
            var results = from g in _table.CreateQuery<Switch>()
                          where g.PartitionKey == "Switch"
                          select g;

            return results;
        }

        public IQueryable<Switch> RetrieveSwitches(string name)
        {
            var results = from g in _table.CreateQuery<Switch>()
                          where (g.PartitionKey == "Switch" && g.Name == name)
                          select g;

            return results;
        }

        public void AddSwitch(Switch newSwitch)
        {
            TableOperation insertOperation = TableOperation.Insert(newSwitch);
            _table.Execute(insertOperation);
        }
    }
}
