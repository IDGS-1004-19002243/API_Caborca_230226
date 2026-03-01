using CMS_Caborca_API.Data;
using CMS_Caborca_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CMS_Caborca_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactoController : ControllerBase
{
    private readonly CaborcaContext _context;
    private readonly IEmailService _email;
    private readonly ILogger<ContactoController> _logger;
    private readonly IConfiguration _config;

    public ContactoController(CaborcaContext context, IEmailService email, ILogger<ContactoController> logger, IConfiguration config)
    {
        _context = context;
        _email = email;
        _logger = logger;
        _config = config;
    }

    // POST: api/Contacto
    [HttpPost]
    public async Task<IActionResult> EnviarContacto([FromBody] ContactoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var recipients = await ObtenerDestinatariosAsync("contacto");

        // Fallback: si no hay destinatarios en BD, usar el correo remitente SMTP
        if (recipients.Count == 0)
        {
            var fallback = _config["Smtp:UserEmail"];
            if (!string.IsNullOrWhiteSpace(fallback))
            {
                _logger.LogWarning("Contacto: sin destinatarios configurados. Usando fallback SMTP: {Fallback}", fallback);
                recipients = [fallback];
            }
            else
            {
                return StatusCode(503, new { mensaje = "No hay destinatarios configurados. Agrega correos en la configuración del CMS." });
            }
        }

        var subject = $"[Caborca Boots] Nuevo mensaje de contacto — {dto.Nombre}";
        var body = HtmlContacto(dto);

        try
        {
            await _email.SendEmailAsync(recipients, subject, body);
            return Ok(new { mensaje = "Mensaje enviado correctamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email de contacto");
            return StatusCode(500, new { mensaje = "No se pudo enviar el mensaje. Verifica las credenciales SMTP." });
        }
    }

    // POST: api/Contacto/Distribuidor
    [HttpPost("Distribuidor")]
    public async Task<IActionResult> EnviarDistribuidor([FromBody] DistribuidorSolicitudDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var recipients = await ObtenerDestinatariosAsync("distribuidores");

        // Fallback al correo SMTP si no hay destinatarios configurados
        if (recipients.Count == 0)
        {
            var fallback = _config["Smtp:UserEmail"];
            if (!string.IsNullOrWhiteSpace(fallback))
            {
                _logger.LogWarning("Distribuidor: sin destinatarios configurados. Usando fallback SMTP: {Fallback}", fallback);
                recipients = [fallback];
            }
            else
            {
                return StatusCode(503, new { mensaje = "No hay destinatarios configurados. Agrega correos en la configuración del CMS." });
            }
        }

        var subject = $"[Caborca Boots] Solicitud de distribuidor — {dto.NegocioNombre ?? dto.NombreCompleto}";
        var body = HtmlDistribuidor(dto);

        try
        {
            await _email.SendEmailAsync(recipients, subject, body);
            return Ok(new { mensaje = "Solicitud enviada correctamente. Te contactaremos pronto." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email de distribuidor");
            return StatusCode(500, new { mensaje = "No se pudo enviar la solicitud. Verifica las credenciales SMTP." });
        }
    }

    // GET: api/Contacto/TestSmtp  (solo desarrollo)
    [HttpGet("TestSmtp")]
    public async Task<IActionResult> TestSmtp()
    {
        var smtpUser = _config["Smtp:UserEmail"];
        var smtpHost = _config["Smtp:Host"];
        var destinatariosContacto   = await ObtenerDestinatariosAsync("contacto");
        var destinatariosDistrib    = await ObtenerDestinatariosAsync("distribuidores");

        try
        {
            await _email.SendEmailAsync(
                [smtpUser ?? ""],
                "[Caborca Boots] ✓ Test SMTP",
                "<h2 style='color:#7C5C3E'>El envío de correo funciona correctamente.</h2><p>Este es un mensaje de prueba.</p>"
            );
            return Ok(new
            {
                smtp = new { host = smtpHost, user = smtpUser, estado = "OK" },
                destinatariosContacto,
                destinatariosDistribuidores = destinatariosDistrib,
                mensaje = "Email de prueba enviado a " + smtpUser
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                smtp = new { host = smtpHost, user = smtpUser, estado = "ERROR" },
                error = ex.Message,
                destinatariosContacto,
                destinatariosDistribuidores = destinatariosDistrib,
            });
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private async Task<List<string>> ObtenerDestinatariosAsync(string tipo)
    {
        try
        {
            var config = await _context.Configuraciones_Del_Sistema
                .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Configuracion_General");

            if (config == null || string.IsNullOrEmpty(config.Valor_Configuracion))
                return [];

            using var doc = JsonDocument.Parse(config.Valor_Configuracion);
            if (!doc.RootElement.TryGetProperty("emails", out var emails))
                return [];

            if (!emails.TryGetProperty(tipo, out var lista))
                return [];

            return lista.EnumerateArray()
                        .Select(e => e.GetString() ?? "")
                        .Where(e => !string.IsNullOrWhiteSpace(e))
                        .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leyendo destinatarios '{Tipo}' de ConfiguracionGeneral", tipo);
            return [];
        }
    }

    // ── Plantillas HTML ─────────────────────────────────────────────────────────

    private static string HtmlContacto(ContactoDto dto) => $"""
        <!DOCTYPE html>
        <html lang="es">
        <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
        <body style="margin:0;padding:0;background:#f5f0e8;font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f5f0e8;padding:32px 16px;">
            <tr><td align="center">
              <table width="600" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);">
                <!-- Header -->
                <tr>
                  <td style="background:#7C5C3E;padding:32px 40px;text-align:center;">
                    <h1 style="margin:0;color:#ffffff;font-size:26px;letter-spacing:2px;font-family:Georgia,serif;">
                      CABORCA BOOTS
                    </h1>
                    <p style="margin:8px 0 0;color:#e8d5bd;font-size:13px;letter-spacing:1px;">
                      NUEVO MENSAJE DE CONTACTO
                    </p>
                  </td>
                </tr>
                <!-- Body -->
                <tr>
                  <td style="padding:40px;">
                    <p style="margin:0 0 24px;color:#555;font-size:15px;line-height:1.6;">
                      Has recibido un nuevo mensaje a través del formulario de contacto del sitio web.
                    </p>
                    <table width="100%" cellpadding="0" cellspacing="0">
                      {FilaInfo("Nombre", dto.Nombre)}
                      {FilaInfo("Correo", $"<a href='mailto:{dto.Correo}' style='color:#7C5C3E;'>{dto.Correo}</a>")}
                      {FilaInfo("Teléfono", dto.Telefono ?? "—")}
                      {FilaInfo("Asunto", dto.Asunto ?? "—")}
                    </table>
                    <!-- Mensaje -->
                    <div style="margin-top:24px;background:#f9f5ef;border-left:4px solid #7C5C3E;border-radius:4px;padding:20px;">
                      <p style="margin:0 0 8px;font-size:12px;font-weight:bold;color:#9B8674;text-transform:uppercase;letter-spacing:1px;">Mensaje</p>
                      <p style="margin:0;font-size:15px;color:#333;line-height:1.7;">{System.Net.WebUtility.HtmlEncode(dto.Mensaje)}</p>
                    </div>
                    <!-- CTA -->
                    <div style="margin-top:32px;text-align:center;">
                      <a href="mailto:{dto.Correo}"
                         style="background:#7C5C3E;color:#ffffff;text-decoration:none;padding:14px 32px;border-radius:8px;font-weight:bold;font-size:14px;letter-spacing:1px;display:inline-block;">
                        RESPONDER AL CLIENTE
                      </a>
                    </div>
                  </td>
                </tr>
                <!-- Footer -->
                <tr>
                  <td style="background:#f0ebe2;padding:20px 40px;text-align:center;border-top:1px solid #e0d5c5;">
                    <p style="margin:0;font-size:12px;color:#999;">
                      Este correo fue generado automáticamente por el sitio web de Caborca Boots.
                    </p>
                  </td>
                </tr>
              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;

    private static string HtmlDistribuidor(DistribuidorSolicitudDto dto) => $"""
        <!DOCTYPE html>
        <html lang="es">
        <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
        <body style="margin:0;padding:0;background:#f5f0e8;font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f5f0e8;padding:32px 16px;">
            <tr><td align="center">
              <table width="600" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);">
                <!-- Header -->
                <tr>
                  <td style="background:#7C5C3E;padding:32px 40px;text-align:center;">
                    <h1 style="margin:0;color:#ffffff;font-size:26px;letter-spacing:2px;font-family:Georgia,serif;">
                      CABORCA BOOTS
                    </h1>
                    <p style="margin:8px 0 0;color:#e8d5bd;font-size:13px;letter-spacing:1px;">
                      SOLICITUD DE DISTRIBUIDOR
                    </p>
                  </td>
                </tr>
                <!-- Body -->
                <tr>
                  <td style="padding:40px;">
                    <p style="margin:0 0 24px;color:#555;font-size:15px;line-height:1.6;">
                      Un prospecto está interesado en convertirse en distribuidor autorizado de Caborca Boots.
                    </p>
                    <table width="100%" cellpadding="0" cellspacing="0">
                      {FilaInfo("Nombre de contacto", dto.NombreCompleto ?? "—")}
                      {FilaInfo("Negocio", dto.NegocioNombre ?? "—")}
                      {FilaInfo("Correo", $"<a href='mailto:{dto.CorreoElectronico}' style='color:#7C5C3E;'>{dto.CorreoElectronico}</a>")}
                      {FilaInfo("Teléfono", dto.Telefono ?? "—")}
                      {FilaInfo("Ciudad", dto.Ciudad ?? "—")}
                    </table>
                    {(string.IsNullOrWhiteSpace(dto.Mensaje) ? "" : $"""
                    <div style="margin-top:24px;background:#f9f5ef;border-left:4px solid #7C5C3E;border-radius:4px;padding:20px;">
                      <p style="margin:0 0 8px;font-size:12px;font-weight:bold;color:#9B8674;text-transform:uppercase;letter-spacing:1px;">Mensaje adicional</p>
                      <p style="margin:0;font-size:15px;color:#333;line-height:1.7;">{System.Net.WebUtility.HtmlEncode(dto.Mensaje)}</p>
                    </div>
                    """)}
                    <div style="margin-top:32px;text-align:center;">
                      <a href="mailto:{dto.CorreoElectronico}"
                         style="background:#7C5C3E;color:#ffffff;text-decoration:none;padding:14px 32px;border-radius:8px;font-weight:bold;font-size:14px;letter-spacing:1px;display:inline-block;">
                        CONTACTAR AL PROSPECTO
                      </a>
                    </div>
                  </td>
                </tr>
                <!-- Footer -->
                <tr>
                  <td style="background:#f0ebe2;padding:20px 40px;text-align:center;border-top:1px solid #e0d5c5;">
                    <p style="margin:0;font-size:12px;color:#999;">
                      Este correo fue generado automáticamente por el sitio web de Caborca Boots.
                    </p>
                  </td>
                </tr>
              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;

    private static string FilaInfo(string label, string value) => $"""
        <tr>
          <td style="padding:10px 0;border-bottom:1px solid #f0e8d8;width:140px;vertical-align:top;">
            <span style="font-size:12px;font-weight:bold;color:#9B8674;text-transform:uppercase;letter-spacing:0.5px;">{label}</span>
          </td>
          <td style="padding:10px 0 10px 16px;border-bottom:1px solid #f0e8d8;vertical-align:top;">
            <span style="font-size:15px;color:#333;">{value}</span>
          </td>
        </tr>
        """;
}

// ── DTOs ────────────────────────────────────────────────────────────────────────

public class ContactoDto
{
    public required string Nombre { get; set; }
    public required string Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Asunto { get; set; }
    public required string Mensaje { get; set; }
}

public class DistribuidorSolicitudDto
{
    public string? NombreCompleto { get; set; }
    public required string CorreoElectronico { get; set; }
    public string? Telefono { get; set; }
    public string? NegocioNombre { get; set; }
    public string? Ciudad { get; set; }
    public string? Mensaje { get; set; }
}
