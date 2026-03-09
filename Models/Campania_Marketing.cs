using System.ComponentModel.DataAnnotations;

namespace SICOVWEB_MCA.Models
{
    public class Campania_Marketing
    {
        [Key]               
        [Display(Name = "Código de Campaña de Marketing")]
        public int Id_Campania { get; set; }

        [Required(ErrorMessage = "El campo Nombre de Campaña es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre de la campaña no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre de Campaña")]
        public required string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public required string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Fecha de inicio")]
        public DateTime Fecha_Inicio { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de fin")]
        public DateTime Fecha_Fin { get; set; }

    }
}
