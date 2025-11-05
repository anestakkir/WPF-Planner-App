using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dapper;
using Microsoft.Data.SqlClient;

namespace new_planner.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private string _username = "";
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        private string _firstName = "";
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }

        private string _lastName = "";
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public event Action? OnRegistrationSuccess;

        public RegistrationViewModel()
        {
            RegisterCommand = new RelayCommand(RegisterUser, CanRegister);
        }

        private bool CanRegister(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   passwordBox != null &&
                   passwordBox.Password.Length > 0;
        }

        private void RegisterUser(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox == null) return;
            string password = passwordBox.Password;

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string passwordHash = HashPassword(password);
            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {

                connection.Open();

                var existingUser = connection.QueryFirstOrDefault<int?>("SELECT user_id FROM Users WHERE username = @Username", new { Username });
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким именем уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var newUserId = connection.ExecuteScalar<int>(
                            @"INSERT INTO Users (username, password_hash, role_id) VALUES (@Username, @PasswordHash, @RoleId); SELECT SCOPE_IDENTITY();",
                            new { Username, PasswordHash = passwordHash, RoleId = 2 }, transaction);

                        connection.Execute(
                            @"INSERT INTO UserProfiles (user_id, first_name, last_name) VALUES (@UserId, @FirstName, @LastName);",
                            new { UserId = newUserId, FirstName, LastName }, transaction);

                        transaction.Commit();
                        MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        OnRegistrationSuccess?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Произошла ошибка при регистрации: {ex.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
