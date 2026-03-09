
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace SICOVWEB_MCA.Models
{
    public class Cotizacion
    {
        [Key]
        public int IdCotizacion { get; set; }

        [Required(ErrorMessage = "El campo Cliente es requerido.")]
        [ForeignKey("Cliente")]
        [Display(Name = "Código Cliente")]
        public int IdCliente { get; set; }
               
        [Required(ErrorMessage = "El campo Empleado es requerido.")]
        [ForeignKey("Empleado")]
        [Display(Name = "Código Empleado")]
        public int IdEmpleado2 { get; set; }

        [Display(Name = "Fecha de cotización")]
        [Required(ErrorMessage = "El campo Fecha es requerido.")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        
        public static ValidationResult ValidateFechaVigencia(DateTime fechaVigencia, ValidationContext context)
        {
            if (fechaVigencia <= DateTime.Now)
            {
                return new ValidationResult("La fecha de vigencia debe ser una fecha futura.");
            }
            return ValidationResult.Success;
        }
        [Display(Name = "Fecha de Vigencia")]
        [CustomValidation(typeof(Cotizacion), nameof(ValidateFechaVigencia))]
        [Required(ErrorMessage = "El campo Fecha de Vigencia es requerido.")]
        public DateTime FechaVigencia { get; set; }

        [Display(Name = "Estatus")]
        public string Estatus { get; set; } = "Nueva";

        [Display(Name = "Persona Contacto")]
        public int? IdContacto { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Moneda es requerido.")]
        [StringLength(10, ErrorMessage = "El campo Tipo de Moneda no puede exceder los 10 caracteres.")]
        [Display(Name = "Tipo de Moneda")]
        public string? TipoMoneda { get; set; } = "MXN";

        [StringLength(45, ErrorMessage = "El campo Condición de Pago no puede exceder los 50 caracteres.")]
        [Display(Name = "Condición de Pago")]
        public string? CondicionPago { get; set; } = "Pago en una exhibición";

        [StringLength(200, ErrorMessage = "El campo Comentario no puede exceder los 200 caracteres.")]
        [Display(Name = "Comentarios")]
        public string? Comentario { get; set; } = "Sin comentarios";

        // Propiedad de navegación para los detalles

        public virtual List<DetalleCotizacionCliente> Detalles { get; set; } = new List<DetalleCotizacionCliente>();

        //Propiedades de navegacion
        [ValidateNever]
        public Cliente Cliente { get; set; }
        [ValidateNever]
        public Empleado Empleado { get; set; }

        [ValidateNever]
        [NotMapped]
        public Contacto_cliente Contacto { get; set; }
    }
}
