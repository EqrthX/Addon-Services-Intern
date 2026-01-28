namespace Addon_Service_Intern.Services;
public class DocsServices
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration; // 2. เพิ่มตัวอ่าน Config

    public DocsServices(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<List<object>> CallGetPDF()
    {
        var client = _httpClientFactory.CreateClient("ServiceB");
        var mySecretKey = _configuration["secret-token"];
        client.DefaultRequestHeaders.Add("X-Secret-Key", mySecretKey);
        var response = await client.GetAsync("/api/Docs/read-pdf");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<object>>();
        }
        return new List<object>();
    }

}

