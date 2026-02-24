using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Productos_Inventario")]
    public class Producto_Inventario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Id_Categoria { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre_ES { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Nombre_EN { get; set; } = null!;

        [Required]
        public string Descripcion_ES { get; set; } = null!;

        [Required]
        public string Descripcion_EN { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public bool Es_Destacado { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado_Publicacion { get; set; } = "Borrador"; // Ej: "Borrador", "Publicado"

        // Navegación
        [ForeignKey(nameof(Id_Categoria))]
        public virtual Categoria_Producto Categoria { get; set; } = null!;

        public virtual ICollection<Imagen_De_Producto> Imagenes { get; set; } = new List<Imagen_De_Producto>();
    }
}
