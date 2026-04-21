# SICOVWEB_MCA — Guía de Agente para Refactorización

> **Sistema Integral de Cotizaciones y Ventas Web — MCA**
> Este archivo es leído automáticamente por Claude Code al iniciar cada sesión.
> Proporciona el contexto, reglas y convenciones del proyecto.

---

## 🎯 Objetivo del Proyecto

Refactorizar el sistema SICOVWEB_MCA de arquitectura MVC pura hacia **MVC + Service Layer + Repository Pattern + ViewModels**, con soporte para múltiples proveedores de base de datos (MySQL y SQL Server), iniciando por el módulo de Inventarios.

---

## 📐 Arquitectura Objetivo

### Antes (MVC puro)
```
Vista.cshtml → Controller → DbContext → Base de datos
```

### Después (MVC + Service Layer + Repository Pattern + ViewModels)
```
Vista.cshtml
    ↕  ViewModel
Controller
    ↕  IService
Service          ← Lógica de negocio
    ↕  IRepository
Repository       ← Acceso a datos
    ↕
IAppDbContext
    ↕
DbContext (MySQL / SQL Server)
    ↕
Base de datos
```

### Ejemplo concreto con Productos
```
CrearProducto.cshtml
    ↕  ProductoViewModel
ProductosController
    ↕  IProductoService
ProductoService
    ↕  IProductoRepository
ProductoRepository
    ↕  IAppDbContext
AppDbContext
    ↕
MySQL / SQL Server
```

### Responsabilidad de cada capa

| Capa | Carpeta | Responsabilidad |
|---|---|---|
| **ViewModels** | `/ViewModels/Inventarios/` | DTOs entre Vista y Controller. Nunca exponer entidades de BD directamente |
| **Controllers** | `/Controllers/Inventarios/` | Recibir petición HTTP → llamar Service → devolver ViewModel a la Vista |
| **Services (Interfaces)** | `/Services/Interfaces/` | Contratos: `IProductoService` |
| **Services (Implementaciones)** | `/Services/Implementations/` | Lógica de negocio: validaciones, transformaciones, reglas del dominio |
| **Repositories (Interfaces)** | `/Repositories/Interfaces/` | Contratos: `IProductoRepository` |
| **Repositories (Implementaciones)** | `/Repositories/Implementations/` | Solo acceso a datos con EF Core. Sin lógica de negocio |
| **Data / Abstractions** | `/Data/Abstractions/` | `IAppDbContext` independiente del proveedor |
| **Data / Configurations** | `/Data/Configurations/` | Setup de MySQL y SQL Server por entorno |

---

## 🗂️ Módulo Activo: Inventarios

**Punto de entrada:** `ProductosController`

Trabajar en este orden dentro del módulo:
1. Crear `ProductoViewModel` (y variantes: `CrearProductoViewModel`, `EditarProductoViewModel`, `ListaProductoViewModel`)
2. Crear `IProductoRepository` con los métodos de acceso a datos
3. Implementar `ProductoRepository` usando EF Core
4. Abstraer el `DbContext` con `IAppDbContext` (solo en el primer módulo)
5. Crear `IProductoService` con los métodos de negocio
6. Implementar `ProductoService` (usa `IProductoRepository`, devuelve ViewModels)
7. Registrar dependencias en `Program.cs`
8. Refactorizar `ProductosController` para inyectar `IProductoService`
9. Verificar que todas las vistas Razor reciben los ViewModels correctos sin cambios en markup

---

## ⚙️ Stack Tecnológico

| Componente | Versión / Tecnología |
|---|---|
| Framework | ASP.NET MVC con .NET 8.0 |
| Lenguaje | C# |
| ORM | Entity Framework Core 8.0.4 |
| Frontend | Razor Pages (.cshtml) + HTML + JS + CSS |
| Base de datos soportadas | MySQL, SQL Server (intercambiables) |

### Paquetes NuGet relevantes
```
Microsoft.EntityFrameworkCore 8.0.4
Pomelo.EntityFrameworkCore.MySql (para MySQL)
Microsoft.EntityFrameworkCore.SqlServer (para SQL Server)
Microsoft.EntityFrameworkCore.Design 8.0.4
```

---

## 📏 Reglas OBLIGATORIAS para Claude Code

### ⛔ Antes de hacer cualquier cambio
1. **SIEMPRE preguntar y esperar confirmación** antes de modificar, crear o eliminar archivos
2. Mostrar el plan con los archivos afectados y el diff esperado
3. Explicar el impacto en las vistas Razor existentes

### ✅ Compatibilidad con Vistas
- Las vistas Razor actuales NO deben modificarse durante la refactorización del backend
- Los `ViewBag`, `ViewData` y modelos que reciben las vistas deben mantenerse idénticos
- Si un cambio en el Controller afecta el modelo de una vista, alertar antes de proceder

### 🧪 Protocolo de Testing por cambio
1. Compilar el proyecto: `dotnet build`
2. Verificar que no hay errores de compilación
3. Si hay tests unitarios: `dotnet test`
4. Confirmar manualmente con el desarrollador que la funcionalidad es correcta
5. Solo marcar el cambio como definitivo tras la confirmación

### 🔒 Archivos protegidos (NO modificar sin aprobación explícita)
- Todas las vistas en `/Views/`
- Archivos de configuración: `appsettings.json`, `appsettings.*.json`
- Migraciones existentes en `/Migrations/`
- `Program.cs` — solo modificar en el paso de registro de dependencias y con confirmación

---

## 🏗️ Convenciones de Código C# para este proyecto

### Nombrado general

```csharp
// ViewModels: sufijo ViewModel, prefijo con acción si es específico
public class ProductoViewModel { }           // listado / detalle general
public class CrearProductoViewModel { }      // solo para formulario de creación
public class EditarProductoViewModel { }     // solo para formulario de edición

// Interfaces de Service: prefijo I + Entidad + Service
public interface IProductoService { }

// Implementaciones de Service: Entidad + Service
public class ProductoService : IProductoService { }

// Interfaces de Repository: prefijo I + Entidad + Repository
public interface IProductoRepository { }

// Implementaciones de Repository: Entidad + Repository
public class ProductoRepository : IProductoRepository { }

// Métodos asíncronos: siempre sufijo Async
Task<IEnumerable<ProductoViewModel>> GetAllAsync();
Task<ProductoViewModel?> GetByIdAsync(int id);
Task<bool> CrearAsync(CrearProductoViewModel vm);
```

---

### Capa: ViewModels

```csharp
// /ViewModels/Inventarios/ProductoViewModel.cs
// ViewModel general para mostrar un producto en vistas de listado o detalle
public class ProductoViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? Descripcion { get; set; }
    // Solo propiedades necesarias para la vista — nunca campos de auditoría de BD
}

// /ViewModels/Inventarios/CrearProductoViewModel.cs
// ViewModel específico para el formulario de creación (puede tener DataAnnotations)
public class CrearProductoViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Codigo { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    public int Stock { get; set; }
    public string? Descripcion { get; set; }
}
```

---

### Capa: Controller

```csharp
// /Controllers/Inventarios/ProductosController.cs
// El Controller SOLO orquesta: recibe HTTP → llama Service → retorna Vista
public class ProductosController : Controller
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.GetAllAsync();
        return View(productos); // IEnumerable<ProductoViewModel>
    }

    public IActionResult Crear() => View(new CrearProductoViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearProductoViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var resultado = await _productoService.CrearAsync(vm);
        if (!resultado.Exitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(vm);
        }

        TempData["Exito"] = "Producto creado correctamente";
        return RedirectToAction(nameof(Index));
    }
}
```

---

### Capa: Service

```csharp
// /Services/Interfaces/IProductoService.cs
public interface IProductoService
{
    Task<IEnumerable<ProductoViewModel>> GetAllAsync();
    Task<ProductoViewModel?> GetByIdAsync(int id);
    Task<ServiceResult> CrearAsync(CrearProductoViewModel vm);
    Task<ServiceResult> EditarAsync(int id, EditarProductoViewModel vm);
    Task<ServiceResult> EliminarAsync(int id);
}

// /Services/Implementations/ProductoService.cs
// El Service contiene TODA la lógica de negocio y hace el mapeo Entidad ↔ ViewModel
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;

    public ProductoService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<ProductoViewModel>> GetAllAsync()
    {
        var productos = await _productoRepository.GetAllAsync();
        // Mapeo manual Entidad → ViewModel (o usar AutoMapper si se decide más adelante)
        return productos.Select(p => new ProductoViewModel
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Codigo = p.Codigo,
            Precio = p.Precio,
            Stock = p.Stock,
            Descripcion = p.Descripcion
        });
    }

    public async Task<ServiceResult> CrearAsync(CrearProductoViewModel vm)
    {
        // Validaciones de negocio aquí (no en el Repository, no en el Controller)
        var existe = await _productoRepository.ExisteCodigoAsync(vm.Codigo);
        if (existe)
            return ServiceResult.Fallo("Ya existe un producto con ese código");

        var entidad = new Producto
        {
            Nombre = vm.Nombre,
            Codigo = vm.Codigo,
            Precio = vm.Precio,
            Stock = vm.Stock,
            Descripcion = vm.Descripcion
        };

        await _productoRepository.AddAsync(entidad);
        return ServiceResult.Ok("Producto creado exitosamente");
    }
}

// /Services/ServiceResult.cs — Objeto de resultado estándar para operaciones de escritura
public class ServiceResult
{
    public bool Exitoso { get; private set; }
    public string Mensaje { get; private set; } = string.Empty;

    public static ServiceResult Ok(string mensaje = "") => new() { Exitoso = true, Mensaje = mensaje };
    public static ServiceResult Fallo(string mensaje) => new() { Exitoso = false, Mensaje = mensaje };
}
```

---

### Capa: Repository

```csharp
// /Repositories/Interfaces/IProductoRepository.cs
// Solo operaciones de datos — sin lógica de negocio, sin ViewModels
public interface IProductoRepository
{
    Task<IEnumerable<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo);
    Task AddAsync(Producto producto);
    Task UpdateAsync(Producto producto);
    Task DeleteAsync(int id);
}

// /Repositories/Implementations/ProductoRepository.cs
public class ProductoRepository : IProductoRepository
{
    private readonly IAppDbContext _context;

    public ProductoRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Producto>> GetAllAsync() =>
        await _context.Productos.AsNoTracking().ToListAsync();

    public async Task<Producto?> GetByIdAsync(int id) =>
        await _context.Productos.FindAsync(id);

    public async Task<bool> ExisteCodigoAsync(string codigo) =>
        await _context.Productos.AnyAsync(p => p.Codigo == codigo);

    public async Task AddAsync(Producto producto)
    {
        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();
    }
}
```

---

### Capa: Data / Abstracción de DbContext

```csharp
// /Data/Abstractions/IAppDbContext.cs
public interface IAppDbContext
{
    DbSet<Producto> Productos { get; }
    // Agregar un DbSet por cada entidad del sistema
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// /Data/AppDbContext.cs — implementa la interfaz
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
}
```

---

### Registro de dependencias (Program.cs)

```csharp
// Selector de proveedor por configuración
var provider = builder.Configuration["DatabaseProvider"]; // "mysql" o "sqlserver"

if (provider == "mysql")
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
else
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Abstracción de BD
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Repositories
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Services
builder.Services.AddScoped<IProductoService, ProductoService>();
```

---

## 🔄 Flujo de trabajo por sesión

Cuando inicies una tarea de refactorización, sigue este orden:

```
1. EXPLORAR  → Leer los archivos afectados antes de proponer cambios
2. PLANIFICAR → Mostrar qué archivos se crean/modifican y por qué
3. CONFIRMAR → Esperar aprobación explícita del desarrollador
4. IMPLEMENTAR → Realizar los cambios acordados, un archivo a la vez
5. VERIFICAR → Compilar y reportar resultado (dotnet build)
6. CERRAR → Confirmar con el desarrollador antes de la siguiente tarea
```

---

## 📋 Checklist de Refactorización por Módulo

Usar este checklist para cada módulo (empezar con Inventarios/Productos):

**ViewModels**
- [ ] `XxxViewModel` creado en `/ViewModels/Inventarios/`
- [ ] `CrearXxxViewModel` creado (con DataAnnotations)
- [ ] `EditarXxxViewModel` creado

**Repository**
- [ ] Interfaz `IXxxRepository` creada en `/Repositories/Interfaces/`
- [ ] Implementación `XxxRepository` creada en `/Repositories/Implementations/`
- [ ] `IAppDbContext` definida en `/Data/Abstractions/` (solo primer módulo)
- [ ] `AppDbContext` implementa `IAppDbContext`

**Service**
- [ ] Interfaz `IXxxService` creada en `/Services/Interfaces/`
- [ ] Implementación `XxxService` creada en `/Services/Implementations/`
- [ ] Mapeos Entidad ↔ ViewModel dentro del Service
- [ ] `ServiceResult` creado en `/Services/` (solo primer módulo)

**Controller & Wiring**
- [ ] `XxxController` refactorizado para inyectar `IXxxService` (no Repository directamente)
- [ ] Dependencias registradas en `Program.cs`
- [ ] Vistas verificadas — reciben ViewModels correctos sin cambios en markup
- [ ] Proyecto compila sin errores (`dotnet build` ✅)

---

## 🚫 Anti-patrones a evitar

```csharp
// ❌ MAL: Controller accede directamente al Repository (saltarse el Service)
public class ProductosController : Controller
{
    private readonly IProductoRepository _repo; // NO — el Controller no debe conocer Repository
}

// ✅ BIEN: Controller solo conoce el Service
public class ProductosController : Controller
{
    private readonly IProductoService _productoService;
    public ProductosController(IProductoService productoService) => _productoService = productoService;
}

// ❌ MAL: Lógica de negocio en el Repository
public async Task<bool> CrearAsync(Producto p)
{
    if (p.Precio <= 0) return false; // NO — esto es lógica de negocio, va en el Service
    await _context.Productos.AddAsync(p);
}

// ❌ MAL: Lógica de negocio en el Controller
public async Task<IActionResult> Crear(CrearProductoViewModel vm)
{
    var existe = await _context.Productos.AnyAsync(p => p.Codigo == vm.Codigo); // NO
}

// ✅ BIEN: Toda la lógica de negocio vive en el Service
// Repository = acceso a datos únicamente
// Controller = recibir HTTP y devolver vistas únicamente

// ❌ MAL: Pasar entidades de BD directamente a la Vista
return View(producto); // donde producto es la entidad EF — expone la BD

// ✅ BIEN: Siempre usar ViewModels entre Controller y Vista
return View(new ProductoViewModel { Id = producto.Id, Nombre = producto.Nombre, ... });

// ❌ MAL: Hardcodear el proveedor de BD
// ✅ BIEN: Leer desde appsettings.json → "DatabaseProvider": "mysql"

// ❌ MAL: Instanciar DbContext manualmente
private readonly AppDbContext _context = new AppDbContext(); // NO
```

---

## 📁 Estructura de carpetas esperada al finalizar

```
SICOVWEB_MCA/
├── Controllers/
│   └── Inventarios/
│       └── ProductosController.cs           ← Refactorizado (solo IProductoService)
│
├── ViewModels/
│   └── Inventarios/
│       ├── ProductoViewModel.cs             ← NUEVO (listado / detalle)
│       ├── CrearProductoViewModel.cs        ← NUEVO (formulario crear)
│       └── EditarProductoViewModel.cs       ← NUEVO (formulario editar)
│
├── Services/
│   ├── Interfaces/
│   │   └── IProductoService.cs             ← NUEVO
│   ├── Implementations/
│   │   └── ProductoService.cs              ← NUEVO (lógica de negocio + mapeo)
│   └── ServiceResult.cs                    ← NUEVO (resultado estándar de operaciones)
│
├── Repositories/
│   ├── Interfaces/
│   │   └── IProductoRepository.cs          ← NUEVO
│   └── Implementations/
│       └── ProductoRepository.cs           ← NUEVO (solo acceso a datos EF Core)
│
├── Data/
│   ├── Abstractions/
│   │   └── IAppDbContext.cs                ← NUEVO
│   ├── AppDbContext.cs                     ← Modificado (implementa IAppDbContext)
│   └── Configurations/
│       └── DatabaseConfiguration.cs       ← NUEVO (helper MySQL / SQL Server)
│
├── Models/
│   └── Inventarios/
│       └── Producto.cs                     ← Sin cambios (entidad EF)
│
├── Views/
│   └── Productos/                          ← SIN CAMBIOS
│
└── Program.cs                              ← Modificado (registro de todas las capas)
```

### Flujo de datos completo (ejemplo Crear Producto)

```
[Usuario llena CrearProducto.cshtml]
        ↓ POST HTTP + CrearProductoViewModel
[ProductosController.Crear(vm)]
        ↓ await _productoService.CrearAsync(vm)
[ProductoService.CrearAsync(vm)]
        ├── Valida reglas de negocio (código único, precio válido, etc.)
        ├── Mapea CrearProductoViewModel → Entidad Producto
        └── await _productoRepository.AddAsync(producto)
                ↓
        [ProductoRepository.AddAsync(producto)]
                ├── _context.Productos.AddAsync(producto)
                └── _context.SaveChangesAsync()
                        ↓
                [IAppDbContext → AppDbContext → MySQL / SQL Server]
        ↑ ServiceResult.Ok() / ServiceResult.Fallo("mensaje")
[ProductosController]
        ↑ RedirectToAction("Index") o View(vm) con error
[Index.cshtml recibe IEnumerable<ProductoViewModel>]
```

---

*Última actualización: Arquitectura 5 capas (ViewModel → Controller → Service → Repository → DbContext) — Módulo activo: Inventarios*
*Equipo: MCA Development*
