using PowerMinder.Core.Entity;
using PowerMinder.Engine.Helpers;

namespace PowerMinder.Core
{
    public class ReminderCore
    {
        private EntityContext entityContext;
        private IUtility utility;

        public ReminderCore(EntityContext entityContext, IUtility utility)
        {
            this.entityContext = entityContext;
            this.utility = utility;
        }

        public bool Save(Reminder reminder)
        {
            // Validations
            if (reminder.ReminderType == ReminderTypeEnum.None)
            {
                throw new InvalidDataException("Invalid ReminderType!");
            }

            if (!reminder.NextTriggerOn.HasValue)
            {
                throw new ArgumentNullException("Invalid NextTriggerOn!");
            }

            // Save
            var existing = entityContext.Reminders.Where(r => r.Id == reminder.Id).FirstOrDefault();
            if (existing == null)
            {
                entityContext.Reminders.Add(reminder);
            }
            else
            {
                existing.Title = reminder.Title;
                existing.Description = reminder.Description;
                existing.OneTimeDateTime = reminder.OneTimeDateTime;
                existing.DailyDays = reminder.DailyDays;
                existing.DailyTime = reminder.DailyTime;
                existing.WeeklyDay = reminder.WeeklyDay;
                existing.WeeklyTime = reminder.WeeklyTime;
                existing.EveryNWeeksDateTime = reminder.EveryNWeeksDateTime;
                existing.EveryNWeeksInterval = reminder.EveryNWeeksInterval;
                existing.MonthlyDay = reminder.MonthlyDay;
                existing.MonthlyTime = reminder.MonthlyTime;
                existing.YearlyDay = reminder.YearlyDay;
                existing.YearlyMonth = reminder.YearlyMonth;
                existing.YearlyTime = reminder.YearlyTime;
                existing.WarningRemindersCount = reminder.WarningRemindersCount;
                existing.WarningRemindersGap = reminder.WarningRemindersGap;
                existing.WarningRemindersGapType = reminder.WarningRemindersGapType;
                existing.NextTriggerOn = reminder.NextTriggerOn;
                existing.IsComplete = reminder.IsComplete;
            }

            return entityContext.SaveChanges() > 0;
        }

        public bool SetNextTriggerOn(Reminder reminder)
        {
            // OneTime reminder
            if (reminder.ReminderType == ReminderTypeEnum.OneTime)
            {
                if (!reminder.OneTimeDateTime.HasValue)
                {
                    throw new ArgumentException("Invalid OneTimeDateTime!");
                }

                reminder.NextTriggerOn = reminder.OneTimeDateTime.Value;
            }

            // Daily reminder
            else if (reminder.ReminderType == ReminderTypeEnum.Daily)
            {
                if (string.IsNullOrEmpty(reminder.DailyDays))
                {
                    throw new ArgumentException("Invalid DailyDays!");
                }

                if (!reminder.DailyTime.HasValue)
                {
                    throw new ArgumentException("Invalid DailyTime!");
                }

                reminder.NextTriggerOn = GetDaily_NextTriggerOn(reminder);
            }

            // Weekly reminder
            else if (reminder.ReminderType == ReminderTypeEnum.Weekly)
            {
                if (string.IsNullOrEmpty(reminder.WeeklyDay))
                {
                    throw new ArgumentException("Invalid WeeklyDay!");
                }

                if (!Enum.GetNames(typeof(DayOfWeek)).Contains(reminder.WeeklyDay))
                {
                    throw new ArgumentException("Invalid WeeklyDay value!");
                }

                if (!reminder.WeeklyTime.HasValue)
                {
                    throw new ArgumentException("Invalid WeeklyTime!");
                }

                reminder.NextTriggerOn = GetWeekly_NextTriggerOn(reminder);
            }

            // EveryNWeeks reminder
            else if (reminder.ReminderType == ReminderTypeEnum.EveryNWeeks)
            {
                if (!reminder.EveryNWeeksInterval.HasValue || reminder.EveryNWeeksInterval.Value <= 0)
                {
                    throw new ArgumentException("Invalid EveryNWeeksInterval!");
                }

                if (!reminder.EveryNWeeksDateTime.HasValue)
                {
                    throw new ArgumentException("Invalid EveryNWeeksDateTime!");
                }

                reminder.NextTriggerOn = GetEveryNWeeks_NextTriggerOn(reminder);
            }

            // Monthly reminder
            else if (reminder.ReminderType == ReminderTypeEnum.Monthly)
            {
                if (string.IsNullOrEmpty(reminder.MonthlyDay))
                {
                    throw new ArgumentException("Invalid MonthlyDay!");
                }

                if (!reminder.MonthlyTime.HasValue)
                {
                    throw new ArgumentException("Invalid MonthlyTime!");
                }

                reminder.NextTriggerOn = GetMonthly_NextTriggerOn(reminder);
            }

            // Yearly reminder
            else if (reminder.ReminderType == ReminderTypeEnum.Yearly)
            {
                if (string.IsNullOrEmpty(reminder.YearlyMonth))
                {
                    throw new ArgumentException("Invalid YearlyMonth!");
                }

                if (!Enum.GetNames(typeof(MonthEnum)).Contains(reminder.YearlyMonth))
                {
                    throw new ArgumentException("Invalid YearlyMonth value!");
                }

                if (string.IsNullOrEmpty(reminder.YearlyDay) || reminder.YearlyDay.GetInt() == 0)
                {
                    throw new ArgumentException("Invalid YearlyDay!");
                }

                if (reminder.YearlyDay.GetInt() > DateTime.DaysInMonth(utility.CurrentDateTime.Year, reminder.YearlyMonth.GetMonth()))
                {
                    throw new ArgumentException("Invalid YearlyDay range!");
                }

                if (reminder.YearlyDay.GetInt() > 28 && reminder.YearlyMonth == "February")
                {
                    throw new ArgumentException("Invalid YearlyDay range!");
                }

                if (!reminder.YearlyTime.HasValue)
                {
                    throw new ArgumentException("Invalid YearlyTime!");
                }

                reminder.NextTriggerOn = GetYearly_NextTriggerOn(reminder);
            }

            return true;
        }

        private DateTime GetDaily_NextTriggerOn(Reminder reminder)
        {
            // Set the start date
            var startDate = utility.CurrentDateTime;
            if (reminder.NextTriggerOn.HasValue)
            {
                startDate = reminder.NextTriggerOn.Value;
            }

            // Create 16 consecutive dates
            IList<DateTime> dates = new List<DateTime>();
            dates.Add(startDate);
            int ix = 0;

            while (dates.Count <= 16)
            {
                dates.Add(dates[ix].AddDays(1));
                ix++;
            }

            // Select dates based on User's selected DayOfWeek
            List<DateTime> selectedDates = new List<DateTime>();
            foreach (DateTime date in dates)
            {
                if (reminder.DailyDays.Contains(date.DayOfWeek.ToString()))
                {
                    selectedDates.Add(new DateTime(
                        date.Year,
                        date.Month,
                        date.Day,
                        reminder.DailyTime.Value.Hour,
                        reminder.DailyTime.Value.Minute,
                        reminder.DailyTime.Value.Second));
                }
            }

            // Sort for ascending order
            selectedDates.Sort();

            // Find date which is just after startDate
            foreach (DateTime date in selectedDates)
            {
                if (startDate < date)
                {
                    return date;
                }
            }

            throw new ApplicationException("GetDaily_NextTriggerOn returned No-date!");
        }

        private DateTime GetWeekly_NextTriggerOn(Reminder reminder)
        {
            // Set the next date
            var nextTriggerOn = new DateTime(
                utility.CurrentDateTime.Year,
                utility.CurrentDateTime.Month,
                utility.CurrentDateTime.Day,
                reminder.WeeklyTime.Value.Hour,
                reminder.WeeklyTime.Value.Minute,
                reminder.WeeklyTime.Value.Second);

            if (reminder.NextTriggerOn.HasValue)
            {
                nextTriggerOn = new DateTime(
                    reminder.NextTriggerOn.Value.Year,
                    reminder.NextTriggerOn.Value.Month,
                    reminder.NextTriggerOn.Value.Day,
                    reminder.WeeklyTime.Value.Hour,
                    reminder.WeeklyTime.Value.Minute,
                    reminder.WeeklyTime.Value.Second);

                nextTriggerOn = nextTriggerOn.AddDays(1);
            }

            while (true)
            {
                if (nextTriggerOn.Date.DayOfWeek.ToString() == reminder.WeeklyDay &&
                    nextTriggerOn > utility.CurrentDateTime)
                {
                    break;
                }

                nextTriggerOn = nextTriggerOn.AddDays(1);
            }

            return nextTriggerOn;
        }

        private DateTime GetEveryNWeeks_NextTriggerOn(Reminder reminder)
        {
            // Set the start date
            var startDate = reminder.EveryNWeeksDateTime.Value;
            if (reminder.NextTriggerOn.HasValue)
            {
                startDate = reminder.NextTriggerOn.Value;
            }

            while (true)
            {
                if (startDate > utility.CurrentDateTime)
                {
                    break;
                }

                startDate = startDate.AddDays(reminder.EveryNWeeksInterval.Value * 7);
            }

            return startDate;
        }

        private DateTime GetMonthly_NextTriggerOn(Reminder reminder)
        {
            // Set the start date
            int day = GetDayOfMonth(reminder.MonthlyDay, utility.CurrentDateTime.Month, utility.CurrentDateTime.Year);

            var startDateTime = new DateTime(
                utility.CurrentDateTime.Year,
                utility.CurrentDateTime.Month,
                day,
                reminder.MonthlyTime.Value.Hour,
                reminder.MonthlyTime.Value.Minute,
                0);

            if (reminder.NextTriggerOn.HasValue && reminder.NextTriggerOn.Value < startDateTime)
            {
                startDateTime = reminder.NextTriggerOn.Value;
            }

            while (startDateTime < utility.CurrentDateTime)
            {
                startDateTime = startDateTime.AddMonths(1);

                startDateTime = new DateTime(
                startDateTime.Year,
                startDateTime.Month,
                GetDayOfMonth(reminder.MonthlyDay, startDateTime.Month, startDateTime.Year),
                reminder.MonthlyTime.Value.Hour,
                reminder.MonthlyTime.Value.Minute,
                0);
            }

            return startDateTime;
        }

        private DateTime GetYearly_NextTriggerOn(Reminder reminder)
        {
            var startDateTime = new DateTime(
                utility.CurrentDateTime.Year,
                reminder.YearlyMonth.GetMonth(),
                GetDayOfMonth(reminder.YearlyDay, reminder.YearlyMonth.GetMonth(), utility.CurrentDateTime.Year),
                reminder.YearlyTime.Value.Hour,
                reminder.YearlyTime.Value.Minute,
                0);

            if (reminder.NextTriggerOn.HasValue && reminder.NextTriggerOn.Value < startDateTime)
            {
                startDateTime = reminder.NextTriggerOn.Value;
            }

            while (startDateTime < utility.CurrentDateTime)
            {
                startDateTime = startDateTime.AddYears(1);

                startDateTime = new DateTime(
                startDateTime.Year,
                startDateTime.Month,
                GetDayOfMonth(reminder.YearlyDay, startDateTime.Month, startDateTime.Year),
                reminder.YearlyTime.Value.Hour,
                reminder.YearlyTime.Value.Minute,
                0);
            }

            return startDateTime;
        }

        private int GetDayOfMonth(string day, int month, int year)
        {
            var result = (day == "last" || int.Parse(day) > DateTime.DaysInMonth(year, month)) ? DateTime.DaysInMonth(year, month) : int.Parse(day);

            return result;
        }

        public bool Execute(Reminder reminder)
        {
            // Send Email
            bool sent = SendEmail(reminder);

            if (!sent)
            {
                // TODO: log exception
                return false;
            }

            // Set NextTriggerOn
            if (reminder.ReminderType == ReminderTypeEnum.OneTime)
            {
                reminder.IsComplete = true;
            }

            else if (reminder.ReminderType == ReminderTypeEnum.Daily)
            {
                SetNextTriggerOn(reminder);
            }

            else if (reminder.ReminderType == ReminderTypeEnum.Weekly)
            {
                SetNextTriggerOn(reminder);
            }

            else if (reminder.ReminderType == ReminderTypeEnum.EveryNWeeks)
            {
                SetNextTriggerOn(reminder);
            }

            else if (reminder.ReminderType == ReminderTypeEnum.Monthly)
            {
                SetNextTriggerOn(reminder);
            }

            else if (reminder.ReminderType == ReminderTypeEnum.Yearly)
            {
                SetNextTriggerOn(reminder);
            }

            return Save(reminder);
        }

        private bool SendEmail(Reminder reminder)
        {
            var toUser = utility.GetUserById(reminder).Result;
            if (toUser == null)
            {
                utility.SendEmailToModertor("PowerMinder:InvalidUser", reminder.UserId.ToString());
                return false;
            }

            return utility.SendEmail(reminder, toUser).Result;
        }

        public bool ExecuteTrigerredReminders()
        {
            var reminders = GetTriggeredReminders(utility.CurrentDateTime);
            foreach (Reminder reminder in reminders)
            {
                Execute(reminder);
                // if returns false for few mails do something
            }

            return true;
        }

        public IList<Reminder> GetTriggeredReminders(DateTime currentDateTime)
        {
            var reminders = entityContext.Reminders.Where(r =>
            r.NextTriggerOn <= currentDateTime &&
            r.IsComplete != true
            ).OrderBy(i => i.NextTriggerOn).ToList();

            return reminders;
        }
    }
}
