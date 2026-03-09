using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Helpers;
using SICOVWEB_MCA.Models;
using System.Net.NetworkInformation;

namespace SICOVWEB_MCA.Controllers
{
    public class ContactosClientesController : Controller
    {
        // Permiso de solo lectura al contexto DB
        private readonly ApplicationDbContext _context;
        // Constructor usa el contexto como parametro
        public ContactosClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Accion mostrar vista CrearContactoCliente
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CrearContactoCliente()
        {
            ViewBag.listaClientes = _context.Clientes.ToList(); // Lista de clientes
            return View();
        }

        //Metodo para Crear un contacto de cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearContactoCliente(Contacto_cliente contactocliente)
        {
            try
            {
                // Validar si ya existe un contacto con el mismo nombre y apellidos
                var existente = _context.Contactos_cliente.FirstOrDefault(c => c.Nombre == contactocliente.Nombre && c.Apellido_paterno == contactocliente.Apellido_paterno && c.Apellido_materno == contactocliente.Apellido_materno);

                //var existente = _context.Contactos_cliente.FirstOrDefault(c => c.Nombre == contactocliente.Nombre);
                if (existente != null)
                {
                    //Mostrar mensaje de error en una alert en la pagina                    
                    TempData["MensajeAlertFalla"] = "Ya existe un contacto de cliente con ese nombre.";
                    ViewBag.listaClientes = _context.Clientes.ToList(); // Lista de clientes
                    return View();
                }

                // Guardar en base de datos
                _context.Contactos_cliente.Add(contactocliente);
                _context.SaveChanges();
                //Mostrar mensaje de exito en una alert en la pagina
                TempData["MensajeAlertExito"] = "Contacto de cliente registrado exitosamente.";
                return RedirectToAction("Buscar");
            }
            catch (Exception ex)
            {
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error al registrar: " + ex.Message;
                Console.WriteLine("****ERROR : " +
                    ex.InnerException);
                ViewBag.listaClientes = _context.Clientes.ToList(); // Lista de clientes
                return View();
            }
        }

       public async Task<IActionResult> ListaContactosClientes(int pagina = 1)
        {
            int tamanioPagina = 10; // Número de registros por página

            var query = _context.Contactos_cliente
                .Include(c => c.Cliente) // Incluir la entidad Cliente
                .AsQueryable();

            // Aplicar paginación
            var listaPaginada = await Paginacion<Contacto_cliente>.CrearAsync(query, pagina, tamanioPagina);

            return View("ListaContactosClientes", listaPaginada);
        }

        // Metodo para buscar Contactos de clientes
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(int? idContacto, string razonSocial, 
            DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            int tamanioPagina = 10; // Número de registros por página

            // Cargar listas para los filtros

            var query = _context.Contactos_cliente
                .Include(c => c.Cliente) // Incluir la entidad Cliente
                .AsQueryable();  

            if (idContacto.HasValue)
                query = query.Where(c => c.Id_contacto == idContacto);

            if (!string.IsNullOrEmpty(razonSocial))
                query = query.Where(c => c.Cliente.Razon_Social == razonSocial);          

            if (fechaInicio.HasValue)
                query = query.Where(c => c.Fecha_alta >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(c => c.Fecha_alta <= fechaFin.Value);

            // Aplicar paginación
            var listaPaginada = await Paginacion<Contacto_cliente>.CrearAsync(query, pagina, tamanioPagina);

            // Mantener los filtros seleccionados al cambiar de página
            ViewBag.Filtros = new
            {
                idContacto,
                razonSocial,
                fechaInicio = fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin = fechaFin?.ToString("yyyy-MM-dd")
            };

            return View("ListaContactosClientes", listaPaginada);
        }

        //Metodo para mostrar la vista editar contacto de cliente
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Editar(int id)
        {
            var contactocliente = await _context.Contactos_cliente.FindAsync(id);
            if (contactocliente == null)
            {
                return NotFound();
            }
            return View(contactocliente);
        }

        //Metodo para editar un contacto de cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int? id ,Contacto_cliente contactocliente)
        {
            if (id == null || id != contactocliente.Id_contacto)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Contactos_cliente.Update(contactocliente);
                    _context.SaveChanges();
                    TempData["MensajeAlertExito"] = "Contacto de cliente actualizado exitosamente.";
                    return RedirectToAction("Buscar");
                }
                catch (Exception ex)
                {
                    TempData["MensajeAlertFalla"] = "Error al actualizar: " + ex.Message ;
                    return View(contactocliente);
                }
            }
            TempData["MensajeAlertFalla"] = "Error al actualizar, verifica los datos ingresados ";
            return View(contactocliente);
        }

        //Metodo para mostrar la vista eliminar contacto de cliente
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var contactocliente = await _context.Contactos_cliente.FirstOrDefaultAsync(c => c.Id_contacto == id);
            if (contactocliente == null)
            {
                return NotFound();
            }
            return View(contactocliente);
        }

        //Metodo para eliminar un contacto de cliente
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarContacto(int id)
        {
            var contactoClienteFromDb = await _context.Contactos_cliente.FindAsync(id); // Busca el Contacto de cliente en la BD
            if (contactoClienteFromDb != null) //Si el contacto cliente es diferente de nulo
            {
                try
                {
                    _context.Contactos_cliente.Remove(contactoClienteFromDb);
                    await _context.SaveChangesAsync();
                    TempData["MensajeAlertExito"] = "Contacto de cliente eliminado exitosamente.";
                    return RedirectToAction("Buscar");
                }
                catch (Exception ex)
                {
                    TempData["MensajeAlertFalla"] = "Error al eliminar: " + ex.Message;
                    return View(contactoClienteFromDb);
                    throw;
                }
            }
            else
            {
                TempData["MensajeAlertFalla"] = "Contacto de cliente no encontrado.";
                return View(contactoClienteFromDb);
            }  
        }
    }
}
