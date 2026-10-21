using System.Net;
using System.Net.Mail;

namespace projeto.Service;

public class EmailService
{
    private readonly string _emailFrom;
    private readonly string _emailPassword;
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    public EmailService()
    {
        _emailFrom = DotNetEnv.Env.GetString("EMAIL_FROM") ?? "";
        _emailPassword = DotNetEnv.Env.GetString("EMAIL_PASSWORD") ?? "";
        _smtpHost = DotNetEnv.Env.GetString("SMTP_HOST") ?? "smtp.gmail.com";

        string smtpPort = DotNetEnv.Env.GetString("SMTP_PORT") ?? "587";

        if (!int.TryParse(smtpPort, out _smtpPort))
        {
            _smtpPort = 587;
        }
    }

    public void Send(string to, string subject, string body)
    {
        ValidateConfiguration();

        if (string.IsNullOrWhiteSpace(to))
        {
            throw new Exception(
                "O endereço de e-mail do destinatário não foi informado."
            );
        }

        try
        {
            MailAddress recipient = new(to);
            MailAddress sender = new(_emailFrom);

            using MailMessage message = new()
            {
                From = sender,
                SubjectEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true,
                Subject = subject,
                Body = body,
                BodyEncoding = System.Text.Encoding.UTF8
            };

            message.To.Add(recipient);

            using SmtpClient smtpServer = new()
            {
                Credentials = new NetworkCredential(
                    _emailFrom,
                    _emailPassword
                ),

                Port = _smtpPort,
                Host = _smtpHost,
                EnableSsl = true
            };

            smtpServer.Send(message);
        }
        catch (FormatException)
        {
            throw new Exception(
                "O endereço de e-mail informado é inválido."
            );
        }
        catch (SmtpException ex)
        {
            throw new Exception(
                "Não foi possível enviar o e-mail de recuperação. " +
                "Verifique a configuração do serviço de e-mail.",
                ex
            );
        }
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_emailFrom))
        {
            throw new Exception(
                "Serviço de e-mail não configurado: EMAIL_FROM não foi informado."
            );
        }

        if (string.IsNullOrWhiteSpace(_emailPassword))
        {
            throw new Exception(
                "Serviço de e-mail não configurado: EMAIL_PASSWORD não foi informado."
            );
        }

        if (string.IsNullOrWhiteSpace(_smtpHost))
        {
            throw new Exception(
                "Serviço de e-mail não configurado: SMTP_HOST não foi informado."
            );
        }
    }
}