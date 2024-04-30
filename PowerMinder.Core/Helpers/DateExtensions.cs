

using PowerMinder.Core.Entity;

namespace PowerMinder.Engine.Helpers
{
    public static class DateExtensions
    {
        public static int GetMonth(this string month)
        {
            var result = (int)Enum.Parse<MonthEnum>(month);

            return result;
        }

        public static int GetInt(this string value)
        {
            bool success = int.TryParse(value, out var result);

            return success ? result : 0;
        }
    }
}
