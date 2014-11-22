using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using liberty.library.Service;
using liberty.library.Data_layer;

namespace liberty.library
{
    /// <summary>
    /// Interaction logic for lib_main.xaml
    /// This class is purely for UI based logic - all validaton and business logic is in services class
    /// </summary>
    public partial class lib_main : Window
    {
        private services sv;

        public lib_main(services service)
        {
            InitializeComponent();
            sv = service;
        }

        private void show(object sender, RoutedEventArgs e)
        {
            stack_add_borrower.Visibility = Visibility.Collapsed;
            stack_add_book.Visibility = Visibility.Collapsed;
            stack_search_book.Visibility = Visibility.Collapsed;
            stack_search_results.Visibility = Visibility.Collapsed;
            stack_borrow.Visibility = Visibility.Collapsed;
            stack_return.Visibility = Visibility.Collapsed;
            stack_overdue.Visibility = Visibility.Collapsed;

            FrameworkElement feSource = e.Source as FrameworkElement;
            switch (feSource.Name)
            {
                case "btnAddBorrower":
                    stack_add_borrower.Visibility = Visibility.Visible;
                    break;
                case "btnAddBook":
                    stack_add_book.Visibility = Visibility.Visible;
                    break;
                case "btnSearch":
                    stack_search_book.Visibility = Visibility.Visible;
                    break;
                case "btnBorrow":
                    stack_borrow.Visibility = Visibility.Visible;
                    grd_borrow.ItemsSource = sv.get_availableBooks();
                    grd_borrower.ItemsSource = sv.getBorrowers();
                    break;
                case "btnReturn":
                    stack_return.Visibility = Visibility.Visible;
                    grd_return.ItemsSource = sv.get_BorrowedBooks();
                    break;
                case "btnOverdue":
                    stack_overdue.Visibility = Visibility.Visible;
                    grd_overdue.ItemsSource = sv.overdueBooks();
                    break;
            }
        }

        private void hide(object sender, RoutedEventArgs e)
        {
            stack_add_borrower.Visibility = Visibility.Collapsed;
            stack_add_book.Visibility = Visibility.Collapsed;
            stack_search_book.Visibility = Visibility.Collapsed;
            stack_search_results.Visibility = Visibility.Collapsed;
            stack_borrow.Visibility = Visibility.Collapsed;
            stack_return.Visibility = Visibility.Collapsed;
            stack_overdue.Visibility = Visibility.Collapsed;

        }

        private void btnSaveBorrower_Click(object sender, RoutedEventArgs e)
        {
            if (sv.SaveBorrower(txtFirstName.Text, txtLastName.Text))
            { 
                txtFirstName.Text = "";
                txtLastName.Text = "";
            }
        }

        private void btnSaveBook_Click(object sender, RoutedEventArgs e)
        {
            if (sv.SaveBook(txtAuthor.Text, txtTitle.Text))
            {
                txtAuthor.Text = "";
                txtTitle.Text = "";
            }
        }

        private void btnSearchTerm_Click(object sender, RoutedEventArgs e)
        {
            grd_searchResults.ItemsSource = sv.SearchBook(txtSearchTerm.Text);
            stack_search_results.Visibility = Visibility.Visible;
        }

        private void btnBorrowBook_Click(object sender, RoutedEventArgs e)
        {
            if (sv.borrowBook((Borrower)grd_borrower.SelectedItem, (Book)grd_borrow.SelectedItem))
            { 
                grd_borrow.ItemsSource = sv.get_availableBooks();
                grd_borrower.ItemsSource = sv.getBorrowers();
            }

        }

        private void btnReturnBook_Click(object sender, RoutedEventArgs e)
        {
            if (sv.returnBook((Book)grd_return.SelectedItem))
                grd_return.ItemsSource = sv.get_BorrowedBooks();

        }



    }
}
