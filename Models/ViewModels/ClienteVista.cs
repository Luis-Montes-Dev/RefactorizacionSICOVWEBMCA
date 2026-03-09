using System.Diagnostics.CodeAnalysis;

namespace SICOVWEB_MCA.Models.ViewModels
{
    public class ClienteVista
    {
        //Se mapean las propiedades de cliente para usarlos en la vista
        public int Id { get; set; }
        public string Razon_Social { get; set; } = string.Empty;
        public string Rfc { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Calle { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string CP { get; set; } = string.Empty;
        public string Condicion_Pago { get; set; } = string.Empty;
        public string Tipo_Cliente { get; set; } = string.Empty;
        public DateTime Fecha_Alta { get; set; }
        [AllowNull]
        public DateTime? Fecha_Baja { get; set; }
    }
}
