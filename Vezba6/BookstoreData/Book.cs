using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreData
{
    public class Book : TableEntity
    {
        public double Price { get; set; }
        public int Count { get; set; }

        public Book(string bookID)
        {
            PartitionKey = "Book";
            RowKey = bookID;
        }

        public Book()
        {

        }

        public override string ToString()
        {
            return $"BookID: {RowKey}; Price: {Price}; Count: {Count}";
        }
    }
}
