using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TaskManager.Application.Interfaces;

namespace TaskManager.Infrastructure.Messaging;

public class EmailService : IEmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _from;

    public EmailService(string host, int port, string username, string password, string from)
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _from = from;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_username, _password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}