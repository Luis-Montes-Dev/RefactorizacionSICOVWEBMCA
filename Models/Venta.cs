using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SICOVWEB_MCA.Models
{
    public class Venta
    {
        public Venta()
        {
        }
        [Key]        
        [Display(Name = "No. Venta")]
        public int Id_venta { get; set; }

        [Required(ErrorMessage = "El campo No. de Cliente es obligatorio.")]
        [ForeignKey("Cliente")]
        [Display(Name = "No. Cliente")]
        public int Id_Cliente { get; set; }

        [Required(ErrorMessage = "El campo No Cotización es obligatorio.")]
        [ForeignKey("Cotizacion")]
        [Display(Name = "Id Cotización")]
        public int Id_Cotizacion { get; set; }

        [ForeignKey("Empleado")]
        [Display(Name = "Id Empleado")]
        public int IdEmpleado { get; set; }

        [ForeignKey("Promocion")]
        [Display(Name = "Id Promoción")]
        [ValidateNever]
        public int? Id_Promocion { get; set; }  
        
       
        
        [Display(Name = "Fecha de Venta")]
        public DateTime Fecha_Venta { get; set; }

        [Display(Name = "Fecha de Entrega")]
        public DateTime? Fecha_Entrega { get; set; }

        [Display(Name = "Estatus")]
        [Required(ErrorMessage = "El campo Estatus es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo Estatus no puede exceder los 50 caracteres.")]
        public string Estatus { get; set; } = "Pendiente"; // Valor por defecto es "Nueva"

        [Display(Name = "Observaciones")]
        [StringLength(500, ErrorMessage = "El campo Observaciones no puede exceder los 500 caracteres.")]
        public string? Observaciones { get; set; } // Campo opcional para observaciones adicionales

       

        [Display(Name = "Persona Contacto")]
        public int? IdContacto { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Moneda es requerido.")]
        [StringLength(10, ErrorMessage = "El campo Tipo de Moneda no puede exceder los 10 caracteres.")]
        [Display(Name = "Tipo de Moneda")]
        public string? TipoMoneda { get; set; }= "MXN";

        [StringLength(45, ErrorMessage = "El campo Condición de Pago no puede exceder los 50 caracteres.")]
        [Display(Name = "Condición de Pago")]
        public string? CondicionPago { get; set; }= "Pago en una exhibición";

        // Propiedas para manejo de cantidades
        
        public decimal MontoTotal { get; set; } // Sumatoria de los detalles agregados
        
        [Display(Name = "Descuento")]
        [Range(0.01, 100, ErrorMessage = "El descuento debe estar entre 0.01 y 100.")]
        public decimal? Descuento { get; set; } // Porcentaje de descuento aplicado a la venta

        public decimal? MontoDescuento { get; set; } // Monto del descuento aplicado

        public decimal? Subtotal { get; set; } // Monto antes de impuestos

        public decimal? IVA { get; set; } // Monto del IVA aplicado

        [Display(Name = "Total con IVA")]
        [ValidateNever]
        public decimal? TotalConIva { get; set; }

        

        // Propiedades de navegación
        [ValidateNever]
        public Cliente Cliente { get; set; } // Relación con la tabla de clientes
        [ValidateNever]
        public Cotizacion? Cotizacion { get; set; } // Relación con la tabla de cotizaciones
        [ValidateNever]
        public Empleado? Empleado { get; set; } // Relación con la tabla de empleados
        [ValidateNever]
        public Promocion? Promocion { get; set; } // Relación con la tabla de promociones
        
        // Propiedad de navegación para los detalles
        
        public virtual List<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

        [ValidateNever]
        [NotMapped]
        public Contacto_cliente Contacto { get; set; }
    }
}
