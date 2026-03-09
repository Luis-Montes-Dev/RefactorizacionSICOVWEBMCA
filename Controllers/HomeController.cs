using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SICOVWEB_MCA.Models;

namespace SICOVWEB_MCA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["MensajeAlertFalla"]= "Para salir de su cuenta utilice el botón Cerrar Sesión.";
                return RedirectToAction("PrincipalAdmin", "Login_Controlador");
            }

            return View();
        }
        public IActionResult Recuperar_Contrasena()
        {
            return View();
        }
        
        public IActionResult Clientes()
        {
            return View();
        }
        public IActionResult Proveedores()
        {
            return View();
        }
        public IActionResult Reportes()
        {
            return View();
        }
        public IActionResult Ventas()
        {
            return View();
        }
        public IActionResult Inventario()
        {
            return View();
        }
        public IActionResult Empleados()
        {
            return View();
        }
        public IActionResult Usuarios()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contacto()
        {
            return View();
        }
        
        public IActionResult Registrar()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
