using PowerMinder.Core.Entity;

namespace PowerMinder.Core.Entity;
public class PowerMinderSettings : AppSettings
{
    public bool IsTestMail { get; set; }

    public string TestMailId { get; set; } = "";
    public string SendGridAPIKey { get; set; } = "";

    public SmtpSettings Smtp { get; set; } = new();

    public MailTemplate Templates { get; set; }=new();
}

//public class SmtpSettings
//{
//    public string FromMail { get; set; } = "";

//    public string SmtpServer { get; set; } = "";

//    public int Port { get; set; } 

//    public string UserName { get; set; } = "";

//    public string Password { get; set; } = "";

//}

public class MailTemplate
{
    public string TemplateName { get; set; } = "";  

    public string SubjectPart { get; set; } = "";

    public string HtmlPart { get; set; } = "";  
}