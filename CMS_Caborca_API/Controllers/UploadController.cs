using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Solo admin puede subir fotos
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha seleccionado ningún archivo.");

            // 1. Crear carpeta uploads si no existe
            // Asegurarse de que WebRootPath no sea nulo (puede serlo si no hay carpeta wwwroot creada aun)
            string webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            
            string uploadsFolder = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 2. Generar nombre único para evitar sobrescribir
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 3. Guardar en disco
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 4. Devolver URL relativa
            // Nota: En producción, esto podría ser una URL completa si usas CDN, pero para local está bien.
            string relativeUrl = $"https://{Request.Host}/uploads/{uniqueFileName}"; 
            return Ok(new { url = relativeUrl });
        }
    }
}
