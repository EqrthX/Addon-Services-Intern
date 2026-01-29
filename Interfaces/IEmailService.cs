namespace Addon_Service_Intern.Interfaces;

public interface IEmailService
{
    Task SendEmailFreeAsync(string toEmail, string subject, string body, IFormFile file);

}


