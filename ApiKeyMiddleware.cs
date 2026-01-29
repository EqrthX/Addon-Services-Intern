using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Addon_Service_Intern;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string API_KEY_HEADER_NAME = "x-api-key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        var requestPath = context.Request.Path.Value?.ToLower();

        // 1. ข้ามการตรวจถ้าเป็น Swagger หรือ Endpoint ที่เราจะกดเทสจากหน้าเว็บ
        if (requestPath != null && (
            requestPath.Contains("swagger") ||
            requestPath.Contains("read-pdf-serviceb") // <-- เพิ่มบรรทัดนี้ เพื่อให้กดเทสได้
           ))
        {
            await _next(context);
            return;
        }

        // 2. อ่านค่า Key ที่ถูกต้อง
        var validKey = configuration["secret-token"];

        // 3. ตรวจสอบ Header
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        if (!validKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}