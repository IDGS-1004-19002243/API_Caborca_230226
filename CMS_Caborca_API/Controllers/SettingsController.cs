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

        // --- Configuración General del CMS ---

        // GET: api/Settings/ConfigList
        // Obtiene todas las configuraciones guardadas con prefijo CMS_Config_
        [HttpGet("ConfigList")]
        [Authorize]
        public async Task<ActionResult<Dictionary<string, object>>> GetConfigList()
        {
            var configItems = await _context.Configuraciones_Del_Sistema
                .Where(c => c.Clave_Configuracion.StartsWith("CMS_Config_"))
                .ToListAsync();

            var result = new Dictionary<string, object>();

            foreach (var item in configItems)
            {
                // Removemos el prefijo para mandar la clave original (ej: 'medioContacto')
                string keyName = item.Clave_Configuracion.Substring("CMS_Config_".Length);
                try
                {
                    result[keyName] = JsonSerializer.Deserialize<object>(item.Valor_Configuracion) ?? new object();
                }
                catch
                {
                    result[keyName] = item.Valor_Configuracion;
                }
            }

            return Ok(result);
        }

        // POST: api/Settings/ConfigList
        // Guarda un lote de configuraciones
        [HttpPost("ConfigList")]
        [Authorize]
        public async Task<ActionResult> SaveConfigList([FromBody] Dictionary<string, JsonElement> configs)
        {
            foreach (var kvp in configs)
            {
                string dbKey = "CMS_Config_" + kvp.Key;
                string jsonValue = kvp.Value.GetRawText();

                var record = await _context.Configuraciones_Del_Sistema
                    .FirstOrDefaultAsync(c => c.Clave_Configuracion == dbKey);

                if (record == null)
                {
                    _context.Configuraciones_Del_Sistema.Add(new Configuracion_Del_Sistema
                    {
                        Clave_Configuracion = dbKey,
                        Valor_Configuracion = jsonValue
                    });
                }
                else
                {
                    record.Valor_Configuracion = jsonValue;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Configuraciones guardadas exitosamente en la base de datos." });
        }
    }
}
