using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Tiendas_Y_Distribuidores")]
    public class Tienda_Y_Distribuidor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Direccion { get; set; } = null!;

        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitud { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitud { get; set; }

        public bool Mostrar_En_Cintillo { get; set; } // Para mostrar logos o pines en el Home del portafolio
    }
}
