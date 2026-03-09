using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SICOVWEB_MCA.Models.ViewModels
{
    public class UsuarioVista
    {
        public int Id { get; set; }        
        public int EmpleadoId { get; set; }
        public string CorreoUsuario { get; set; }        
        public string? Contrasena { get; set; } 
        public string TipoUsuario { get; set; } 
    }
}
