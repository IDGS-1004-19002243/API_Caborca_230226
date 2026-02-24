using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS_Caborca_API.Migrations
{
    /// <inheritdoc />
    public partial class ReestructuracionHibridaV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias_Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_ES = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre_EN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuracion_De_Correos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origen_Formulario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correos_Destinatarios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracion_De_Correos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuracion_Del_Sistema",
                columns: table => new
                {
                    Clave_Configuracion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor_Configuracion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracion_Del_Sistema", x => x.Clave_Configuracion);
                });

            migrationBuilder.CreateTable(
                name: "Contenidos_Paginas",
                columns: table => new
                {
                    Id_Contenido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Pagina = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Seccion_Pagina = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Clave_Identificadora = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contenido_Borrador_Stage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contenido_Publicado_Produccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo_De_Contenido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenidos_Paginas", x => x.Id_Contenido);
                });

            migrationBuilder.CreateTable(
                name: "Prospectos_Recibidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origen_Formulario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prospectos_Recibidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiendas_Y_Distribuidores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Mostrar_En_Cintillo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiendas_Y_Distribuidores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios_Administradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Token_Ultima_Sesion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios_Administradores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos_Inventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Categoria = table.Column<int>(type: "int", nullable: false),
                    Nombre_ES = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nombre_EN = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion_ES = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion_EN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Es_Destacado = table.Column<bool>(type: "bit", nullable: false),
                    Estado_Publicacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos_Inventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Inventario_Categorias_Productos_Id_Categoria",
                        column: x => x.Id_Categoria,
                        principalTable: "Categorias_Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Imagenes_De_Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Producto = table.Column<int>(type: "int", nullable: false),
                    URL_Cloudinary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Es_Principal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagenes_De_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imagenes_De_Productos_Productos_Inventario_Id_Producto",
                        column: x => x.Id_Producto,
                        principalTable: "Productos_Inventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contenidos_Paginas_Clave_Identificadora",
                table: "Contenidos_Paginas",
                column: "Clave_Identificadora",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_De_Productos_Id_Producto",
                table: "Imagenes_De_Productos",
                column: "Id_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Inventario_Id_Categoria",
                table: "Productos_Inventario",
                column: "Id_Categoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuracion_De_Correos");

            migrationBuilder.DropTable(
                name: "Configuracion_Del_Sistema");

            migrationBuilder.DropTable(
                name: "Contenidos_Paginas");

            migrationBuilder.DropTable(
                name: "Imagenes_De_Productos");

            migrationBuilder.DropTable(
                name: "Prospectos_Recibidos");

            migrationBuilder.DropTable(
                name: "Tiendas_Y_Distribuidores");

            migrationBuilder.DropTable(
                name: "Usuarios_Administradores");

            migrationBuilder.DropTable(
                name: "Productos_Inventario");

            migrationBuilder.DropTable(
                name: "Categorias_Productos");
        }
    }
}
