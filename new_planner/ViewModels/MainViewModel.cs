using Dapper;
using Microsoft.Data.SqlClient;
using new_planner.Models;
using new_planner.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace new_planner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private class UserProfileData { public string FirstName { get; set; } public string LastName { get; set; } }
        private string _welcomeMessage = "Загрузка данных...";
        public string WelcomeMessage { get => _welcomeMessage; set { _welcomeMessage = value; OnPropertyChanged(); } }
        private string _userRoleInfo = "";
        public string UserRoleInfo { get => _userRoleInfo; set { _userRoleInfo = value; OnPropertyChanged(); } }
        private bool _isAdmin;
        public bool IsAdmin { get => _isAdmin; set { _isAdmin = value; OnPropertyChanged(); } }
        private string _adminStatistics = "";
        public string AdminStatistics { get => _adminStatistics; set { _adminStatistics = value; OnPropertyChanged(); } }
        public ObservableCollection<Event> PastEvents { get; set; }
        public ObservableCollection<Event> CurrentEvents { get; set; }
        public ObservableCollection<Event> FutureEvents { get; set; }
        private Event? _selectedEvent;
        public Event? SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AssignmentInfo));
            }
        }

        public bool CanAddEvents { get; private set; }
        public bool CanDeleteEvents { get; private set; }
        public bool CanEditEvents { get; private set; }
        public bool ShowCreatorColumn { get; private set; }

        public ICommand AddEventCommand { get; }
        public ICommand EditEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand ManageUsersCommand { get; }

        public MainViewModel()
        {
            PastEvents = new ObservableCollection<Event>();
            CurrentEvents = new ObservableCollection<Event>();
            FutureEvents = new ObservableCollection<Event>();

            var userRole = SessionContext.CurrentUser?.RoleId;
            IsAdmin = (userRole == 1);
            CanAddEvents = (userRole == 1 || userRole == 2 || userRole == 4);
            CanEditEvents = (userRole == 1 || userRole == 2 || userRole == 4);
            CanDeleteEvents = (userRole == 1 || userRole == 2 || userRole == 4);
            ShowCreatorColumn = (userRole == 1 || userRole == 3 || userRole == 4);

            ManageUsersCommand = new RelayCommand(OpenUserManagementWindow, (p) => IsAdmin);
            AddEventCommand = new AsyncRelayCommand(OpenAddEventWindowAsync, (p) => CanAddEvents);
            EditEventCommand = new AsyncRelayCommand(OpenEditEventWindowAsync, (p) => SelectedEvent != null && CanEditEvents);
            DeleteEventCommand = new RelayCommand(DeleteSelectedEvent, (p) => SelectedEvent != null && CanDeleteEvents);
        }

        private async Task OpenAddEventWindowAsync(object? parameter)
        {
            var eventWindow = new EventWindow();
            eventWindow.ShowDialog();
            await LoadEventsAsync();
        }

        private async Task OpenEditEventWindowAsync(object? parameter)
        {
            if (SelectedEvent == null) return;
            var userRole = SessionContext.CurrentUser?.RoleId;
            if ((userRole == 2 || userRole == 4) && SelectedEvent.CreatorUserId != SessionContext.CurrentUser.UserId)
            {
                MessageBox.Show("Вы можете редактировать только свои мероприятия.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var eventWindow = new EventWindow(SelectedEvent);
            eventWindow.ShowDialog();
            await LoadEventsAsync();
        }

        private void DeleteSelectedEvent(object? parameter)
        {
            if (SelectedEvent == null) return;
            var userRole = SessionContext.CurrentUser?.RoleId;
            if ((userRole == 2 || userRole == 4) && SelectedEvent.CreatorUserId != SessionContext.CurrentUser.UserId)
            {
                MessageBox.Show("Вы не можете удалять чужое мероприятие.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Вы уверены, что хотите удалить дело \"{SelectedEvent.Title}\"?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
                    {
                        connection.Execute("DELETE FROM Events WHERE event_id = @EventId", new { SelectedEvent.EventId });
                    }
                    if (CurrentEvents.Contains(SelectedEvent)) { CurrentEvents.Remove(SelectedEvent); }
                    else if (FutureEvents.Contains(SelectedEvent)) { FutureEvents.Remove(SelectedEvent); }
                    else if (PastEvents.Contains(SelectedEvent)) { PastEvents.Remove(SelectedEvent); }
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                await LoadCurrentUserAsync();
                await LoadEventsAsync();
                if (IsAdmin) { await LoadAdminStatisticsAsync(); }
            }
            catch (Exception ex) { MessageBox.Show($"Произошла ошибка при загрузке данных: {ex.ToString()}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private async Task LoadAdminStatisticsAsync()
        {
            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                var totalEvents = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Events;");
                var totalUsers = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users;");
                AdminStatistics = $"Статистика системы:\n- Всего пользователей: {totalUsers}\n- Всего мероприятий: {totalEvents}";
            }
        }

        private async Task LoadCurrentUserAsync()
        {
            if (SessionContext.CurrentUser == null) { WelcomeMessage = "Ошибка: Пользователь не определен."; return; }
            UserRoleInfo = $"Вы вошли в систему как: {SessionContext.CurrentUser.Username}";
            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                var profileData = await connection.QueryFirstOrDefaultAsync<UserProfileData>(@"SELECT p.first_name as FirstName, p.last_name as LastName FROM Users u JOIN UserProfiles p ON u.user_id = p.user_id WHERE u.user_id = @UserId", new { UserId = SessionContext.CurrentUser.UserId });
                if (profileData != null && !string.IsNullOrWhiteSpace(profileData.FirstName) && !string.IsNullOrWhiteSpace(profileData.LastName))
                { WelcomeMessage = $"Добро пожаловать, {profileData.LastName} {profileData.FirstName}!"; }
                else { WelcomeMessage = $"Добро пожаловать, {SessionContext.CurrentUser.Username}!"; }
            }
        }

        public async Task LoadEventsAsync()
        {
            if (SessionContext.CurrentUser == null) return;
            PastEvents.Clear(); CurrentEvents.Clear(); FutureEvents.Clear();
            var currentUser = SessionContext.CurrentUser;

            // ФИНАЛЬНЫЙ, ПОЛНЫЙ И ПРАВИЛЬНЫЙ ЗАПРОС
            var sql = @"SELECT 
                    e.event_id as EventId, e.title as Title, e.description as Description,
                    e.start_time as StartTime, e.end_time as EndTime, e.location as Location,
                    e.creator_user_id as CreatorUserId, e.status_id as StatusId, e.image_path as ImagePath,
                    e.reminder_enabled as ReminderEnabled, e.reminder_minutes_before as ReminderMinutesBefore,
                    e.assignee_user_id as AssigneeUserId, e.assignment_notified as AssignmentNotified,
                    creator.username as CreatorUsername,
                    assignee.username as AssigneeUsername
                FROM Events e
                JOIN Users creator ON e.creator_user_id = creator.user_id
                LEFT JOIN Users assignee ON e.assignee_user_id = assignee.user_id
                JOIN Roles r ON creator.role_id = r.role_id";

            string filterSql = "";

            if (currentUser.RoleId == 1 || currentUser.RoleId == 3) // Админ и Директор
            { /* видят все, фильтр не нужен */ }
            else if (currentUser.RoleId == 4) // Менеджер
            { filterSql = " WHERE e.creator_user_id = @UserId OR e.assignee_user_id = @UserId OR r.role_name = 'User'"; }
            else // Пользователь
            { filterSql = " WHERE e.creator_user_id = @UserId OR e.assignee_user_id = @UserId"; }

            var finalSql = $"{sql}{filterSql} ORDER BY e.start_time";

            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                var allEvents = await connection.QueryAsync<Event>(finalSql, new { UserId = currentUser.UserId });
                var now = DateTime.Now;
                foreach (var ev in allEvents)
                {
                    if (ev.EndTime < now) { PastEvents.Add(ev); }
                    else if (ev.StartTime.Date == DateTime.Today) { CurrentEvents.Add(ev); }
                    else { FutureEvents.Add(ev); }
                }
            }
            await CheckForNewAssignments();
        }

        private void OpenUserManagementWindow(object? parameter)
        {
            var userManagementWindow = new UserManagementView();
            userManagementWindow.ShowDialog();
            _ = LoadAdminStatisticsAsync();
        }

        private async Task CheckForNewAssignments()
        {
            if (SessionContext.CurrentUser == null) return;
            try
            {
                using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    await connection.OpenAsync();
                    var newAssignmentIds = await connection.QueryAsync<int>(
                        @"SELECT event_id FROM Events WHERE assignee_user_id = @UserId AND assignment_notified = 0",
                        new { UserId = SessionContext.CurrentUser.UserId });

                    if (newAssignmentIds.Any())
                    {
                        var newAssignments = await connection.QueryAsync<Event>(
                            "SELECT title FROM Events WHERE event_id IN @EventIds",
                            new { EventIds = newAssignmentIds });
                        var eventNames = string.Join("\n- ", newAssignments.Select(e => e.Title));
                        MessageBox.Show($"Вам назначены новые задачи:\n- {eventNames}", "Новые назначения", MessageBoxButton.OK, MessageBoxImage.Information);

                        await connection.ExecuteAsync(
                            "UPDATE Events SET assignment_notified = 1 WHERE event_id IN @EventIds",
                            new { EventIds = newAssignmentIds });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Ошибка при проверке новых назначений: {ex.Message}"); }
        }

        public string? AssignmentInfo
        {
            get
            {
                if (SelectedEvent == null || SelectedEvent.AssigneeUserId == null) return null;
                var currentUser = SessionContext.CurrentUser;
                if (SelectedEvent.CreatorUserId == currentUser?.UserId && SelectedEvent.AssigneeUserId != currentUser?.UserId)
                { return $"Назначено: {SelectedEvent.AssigneeUsername}"; }
                if (SelectedEvent.AssigneeUserId == currentUser?.UserId && SelectedEvent.CreatorUserId != currentUser?.UserId)
                { return $"Назначено от: {SelectedEvent.CreatorUsername}"; }
                return $"От: {SelectedEvent.CreatorUsername} -> Кому: {SelectedEvent.AssigneeUsername}";
            }
        }
    }
}