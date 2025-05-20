using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Biblioteka.Data;
using Biblioteka.Windows;

namespace Biblioteka.Pages
{
    
    public partial class LoansPage : Page
    {
        
        public LoansPage()
        {  
            InitializeComponent();
            LoadLoans();
        }

        
        private void LoadLoans()
        {
            // Настройка источника данных для DataGrid:
            // 1. Database.GetLoans() - получает DataTable с информацией о выдачах
            // 2. .DefaultView - создает представление данных для привязки
            // 3. Привязка к ItemsSource DataGrid (LoansGrid)
            LoansGrid.ItemsSource = Database.GetLoans().DefaultView;
        }

        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Создание окна формы для оформления новой выдачи
            var form = new LoanFormWindow();
            // Отображение окна как модального диалога
            if (form.ShowDialog() == true) // Если пользователь подтвердил выдачу
                LoadLoans(); // Обновляем список выдач
        }

        
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            
            if (LoansGrid.SelectedItem is DataRowView row)
            {
                // Проверка, что книга еще не возвращена
                if (row["actual_return_date"] != DBNull.Value)
                {
                    // Если книга уже возвращена, показываем сообщение
                    MessageBox.Show("Книга уже возвращена.");
                    return; 
                }

                // Получаем ID выдачи из выделенной строки
                int id = (int)row["id_loan"];
                // Вызываем метод отметки возврата в базе данных
                Database.MarkLoanReturned(id);
                // Обновляем список выдач
                LoadLoans();
            }
            else
            {
               
                MessageBox.Show("Выберите запись для возврата.");
            }
        }
    }