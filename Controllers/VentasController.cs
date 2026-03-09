using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using SICOVWEB_MCA.Extensions;
using SICOVWEB_MCA.Helpers;
using SICOVWEB_MCA.Models;
using SICOVWEB_MCA.PDFDocs;

namespace SICOVWEB_MCA.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public JsonResult GetContactosPorCliente(int idCliente)
        {
            var contactos = _context.Contactos_cliente
                .Where(c => c.Id_Cliente == idCliente)
                .Select(c => new { c.Id_contacto, c.Nombre })
                .ToList();

            return Json(contactos);
        }

        public JsonResult GetProductos()
        {
            var productos = _context.Productos.Select(p => new
            {
                IdProducto = p.Id_producto,
                NombreProducto = p.Nombre,
                Descripcion = p.Descripcion,
                Unidad = p.Unidad,
                PrecioVenta = p.Precio_Venta
            }).ToList();

            return Json(productos);
        }

        // GET: CrearVenta
        [HttpGet]
        [Authorize]
        public IActionResult CrearVenta()
        {
            // Cargar los datos para los selectores
            ViewBag.listaClientes = _context.Clientes.ToList();
            ViewBag.listaContactos = _context.Contactos_cliente.ToList();
            ViewBag.listaCotizaciones = _context.Cotizaciones.ToList();
            ViewBag.listaProductos = _context.Productos.ToList();
            ViewBag.listaEmpleados = _context.Empleados.Select(e => new
            {
                Id = e.Id,
                NombreCompleto = e.Nombre + " " + e.Apellido_Paterno + " " + e.Apellido_Materno
            }).ToList();
            ViewBag.listaPromociones = _context.Promociones.ToList();
            return View();
        }

        // Metodo Post para crear una nueva venta
        [HttpPost]
        [ValidateAntiForgeryToken] //Protege contra ataques CSRF
        [Authorize]
        public IActionResult CrearVenta(Venta venta)
        {
            // Asignar la cotizacion correspondiente si se selecciono una 
            
            if (venta.Id_Cotizacion != 0)
            {
                venta.Cotizacion = _context.Cotizaciones.FirstOrDefault(c => c.IdCotizacion == venta.Id_Cotizacion);
            }

            if (venta.IdContacto != null)
            {
                // Obtener el Contacto_cliente del formulario
                venta.Contacto = _context.Contactos_cliente.FirstOrDefault(c => c.Id_contacto == venta.IdContacto);
            }
            
            //Asignar el EmpleadoId del usuario autenticado           
            venta.IdEmpleado = User.GetEmpleadoId().Value;
            
            // Obtener el Empleado del usuario autenticado
            venta.Empleado = _context.Empleados.FirstOrDefault(e => e.Id == venta.IdEmpleado);

            // Calcular el subtotal de cada DetalleVenta
            foreach (var detalle in venta.DetalleVentas)
            {
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario; // Calcular subtotal
                //venta.MontoTotal += detalle.Subtotal; // Sumar al total de la venta
                detalle.Id_venta = venta.Id_venta; // Asegurar que el Id_venta esté asignado
                detalle.Venta = venta; // Establecer la referencia a la venta
            }
            // Procesar los montos de la venta
            // Verificar si se selecciono alguna promoción
            if (venta.Id_Promocion != null)
            {
                // Obtener la promoción seleccionada             

                venta.Promocion = _context.Promociones.FirstOrDefault(p => p.Id_promocion ==  venta.Id_Promocion);
                if (venta.Promocion != null)
                {
                    venta.Descuento = venta.Promocion.Descuento;                   
                    venta.MontoDescuento = (venta.MontoTotal * venta.Descuento) / 100;
                    venta.Subtotal = venta.MontoTotal - venta.MontoDescuento;
                    venta.IVA = venta.Subtotal * 0.16m; // Calcular IVA al 16%
                    venta.TotalConIva = venta.Subtotal + venta.IVA;
                }
            }
            else
            {
                // Si no hay promoción, calcular sin descuento
                venta.Subtotal = venta.MontoTotal;
                venta.IVA = venta.Subtotal * 0.16m; // Calcular IVA al 16%
                venta.TotalConIva = venta.Subtotal + venta.IVA;
            }

            try
            {
                if (ModelState.IsValid)
                {

                    _context.Ventas.Add(venta);
                    _context.SaveChanges();

                    TempData["MensajeAlertExito"] = "Venta creada exitosamente.";
                    // Retornar la vista con la venta recién creada
                    return RedirectToAction("VistaVenta", venta);
                }
                else
                {
                    TempData["MensajeAlertFalla"] = "Error: Verifica los datos ingresados.";
                    // Recargar listas para la vista
                    ViewBag.listaClientes = _context.Clientes.ToList();
                    ViewBag.listaContactos = _context.Contactos_cliente.ToList();
                    ViewBag.listaCotizaciones = _context.Cotizaciones.ToList();
                    ViewBag.listaProductos = _context.Productos.ToList();
                    ViewBag.listaEmpleados = _context.Empleados.Select(e => new
                    {
                        Id = e.Id,
                        NombreCompleto = e.Nombre + " " + e.Apellido_Paterno + " " + e.Apellido_Materno
                    }).ToList();
                    ViewBag.listaPromociones = _context.Promociones.ToList();
                    return View(venta); // Devuelve la vista con el modelo para mostrar errores
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al crear la venta: " + ex.Message;
                Console.WriteLine("====>> Excepción interna : " + ex.InnerException );
                // Recargar listas
                ViewBag.listaClientes = _context.Clientes.ToList();
                ViewBag.listaContactos = _context.Contactos_cliente.ToList();
                ViewBag.listaCotizaciones = _context.Cotizaciones.ToList();
                ViewBag.listaProductos = _context.Productos.ToList();
                ViewBag.listaEmpleados = _context.Empleados.Select(e => new
                {
                    Id = e.Id,
                    NombreCompleto = e.Nombre + " " + e.Apellido_Paterno + " " + e.Apellido_Materno
                }).ToList();
                ViewBag.listaPromociones = _context.Promociones.ToList();
                return View(venta);
            }
        }

        // GET: VistaVenta
        [Authorize]
        public async Task<IActionResult> VistaVenta(Venta venta)
        {
            venta.Cliente = _context.Clientes.FirstOrDefault(c => c.Id_cliente == venta.Id_Cliente);
            venta.Empleado = _context.Empleados.FirstOrDefault(e => e.Id == venta.IdEmpleado);
            venta.DetalleVentas = _context.DetalleVentas
                .Where(d => d.Id_venta == venta.Id_venta)
                .Include(d => d.Producto)                
                .ToList();
            venta.Contacto = _context.Contactos_cliente.FirstOrDefault(c => c.Id_contacto == venta.IdContacto);

            if (venta == null)
            {
                return NotFound();
            }

            return View("VistaVenta", venta);
        }

        [HttpGet]
        public IActionResult VistaVentaPorId(int id)
        {
            var venta = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Cotizacion)
                .Include(v => v.Empleado)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefault(v => v.Id_venta == id);


            if (venta == null)
            {
                return NotFound();
            }
            else if (venta.DetalleVentas == null || !venta.DetalleVentas.Any())
            {
                TempData["MensajeAlertFalla"] = "La venta no tiene detalles asociados.";

            }
            else if (venta.Cliente == null)
            {
                TempData["MensajeAlertFalla"] = "La venta no tiene un cliente asociado.";

            }
            else if (venta.Empleado == null)
            {
                TempData["MensajeAlertFalla"] = "La venta no tiene un empleado asociado.";

            }
            else if (venta.Promocion == null && venta.Id_Promocion != null)
            {
                TempData["MensajeAlertFalla"] = "La venta tiene una promoción asociada que no existe.";

            }
            else
            {
                venta.Promocion = _context.Promociones.FirstOrDefault(p => p.Id_promocion == venta.Id_Promocion);
            }
           

            TempData["MensajeAlertExito"] = "Venta encontrada exitosamente.";
            venta.Contacto = _context.Contactos_cliente.FirstOrDefault(c => c.Id_contacto == venta.IdContacto);



            return View("VistaVenta", venta);
        }
        
        // GET: Lista de ventas
        [Authorize]
        public async Task<IActionResult> ListaVentas(int pagina = 1)
        {
            int tamanioPagina = 10; // registros por página
            // Crear la consulta base
            var query = _context.Ventas
                .Include(c => c.Cliente)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleVentas)
                .ThenInclude(d => d.Producto)
                .AsQueryable();

            query = query.OrderByDescending(c => c.Fecha_Venta);

            // Aplicar paginación
            var listaPaginada = await Paginacion<Venta>.CrearAsync(query, pagina, tamanioPagina);

            return View(listaPaginada);

        }

        // Metodo para buscar Ventas con filtros
        [HttpGet]
        public async Task<IActionResult> Buscar(int? id, string razonSocial,string nombreEmpleado, 
            string estatus, DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            int tamanioPagina = 10; // registros por página

            var query = _context.Ventas
                .Include(c => c.Cliente)
                .Include(c => c.Empleado)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(c => c.Id_venta == id);

            if (!string.IsNullOrEmpty(razonSocial))
                query = query.Where(c => c.Cliente.Razon_Social == razonSocial);

            if (!string.IsNullOrEmpty(nombreEmpleado))
                query = query.Where(c => c.Empleado.Nombre == nombreEmpleado);

            if (!string.IsNullOrEmpty(estatus))
                query = query.Where(c => c.Estatus == estatus);

            if (fechaInicio.HasValue)
                query = query.Where(c => c.Fecha_Venta >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(c => c.Fecha_Venta <= fechaFin.Value);

            query = query.OrderByDescending(c => c.Fecha_Venta);

            // Aplicar paginación
            var listaPaginada = await Paginacion<Venta>.CrearAsync(query, pagina, tamanioPagina);

            // Mantener los filtros seleccionados al cambiar de página
            ViewBag.Filtros = new
            {
                id,
                razonSocial,
                nombreEmpleado,
                estatus,
                fechaInicio = fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin = fechaFin?.ToString("yyyy-MM-dd")
            };

            return View("ListaVentas", listaPaginada);
        }

        // Metodo GET para editar una venta
        [HttpGet]
        [Authorize]
        public IActionResult EditarVenta(int? id)
        {
            // Verificar si el id es nulo
            if (id == null)
                return NotFound();
            // Buscar la venta por id y cargar los detalles
            var venta = _context.Ventas
                .Include(c => c.Cliente)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleVentas) // Incluir los detalles de la venta
                .ThenInclude(d => d.Producto) // Incluir los productos de cada detalle
                .FirstOrDefault(v => v.Id_venta == id);
            if (venta == null)
                return NotFound();
            // Buscar el objeto producto de cada detalleVentas en la lista de detalles

            ViewBag.listaClientes = _context.Clientes.ToList();
            ViewBag.listaContactos = _context.Contactos_cliente.ToList();
            ViewBag.listaCotizaciones = _context.Cotizaciones.ToList();
            ViewBag.listaProductos = _context.Productos.ToList();
            ViewBag.listaEmpleados = _context.Empleados.Select(e => new
            {
                Id = e.Id,
                NombreCompleto = e.Nombre + " " + e.Apellido_Paterno + " " + e.Apellido_Materno
            }).ToList();
            ViewBag.listaPromociones = _context.Promociones.ToList();
            // Retornar la vista de edición con el modelo de cotización
            return View(venta);
        }
        // Metodo POST para editar una venta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarVenta(Venta venta)
        {
            if (!ModelState.IsValid)
            {
                // Repoblar las listas
                ViewBag.listaClientes = _context.Clientes.ToList();
                ViewBag.listaContactos = _context.Contactos_cliente.ToList();
                ViewBag.listaCotizaciones = _context.Cotizaciones.ToList();
                ViewBag.listaProductos = _context.Productos.ToList();
                ViewBag.listaEmpleados = _context.Empleados.Select(e => new
                {
                    Id = e.Id,
                    NombreCompleto = e.Nombre + " " + e.Apellido_Paterno + " " + e.Apellido_Materno
                }).ToList();
                ViewBag.listaPromociones = _context.Promociones.ToList();
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensajeAlertFalla"] = "Datos inválidos. " + string.Join(" ", errores);

                return View(venta);
            }
            try
            {
                var ventaExistente = _context.Ventas // Cargar la venta con sus detalles
                    .Include(v => v.DetalleVentas)
                    .FirstOrDefault(v => v.Id_venta == venta.Id_venta);

                if (ventaExistente == null) // Verificar si la venta existe
                {
                    return NotFound();
                }

                // Actualizar los campos simples de la venta
                ventaExistente.Id_Cliente = venta.Id_Cliente;
                ventaExistente.Id_Cotizacion = venta.Id_Cotizacion;
                ventaExistente.IdEmpleado = venta.IdEmpleado;
                ventaExistente.Id_Promocion = venta.Id_Promocion;
                ventaExistente.Fecha_Venta = venta.Fecha_Venta;
                ventaExistente.Fecha_Entrega = venta.Fecha_Entrega;
                ventaExistente.Estatus = venta.Estatus;
                ventaExistente.Observaciones = venta.Observaciones;
                ventaExistente.IdContacto = venta.IdContacto;
                ventaExistente.TipoMoneda = venta.TipoMoneda;
                ventaExistente.CondicionPago = venta.CondicionPago;

                // Procesar los detalles de venta
                foreach (var detalle in venta.DetalleVentas)
                {
                    if (detalle.Eliminar)
                    {
                        var detalleExistente = ventaExistente.DetalleVentas.FirstOrDefault(d => d.IdDetalleVenta == detalle.IdDetalleVenta);
                        if (detalleExistente != null)
                            // Restar del total de la venta
                            ventaExistente.MontoTotal -= detalleExistente.Subtotal;                        

                        _context.DetalleVentas.Remove(detalleExistente);
                    }
                    else if (detalle.IdDetalleVenta == 0)
                    {
                        // nuevo detalle
                        detalle.Id_venta = venta.Id_venta; // Asegurar que el Id_venta esté asignado
                        // Calcular subtotal
                        detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                        ventaExistente.MontoTotal += detalle.Subtotal; // Sumar al total de la venta
                        detalle.Venta = ventaExistente; // Establecer la referencia a la venta existente
                        
                        _context.DetalleVentas.Add(detalle);
                    }
                    else
                    {
                        // actualizar existente
                        var existente = ventaExistente.DetalleVentas.FirstOrDefault(d => d.IdDetalleVenta == detalle.IdDetalleVenta);
                        if (existente != null)
                        {
                            existente.IdProducto = detalle.IdProducto;
                            existente.Cantidad = detalle.Cantidad;
                            existente.PrecioUnitario = detalle.PrecioUnitario;
                            existente.Descripcion = detalle.Descripcion;
                            existente.Unidad = detalle.Unidad;
                            existente.TiempoEntrega = detalle.TiempoEntrega;
                            // Recalcular subtotal
                            ventaExistente.MontoTotal -= existente.Subtotal; // Restar el subtotal antiguo
                            existente.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                            ventaExistente.MontoTotal += existente.Subtotal; // Sumar el nuevo subtotal

                        }
                    }
                }


                // Procesar los montos de la venta
                // Actualizar venta.Subtotal
                ventaExistente.Subtotal = ventaExistente.MontoTotal;

                // Verificar si se selecciono alguna promoción
                if (ventaExistente.Id_Promocion != null)
                {
                    // Obtener la promoción seleccionada   
                    ventaExistente.Promocion = _context.Promociones.FirstOrDefault(p => p.Id_promocion == ventaExistente.Id_Promocion);
                    if (ventaExistente.Promocion != null)
                    {
                        ventaExistente.Descuento = ventaExistente.Promocion.Descuento;
                        ventaExistente.MontoDescuento = (ventaExistente.MontoTotal * ventaExistente.Descuento) / 100;
                        ventaExistente.Subtotal = ventaExistente.MontoTotal - ventaExistente.MontoDescuento;
                        ventaExistente.IVA = ventaExistente.Subtotal * 0.16m; // Calcular IVA al 16%
                        ventaExistente.TotalConIva = ventaExistente.Subtotal + ventaExistente.IVA;
                    }
                }
                else
                {
                    // Si no hay promoción, calcular sin descuento
                    ventaExistente.Subtotal = ventaExistente.MontoTotal;
                    ventaExistente.IVA = ventaExistente.Subtotal * 0.16m; // Calcular IVA al 16%
                    ventaExistente.TotalConIva = ventaExistente.Subtotal + ventaExistente.IVA;
                }

                // Guardar los cambios en la base de datos
                _context.SaveChanges();
                TempData["MensajeAlertExito"] = "Venta actualizada correctamente.";
                return RedirectToAction("ListaVentas"); // Redirigir a la lista de ventas

            }
            catch (Exception ex) // Captura cualquier excepción
            {
                TempData["MensajeAlertFalla"] = "Excepción -> Error al actualizar la venta: "  + ex.Message;
                Console.WriteLine("==>> Excepción producida: " + ex.InnerException);
                return View(venta);
            }

        } // Fin de EditarVenta

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EliminarVenta(int? id)
        {
            if (id == null)
                return NotFound();

            var venta = await _context.Ventas                
                .Include(c => c.Cliente)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleVentas)
                .ThenInclude(d => d.Producto )
                .FirstOrDefaultAsync(c => c.Id_venta == id);

            if (venta == null)
                return NotFound();
            // Cargar el contacto asociado si existe
            if (venta.IdContacto != null)
            {
                venta.Contacto = _context.Contactos_cliente.FirstOrDefault(ct => ct.Id_contacto == venta.IdContacto);
            }
            else
            {
                TempData["ContactoCotizacion"] = "Sin datos.";
            }
            return View(venta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarVenta(int id)
        {
            var venta = await _context.Ventas // Carga la venta con sus detalles
                .Include(c => c.DetalleVentas)
                .FirstOrDefaultAsync(c => c.Id_venta == id);

            if (venta != null)
            {
                _context.DetalleVentas.RemoveRange(venta.DetalleVentas); // Elimina los detalles asociados a la venta
                _context.Ventas.Remove(venta);
                await _context.SaveChangesAsync();

                TempData["MensajeAlertExito"] = "Venta eliminada correctamente.";
            }
            else
            {
                TempData["MensajeAlertFalla"] = "Venta no encontrada.";
            }

            return RedirectToAction("ListaVentas");
        }

        // Generar y descargar PDF de la Orden de Venta
        public IActionResult DescargarPDF(int id)
        {
            QuestPDF.Settings.EnableDebugging = true; // Habilita el modo de depuración para QuestPDF
            var venta = _context.Ventas // Carga la venta con sus detalles
                .Include(c => c.Cliente)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefault(c => c.Id_venta == id);

            // Obtener los Contacto_cliente asociados 
            
            if (venta.IdContacto != null)
            {
                venta.Contacto = _context.Contactos_cliente.FirstOrDefault(c => c.Id_contacto == venta.IdContacto);
            }
            // Obtener la promoción asociada si existe
            if (venta.Id_Promocion != null)
            {
                venta.Promocion = _context.Promociones.FirstOrDefault(p => p.Id_promocion == venta.Id_Promocion);
            }
            if (venta == null)
                return NotFound();

            var documento = new VentaDocumento(venta, venta.Contacto, venta.Promocion); // Crea una instancia del documento de cotización

            var stream = new MemoryStream();
            documento.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Orden_Venta_{venta.Id_venta}.pdf");
        }
    }
}
