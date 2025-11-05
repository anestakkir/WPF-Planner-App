using System;

namespace new_planner.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public int CreatorUserId { get; set; }
        public int StatusId { get; set; }

        public int? AssigneeUserId { get; set; }

        public string? AssigneeUsername { get; set; }

        public bool AssignmentNotified {get;  set; }

        public string? ImagePath { get; set; }

        public bool ReminderEnabled { get; set; }
        public int? ReminderMinutesBefore { get; set; }

        public string CreatorUsername { get; set; } = string.Empty;
    }
}
