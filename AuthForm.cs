using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace estate_agency
{
    public partial class AuthForm : Form
    {
        NpgsqlConnection connection;
        public static bool authSuccessful = false;

        public AuthForm()
        {
            InitializeComponent();

            try
            {
                connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=estate_agency;Username=postgres;Password=1111;");
                connection.Open();
            }
            catch (NpgsqlException npgex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {npgex.Message}\nПроверьте настройки подключения.");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка: {ex.Message}");
                Environment.Exit(1); 
            }
        }

        private void AuthBtn_Click(object sender, EventArgs e)
        {
            
                string username = loginInput.Text.Trim();
                string password = passInput.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Логин и пароль не могут быть пустыми");
                    return;
                }

                if (UserExists(username, password))
                {
                    try
                    {
                        connection.Close();
                    }
                    catch (Exception closeEx)
                    {
                        MessageBox.Show($"Ошибка при закрытии соединения: {closeEx.Message}");
                    }

                    MessageBox.Show($"Добро пожаловать, {username}!");
                    authSuccessful = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Допущена ошибка в данных пользователя.");
                }
            
        }

        private bool UserExists(string username, string password)
        {
            try
            {
                using (var command = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM сотрудники WHERE логин = @Username AND пароль = @Password",
                    connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
            catch (NpgsqlException npgex)
            {
                MessageBox.Show($"Ошибка базы данных: {npgex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке пользователя: {ex.Message}");
                return false;
            }
        }

 
        private void AuthForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при закрытии соединения: {ex.Message}");
            }
            finally
            {
                connection?.Dispose();
            }
        }
    }
}