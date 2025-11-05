using Dapper;
using Microsoft.Data.SqlClient;
using new_planner.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace new_planner.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        public ObservableCollection<UserDisplayModel> Users { get; set; }
        public ObservableCollection<string> AllRoles { get; set; }
       

        private UserDisplayModel? _selectedUser;
        public UserDisplayModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();

                // =======================================================
                // ПРИНУДИТЕЛЬНО ОБНОВЛЯЕМ СОСТОЯНИЕ КОНКРЕТНЫХ КОМАНД
                // =======================================================
                DeleteUserCommand.RaiseCanExecuteChanged();
                ResetPasswordCommand.RaiseCanExecuteChanged();
            }
        }

        public AsyncRelayCommand SaveChangesCommand { get; }
        public AsyncRelayCommand DeleteUserCommand { get; }
        public AsyncRelayCommand ResetPasswordCommand { get; }
        public UserManagementViewModel()
        {
            Users = new ObservableCollection<UserDisplayModel>();
            AllRoles = new ObservableCollection<string>();

            // Мы больше не используем ICommand, а напрямую создаем AsyncRelayCommand
            SaveChangesCommand = new AsyncRelayCommand(async _ => await SaveChanges());
            DeleteUserCommand = new AsyncRelayCommand(async _ => await DeleteUser(), _ => SelectedUser != null);
            ResetPasswordCommand = new AsyncRelayCommand(async _ => await ResetPassword(), _ => SelectedUser != null);

            _ = LoadData();
        }

        private async Task LoadData()
        {
            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // Загружаем список всех ролей для выпадающего списка
                var roles = await connection.QueryAsync<string>("SELECT role_name FROM Roles");
                AllRoles.Clear();
                foreach (var role in roles) AllRoles.Add(role);

                // Загружаем всех пользователей с их данными
                var users = await connection.QueryAsync<UserDisplayModel>(
                     @"SELECT 
                        u.user_id as UserId, 
                        u.username as Username, 
                        p.first_name as FirstName, 
                        p.last_name as LastName,
                        r.role_name as RoleName
                        FROM Users u
                        LEFT JOIN UserProfiles p ON u.user_id = p.user_id
                        JOIN Roles r ON u.role_id = r.role_id  -- Заменяем LEFT JOIN на обычный JOIN
                        ORDER BY u.user_id");
                Users.Clear();
                foreach (var user in users) Users.Add(user);
            }
        }
        private async Task SaveChanges()
        {
            try
            {
                using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // Мы больше не меняем роль, поэтому обновляем только Users и UserProfiles
                    foreach (var user in Users)
                    {
                        await connection.ExecuteAsync(
                            @"UPDATE u SET u.username = @Username
                      FROM Users u WHERE u.user_id = @UserId;
                      
                      UPDATE p SET p.first_name = @FirstName, p.last_name = @LastName
                      FROM UserProfiles p WHERE p.user_id = @UserId;",
                            user); // user содержит UserId, Username, FirstName, LastName
                    }
                }
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task DeleteUser()
        {
            if (SelectedUser == null) return;
            if (SelectedUser.UserId == SessionContext.CurrentUser?.UserId)
            {
                MessageBox.Show("Вы не можете удалить свою собственную учетную запись.", "Операция невозможна", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {SelectedUser.Username}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // Каскадное удаление само удалит запись из UserProfiles
                    await connection.ExecuteAsync("DELETE FROM Users WHERE user_id = @UserId", new { SelectedUser.UserId });
                }
                Users.Remove(SelectedUser);
            }
        }

        private async Task ResetPassword()
        {
            if (SelectedUser == null) return;
            // Здесь нужен простой диалог ввода . Для WPF нет встроенного, поэтому сделаем простую заглушку
            // В реальном проекте здесь было бы кастомное окно
            var newPassword = "newpassword"; // Заглушка. В идеале - окно ввода.
            MessageBox.Show($"Пароль для {SelectedUser.Username} будет сброшен на '{newPassword}'. Это временная заглушка.", "Сброс пароля");
            var passwordHash = HashPassword(newPassword);
            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                await connection.ExecuteAsync("UPDATE Users SET password_hash = @PasswordHash WHERE user_id = @UserId",
                    new { PasswordHash = passwordHash, SelectedUser.UserId });
            }
            MessageBox.Show("Пароль успешно сброшен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
