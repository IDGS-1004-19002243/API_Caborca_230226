using CMS_Caborca_API.Data;
using CMS_Caborca_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS_Caborca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogosController : ControllerBase
    {
        private readonly CaborcaContext _context;

        public CatalogosController(CaborcaContext context)
        {
            _context = context;
        }

        // --- CATEGORÍAS ---

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<Categoria_Producto>>> GetCategorias()
        {
            return await _context.Categorias_Productos.ToListAsync();
        }

        [HttpPost("categorias")]
        [Authorize]
        public async Task<ActionResult<Categoria_Producto>> PostCategoria(Categoria_Producto categoria)
        {
            _context.Categorias_Productos.Add(categoria);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategorias), new { id = categoria.Id }, categoria);
        }

        // --- PRODUCTOS ---

        // PÚBLICO: Sólo retorna productos Publicados
        [HttpGet("productos/publicos")]
        public async Task<ActionResult<IEnumerable<Producto_Inventario>>> GetProductosPublicos()
        {
            return await _context.Productos_Inventario
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .Where(p => p.Estado_Publicacion == "Publicado")
                .ToListAsync();
        }

        // CMS ADMIN: Retorna todos los productos
        [HttpGet("productos")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Producto_Inventario>>> GetAllProductos()
        {
            return await _context.Productos_Inventario
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .ToListAsync();
        }

        [HttpPost("productos")]
        [Authorize]
        public async Task<ActionResult<Producto_Inventario>> PostProducto(Producto_Inventario producto)
        {
            _context.Productos_Inventario.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllProductos), new { id = producto.Id }, producto);
        }

        [HttpPut("productos/{id}")]
        [Authorize]
        public async Task<IActionResult> PutProducto(int id, Producto_Inventario producto)
        {
            if (id != producto.Id) return BadRequest();
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- IMÁGENES ---
        [HttpPost("imagenes")]
        [Authorize]
        public async Task<ActionResult<Imagen_De_Producto>> PostImagen(Imagen_De_Producto imagen)
        {
            // Resetear Es_Principal si esta es la nueva foto principal
            if (imagen.Es_Principal)
            {
                var principalesAnteriores = await _context.Imagenes_De_Productos
                    .Where(i => i.Id_Producto == imagen.Id_Producto).ToListAsync();
                foreach (var i in principalesAnteriores) i.Es_Principal = false;
            }

            _context.Imagenes_De_Productos.Add(imagen);
            await _context.SaveChangesAsync();
            return Ok(imagen);
        }
    }
}
