using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS_Caborca_API.Migrations
{
    /// <inheritdoc />
    public partial class FixContenidoUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contenidos_Paginas_Clave_Identificadora",
                table: "Contenidos_Paginas");

            migrationBuilder.CreateIndex(
                name: "IX_Contenidos_Paginas_Nombre_Pagina_Clave_Identificadora",
                table: "Contenidos_Paginas",
                columns: new[] { "Nombre_Pagina", "Clave_Identificadora" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contenidos_Paginas_Nombre_Pagina_Clave_Identificadora",
                table: "Contenidos_Paginas");

            migrationBuilder.CreateIndex(
                name: "IX_Contenidos_Paginas_Clave_Identificadora",
                table: "Contenidos_Paginas",
                column: "Clave_Identificadora",
                unique: true);
        }
    }
}
