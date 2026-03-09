using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Producto
    {
        [Key]        
        [Display(Name = "Id Producto")]
        public int Id_producto { get; set; }

        [Required(ErrorMessage = "El campo No. de proveedor es obligatorio.")]
        [Display(Name = "No. proveedor")]
        [ForeignKey("Proveedor")]
        public int Idproveedor { get; set; } // Relación con la tabla de proveedores

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El campo Nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Marca es obligatorio.")]
        [Display(Name = "Marca")]
        [StringLength(50, ErrorMessage = "El campo Marca no puede exceder los 50 caracteres.")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Descripción es obligatorio.")]
        [Display(Name = "Descripción")]
        [StringLength(150, ErrorMessage = "El campo Descripción no puede exceder los 150 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo Precio de compra es obligatorio.")]
        [Display(Name = "Precio de compra")]
        [Range(0.01, float.MaxValue, ErrorMessage = "El campo Precio de compra debe ser mayor que 0.")]
        public decimal Precio_Compra { get; set; }

        [Required(ErrorMessage = "El campo Precio de venta es obligatorio.")]
        [Display(Name = "Precio de venta")]
        [Range(0.01, float.MaxValue, ErrorMessage = "El campo Precio de venta debe ser mayor que 0.")]
        public decimal Precio_Venta { get; set; }

        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
        [Required(ErrorMessage = "El campo Unidades es obligatorio.")]
        [Display(Name ="Unidades")]
        public string? Unidad { get; set; }

        [Required(ErrorMessage = "El campo Margen de ganancia es obligatorio.")]
        [Display(Name = "Margen de ganancia")]
        public decimal Margen { get; set; }

        [Required(ErrorMessage = "El campo Stock actual es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El campo Stock actual debe ser mayor o igual que 0.")]
        [Display(Name = "Stock actual")]
        public int StockActual { get; set; }

        [Required(ErrorMessage = "El campo Stock mínimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El campo Stock mínimo debe ser mayor o igual que 0.")]
        [Display(Name = "Stock mínimo")]
        public int StockMinimo { get; set; }

        // Propiedades de navegación

        public Proveedor? Proveedor { get; set; } // Relación con la tabla de proveedores

        // Lista de movimientos de almacén relacionados con este producto
        [ValidateNever]
        public List<Movimiento_Almacen>? Movimientos_Almacen { get; set; } 


    }
}
