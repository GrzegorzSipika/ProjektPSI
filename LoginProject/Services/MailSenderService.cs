using LoginProject.Common.Entities;
using MailKit.Net.Smtp;
using MimeKit;

namespace LoginProject.Services;

public interface IMailSenderService
{
    Task SendMail(string toEmail, string subject, string body);
}

public class MailSenderService : IMailSenderService
{
    private readonly EmailConfiguration _emailConfiguration;
    
    public MailSenderService(EmailConfiguration emailConfiguration)
    {
        _emailConfiguration = emailConfiguration;
    }

    public async Task SendMail(string toEmail, string subject, string body)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailConfiguration.From, _emailConfiguration.From));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = subject;
        message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
        {
            Text = body
        };
        
        
        using var client = new SmtpClient();
        try
        {
            client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(_emailConfiguration.Username, _emailConfiguration.Password);
            client.Send(message);

            client.Disconnect(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
        
    }
}