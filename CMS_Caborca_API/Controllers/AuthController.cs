using CMS_Caborca_API.Data;
using CMS_Caborca_API.Models;
using CMS_Caborca_API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CaborcaContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(CaborcaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            // 2. Verificar Contraseña
            // NOTA: Implementamos validación temporal para los usuarios semilla
            // En producción aquí usaríamos: BCrypt.Verify(request.Password, user.PasswordHash)
            bool passwordValido = false;

            if (user.Usuario == "superadmin" && request.Password == "super123") passwordValido = true;
            else if (user.Usuario == "admin" && request.Password == "admin123") passwordValido = true;
            else if (user.PasswordHash == request.Password) passwordValido = true; // Para pruebas simples

            if (!passwordValido)
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