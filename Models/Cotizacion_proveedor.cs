using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Cotizacion_proveedor
    {
        [Key]        
        [Display(Name = "No. Cotización proveedor")]
        public int Id_cotizacion { get; set; }

        [Required(ErrorMessage = "El campo No. Proveedor es obligatorio.")]
        [ForeignKey("Proveedor")]
        [Display(Name = "Id Proveedor")]
        public int Id_Proveedor { get; set; }

        [Required(ErrorMessage = "El campo Empleado es obligatorio.")]
        [Display(Name = "Empleado")]
        [ForeignKey("Empleado")]
        public int Id_Empleado3 { get; set; }

        [Required(ErrorMessage = "El campo Precio Total es obligatorio.")]        
        [Display(Name = "Precio Total")]
        public decimal Precio_total { get; set; }

        [Required(ErrorMessage = "El campo Fecha Cotización es obligatorio.")]
        [Display(Name = "Fecha Cotización")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El campo Fecha Vigencia es obligatorio.")]
        [Display(Name = "Fecha Vigencia")]
        public DateTime Fecha_Vigencia { get; set; }

        [Required(ErrorMessage = "El campo Estatus es obligatorio.")]
        [Display(Name = "Estatus")]
        public string Estatus { get; set; }

        [StringLength(150, ErrorMessage = "El campo Comentario no puede exceder los 150 caracteres.")]
        public string? Comentario { get; set; }

        [Display(Name = "Tipo de Moneda")]
        public string? Tipo_Moneda { get; set; }

        [Display(Name = "Condiciones de Pago")]
        public string? Condiciones_Pago { get; set; }


        // Propiedades de navegación
        [ForeignKey("Id_Proveedor")]
        public Proveedor? Proveedor { get; set; }
        [ForeignKey("Id_Empleado3")]
        public Empleado? Empleado { get; set; }

        // Relación uno a muchos con DetalleCotizacionProveedor
        public List<DetalleCotizacionProveedor>? Detalles { get; set; }
    }
}
