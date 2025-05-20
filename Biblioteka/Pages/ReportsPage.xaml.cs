using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Biblioteka.Data;

namespace Biblioteka.Pages
{
    
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            
            InitializeComponent();
            ReportSelector.SelectedIndex = 0;   
            GenerateReport();
        }

        
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
           
            GenerateReport();
        }

        // Обработчик изменения выбранного отчета
        private void ReportSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateReport();
        }

        
        private void GenerateReport()
        {
            
            ReportContainer.Children.Clear();
            // Получение текста выбранного элемента комбобокса
            string selected = ((ComboBoxItem)ReportSelector.SelectedItem).Content.ToString();

            // Выбор соответствующего отчета по тексту
            if (selected == "Выданные книги за месяц")
                LoadIssuedBooksReport(); // Загрузка отчета по выданным книгам
            else if (selected == "Просроченные возвраты")
                LoadOverdueReport(); // Загрузка отчета по просрочкам
            else if (selected == "Активность читателей")
                LoadReaderActivityReport(); // Загрузка отчета по активности
        }

        // Метод загрузки отчета по выданным книгам
        private void LoadIssuedBooksReport()
        {
            
            var data = Database.GetIssuedBooksLastMonth();

            // Создание заголовка с количеством записей
            TextBlock header = CreateHeader($"📚 Выданные книги за последние 30 дней: {data.Rows.Count}");
            ReportContainer.Children.Add(header);

            // Создание карточек для каждой записи
            foreach (DataRow row in data.Rows)
            {
                string info = $"Книга: {row["title"]}\nЧитатель: {row["reader"]}\n" +
                             $"Дата выдачи: {Convert.ToDateTime(row["loan_date"]).ToShortDateString()}";
                ReportContainer.Children.Add(CreateCard(info));
            }
        }

        // Метод загрузки отчета по просроченным возвратам
        private void LoadOverdueReport()
        {
            
            var data = Database.GetOverdueLoans();

            // Создание заголовка с количеством записей
            TextBlock header = CreateHeader($"⏰ Просроченные возвраты: {data.Rows.Count}");
            ReportContainer.Children.Add(header);

            // Создание карточек для каждой записи
            foreach (DataRow row in data.Rows)
            {
                string info = $"Читатель: {row["reader"]}\nКнига: {row["title"]}\n" +
                             $"Возврат должен быть: {Convert.ToDateTime(row["return_date"]).ToShortDateString()}";
                ReportContainer.Children.Add(CreateCard(info));
            }
        }

        
        private void LoadReaderActivityReport()
        {
            // Получение данных из базы
            var data = Database.GetReaderActivity();

            // Создание заголовка
            TextBlock header = CreateHeader("📈 Топ активных читателей:");
            ReportContainer.Children.Add(header);

            // Создание карточек для каждого читателя
            foreach (DataRow row in data.Rows)
            {
                string info = $"{row["reader"]} — книг: {row["count"]}";
                ReportContainer.Children.Add(CreateCard(info));
            }
        }

        // Метод создания заголовка отчета
        private TextBlock CreateHeader(string text)
        {
            return new TextBlock
            {
                Text = text, 
                FontSize = 20, 
                FontWeight = FontWeights.Bold, // Жирный шрифт
                Foreground = Brushes.White, 
                Margin = new Thickness(10, 0, 0, 10) 
            };
        }

       
        private Border CreateCard(string content)
        {
            return new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(45, 45, 48)), 
                BorderBrush = Brushes.Gray, 
                BorderThickness = new Thickness(1), 
                CornerRadius = new CornerRadius(6), // Закругленные углы
                Padding = new Thickness(15), 
                Margin = new Thickness(5, 5, 5, 5), 
                Child = new TextBlock // Текстовый блок внутри
                {
                    Text = content,
                    Foreground = Brushes.White, 
                    FontSize = 14, 
                    TextWrapping = TextWrapping.Wrap // Перенос текста
                }
            };
        }
    }