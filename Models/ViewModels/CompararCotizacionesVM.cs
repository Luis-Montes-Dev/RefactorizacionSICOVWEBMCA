namespace SICOVWEB_MCA.Models.ViewModels
{
    public class CompararCotizacionesVM
    {
        public int? IdCotizacion1 { get; set; }
        public int? IdCotizacion2 { get; set; }
        public Cotizacion_proveedor? Cotizacion1 { get; set; }
        public Cotizacion_proveedor? Cotizacion2 { get; set; }
    }
}
