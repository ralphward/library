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
using System.Windows.Media.Animation;

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
        private ColorAnimation ca;
        private ColorAnimation grd_ca;

        public lib_main(services service)
        {
            InitializeComponent();
            ca = new ColorAnimation();
            ca.From = Colors.White;
            ca.To = Colors.LightGray;
            ca.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            ca.AutoReverse = true;

            grd_ca = new ColorAnimation();
            grd_ca.From = Colors.Black;
            grd_ca.To = Colors.White; 
            grd_ca.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            grd_ca.AutoReverse = true;

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
                    grd_borrow.ItemsSource = sv.get_availableBooks();
                    // Quick hack as this DataGrid is bound directly to the _borrowers datasource
                    // Needs to be changed so it has a listener on the modified event of the Borrowes List
                    grd_borrower.ItemsSource = null;
                    grd_borrower.ItemsSource = sv.getBorrowers();
                    stack_borrow.Visibility = Visibility.Visible;
                    break;
                case "btnReturn":
                    stack_return.Visibility = Visibility.Visible;
                    grd_return.ItemsSource = sv.get_BorrowedBooks();
                    break;
                case "btnOverdue":
                    grd_overdue.ItemsSource = sv.overdueBooks();
                    stack_overdue.Visibility = Visibility.Visible;
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
            else
            {
                txtFirstName.Background = new SolidColorBrush(Colors.White);
                txtFirstName.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                txtLastName.Background = new SolidColorBrush(Colors.White);
                txtLastName.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }

        }

        private void btnSaveBook_Click(object sender, RoutedEventArgs e)
        {
            if (sv.SaveBook(txtAuthor.Text, txtTitle.Text))
            {
                txtAuthor.Text = "";
                txtTitle.Text = "";
            }
            else
            {
                txtAuthor.Background = new SolidColorBrush(Colors.White);
                txtAuthor.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                txtTitle.Background = new SolidColorBrush(Colors.White);
                txtTitle.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
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
            else
            {
                grd_borrow.BorderBrush = new SolidColorBrush(Colors.Black);
                grd_borrow.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, grd_ca);
                grd_borrower.BorderBrush = new SolidColorBrush(Colors.Black);
                grd_borrower.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, grd_ca);
            }

        }

        private void btnReturnBook_Click(object sender, RoutedEventArgs e)
        {
            if (sv.returnBook((Borrowed_Book)grd_return.SelectedItem))
                grd_return.ItemsSource = sv.get_BorrowedBooks();
            else
            {
                grd_return.BorderBrush = new SolidColorBrush(Colors.Black);
                grd_return.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, grd_ca);
            }
        }

    }
}
