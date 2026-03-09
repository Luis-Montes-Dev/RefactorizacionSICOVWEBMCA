using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Cliente
    {
        public Cliente() { }

        [Key]        
        [Display(Name = "Código de Cliente")]
        public int Id_cliente { get; set; }

        [Required(ErrorMessage = "El campo Razón Social es requerido.")]
        [StringLength(50, ErrorMessage = "La razón social no puede exceder los 50 caracteres.")]        
        [Display(Name = "Razón Social")]
        public string Razon_Social { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo RFC es requerido.")]
        [StringLength(50, ErrorMessage = "El RFC no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÑ&]{3,4}\d{6}[a-zA-Z0-9]{3}$", ErrorMessage = "El RFC no tiene un formato válido.")]
        [Display(Name = "RFC")]
        public string Rfc { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Correo es requerido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo Electrónico")]
        public required string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Teléfono es requerido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El campo Teléfono debe ser un número de 10 dígitos.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Calle es requerido.")]
        [StringLength(50, ErrorMessage = "La calle no puede exceder los 50 caracteres.")]
        [Display(Name = "Calle")]
        public string Calle { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Número es requerido.")]
        [StringLength(10, ErrorMessage = "El número no puede exceder los 10 caracteres.")]
        [Display(Name = "Número")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage ="El campo Colonia es requerido.")]
        [StringLength(50, ErrorMessage = "La colonia no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Colonia")]
        public string Colonia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Localidad es requerido.")]
        [StringLength(50, ErrorMessage = "La localidad no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Localidad")]
        public string Localidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Estado es requerido.")]
        [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo País es requerido.")]
        [StringLength(50, ErrorMessage = "El país no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Display(Name = "País")]
        public string Pais { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Código Postal es requerido.")]
        [StringLength(10, ErrorMessage = "El código postal no puede exceder los 10 caracteres.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "El código postal debe ser un número de 5 dígitos.")]
        [Display(Name = "Código Postal")]
        public string CP { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Condición de Pago es requerido.")]
        [StringLength(50, ErrorMessage = "La condición de pago no puede exceder los 50 caracteres.")]        
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras y números.")]//Data Annotation para permitir letras, numeros y espacios
        [Display(Name = "Condición de Pago")]
        public string Condicion_Pago { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Estatus es requerido.")]
        [StringLength(50, ErrorMessage = "El estatus no puede exceder los 50 caracteres.")]
        [Display(Name = "Estatus")]
        public string Estatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Tipo de Cliente es requerido.")]
        [Display(Name = "Tipo de Cliente")]       
        public string Tipo { get; set; }

        [Required(ErrorMessage = "El campo Fecha de Alta es requerido.")]
        [Display(Name = "Fecha de Alta")]
        public DateTime Fecha_Alta { get; set; } = DateTime.Now;

        [CustomValidation(typeof(Cliente), nameof(ValidateFechaBaja))]
        [Display(Name = "Fecha de Baja")]
        public DateTime? Fecha_Baja { get; set; }


        // Metodo para validar que la fecha de baja sea posterior a la fecha de alta
        public static ValidationResult ValidateFechaBaja(DateTime? fechaBaja, ValidationContext context)
        {
            var cliente = (Cliente)context.ObjectInstance;
            if (fechaBaja.HasValue && fechaBaja.Value < cliente.Fecha_Alta)
            {
                return new ValidationResult("La fecha de baja debe ser posterior a la fecha de alta.");
            }
            return ValidationResult.Success;
        }

        // Para obtener lista de cotizaciones del cliente
        [NotMapped]
        [ValidateNever]
        public List<Cotizacion> Cotizaciones { get; set; }

        // Metodo para obtener lista de ventas del cliente
        [NotMapped]
        [ValidateNever]
        public List<Venta> Ventas { get; set; }

    }
}
