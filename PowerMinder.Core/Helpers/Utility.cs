using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using PowerMinder.Core.Entity;
using PowerMinder.Core.Helpers;
//using PowerMinder.Engine.Entity;
//using PowerMinder.Engine.ViewModels;
using SendGrid;
using SendGrid.Helpers.Mail;
//using TyzenR.Account;
//using TyzenR.Account.Managers;

namespace PowerMinder.Engine.Helpers
{
    public class Utility : IUtility
    {
        private readonly PowerMinderSettings appSettings;
        //private readonly IUserManager _userManager;
        private readonly IServiceProvider _serviceProvider;
        private bool UseSendGrid = true;

        public Utility()
        {

        }

        public Utility(IOptions<PowerMinderSettings> options, IServiceProvider serviceProvider)
        {
            appSettings = options.Value;
            _serviceProvider = serviceProvider;
        }

        //public Utility(UserManager userManager, IOptions<PowerMinderSettings> options)
        //{
        //    appSettings = options.Value;
        //    _userManager = userManager;
        //}

        private async Task<bool> SendEmail(EmailMessage mailMessage)
        {
            bool status = false;
            try
            {
                if (!UseSendGrid)
                {
                    var mail = new MimeMessage();
                    mail.From.Add(new MailboxAddress("PowerMinder", appSettings.Smtp.FromMail));
                    mail.To.AddRange(mailMessage.To);
                    mail.Subject = mailMessage.Subject;
                    mail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailMessage.Body };

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(
                            appSettings.Smtp.SmtpServer,
                            appSettings.Smtp.Port,
                            SecureSocketOptions.StartTlsWhenAvailable
                            );

                        await client.AuthenticateAsync(appSettings.Smtp.UserName, appSettings.Smtp.Password);
                        client.Send(mail);
                        client.Disconnect(true);
                        status = true;
                    }
                }
                else // SendGrid
                {
                    var client = new SendGridClient(appSettings.SendGridAPIKey);
                    var from = new EmailAddress(appSettings.Smtp.FromMail, "PowerMinder");
                    var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(mailMessage.To.First().Address), mailMessage.Subject, mailMessage.Body, mailMessage.Body);
                    var response = await client.SendEmailAsync(msg);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        public async Task<bool> SendEmail(Reminder reminder, User user)
        {
            string mailBody = appSettings.Templates.HtmlPart;
            var pathToFile = Path.Combine(Directory.GetParent(AppContext.BaseDirectory).FullName, "Templates/ReminderEmail.html");

            if (File.Exists(pathToFile))
                mailBody = File.ReadAllText(pathToFile);

            mailBody = mailBody.Replace("{{Name}}", user.FirstName + " " + user.LastName);
            mailBody = mailBody.Replace("{{Title}}", reminder.Title);
            mailBody = mailBody.Replace("{{Description}}", reminder.Description);
            mailBody = mailBody.Replace("{{EventDate}}", reminder.NextTriggerOn.Value.ToString("dd MM,yyyy hh:mm tt"));

            var ReminderType = General.GetReminderTypeString(reminder.ReminderType);
            var link = $"{appSettings.PowerMinderUrl}/reminder/{ReminderType}/edit/{reminder.Id}".ToLower();
            mailBody = mailBody.Replace("{{ViewLink}}", $"{appSettings.PowerMinderUrl}");
            mailBody = mailBody.Replace("{{EditLink}}", link);
            mailBody = mailBody.Replace("{{DeleteLink}}", link);

            var ToMail = new List<string>() { user.Email };

            if (appSettings.IsTestMail)
                ToMail.AddRange(appSettings.TestMailId.Split(',', ';').ToList());

            var message = new EmailMessage(ToMail.ToList(), reminder.Title ?? "Test", mailBody ?? "TestMail");

            return await SendEmail(message);
        }

        public DateTime CurrentDateTime
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        //public User CurrentUser
        //{
        //    get
        //    {
        //        var user = _userManager.CurrentUser;
        //        if (user is not null)
        //        {
        //            return new User()
        //            {
        //                Id = user.Id,
        //                Email = user.Email,
        //                FirstName = user.FirstName,
        //                LastName = user.LastName,
        //                Provider = user.AuthProvider,
        //                IsConfirmed = user.IsVerified,
        //                TimeZone = user.TimeZone,
        //            };
        //        }
        //        return new();
        //    }
        //}

        public DateTime ConvertToUTCDateTime(DateTime DT)
        {
            return ConvertToUTCDateTime(DT.Year, DT.Month, DT.Day, DT.Hour, DT.Minute, DT.ToString("tt"));
        }

        public DateTime ConvertToUTCDateTime(int Year, int Month, int Day, int Hour, int Minute, string AmPm)
        {
            int Sec = 0, MSec = 0;
            if (Hour == 12 && AmPm == "AM") Hour = 0;
            if (Hour < 12 && AmPm == "PM") Hour += 12;
            Sec = (Hour == 12 && Minute == 0) ? Sec : this.CurrentDateTime.Second;
            MSec = (Hour == 12 && Minute == 0) ? MSec : this.CurrentDateTime.Millisecond;
            var DT = new DateTime(Year, Month, Day, Hour, Minute, Sec, MSec, DateTimeKind.Unspecified);

            var tZone = TimeZoneInfo.FindSystemTimeZoneById(General.UserTimeZone);

            var time = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DT, tZone.StandardName, "UTC");

            return time;// DT.ToUniversalTime();
        }
        public DateTime ConvertToDateTime(int Year, int Month, int Day, int Hour, int Minute, string AmPm)
        {
            int Sec = 0, MSec = 0;
            if (Hour == 12 && AmPm == "AM") Hour = 0;
            if (Hour < 12 && AmPm == "PM") Hour += 12;
            Sec = (Hour == 12 && Minute == 0) ? Sec : this.CurrentDateTime.Second;
            MSec = (Hour == 12 && Minute == 0) ? MSec : this.CurrentDateTime.Millisecond;
            var DT = new DateTime(Year, Month, Day, Hour, Minute, Sec, MSec, DateTimeKind.Local);

            return DT;
        }

        public ScheduledTime ConvertToScheduledTime(DateTime? dateTime)
        {
            dateTime = General.GetLocalTime(dateTime.Value);
            return new ScheduledTime
            {
                Year = dateTime.Value.Year,
                Month = (MonthEnum)dateTime.Value.Month,
                Day = dateTime.Value.Day,
                Hour = dateTime.Value.Hour > 12 ? dateTime.Value.Hour - 12 : dateTime.Value.Hour,
                Minute = dateTime.Value.Minute,
                AmPm = dateTime?.ToString("tt") == "AM" ? AmPmEnum.AM : AmPmEnum.PM
            };
        }

        /// <summary>
        /// This methos not works with PowerMinder Scheduler 
        /// So Another Methos is created  
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        //public async Task<User> GetUserById(Guid UserId)
        //{
        //    var _user = _userManager.GetUserById(UserId);
        //    User user = new User
        //    {
        //        Id = _user.Id,
        //        Email = _user.Email,
        //        FirstName = _user.FirstName,
        //        LastName = _user.LastName,
        //        Provider = _user.AuthProvider,
        //        IsConfirmed = _user.IsVerified
        //    };
        //    return await Task.FromResult(user);
        //}

        /// <summary>
        /// This method works with PowerMinder Scheduler
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns></returns>
        public async Task<User> GetUserById(Reminder reminder)
        {
            try
            {

                var AccContext = _serviceProvider.GetService<AccountContext>();
                var _user = AccContext?.Users.AsNoTracking().Where(u => u.Id == reminder.UserId).FirstOrDefault();


                if (_user == null)
                {
                    return null;
                }

                User user = new User
                {
                    Id = _user.Id,
                    Email = _user.Email,
                    FirstName = _user.FirstName,
                    LastName = _user.LastName,
                    Provider = _user.AuthProvider,
                    IsConfirmed = _user.IsActivated
                };

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return await Task.FromResult(new User());
            }

        }

        public async Task<bool> SendEmail(string toEmail, string subject, string body)
        {
            return await SendEmail(new EmailMessage(toEmail.Split(","), subject, body));
        }

        public async Task<bool> SendEmailToModertor(string subject, string body)
        {
            return await SendEmail("jeanpaulazure@gmail.com", subject, body);
        }

        public async Task<bool> SendBatchEmails(IList<string> emails, int batchSize = 1000)
        {
            // work in progress
            // If I provide 5000 emails - it will send in 5 batch of 1000s
            if (emails == null || !emails.Any())
            {
                return false; // No emails to send
            }


            var emailList = emails.ToList();

            for (int i = 0; i < emailList.Count; i += batchSize)
            {
                var batch = emailList.Skip(i).Take(batchSize).ToList();
                await Task.WhenAll(batch.Select(email => SendEmail(email, "[Subject]", "[Body]")));
            }
            return true;

        }
    }
}
