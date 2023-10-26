using System;
using System.Data;
using System.Data.SQLite;

public class DatabaseHelperr
{
    private string connectionString;

    public DatabaseHelperr(string databasePath)
    {
        connectionString = $"Data Source={databasePath};Version=3;";
    }

    public void CreateDatabase()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Revenue (Value REAL)";
            using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
            {
                createTableCommand.ExecuteNonQuery();
            }
        }
    }

    public void InsertRevenue(double value)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string insertQuery = "INSERT INTO Revenue (Value) VALUES (@Value)";
            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@Value", value);
                insertCommand.ExecuteNonQuery();
            }
        }
    }

    public double GetTotalRevenue()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string selectTotalQuery = "SELECT SUM(Value) FROM Revenue";
            using (SQLiteCommand selectTotalCommand = new SQLiteCommand(selectTotalQuery, connection))
            {
                object result = selectTotalCommand.ExecuteScalar();
                if (result is double)
                {
                    return (double)result;
                }
                return 0;
            }
        }
    }
}
