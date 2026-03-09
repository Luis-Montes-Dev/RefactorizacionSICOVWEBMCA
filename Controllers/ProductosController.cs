using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Models;
using MySqlConnector;
using NPOI.XWPF.UserModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SICOVWEB_MCA.Helpers;
using System.Net.NetworkInformation;

namespace SICOVWEB_MCA.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Mostrar Vista CrearProducto
        [HttpGet]
        [Authorize]
        public IActionResult CrearProducto()
        {
            // Cargar una lista de nombres de proveedores para el dropdown
            
            ViewBag.listaProveedores = _context.Proveedores.Select(p => new // Lista de proveedores
            {
                IdProveedor = p.IdProveedor,
                NombreProveedor = p.Razon_social
            }).ToList();
            
            return View();
        }

        //Metodo para crear un producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearProducto(Producto producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Productos.Add(producto); // Agregar el producto al contexto Db
                    _context.SaveChanges(); // Guardar los cambios en la base de datos
                    TempData["MensajeAlertExito"] = "Producto registrado exitosamente."; // Mensaje de éxito
                    return RedirectToAction("ListaProductos"); // Redirigir a la lista de productos
                }
                else
                {
                    TempData["MensajeAlertFalla"] = "Error al registrar: Verifique los datos ingresados."; // Mensaje de error
                    return View(producto); // Volver a mostrar la vista con el modelo actual
                }               
            }
            catch (Exception ex)
            {
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error interno: " + ex.Message + ex.InnerException;
                return View(producto);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListaProductos(int pagina = 1)
        {
            int tamanioPagina = 10; 
            try
            {
                // Construir consulta base
                var query = _context.Productos
                    .Include(p => p.Proveedor)
                    .AsQueryable();

                // Aplicar paginación
                var listaPaginada = await Paginacion<Producto>.CrearAsync(query, pagina, tamanioPagina);
                
                return View("ListaProductos", listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al cargar la lista de productos." + ex.Message;
                return View("ListaProductos"); // Retornar una lista vacía en caso de error
            }
            
        }

        // Metodo para buscar Productos con filtros
        [HttpGet]
        public async Task<IActionResult> Buscar(int? id, string razonSocial, string nombre, string marca, int pagina = 1)
        {
            int tamanioPagina = 10;

            var query = _context.Productos
                .Include(p => p.Proveedor) // Incluir la entidad Proveedor
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(c => c.Id_producto == id);

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(c => c.Nombre == nombre);

            if (!string.IsNullOrEmpty(razonSocial))
                query = query.Where(c => c.Proveedor.Razon_social == razonSocial);
            
            if (!string.IsNullOrEmpty(marca))
                query = query.Where(c => c.Marca == marca);

            // Aplicar paginación
            var listaPaginada = await Paginacion<Producto>.CrearAsync(query, pagina, tamanioPagina);

            // Mantener los filtros seleccionados al cambiar de página
            ViewBag.Filtros = new
            {
                id,
                razonSocial,
                nombre,
                marca,
            };

            return View("ListaProductos", listaPaginada);
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditarProducto(int id)
        {
            try
            {
                var producto = _context.Productos.Find(id);
                if (producto == null)
                {
                    TempData["MensajeAlertFalla"] = "Producto no encontrado.";
                    return RedirectToAction("ListaProductos");
                }
                // Cargar una lista de nombres de proveedores para el dropdown
                ViewBag.listaProveedores = _context.Proveedores.Select(p => new // Lista de proveedores
                {
                    IdProveedor = p.IdProveedor,
                    NombreProveedor = p.Razon_social
                }).ToList();
                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al cargar el producto." + ex.Message;
                return RedirectToAction("ListaProductos");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarProducto(Producto producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Productos.Update(producto); // Actualizar el producto en el contexto Db
                    _context.SaveChanges(); // Guardar los cambios en la base de datos
                    TempData["MensajeAlertExito"] = "Producto actualizado exitosamente."; // Mensaje de éxito
                    return RedirectToAction("ListaProductos"); // Redirigir a la lista de productos
                }
                else
                {
                    TempData["MensajeAlertFalla"] = "Error al actualizar: Verifique los datos ingresados."; // Mensaje de error
                    return View(producto); // Volver a mostrar la vista con el modelo actual
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error interno: " + ex.Message + ex.InnerException;
                return View(producto);
            }
        }

        //Metodo Get para eliminar un producto
        [HttpGet]
        [Authorize]
        public IActionResult EliminarProducto(int id)
        {
            // Cargar una lista de nombres de proveedores para el dropdown
            ViewBag.listaProveedores = _context.Proveedores.Select(p => new // Lista de proveedores
            {
                IdProveedor = p.IdProveedor,
                NombreProveedor = p.Razon_social
            }).ToList();
            try
            {
                var producto = _context.Productos.Find(id);
                if (producto == null)
                {
                    TempData["MensajeAlertFalla"] = "Producto no encontrado.";
                    return RedirectToAction("ListaProductos"); // Redirigir a la lista de productos
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al eliminar el producto.";
                return RedirectToAction("ListaProductos");
            }
        }

        //Metodo post para eliminar un producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProducto(Producto producto)
        {
            try
            {
                var prod = _context.Productos.Find(producto.Id_producto);
                if (prod == null)
                {
                    TempData["MensajeAlertFalla"] = "Producto no encontrado.";
                    return View(producto);
                }
                _context.Productos.Remove(prod); // Eliminar el producto del contexto Db
                _context.SaveChanges(); // Guardar los cambios en la base de datos
                TempData["MensajeAlertExito"] = "Producto eliminado exitosamente."; // Mensaje de éxito
                return RedirectToAction("ListaProductos"); // Redirigir a la lista de productos
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al eliminar el producto.";
                Console.WriteLine("EXCEPCIÓN ENCONTRADA"+ex.InnerException);
                return RedirectToAction("ListaProductos");
            }
        }

        // Metodo mostrar lista de productos con stock bajo

        [HttpGet]
        [Authorize]
        public IActionResult ListaProductosStockBajo()
        {
            try
            { 
                // Obtener la lista de productos con StockActual <= StockMinimo

                var listaProductosStockBajo = _context.Productos
                .Include(p => p.Proveedor) // Incluir la entidad Proveedor
                .Where(p => p.StockActual <= p.StockMinimo)
                .ToList();
                if(listaProductosStockBajo.Count == 0)
                {
                    TempData["MensajeAlertInfo"] = "No hay productos con stock bajo.";
                    return RedirectToAction("ListaProductos");
                }
                return PartialView("_ListaProductosStockBajo", listaProductosStockBajo);
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al cargar la lista de productos." + ex.Message;
                return RedirectToAction("ListaProductos");
            }

        }

        // Metodo para mostrar el detalle de un producto
        [HttpGet]
        [Authorize]
        public IActionResult InfoProducto(int id)
        {
            try
            {
                var producto = _context.Productos
                .Include(p => p.Proveedor) // Incluir la entidad Proveedor
                .Include(p => p.Movimientos_Almacen) // Incluir los movimientos de almacén
                .FirstOrDefault(p => p.Id_producto == id); // Buscar el producto por su Id
                if (producto == null)
                {
                    TempData["MensajeAlertFalla"] = "Producto no encontrado.";
                    return RedirectToAction("ListaProductos");
                }
                
                return View(producto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==>> ERROR INTERNO = " + ex.InnerException);
                TempData["MensajeAlertFalla"] = "Error al cargar el detalle del producto." + ex.Message;
                return RedirectToAction("ListaProductos");
               
            }
        }
    }
}
