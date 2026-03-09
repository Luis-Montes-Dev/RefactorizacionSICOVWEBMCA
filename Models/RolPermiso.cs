using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class RolPermiso
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "El código de rol permiso debe ser mayor a cero")]
        [Display(Name = "Código de Rol-Permiso")]
        public int RolPermisoId { get; set; }

        [Required(ErrorMessage = "El campo Rol es requerido.")]
        [Display(Name = "Código de Rol")]
        [ForeignKey("Rol")]
        public int rolId { get; set; }

        [Required(ErrorMessage = "El campo Permiso es requerido.")]
        [Display(Name = "Código de Permiso")]
        [ForeignKey("Permiso")]
        public int permisoId { get; set; }

        // Propiedades de navegación

        public Rol? Rol { get; set; }

        public Permiso? Permiso { get; set; }
    }
}
