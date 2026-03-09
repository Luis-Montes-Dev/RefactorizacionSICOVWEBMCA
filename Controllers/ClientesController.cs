using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Helpers;
using SICOVWEB_MCA.Models;
using SICOVWEB_MCA.Models.ViewModels;

namespace SICOVWEB_MCA.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;//Permiso de solo lectura al contexto DB

        public ClientesController(ApplicationDbContext context)//Constructor usa el contexto como parametro
        {
            _context = context;
        }
        // Accion para mostrar la vista VistaClientes
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult VistaClientes()
        {
            return View();
        }

        // Metodo para mostrar la vista Crear cliente
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CrearCliente()
        {
            return View();
        }


        // Metodo para mostrar la lista de clientes <<<<ELIMINAR<<<<
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult ListaClientes()
        {
            var listaClientes = _context.Clientes.ToList();                

            return View("ListaClientes", listaClientes);
        }

        // Metodo para buscar clientes
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(int? idCliente, string razonSocial, 
            string tipo, DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            int tamanioPagina = 10; // Número de registros por página

            // Construir la consulta base
            var query = _context.Clientes.AsQueryable();

            if (idCliente.HasValue)
                query = query.Where(c => c.Id_cliente == idCliente);

            if (!string.IsNullOrEmpty(razonSocial))
                query = query.Where(c => c.Razon_Social == razonSocial);

            if (!string.IsNullOrEmpty(tipo))
                query = query.Where(c => c.Tipo == tipo);

            if (fechaInicio.HasValue)
                query = query.Where(c => c.Fecha_Alta >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(c => c.Fecha_Alta <= fechaFin.Value);

            // Aplicar paginación
            var listaPaginada = await Paginacion<Cliente>.CrearAsync(query, pagina, tamanioPagina);

            return View("ListaClientes", listaPaginada);
        }

        // Metodos para manejar las acciones de clientes

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCliente(Cliente cliente) // Método para registrar un nuevo cliente
        {
            // Convertir RFC a mayusculas
            cliente.Rfc = cliente.Rfc.ToUpper();
            try
            {
                // Validar si ya existe un cliente con la misma razon social
                var existente = _context.Clientes.FirstOrDefault(c => c.Razon_Social == cliente.Razon_Social);
                if (existente != null)
                {
                    ViewBag.Mensaje = "Ya existe un cliente con esa Razón Social.";
                    //Mostrar mensaje de error en una alert en la pagina                    
                    TempData["MensajeAlertFalla"] = "Ya existe un cliente con esa Razón Social.";                    
                    return View();
                }                                
                // Guardar en base de datos
                _context.Clientes.Add(cliente);
                _context.SaveChanges();
                ViewBag.Mensaje = "Cliente registrado exitosamente.";
                //Mostrar mensaje de exito en una alert en la pagina
                TempData["MensajeAlertExito"] = "Cliente registrado exitosamente.";
                return RedirectToAction("Buscar");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error al registrar: " + ex.Message;
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error al registrar: " + ex.Message;
                return View();
            }
        }

        //Metodo para mostrar la vista de editar cliente        
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Editar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        //Metodo para editar un cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Cliente cliente)
        {
            if (id != cliente.Id_cliente) // Verifica si el ID del cliente coincide
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {                
                try
                {
                    _context.Update(cliente); // Actualiza el cliente en el contexto
                    await _context.SaveChangesAsync();// Guarda los cambios en la base de datos                    
                }
                catch (DbUpdateConcurrencyException ex)

                {
                    // Enviar un mensaje a la vista con tempdata si ocurre un error
                    TempData["MensajeAlertFalla"] = "Error al editar el cliente." + ex.Message;
                    return View(cliente);
                }
                // Enviar un mensaje a la vista con tempdata si se edita correctamente
                TempData["MensajeAlertExito"] = "Cliente actualizado exitosamente.";
                return RedirectToAction("Buscar");
            }
            TempData["MensajeAlertFalla"] = "Error al actualizar el cliente, verificar los datos ingresados.";
            return View(cliente);
        }

        //Metodo para confirmar la eliminacion de un cliente
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Eliminar(int? id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id_cliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        //Metodo para eliminar un cliente
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id); // Busca el cliente en la BD
            if (cliente != null) //Si el cliente es diferente de nulo
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["MensajeAlertExito"] = "Cliente eliminado correctamente.";
            }
            else
            {
                TempData["MensajeAlertFalla"] = "Cliente no encontrado.";
                return NotFound();
            }

                return RedirectToAction("Buscar");
        }

        // Metodo para mostrar reporte del cliente con su informacion con una lista de cotizaciones y de ventas asociadas
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ReporteCliente(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id_cliente == id);
            if (cliente == null)
            {
                TempData["MensajeAlertFalla"] = "Cliente no encontrado.";
                return RedirectToAction("Buscar");
            }
            // Incluir las cotizaciones y ventas asociadas al cliente
            cliente.Cotizaciones = await _context.Cotizaciones
                .Include(c => c.Empleado)
                .Where(c => c.IdCliente == id)
                .ToListAsync();
            cliente.Ventas = await _context.Ventas
                .Include(v => v.Empleado)
                .Where(v => v.Id_Cliente == id)
                .ToListAsync();
            TempData["MensajeAlertExito"] = "Reporte generado correctamente.";
            return View(cliente);
        }
    }
}
