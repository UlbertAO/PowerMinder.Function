namespace PowerMinder.Core.Entity
{
    public class AppSettings
    {
        public string CookieDomain { get; set; }
        public string CorsOrigin { get; set; }
        public string AccountUrl { get; set; }
        public string PowerMinderUrl { get; set; }
        public string StockAlertsUrl { get; set; }
        public string FuturecapsUrl { get; set; }
        public string FiwiserUrl { get; set; }
        public string PublisherUrl { get; set; }
        public string InvestorUrl { get; set; }
        public SmtpSettings Smtp { get; set; }
    }

    public class SmtpSettings
    {
        public string FromMail { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
