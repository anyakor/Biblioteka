using System;
using System.Data;
using System.Windows;
using Biblioteka.Data;

namespace Biblioteka.Windows
{

       public partial class BookFormWindow : Window
    {
        private bool isEditMode = false; // Переменная для отслеживания режима редактирования
        private DataRowView existingRow; // Существующая строка для редактирования
        private DataRowView workingRow; // Рабочая строка для добавления или редактирования

        // Конструктор для создания нового окна книги
        public BookFormWindow()
        {
            InitializeComponent(); 
            LoadComboBoxes(); 
            CreateNewRow(); 
        }

        // Конструктор для редактирования существующей книги
        public BookFormWindow(DataRowView row) : this()
        {
            isEditMode = true; // Установка режима редактирования
            existingRow = row; // Сохранение существующей строки
            workingRow = row; // Установка рабочей строки на существующую

            // Заполнение полей формы данными из существующей строки
            TitleBox.Text = row["title"].ToString();
            YearBox.Text = row["year_published"].ToString();
            IsbnBox.Text = row["isbn"].ToString();
            CategoryBox.SelectedValue = row["id_category"];
            AuthorBox.SelectedValue = row["id_author"];
            TotalBox.Text = row["total_copies"].ToString();
            AvailableBox.Text = row["available_copies"].ToString();
        }

        // Метод для загрузки данных в комбобоксы
        private void LoadComboBoxes()
        {
            CategoryBox.ItemsSource = Database.GetCategories().DefaultView; // Загрузка категорий в комбобокс
            AuthorBox.ItemsSource = Database.GetAuthors().DefaultView; // Загрузка авторов в комбобокс
        }

        // Метод для создания новой строки в таблице
        private void CreateNewRow()
        {
            if (!isEditMode) 
            {
                var table = new DataTable(); 
                table.Columns.Add("id_book", typeof(int));
                table.Columns.Add("title");
                table.Columns.Add("year_published", typeof(int));
                table.Columns.Add("isbn");
                table.Columns.Add("id_category", typeof(int));
                table.Columns.Add("id_author", typeof(int));
                table.Columns.Add("total_copies", typeof(int));
                table.Columns.Add("available_copies", typeof(int));
                table.Rows.Add(); // Добавление пустой строки в таблицу
                workingRow = table.DefaultView[0]; // Установка рабочей строки на только что созданную строку
            }
        }

        // Обработчик события нажатия кнопки "Сохранить"
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверка корректности числовых полей
            if (!int.TryParse(YearBox.Text, out int year) ||
                !int.TryParse(TotalBox.Text, out int total) ||
                !int.TryParse(AvailableBox.Text, out int avail))
            {
                MessageBox.Show("Проверьте числовые поля."); // Сообщение об ошибке
                return; 
            }

            // Заполнение рабочей строки данными из полей формы
            workingRow["title"] = TitleBox.Text;
            workingRow["year_published"] = year;
            workingRow["isbn"] = IsbnBox.Text;
            workingRow["id_category"] = CategoryBox.SelectedValue;
            workingRow["id_author"] = AuthorBox.SelectedValue;
            workingRow["total_copies"] = total;
            workingRow["available_copies"] = avail;

            try
            {
                // В зависимости от режима редактирования, обновляем или вставляем книгу в базу данных
                if (isEditMode)
                    Database.UpdateBook(workingRow); // Обновление существующей книги
                else
                    Database.InsertBook(workingRow); // Вставка новой книги

                DialogResult = true; // Установка результата диалога на "успешно"
                Close(); 
            }
            catch (Exception ex)
            {
            MessageBox.Show("Ошибка при сохранении: " + ex.Message); // Сообщение об ошибке при сохранении
            }
        }

        // Обработчик события нажатия кнопки "Отмена"
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Установка результата диалога на "неуспешно"
            Close(); 
        }
}


