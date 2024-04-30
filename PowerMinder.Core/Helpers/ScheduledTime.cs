using PowerMinder.Core.Entity;
using PowerMinder.Engine.Helpers;
using System.ComponentModel.DataAnnotations;


namespace PowerMinder.Core.Helpers
{
    public class ScheduledTime
    {
        private static Utility utility = new();

        private static DateTime time { get { return General.GetLocalTime(utility.CurrentDateTime); } }

        [Required]
        public int Day { get; set; } = time.Day;

        [Required]
        public MonthEnum Month { get; set; } = (MonthEnum)time.Month;

        [Required]
        public int Year { get; set; } = time.Year;

        [RegularExpression(@"^([1-9]|1[0-2])$", ErrorMessage = "Valid Hr inputs are from 1 to 12")]
        public int Hour { get; set; } = (time.Hour > 12) ? time.Hour - 12 : time.Hour;

        [RegularExpression(@"^([0-9]|0[0-9]|[1-5][0-9])$", ErrorMessage = "Valid Hr inputs are from 0 to 59")]
        public int Minute { get; set; } = time.Minute;

        [Required]
        public AmPmEnum AmPm { get; set; } = time.ToString("tt") == "AM" ? AmPmEnum.AM : AmPmEnum.PM;

        public string WeeklyDay { get; set; } = "Monday";

        public int WeeksInterval { get; set; } = 1;

        public int MonthlyDay { get; set; } = time.Day;

        public int YearlyDay { get; set; } = time.Day;

        public string YearlyMonth { get; set; } = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(time.Month);

    }

}
