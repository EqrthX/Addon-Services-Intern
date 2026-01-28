using Addon_Service_Intern;
using Addon_Service_Intern.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("data.json", optional: false, reloadOnChange: true);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("ServiceB", client =>
{
    client.BaseAddress = new Uri("https://localhost:7075/");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // เพิ่มบรรทัดนี้เพื่อข้ามการตรวจ SSL บน Localhost
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});
builder.Services.AddScoped<DocsServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "Temp_PDF");

if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

// เปิดท่อให้เข้าถึงไฟล์ได้
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadPath),
    RequestPath = "/files" 
});

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
