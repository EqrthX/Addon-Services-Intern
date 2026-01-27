using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ConfigDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadfileController : Controller
    {
        private string validPdf = ".pdf";

        [HttpPost("upload-pdf")]
        public async Task<IActionResult> UploadfilePdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("No file uploaded!");
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (ext != validPdf || file.ContentType != "application/pdf")
            {
                throw new Exception("ไม่ใช่ File PDF");
            }

            string dateTime = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            const long MaxFileSize = 10 * 1024 * 1024;

            if (file.Length > MaxFileSize)
            {
                throw new Exception("ไฟล์ขนาดใหญ่เกิน 10 MB");
            }

            string fileName = $"{dateTime}_{file.FileName}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp_PDF");
            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new
                {
                    fileName = file.FileName,
                    path = filePath
                });
            }
            catch (IOException)
            {
                throw new Exception("Uploaded file pdf error");
            }
        }

        [HttpGet("get-pdf")]
        public async Task<IActionResult> GetPDF()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp_PDF");

            if (!Directory.Exists(folderPath)) return Ok(new List<string>());

            string[] filePaths = Directory.GetFiles(folderPath, "*.pdf");
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            var fileUrls = new List<object>();

            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);

                var fileName = Path.GetFileName(filePath);
                string escapedPath = Uri.EscapeDataString(fileName);
                var publicUrl = $"{baseUrl}/files/{escapedPath}";
                
                fileUrls.Add(new
                {
                    FileName = fileName,
                    Url = publicUrl,
                    SizeFile = fileInfo.Length,
                    FileCreationTime = fileInfo.CreationTime
                });
            }

            return Ok(fileUrls);
        }
    }
}
