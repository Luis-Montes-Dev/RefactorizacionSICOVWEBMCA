using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Models;
using System.Net.Mail;
using System.Net;
using SICOVWEB_MCA.Helpers;

namespace SICOVWEB_MCA.Controllers
{
    public class PromocionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PromocionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get mostrar vista crear promocion
        [HttpGet]
        [Authorize]
        public IActionResult CrearPromocion()
        {
            //Cargar las campañas de marketing existentes para el dropdown
            ViewBag.Campanias = _context.Campanias_Marketing.ToList();
            return View();
        }

        //Post crear promocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPromocion(Promocion promocion)
        {
            // Validar las fechas de la promocion
            if (promocion.Fecha_Inicio >= promocion.Fecha_Fin)
            {
                TempData["MensajeAlertFalla"] = "La fecha de inicio debe ser anterior a la fecha de fin.";
                ViewBag.Campanias = _context.Campanias_Marketing.ToList();// Recargar las campañas de marketing existentes para el dropdown
                return View(promocion);
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["MensajeAlertFalla"] = "Por favor, verifique los datos ingresados.";
                    ViewBag.Campanias = _context.Campanias_Marketing.ToList();// Recargar las campañas de marketing existentes para el dropdown
                    return View(promocion);
                }
                // Validar si ya existe una promocion con el mismo nombre 
                var existente = _context.Promociones.FirstOrDefault(p => p.Nombre == promocion.Nombre);
                if (existente != null) // Si ya existe una promocion con ese nombre
                {
                    //Mostrar mensaje de error en una alert en la pagina                    
                    TempData["MensajeAlertFalla"] = "Ya existe una promoción con ese Nombre.";
                    ViewBag.Campanias = _context.Campanias_Marketing.ToList();// Recargar las campañas de marketing existentes para el dropdown
                    return View(promocion);
                }

                // Guardar en base de datos
                _context.Promociones.Add(promocion);
                _context.SaveChanges();
                // Si la promocion se guardo correctamente Mandar promocion por correo a los clientes registrados
                _ = MandarPromocionAsync(promocion);

                //Mostrar mensaje de exito en una alert en la pagina
                TempData["MensajeAlertExito"] = "Promoción registrada exitosamente.";
                ViewBag.Campanias = _context.Campanias_Marketing.ToList();// Recargar las campañas de marketing existentes para el dropdown
                return View();
            }
            catch (Exception ex)
            {
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error al registrar: " + ex.Message;
                Console.WriteLine(">>>> ERROR INTERNO: " + ex.InnerException);
                ViewBag.Campanias = _context.Campanias_Marketing.ToList();// Recargar las campañas de marketing existentes para el dropdown
                return View(promocion);
            }

        }

        //GET Mostrar vista Lista de promociones
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListaPromociones(int pagina = 1)
        {
            int tamanioPagina = 10; // Número de elementos por página

            // Crear la consulta base
            var query = _context.Promociones
                .Include(p => p.Campania)
                .AsQueryable();

            // Aplicar paginación directamente sobre la consulta EF
            var listaPaginada = await Paginacion<Promocion>.CrearAsync(query, pagina, tamanioPagina);


            return View("ListaPromociones", listaPaginada);
        }

        // Metodo para buscar Promociones con filtros
        [HttpGet]
        public async Task<IActionResult> Buscar(int? id, string nombre, string estatus, 
            DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            int tamanioPagina = 10; // Número de elementos por página

            var query = _context.Promociones
                .Include(p => p.Campania)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(c => c.Id_promocion == id);

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(c => c.Nombre == nombre);

            
            if (fechaInicio.HasValue)
                query = query.Where(c => c.Fecha_Inicio >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(c => c.Fecha_Inicio <= fechaFin.Value);

            // Aplicar paginación directamente sobre la consulta EF
            var listaPaginada = await Paginacion<Promocion>.CrearAsync(query, pagina, tamanioPagina);

            // Mantener los filtros seleccionados al cambiar de página
            ViewBag.Filtros = new
            {
                id,
                nombre,
                estatus,
                fechaInicio = fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin = fechaFin?.ToString("yyyy-MM-dd")
            };

            return View("ListaPromociones", listaPaginada);
        }

        //GET Mostrar vista editar promocion
        [HttpGet]
        [Authorize]
        public IActionResult EditarPromocion(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["MensajeAlertFalla"] = "No se encontró la promoción identificador invalido.";
                return View("ListaPromociones");
            }
            var promocion = _context.Promociones.Find(id);
            if (promocion == null)
            {
                TempData["MensajeAlertFalla"] = "No se encontró la promoción.";
                return View("ListaPromociones");
            }
            if (promocion.Id_Campania != 0)
            {
                // Obtener los datos de la campaña de marketing asociada
                promocion.Campania = _context.Campanias_Marketing.FirstOrDefault(c => c.Id_Campania == promocion.Id_Campania);
            }

            return View(promocion);
        }

        //POST Editar promocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPromocion(Promocion promocion)
        {
            // Validar las fechas de la promocion
            if (promocion.Fecha_Inicio >= promocion.Fecha_Fin)
            {
                TempData["MensajeAlertFalla"] = "La fecha de inicio debe ser anterior a la fecha de fin.";
                return View(promocion);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["MensajeAlertFalla"] = "Por favor, corrija los errores en el formulario.";
                    return View(promocion);
                }
                // Validar si ya existe una promocion con el mismo nombre y diferente Id
                var existente = _context.Promociones.FirstOrDefault(p => p.Nombre == promocion.Nombre && p.Id_promocion != promocion.Id_promocion);
                if (existente != null) // Si ya existe una promocion con ese nombre
                {
                    //Mostrar mensaje de error en una alert en la pagina                    
                    TempData["MensajeAlertFalla"] = "Ya existe una promocion con ese Nombre.";
                    return View(promocion);
                }
                // Actualizar en base de datos
                _context.Promociones.Update(promocion);
                await _context.SaveChangesAsync();
                //Mostrar mensaje de exito en una alert en la pagina
                TempData["MensajeAlertExito"] = "Promoción actualizada exitosamente.";
                return RedirectToAction("ListaPromociones");
            }
            catch (Exception ex)
            {
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error al actualizar: " + ex.Message;
                Console.WriteLine(">>>> ERROR INTERNO: " + ex.InnerException);
                return View(promocion);
            }
        }

        // GET Eliminar promocion

        [HttpGet]
        [Authorize]
        public IActionResult EliminarPromocion(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["MensajeAlertFalla"] = "No se encontró la promoción identificador invalido.";
                return RedirectToAction("ListaPromociones");
            }
            var promocion = _context.Promociones.Find(id);
            if (promocion == null)
            {
                TempData["MensajeAlertFalla"] = "No se encontró la promoción.";
                return RedirectToAction("ListaPromociones");
            }
            if (promocion.Id_Campania != 0)
            {
                // Obtener los datos de la campaña de marketing asociada
                promocion.Campania = _context.Campanias_Marketing.FirstOrDefault(c => c.Id_Campania == promocion.Id_Campania);
            }
            return View(promocion);
        }

        // POST Eliminar promocion
        [HttpPost]
        [Authorize]
        public IActionResult EliminarPromocion(Promocion promocion)
        {
           
            if (promocion == null) // Si no se encuentra la promocion
            {
                TempData["MensajeAlertFalla"] = "La promoción no existe.";
                return RedirectToAction("ListaPromociones");
            }
            try
            {
                _context.Promociones.Remove(promocion);
                _context.SaveChanges();
                TempData["MensajeAlertExito"] = "Promoción eliminada exitosamente.";
                return RedirectToAction("ListaPromociones");
            }
            catch (Exception ex)
            {
                //Mostrar mensaje de error en una alert en la pagina
                TempData["MensajeAlertFalla"] = "Error al eliminar: " + ex.Message;
                Console.WriteLine(">>>> ERROR INTERNO: " + ex.InnerException);
                return RedirectToAction("ListaPromociones");
            }
        }

        //Metodo para enviar por correo la lista de promociones (vigentes) a los correos de los clientes registrados

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarPromocionesPorCorreoAsync()
        {
            
            // Obtenemos la lista de promociones vigentes
            var promocionesVigentes = _context.Promociones
                .Where(p => p.Fecha_Inicio <= DateTime.Now && p.Fecha_Fin >= DateTime.Now)
                .ToList();
            // Obtenemos la lista de correos de los clientes registrados
            var correosClientes = _context.Clientes
                .Select(c => c.Correo)
                .ToList();

            // Generación del contenido del correo.
            string asuntoCorreo = "Promociones vigentes MCA";
            string cuerpoCorreo = "Estimado cliente,\n\nLe presentamos las promociones vigentes:\n\n";
            int index = 1;
            foreach (var promo in promocionesVigentes)
            {
                string P = "Promoción # "+$"{index}" +$"- {promo.Nombre}: {promo.Descripcion} (Descuento: {promo.Descuento}%, Vigencia: {promo.Fecha_Inicio.ToShortDateString()} - {promo.Fecha_Fin.ToShortDateString()})\n";
                index++;
                cuerpoCorreo += P;
            }
            string cuerpoFinal = "\n\n¡Aproveche estas ofertas exclusivas!\n\nAtentamente,\nEquipo MCA";

            // Configurar el cliente SMTP
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("sicovwebsoporte@gmail.com", "jsbn bowq cnkn rjny"),
                EnableSsl = true,
            };
            
            // Agregar los destinatarios
            foreach (var correo in correosClientes)
            {
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("sicovwebsoporte@gmail.com");
                    mailMessage.To.Add(correo);
                    mailMessage.Subject = asuntoCorreo;
                    mailMessage.Body = cuerpoCorreo + cuerpoFinal;
                    mailMessage.IsBodyHtml = false;

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        Console.WriteLine("Correo enviado a: "+ correo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al enviar a " + correo + ": " + ex.Message);
                        Console.WriteLine("SMTP Error: " + ex.InnerException);
                        return RedirectToAction("ListaPromociones");
                    }
                }
            }
            
            TempData["MensajeAlertExito"] = "La lista de promociones vigentes ha sido enviada por correo electrónico a los clientes registrados.";
            return RedirectToAction("ListaPromociones");
        }

        //Método para enviar una promoción nueva a los correos de los clientes registrados

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MandarPromocionAsync(Promocion promocion)
        {   
            // Obtenemos la lista de correos de los clientes registrados
            var correosClientes = _context.Clientes
                .Select(c => c.Correo)
                .ToList();

            // Generación del contenido del correo.
            string asuntoCorreo = "Promoción nueva MCA";
            string cuerpoCorreo = "Estimado cliente,\n\nLe presentamos la nueva promoción:\n\n" +
                $"Promoción : {promocion.Nombre}\n" +
                $"{promocion.Descripcion}\n"+
                $"Descuento: {promocion.Descuento} %\n"+
                $"Vigencia: {promocion.Fecha_Inicio.ToShortDateString()} - {promocion.Fecha_Fin.ToShortDateString()}\n"+
                "\n\n¡Aproveche estas ofertas exclusivas!\n\nAtentamente,\nEquipo MCA";
            
            // Configurar el cliente SMTP
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("sicovwebsoporte@gmail.com", "jsbn bowq cnkn rjny"),
                EnableSsl = true,
            };

            // Agregar los destinatarios
            foreach (var correo in correosClientes)
            {
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("sicovwebsoporte@gmail.com");
                    mailMessage.To.Add(correo);
                    mailMessage.Subject = asuntoCorreo;
                    mailMessage.Body = cuerpoCorreo ;
                    mailMessage.IsBodyHtml = false;

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        Console.WriteLine("Correo enviado a: " + correo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al enviar a " + correo + ": " + ex.Message);
                        Console.WriteLine("SMTP Error: " + ex.InnerException);
                        return RedirectToAction("ListaPromociones");
                    }
                }
            }

            TempData["MensajeAlertExito"] = "Promoción enviada por correo.";
            return RedirectToAction("ListaPromociones");
        }
    }
}
