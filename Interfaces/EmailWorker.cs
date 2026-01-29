namespace Addon_Service_Intern.Interfaces;

public class EmailWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(IServiceProvider serviceProvider, ILogger<EmailWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Email Worker running at: {time}", DateTimeOffset.Now);
            using (var scope = _serviceProvider.CreateScope()) { 
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                try
                {
                    await emailService.SendEmailFreeAsync("Nontprawitch.saetang@gmail.com", "เทส Background", "ทดสอบอัติโนมัติ Background Service", null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "เกิดข้อผิดพลาดในการส่ง Email ผ่าน Background Service");
                }
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
