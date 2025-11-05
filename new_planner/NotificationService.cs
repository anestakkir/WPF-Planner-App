// Файл: NotificationService.cs
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Windows;

namespace new_planner
{
    public static class NotificationService
    {
        // Вместо простого флага, храним ID последнего дела, для которого было показано уведомление
        private static int? _lastNotifiedEventId = null;

        public static void CheckEvents(int userId)
        {
            ShowUpcomingEventReminder(userId);
        }

       

        private static void ShowUpcomingEventReminder(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    var now = DateTime.Now;
                    var upcomingEvent = connection.QueryFirstOrDefault<Models.Event>(
                         @"SELECT TOP 1 
                            event_id as EventId,
                            title as Title,
                            description as Description,
                            start_time as StartTime,
                            end_time as EndTime,
                            location as Location,
                            creator_user_id as CreatorUserId,
                            status_id as StatusId,
                            image_path as ImagePath
                          FROM Events 
                          WHERE start_time > @Now AND creator_user_id = @UserId 
                          ORDER BY start_time ASC",
                    new { Now = now, UserId = userId });

                    if (upcomingEvent != null)
                    {
                        var timeUntilEvent = upcomingEvent.StartTime - now;

                        if (timeUntilEvent.TotalMinutes <= 15 && upcomingEvent.EventId != _lastNotifiedEventId)
                        {
                            // Используем правильное форматирование
                            MessageBox.Show(
                                $"Напоминание!\n\nДело: \"{upcomingEvent.Title}\"\nНачало: {upcomingEvent.StartTime:HH:mm}",
                                "Ближайшее дело",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                            _lastNotifiedEventId = upcomingEvent.EventId;
                        }
                    }
                }
            }
            catch { /* Игнорируем ошибки в фоновом режиме */ }
        }
    }
}