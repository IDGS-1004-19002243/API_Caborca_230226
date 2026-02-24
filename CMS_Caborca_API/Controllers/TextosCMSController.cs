using CMS_Caborca_API.Data;
using CMS_Caborca_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/cms/content")]
    [ApiController]
    public class TextosCMSController : ControllerBase
    {
        private readonly CaborcaContext _context;

        public TextosCMSController(CaborcaContext context)
        {
            _context = context;
        }

        // GET: api/cms/content/{pagina}
        // Devuelve todo el contenido de una página específica en formato JSON consolidado
        [HttpGet("{pagina}")]
        public async Task<ActionResult<Dictionary<string, object>>> GetTextos(string pagina)
        {
            // Validamos si es usuario del CMS para mostrar borrador, o público para mostrar publicado.
            bool isCMS = User.Identity?.IsAuthenticated == true;

            var contenidos = await _context.Contenidos_Paginas
                .Where(c => c.Nombre_Pagina == pagina)
                .ToListAsync();

            var result = new Dictionary<string, object>();

            foreach (var item in contenidos)
            {
                string jsonBody = isCMS 
                    ? (item.Contenido_Borrador_Stage ?? item.Contenido_Publicado_Produccion ?? "{}") 
                    : (item.Contenido_Publicado_Produccion ?? "{}");

                try 
                {
                    result[item.Clave_Identificadora] = JsonSerializer.Deserialize<object>(jsonBody) ?? new object();
                } 
                catch 
                {
                    // Fallback to raw string if it was not valid JSON
                    result[item.Clave_Identificadora] = jsonBody;
                }
            }

            return Ok(result);
        }

        // PUT: api/cms/content/{pagina}
        // Actualiza el contenido (Borrador) de una página específica enviando json anidado.
        [HttpPut("{pagina}")]
        [Authorize]
        public async Task<ActionResult> UpdateTextos(string pagina, [FromBody] Dictionary<string, JsonElement> textos)
        {
            foreach (var kvp in textos)
            {
                var record = await _context.Contenidos_Paginas
                    .FirstOrDefaultAsync(c => c.Nombre_Pagina == pagina && c.Clave_Identificadora == kvp.Key);

                if (record == null)
                {
                    // Lo creamos si no existe
                    record = new Contenido_Pagina
                    {
                        Nombre_Pagina = pagina,
                        Seccion_Pagina = "General", 
                        Clave_Identificadora = kvp.Key,
                        Tipo_De_Contenido = "json",
                        Contenido_Publicado_Produccion = ""
                    };
                    _context.Contenidos_Paginas.Add(record);
                }

                // Guardamos la nueva edición en la columna de Borrador / Stage serializando el nodo
                record.Contenido_Borrador_Stage = kvp.Value.GetRawText();
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Textos de '{pagina}' guardados exitosamente como Borrador." });
        }
    }
}
