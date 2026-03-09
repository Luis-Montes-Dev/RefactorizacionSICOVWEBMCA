using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Compra
    {
        [Key]       
        [Display(Name = "Código de Compra")]
        public int Id_compra { get; set; }

        [Required(ErrorMessage = "El campo Código Proveedor es requerido.")]
        [Display(Name = "Código Proveedor")]
        [ForeignKey("Proveedor")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El campo Código Cotización de Proveedor es requerido.")]
        [Display(Name = "Código Cotización de Proveedor")]
        [ForeignKey("CotizacionProveedor")]
        public int Id_Cotizacion_prov { get; set; }
        

        [Required(ErrorMessage = "El campo Código Empleado es requerido.")]
        [Display(Name = "Código Empleado")]
        [ForeignKey("Empleado")]
        public int IdEmpleado { get; set; }

        [Required(ErrorMessage = "El campo Fecha de compra es requerido.")]
        [Display(Name = "Fecha de Compra")]
        public DateTime Fecha_compra { get; set; } = DateTime.Now;
       

        [Required(ErrorMessage = "El campo Precio total es requerido.")]
        [Range(0.01, float.MaxValue, ErrorMessage = "El precio total debe ser mayor a cero")]
        [Display(Name = "Precio Total")]
        public decimal Costo_Total { get; set; }

        [Display(Name = "Tipo de Moneda")]
        public string? Tipo_Moneda { get; set; }

        [Required(ErrorMessage = "El campo Condición de pago es requerido.")]
        [StringLength(50, ErrorMessage = "La condición de pago no puede exceder los 50 caracteres.")]
        [Display(Name = "Condición de Pago")]
        public string Condicion_Pago { get; set; } = string.Empty;

        // Propiedades de navegación
        public Proveedor? Proveedor { get; set; }
        public Cotizacion_proveedor? CotizacionProveedor { get; set; }        
        public Empleado? Empleado { get; set; }

        // Relación uno a muchos con DetalleCotizacionProveedor
        public List<DetalleCompra>? Detalles { get; set; }
    }
}
