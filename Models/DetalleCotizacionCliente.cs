using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class DetalleCotizacionCliente
    {
        [Key]        
        [Display(Name = "Código de Detalle")]
        public int IdDetalle { get; set; }


        [Display(Name = "Código de Cotización")]
        public int IdCotizacion { get; set; }


        [Display(Name = "Código de Producto")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Código de Producto debe ser un número mayor a cero.")]
        public int IdProducto { get; set; } = 19;

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
        [Range(0.00, double.MaxValue, ErrorMessage = "El precio unitario debe ser positivo")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        [StringLength(50, ErrorMessage = "El campo Tiempo de Entrega no puede exceder los 50 caracteres.")]
        [Display(Name = "Tiempo de Entrega")]
        public string? TiempoEntrega { get; set; }= "Consultar cotización";

        // Propiedades de navegación
        [ValidateNever]
        [ForeignKey("IdCotizacion")]
        public Cotizacion? Cotizacion { get; set; }
        
        [ValidateNever]
        [ForeignKey("IdProducto")]
        public Producto? Producto { get; set; }
        [ValidateNever]
        public decimal PrecioTotal => Cantidad * PrecioUnitario;

        [NotMapped]
        public bool Eliminar { get; set; } = false;
    }
}
