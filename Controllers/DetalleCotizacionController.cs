using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Models;

namespace SICOVWEB_MCA.Controllers
{
    public class DetalleCotizacionController : Controller
    {
        public IActionResult _DetalleCotizacionCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GuardarDetalleCotización(DetalleCotizacionCliente detalle)
        {


            return View("~/Views/Cotizaciones/VistaCotizaciones.cshtml", detalle);
        }
    }
}
