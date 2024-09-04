using System.Net.Mail;
using TS = System.Threading.Tasks; 

namespace TaskManagement.API;


public interface IEmailSender 
{
    TS.Task SendEmailAsync(string email, string subject, string message, object token);
}
public class EmailSender : IEmailSender
{
    private readonly string _host;
    private readonly int _port;

    public EmailSender(IConfiguration configuration)
    {
        _host = configuration["MailCatch:Host"] ?? "localhost"; 
        _port = int.Parse(configuration["MailCatch:Port"] ?? "1025"); 
    }

    public async TS.Task SendEmailAsync(string email, string subject, string message, object token)
    {
        using (var client = new SmtpClient(_host, _port))
        {
            var from = new MailAddress("elish@gmail.com");
            var to = new MailAddress(email);
            var msg = new MailMessage(from, to)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = false 
            };
            client.SendAsync(msg,token);
        }
    }
}


