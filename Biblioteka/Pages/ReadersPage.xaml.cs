using System.Data;
using System.Windows;
using System.Windows.Controls;
using Biblioteka.Data;
using Biblioteka.Windows;

namespace Biblioteka.Pages
{
   
    public partial class ReadersPage : Page
    {
        
        public ReadersPage()
        {  
            InitializeComponent();
            LoadReaders();
        }

        
        private void LoadReaders(string filter = "")
        {
            // Настройка источника данных для DataGrid:
            // 1. Database.GetReaders(filter) - получает DataTable с читателями
            // 2. .DefaultView - создает представление для привязки данных
            // 3. Привязка к ItemsSource элемента ReadersGrid
            ReadersGrid.ItemsSource = Database.GetReaders(filter).DefaultView;
        }

        
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            // Загружаем читателей с фильтром из текстового поля SearchBox
            LoadReaders(SearchBox.Text);
        }

        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Создаем окно формы для добавления читателя
            var form = new ReaderFormWindow();
            // Показываем окно как модальное диалоговое
            if (form.ShowDialog() == true) // Если пользователь нажал "Сохранить"
                LoadReaders(); // Обновляем список читателей
        }

        
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что в таблице выбран читатель
            if (ReadersGrid.SelectedItem is DataRowView row)
            {
                // Создаем окно формы, передавая данные выбранного читателя
                var form = new ReaderFormWindow(row);
                // Показываем окно редактирования
                if (form.ShowDialog() == true) 
                    LoadReaders(); 
            }
        }

        
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что в таблице выбран читатель
            if (ReadersGrid.SelectedItem is DataRowView row)
            {
                // Получаем ID читателя из выделенной строки
                int id = (int)row["id_reader"];
                // Запрашиваем подтверждение удаления
                if (MessageBox.Show("Удалить читателя?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Вызываем метод удаления из класса Database
                    Database.DeleteReader(id);  
                    LoadReaders();
                }
            }
        }
    }