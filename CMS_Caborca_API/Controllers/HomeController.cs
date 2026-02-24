using CMS_Caborca_API.Data;
using CMS_Caborca_API.Models;
using CMS_Caborca_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly CaborcaContext _context;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // Claves en la tabla Contenidos_Paginas
        private const string KEY_CAROUSEL = "home_carousel";
        private const string KEY_FORM_DIST = "home_form_distribuidor";
        private const string KEY_SUSTENTABILIDAD = "home_sustentabilidad";
        private const string KEY_ARTE_CREACION = "home_arte_creacion";
        private const string KEY_DIST_LOGOS = "home_distribuidores_logos";

        public HomeController(CaborcaContext context)
        {
            _context = context;
        }

        // GET: api/Home
        // Devuelve todo el contenido del Home
        [HttpGet]
        public async Task<ActionResult<HomeDto>> GetHomeContent()
        {
            var response = new HomeDto();

            // Si el header tiene token (CMS Admin), IsAuthenticated es true
            bool isCMS = User.Identity?.IsAuthenticated == true;

            var keys = new[] { KEY_CAROUSEL, KEY_FORM_DIST, KEY_SUSTENTABILIDAD, KEY_ARTE_CREACION, KEY_DIST_LOGOS,
                                // Compatibilidad con clave antigua "home_distribuidores"
                                "home_distribuidores" };
            var records = await _context.Contenidos_Paginas
                .Where(c => keys.Contains(c.Clave_Identificadora))
                .ToListAsync();

            string GetJson(string clave)
            {
                var rec = records.FirstOrDefault(c => c.Clave_Identificadora == clave);
                if (rec == null) return string.Empty;
                return isCMS
                    ? (rec.Contenido_Borrador_Stage ?? rec.Contenido_Publicado_Produccion ?? string.Empty)
                    : (rec.Contenido_Publicado_Produccion ?? string.Empty);
            }

            // 1. Carousel
            var jsonCar = GetJson(KEY_CAROUSEL);
            if (!string.IsNullOrEmpty(jsonCar))
                response.Carousel = JsonSerializer.Deserialize<List<HomeCarouselItemDto>>(jsonCar, _jsonOptions) ?? new();

            // 2. Form Distribuidor (primero busca clave nueva, si no la vieja)
            var jsonFormDist = GetJson(KEY_FORM_DIST);
            if (string.IsNullOrEmpty(jsonFormDist))
                jsonFormDist = GetJson("home_distribuidores"); // compatibilidad
            if (!string.IsNullOrEmpty(jsonFormDist))
                response.FormDistribuidor = JsonSerializer.Deserialize<HomeSeccionDto>(jsonFormDist, _jsonOptions) ?? new();

            // 3. Sustentabilidad
            var jsonSust = GetJson(KEY_SUSTENTABILIDAD);
            if (!string.IsNullOrEmpty(jsonSust))
                response.Sustentabilidad = JsonSerializer.Deserialize<HomeSeccionDto>(jsonSust, _jsonOptions) ?? new();

            // 4. Arte de la Creación
            var jsonArte = GetJson(KEY_ARTE_CREACION);
            if (!string.IsNullOrEmpty(jsonArte))
                response.ArteCreacion = JsonSerializer.Deserialize<HomeArteCreacionDto>(jsonArte, _jsonOptions) ?? new();

            // 5. Distribuidores Logos
            var jsonLogos = GetJson(KEY_DIST_LOGOS);
            if (!string.IsNullOrEmpty(jsonLogos))
                response.DistribuidoresLogos = JsonSerializer.Deserialize<HomeDistribuidoresLogosDto>(jsonLogos, _jsonOptions) ?? new();

            return Ok(response);
        }

        // PUT: api/Home
        // Guarda los cambios en el Stage (Requiere Token)
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateHomeContent([FromBody] HomeDto request)
        {
            // 1. Carousel
            var recCar = await GetOrCreateRecord("Inicio", "Carousel", KEY_CAROUSEL);
            recCar.Contenido_Borrador_Stage = JsonSerializer.Serialize(request.Carousel);

            // 2. Form Distribuidor
            var recFormDist = await GetOrCreateRecord("Inicio", "Form Dist.", KEY_FORM_DIST);
            recFormDist.Contenido_Borrador_Stage = JsonSerializer.Serialize(request.FormDistribuidor);

            // 3. Sustentabilidad
            var recSust = await GetOrCreateRecord("Inicio", "Sustentabilidad", KEY_SUSTENTABILIDAD);
            recSust.Contenido_Borrador_Stage = JsonSerializer.Serialize(request.Sustentabilidad);

            // 4. Arte de la Creación
            var recArte = await GetOrCreateRecord("Inicio", "Arte Creacion", KEY_ARTE_CREACION);
            recArte.Contenido_Borrador_Stage = JsonSerializer.Serialize(request.ArteCreacion);

            // 5. Distribuidores Logos
            var recLogos = await GetOrCreateRecord("Inicio", "Logos Dist.", KEY_DIST_LOGOS);
            recLogos.Contenido_Borrador_Stage = JsonSerializer.Serialize(request.DistribuidoresLogos);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Contenido guardado como BORRADOR. Usa 'Publicar' para verlo en el portafolio." });
        }

        // POST: api/Home/deploy
        // Publica el contenido del Stage a Producción (Requiere Token)
        [HttpPost("deploy")]
        [Authorize]
        public async Task<ActionResult> DeployHomeContent()
        {
            var keys = new[] { KEY_CAROUSEL, KEY_FORM_DIST, KEY_SUSTENTABILIDAD, KEY_ARTE_CREACION, KEY_DIST_LOGOS };
            var records = await _context.Contenidos_Paginas
                .Where(c => keys.Contains(c.Clave_Identificadora))
                .ToListAsync();

            foreach (var record in records)
            {
                if (!string.IsNullOrEmpty(record.Contenido_Borrador_Stage))
                {
                    record.Contenido_Publicado_Produccion = record.Contenido_Borrador_Stage;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "¡Contenido publicado! El portafolio ya muestra los últimos cambios." });
        }

        private async Task<Contenido_Pagina> GetOrCreateRecord(string pagina, string seccion, string clave)
        {
            var record = await _context.Contenidos_Paginas.FirstOrDefaultAsync(c => c.Clave_Identificadora == clave);
            if (record == null)
            {
                record = new Contenido_Pagina
                {
                    Nombre_Pagina = pagina,
                    Seccion_Pagina = seccion,
                    Clave_Identificadora = clave,
                    Tipo_De_Contenido = "json",
                    Contenido_Borrador_Stage = "",
                    Contenido_Publicado_Produccion = ""
                };
                _context.Contenidos_Paginas.Add(record);
            }
            return record;
        }
    }
}
