using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SICOVWEB_MCA.Helpers
{
    public class Paginacion<T> : List<T>
    {
        public int PaginaActual { get; private set; } // Página actual
        public int TotalPaginas { get; private set; } // Total de páginas

        public Paginacion(List<T> items, int count, int paginaActual, int tamañoPagina) // Constructor
        {
            PaginaActual = paginaActual; // Asignar página actual
            TotalPaginas = (int)Math.Ceiling(count / (double)tamañoPagina); // Calcular total de páginas
            AddRange(items); // Agregar elementos a la lista
        }

        public bool TienePaginaAnterior => PaginaActual > 1; // Indica si hay página anterior
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas; // Indica si hay página siguiente

        public static async Task<Paginacion<T>> CrearAsync(IQueryable<T> fuente, int paginaActual, int tamañoPagina) // Método estático para crear paginación
        {
            var count = await fuente.CountAsync(); // Contar elementos en la fuente
            var items = await fuente.Skip((paginaActual - 1) * tamañoPagina).Take(tamañoPagina).ToListAsync(); // Obtener elementos para la página actual
            return new Paginacion<T>(items, count, paginaActual, tamañoPagina); // Retornar instancia de Paginacion
        }
    }
}
