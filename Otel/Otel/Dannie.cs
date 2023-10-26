using System.Data.SQLite;
using System.Data;
using System;
using System.IO;

public class DatabaseHelper
{
    private SQLiteConnection connection;
    private SQLiteDataAdapter dataAdapter;
    private DataTable dataTable;

    public DatabaseHelper(string databasePath)
    {
        string connectionString = $"Data Source={databasePath};Version=3;";
        connection = new SQLiteConnection(connectionString);

        if (!File.Exists(databasePath))
        {
            SQLiteConnection.CreateFile(databasePath);
        }

        connection.Open();

        string createTableQuery = "CREATE TABLE IF NOT EXISTS BDOtel (kient TEXT, nomer TEXT, summa REAL, nomerT TEXT, FIO TEXT)";
        using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void CreateDatabase()
    {
        // Метод для создания базы данных, если она не существует
    }

    public void InsertRevenue(string kient, string nomer, double summa, string nomerT, string FIO)
    {
        string insertQuery = "INSERT INTO BDOtel (kient, nomer, summa, nomerT, FIO) VALUES (@kient, @nomer, @summa, @nomerT, @FIO)";
        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@kient", kient);
            command.Parameters.AddWithValue("@nomer", nomer);
            command.Parameters.AddWithValue("@summa", summa);
            command.Parameters.AddWithValue("@nomerT", nomerT);
            command.Parameters.AddWithValue("@FIO", FIO);

            command.ExecuteNonQuery();
        }
    }

    public bool AddUser(string username, string password, string nomerT, string FIO)
    {
        string insertQuery = "INSERT INTO BDOtel (Login, Password, nomerT, FIO) VALUES (@Login, @Password, @nomerT, @FIO)";
        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@Login", username);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@nomerT", nomerT);
            command.Parameters.AddWithValue("@FIO", FIO);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }

    public double GetTotalRevenue()
    {
        string selectQuery = "SELECT SUM(summa) FROM BDOtel";
        using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
        {
            object result = command.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToDouble(result);
            }
            return 0.0;
        }
    }

    public void Close()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
    }
}