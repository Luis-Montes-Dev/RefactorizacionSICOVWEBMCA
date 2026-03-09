using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Promocion
    {
        [Key]        
        [Display(Name = "No. Promocion")]
        public int Id_promocion { get; set; }
                
        [ForeignKey("Campania")]
        [Display(Name = "No. Campaña")]
        public int Id_Campania { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Descripción es obligatorio.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Descuento es obligatorio.")]
        [Range(0.01, 100, ErrorMessage = "El campo Descuento debe estar entre 0.01 y 100.")]
        [Display(Name = "Descuento")]
        public decimal Descuento { get; set; }

        [Required(ErrorMessage = "El campo Fecha de inicio es obligatorio.")]
        [Display(Name = "Fecha de inicio")]
        public DateTime Fecha_Inicio { get; set; }

        [Required(ErrorMessage = "El campo Fecha de fin es obligatorio.")]
        [Display(Name = "Fecha de fin")]
        public DateTime Fecha_Fin { get; set; }

        // Propiedades de navegación
        [ForeignKey("Id_Campania")]
        [ValidateNever] // Evitar validación en vistas
        public Campania_Marketing Campania { get; set; } // Relación con la tabla de campañas

        
    }
}
