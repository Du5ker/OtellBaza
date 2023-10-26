using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

public class UserDatabaseHelper
{
    private SQLiteConnection connection;
    private SQLiteDataAdapter dataAdapter;
    private DataTable dataTable;

    public UserDatabaseHelper(string databasePath)
    {
        string connectionString = $"Data Source={databasePath};Version=3;";
        connection = new SQLiteConnection(connectionString);

        if (!File.Exists(databasePath))
        {
            SQLiteConnection.CreateFile(databasePath);
        }

        connection.Open();

        string createTableQuery = "CREATE TABLE IF NOT EXISTS Users (username TEXT, password TEXT, nomerT TEXT, FIO TEXT)";
        using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public bool ValidateUser(string username, string password)
    {
        string selectQuery = "SELECT COUNT(*) FROM Users WHERE username = @username AND password = @password";
        using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            long count = (long)command.ExecuteScalar();
            return count > 0;
        }
    }

    public bool AddUser(string username, string password, string nomerT, string FIO)
    {
        try
        {
            // Проверка наличия пользователя в базе данных
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE username = @username";
            using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@username", username);
                int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (userCount > 0)
                {
                    // Пользователь с таким именем уже существует
                    return false;
                }
            }

            // Если пользователь с таким именем не существует, добавьте его
            string insertQuery = "INSERT INTO Users (username, password, nomerT, FIO) VALUES (@username, @password, @nomerT, @FIO)";
            using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@nomerT", nomerT);
                command.Parameters.AddWithValue("@FIO", FIO);

                command.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            // Обработка и логирование ошибки
            Console.WriteLine("Ошибка при добавлении пользователя: " + ex.Message);
            return false;
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