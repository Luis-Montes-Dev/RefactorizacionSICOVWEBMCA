using System.ComponentModel.DataAnnotations;

namespace SICOVWEB_MCA.Models
{
    public class Rol
    {
        [Key]
        [Range(0,int.MaxValue , ErrorMessage ="El código de rol debe ser mayor a cero")]
        [Display(Name = "Código de Rol")]       
        public string RolId { get; set; }

        [Required(ErrorMessage = "El campo  es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre del rol no puede exceder los 50 caracteres.")]
        [Display(Name = "Nombre del Rol")]
        public string NombreRol { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
