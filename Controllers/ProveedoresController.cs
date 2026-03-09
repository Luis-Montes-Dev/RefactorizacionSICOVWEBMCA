using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Helpers;
using SICOVWEB_MCA.Models;

namespace SICOVWEB_MCA.Controllers
{
    public class ProveedoresController : Controller
    {
        // Propiedad para el contexto de la base de datos
        private readonly ApplicationDbContext _context;
        public ProveedoresController(ApplicationDbContext context)
        {
            _context = context; // Inicializa el contexto de la base de datos
        }

        // Metodo para mostrar la vista de crear proveedores
        [HttpGet]
        [Authorize]
        public IActionResult CrearProveedor()
        {
            return View();
        }

        // Metodo para crear un proveedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearProveedor(Proveedor proveedor)
        {
            // convertir RFC a mayusculas
            proveedor.Rfc = proveedor.Rfc.ToUpper();
            try
            {
                // Validar si ya existe un Empleado con el mismo RFC 
                var existente = _context.Proveedores.FirstOrDefault(p => p.Razon_social == proveedor.Razon_social);
                if (existente != null)
                {
                    //Mostrar mensaje de error en una alert en la pagina                    
                    TempData["MensajeAlertFalla"] = "Ya existe un proveedor con esa Razón Social.";
                    return View(proveedor);
                }

                // Guardar en base de datos
                _context.Proveedores.Add(proveedor);
                _context.SaveChanges();

                //Mostrar mensaje de exito en una alert en la pagina
                TempData["MensajeAlertExito"] = "Proveedor registrado exitosamente.";
                return RedirectToAction("Buscar");
            }
            catch (Exception ex)
            {
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error al registrar: " + ex.Message ;
                return View(proveedor);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListaProveedores(int pagina = 1)
        {
            int tamanioPagina = 10; // Número de registros por página
            var query = _context.Proveedores.AsQueryable();

            // Aplicar paginación
            var listaPaginada = await Paginacion<Proveedor>.CrearAsync(query, pagina, tamanioPagina);

            return View("ListaProveedores", listaPaginada);
        }

        // Metodo para buscar Proveedores con filtros
        [HttpGet]
        public async Task<IActionResult> Buscar(int? id, string razonSocial, string estatus ,  
            DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            int tamanioPagina = 10; // Número de registros por página

            // Cargar listas para los filtros

            var query = _context.Proveedores.AsQueryable();

            if (id.HasValue)
                query = query.Where(c => c.IdProveedor == id);

            if (!string.IsNullOrEmpty(razonSocial))
                query = query.Where(c => c.Razon_social == razonSocial);

            if (!string.IsNullOrEmpty(estatus))
                query = query.Where(c => c.Estatus == estatus);

            if (fechaInicio.HasValue)
                query = query.Where(c => c.Fecha_alta >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(c => c.Fecha_alta <= fechaFin.Value);

            // Aplicar paginación
            var listaPaginada = await Paginacion<Proveedor>.CrearAsync(query, pagina, tamanioPagina);

            // Mantener los filtros seleccionados al cambiar de página
            ViewBag.Filtros = new
            {
                id,
                razonSocial,
                estatus,
                fechaInicio = fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin = fechaFin?.ToString("yyyy-MM-dd")
            };
            return View("ListaProveedores", listaPaginada);
        }

        //Metodo para mostrar la vista de editar proveedores

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Editar(int id)
        {
            var proveedores = await _context.Proveedores.FindAsync(id);
            if (proveedores == null)
            {
                return NotFound();
            }
            return View(proveedores);
        }

        //Metodo para editar un proveedores
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Proveedor proveedor)
        {
            if (id != proveedor.IdProveedor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor); // Actualiza el proveedores en el contexto
                    await _context.SaveChangesAsync();// Guarda los cambios en la base de datos                    
                }
                catch (DbUpdateConcurrencyException ex)

                {
                    if (!_context.Proveedores.Any(e => e.IdProveedor == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["MensajeAlertFalla"] = "Error al editar proveedor: " + ex.Message;
                        throw;
                    }
                }
                // Enviar un mensaje a la vista con tempdata si se edita correctamente
                TempData["MensajeAlertExito"] = "Proveedor editado exitosamente.";
                return RedirectToAction("ListaProveedores");
            }
            TempData["MensajeAlertFalla"] = "Error al editar proveedor, verificar los datos.";
            return View(proveedor);
        }

        //Metodo para confirmar la eliminacion de un Proveedor
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Eliminar(int? id)
        {
            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(c => c.IdProveedor == id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        //Metodo para eliminar un proveedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int IdProveedor)
        {
            try
            {
                // Verificar si el proveedor tiene registros relacionados en otras tablas
                bool tieneRegistrosRelacionados = _context.Productos.Any(p => p.Idproveedor == IdProveedor);
                if (tieneRegistrosRelacionados)
                {
                    TempData["MensajeAlertFalla"] = "No se puede eliminar el proveedor porque tiene registros relacionados.";
                    return RedirectToAction("ListaProveedores");
                }
                // Si no tiene registros relacionados, eliminar
                var proveedor = await _context.Proveedores.FindAsync(IdProveedor);
                if (proveedor != null)
                {
                    _context.Proveedores.Remove(proveedor);
                    await _context.SaveChangesAsync();
                    TempData["MensajeAlertExito"] = "Proveedor eliminado correctamente.";
                }
                else
                {
                    TempData["MensajeAlertFalla"] = "Proveedor no encontrado.";
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al eliminar el proveedor: " + ex.Message;
                return RedirectToAction("ListaProveedores");
            }
           
            return RedirectToAction("ListaProveedores");
        }

        // Método para mostrar reporte del proveedor con su información, con una lista de cotizaciones, compras, y productos asociadas
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReporteProveedores(int id)
        {
            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.IdProveedor == id);
            if (proveedor == null)
            {
                TempData["MensajeAlertFalla"] = "Proveedor no encontrado.";
                return RedirectToAction("Buscar");
            }
            // Incluir las cotizaciones asociadas
            proveedor.Cotizaciones = await _context.Cotizaciones_proveedores
                .Include(c => c.Empleado)
                .Where(c => c.Id_Proveedor == id)
                .ToListAsync();
            // Incluir las compras asociadas
            proveedor.Compras = await _context.Compras
                .Include(c => c.Empleado)
                .Where(c => c.IdProveedor == id)
                .ToListAsync();
            // Incluir los productos asociados
            proveedor.Productos = await _context.Productos
                .Include(p => p.Proveedor)
                .Where(p => p.Idproveedor == id)
                .ToListAsync();
            TempData["MensajeAlertExito"] = "Reporte generado correctamente.";
            return View(proveedor);
        }
    }
}
