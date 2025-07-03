using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Services;

public class EmailSender
{
    private string hostEmail;
    private string fromEmail;
    private string password;
    private int port;
    public EmailSender(IConfiguration configuration)
    {
        fromEmail = configuration["EmailSender:FromEmail"]!;
        hostEmail = configuration["EmailSender:HostEmail"]!;
        port = int.Parse(configuration["EmailSender:Port"]!);
        password =configuration["EmailSender:Password"]!;
        
    }
    public void SendEmail(string subject, string toEmail, string message)
    {
        var client = new SmtpClient(hostEmail, port);
        client.EnableSsl = true;

        client.Credentials = new NetworkCredential(fromEmail, password);

        client.Send(fromEmail, toEmail,subject, message);
    }
}
