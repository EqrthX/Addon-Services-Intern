using MailKit.Net.Smtp;
using MimeKit;
using Addon_Service_Intern.Interfaces;

namespace Addon_Service_Intern.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private MimeMessage FormEmail(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("MyDotNetTestSMTP", "nontprawitchkung@gmail.com"));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        string htmlBody = $@"
            <html>
                <body style='background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: auto; background: white; font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #d9534f; border-bottom: 2px solid #d9534f; padding-bottom: 10px;'>🔔 Notification System</h2>
                        <div style='padding: 10px 0;'>
                            {body}
                        </div>
                        <hr style='border: 0; border-top: 1px solid #eee;'>
                        <p style='font-size: 12px; color: #888;'>อีเมลนี้เป็นการแจ้งเตือนอัตโนมัติ วันที่ {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                    </div>
                </body>
            </html>
        ";
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlBody };
        return email;
    }
    public async Task SendEmailFreeAsync(string toEmail, string subject, string body, IFormFile file)
    {
        var emailMessage = FormEmail(toEmail, subject, body);

        var builder = new BodyBuilder();
        builder.HtmlBody = emailMessage.HtmlBody;

        if(file != null || file.Length > 0)
        {
            using(var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            }
        }
        emailMessage.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(_configuration["EmailServices:Host"], int.Parse(_configuration["EmailServices:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailServices:Email"], _configuration["EmailServices:Password"]);
            await smtp.SendAsync(emailMessage);
        }
        catch
        {
            Console.WriteLine("ส่งเมลไม่สำเร็จ");
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

}
