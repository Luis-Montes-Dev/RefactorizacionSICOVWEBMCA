using System.ComponentModel.DataAnnotations;
namespace SICOVWEB_MCA.Models
{
    public class Permiso
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "El código de permiso debe ser mayor a cero")]
        [Display(Name = "Código de Permiso")]
        public int PermisoId { get; set; }

        [Required(ErrorMessage = "El campo Nombre de Permiso es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre del permiso no puede exceder los 50 caracteres.")]
        [Display(Name = "Nombre de Permiso")]
        public string NombrePermiso { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
