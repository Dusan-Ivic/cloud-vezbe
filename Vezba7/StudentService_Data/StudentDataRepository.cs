using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentService_Data
{
    public class StudentDataRepository
    {
        private CloudTable _table;
        private CloudStorageAccount _storageAccount;

        public StudentDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient
                = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("StudentTable");
            _table.CreateIfNotExists();
        }

        public IQueryable<Student> RetrieveAllStudents()
        {
            var results = from g in _table.CreateQuery<Student>()
                          where g.PartitionKey == "Student"
                          select g;

            return results;
        }

        public Student GetStudent(string rowKey)
        {
            var results = from g in _table.CreateQuery<Student>()
                          where (g.PartitionKey == "Student" && g.RowKey == rowKey)
                          select g;

            return results.FirstOrDefault();
        }

        public void UpdateStudent(Student student)
        {
            TableOperation replaceOperation = TableOperation.Replace(student);
            _table.Execute(replaceOperation);
        }

        public void AddStudent(Student newStudent)
        {
            TableOperation insertOperation = TableOperation.Insert(newStudent);
            _table.Execute(insertOperation);
        }

        public bool Exists(string rowKey)
        {
            var results = from g in _table.CreateQuery<Student>()
                          where (g.PartitionKey == "Student" && g.RowKey == rowKey)
                          select g;

            return results.ToList().Count > 0;
        }
    }
}
