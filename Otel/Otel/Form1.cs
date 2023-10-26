using Otel;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BDOtelApp
{
    public partial class Form1 : Form
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteDataAdapter dataAdapter;
        private DataTable dataTable;

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
        }

        private Random random = new Random();

        private void GenerateRandomNumber()
        {
            int randomNumber = random.Next(5000, 7001);
            textBox7.Text = randomNumber.ToString();
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=BDOtel.db;Version=3;";
            connection = new SQLiteConnection(connectionString);

            if (!File.Exists("BDOtel.db"))
            {
                SQLiteConnection.CreateFile("BDOtel.db");
            }

            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS BDOtel (kient TEXT, nomer TEXT, summa REAL, nomerT TEXT, FIO TEXT)";
            command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        private void LoadData()
        {
            string selectQuery = "SELECT * FROM BDOtel";
            dataAdapter = new SQLiteDataAdapter(selectQuery, connection);
            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;
        }

        private void InsertData(string kient, string nomer, double summa, string nomerT, string FIO)
        {
            try
            {
                string insertQuery = "INSERT INTO BDOtel (kient, nomer, summa, nomerT, FIO) VALUES (@kient, @nomer, @summa, @nomerT, @FIO)";
                command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@kient", kient);
                command.Parameters.AddWithValue("@nomer", nomer);
                command.Parameters.AddWithValue("@summa", summa);
                command.Parameters.AddWithValue("@nomerT", nomerT);
                command.Parameters.AddWithValue("@FIO", FIO);
                command.ExecuteNonQuery();

                LoadData(); // Обновление данных
            }
            catch (SQLiteException ex)
            {
                // Обработка и логирование ошибки
                MessageBox.Show("Ошибка при добавлении данных в базу данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteData(int rowNumber)
        {
            string deleteQuery = "DELETE FROM BDOtel WHERE ROWID = @rowNumber";
            command = new SQLiteCommand(deleteQuery, connection);
            command.Parameters.AddWithValue("@rowNumber", rowNumber);
            command.ExecuteNonQuery();

            LoadData(); // Обновление данных
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kient = textBox1.Text;
            string nomer = textBox2.Text;
            string summaText = textBox3.Text;
            string nomerT = NomerT.Text; // Получаем значение "nomerT" из TextBox
            string FIO = FIO0.Text; // Получаем значение "FIO" из TextBox

            if (string.IsNullOrEmpty(kient) || string.IsNullOrEmpty(nomer) || string.IsNullOrEmpty(summaText) || string.IsNullOrEmpty(nomerT) || string.IsNullOrEmpty(FIO))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(summaText, out double summa))
            {
                MessageBox.Show("Сумма должна быть числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                InsertData(kient, nomer, summa, nomerT, FIO);
                MessageBox.Show("Данные успешно добавлены в базу данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Ошибка при добавлении данных в базу данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Получение общей суммы и отображение ее в TextBox10
            DatabaseHelper db = new DatabaseHelper("BDOtel.db");

            // Создание базы данных, если она еще не существует
            db.CreateDatabase();

            // Запись значения в базу данных
            db.InsertRevenue(kient, nomer, summa, nomerT, FIO);

            // Получение общей суммы и отображение ее в TextBox10
            double totalRevenue = db.GetTotalRevenue();
            textBox10.Text = totalRevenue.ToString("N1"); // Форматирование числа с двумя знаками после запятой
        }

        //Добавив блок try-catch в метод button1_Click, вы сможете увидеть сообщение об ошибке, которое может помочь вам определить причину проблемы.Если у вас возникают ошибки, пожалуйста, предоставьте сообщение об ошибке, чтобы я мог помочь вам дальше.


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxRowNumber.Text, out int rowNumber))
            {
                DeleteData(rowNumber);
                MessageBox.Show("Строка с номером " + rowNumber + " удалена из базы данных.");
            }
            else
            {
                MessageBox.Show("Номер строки должен быть целым числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string username = textBox5.Text;
            string password = textBox6.Text;

            if (username == "admin" && password == "123")
            {
                // Скрываем панель
                panel1.Hide();
            }
            else if (username == "yborshik" && password == "123")
            {
                panel2.Visible = true;
            }
            else
            {
                string nomerT = textBoxNomerT.Text;
                string FIO = textBoxFIO.Text;

                UserDatabaseHelper db = new UserDatabaseHelper("Users.db"); // Замените на свой путь к базе данных

                if (db.ValidateUser(username, password))
                {
                    userrr.Visible = true;
                }
                else
                {
                    // Выводим сообщение об ошибке
                    MessageBox.Show("Не удалось войти. Неправильный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                db.Close();
            }
        }


        private void registration_Click(object sender, EventArgs e)
        {
            panel3.Hide();
            string username = textBoxl.Text;
            string password = textBoxp.Text;
            string nomerT = textBoxNomerTt.Text; // Получаем значение "nomerT" из TextBox
            string FIO = textBoxFIOo.Text; // Получаем значение "FIO" из TextBox

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Используем UserDatabaseHelper для добавления пользователя в базу данных
                UserDatabaseHelper db = new UserDatabaseHelper("Users.db"); // Замените на свой путь к базе данных

                if (db.AddUser(username, password, nomerT, FIO))
                {
                    MessageBox.Show("Пользователь успешно добавлен в базу данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении пользователя в базу данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                db.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegistrationPanel_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox4.Text, out int rowNumber))
            {
                DeleteData(rowNumber);
                MessageBox.Show("Строка с номером " + rowNumber + " удалена из базы данных.");
            }
            else
            {
                MessageBox.Show("Номер строки должен быть целым числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            GenerateRandomNumber();
            string kient = textBox9.Text;
            string nomer = textBox8.Text;
            string summaText = textBox7.Text;
            string nomerT = textBox13.Text; // Получаем значение "nomerT" из TextBox
            string FIO = textBox14.Text; // Получаем значение "FIO" из TextBox

            if (string.IsNullOrEmpty(kient) || string.IsNullOrEmpty(nomer) || string.IsNullOrEmpty(summaText) || string.IsNullOrEmpty(nomerT) || string.IsNullOrEmpty(FIO))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(summaText, out double summa))
            {
                MessageBox.Show("Сумма должна быть числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Создание экземпляра класса DatabaseHelper с путем к базе данных
                DatabaseHelper db = new DatabaseHelper("BDOtel.db");

                // Создание базы данных, если она еще не существует
                db.CreateDatabase();

                // Запись значения в базу данных
                db.InsertRevenue(kient, nomer, summa, nomerT, FIO);

                // Получение общей суммы и отображение ее в TextBox10
                double totalRevenue = db.GetTotalRevenue();
                textBox10.Text = totalRevenue.ToString("N2"); // Форматирование числа с двумя знаками после запятой

                MessageBox.Show("Данные успешно добавлены в базу данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Ошибка при добавлении данных в базу данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Обновление данных в DataGridView
            LoadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            userrr.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCopyText_Click(object sender, EventArgs e)
        {
            textBox12.Text = textBox11.Text; // Копирование текста из textBox15 в textBox16
            textBox11.Text = string.Empty; // Очистка textBox15
        }
    }
}
