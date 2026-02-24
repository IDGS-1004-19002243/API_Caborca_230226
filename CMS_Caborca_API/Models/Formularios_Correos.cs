using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Configuracion_De_Correos")]
    public class Configuracion_De_Correo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Origen_Formulario { get; set; } = null!; // Ej: 'Contacto', 'Distribuidores'

        [Required]
        [MaxLength(500)]
        public string Correos_Destinatarios { get; set; } = null!; // Correos separados por coma
    }

    [Table("Prospectos_Recibidos")]
    public class Prospecto_Recibido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Origen_Formulario { get; set; } = null!; // Ej: 'Contacto', 'Distribuidores'

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Correo { get; set; } = null!;

        [Required]
        public string Mensaje { get; set; } = null!;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
