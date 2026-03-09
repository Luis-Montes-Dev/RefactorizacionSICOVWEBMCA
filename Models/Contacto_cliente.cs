using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Contacto_cliente
    {
        [Key]        
        [Display(Name = "Código de Contacto")]
        public int Id_contacto { get; set; }

        [Required(ErrorMessage = "El campo Código Cliente es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Código Cliente debe ser mayor que 0.")]
        [Display(Name = "Código Cliente")]
        [ForeignKey("Cliente")]
        public int Id_Cliente { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Apellido Paterno es requerido.")]
        [StringLength(50, ErrorMessage = "El apellido paterno no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Apellido Paterno")]
        public string Apellido_paterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Apellido Materno es requerido.")]
        [StringLength(50, ErrorMessage = "El apellido materno no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Apellido Materno")]
        public string Apellido_materno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Cargo es requerido.")]
        [StringLength(50, ErrorMessage = "El cargo no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Correo electrónico es requerido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Teléfono es requerido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El campo Teléfono debe ser un número de 10 dígitos.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Fecha de alta es requerido.")]
        [Display(Name = "Fecha de Alta")]
        public DateTime Fecha_alta { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Baja")]
        [CustomValidation(typeof(Contacto_cliente), nameof(ValidateFechaBaja))]
        public DateTime? Fecha_baja { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        [ForeignKey("Id_Cliente")]
        public Cliente? Cliente { get; set; }

        // Metodo para validar que la fecha de baja sea posterior a la fecha de alta
        public static ValidationResult ValidateFechaBaja(DateTime? fechaBaja, ValidationContext context)
        {
            var contacto = (Contacto_cliente)context.ObjectInstance;
            if (fechaBaja.HasValue && fechaBaja.Value < contacto.Fecha_alta)
            {
                return new ValidationResult("La fecha de baja debe ser posterior a la fecha de alta.");
            }
            return ValidationResult.Success;
        }
    }
}
