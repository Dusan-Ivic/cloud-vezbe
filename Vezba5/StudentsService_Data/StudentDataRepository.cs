using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsService_Data
{
    public class StudentDataRepository
    {
        private CloudTable table;
        private CloudStorageAccount storageAccount;

        public StudentDataRepository()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(storageAccount.TableEndpoint.AbsoluteUri),
                storageAccount.Credentials
            );
            table = tableClient.GetTableReference("StudentTable");
            table.CreateIfNotExists();
        }

        public IQueryable<Student> RetrieveAllStudents()
        {
            var results = from g in table.CreateQuery<Student>()
                          where g.PartitionKey == "Student"
                          select g;
            return results;
        }

        public void AddStudent(Student newStudent)
        {
            TableOperation insertOperation = TableOperation.Insert(newStudent);
            table.Execute(insertOperation);
        }
    }
}
