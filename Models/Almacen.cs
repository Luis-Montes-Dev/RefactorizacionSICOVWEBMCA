using System.ComponentModel.DataAnnotations;

namespace SICOVWEB_MCA.Models
{
    public class Almacen
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "El código de almacén debe ser mayor a cero")]
        [Display(Name = "Código de Almacén")]
        public int AlmacenId { get; set; }

        [Required(ErrorMessage = "El campo Nombre de Almacén es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre del almacén no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre de Almacén")]
        public string Nombre { get; set; }= string.Empty;

        [StringLength(150, ErrorMessage = "La ubicación no puede exceder los 150 caracteres.")]
        [Display(Name = "Ubicación")]
        public string? Ubicacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Tipo de almacén es requerido.")]
        [Display(Name = "Tipo de Almacén")]
        [EnumDataType(typeof(TipoAlmacen), ErrorMessage = "El tipo de almacén no es válido.")]
        public TipoAlmacen Tipo_almacen { get; set; } // Propio o Externo

        public enum TipoAlmacen
        {
            Propio,
            Externo
        }
    }
}
