using System;
using System.Data;
using System.Windows;
using Biblioteka.Data;

namespace Biblioteka.Windows
{
    public partial class ReaderFormWindow : Window
    {
        private bool isEditMode = false; // Переменная для определения режима редактирования
        private DataRowView workingRow; // Переменная для хранения текущей строки данных читателя

        
        public ReaderFormWindow()
        {
            InitializeComponent(); 
            CreateNewRow(); 
        }

        
        public ReaderFormWindow(DataRowView row) : this()
        {
            isEditMode = true; // Установка режима редактирования
            workingRow = row; // Сохранение ссылки на редактируемую строку

            // Заполнение полей формы данными из выбранной строки
            LastNameBox.Text = row["last_name"].ToString();
            FirstNameBox.Text = row["first_name"].ToString();
            PatronymicBox.Text = row["patronymic"].ToString();
            PassportBox.Text = row["passport"].ToString();
            PhoneBox.Text = row["phone"].ToString();
            EmailBox.Text = row["email"].ToString();
        }

        // Метод для создания новой строки в таблице данных
        private void CreateNewRow()
        {
            if (!isEditMode) 
            {
                var table = new DataTable(); 
                                             
                table.Columns.Add("id_reader", typeof(int));
                table.Columns.Add("last_name");
                table.Columns.Add("first_name");
                table.Columns.Add("patronymic");
                table.Columns.Add("passport");
                table.Columns.Add("phone");
                table.Columns.Add("email");
                table.Rows.Add(); 
                workingRow = table.DefaultView[0]; // Сохранение ссылки на созданную строку
            }
        }

       
        private void Save_Click(object sender, RoutedEventArgs e)
        {
           
            workingRow["last_name"] = LastNameBox.Text;
            workingRow["first_name"] = FirstNameBox.Text;
            workingRow["patronymic"] = PatronymicBox.Text;
            workingRow["passport"] = PassportBox.Text;
            workingRow["phone"] = PhoneBox.Text;
            workingRow["email"] = EmailBox.Text;

            try
            {
                // В зависимости от режима, обновление или вставка читателя в базу данных
                if (isEditMode)
                    Database.UpdateReader(workingRow); // Обновление существующего читателя
                else
                    Database.InsertReader(workingRow); // Вставка нового читателя

                DialogResult = true; 
                Close(); 
            }
            catch (Exception ex) // Обработка исключений
            {
                MessageBox.Show("Ошибка: " + ex.Message); 
            }
        }

        
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            Close(); 
        }
    }