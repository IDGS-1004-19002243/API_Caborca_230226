using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Configuracion_Del_Sistema")]
    public class Configuracion_Del_Sistema
    {
        [Key]
        [MaxLength(100)]
        public string Clave_Configuracion { get; set; } = null!; // Ej: 'Modo_Mantenimiento'

        [Required]
        public string Valor_Configuracion { get; set; } = null!; // Ej: 'ON', 'OFF', 'True', etc.
    }
}
