using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Evaluacion_Venta
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Id debe ser mayor que 0.")]
        [Display(Name = "No. Evaluación venta")]
        public int Id_evaluacion { get; set; }

        [Required(ErrorMessage = "El campo Id Venta es obligatorio.")]
        [ForeignKey("Venta")]
        [Display(Name = "Id Venta")]
        public int Id_venta { get; set; }

        [Required(ErrorMessage = "El campo Calificación es obligatorio.")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        [Display(Name = "Calificación")]
        public int Calificacion { get; set; }

        [StringLength(150, ErrorMessage = "El comentario no puede exceder los 150 caracteres.")]
        [Display(Name = "Comentario")]
        public string Comentarios { get; set; } = string.Empty;

        // Propiedades de navegación
        public Venta? Venta { get; set; }
    }
}
