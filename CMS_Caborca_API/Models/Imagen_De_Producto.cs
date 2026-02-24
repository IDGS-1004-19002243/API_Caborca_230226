using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Imagenes_De_Productos")]
    public class Imagen_De_Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Id_Producto { get; set; }

        [Required]
        [MaxLength(500)]
        public string URL_Cloudinary { get; set; } = null!; // Almacena solo la ruta CDN

        public bool Es_Principal { get; set; } // Si es verdadera, es la que sale como portada del calzado

        // Navegación
        [ForeignKey(nameof(Id_Producto))]
        public virtual Producto_Inventario Producto { get; set; } = null!;
    }
}
