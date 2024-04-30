using MimeKit;

namespace PowerMinder.Core.Entity
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; } = new();

        public string Subject { get; set; }

        public string Body { get; set; }

        public EmailMessage(IEnumerable<string> To, string Subject, string Body)
        {
            this.To.AddRange(To.Select(a => new MailboxAddress("", a)));
            this.Subject = Subject;
            this.Body = Body;
        }
    }
}
