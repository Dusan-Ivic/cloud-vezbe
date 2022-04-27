using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsService_Data
{
    public class Student : TableEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        public Student(string indexNo)
        {
            PartitionKey = "Student";
            RowKey = indexNo;
        }

        public Student()
        {

        }
    }
}
