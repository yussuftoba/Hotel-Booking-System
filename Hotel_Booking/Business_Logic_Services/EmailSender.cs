using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Services;

public class EmailSender
{
    private string apiKey;
    private string fromEmail;
    private string senderName;
    public EmailSender(IConfiguration configuration)
    {
        apiKey = configuration["EmailSender:ApiKey"]!;
        fromEmail = configuration["EmailSender:FromEmail"]!;
        senderName = configuration["EmailSender:SenderName"]!;
    }
    public async Task SendEmail(string subject, string toEmail, string userName, string message)
    {
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, senderName);
        var to = new EmailAddress(toEmail, userName);
        var plainTextContent = message;
        var htmlContent = "";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }
}
