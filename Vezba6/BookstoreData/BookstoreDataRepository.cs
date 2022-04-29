using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreData
{
    public class BookstoreDataRepository
    {
        private CloudTable _table;
        private CloudStorageAccount _storageAccount;

        private string _bookID;
        private int _count;

        public BookstoreDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BookstoreDataConnectionString"));
            CloudTableClient tableClient
                = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("BookTable");

            if (!_table.Exists())
            {
                _table.CreateIfNotExists();

                TableOperation insertOperation;

                Book book1 = new Book("B1");
                book1.Count = 2;
                book1.Price = 1000;
                insertOperation = TableOperation.Insert(book1);
                _table.Execute(insertOperation);

                Book book2 = new Book("B2");
                book2.Count = 3;
                book2.Price = 150;
                insertOperation = TableOperation.Insert(book2);
                _table.Execute(insertOperation);
            }
        }

        public IQueryable<Book> ListAvailableItems()
        {
            IQueryable<Book> books = from g in _table.CreateQuery<Book>()
                                     where g.PartitionKey == "Book"
                                     select g;
            return books;
        }

        public void EnlistPurchase(string bookID, int count)
        {
            _bookID = bookID;
            _count = count;
        }

        public double GetItemPrice(string bookID)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Book>("Book", bookID);
            TableResult tableResult = _table.Execute(retrieveOperation);
            Book book = tableResult.Result as Book;

            return book.Price;
        }

        public bool Prepare()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Book>("Book", _bookID);
            TableResult tableResult = _table.Execute(retrieveOperation);
            Book book = tableResult.Result as Book;

            if (book != null)
            {
                if (book.Count >= _count)
                {
                    Book newBook = new Book($"{_bookID}prep");
                    newBook.Count = book.Count - _count;

                    TableOperation insertOperation = TableOperation.Insert(newBook);
                    _table.Execute(insertOperation);

                    return true;
                }
            }

            return false;
        }

        public void Commit()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Book>("Book", $"{_bookID}prep");
            TableResult tableResult = _table.Execute(retrieveOperation);
            Book sourceBook = tableResult.Result as Book;

            retrieveOperation = TableOperation.Retrieve<Book>("Book", _bookID);
            tableResult = _table.Execute(retrieveOperation);
            Book destinationBook = tableResult.Result as Book;

            if (sourceBook != null & destinationBook != null)
            {
                destinationBook.Count = sourceBook.Count;
                TableOperation replaceOperation = TableOperation.Replace(destinationBook);
                _table.Execute(replaceOperation);
            }

            TableOperation deleteOperation = TableOperation.Delete(sourceBook);
            _table.Execute(deleteOperation);
        }

        public void Rollback()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Book>("Book", $"{_bookID}prep");
            TableResult tableResult = _table.Execute(retrieveOperation);
            Book book = tableResult.Result as Book;

            if (book != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(book);
                _table.Execute(deleteOperation);
            }
        }
    }
}
