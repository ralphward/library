using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liberty.library.Data_layer
{
    ///<summary
    /// This class is used to control access to the data for the Liberty Library
    /// This is mostly collection manipulation
    /// This will contain and log exceptions where the data hasn't been corrupted
    /// Any exceptions that could result in corrupted data will be thrown back to the Service Layer to exit the app
    /// TODO:: This class has the potential for race time collisions to cause books or borrowers to have the same UID - version 1.0 will add appropriate data locking procedures
    ///</summary>
    public class data_layer
    {

        private List<Borrower> _borrowers;
        private List<Book> _books;

        public List<Book> books
        {
            get
            {
                return _books;
            }
        }

        public List<Borrower> borrowers
        {
            get
            {
                return _borrowers;
            }
        }

        public data_layer()
        {
            init_data();
        }

        private void init_data()
        {
            try
            {
                _borrowers = new List<Borrower>()
                {
                    new Borrower() {UID = 1, f_name = "Trillian", l_name = "Astra"},
                    new Borrower() {UID = 2, f_name = "Zaphod", l_name = "Beeblebrox"},
                    new Borrower() {UID = 3, f_name = "Arthur", l_name = "Dent"},
                    new Borrower() {UID = 4, f_name = "Ford", l_name = "Prefect"},
                };

                _books = new List<Book>()
                {
                    new Book() {UID = 1, author = "Douglas Adams", title = "The hitchhiker's guide to the galaxy series", dt_borrowed = default(DateTime), borrower_UID = -1},
                    new Book() {UID = 2, author = "Douglas Adams", title = "The Restaurant at the End of the Universe", dt_borrowed = default(DateTime), borrower_UID = -1},
                    new Book() {UID = 3, author = "Douglas Adams", title = "Life the Universe and Everything", dt_borrowed = DateTime.Now.AddDays(-9), borrower_UID = 1},
                    new Book() {UID = 4, author = "Douglas Adams", title = "So long and thanks for all the fish", dt_borrowed = DateTime.Now.AddDays(-6), borrower_UID = 3},
                    new Book() {UID = 5, author = "Douglas Adams", title = "Mostly harmless", dt_borrowed = default(DateTime), borrower_UID = -1},
                    new Book() {UID = 6, author = "Eoin Colfer", title = "And another thing", dt_borrowed = default(DateTime), borrower_UID = -1},
                };

            }
            catch (Exception ex)
            {
                // If an exception is thrown here we likely have corrupted data loaded and it's best to quit the application entirely - log the exception stack trace se we don't lose it
                // and re-throw the exception
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void addBook(string l_title, string l_author)
        {
            try
            {
                int i = _books.Max(Book => Book.UID) + 1;
                _books.Add(new Book 
                {
                    UID = i, 
                    author = l_author,
                    title = l_title,
                    dt_borrowed = default(DateTime),
                    borrower_UID = -1
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void addBorrower(string l_f_name, string l_l_name)
        {
            try
            {
                // TODO:: potential for race time collisions with UID - implement a locking procedure
                int i = _borrowers.Max(Borrower => Borrower.UID) + 1;
                _borrowers.Add(new Borrower
                {
                    UID = i,
                    f_name = l_f_name,
                    l_name = l_l_name
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public bool borrowBook(int l_borrower, int l_book)
        {
            try
            {
                for (int i = 0; i <_books.Count; i++)
                {
                    if (_books[i].UID == l_book)
                    { 
                        _books[i].borrower_UID = l_borrower;
                        _books[i].dt_borrowed = DateTime.Now;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception and let the user know it failed
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool returnBook(int l_book)
        {
            try
            {
                for (int i = 0; i < _books.Count; i++)
                {
                    if (_books[i].UID == l_book)
                    {
                        _books[i].borrower_UID = -1;
                        _books[i].dt_borrowed = default(DateTime);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception and let the user know it failed
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool returnBook(int l_borrower, int l_book)
        {
            try
            {
                Book b =
                    (from Book in _books
                     where Book.UID == l_book
                     && Book.borrower_UID == l_borrower
                     select Book).SingleOrDefault();

                b.borrower_UID = -1;
                b.dt_borrowed = default(DateTime);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception and let the user know it failed
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<Book> searchBooks(string search_key)
        {
            List<Book> s_books = new List<Book> { };
            try
            {
                // allow fuzzy search - heavy perfomance sacrifice
                string[] filters = search_key.Split(new[] { ' ' });

                var l_books = (from Book in _books
                               where filters.All(f => Book.author.Contains(f) || Book.title.Contains(f))
                               select Book);

                foreach (Book l_book in l_books)
                {
                    s_books.Add(l_book);
                }

                return s_books;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return s_books;
            }
        }

        public List<Book> availableBooks()
        {
            List<Book> l_available = new List<Book> { };
            try
            {
                var available_list = (from Book in _books
                                 where Book.borrower_UID == -1
                                 select Book);

                foreach (var available in available_list)
                {
                    l_available.Add(available);
                }

                return l_available;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return l_available;
            }
        }

        public List<Book> borrowedBooks()
        {
            List<Book> l_available = new List<Book> { };
            try
            {
                var available_list = (from Book in _books
                                      where Book.borrower_UID != -1
                                      select Book);

                foreach (var available in available_list)
                {
                    l_available.Add(available);
                }

                return l_available;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return l_available;
            }
        }

        public List<Overdue> overdueBooks()
        {
            List<Overdue> o_due = new List<Overdue> { };

            try
            {
                var overdue_list = (from Book in _books
                                    where Book.dt_borrowed < DateTime.Now.AddDays(-7)
                                    orderby Book.UID
                                    select Book);
                foreach (var overdue_item in overdue_list)
                {
                    var o_borrower = (from Borrower in _borrowers
                                      where Borrower.UID == overdue_item.borrower_UID
                                      select Borrower).Distinct();

                    o_due.Add(new Overdue(){
                            late_book = overdue_item,
                            late_borrower = o_borrower.First()
                            });

                }

                return o_due;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return o_due;
            }
        }
    
    }

    public class Book
    {
        public int UID { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public DateTime dt_borrowed;
        public int borrower_UID;
    }

    public class Borrower
    {
        public int UID { get; set; }
        public string f_name { get; set; }
        public string l_name { get; set; }

    }

    public class Overdue
    {
        public Book late_book { get; set; }
        public Borrower late_borrower { get; set; }
    }

}
