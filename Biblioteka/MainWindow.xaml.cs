using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;

namespace Biblioteka
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Books_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.BooksPage());
        }

        private void Readers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ReadersPage());
        }

        private void Loans_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.LoansPage());
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ReportsPage());
        }
    }
}
