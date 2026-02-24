using System;
using System.Collections.Generic;
using CMS_Caborca_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS_Caborca_API.Data;

public partial class CaborcaContext : DbContext
{
    public CaborcaContext()
    {
    }

    public CaborcaContext(DbContextOptions<CaborcaContext> options)
        : base(options)
    {
    }

    // --- NUEVAS ENTIDADES (PROPUESTA V1) ---
    public virtual DbSet<Contenido_Pagina> Contenidos_Paginas { get; set; } = null!;
    public virtual DbSet<Configuracion_Del_Sistema> Configuraciones_Del_Sistema { get; set; } = null!;
    public virtual DbSet<Categoria_Producto> Categorias_Productos { get; set; } = null!;
    public virtual DbSet<Producto_Inventario> Productos_Inventario { get; set; } = null!;
    public virtual DbSet<Imagen_De_Producto> Imagenes_De_Productos { get; set; } = null!;
    public virtual DbSet<Tienda_Y_Distribuidor> Tiendas_Y_Distribuidores { get; set; } = null!;
    public virtual DbSet<Configuracion_De_Correo> Configuraciones_De_Correos { get; set; } = null!;
    public virtual DbSet<Prospecto_Recibido> Prospectos_Recibidos { get; set; } = null!;
    public virtual DbSet<Usuario_Administrador> Usuarios_Administradores { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique Key para Contenidos_Paginas (Composite Index: Nombre_Pagina + Clave_Identificadora)
        modelBuilder.Entity<Contenido_Pagina>()
            .HasIndex(c => new { c.Nombre_Pagina, c.Clave_Identificadora })
            .IsUnique();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
