using CMS_Caborca_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/cms")]
    [ApiController]
    public class CmsDeployController : ControllerBase
    {
        private readonly CaborcaContext _context;

        public CmsDeployController(CaborcaContext context)
        {
            _context = context;
        }

        // ============================================================
        // POST api/cms/deploy
        // Lógica de negocio core: Copia todo el contenido de 'Borrador'
        // hacia la columna de 'Publicado_Produccion'
        // ============================================================
        [HttpPost("deploy")]
        [Authorize] // En un futuro se puede restringir por roles (ej: solo 'Admin')
        public async Task<IActionResult> DeployToProduction()
        {
            try
            {
                // Usamos ExecuteUpdateAsync (disponible en EF Core 7+) para una ejecución masiva súper óptima.
                // En SQL Server, esto se traduce a: 
                // UPDATE Contenidos_Paginas SET Contenido_Publicado_Produccion = Contenido_Borrador_Stage WHERE Contenido_Borrador_Stage IS NOT NULL
                int rowsAffected = await _context.Contenidos_Paginas
                    .Where(c => c.Contenido_Borrador_Stage != null)
                    .ExecuteUpdateAsync(s => s.SetProperty(
                        p => p.Contenido_Publicado_Produccion,
                        p => p.Contenido_Borrador_Stage
                    ));

                // Registrar el despliegue en bitácora (opcional pero muy recomendado en CMS)
                // Se removió temporalmente la entidad BitacoraDespliegues en V1. 
                // Para restaurarlo, se debe crear la entidad correspondiente.
                
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "¡Despliegue a Producción exitoso!", 
                    filasActualizadas = rowsAffected 
                });
            }
            catch (Exception ex)
            {
                // Manejo básico de errores
                return StatusCode(500, new { message = "Error durante el despliegue.", error = ex.Message });
            }
        }
    }
}
