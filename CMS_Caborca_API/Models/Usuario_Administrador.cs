using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Caborca_API.Models
{
    [Table("Usuarios_Administradores")]
    public class Usuario_Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Usuario { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Rol { get; set; } = "Admin"; // Ej: "Admin", "Editor"

        public string? Token_Ultima_Sesion { get; set; }
    }
}
