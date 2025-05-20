using System.Data;
using System.Windows;
using System.Windows.Controls;
using Biblioteka.Data;
using Biblioteka.Windows;

namespace Biblioteka.Pages
{
   
    public partial class BooksPage : Page
    {
       
        public BooksPage()
        {
            InitializeComponent();
            LoadBooks();
        }

        // Метод загрузки книг из базы данных
        private void LoadBooks(string filter = "")
        {
            // Установка источника данных для DataGrid:
            // 1. Database.GetBooks(filter) - получает DataTable с книгами
            // 2. .DefaultView - создает представление данных
            // 3. Привязываем к ItemsSource DataGrid
            BooksGrid.ItemsSource = Database.GetBooks(filter).DefaultView;
        }

        
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            // Загружаем книги с фильтром из текстового поля поиска
            LoadBooks(SearchBox.Text);
        }

        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Создаем окно формы для добавления новой книги
            var form = new BookFormWindow();
            // Показываем окно как диалоговое (модальное)
            if (form.ShowDialog() == true) // Если пользователь нажал "Сохранить"
                LoadBooks(); // Обновляем список книг
        }

       
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            
            if (BooksGrid.SelectedItem is DataRowView row)
            {
                // Создаем окно формы, передавая выбранную книгу
                var form = new BookFormWindow(row);
                // Показываем окно редактирования
                if (form.ShowDialog() == true) // Если изменения сохранены
                    LoadBooks(); // Обновляем список книг
            }
        }

       
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
           
            if (BooksGrid.SelectedItem is DataRowView row)
            {
                // Получаем ID книги из выделенной строки
                int id = (int)row["id_book"];
                // Запрашиваем подтверждение удаления
                if (MessageBox.Show("Удалить книгу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Вызываем метод удаления из класса Database
                    Database.DeleteBook(id);
                    // Обновляем список книг
                    LoadBooks();
                }
            }
        }
    }