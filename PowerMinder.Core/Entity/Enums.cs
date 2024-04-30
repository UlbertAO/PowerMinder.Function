namespace PowerMinder.Core.Entity
{
    public enum ReminderTypeEnum
    {
        None,
        OneTime,
        Daily,
        Weekly,
        EveryNWeeks,
        Monthly,
        Yearly
    }

    public enum MonthEnum
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum AmPmEnum
    {
        AM,
        PM
    }

    public enum ReminderPeriodTypeEnum
    {
        Minutes,
        Hours,
        Days,
        Weeks
    }

    public enum WarningRemindersGapTypeEnum
    {
        Hours,
        Days,
        Weeks
    }
}
