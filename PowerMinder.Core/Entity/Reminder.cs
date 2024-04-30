using PowerMinder.Core.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerMinder.Core.Entity
{
    [Table("Reminder")]
    public class Reminder : BaseEntity
    {
        // Core Properties
        public Guid UserId { get; set; } 
        public ReminderTypeEnum ReminderType { get; set; }
        public bool IsComplete { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Scheduler uses this property to trigger reminders
        public DateTime? NextTriggerOn { get; set; }

        // OneTime Reminder
        public DateTime? OneTimeDateTime { get; set; }

        // Daily Reminder
        public string? DailyDays { get; set; }
        public DateTime? DailyTime { get; set; }

        // Weekly Reminder
        public string? WeeklyDay { get; set; }
        public DateTime? WeeklyTime { get; set; }

        // EveryNWeeks Reminder
        public int? EveryNWeeksInterval { get; set; }
        public DateTime? EveryNWeeksDateTime { get; set; }

        // Monthly Reminder
        public DateTime? MonthlyTime { get; set; }
        public string MonthlyDay { get; set; } = string.Empty;

        // Yearly Reminder
        public string? YearlyMonth { get; set; }
        public string? YearlyDay { get; set;}
        public DateTime? YearlyTime { get; set; }

        // Warning Reminder
        public int WarningRemindersCount { get; set; }
        public int WarningRemindersGap { get; set; }
        public WarningRemindersGapTypeEnum WarningRemindersGapType { get; set; }

    }
}