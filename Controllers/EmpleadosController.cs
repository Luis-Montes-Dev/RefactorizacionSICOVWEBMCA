using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Helpers;
using SICOVWEB_MCA.Models;
using SICOVWEB_MCA.Models.ViewModels;

namespace SICOVWEB_MCA.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmpleadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public IActionResult CrearEmpleado()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearEmpleado(Empleado empleado)
        {
            // Convertir RFC a mayusculas
            empleado.RFC = empleado.RFC.ToUpper();

            try
            {
                // Validar si ya existe un Empleado con el mismo RFC 
                var existente = _context.Empleados.FirstOrDefault(e => e.RFC == empleado.RFC);
                if (existente != null)
                {
                    //Mostrar mensaje de error en una alert en la pagina                    
                    TempData["MensajeAlertFalla"] = "Ya existe un empleado con ese RFC.";
                    return View(empleado);
                }

                // Guardar en base de datos
                _context.Empleados.Add(empleado);
                _context.SaveChanges();
                
                //Mostrar mensaje de exito en una alert en la pagina
                TempData["MensajeAlertExito"] = "Empleado registrado exitosamente.";
                return RedirectToAction("Buscar");
            }
            catch (Exception ex)
            {  
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error al registrar: " + ex.Message ;
                return View(empleado);
            }
        }
        // Metodo para buscar empleados
        [HttpGet]
        public async Task<IActionResult> Buscar(int? idEmpleado, string apellidoPaterno, 
            string estatus, DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            int tamanioPagina = 10;

            // Construir la consulta con los filtros
            var query = _context.Empleados.AsQueryable();

            if (idEmpleado.HasValue)
                query = query.Where(c => c.Id == idEmpleado);

            if (!string.IsNullOrEmpty(apellidoPaterno))
                query = query.Where(c => c.Apellido_Paterno == apellidoPaterno);

            if (!string.IsNullOrEmpty(estatus))
                query = query.Where(c => c.Estatus == estatus);

            if (fechaInicio.HasValue)
                query = query.Where(c => c.Fecha_Alta >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(c => c.Fecha_Alta <= fechaFin.Value);

            //var lista = query.ToList();

            // Aplicar paginación
            var listaPaginada = await Paginacion<Empleado>.CrearAsync(query, pagina, tamanioPagina);

            // Mantener los filtros seleccionados al cambiar de página
            ViewBag.Filtros = new
            {
                idEmpleado,
                apellidoPaterno,
                estatus,
                fechaInicio = fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin = fechaFin?.ToString("yyyy-MM-dd")
            };

            return View("ListaEmpleados", listaPaginada);
        }

        //Metodo para mostrar la vista de editar empleados

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Editar(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        //Metodo para editar un empleado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado); // Actualiza el empleado en el contexto
                    await _context.SaveChangesAsync();// Guarda los cambios en la base de datos                    
                }
                catch (DbUpdateConcurrencyException ex)

                {
                    // Enviar un mensaje a la vista con tempdata si ocurre un error
                    TempData["MensajeAlertFalla"] = "Error al editar empleado." + ex.Message + ex.InnerException;
                    if (!_context.Empleados.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["MensajeAlertFalla"] = "Error al editar empleado: " + ex.Message;
                        throw;
                    }
                }
                // Enviar un mensaje a la vista con tempdata si se edita correctamente
                TempData["MensajeAlertExito"] = "Empleado editado exitosamente.";
                return RedirectToAction("Buscar");
            }
            return View(empleado);
        }

        //Metodo para confirmar la eliminacion de un empleado
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Eliminar(int? id)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(c => c.Id == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        //Metodo para eliminar un empleado

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id); // Busca el cliente en la BD
            if (empleado != null) //Si el cliente es diferente de nulo
            {
                _context.Empleados.Remove(empleado);
                await _context.SaveChangesAsync();
                TempData["MensajeAlertExito"] = "Empleado eliminado correctamente.";
            }
            else
            {
                TempData["MensajeAlertFalla"] = "Empleado no encontrado.";
                return NotFound();
            }

            return RedirectToAction("Buscar");
        }
    }
}
