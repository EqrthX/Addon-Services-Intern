using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
