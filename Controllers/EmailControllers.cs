using Microsoft.AspNetCore.Mvc;
using Addon_Service_Intern.Services;
using Addon_Service_Intern.Interfaces;
namespace Addon_Service_Intern.Controllers;
public class EmailRequest
{
    public string ToEmail { get; set; }
    public IFormFile file { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class EmailControllers : ControllerBase
{
    private readonly IEmailService _emailService;
    
    public EmailControllers(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send-email-free")]
    public async Task<IActionResult> SendEmailTest([FromForm] EmailRequest request)
    {
        string subject = "<h1>[ทดสอบการส่ง!] Email</h1>";
        string body = "<h3>ส่งข้อมูล Email </h3>";
        await _emailService.SendEmailFreeAsync(request.ToEmail, subject, body, request.file);

        return Ok(new
        {
            message = "ส่งเมลสำเร็จ",
        });
    }
}
