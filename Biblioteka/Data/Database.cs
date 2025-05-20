using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Biblioteka.Data
{
    
public static class Database
{
    
    private static string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

   
    public static DataTable GetBooks(string titleFilter = "")
    {
        // Использование подключения (автоматически закрывается после использования)
        using (var conn = new MySqlConnection(connStr))
        {
            // Открытие соединения с БД
            conn.Open();
            
            // Создание SQL-команды для выборки книг с соединением таблиц
            var cmd = new MySqlCommand(@"
                SELECT 
                    b.id_book, 
                    b.title, 
                    b.year_published, 
                    b.isbn,
                    c.name_category AS category,
                    CONCAT(a.last_name, ' ', a.first_name, ' ', a.patronymic) AS author,
                    b.total_copies,
                    b.available_copies,
                    b.id_category,
                    b.id_author
                FROM books b
                JOIN categories c ON b.id_category = c.id_category
                JOIN authors a ON b.id_author = a.id_author
                WHERE b.title LIKE @filter", conn);

            // Добавление параметра фильтра (поиск по части названия)
            cmd.Parameters.AddWithValue("@filter", "%" + titleFilter + "%");

            // Создание адаптера для выполнения запроса
            var adapter = new MySqlDataAdapter(cmd);
            
            // Создание таблицы для результатов
            var table = new DataTable();
            
            // Заполнение таблицы данными из БД
            adapter.Fill(table);
            
            
            return table;
        }
    }

    // Метод получения всех категорий
    public static DataTable GetCategories()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Простой запрос для получения id и названий категорий
            var cmd = new MySqlCommand("SELECT id_category, name_category FROM categories", conn);
            
            var adapter = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    // Метод получения всех авторов
    public static DataTable GetAuthors()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Запрос с объединением ФИО автора в одну строку
            var cmd = new MySqlCommand("SELECT id_author, CONCAT(last_name, ' ', first_name, ' ', patronymic) AS fullname FROM authors", conn);
            
            var adapter = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    // Метод добавления новой книги
    public static void InsertBook(DataRowView row)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // SQL-команда для вставки новой книги
            var cmd = new MySqlCommand(@"INSERT INTO books 
                (title, year_published, isbn, id_category, id_author, total_copies, available_copies)
                VALUES (@title, @year, @isbn, @cat, @auth, @total, @avail)", conn);

            // Добавление параметров из переданной строки данных
            cmd.Parameters.AddWithValue("@title", row["title"]);
            cmd.Parameters.AddWithValue("@year", row["year_published"]);
            cmd.Parameters.AddWithValue("@isbn", row["isbn"]);
            cmd.Parameters.AddWithValue("@cat", row["id_category"]);
            cmd.Parameters.AddWithValue("@auth", row["id_author"]);
            cmd.Parameters.AddWithValue("@total", row["total_copies"]);
            cmd.Parameters.AddWithValue("@avail", row["available_copies"]);
            
            // Выполнение команды (без возврата результатов)
            cmd.ExecuteNonQuery();
        }
    }

    // Метод обновления информации о книге
    public static void UpdateBook(DataRowView row)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // SQL-команда для обновления всех полей книги
            var cmd = new MySqlCommand(@"UPDATE books SET 
                title=@title, year_published=@year, isbn=@isbn,
                id_category=@cat, id_author=@auth,
                total_copies=@total, available_copies=@avail
                WHERE id_book=@id", conn);

            // Добавление параметров, включая ID книги для условия WHERE
            cmd.Parameters.AddWithValue("@id", row["id_book"]);
            cmd.Parameters.AddWithValue("@title", row["title"]);
            cmd.Parameters.AddWithValue("@year", row["year_published"]);
            cmd.Parameters.AddWithValue("@isbn", row["isbn"]);
            cmd.Parameters.AddWithValue("@cat", row["id_category"]);
            cmd.Parameters.AddWithValue("@auth", row["id_author"]);
            cmd.Parameters.AddWithValue("@total", row["total_copies"]);
            cmd.Parameters.AddWithValue("@avail", row["available_copies"]);
            
            cmd.ExecuteNonQuery();
        }
    }

    // Метод удаления книги по ID
    public static void DeleteBook(int id)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Простая команда удаления по ID
            var cmd = new MySqlCommand("DELETE FROM books WHERE id_book = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    // Метод получения читателей с фильтрацией по фамилии
    public static DataTable GetReaders(string filter = "")
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Запрос с объединением ФИО и фильтрацией
            var cmd = new MySqlCommand(@"
        SELECT id_reader, 
               last_name, 
               first_name, 
               patronymic,
               passport, phone, email,
               CONCAT(last_name, ' ', first_name, ' ', patronymic) AS full_name
        FROM readers
        WHERE last_name LIKE @filter", conn);
            
            cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");

            var adapter = new MySqlDataAdapter(cmd);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }
    }

    // Метод добавления нового читателя
    public static void InsertReader(DataRowView row)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Команда вставки со всеми полями читателя
            var cmd = new MySqlCommand("INSERT INTO readers (last_name, first_name, patronymic, passport, phone, email) VALUES (@l, @f, @p, @pass, @ph, @e)", conn);
            
            // Параметры из переданной строки
            cmd.Parameters.AddWithValue("@l", row["last_name"]);
            cmd.Parameters.AddWithValue("@f", row["first_name"]);
            cmd.Parameters.AddWithValue("@p", row["patronymic"]);
            cmd.Parameters.AddWithValue("@pass", row["passport"]);
            cmd.Parameters.AddWithValue("@ph", row["phone"]);
            cmd.Parameters.AddWithValue("@e", row["email"]);
            
            cmd.ExecuteNonQuery();
        }
    }

    // Метод обновления данных читателя
    public static void UpdateReader(DataRowView row)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Команда обновления всех полей читателя
            var cmd = new MySqlCommand("UPDATE readers SET last_name=@l, first_name=@f, patronymic=@p, passport=@pass, phone=@ph, email=@e WHERE id_reader=@id", conn);
            
            cmd.Parameters.AddWithValue("@id", row["id_reader"]);
            cmd.Parameters.AddWithValue("@l", row["last_name"]);
            cmd.Parameters.AddWithValue("@f", row["first_name"]);
            cmd.Parameters.AddWithValue("@p", row["patronymic"]);
            cmd.Parameters.AddWithValue("@pass", row["passport"]);
            cmd.Parameters.AddWithValue("@ph", row["phone"]);
            cmd.Parameters.AddWithValue("@e", row["email"]);
            
            cmd.ExecuteNonQuery();
        }
    }

    // Метод удаления читателя по ID
    public static void DeleteReader(int id)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            var cmd = new MySqlCommand("DELETE FROM readers WHERE id_reader=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    // Метод получения всех выдач книг
    public static DataTable GetLoans()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Сложный запрос с соединением нескольких таблиц
            var query = @"
        SELECT l.id_loan, b.title AS book_title,
               CONCAT(r.last_name, ' ', r.first_name) AS reader_name,
               CONCAT(e.last_name, ' ', e.first_name) AS employee_name,
               l.loan_date, l.return_date, l.actual_return_date
        FROM loans l
        JOIN books b ON l.id_book = b.id_book
        JOIN readers r ON l.id_reader = r.id_reader
        JOIN employees e ON l.id_employee = e.id_employee";
            
            var adapter = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    // Метод получения доступных книг (которые можно выдать)
    public static DataTable GetAvailableBooks()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Простой запрос для книг с available_copies > 0
            var query = "SELECT id_book, title FROM books WHERE available_copies > 0";
            
            var adapter = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    // Метод получения списка сотрудников
    public static DataTable GetEmployees()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Запрос с объединением ФИО сотрудника
            var query = "SELECT id_employee, CONCAT(last_name, ' ', first_name) AS full_name FROM employees";
            
            var adapter = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    // Метод оформления выдачи книги
    public static void InsertLoan(int bookId, int readerId, int employeeId, DateTime loanDate, DateTime returnDate)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();

            // 1. Добавление записи о выдаче
            var cmd = new MySqlCommand(@"
        INSERT INTO loans (id_book, id_reader, id_employee, loan_date, return_date)
        VALUES (@b, @r, @e, @ld, @rd)", conn);
            
            cmd.Parameters.AddWithValue("@b", bookId);
            cmd.Parameters.AddWithValue("@r", readerId);
            cmd.Parameters.AddWithValue("@e", employeeId);
            cmd.Parameters.AddWithValue("@ld", loanDate);
            cmd.Parameters.AddWithValue("@rd", returnDate);
            cmd.ExecuteNonQuery();

            // 2. Уменьшение количества доступных книг
            var cmd2 = new MySqlCommand("UPDATE books SET available_copies = available_copies - 1 WHERE id_book = @b", conn);
            cmd2.Parameters.AddWithValue("@b", bookId);
            cmd2.ExecuteNonQuery();
        }
    }

    // Метод отметки возврата книги
    public static void MarkLoanReturned(int loanId)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();

            // 1. Получение ID книги по ID выдачи
            var getCmd = new MySqlCommand("SELECT id_book FROM loans WHERE id_loan = @id", conn);
            getCmd.Parameters.AddWithValue("@id", loanId);
            int bookId = Convert.ToInt32(getCmd.ExecuteScalar());

            // 2. Обновление даты возврата (текущая дата)
            var updateCmd = new MySqlCommand("UPDATE loans SET actual_return_date = CURDATE() WHERE id_loan = @id", conn);
            updateCmd.Parameters.AddWithValue("@id", loanId);
            updateCmd.ExecuteNonQuery();

            // 3. Увеличение количества доступных книг
            var bookCmd = new MySqlCommand("UPDATE books SET available_copies = available_copies + 1 WHERE id_book = @b", conn);
            bookCmd.Parameters.AddWithValue("@b", bookId);
            bookCmd.ExecuteNonQuery();
        }
    }

    // Метод получения книг, выданных за последние 30 дней
    public static DataTable GetIssuedBooksLastMonth()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Запрос с фильтрацией по дате (последние 30 дней)
            string query = @"
        SELECT b.title, 
               CONCAT(r.last_name, ' ', r.first_name) AS reader,
               l.loan_date
        FROM loans l
        JOIN books b ON l.id_book = b.id_book
        JOIN readers r ON l.id_reader = r.id_reader
        WHERE l.loan_date >= CURDATE() - INTERVAL 30 DAY";
            
            var adapter = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    // Метод получения просроченных выдач
    public static DataTable GetOverdueLoans()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Запрос для поиска не возвращенных книг с истекшим сроком
            string query = @"
        SELECT b.title,
               CONCAT(r.last_name, ' ', r.first_name) AS reader,
               l.return_date
        FROM loans l
        JOIN books b ON l.id_book = b.id_book
        JOIN readers r ON l.id_reader = r.id_reader
        WHERE l.return_date < CURDATE() AND l.actual_return_date IS NULL";
            
            var adapter = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    // Метод получения статистики активности читателей
    public static DataTable GetReaderActivity()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            
            // Запрос с группировкой и сортировкой (топ-10 активных читателей)
            string query = @"
        SELECT CONCAT(r.last_name, ' ', r.first_name) AS reader,
               COUNT(*) AS count
        FROM loans l
        JOIN readers r ON l.id_reader = r.id_reader
        GROUP BY l.id_reader
        ORDER BY count DESC
        LIMIT 10";
            
            var adapter = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}

}