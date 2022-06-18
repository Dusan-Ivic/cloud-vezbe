using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationService_Data
{
    public class OperationDataRepository
    {
        private CloudTable _table;
        private CloudStorageAccount _storageAccount;

        public OperationDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));

            CloudTableClient tableClient
                = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);

            _table = tableClient.GetTableReference("Rezultat");

            _table.CreateIfNotExists();
        }

        public IQueryable<Operation> RetrieveAllEntities()
        {
            var results = from g in _table.CreateQuery<Operation>()
                          where g.PartitionKey == "Operation"
                          select g;

            return results;
        }

        public void InsertEntity(Operation newOperation)
        {
            TableOperation insertOperation = TableOperation.Insert(newOperation);
            _table.Execute(insertOperation);
        }
    }
}
