using System.Net;
using System.Net.Mail;

namespace projeto.Service;

public class EmailService
{

    private readonly string EmailFrom = DotNetEnv.Env.GetString("EMAIL_FROM");
    private readonly string EmailPassword = DotNetEnv.Env.GetString("EMAIL_PASSWORD");
    
    public void Send(string to, string subject, string body)
    {
        MailMessage message = new()
        {
            From = new MailAddress(EmailFrom),
            To = { new MailAddress(to) },
            SubjectEncoding = System.Text.Encoding.UTF8,
            IsBodyHtml = true,
            Subject = subject,
            Body = body,
            BodyEncoding = System.Text.Encoding.UTF8
        };
        SmtpClient SMTPServer = new()
        {
            Credentials = new NetworkCredential(EmailFrom, EmailPassword),
            Port = 587,
            Host = "smtp.gmail.com",
            EnableSsl = true
        };

        try
        {
            SMTPServer.Send(message);
        } catch (Exception)
        {
            throw;
        }

    }

}