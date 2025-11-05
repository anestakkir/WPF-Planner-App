using Dapper;
using Microsoft.Data.SqlClient;
using new_planner.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace new_planner.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public event Action? OnLoginSuccess;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(LoginUser, CanLogin);
        }

        private bool CanLogin(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            return !string.IsNullOrWhiteSpace(Username) &&
                   passwordBox != null &&
                   passwordBox.Password.Length > 0;
        }

        private void LoginUser(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox == null) return;

            string password = passwordBox.Password;
            string passwordHash = HashPassword(password);

            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                var user = connection.QueryFirstOrDefault<User>(
                    @"SELECT 
                    user_id as UserId,
                    username as Username,
                    password_hash as PasswordHash,
                    role_id as RoleId
                    FROM Users 
                    WHERE username = @Username AND password_hash = @PasswordHash",
                new { Username, PasswordHash = passwordHash });
                
                if (user != null)
                {
                    SessionContext.CurrentUser = user;
                    OnLoginSuccess?.Invoke();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
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
