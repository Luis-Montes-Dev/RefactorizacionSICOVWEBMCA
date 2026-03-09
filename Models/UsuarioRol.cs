using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class UsuarioRol
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "El código de usuario rol debe ser mayor a cero")]
        [Display(Name = "Código de Usuario-Rol")]
        public int usuarioRolId { get; set; }

        [Required(ErrorMessage = "El campo Usuario es requerido.")]
        [Display(Name = "Código de Usuario")]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El campo Rol es requerido.")]
        [Display(Name = "Código de Rol")]
        [ForeignKey("Rol")]
        public int RolId { get; set; }

        // Propiedades de navegación
        public Usuario? Usuario { get; set; }
        public Rol? Rol { get; set; }
    }
}
