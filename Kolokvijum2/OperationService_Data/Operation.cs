using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationService_Data
{
    public class Operation : TableEntity
    {
        public int Operand1 { get; set; }
        public int Operand2 { get; set; }
        public string Operator { get; set; }
        public int Result { get; set; }

        public Operation(string timestamp)
        {
            PartitionKey = "Operation";
            RowKey = timestamp;
        }

        public Operation()
        {

        }
    }
}
