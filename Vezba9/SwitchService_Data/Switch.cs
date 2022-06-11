using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchService_Data
{
    public class Switch : TableEntity
    {
        public string Name { get; set; }
        public string State { get; set; }

        public Switch(string timestamp)
        {
            PartitionKey = "Switch";
            RowKey = timestamp;
        }

        public Switch()
        {

        }
    }
}
