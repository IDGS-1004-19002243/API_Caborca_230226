using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Categorias_Productos")]
    public class Categoria_Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre_ES { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Nombre_EN { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Slug { get; set; } = null!; // Para URLs amigables (ej. 'botas-vaqueras')

        // Propiedad de navegación
        public virtual ICollection<Producto_Inventario> Productos { get; set; } = new List<Producto_Inventario>();
    }
}
