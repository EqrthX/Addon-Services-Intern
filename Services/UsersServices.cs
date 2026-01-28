using Addon_Service_Intern.Domain;
using Addon_Service_Intern.Interfaces;
using System.Text.Json;

namespace Addon_Service_Intern.Services;

public class UsersServices: IMockAuthService
{
    private readonly string _jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
    public bool ValidateToken(string inputToken)
    {
        try
        {
            if (!File.Exists(_jsonPath)) return false;

            var jsonString = File.ReadAllText(_jsonPath);
            // ดึงค่าจาก JSON โดยระบุ Key "secret-token"
            using var doc = JsonDocument.Parse(jsonString);
            if (doc.RootElement.TryGetProperty("secret-token", out var tokenElement))
            {
                string validToken = tokenElement.GetString();
                return !string.IsNullOrEmpty(inputToken) && inputToken == validToken;
            }

            return false;
        }
        catch
        {
            return false; // กรณีไฟล์ JSON รูปแบบผิด
        }
    }
}
