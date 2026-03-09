using System.Security.Claims;
using System.Security.Principal;

namespace SICOVWEB_MCA.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetNombre(this IPrincipal user)
        {
            return (user as ClaimsPrincipal)?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int? GetUsuarioId(this IPrincipal user)
        {
            var value = (user as ClaimsPrincipal)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out int id) ? id : null;
        }

        public static string GetRol(this IPrincipal user)
        {
            return (user as ClaimsPrincipal)?.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static int? GetEmpleadoId(this IPrincipal user)
        {
            var value = (user as ClaimsPrincipal)?.FindFirst(ClaimTypes.Sid)?.Value;
            return int.TryParse(value, out int id) ? id : null;
        }

        // Método genérico en caso de agregar más claims personalizados
        public static string GetClaimValue(this IPrincipal user, string claimType)
        {
            return (user as ClaimsPrincipal)?.FindFirst(claimType)?.Value;
        }

    }
}
