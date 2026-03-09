using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SICOVWEB_MCA.Models
{
    public class Inventario
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Id debe ser mayor que 0.")]
        [Display(Name = "Id Inventario")]
        public int Id_inventario { get; set; }

        [Required(ErrorMessage = "El campo No. Almacén es obligatorio.")]
        [ForeignKey("Almacen")]
        [Display(Name = "No. Almacén")]
        public int Id_Almacen { get; set; }

        [Required(ErrorMessage = "El campo No. de Producto es obligatorio.")]
        [ForeignKey("Producto")]
        [Display(Name = "No. Producto")]
        public int Id_Producto { get; set; }

        [Required(ErrorMessage = "El campo Stock Actual es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El campo Stock Actual debe ser mayor o igual que 0.")]
        [Display(Name = "Stock Actual")]
        public int StockActual { get; set; }

        [Required(ErrorMessage = "El campo Stock Mínimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El campo Stock Mínimo debe ser mayor o igual que 0.")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        // Propiedades de navegación
        public Almacen? Almacen { get; set; }
        public Producto? Producto { get; set; }
    }
}
