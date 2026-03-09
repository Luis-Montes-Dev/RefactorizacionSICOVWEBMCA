using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class DetalleVenta
    {
        [Key]
        [Display(Name = "Código Detalle Venta")]
        public int IdDetalleVenta { get; set; }

        [ForeignKey("Venta")]
        [Display(Name = "Código de Venta")]
        public int Id_venta { get; set; }

        [Display(Name = "Código de Producto")]
        public int IdProducto  { get; set; }

        [StringLength(150, ErrorMessage = "El campo Descripción del Producto no puede exceder los 150 caracteres.")]
        [Display(Name = "Descripción del Producto")]
        public string? Descripcion { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El campo Unidad no puede exceder los 50 caracteres.")]
        [Display(Name = "Unidad")]
        public string? Unidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Cantidad es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo Precio Unitario es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a cero")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [StringLength(50, ErrorMessage = "El campo Tiempo de Entrega no puede exceder los 50 caracteres.")]
        [Display(Name = "Tiempo de Entrega")]
        public string? TiempoEntrega { get; set; } = "Consultar cotización";

        // Propiedades de navegación
        [ValidateNever]
        [ForeignKey("Id_venta")]
        public Venta? Venta { get; set; }
        
        [ValidateNever]
        [ForeignKey("IdProducto")]
        public Producto? Producto { get; set; }

        [NotMapped]
        public bool Eliminar { get; set; } = false;

    }
}
