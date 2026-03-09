using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICOVWEB_MCA.Helpers;
using SICOVWEB_MCA.Models;
using SICOVWEB_MCA.Models.ViewModels;
using System.Net.NetworkInformation;

namespace SICOVWEB_MCA.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;//Permiso de solo lectura al contexto DB

        public UsuariosController(ApplicationDbContext context)//Constructor usa el contexto como parametro
        {
            _context = context;
        }

        //Metodo para devolver la vista Registrar como vista parcial
        [HttpGet]
        [Authorize(Roles ="admin")]
        public IActionResult MostrarRegistrar()
        {
            return PartialView("_RegistrarUsuario"); // Vista parcial para registrar usuario
        }

        [HttpPost]
        public IActionResult Registrar(Usuario usuario) // Método para registrar un nuevo usuario
        {
            try
            {
                // Validar si ya existe el correo
                var existente = _context.Usuarios.FirstOrDefault(u => u.CorreoUsuario == usuario.CorreoUsuario);
                if (existente != null)
                {
                    TempData["MensajeAlertFalla"] = "Ya existe un usuario con ese correo.";

                    return RedirectToAction("MostrarRegistrar", usuario); // Regresa a la vista de registro si ya existe el correo
                }
                // Validar la contraseña con minimo 8 caracteres, al menos una mayúscula, una minúscula, un número y un carácter especial
                if (string.IsNullOrEmpty(usuario.Contrasena) || usuario.Contrasena.Length < 8 ||
                    !usuario.Contrasena.Any(char.IsUpper) || !usuario.Contrasena.Any(char.IsLower) ||
                    !usuario.Contrasena.Any(char.IsDigit) || !usuario.Contrasena.Any(ch => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(ch)))
                {
                    TempData["MensajeAlertFalla"] = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial.";
                    return RedirectToAction("MostrarRegistrar", usuario);
                }
                // Hashear la contraseña
                var hasher = new PasswordHasher<Usuario>();
                usuario.Contrasena = hasher.HashPassword(usuario, usuario.Contrasena);

                // Guardar en base de datos
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                TempData["MensajeAlertExito"] = "Usuario registrado exitosamente.";

                return RedirectToAction("Buscar");
            }
            catch (Exception ex)
            {
                TempData["MensajeAlertFalla"] = "Error al registrar: " + ex.Message;

                return RedirectToAction("MostrarRegistrar" , usuario);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(int? idUsuario, string apellidoPaterno, string tipo, int pagina = 1)
        {
            int tamanioPagina = 10; // Número de registros por página

            // Cargar listas para los filtros

            var query = _context.Usuarios
                .Include(u => u.Empleado)
                .AsQueryable();

            if (idUsuario.HasValue)
                query = query.Where(c => c.Id == idUsuario);

            if (!string.IsNullOrEmpty(apellidoPaterno))
                query = query.Where(c => c.Empleado.Apellido_Paterno == apellidoPaterno);

            if (!string.IsNullOrEmpty(tipo))
                query = query.Where(c => c.TipoUsuario == tipo);

           
            // Aplicar paginación
            var listaPaginada = await Paginacion<Usuario>.CrearAsync(query, pagina, tamanioPagina);
            // Mantener los filtros seleccionados al cambiar de página
            ViewBag.Filtros = new
            {
                idUsuario,
                apellidoPaterno,
                tipo,
            };

            return View("_ListaUsuarios", listaPaginada);
        }

        //Metodo Get para mostrar la vista de editar usuario
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return PartialView("_Editar", usuario);
        }

        //Metodo Post para editar un usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Enviar un mensaje a la vista con tempdata si se edita correctamente
                TempData["MensajeExito"] = "Usuario editado exitosamente.";
                return RedirectToAction("Buscar");
            }
            return PartialView("_Editar", usuario); // Si hay errores, regresa a la vista de edición
        }

        //Metodo para confirmar la eliminacion de un usuario
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return PartialView("_Eliminar" , usuario);
        }

        //Metodo para eliminar un usuario

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                
            }
            TempData["Mensaje"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Buscar");
        }

        //Metodo para buscar un usuario por correo
        [HttpPost]
        public IActionResult BuscarUsuario(string correoUsuario)
        {
            if (string.IsNullOrEmpty(correoUsuario))
            {
                TempData["MensajeAlertFalla"] = "El campo de búsqueda no puede estar vacío.";
                return PartialView("_BuscarUsuario");
            }
            var usuario = _context.Usuarios
                .Where(u => u.CorreoUsuario.Contains(correoUsuario))
                .Select(u => new UsuarioVista
                {
                    Id = u.Id,
                    EmpleadoId = u.EmpleadoId,
                    CorreoUsuario = u.CorreoUsuario,
                    TipoUsuario = u.TipoUsuario.ToString()
                }).ToList();
            if (usuario.Count == 0)
            {
                TempData["MensajeAlertFalla"] = "No se encontraron usuarios con ese correo.";
            }
            return PartialView("_ListaUsuarios", usuario);
        }
        
    }
}
