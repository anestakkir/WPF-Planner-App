// Файл: ViewModels/EventViewModel.cs
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using new_planner.Models;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace new_planner.ViewModels
{
    public class EventViewModel : ViewModelBase
    {
        private int? _eventId;

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public bool CanAssign { get; private set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public string StartHours { get; set; } = DateTime.Now.Hour.ToString("00");
        public string StartMinutes { get; set; } = DateTime.Now.Minute.ToString("00");
        public DateTime EndDate { get; set; } = DateTime.Today;
        public string EndHours { get; set; } = DateTime.Now.AddHours(1).Hour.ToString("00");
        public string EndMinutes { get; set; } = DateTime.Now.Minute.ToString("00");

        private string? _imagePath;
        public string? ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        private bool _reminderEnabled;
        public bool ReminderEnabled
        {
            get => _reminderEnabled;
            set { _reminderEnabled = value; OnPropertyChanged(); }
        }

        public string? ReminderMinutesBefore { get; set; } = "15"; // Начальное значение для удобства

        public ICommand SaveCommand { get; }
        public ICommand SelectImageCommand { get; }
        public event Action? OnSaveSuccess;

        
        private bool _isAssigning;
        public bool IsAssigning
        {
            get => _isAssigning;
            set { _isAssigning = value; OnPropertyChanged(); }
        }

        public ObservableCollection<AssignableUser> AssignableUsers { get; set; }
        public AssignableUser? SelectedAssignableUser { get; set; }
       

        public EventViewModel()
        {
            SaveCommand = new RelayCommand(SaveChanges, CanSave);
            SelectImageCommand = new RelayCommand(SelectImage);

            AssignableUsers = new ObservableCollection<AssignableUser>();
            _ = LoadAssignableUsers();

            var userRole = SessionContext.CurrentUser?.RoleId;
            CanAssign = (userRole == 1 || userRole == 4);
        }

        public EventViewModel(Event eventToEdit)
        {
            _eventId = eventToEdit.EventId;
            Title = eventToEdit.Title;
            Description = eventToEdit.Description ?? "";
            Location = eventToEdit.Location ?? "";
            ImagePath = eventToEdit.ImagePath;
            StartDate = eventToEdit.StartTime.Date;
            StartHours = eventToEdit.StartTime.Hour.ToString("00");
            StartMinutes = eventToEdit.StartTime.Minute.ToString("00");
            EndDate = eventToEdit.EndTime.Date;
            EndHours = eventToEdit.EndTime.Hour.ToString("00");
            EndMinutes = eventToEdit.EndTime.Minute.ToString("00");

            ReminderEnabled = eventToEdit.ReminderEnabled;
            ReminderMinutesBefore = eventToEdit.ReminderEnabled ? eventToEdit.ReminderMinutesBefore.ToString() : null;

            SaveCommand = new RelayCommand(SaveChanges, CanSave);
            SelectImageCommand = new RelayCommand(SelectImage);

            AssignableUsers = new ObservableCollection<AssignableUser>();
            _ = LoadAssignableUsers(eventToEdit.AssigneeUserId);

            // Если у дела есть назначенный, включаем галочку
            if (eventToEdit.AssigneeUserId.HasValue)
            {
                IsAssigning = true;
            }
        }

        private async Task LoadAssignableUsers(int? currentAssigneeId = null)
        {
            using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // Менеджер может назначать дела только обычным Пользователям (role_id = 2)
                var users = await connection.QueryAsync<AssignableUser>(
                    "SELECT u.user_id as UserId, u.username as Username FROM Users u JOIN Roles r ON u.role_id = r.role_id WHERE r.role_name = 'User'");

                AssignableUsers.Clear();
                foreach (var user in users)
                {
                    AssignableUsers.Add(user);
                }

                // Если мы редактируем дело, нужно выбрать текущего назначенного в списке
                if (currentAssigneeId.HasValue)
                {
                    SelectedAssignableUser = AssignableUsers.FirstOrDefault(u => u.UserId == currentAssigneeId.Value);
                }
            }
        }


        private void SelectImage(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите картинку для мероприятия",
                Filter = "Файлы изображений (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }

        private void SaveChanges(object? parameter)
        {
            // --- Шаг 1: Валидация ввода ---
            if (!int.TryParse(StartHours, out int startH) || !int.TryParse(StartMinutes, out int startM) ||
                !int.TryParse(EndHours, out int endH) || !int.TryParse(EndMinutes, out int endM) ||
                startH > 23 || startM > 59 || endH > 23 || endM > 59)
            {
                MessageBox.Show("Пожалуйста, введите корректное время.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startTime = StartDate.Date.AddHours(startH).AddMinutes(startM);
            DateTime endTime = EndDate.Date.AddHours(endH).AddMinutes(endM);

            if (endTime <= startTime)
            {
                MessageBox.Show("Время окончания должно быть позже времени начала.", "Ошибка логики", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? reminderMinutes = null;
            if (ReminderEnabled)
            {
                if (!int.TryParse(ReminderMinutesBefore, out int parsedMinutes) || parsedMinutes < 0)
                {
                    MessageBox.Show("Пожалуйста, введите корректное количество минут для напоминания (положительное число).", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                reminderMinutes = parsedMinutes;
            }

            // --- Шаг 2: Определяем, кому назначено дело ---
            int? assigneeId = null;
            if (IsAssigning)
            {
                if (SelectedAssignableUser == null)
                {
                    MessageBox.Show("Пожалуйста, выберите сотрудника для назначения.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                assigneeId = SelectedAssignableUser.UserId;
            }

            try
            {
                using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();

                    // --- Шаг 3: Проверка накладок ---
                    var sqlBuilder = new StringBuilder("SELECT title FROM Events WHERE creator_user_id = @UserId AND start_time < @EndTime AND end_time > @StartTime");
                    if (_eventId.HasValue)
                    {
                        sqlBuilder.Append(" AND event_id <> @EventId");
                    }
                    var conflictingEventTitle = connection.QueryFirstOrDefault<string>(sqlBuilder.ToString(), new { UserId = SessionContext.CurrentUser!.UserId, StartTime = startTime, EndTime = endTime, EventId = _eventId });

                    if (conflictingEventTitle != null)
                    {
                        var result = MessageBox.Show($"Внимание! Это время пересекается с вашим делом \"{conflictingEventTitle}\".\n\nВсе равно сохранить?", "Обнаружена накладка", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    // --- Шаг 4: Сохранение в базу ---
                    if (_eventId == null) // Создание нового дела
                    {
                        connection.Execute(
                            @"INSERT INTO Events 
                        (title, description, start_time, end_time, location, creator_user_id, status_id, image_path, reminder_enabled, reminder_minutes_before, assignee_user_id)
                      VALUES 
                        (@Title, @Description, @StartTime, @EndTime, @Location, @CreatorUserId, @StatusId, @ImagePath, @ReminderEnabled, @ReminderMinutesBefore, @AssigneeUserId);",
                            new
                            {
                                Title,
                                Description,
                                StartTime = startTime,
                                EndTime = endTime,
                                Location,
                                CreatorUserId = SessionContext.CurrentUser!.UserId,
                                StatusId = 1,
                                ImagePath,
                                ReminderEnabled,
                                ReminderMinutesBefore = reminderMinutes,
                                AssigneeUserId = assigneeId // Добавлено новое поле
                            });
                        MessageBox.Show("Мероприятие успешно создано!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else // Редактирование существующего
                    {
                        connection.Execute(
                            @"UPDATE Events SET 
                        title = @Title, 
                        description = @Description, 
                        start_time = @StartTime, 
                        end_time = @EndTime, 
                        location = @Location, 
                        image_path = @ImagePath, 
                        reminder_enabled = @ReminderEnabled, 
                        reminder_minutes_before = @ReminderMinutesBefore,
                        assignee_user_id = @AssigneeUserId -- Добавлено новое поле
                      WHERE event_id = @EventId",
                            new
                            {
                                Title,
                                Description,
                                StartTime = startTime,
                                EndTime = endTime,
                                Location,
                                ImagePath,
                                ReminderEnabled,
                                ReminderMinutesBefore = reminderMinutes,
                                AssigneeUserId = assigneeId, // Добавлено новое поле
                                EventId = _eventId
                            });
                        MessageBox.Show("Мероприятие успешно обновлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                OnSaveSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения в базу данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave(object? parameter) => !string.IsNullOrWhiteSpace(Title);
    }
}