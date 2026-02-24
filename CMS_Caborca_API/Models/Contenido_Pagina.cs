using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Contenidos_Paginas")]
    public class Contenido_Pagina
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Contenido { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre_Pagina { get; set; } = null!; // Ej: 'Inicio', 'Nosotros'

        [Required]
        [MaxLength(100)]
        public string Seccion_Pagina { get; set; } = null!; // Ej: 'Banner', 'Directorio'

        [Required]
        [MaxLength(200)]
        public string Clave_Identificadora { get; set; } = null!; // Ej: 'inicio_titulo_banner' (Unique Key se configura en Context)

        public string? Contenido_Borrador_Stage { get; set; } // Lo que el CMS edita

        public string? Contenido_Publicado_Produccion { get; set; } // Lo que lee el Frontend público

        [Required]
        [MaxLength(50)]
        public string Tipo_De_Contenido { get; set; } = "texto"; // 'texto' o 'imagen_url'
    }
}
