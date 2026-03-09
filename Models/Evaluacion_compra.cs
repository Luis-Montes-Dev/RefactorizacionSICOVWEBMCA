using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICOVWEB_MCA.Models
{
    public class Evaluacion_compra
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "El campo Id debe ser mayor que 0.")]
        [Display(Name = "No. Evaluación compra")]
        public int Id_evaluacion { get; set; }

        [Required(ErrorMessage = "El campo No. de Compra es obligatorio.")]
        [ForeignKey("Compra")]
        [Display(Name = "Id Compra")]
        public int Id_compra { get; set; }

        [Required(ErrorMessage = "El campo Calificación es obligatorio.")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        [Display(Name = "Calificación")]
        public int Calificación { get; set; }

        [StringLength(150, ErrorMessage = "El comentario no puede exceder los 150 caracteres.")]
        [Display(Name = "Comentario")]
        public required string Comentario { get; set; }

        // Propiedades de navegación
        public Compra? Compra { get; set; }
    }
}
