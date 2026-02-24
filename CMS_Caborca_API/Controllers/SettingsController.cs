using CMS_Caborca_API.Data;
using CMS_Caborca_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly CaborcaContext _context;

        public SettingsController(CaborcaContext context)
        {
            _context = context;
        }

        // GET: api/Settings/Mantenimiento
        [HttpGet("Mantenimiento")]
        public async Task<ActionResult<object>> GetMantenimiento()
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Modo_Mantenimiento");

            if (config != null && !string.IsNullOrEmpty(config.Valor_Configuracion))
            {
                // El frontend espera un objeto con { titulo, subtitulo, mensaje, imagenFondo, redes... }
                try
                {
                    var data = JsonSerializer.Deserialize<object>(config.Valor_Configuracion);
                    return Ok(data);
                }
                catch
                {
                    return Ok(new { });
                }
            }
            
            return Ok(new { });
        }

        // PUT: api/Settings/Mantenimiento
        [HttpPut("Mantenimiento")]
        [Authorize]
        public async Task<ActionResult> UpdateMantenimiento([FromBody] object data)
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Modo_Mantenimiento");

            string json = JsonSerializer.Serialize(data);

            if (config == null)
            {
                config = new Configuracion_Del_Sistema
                {
                    Clave_Configuracion = "Modo_Mantenimiento",
                    Valor_Configuracion = json
                };
                _context.Configuraciones_Del_Sistema.Add(config);
            }
            else
            {
                config.Valor_Configuracion = json;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Configuración de mantenimiento guardada exitosamente." });
        }
    }
}
