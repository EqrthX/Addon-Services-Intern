using Microsoft.AspNetCore.Mvc;

namespace UploadPDF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadfileController : Controller
    {
        private string validPdf = ".pdf";

        [HttpPost]
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
    }
}
