using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Empleado
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Id debe ser mayor que 0.")]
        [Display(Name = "Id Empleado")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Nombre")]
        public required string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Apellido Paterno es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Apellido Paterno no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Apellido Paterno")]
        public required string Apellido_Paterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Apellido Materno es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Apellido Materno no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Apellido Materno")]
        public required string Apellido_Materno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Puesto es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Puesto no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Puesto")]
        public required string Puesto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo Correo electrónico no es una dirección de correo válida.")]
        [StringLength(100, ErrorMessage = "El campo Correo electrónico no puede exceder los 100 caracteres.")]
        [Display(Name = "Correo electrónico")]
        public required string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Teléfono es obligatorio.")]
        [StringLength(10, ErrorMessage = "El campo Teléfono no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El campo Teléfono debe ser un número de 10 dígitos.")]
        [Display(Name = "Teléfono")]
        public required string Telefono { get; set; }= string.Empty;

        [Required(ErrorMessage = "El campo Calle es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Calle no puede exceder los 50 caracteres.")]
        [Display(Name = "Calle")]
        public required string Calle { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Numero es obligatorio.")]
        [StringLength(10, ErrorMessage = "El campo Numero no puede exceder los 10 caracteres.")]
        [Display(Name = "Número")]
        public required string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Colonia es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Colonia no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Colonia")]
        public required string Colonia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Localidad es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Localidad no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Localidad")]
        public required string Localidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Estado es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Estado no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Estado")]
        public required string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo País es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo País no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "País")]
        public required string Pais { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo CP es obligatorio.")]
        [StringLength(10, ErrorMessage = "El campo CP no puede exceder los 10 caracteres.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "El código postal debe ser un número de 5 dígitos.")]
        [Display(Name = "CP")]
        public required string CP { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo RFC es obligatorio.")]
        [StringLength(13, ErrorMessage = "El campo RFC no puede exceder los 13 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÑ&]{3,4}\d{6}[a-zA-Z0-9]{3}$",ErrorMessage = "El RFC no tiene un formato válido.")]
        [Display(Name = "RFC")]
        public required string RFC { get; set; } = string.Empty;

        
        [Display(Name = "Estatus")]
        [StringLength(20, ErrorMessage = "El campo Estatus no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El campo Estatus solo puede contener letras.")]
        public required string Estatus { get; set; }

        [Required(ErrorMessage = "El campo Fecha de alta es obligatorio.")]
        [Display(Name = "Fecha de alta")]
        public DateTime Fecha_Alta { get; set; }


        // Validar que la fecha de baja sea posterior a la fecha de alta        
        [CustomValidation(typeof(Empleado), nameof(ValidateFechaBaja))]        
        [Display(Name = "Fecha de baja")]
        public DateTime? Fecha_Baja { get; set; }


        // Metodo para validar que la fecha de baja sea posterior a la fecha de alta
        public static ValidationResult ValidateFechaBaja(DateTime? fechaBaja, ValidationContext context)
        {
            var empleado = (Empleado)context.ObjectInstance;
            if (fechaBaja.HasValue && fechaBaja.Value < empleado.Fecha_Alta)
            {
                return new ValidationResult("La fecha de baja debe ser posterior a la fecha de alta.");
            }
            return ValidationResult.Success;
        }
    }
}
