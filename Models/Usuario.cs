using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Usuario
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Id debe ser mayor que 0.")]
        [Display(Name = "No. Usuario")]
        public int Id { get; set; }

        // Relación con Empleado usando Data Annotations
        [ForeignKey("Empleado")]
        [Display(Name = "No. Empleado")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "El campo Correo electrónico es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Correo electrónico no puede exceder los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El campo Correo electrónico no es una dirección de correo válida.")]
        [Display(Name = "Correo electrónico")]
        public string CorreoUsuario { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        [StringLength(255, ErrorMessage = "El campo Contraseña no puede exceder los 255 caracteres.")]        
        [Display(Name = "Contraseña")]
        public string? Contrasena { get; set; }

        [Required(ErrorMessage = "El campo Tipo de usuario es obligatorio.")]
        public string? TipoUsuario { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        [ForeignKey("EmpleadoId")]
        public Empleado? Empleado { get; set; }
    }
}
