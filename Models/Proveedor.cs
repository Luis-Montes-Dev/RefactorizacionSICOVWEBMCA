using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Proveedor
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Id debe ser mayor que 0.")]
        [Display(Name = "No. Proveedor")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El campo Razón social es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Razón social no puede exceder los 50 caracteres.")]
        [Display(Name = "Razón social")]
        public string Razon_social { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo RFC es obligatorio.")]
        [StringLength(13, ErrorMessage = "El campo RFC no puede exceder los 13 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÑ&]{3,4}\d{6}[a-zA-Z0-9]{3}$", ErrorMessage = "El RFC no tiene un formato válido.")]
        [Display(Name = "RFC")]
        public string Rfc { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Nombre Correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo Correo electrónico no es una dirección de correo válida.")]
        [StringLength(100, ErrorMessage = "El campo Correo electrónico no puede exceder los 100 caracteres.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El campo Teléfono no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El campo Teléfono debe ser un número de 10 dígitos.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Calle es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Calle no puede exceder los 100 caracteres.")]
        [Display(Name = "Calle")]
        public string Calle { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Número es obligatorio.")]
        [StringLength(10, ErrorMessage = "El campo Número no puede exceder los 10 caracteres.")]
        [Display(Name = "Número")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Colonia es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Colonia no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Colonia")]
        public string Colonia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Localidad es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Localidad no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Localidad")]
        public string Localidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Estado es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Estado no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo País es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo País no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "País")]
        public string Pais { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Código postal es obligatorio.")]
        [StringLength(10, ErrorMessage = "El campo Código postal no puede exceder los 10 caracteres.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "El código postal debe ser un número de 5 dígitos.")]
        [Display(Name = "Código postal")]
        public string CP { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Condición de pago es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Condición de pago no puede exceder los 50 caracteres.")]        
        [Display(Name = "Condición de pago")]
        public string Condicion_pago { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Estatus es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Estatus no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Estatus")]
        public string Estatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Fecha de alta es obligatorio.")]
        [Display(Name = "Fecha de alta")]
        public DateTime Fecha_alta { get; set; }

        [CustomValidation(typeof(Proveedor), nameof(ValidateFechaBaja))]
        [Display(Name = "Fecha de baja")]
        public DateTime? Fecha_baja { get; set; }

        // Para obtener una lista de cotizaciones del proveedor
        [NotMapped]
        [ValidateNever]
        public List<Cotizacion_proveedor> Cotizaciones { get; set; }

        // Para obtener una lista de compras al proveedor
        [NotMapped]
        [ValidateNever]
        public List<Compra> Compras { get; set; }

        // Para obtener una lista de productos del proveedor
        [NotMapped]
        [ValidateNever]
        public List<Producto> Productos { get; set; }

        // Metodo para validar que la fecha de baja sea posterior a la fecha de alta
        public static ValidationResult ValidateFechaBaja(DateTime? fechaBaja, ValidationContext context)
        {
            var prov = (Proveedor)context.ObjectInstance;
            if (fechaBaja.HasValue && fechaBaja.Value < prov.Fecha_alta)
            {
                return new ValidationResult("La fecha de baja debe ser posterior a la fecha de alta.");
            }
            return ValidationResult.Success;
        }
    }
}
