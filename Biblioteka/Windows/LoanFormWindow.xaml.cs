using System;
using System.Data;
using System.Windows;
using Biblioteka.Data;

namespace Biblioteka.Windows
{
    public partial class LoanFormWindow : Window
    {
       
        public LoanFormWindow()
        {
            InitializeComponent(); 
            LoadCombos(); 
            ReturnDatePicker.SelectedDate = DateTime.Today.AddDays(14); // Установка даты возврата на 14 дней вперед от текущей даты
        }

        // Метод для загрузки данных в комбобоксы
        private void LoadCombos()
        {
            BookBox.ItemsSource = Database.GetAvailableBooks().DefaultView; // Загрузка доступных книг в комбобокс
            ReaderBox.ItemsSource = Database.GetReaders("").DefaultView; // Загрузка читателей в комбобокс
            EmployeeBox.ItemsSource = Database.GetEmployees().DefaultView; // Загрузка сотрудников в комбобокс
        }

        // Обработчик события нажатия кнопки "Сохранить"
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, что все поля заполнены
            if (BookBox.SelectedValue == null || ReaderBox.SelectedValue == null ||
                EmployeeBox.SelectedValue == null || ReturnDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля."); 
                return;
            }

            
            Database.InsertLoan(
                (int)BookBox.SelectedValue, 
                (int)ReaderBox.SelectedValue, 
                (int)EmployeeBox.SelectedValue, 
                DateTime.Today, 
                ReturnDatePicker.SelectedDate.Value 

            DialogResult = true; 
            Close(); 
        }

       
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            Close(); 
        }
    }
