using BookstoreData;
using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore
{
    public class BookstoreServerProvider : IBookstore, ITransaction
    {
        public BookstoreDataRepository bookstoreDataRepository = new BookstoreDataRepository();

        public void ListAvailableItems()
        {
            List<Book> books = bookstoreDataRepository.ListAvailableItems().ToList();

            foreach (Book book in books)
            {
                Trace.TraceInformation(book.ToString());
            }
        }

        public void EnlistPurchase(string bookID, int count)
        {
            bookstoreDataRepository.EnlistPurchase(bookID, count);
        }

        public double GetItemPrice(string bookID)
        {
            return bookstoreDataRepository.GetItemPrice(bookID);
        }

        public bool Prepare()
        {
            return bookstoreDataRepository.Prepare();
        }

        public void Commit()
        {
            bookstoreDataRepository.Commit();
        }

        public void Rollback()
        {
            bookstoreDataRepository.Rollback();
        }
    }
}
