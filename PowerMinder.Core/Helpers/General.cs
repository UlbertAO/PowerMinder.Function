
using PowerMinder.Core.Entity;
using PowerMinder.Core.Helpers;


namespace PowerMinder.Engine.Helpers
{
    public static class General
    {
        public static string UserTimeZone = "";
        private static Utility utility { get; set; } = new Utility();

        public static string GetUserTimeZone(string TimeZoneId="")
        {
            TimeZoneId = TimeZoneId == "" ? General.UserTimeZone:TimeZoneId;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            return $"{tz.DisplayName} - {tz.StandardName}";
        }

        public static DateTime GetLocalTime(DateTime UtcTime)
        {
            var TZ_User = TimeZoneInfo.FindSystemTimeZoneById(General.UserTimeZone);
            
            var aa= TimeZoneInfo.ConvertTimeFromUtc(UtcTime, TZ_User);

            return aa;
        }
        public static DateTime ConvertToUTCDateTime(DateTime DT)
        {
            return utility.ConvertToUTCDateTime(DT);
        }
        public static DateTime ConvertToUTCDateTime(int Year, int Month, int Day, int Hour, int Minute, string AmPm)
        {
            return utility.ConvertToUTCDateTime(Year, Month, Day, Hour, Minute, AmPm);
        }
        public static DateTime ConvertToDateTime(int Year, int Month, int Day, int Hour, int Minute, string AmPm)
        {
            return utility.ConvertToDateTime(Year, Month, Day, Hour, Minute, AmPm);
        }

        //public static ReminderViewModel InitModel(Guid UserId, string ReminderType)
        //{
        //    return new ReminderViewModel
        //    {
        //        UserId = UserId,
        //        ReminderType = ReminderType,
        //        WarningRemindersCount = 1,
        //        WarningRemindersGap = 1,
        //        ScheduledTime = new(),
        //    };
        //}

        //public static List<SelectListItem> InitWeekDays(string? SelectedWeekDays = "")
        //{
        //    var days = WeekDaysList();
        //    var items = new List<SelectListItem>();

        //    foreach (var i in days)
        //    {
        //        bool IsSelected = true;

        //        if (!string.IsNullOrEmpty(SelectedWeekDays))
        //            IsSelected = SelectedWeekDays.Split(",").Contains(i.Value);

        //        items.Add(new SelectListItem { Text = i.Value, Value = i.Value, Selected = IsSelected });

        //    }

        //    return items;
        //}

        public static Dictionary<int, string> MonthsList()
        {
            return Enumerable.Range(1, 12).Select(i => new
            {
                Key = i,
                Value = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
            }).ToDictionary(x => x.Key, x => x.Value);
        }

        public static List<int> YearsList()
        {
            return Enumerable.Range(utility.CurrentDateTime.Year - 1, 52).ToList();
        }

        public static Dictionary<int, string> WeekDaysList()
        {
            // Monday to Saturday
            var ww = Enumerable.Range(1, 6).Select(i => new
            {
                Key = i,
                Value = ((DayOfWeek)i).ToString()
            }).ToDictionary(x => x.Key, x => x.Value);

            // Add Sunday
            ww.Add(7, ((DayOfWeek)0).ToString());
            return ww;
        }

        public static Dictionary<int, string> WeekNumbersList()
        {
            string[] Numbers = { "", "First", "Second", "Third", "Fourth", "Last" };

            var ww = Enumerable.Range(1, Numbers.Count() - 1).Select(i => new
            {
                Key = i,
                Value = Numbers[i].ToString()
            }).ToDictionary(x => x.Key, x => x.Value);

            return ww;
        }

        public static ReminderTypeEnum GetReminderTypeEnum(string ReminderType)
        {
            return ReminderType switch
            {
                ReminderTypes.OneTime => ReminderTypeEnum.OneTime,
                ReminderTypes.Daily => ReminderTypeEnum.Daily,
                ReminderTypes.EveryNWeeks => ReminderTypeEnum.EveryNWeeks,
                ReminderTypes.Weekly => ReminderTypeEnum.Weekly,
                ReminderTypes.Monthly => ReminderTypeEnum.Monthly,
                ReminderTypes.Yearly => ReminderTypeEnum.Yearly,
                _ => throw new NotImplementedException(),
            };
        }

        public static string GetReminderTypeString(ReminderTypeEnum ReminderType)
        {
            return ReminderType switch
            {
                ReminderTypeEnum.OneTime => ReminderTypes.OneTime,
                ReminderTypeEnum.EveryNWeeks => ReminderTypes.EveryNWeeks,
                ReminderTypeEnum.Yearly => ReminderTypes.Yearly,
                ReminderTypeEnum.Weekly => ReminderTypes.Weekly,
                ReminderTypeEnum.Monthly => ReminderTypes.Monthly,
                ReminderTypeEnum.Daily => ReminderTypes.Daily,
                ReminderTypeEnum.None => "",
                _ => "",
            };
        }

        public static DateTime? ReminderInDateTime(Reminder Reminder)
        {
            return Reminder.ReminderType switch
            {
                ReminderTypeEnum.OneTime => Reminder.OneTimeDateTime,
                ReminderTypeEnum.EveryNWeeks => Reminder.EveryNWeeksDateTime,
                ReminderTypeEnum.Yearly => Reminder.YearlyTime,
                ReminderTypeEnum.Weekly => Reminder.WeeklyTime,
                ReminderTypeEnum.Monthly => Reminder.MonthlyTime,
                ReminderTypeEnum.Daily => Reminder.DailyTime,
                _ => throw new NotImplementedException(),
            };
        }

        public static ScheduledTime ConvertToScheduledTime(DateTime? dateTime)
        {
            return utility.ConvertToScheduledTime(dateTime);
        }

    }
}
