using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using liberty.library.Data_layer;

namespace liberty.library.Service
{
    public class services
    {
        private data_layer dl;

        public services()
        {
            try 
            { 
                dl = new data_layer();
                lib_main lm = new lib_main(this);
                lm.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
            }

        }

        public List<Borrower> getBorrowers()
        {
            return dl.borrowers;
        }

        public bool borrowBook(Borrower i_borrow, Book i_book)
        {
            if(i_book != null && i_borrow != null)
                return dl.borrowBook(i_borrow.UID, i_book.UID);
            return false;
        }

        public bool returnBook(Book i_book)
        {
            if (i_book != null)
                return dl.returnBook(i_book.UID);
            return false;
        }

        public List<Book> get_availableBooks()
        {
            return dl.availableBooks();
        }

        public IList<Book> SearchBook(string strSearch)
        {
            return dl.searchBooks(strSearch);
        }

        public bool SaveBorrower(string f_name, string l_name)
        {
            if (f_name.Length > 0 && l_name.Length > 0)
                dl.addBorrower(f_name, l_name);

            return (f_name.Length > 0 && l_name.Length > 0);
        }

        public bool SaveBook(string title, string author)
        {
            if (title.Length > 0 && author.Length > 0)
                dl.addBook(title, author);

            return (title.Length > 0 && author.Length > 0);
        }

        public List<Book> get_BorrowedBooks()
        {
            return dl.borrowedBooks();
        }

        public List<Overdue> overdueBooks()
        {
            return dl.overdueBooks();
        }
    }
}
