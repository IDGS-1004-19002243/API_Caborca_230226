using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Solo admin puede subir fotos
    public class UploadController : ControllerBase
    {
        private readonly Cloudinary _cloudinary = null!;

        public UploadController(IConfiguration configuration)
        {
            try {
                var cloudinaryConfig = configuration.GetSection("Cloudinary");
                var account = new Account(
                    cloudinaryConfig["CloudName"] ?? "missing",
                    cloudinaryConfig["ApiKey"] ?? "missing",
                    cloudinaryConfig["ApiSecret"] ?? "missing"
                );
                _cloudinary = new Cloudinary(account);
                _cloudinary.Api.Secure = true;
            } catch (Exception ex) {
                Console.WriteLine($"Error init Cloudinary: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha seleccionado ningún archivo.");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "cms_caborca" // Carpeta opcional en tu cuenta de Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                return StatusCode(500, $"Error subiendo imagen a Cloudinary: {uploadResult.Error.Message}");
            }

            // Devolver URL segura (HTTPS) de Cloudinary
            return Ok(new { url = uploadResult.SecureUrl.ToString() });
        }
    }
}
