using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PowerMinder.Core;

namespace PowerMinder.Function
{
    public class ReminderFunction
    {
        private readonly ILogger _logger;
        private ReminderCore _reminderCore;


        public ReminderFunction(ILoggerFactory loggerFactory, ReminderCore reminderCore)
        {
            _logger = loggerFactory.CreateLogger<ReminderFunction>();
            _reminderCore = reminderCore;
        }

        [Function("ReminderFunction")]
        public void Run([TimerTrigger("0 0/5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"ReminderFunction started at: {DateTime.Now}");
            _reminderCore.ExecuteTrigerredReminders();
            _logger.LogInformation($"ReminderFunction ended at: {DateTime.Now}");


        }
    }
}
