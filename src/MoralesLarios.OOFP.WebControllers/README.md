# MoralesLarios.OOFP.WebControllers — CRUD REST completo heredando de una sola clase

Este proyecto es la **última capa** de la pila FOOP para aplicaciones web. Su única responsabilidad es ofrecer **controladores base genéricos** de ASP.NET Core que ya traen implementado el CRUD HTTP completo: rutas, verbos, conversión de la clave primaria, delegación en el servicio y traducción de errores a `ProblemDetails`. Tú solo declaras la clase derivada y eliges los tipos.

La cadena completa queda así: **ruta HTTP → `WebControllers` → `WebServices` (`IGenServiceFp`) → `EFCore` (repositorio funcional) → base de datos**, y el error viaja de vuelta por el mismo raíl `MlResult<T>` hasta convertirse en un `ProblemDetails` con el código de estado correcto. Nada de `try/catch`, nada de `if (x == null) return NotFound()`.

```csharp
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }
```

Esas tres líneas publican siete endpoints funcionando.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Arquitectura: de la ruta HTTP al repositorio](#arquitectura-de-la-ruta-http-al-repositorio)
5. [`SimpleMlControllerBase<TEntity, TDto, TPk>`](#simplemlcontrollerbasetentity-tdto-tpk)
6. [`SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>`](#simplemlcontrollerbasetentity-trequest-tresponse-tpk)
7. [`SimpleMlComplexPkControllerBase<TEntity, TDto>`](#simplemlcomplexpkcontrollerbasetentity-tdto)
8. [`SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>`](#simplemlcomplexpkcontrollerbasetentity-trequest-tresponse)
9. [`PkParameterAttribute`](#pkparameterattribute)
10. [`Helpers.Extensions` — conversión de claves](#helpersextensions--conversión-de-claves)
11. [`RegisterServices`](#registerservices)
12. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
13. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
14. [Ejemplos prácticos](#ejemplos-prácticos)
15. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
16. [Mejores prácticas](#mejores-prácticas)
17. [Resumen](#resumen)
18. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

En cualquier API REST con varias entidades se repite el mismo controlador una y otra vez: siete acciones idénticas salvo por los tipos, con la misma conversión del `id` de la ruta, los mismos `if` de comprobación y los mismos códigos de estado.

**❌ Sin `WebControllers` — un controlador por entidad, todo repetido:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    public UsersController(IUserService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _svc.AllAsync();
            return Ok(users);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!int.TryParse(id, out var pk)) return BadRequest("id inválido");
        var user = await _svc.FindByIdAsync(pk);
        if (user is null) return NotFound();
        return Ok(user);
    }

    // … y lo mismo para POST, PUT (×2) y DELETE (×2).
    // Y otra vez entero para Orders, Products, Invoices…
}
```

**✅ Con `WebControllers` — una clase por entidad, cero cuerpo:**
```csharp
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }

[Route("api/[controller]")]
public class OrdersController(IGenServiceFp<Order, OrderDto> svc)
    : SimpleMlControllerBase<Order, OrderDto, Guid>(svc) { }
```

> 💡 **La idea clave:** el controlador no toma decisiones. Traduce la petición a una llamada al servicio y el `MlResult<T>` resultante a un `IActionResult`. Todas las reglas (existencia, validación, permisos) viven en el raíl, y el código de estado se decide donde nace el error, con `MlProblemsDetails.*`.

---

## Instalación y dependencias

```xml
<PackageReference Include="MoralesLarios.OOFP.WebControllers" Version="1.0.5" />
```

`TargetFramework`: **`net8.0`** · `ImplicitUsings`: habilitado · `Nullable`: habilitado.

| Referencia | Tipo | Para qué |
|---|---|---|
| `Microsoft.AspNetCore.Mvc.Core` **2.3.9** | NuGet | `ControllerBase`, `[ApiController]`, `[HttpGet]`, `IActionResult` |
| `MoralesLarios.OOFP.WebApi` | Proyecto | `MlActionResults`, `MlResultWebExtensionsPlus`, `ProblemDetailsInfo` |
| `MoralesLarios.OOFP.WebServices` | Proyecto | `IGenServiceFp<,>`, `IGenServiceFp<,,>`, `MlProblemsDetails` |

Por transitividad llegan también el núcleo `MoralesLarios.OOFP`, `Internals`, `ValueObjects` y `EFCore`.

> ⚠️ **Sobre el paquete MVC.** Igual que en `WebApi`, se referencia el paquete NuGet `Microsoft.AspNetCore.Mvc.Core` en lugar de usar `<FrameworkReference Include="Microsoft.AspNetCore.App" />`. La versión aquí (2.3.9) es más moderna que la de `WebApi` (2.1.0), pero la mezcla de ambas en el mismo grafo puede producir avisos de resolución de ensamblados. En una aplicación ASP.NET Core real el `FrameworkReference` ya está presente y todo funciona; en bibliotecas intermedias es la fuente habitual de conflictos.

**Namespaces relevantes:**
```csharp
using MoralesLarios.OOFP.WebControllers.Controllers;  // las 4 clases base
using MoralesLarios.OOFP.WebControllers.Attributes;   // PkParameterAttribute
using MoralesLarios.OOFP.WebControllers.Helpers;      // ConverterTo, GetPkValues
using MoralesLarios.OOFP.WebServices.Services;        // IGenServiceFp<,>
```

> 💡 El `GlobalUsings.cs` del proyecto ya incorpora `Microsoft.AspNetCore.Mvc`, `Microsoft.AspNetCore.Http`, `MoralesLarios.OOFP.Types`, `MoralesLarios.OOFP.ValueObjects`, `MoralesLarios.OOFP.Internals.Info`, `MoralesLarios.OOFP.WebApi.Helpers` y `MoralesLarios.OOFP.WebServices`, de modo que dentro del proyecto no hay `using` explícitos.

---

## Estructura del proyecto

```text
MoralesLarios.OOFP.WebControllers/
├── Attributes/
│   └── PkParameterAttribute.cs           // documentación del parámetro de PK compuesta
├── Controllers/
│   ├── SimpleMlControllerBase.cs         // 2 clases: PK simple, versión normal y duplex
│   └── SimpleMlComplexPkControllerBase.cs// 2 clases: PK compuesta, normal y duplex
├── Helpers/
│   └── Extensions.cs                     // ConverterTo, ConvertDateTime, GetPkValues
├── GlobalUsings.cs
└── RegisterServices.cs                   // AddWebControllers() — actualmente vacío
```

Cuatro clases base, un atributo y un puñado de utilidades de conversión. Todo el peso real está en `WebServices` y `WebApi`.

| Archivo | Clases públicas |
|---|---|
| `SimpleMlControllerBase.cs` | `SimpleMlControllerBase<TEntity, TDto, TPk>`, `SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>` |
| `SimpleMlComplexPkControllerBase.cs` | `SimpleMlComplexPkControllerBase<TEntity, TDto>`, `SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>` |

---

## Arquitectura: de la ruta HTTP al repositorio

```text
  ①  PETICIÓN
      GET /api/users/id-str/42
              │
              ▼
  ②  CONTROLADOR  (este proyecto)
      EnsureFp.NotNullAsync(id)                    ← el id no puede ser null
        .TryMapAsync(_ => id.ConverterTo(typeof(TPk)))   ← "42" → 42 (int)
              │
              ▼
  ③  SERVICIO  (MoralesLarios.OOFP.WebServices)
      _genServiceFp.FindByIdProblemsDetailsAsync(
            notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
            pk: 42)
              │                     ↑ aquí se decide que "no existe" = 404
              ▼
  ④  REPOSITORIO  (MoralesLarios.OOFP.EFCore)
      TryFindAsync(pk) → MlResult<User>
              │
              ▼
  ⑤  TRADUCCIÓN  (MoralesLarios.OOFP.WebApi)
      .ToGetPdActionResultAsync()
        · válido → 200 OK con el DTO
        · fallo  → lee el detalle "ProblemsDetails" y devuelve ese código
              │
              ▼
  ⑥  RESPUESTA
      200 OK  /  404 Not Found  /  409 Conflict  /  500 …
```

**Los tres puntos que hay que entender:**

| Paso | Quién decide | Consecuencia si falta |
|---|---|---|
| ② Conversión del `id` | El controlador base | Un `id` no convertible corta el raíl antes de tocar la base de datos |
| ③ Clasificación del error | El servicio, con `MlProblemsDetails.*` | Sin el detalle `"ProblemsDetails"` la respuesta será un **500**, aunque el error sea un "no encontrado" |
| ⑤ Traducción a HTTP | `MlResultWebExtensionsPlus` | Es puramente mecánica: no inventa códigos |

> ❗ **Regla de oro.** El controlador nunca decide el código de estado (salvo el `404` de un `id` mal formado). Si un endpoint devuelve `500` cuando esperabas `404`, el problema está en el paso ③, no aquí.

---

## `SimpleMlControllerBase<TEntity, TDto, TPk>`

El caso más común: una entidad, un DTO que sirve tanto de entrada como de salida, y una clave primaria de un solo campo.

```csharp
[ApiController]
public class SimpleMlControllerBase<TEntity, TDto, TPk>(IGenServiceFp<TEntity, TDto> _genServiceFp)
    : ControllerBase
    where TEntity : class
    where TDto    : class
```

Fíjate en que `TPk` **no tiene restricción** y **no se usa como tipo de parámetro**: sirve únicamente como destino de la conversión `string → TPk` que hace `ConverterTo`.

### Endpoints publicados

| Verbo | Plantilla | Método | Éxito | Fallo |
|---|---|---|---|---|
| `GET` | *(vacía)* | `GetAllAsync(ct)` | `200 OK` + colección de `TDto` | `ProblemDetails` |
| `GET` | `id-str/{id}` | `GetByIdAsync(id, ct)` | `200 OK` + `TDto` | `404` si el `id` no convierte; si no, el código del error |
| `POST` | *(vacía)* | `PostAsync([FromBody] dto, ct)` | `201 Created` | `ProblemDetails` |
| `PUT` | `{id}` | `PutAsync(id, [FromBody] dto, ct)` | `204 No Content` | el código del error |
| `PUT` | *(vacía)* | `PutAsync([FromBody] dto, ct)` | `204 No Content` | el código del error |
| `DELETE` | `{id}` | `DeleteAsync(id, ct)` | `204 No Content` | el código del error |
| `DELETE` | *(vacía)* | `DeleteAsync([FromBody] dto, ct)` | `204 No Content` | el código del error |

Todas las acciones son `virtual` y todas reciben un `CancellationToken ct = default!` que se propaga hasta EF Core.

> ⚠️ **Las plantillas son relativas y la base no lleva `[Route]`.** Debes poner `[Route("api/[controller]")]` (o similar) en **tu** clase derivada. Si lo omites, con `[ApiController]` las acciones no tendrán ruta válida y el arranque fallará o todas colisionarán en la raíz.

### Cómo se implementa `GetByIdAsync`

Es el método más interesante porque muestra el patrón completo:

```csharp
[HttpGet("id-str/{id}", Name = $"[controller]_[action]")]
public virtual async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!)
    => await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                     .TryMapAsync(_ => id.ConverterTo(typeof(TPk)),
                                  ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                     .MatchAsync(
                          fail      : _     => MlActionResults.NotFound(
                                                   detail: $"Path {id} not exists or is diferent type to PK of '{typeof(TPk).Name}' was not found."),
                          validAsync: idObj => _genServiceFp.FindByIdProblemsDetailsAsync(
                                                   notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
                                                   ct                  : ct,
                                                   pk                  : idObj)
                                                            .ToGetPdActionResultAsync());
```

| Paso | Qué hace |
|---|---|
| `EnsureFp.NotNullAsync` | Corta si el `id` es `null` |
| `TryMapAsync(ConverterTo)` | Convierte `"42"` en `42` capturando la excepción de parseo |
| `MatchAsync(fail: …)` | Cualquier fallo previo se traduce a **`404 Not Found`** |
| `MatchAsync(validAsync: …)` | Llama al servicio pasando ya el objeto tipado como PK |

> ⚠️ **Un `id` mal formado devuelve `404`, no `400`.** Es discutible: `"abc"` no es una PK inexistente, es una petición mal construida. Se documenta aquí porque es el comportamiento real y afecta a los clientes de tu API. Si prefieres `400`, sobreescribe `GetByIdAsync` (ver Ejemplo 4).

> ❗ **Asimetría importante:** `PutAsync(id, …)` y `DeleteAsync(id, …)` **no** tienen ese `MatchAsync`. Usan `BindAsync`, así que un `id` no convertible produce un error **sin** el detalle `"ProblemsDetails"` y termina en un **`500 Internal Server Error`**. Mismo error de entrada, dos códigos distintos según el verbo.

### `PostAsync` y el `Location`

```csharp
[HttpPost]
public virtual async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!)
    => await _genServiceFp.CreateAsync(dto, ct: ct)
                          .ToPostActionResultAsync();
```

> ❗ **Cabecera `Location` incorrecta.** Se usa la sobrecarga *sin* `Uri`, y esa sobrecarga de `MlResultWebExtensionsPlus` incrusta la constante literal `"https://www.netalpunto.net"`. Es decir: **todos** los `201 Created` de estos controladores base devuelven un `Location` que apunta al dominio del autor de la librería. Si tu API debe cumplir REST correctamente, sobreescribe `PostAsync` y usa `ToPostPdActionResult(uri)` con la URL real del recurso creado (ver Ejemplo 3).

### Sobreescritura selectiva

Todo es `virtual`, así que puedes quedarte con seis endpoints y cambiar uno:

```csharp
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc)
{
    // endpoint nuevo, además de los siete heredados
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAsync(CancellationToken ct = default)
        => await svc.AllAsync(ct: ct)
                    .MapAsync(users => users.Where(u => u.IsActive).ToList())
                    .ToGetPdActionResultAsync();
}
```

> ⚠️ **No envuelvas en `Ok(...)` lo que devuelve `base`.** Los métodos base ya devuelven un `IActionResult`; hacer `return Ok(await base.GetAllAsync(ct))` serializaría el propio `ObjectResult` y deformaría el JSON.

> 💡 El parámetro del constructor primario se llama `_genServiceFp` y es privado del tipo base: **no lo verás desde la clase derivada**. Si necesitas el servicio en tu código, captúralo en tu propio constructor primario (como `svc` en el ejemplo anterior) y pásalo al base.

---

## `SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>`

Variante *duplex*: el tipo que entra por el cuerpo (`TRequest`) es distinto del que sale (`TResponse`). Delega en `IGenServiceFp<TEntity, TRequest, TResponse>`.

```csharp
[ApiController]
public class SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>(
        IGenServiceFp<TEntity, TRequest, TResponse> _genServiceFp)
    : ControllerBase
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
```

Mismos siete endpoints, mismas plantillas y mismo comportamiento que la versión de tres parámetros. Las únicas diferencias:

- `POST`, `PUT` y `DELETE` con cuerpo reciben `TRequest`.
- `GET` devuelve `TResponse`.
- En `PutAsync(id, …)` el parámetro del servicio se llama `dtoRequest` en lugar de `dto`.

**Cuándo usarla:** cuando quieras que el cliente no pueda enviar campos calculados o de auditoría (`Id`, `CreatedAt`, `Total`…) pero sí verlos en las respuestas. Es la separación mínima entre comando y proyección de lectura.

```csharp
public record OrderRequest(int CustomerId, List<int> ProductIds);
public record OrderResponse(int Id, string CustomerName, decimal Total, DateTime CreatedAt);

[Route("api/[controller]")]
public class OrdersController(IGenServiceFp<Order, OrderRequest, OrderResponse> svc)
    : SimpleMlControllerBase<Order, OrderRequest, OrderResponse, int>(svc) { }
```

> ⚠️ Recuerda que el mapeo `TRequest → TEntity → TResponse` lo hace **Mapster** en `WebServices`. Si los nombres no coinciden, necesitas un `TypeAdapterConfig` propio; el controlador no interviene en el mapeo.

---

## `SimpleMlComplexPkControllerBase<TEntity, TDto>`

Para entidades con **clave primaria compuesta**. Aquí no hay un `TPk` único, así que el constructor primario pide una función que sepa extraer los campos de la PK de una entidad:

```csharp
[ApiController]
public class SimpleMlComplexPkControllerBase<TEntity, TDto>(
        IGenServiceFp<TEntity, TDto> _genServiceFp,
        Func<TEntity, object[]>      _pkFields)
    : ControllerBase
    where TEntity : class
    where TDto    : class
```

`_pkFields` cumple **dos** cometidos: define el **orden** de los valores en la ruta y sirve para **inferir los tipos** a los que hay que convertir cada trozo del `string`.

### Endpoints publicados

| Verbo | Plantilla | Método |
|---|---|---|
| `GET` | *(vacía)* | `GetAllAsync(ct)` |
| `GET` | `id-str/{ids}` | `GetByIdAsync([FromRoute][PkParameter] string ids, ct)` |
| `POST` | *(vacía)* | `PostAsync([FromBody] TDto dto, ct)` |
| `PUT` | `{ids}` | `PutAsync([FromRoute][PkParameter] string ids, [FromBody] TDto dto, ct)` |
| `PUT` | *(vacía)* | `PutAsync([FromBody] TDto dto, ct)` |
| `DELETE` | `{ids}` | `DeleteAsync([FromRoute][PkParameter] string ids, ct)` |
| `DELETE` | *(vacía)* | `DeleteAsync([FromBody] TDto dto, ct)` |

### El formato del parámetro `ids`

Los valores viajan **separados por comas**, en el orden de `_pkFields`:

```http
GET /api/prueba-complex/id-str/Madrid,Norte,2024-01-15T00:00:00.000
```

La conversión la hace `Extensions.GetPkValues`, que crea una instancia *sample* de `TEntity` con `Activator.CreateInstance<TEntity>()`, llama a `_pkFields(sample)` para ver qué tipo tiene cada posición y aplica `Convert.ChangeType` con `CultureInfo.InvariantCulture`.

**Tipos que funcionan de verdad:**

| Tipo de la PK | ¿Funciona? | Notas |
|---|---|---|
| `int`, `long`, `short`, `byte`, `decimal`, `double`, `float` | ✅ | El *sample* devuelve `0`, se infiere el tipo correctamente |
| `bool` | ✅ | `true` / `false` |
| `DateTime` | ✅ | Formato invariante, ISO 8601 recomendado |
| `char` | ✅ | Un solo carácter |
| `string` | ✅ | El *sample* devuelve `null` y `GetPkValues` cae al `typeof(string)` por defecto |
| `Guid` | ❌ | `Guid` no implementa `IConvertible` → `Convert.ChangeType` lanza `InvalidCastException` |
| `DateOnly`, `TimeOnly` | ❌ | Tampoco son `IConvertible` |
| `enum` | ❌ | El *sample* devuelve el valor `0`, y `ChangeType` al tipo enum falla |
| `int?`, `DateTime?` (nullables) | ❌ | El *sample* devuelve `null` → se tratan como `string` y llegan al repositorio con el tipo equivocado |

> ❗ **Los valores no pueden contener comas.** El `split` es literal: si una PK de tipo `string` contiene una coma (`"Madrid, España"`), la ruta se parte mal y obtendrás un error de "número de valores no coincide".

> ❗ **`TEntity` necesita constructor público sin parámetros.** `Activator.CreateInstance<TEntity>()` lo exige. Las entidades EF Core lo tienen casi siempre, pero un `record` posicional sin constructor vacío hará fallar **todos** los endpoints con `{ids}`.

### Comportamiento ante un `ids` inválido

Igual que en la versión de PK simple, con la misma asimetría:

| Endpoint | Si `ids` no se puede convertir |
|---|---|
| `GET id-str/{ids}` | `404 Not Found` con *"Row with ids: … not found."* |
| `PUT {ids}` | **`500`** (el error no lleva `"ProblemsDetails"`) |
| `DELETE {ids}` | **`500`** (idem) |

### Ejemplo de derivación

```csharp
[Route("api/[controller]")]
public class PruebaComplexController(IGenServiceFp<PruebaComplex, PruebaComplexDto> svc)
    : SimpleMlComplexPkControllerBase<PruebaComplex, PruebaComplexDto>(
          svc,
          e => new object[] { e.Ciudad, e.Zona, e.Fecha }) { }
```

El orden `{ Ciudad, Zona, Fecha }` es el **contrato de la URL**: cambiarlo rompe a todos los clientes.

> 💡 El orden debe coincidir además con el orden de la clave compuesta declarada en tu `IEntityTypeConfiguration<TEntity>` (`HasKey(e => new { e.Ciudad, e.Zona, e.Fecha })`), porque el array se pasa tal cual a `DbSet.FindAsync`.

---

## `SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>`

Combinación de las dos variantes anteriores: PK compuesta **y** modelos de entrada/salida distintos.

```csharp
[ApiController]
public class SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>(
        IGenServiceFp<TEntity, TRequest, TResponse> _genServiceFp,
        Func<TEntity, object[]>                     _pkFields)
    : ControllerBase
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
```

Los siete endpoints, el formato de `ids`, las limitaciones de tipos y la asimetría de códigos de estado son **exactamente los mismos** que en la versión con `TDto`. Lo único que cambia es que el cuerpo se recibe como `TRequest` y la respuesta se serializa como `TResponse`.

```csharp
[Route("api/[controller]")]
public class TarifasController(IGenServiceFp<Tarifa, TarifaRequest, TarifaResponse> svc)
    : SimpleMlComplexPkControllerBase<Tarifa, TarifaRequest, TarifaResponse>(
          svc,
          t => new object[] { t.ZonaId, t.Temporada }) { }
```

---

## `PkParameterAttribute`

Atributo puramente **documental** para los parámetros de PK compuesta.

```csharp
[AttributeUsage(AttributeTargets.Parameter)]
public class PkParameterAttribute : Attribute
{
    public string Description { get; set; }

    public PkParameterAttribute(string description = null!);
}
```

Si no pasas descripción, se usa este texto por defecto:

> *"Valores de la clave primaria separados por comas. Para DateTime usa formato ISO 8601: yyyy-MM-ddTHH:mm:ss.fff (Ejemplo: '1,2' para PKs compuestas o '2026-05-16T07:34:29.239' para DateTime)"*

Los controladores de PK compuesta ya lo aplican a sus parámetros `ids`, y puedes usarlo también en tus propios endpoints:

```csharp
[HttpGet("por-clave/{ids}")]
public Task<IActionResult> PorClave(
    [FromRoute]
    [PkParameter("PK compuesta en orden Ciudad,Zona,Fecha. Ej: Madrid,Norte,2024-01-15T00:00:00.000")]
    string ids,
    CancellationToken ct = default) => /* … */;
```

> ❗ **El atributo no hace nada por sí solo.** No hereda de `DescriptionAttribute` ni implementa ninguna interfaz que Swashbuckle o NSwag lean automáticamente. Para que aparezca en tu OpenAPI necesitas un `IOperationFilter` propio que lo busque en los `ParameterDescriptor`. Ese filtro **no viene incluido** en el proyecto: hay un ejemplo en la sección de ejemplos prácticos.

---

## `Helpers.Extensions` — conversión de claves

Tres utilidades públicas, más una privada. Son la maquinaria que convierte los trozos de la URL en valores tipados.

```csharp
public static object   ConverterTo(this string value, Type property);
public static object[] GetPkValues<TEntity>(this string ids, Func<TEntity, object[]> pkFields) where TEntity : class;
public static object[] GetPkValues<TEntity>(string[] values, Func<TEntity, object[]> pkFields) where TEntity : class;
public static string   GetPkValuesErrorMessage(this Exception ex, string ids);
```

### `ConverterTo`

Convierte un `string` al tipo indicado usando un `switch` sobre `Type.FullName`.

| Grupo | Tipos soportados |
|---|---|
| Enteros | `Int16`, `Int32`, `Int64`, `SByte`, `Byte`, `UInt16`, `UInt32`, `UInt64` |
| Decimales | `Single`, `Double`, `Decimal` |
| Otros | `Boolean`, `Char`, `String`, `DateTime` |
| Nullables | Los equivalentes `Nullable<T>` de todos los anteriores |

Comportamiento:

- `value == null` → devuelve `null`.
- Tipo no contemplado (`Guid`, `DateOnly`, `TimeOnly`, `enum`…) → `FormatException` con el mensaje *"The type {0} is not soported"* (sí, con la errata).
- Cadena no parseable → la excepción del `Parse` correspondiente (`FormatException`, `OverflowException`…).

En los controladores siempre se invoca dentro de un `TryMapAsync`, por lo que la excepción nunca escapa: se convierte en un `MlResult` fallido.

**El caso `DateTime` merece atención.** Se resuelve con un helper privado, `ConvertDateTime`, que:

1. Intenta `DateTime.TryParse(value, out result)` — **con la cultura del hilo actual**.
2. Si falla, prueba con una lista fija de formatos: `M/d/yyyy h:mm:ss tt`, `MM/dd/yyyy hh:mm:ss`, `yyyyMMdd`, `MMddyyyy`, `dd/MM/yyyy`, `ddMMyyyy`, entre otros.
3. Si nada encaja, lanza `FormatException`.

> ❗ **Dependencia de la cultura del servidor.** Como el primer intento usa la cultura del hilo, la misma URL puede interpretarse de forma distinta según la configuración regional del proceso: `03/04/2024` es el 3 de abril en España y el 4 de marzo en EE. UU. Para evitar ambigüedades **envía siempre las fechas en ISO 8601** (`2024-04-03T00:00:00.000`), que se interpreta igual en cualquier cultura.

> ⚠️ **Nullables y cadena vacía.** En la rama `System.Nullable` de `ConverterTo`, todos los tipos comprueban `string.IsNullOrEmpty(value)` antes de parsear… **salvo `DateTime?`**, que llama directamente a `ConvertDateTime(value)`. Un `DateTime?` con cadena vacía lanza `FormatException` en lugar de devolver `null`.

### `GetPkValues`

```csharp
// 1) Sobrecarga de extensión: parte el string por comas y delega en la segunda.
public static object[] GetPkValues<TEntity>(this string ids, Func<TEntity, object[]> pkFields)
    => GetPkValues(ids.Split(','), pkFields);

// 2) Motor real.
public static object[] GetPkValues<TEntity>(string[] values, Func<TEntity, object[]> pkFields)
{
    var sample  = Activator.CreateInstance<TEntity>();
    var sampleP = pkFields(sample);

    if (values.Length != sampleP.Length)
        throw new ArgumentException($"The number of provided values ({values.Length}) does not match " +
                                    $"the number of primary key fields ({sampleP.Length}).", nameof(values));

    var result = new object[values.Length];
    for (int i = 0; i < values.Length; i++)
    {
        var t = sampleP[i]?.GetType() ?? typeof(string);
        result[i] = Convert.ChangeType(values[i], t, CultureInfo.InvariantCulture);
    }
    return result;
}
```

Ventaja frente a `ConverterTo`: usa `CultureInfo.InvariantCulture`, así que **no** depende de la cultura del servidor. Desventaja: solo admite tipos `IConvertible`, lo que descarta `Guid`, `DateOnly`, `TimeOnly` y los enums.

### `GetPkValuesErrorMessage`

```csharp
public static string GetPkValuesErrorMessage(this Exception ex, string ids)
    => $"{ids} not be extract values of pkFields. Ids string array not converted to pkFields. ex: {ex.Message}";
```

> ⚠️ El mensaje está en inglés defectuoso (*"not be extract"*) y **se expone al cliente** en el `detail` del `ProblemDetails`. Si tu API es pública, sobreescribe los endpoints o traduce el error con `MapIfFail` antes de responder.

---

## `RegisterServices`

```csharp
public static class RegisterServices
{
    public static IServiceCollection AddWebControllers(this IServiceCollection services)
    {
        return services;   // ← no registra nada
    }
}
```

> ❗ **`AddWebControllers()` es un *no-op***: existe el método pero el cuerpo está vacío. Llamarlo no rompe nada, pero tampoco hace nada. **No lo necesitas.** Lo que sí debes registrar es:

```csharp
// 1) DbContext
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));

// 2) Repositorios funcionales (MoralesLarios.OOFP.EFCore)
builder.Services.AddScopedOOFPRepos<User, AppDbContext>();

// 3) Servicios genéricos (MoralesLarios.OOFP.WebServices)
builder.Services.AddScopedtGenServicesFpWithoutReposGeneral();   // ⚠️ la errata "Scopedt" es real

// 4) MVC
builder.Services.AddControllers();
```

Con eso, cualquier controlador que herede de las clases base ya resuelve su `IGenServiceFp<,>` por inyección.

> ⚠️ Para las variantes de **PK compuesta** hay un requisito extra: el segundo parámetro del constructor, `Func<TEntity, object[]> _pkFields`, **no se puede resolver por DI**. Debes proporcionarlo tú en el constructor primario de la clase derivada (como en los ejemplos), o registrar explícitamente ese delegado en el contenedor.

---

## ⚠️ Particularidades reales del código fuente

1. **Las clases base no son `abstract`.** Son genéricas abiertas, así que ASP.NET Core no las descubre como controladores (`ContainsGenericParameters` las descarta) y en la práctica no publican rutas por sí mismas. Aun así, declararlas `abstract` sería lo correcto: nada impide hoy instanciarlas directamente.

2. **`[ApiController]` está en la base, `[Route]` no.** Debes añadir `[Route("api/[controller]")]` en cada clase derivada. Sin él, las siete acciones quedan con plantillas relativas sin prefijo.

3. **`Name = $"[controller]_[action]"` usa interpolación innecesaria.** No hay ninguna expresión dentro de las llaves… porque no hay llaves: son *tokens* de enrutado de ASP.NET Core que se sustituyen en tiempo de arranque. El `$` es superfluo pero inofensivo.

4. **Ruta `id-str/{id}` en lugar de `{id}`.** El `GET` por clave **no** está en `/api/users/42` sino en `/api/users/id-str/42`, mientras que `PUT` y `DELETE` sí usan `/api/users/42`. Es una incoherencia del diseño REST que conviene conocer: el `GET` no comparte plantilla con el resto de verbos.

5. **`POST` devuelve un `Location` literal ajeno.** `ToPostActionResultAsync()` sin `Uri` inserta `"https://www.netalpunto.net"` como cabecera `Location` del `201 Created`.

6. **Códigos de estado asimétricos para el mismo error.** Un `id`/`ids` no convertible produce `404` en `GET` y `500` en `PUT`/`DELETE`, porque solo el `GET` traduce el fallo con `MatchAsync` + `MlActionResults.NotFound`.

7. **`404` en lugar de `400` para entrada mal formada.** Semánticamente un `id` no parseable es un error del cliente (`400`), no un recurso ausente.

8. **`DELETE` con cuerpo (`[FromBody]`).** Las sobrecargas `DeleteAsync(TDto dto, …)` y `PutAsync(TDto dto, …)` sin ruta reciben el DTO por el cuerpo. Muchos proxies, CDNs y clientes HTTP descartan el cuerpo de un `DELETE`; úsalas con precaución o prefiere las variantes con `{id}`.

9. **`GetAllAsync` no pagina.** Llama a `_genServiceFp.AllAsync(ct)`, que trae la tabla completa. Para paginar necesitas un endpoint propio con `PaginationInfo` y las cabeceras de `MlRequestWebExtensions`.

10. **Ningún `[ProducesResponseType]`.** Como todas las acciones devuelven `Task<IActionResult>`, Swagger no puede inferir ni el tipo de éxito ni los códigos posibles: tu OpenAPI saldrá vacío de esquemas salvo que los declares en la clase derivada.

11. **`PkParameterAttribute` no lo lee nadie.** No hay `IOperationFilter` ni `ISchemaFilter` incluido; el atributo es inerte hasta que escribas el tuyo.

12. **`ConvertDateTime` depende de la cultura del hilo** en su primer intento (`DateTime.TryParse` sin `IFormatProvider`), mientras que `GetPkValues` usa `InvariantCulture`. Dos rutas de conversión con reglas distintas dentro del mismo proyecto.

13. **`GetPkValues` exige `IConvertible` y un constructor sin parámetros.** `Guid`, `DateOnly`, `TimeOnly`, los enums y los nullables de tipos valor no funcionan como parte de una PK compuesta.

14. **`_pkFields` no es resoluble por DI.** El constructor primario de las variantes de PK compuesta mezcla un servicio inyectable con un delegado que debes aportar a mano.

15. **Los mensajes de error están en inglés defectuoso** (*"isn't null"*, *"can't be converted"*, *"not be extract values of pkFields"*, *"is diferent type"*, *"not soported"*) y viajan al cliente dentro del `detail`.

16. **Código muerto.** `SimpleMlComplexPkControllerBase.cs` conserva unas veinte líneas comentadas con una versión anterior de `GetPkValues` y sus helpers. `RegisterServices.AddWebControllers` está vacío.

17. **`PkParameterAttribute.cs` está guardado con una codificación que no es UTF-8.** Sus comentarios XML muestran caracteres corruptos (`parmetros`, `Descripcin`), lo que ensucia el IntelliSense y la documentación generada.

18. **`Description` es `string` no anulable pero se inicializa desde un parámetro `null!`.** Funciona porque hay un valor por defecto, pero el `null!` desactiva la comprobación del compilador.

---

## ⚠️ Lo que NO incluye

- ❌ **Paginación** en `GetAllAsync` → usa `PaginationInfo` de [`Internals`](../MoralesLarios.OOFP.Internals/README.md) y las cabeceras de [`WebApi`](../MoralesLarios.OOFP.WebApi/README.md).
- ❌ **Filtrado, ordenación o búsqueda** → añade endpoints propios llamando al repositorio de [`EFCore`](../MoralesLarios.OOFP.EFCore/README.md).
- ❌ **Autenticación ni autorización** → aplica `[Authorize]` en tus clases derivadas.
- ❌ **Caché HTTP e invalidación** → eso lo cubre `MoralesLarios.OOFP.WebControllers.Cache`.
- ❌ **Validación del DTO más allá de `NotNull`** → intégrala con [`Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) o [`Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md).
- ❌ **Versionado de API, HATEOAS, `ETag` ni `If-Match`.**
- ❌ **Documentación OpenAPI automática** → declara `[ProducesResponseType]` tú mismo.
- ❌ **Registro de dependencias** → `AddWebControllers()` está vacío; registra `EFCore` y `WebServices`.
- ❌ **Endpoints por lotes** (`PATCH` parcial, `POST` masivo, borrado múltiple).

---

## Ejemplos prácticos

### Ejemplo 1 — API CRUD completa en un archivo

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));
builder.Services.AddScopedOOFPRepos<User, AppDbContext>();
builder.Services.AddScopedtGenServicesFpWithoutReposGeneral();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();


// Controllers/UsersController.cs
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }
```

Endpoints resultantes:

```text
GET    /api/users
GET    /api/users/id-str/42
POST   /api/users
PUT    /api/users/42
PUT    /api/users
DELETE /api/users/42
DELETE /api/users
```

### Ejemplo 2 — documentar la API para Swagger

Los métodos base no declaran tipos de respuesta. Redeclara las acciones que te importen solo para anotarlas:

```csharp
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),       StatusCodes.Status500InternalServerError)]
    public override Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
        => base.GetAllAsync(ct);

    [HttpGet("id-str/{id}")]
    [ProducesResponseType(typeof(UserDto),        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public override Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!)
        => base.GetByIdAsync(id, ct);
}
```

> ⚠️ Al sobreescribir con `[HttpGet(...)]` estás **redeclarando** la ruta: repite la plantilla exactamente igual que en la base (`"id-str/{id}"`) o cambiarás el contrato sin darte cuenta.

### Ejemplo 3 — arreglar la cabecera `Location` del `201 Created`

```csharp
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc)
{
    [HttpPost]
    public override async Task<IActionResult> PostAsync([FromBody] UserDto dto,
                                                        CancellationToken ct = default!)
        => await svc.CreateAsync(dto, ct: ct)
                    .BindAsync(created =>
                         EnsureFp.NotNull(created, "El servicio no devolvió la entidad creada"))
                    .MatchAsync(
                         fail      : errors  => errors.GetProblemDetails()
                                                      .Match(valid: pd => pd.ToMlActionResult(),
                                                             fail : _  => MlActionResults.InternalServerError()),
                         validAsync: created => Task.FromResult<IActionResult>(
                                                    Created($"/api/users/id-str/{created.Id}", created)));
}
```

Ahora el `Location` apunta al recurso real de tu API y no al dominio literal de la librería.

### Ejemplo 4 — devolver `400` en lugar de `404` cuando el `id` es inválido

```csharp
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc)
{
    [HttpGet("id-str/{id}")]
    public override async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!)
    {
        if (!int.TryParse(id, out var pk))
            return MlActionResults.BadRequest(
                       detail: $"El identificador '{id}' no es un entero válido.");

        return await svc.FindByIdProblemsDetailsAsync(
                            notFoundErrorDetails: MlProblemsDetails.NotFoundError(
                                                      detail: $"No existe el usuario {pk}."),
                            ct: ct,
                            pk: pk)
                        .ToGetPdActionResultAsync();
    }
}
```

> 💡 Aquí sí hay un `if`, y está bien: es una comprobación de **forma del protocolo**, no una regla de negocio. La frontera HTTP es el único sitio donde el `if` está justificado.

### Ejemplo 5 — homogeneizar el `500` de `PUT`/`DELETE` a `400`

El problema es que el fallo de conversión no lleva el detalle `"ProblemsDetails"`. Basta con reclasificarlo:

```csharp
[HttpDelete("{id}")]
public override async Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default!)
    => await EnsureFp.NotNullAsync(id, "El identificador es obligatorio")
                     .TryMapAsync(_  => id.ConverterTo(typeof(int)),
                                  ex => $"'{id}' no es un identificador válido: {ex.Message}")
                     .MapIfFailAsync(errors => MlProblemsDetails.BadRequestError(
                                                   detail: errors.ToErrorsDescription()))
                     .BindAsync(pk => svc.DeleteProblemDetailsAsync(
                                            notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
                                            ct: ct, pk: pk))
                     .ToDeletePdActionResultAsync();
```

### Ejemplo 6 — PK compuesta con tres campos

```csharp
// Entidad
public class Tarifa
{
    public string   Zona      { get; set; } = default!;
    public string   Temporada { get; set; } = default!;
    public DateTime Vigencia  { get; set; }
    public decimal  Precio    { get; set; }
}

// Configuración EF Core: el orden de HasKey debe coincidir con el de _pkFields
public class TarifaConfig : IEntityTypeConfiguration<Tarifa>
{
    public void Configure(EntityTypeBuilder<Tarifa> b)
        => b.HasKey(t => new { t.Zona, t.Temporada, t.Vigencia });
}

// Controlador
[Route("api/[controller]")]
public class TarifasController(IGenServiceFp<Tarifa, TarifaDto> svc)
    : SimpleMlComplexPkControllerBase<Tarifa, TarifaDto>(
          svc,
          t => new object[] { t.Zona, t.Temporada, t.Vigencia }) { }
```

Petición:

```http
GET /api/tarifas/id-str/Norte,Verano,2024-06-01T00:00:00.000
```

### Ejemplo 7 — PK con `Guid`: cómo sortear la limitación

`Guid` no es `IConvertible`, así que la PK compuesta no lo admite. Con PK simple sí puedes sobreescribir la conversión:

```csharp
[Route("api/[controller]")]
public class DocumentsController(IGenServiceFp<Document, DocumentDto> svc)
    : SimpleMlControllerBase<Document, DocumentDto, Guid>(svc)
{
    [HttpGet("id-str/{id}")]
    public override async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!)
        => await EnsureFp.NotNullAsync(id, "El identificador es obligatorio")
                         .TryMapAsync(_  => Guid.Parse(id),
                                      ex => $"'{id}' no es un GUID válido: {ex.Message}")
                         .MapIfFailAsync(errors => MlProblemsDetails.BadRequestError(
                                                       detail: errors.ToErrorsDescription()))
                         .BindAsync(pk => svc.FindByIdProblemsDetailsAsync(
                                                 notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
                                                 ct: ct, pk: pk))
                         .ToGetPdActionResultAsync();
}
```

> 💡 Para PK compuestas con `Guid` la única salida limpia es no usar `SimpleMlComplexPkControllerBase` y escribir el endpoint a mano, o registrar tu propio `Func<TEntity, object[]>` combinado con una conversión previa.

### Ejemplo 8 — hacer visible `PkParameterAttribute` en Swagger

El atributo es inerte; este filtro lo activa:

```csharp
public class PkParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var p in context.ApiDescription.ParameterDescriptions)
        {
            var attr = (p.ParameterDescriptor as ControllerParameterDescriptor)
                          ?.ParameterInfo
                           .GetCustomAttribute<PkParameterAttribute>();

            if (attr is null) continue;

            var target = operation.Parameters.FirstOrDefault(op => op.Name == p.Name);
            if (target is not null) target.Description = attr.Description;
        }
    }
}

// Registro
builder.Services.AddSwaggerGen(o => o.OperationFilter<PkParameterOperationFilter>());
```

### Ejemplo 9 — añadir paginación sin perder el CRUD heredado

```csharp
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc,
                             IEFRepoReaderPaginationFp<User> repoPag)
    : SimpleMlControllerBase<User, UserDto, int>(svc)
{
    /// <summary>Lee X-Page-Number y X-Page-Size de las cabeceras.</summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PaginationResultInfo<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedAsync(CancellationToken ct = default)
        => await Request.GetHeaderPaginationInfoAsync()
                        .MapIfFailAsync(errors => MlProblemsDetails.BadRequestError(
                                                      detail: errors.ToErrorsDescription()))
                        .BindAsync(pag => repoPag.TryGetInternalDataAsync(pag, ct: ct))
                        .ToGetPdActionResultAsync();
}
```

### Ejemplo 10 — combinarlo con la caché de salida

```csharp
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc, IOutputCacheStore store)
    : SimpleMlCacheControllerBase<User, UserDto, int>(svc, store) { }
```

`SimpleMlCacheControllerBase` vive en el proyecto **`MoralesLarios.OOFP.WebControllers.Cache`** y añade caché HTTP con invalidación automática en las escrituras.

### Ejemplo 11 — errores frecuentes (❌ / ✅)

**Olvidar el `[Route]` en la clase derivada:**
```csharp
// ❌ Sin [Route], las siete acciones quedan sin prefijo y colisionan entre controladores
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }

// ✅
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }
```

**Envolver el resultado del `base` en `Ok(...)`:**
```csharp
// ❌ base ya devuelve un IActionResult: esto serializa el propio ObjectResult
public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
    => Ok(await base.GetAllAsync(ct));

// ✅
public override Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
    => base.GetAllAsync(ct);
```

**Usar un `Guid` como PK compuesta:**
```csharp
// ❌ Convert.ChangeType lanza InvalidCastException: Guid no es IConvertible
: SimpleMlComplexPkControllerBase<Doc, DocDto>(svc, d => new object[] { d.TenantId /* Guid */, d.Codigo })

// ✅ usa tipos IConvertible en la PK compuesta, o escribe el endpoint a mano
: SimpleMlComplexPkControllerBase<Doc, DocDto>(svc, d => new object[] { d.TenantCodigo /* string */, d.Codigo })
```

**Registrar `AddWebControllers()` esperando que haga algo:**
```csharp
// ❌ el método existe pero su cuerpo está vacío: no registra nada
builder.Services.AddWebControllers();

// ✅ registra lo que de verdad hace falta
builder.Services.AddScopedOOFPRepos<User, AppDbContext>();
builder.Services.AddScopedtGenServicesFpWithoutReposGeneral();
builder.Services.AddControllers();
```

**Esperar `404` en un `PUT` con `id` mal formado:**
```csharp
// ❌ Suposición incorrecta: PUT /api/users/abc devuelve 500, no 404
// (el fallo de conversión no lleva el detalle "ProblemsDetails")

// ✅ reclasifica el error explícitamente (ver Ejemplo 5)
.MapIfFailAsync(errors => MlProblemsDetails.BadRequestError(detail: errors.ToErrorsDescription()))
```

**Enviar fechas dependientes de la cultura:**
```csharp
// ❌ ambiguo: 3 de abril o 4 de marzo según la cultura del servidor
GET /api/tarifas/id-str/Norte,03/04/2024

// ✅ ISO 8601, siempre
GET /api/tarifas/id-str/Norte,2024-04-03T00:00:00.000
```

**Confiar en que el DTO se valida solo:**
```csharp
// ❌ los controladores base solo comprueban NotNull sobre el DTO
[HttpPost] // hereda PostAsync: un DTO con Email = "" llega a la base de datos

// ✅ valida en el servicio o antes de llamarlo
public override Task<IActionResult> PostAsync([FromBody] UserDto dto, CancellationToken ct = default!)
    => dto.ValidateObject()
          .BindAsync(d => svc.CreateAsync(d, ct: ct))
          .ToPostPdActionResultAsync(new Uri("/api/users", UriKind.Relative));
```

---

## Tabla de decisión rápida

**¿Qué clase base necesito?**

| Situación | Clase base |
|---|---|
| PK de un campo, un solo DTO | `SimpleMlControllerBase<TEntity, TDto, TPk>` |
| PK de un campo, DTO de entrada ≠ DTO de salida | `SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>` |
| PK compuesta, un solo DTO | `SimpleMlComplexPkControllerBase<TEntity, TDto>` |
| PK compuesta, entrada ≠ salida | `SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>` |
| Necesito caché HTTP con invalidación | `SimpleMlCacheControllerBase<,,>` (`WebControllers.Cache`) |
| PK de tipo `Guid`, `DateOnly`, `TimeOnly` o `enum` | Base de PK simple **sobreescribiendo** la conversión, o endpoint a mano |
| Nada encaja | `ControllerBase` + `IGenServiceFp<,>` + `MlResultWebExtensionsPlus` |

**¿Qué hago ante un requisito concreto?**

| Quiero… | Cómo |
|---|---|
| Prefijo de ruta (`/api/users`) | `[Route("api/[controller]")]` en la clase derivada (**obligatorio**) |
| Añadir un endpoint sin perder el CRUD | Método nuevo en la derivada con su propio `[HttpGet(...)]` |
| Cambiar un endpoint heredado | `public override` sobre el método `virtual` **repitiendo su atributo de ruta** |
| Quitar un endpoint heredado | Sobreescribirlo y devolver `MlActionResults.MethodNotAllowed()` (o no usar estas bases) |
| Un `Location` correcto en el `201` | Sobreescribir `PostAsync` y usar `ToPostPdActionResult(uri)` |
| `400` en vez de `404`/`500` para un `id` inválido | Sobreescribir con `MlActionResults.BadRequest` o `MlProblemsDetails.BadRequestError` |
| Paginar | Endpoint propio con `GetHeaderPaginationInfo` + `IEFRepoReaderPaginationFp<T>` |
| Proteger la API | `[Authorize]` en la derivada o política global |
| Documentar en Swagger | `[ProducesResponseType]` + `IOperationFilter` para `PkParameterAttribute` |
| Validar el DTO | `ValidateObject()` / `MlValidableFp<T>` antes del `BindAsync` al servicio |
| Que un "no existe" devuelva `404` | Que el servicio use `MlProblemsDetails.NotFoundError()` (así lo hacen ya las bases) |
| Traducir mensajes de error al español | `MapIfFail` / `MapIfFailAsync` antes de la extensión `To*ActionResult*` |

**Códigos de estado que produce cada acción heredada**

| Acción | Éxito | `id`/`ids` inválido | No existe | Error de dominio |
|---|---|---|---|---|
| `GetAllAsync` | `200` | — | — | según `MlProblemsDetails` |
| `GetByIdAsync` | `200` | `404` | `404` | según `MlProblemsDetails` |
| `PostAsync` | `201` (⚠️ `Location` literal) | — | — | según `MlProblemsDetails` |
| `PutAsync(id, dto)` | `204` | ⚠️ `500` | `404` | según `MlProblemsDetails` |
| `PutAsync(dto)` | `204` | — | `404` | según `MlProblemsDetails` |
| `DeleteAsync(id)` | `204` | ⚠️ `500` | `404` | según `MlProblemsDetails` |
| `DeleteAsync(dto)` | `204` | — | `404` | según `MlProblemsDetails` |

---

## Mejores prácticas

1. **Pon siempre `[Route("api/[controller]")]`** en la clase derivada. La base no lo trae y sin él no hay contrato de URL.

2. **Declara tus clases derivadas `sealed`** salvo que planees una jerarquía. Los controladores concretos no suelen necesitar herencia adicional.

3. **Sobreescribe `PostAsync`** si tu API es pública o la consumen terceros: el `Location` por defecto es incorrecto.

4. **Trata el `id-str/{id}` como una decisión consciente.** Si prefieres `/api/users/42` para el `GET`, sobreescribe la acción con la plantilla `"{id}"` y documenta el cambio.

5. **Usa tipos `IConvertible` en las PK compuestas** (`int`, `string`, `DateTime`, `decimal`, `bool`). Evita `Guid`, `DateOnly`, `TimeOnly` y enums en el array de `_pkFields`.

6. **Envía siempre fechas en ISO 8601** por la ruta. Es la única forma de no depender de la cultura del servidor.

7. **No uses comas dentro de los valores de una PK compuesta de tipo `string`.** El separador no es escapable.

8. **Mantén el orden de `_pkFields` sincronizado con el `HasKey`** de la configuración EF Core; y una vez publicado, no lo cambies: es parte del contrato público.

9. **Clasifica los errores en el servicio, no en el controlador.** Cualquier error que llegue sin el detalle `"ProblemsDetails"` se convertirá en un `500`.

10. **Añade `[ProducesResponseType]`** en las acciones que sobreescribas si te importa la calidad de tu OpenAPI: sin ellos, el esquema queda vacío.

11. **Aplica autorización explícitamente.** Estas bases publican siete endpoints, incluidos `DELETE`: sin `[Authorize]` quedan abiertos.

12. **No expones los mensajes de error tal cual** si tu API es pública: reescríbelos con `MapIfFail` para no filtrar textos internos en inglés defectuoso.

13. **No llames a `AddWebControllers()`**: está vacío. Registra `AddScopedOOFPRepos<,>` y `AddScopedtGenServicesFpWithoutReposGeneral()`.

14. **Añade un endpoint paginado** en cuanto una tabla pueda crecer: `GetAllAsync` trae la tabla completa.

15. **Prefiere las variantes con `{id}`/`{ids}`** frente a las que reciben el DTO por el cuerpo en `PUT`/`DELETE`: son más REST y no dependen de que la infraestructura respete el cuerpo de un `DELETE`.

16. **Si necesitas el servicio en tu código**, captúralo en tu propio constructor primario: el `_genServiceFp` de la base no es accesible desde la derivada.

---

## Resumen

- **`WebControllers` es la capa HTTP de la pila FOOP:** cuatro clases base genéricas que publican un CRUD REST completo (7 endpoints) heredando y nada más.
- **Dos ejes de variación:** PK simple (`TPk`) frente a PK compuesta (`Func<TEntity, object[]>`), y DTO único frente a par `TRequest`/`TResponse`.
- **Todas las acciones son `virtual`:** puedes quedarte con las que valen y sobreescribir las que no, o añadir endpoints propios.
- **El controlador no decide códigos de estado.** Los toma del detalle `"ProblemsDetails"` que el servicio adjunta con `MlProblemsDetails.*`; el único código que decide por su cuenta es el `404` del `GET` con `id` no convertible.
- **La conversión de la PK es el punto delicado:** `ConverterTo` depende de la cultura del hilo para `DateTime`, y `GetPkValues` exige `IConvertible` más un constructor sin parámetros.
- **Hay que conocer tres asperezas del código:** el `Location` literal del `201`, el `500` (en lugar de `404`/`400`) de `PUT`/`DELETE` con `id` inválido, y que `AddWebControllers()` no registra nada.
- **Falta todo lo transversal:** paginación, filtrado, autorización, caché, versionado y `[ProducesResponseType]`. Se añaden en la clase derivada.
- **Regla práctica:** empieza heredando; en el momento en que necesites tocar más de dos acciones, es más limpio escribir el controlador a mano usando directamente `IGenServiceFp<,>` y las extensiones de `WebApi`.

---

## Ver también

### Navegación general

- 📄 [README de la solución](../README.md) — mapa completo de todos los proyectos.
- 📄 [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) — el tipo `MlResult<T>` y su ecosistema.
- 📄 [Introducción a la documentación funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md) — por dónde empezar si vienes de OOP.

### Proyectos relacionados

- 📄 [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — de dónde salen `MlActionResults`, `MlResultWebExtensionsPlus` y `MlRequestWebExtensions`.
- 📄 [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — `IGenServiceFp<,>` y `MlProblemsDetails`: **aquí se deciden los códigos de estado**.
- 📄 [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) — los repositorios funcionales que sostienen todo lo anterior.
- 📄 [`MoralesLarios.OOFP.Internals`](../MoralesLarios.OOFP.Internals/README.md) — `PaginationInfo` y `PaginationResultInfo<T>` para tus endpoints paginados.
- 📄 [`MoralesLarios.OOFP.Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) — validación de DTOs en el raíl.
- 📄 [`MoralesLarios.OOFP.Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) — alternativa con FluentValidation.

### Documentación del núcleo útil aquí

- 📄 [`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md) — el tipo que atraviesa todas las capas.
- 📄 [`MlErrorsDetails`](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md) — cómo se transportan los errores y sus detalles.
- 📄 [`EnsureFp`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) — las guardas que usan los controladores (`NotNullAsync`).
- 📄 [`Bind`](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) — encadenar operaciones que pueden fallar.
- 📄 [`Map`](../MoralesLarios.FOOP/__Doc/Map/1_Map.md) y [`TryMap`](../MoralesLarios.FOOP/__Doc/Map/1_Map.md) — transformar el valor y capturar excepciones.
- 📄 [`Match`](../MoralesLarios.FOOP/__Doc/Match/1_Match.md) — salir del raíl para construir el `IActionResult`.
- 📄 [Operaciones asíncronas](../MoralesLarios.FOOP/__Doc/1_Intro.md#sufijos-de-asincronía) — la variante `*Async` de cada operador.
