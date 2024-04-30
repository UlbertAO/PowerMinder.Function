using PowerMinder.Core.Entity;
using PowerMinder.Core.Helpers;

namespace PowerMinder.Engine.Helpers
{
    public interface IUtility
    {
        Task<bool> SendEmail(string toEmail, string subject, string body);
        Task<bool> SendEmail(Reminder reminder, User user);
        Task<bool> SendEmailToModertor(string subject, string body);
        DateTime CurrentDateTime { get; }
        DateTime ConvertToUTCDateTime(DateTime DT);
        DateTime ConvertToUTCDateTime(int Year, int Month, int Day, int Hour, int Minute, string AmPm);
        DateTime ConvertToDateTime(int Year, int Month, int Day, int Hour, int Minute, string AmPm);
        ScheduledTime ConvertToScheduledTime(DateTime? dateTime);
        //Task<User> GetUserById(Guid UserId);
        Task<User> GetUserById(Reminder reminder);
    }
}
