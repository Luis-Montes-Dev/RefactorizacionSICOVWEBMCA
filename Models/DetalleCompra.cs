using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SICOVWEB_MCA.Models
{
    public class DetalleCompra
    {
        [Key]
        public int Id_Detalle { get; set; }

        [Required(ErrorMessage = "El campo No. Cotización Proveedor es obligatorio.")]
        [Display(Name = "Numero de Compra")]
        [ForeignKey("Compra")]
        public int Id_Compra { get; set; }

        [Required(ErrorMessage = "El Nombre de producto es obligatorio.")]
        [StringLength(45)]
        [Display(Name = "Nombre de producto")]
        public string Nombre_producto { get; set; }

        [Required(ErrorMessage = "La descripción de producto es obligatoria.")]
        [StringLength(150)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo Cantidad es obligatorio.")]
        [Display(Name = "Cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo Precio Unitario es obligatorio.")]
        [Display(Name = "Precio Unitario")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El campo Precio Unitario debe ser mayor que 0.")]
        public decimal Precio_Unitario { get; set; }

        public decimal Subtotal { get; set; }

        public int Tiempo_Entrega { get; set; }

        // Propiedades de navegación
        [ForeignKey("Id_Compra")]
        public Compra? Compra { get; set; }

    }
}
