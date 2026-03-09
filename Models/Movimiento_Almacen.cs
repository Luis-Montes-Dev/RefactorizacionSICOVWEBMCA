using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Movimiento_Almacen
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Id debe ser mayor que 0.")]
        [Display(Name = "Id Movimiento")]
        public int Id_movimiento { get; set; }

        [Required(ErrorMessage = "El campo No. de Almacén es obligatorio.")]
        [ForeignKey("Almacen")]
        [Display(Name = "No. Almacén")]
        public int Id_Almacen { get; set; }

        [Required(ErrorMessage = "El campo Tipo es obligatorio.")]
        [Display(Name = "Tipo de movimiento")]
        public string Tipo { get; set; } // "Entrada" o "Salida"

        [Required(ErrorMessage = "El campo Id Producto es obligatorio.")]
        [ForeignKey("Producto")]
        [Display(Name = "Id Producto")]
        public int Id_Producto { get; set; }
        

        [Display(Name = "Id Compra")]
        [ForeignKey("Compra")]
        public int? Id_Compra { get; set; } // Relación con la tabla de compras

        [Display(Name = "Id Venta")]
        [ForeignKey("Venta")]
        public int? Id_Venta { get; set; } // Relación con la tabla de ventas

        [Required(ErrorMessage = "El campo Cantidad es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Cantidad debe ser mayor que 0.")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo Fecha es obligatorio.")]
        [Display(Name = "Fecha de movimiento")]
        public DateTime Fecha { get; set; }
        

        // Propiedades de navegación
        public Almacen? Almacen { get; set; }
        public Producto? Producto { get; set; }
        public Compra? Compra { get; set; } // Relación con la tabla de compras
        public Venta? Venta { get; set; } // Relación con la tabla de ventas

    }
}
