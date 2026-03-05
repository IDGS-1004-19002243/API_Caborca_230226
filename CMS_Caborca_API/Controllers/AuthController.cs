using CMS_Caborca_API.Data;
using CMS_Caborca_API.Models;
using CMS_Caborca_API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CMS_Caborca_API.Services;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CaborcaContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthController(CaborcaContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            // 1. Buscar usuario en la BD
            var user = await _context.Usuarios_Administradores
                .FirstOrDefaultAsync(u => u.Usuario == request.Usuario);

            if (user == null)
            {
                return Unauthorized("Usuario no existe.");
            }

            // 2. Verificar Contraseña contra la base de datos
            if (user.PasswordHash != request.Password)
            {
                return Unauthorized("Contraseña incorrecta.");
            }

            // 3. Generar el Token JWT
            string token = CrearToken(user);

            // 4. Actualizar último acceso (En este caso lo usamos como Token_Ultima_Sesion temporal. Puede mejorarse).
            user.Token_Ultima_Sesion = token; 
            await _context.SaveChangesAsync();

            return Ok(new { token = token, rol = user.Rol });
        }

        [HttpGet("users")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult> GetUsers()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var user = await _context.Usuarios_Administradores.FirstOrDefaultAsync(u => u.Usuario == username);
            if (user == null) return NotFound("Usuario no encontrado.");

            bool isSuperAdmin = user.Rol == "SuperAdmin" || user.Usuario.ToLower() == "superadmin";
            if (!isSuperAdmin) return StatusCode(403, "No autorizado.");

            var users = await _context.Usuarios_Administradores
                .Select(u => new { u.Usuario, u.Rol })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto request)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var adminUser = await _context.Usuarios_Administradores
                .FirstOrDefaultAsync(u => u.Usuario == username);

            if (adminUser == null) return NotFound("Usuario no encontrado.");

            bool isSuperAdmin = adminUser.Rol == "SuperAdmin" || adminUser.Usuario.ToLower() == "superadmin";
            if (!isSuperAdmin)
            {
                return StatusCode(403, "Solo el SuperAdmin puede cambiar la contraseña.");
            }

            if (adminUser.PasswordHash != request.CurrentPassword)
                return BadRequest("La contraseña actual es incorrecta.");

            var targetUser = adminUser;
            if (!string.IsNullOrEmpty(request.TargetUsername))
            {
                targetUser = await _context.Usuarios_Administradores
                    .FirstOrDefaultAsync(u => u.Usuario == request.TargetUsername);
                if (targetUser == null) return NotFound("El usuario destino no existe.");
            }

            // Almacenar la nueva contraseña directamente (o usar BCrypt en un escenario real)
            targetUser.PasswordHash = request.NewPassword;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contraseña actualizada exitosamente." });
        }

        private string CrearToken(Usuario_Administrador user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim(ClaimTypes.Role, user.Rol ?? "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token válido por 1 día
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}