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

        // GET: api/Settings/DeploySchedule
        [HttpGet("DeploySchedule")]
        public async Task<ActionResult<object>> GetDeploySchedule()
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Deploy_Schedule");

            if (config != null && !string.IsNullOrEmpty(config.Valor_Configuracion))
            {
                return Ok(new { date = config.Valor_Configuracion });
            }

            return Ok(new { date = (string?)null });
        }

        public class ScheduleDto
        {
            public string Date { get; set; } = null!;
        }

        // POST: api/Settings/DeploySchedule
        [HttpPost("DeploySchedule")]
        [Authorize]
        public async Task<ActionResult> SetDeploySchedule([FromBody] ScheduleDto request)
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Deploy_Schedule");

            if (config == null)
            {
                config = new Configuracion_Del_Sistema
                {
                    Clave_Configuracion = "Deploy_Schedule",
                    Valor_Configuracion = request.Date
                };
                _context.Configuraciones_Del_Sistema.Add(config);
            }
            else
            {
                config.Valor_Configuracion = request.Date;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Despliegue programado." });
        }

        // GET: api/Settings/ConfiguracionGeneral
        [HttpGet("ConfiguracionGeneral")]
        public async Task<ActionResult<object>> GetConfiguracionGeneral()
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Configuracion_General");

            if (config != null && !string.IsNullOrEmpty(config.Valor_Configuracion))
            {
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

        // PUT: api/Settings/ConfiguracionGeneral
        [HttpPut("ConfiguracionGeneral")]
        [Authorize]
        public async Task<ActionResult> UpdateConfiguracionGeneral([FromBody] object data)
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Configuracion_General");

            string json = JsonSerializer.Serialize(data);

            if (config == null)
            {
                config = new Configuracion_Del_Sistema
                {
                    Clave_Configuracion = "Configuracion_General",
                    Valor_Configuracion = json
                };
                _context.Configuraciones_Del_Sistema.Add(config);
            }
            else
            {
                config.Valor_Configuracion = json;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Configuración general guardada exitosamente." });
        }

        // GET: api/Settings/CatalogoHombre
        [HttpGet("CatalogoHombre")]
        public async Task<ActionResult<object>> GetCatalogoHombre()
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Catalogo_Hombre");

            if (config != null && !string.IsNullOrEmpty(config.Valor_Configuracion))
            {
                try { return Ok(JsonSerializer.Deserialize<object>(config.Valor_Configuracion)); } catch { return Ok(new List<object>()); }
            }
            return Ok(new List<object>());
        }

        // PUT: api/Settings/CatalogoHombre
        [HttpPut("CatalogoHombre")]
        [Authorize]
        public async Task<ActionResult> UpdateCatalogoHombre([FromBody] object data)
        {
            var config = await _context.Configuraciones_Del_Sistema.FirstOrDefaultAsync(c => c.Clave_Configuracion == "Catalogo_Hombre");
            string json = JsonSerializer.Serialize(data);
            if (config == null) _context.Configuraciones_Del_Sistema.Add(new Configuracion_Del_Sistema { Clave_Configuracion = "Catalogo_Hombre", Valor_Configuracion = json });
            else config.Valor_Configuracion = json;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Catálogo Hombre guardado exitosamente." });
        }

        // GET: api/Settings/CatalogoMujer
        [HttpGet("CatalogoMujer")]
        public async Task<ActionResult<object>> GetCatalogoMujer()
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Catalogo_Mujer");

            if (config != null && !string.IsNullOrEmpty(config.Valor_Configuracion))
            {
                try { return Ok(JsonSerializer.Deserialize<object>(config.Valor_Configuracion)); } catch { return Ok(new List<object>()); }
            }
            return Ok(new List<object>());
        }

        // PUT: api/Settings/CatalogoMujer
        [HttpPut("CatalogoMujer")]
        [Authorize]
        public async Task<ActionResult> UpdateCatalogoMujer([FromBody] object data)
        {
            var config = await _context.Configuraciones_Del_Sistema.FirstOrDefaultAsync(c => c.Clave_Configuracion == "Catalogo_Mujer");
            string json = JsonSerializer.Serialize(data);
            if (config == null) _context.Configuraciones_Del_Sistema.Add(new Configuracion_Del_Sistema { Clave_Configuracion = "Catalogo_Mujer", Valor_Configuracion = json });
            else config.Valor_Configuracion = json;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Catálogo Mujer guardado exitosamente." });
        }
    }
}
