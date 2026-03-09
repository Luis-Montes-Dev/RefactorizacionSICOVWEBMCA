using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SICOVWEB_MCA.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;


namespace SICOVWEB_MCA.Controllers
{
    public class ImportadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ImportadorController(ApplicationDbContext context)
        {
            _context = context;
        }


        //Metodo para Mostrar la vista Importador Clientes de Excel
        [HttpGet]
        [Authorize]
        public IActionResult ImportadorExcel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MostrarDatos([FromForm] IFormFile ArchivoExcel)
        {
            Stream stream = ArchivoExcel.OpenReadStream();

            IWorkbook MiExcel = null;

            if (Path.GetExtension(ArchivoExcel.FileName) == ".xlsx")
            {
                MiExcel = new XSSFWorkbook(stream);
            }
            else
            {
                MiExcel = new HSSFWorkbook(stream);
            }

            ISheet HojaExcel = MiExcel.GetSheetAt(0);

            int cantidadFilas = HojaExcel.LastRowNum;

            List<Cliente> lista = new List<Cliente>();

            for (int i = 1; i <= cantidadFilas; i++)
            {
                IRow fila = HojaExcel.GetRow(i);

                if (fila == null) continue; // fila vacía, la saltamos

                lista.Add(new Cliente
                {
                    Razon_Social = GetStringValue(fila.GetCell(0)),
                    Rfc = GetStringValue(fila.GetCell(1)),
                    Correo = GetStringValue(fila.GetCell(2)),
                    Telefono = GetStringValue(fila.GetCell(3)),
                    Calle = GetStringValue(fila.GetCell(4)),
                    Numero = GetStringValue(fila.GetCell(5)),
                    Colonia = GetStringValue(fila.GetCell(6)),
                    Localidad = GetStringValue(fila.GetCell(7)),
                    Estado = GetStringValue(fila.GetCell(8)),
                    Pais = GetStringValue(fila.GetCell(9)),
                    CP = GetStringValue(fila.GetCell(10)),
                    Condicion_Pago = GetStringValue(fila.GetCell(11)),
                    Estatus = GetStringValue(fila.GetCell(12)),
                    Tipo = GetStringValue(fila.GetCell(13)),
                    Fecha_Alta = (DateTime)GetDateValue(fila.GetCell(14)),
                    Fecha_Baja = GetDateValue(fila.GetCell(15))
                });
            }
            TempData["MensajeAlertExito"] = "Archivo cargado correctamente. Verifique los datos antes de guardarlos.";
            return PartialView("_ListadoClientesExcel", lista);
        }

        public string GetStringValue(ICell cell)
        {
            return cell == null ? "" : cell.ToString().Trim();
        }

        public DateTime? GetDateValue(ICell cell)
        {
            if (cell == null) return null;
            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
            {
                return cell.DateCellValue;
            }
            return null;
        }

        [HttpPost]
        public IActionResult GuardarDatosExcel(List<Cliente> clientes)
        {
            if (clientes != null && clientes.Any())
            {
                if (!ModelState.IsValid)
                {
                    //return Json(new { success = false, message = "Datos inválidos." });
                    var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData["MensajeAlertError"] = "Datos inválidos. " + string.Join(" ", errores);
                    return Json(new { success = false, message = "Datos inválidos. " + string.Join(" ", errores) });
                    
                }
                try
                {
                    _context.Clientes.AddRange(clientes);
                    _context.SaveChanges();
                    TempData["MensajeAlertExito"] = "Datos guardados correctamente.";
                    return Json(new { success = true, message = "Datos guardados correctamente." });
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    TempData["MensajeAlertError"] = "Excepción al guardar. " + ex.Message + (ex.InnerException != null ? " Detalle: " + ex.InnerException.Message : "");
                    return Json(new { success = false, message = "Excepción al guardar ."+ ex.Message+ex.InnerException });

                    throw;
                }
                
            }
            TempData["MensajeAlertError"] = "No hay datos para guardar.";
            return Json(new { success = false, message = "No hay datos para guardar." });
        }

        // Metodo para Mostrar la vista Importador Productos de Excel
        [HttpGet]
        [Authorize]
        public IActionResult ImportadorExcelProductos()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MostrarDatosProductos([FromForm] IFormFile ArchivoExcel)
        {
            Stream stream = ArchivoExcel.OpenReadStream();

            IWorkbook MiExcel = null;

            if (Path.GetExtension(ArchivoExcel.FileName) == ".xlsx")
            {
                MiExcel = new XSSFWorkbook(stream);
            }
            else
            {
                MiExcel = new HSSFWorkbook(stream);
            }

            ISheet HojaExcel = MiExcel.GetSheetAt(0);

            int cantidadFilas = HojaExcel.LastRowNum;

            List<Producto> lista = new List<Producto>();

            for (int i = 1; i <= cantidadFilas; i++)
            {
                IRow fila = HojaExcel.GetRow(i);

                if (fila == null) continue; // fila vacía, la saltamos

                lista.Add(new Producto
                {
                    Idproveedor = int.Parse(GetStringValue(fila.GetCell(0))),
                    Nombre = GetStringValue(fila.GetCell(1)),
                    Marca = GetStringValue(fila.GetCell(2)),
                    Descripcion = GetStringValue(fila.GetCell(3)),
                    Precio_Compra = decimal.Parse(GetStringValue(fila.GetCell(4))),
                    Precio_Venta = decimal.Parse(GetStringValue(fila.GetCell(5))),
                    Unidad = GetStringValue(fila.GetCell(6)),
                    // Calcular el margen de ganancia como numero decimal
                    Margen = ((decimal.Parse(GetStringValue(fila.GetCell(5))) - decimal.Parse(GetStringValue(fila.GetCell(4)))) / decimal.Parse(GetStringValue(fila.GetCell(4)))) * 100,
                    StockActual = int.Parse(GetStringValue(fila.GetCell(8))),
                    StockMinimo = int.Parse(GetStringValue(fila.GetCell(9)))
                });
            }
            TempData["MensajeAlertExito"] = "Archivo cargado correctamente. Verifique los datos antes de guardarlos.";
            return PartialView("_ListadoProductosExcel", lista);
        }

        [HttpPost]
        public IActionResult GuardarDatosProductos(List<Producto> productos)
        {
            if (productos != null && productos.Any())
            {
                if (!ModelState.IsValid)
                {
                    //return Json(new { success = false, message = "Datos inválidos." });
                    var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData["MensajeAlertError"] = "Datos inválidos. " + string.Join(" ", errores);
                    return Json(new { success = false, message = "Datos inválidos. " + string.Join(" ", errores) });

                }
                try
                {
                    _context.Productos.AddRange(productos);
                    _context.SaveChanges();
                    TempData["MensajeAlertExito"] = "Datos guardados correctamente.";
                    return Json(new { success = true, message = "Datos guardados correctamente." });
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    TempData["MensajeAlertError"] = "Excepción al guardar. " + ex.Message + (ex.InnerException != null ? " Detalle: " + ex.InnerException.Message : "");
                    return Json(new { success = false, message = "Excepción al guardar ." + ex.Message + ex.InnerException });

                    throw;
                }

            }
            TempData["MensajeAlertError"] = "No hay datos para guardar.";
            return Json(new { success = false, message = "No hay datos para guardar." });
        }
    }
}
