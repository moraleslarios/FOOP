# MoralesLarios.OOFP.Internals — tipos compartidos de paginación

Librería **muy pequeña y sin lógica de negocio** cuyo único cometido es ofrecer un **contrato común de paginación** para todo el ecosistema `MoralesLarios.OOFP`.

Es la pieza que permite que un repositorio de EF Core, un servicio de aplicación, un controlador REST y un cliente HTTP **hablen el mismo idioma** cuando se trata de "página 2, de 20 en 20, sobre un total de 350 registros", sin que ninguno de ellos tenga que depender de los demás.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [`PaginationInfo` — la petición de página](#paginationinfo--la-petición-de-página)
5. [`PaginationResultInfo<T>` — la respuesta paginada](#paginationresultinfot--la-respuesta-paginada)
6. [Conversiones implícitas desde tuplas](#conversiones-implícitas-desde-tuplas)
7. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
8. [⚠️ Lo que estos tipos NO incluyen](#️-lo-que-estos-tipos-no-incluyen)
9. [Ejemplos prácticos](#ejemplos-prácticos)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Mejores prácticas](#mejores-prácticas)
12. [Resumen](#resumen)
13. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

Sin un tipo compartido, la paginación se propaga por la aplicación como una nube de parámetros suelos que **nadie valida** y que cada capa interpreta a su manera:

❌ **Sin `Internals`:**

```csharp
// El repositorio recibe enteros crudos: nada garantiza que sean válidos.
Task<IEnumerable<User>> GetPageAsync(int pageNumber, int pageSize);

// ...y en cada capa hay que repetir la misma defensa a mano:
if (pageNumber < 1)   pageNumber = 1;
if (pageSize  < 1)    pageSize   = 10;
if (pageSize  > 1000) pageSize   = 1000;   // ¿y si aquí alguien pone 5000?

// El resultado se devuelve troceado en piezas que hay que volver a casar:
var items      = await GetPageAsync(2, 20);
var totalCount = await CountAsync();       // dos viajes, dos verdades
```

✅ **Con `Internals`:**

```csharp
// Un único parámetro que YA viene normalizado y es imposible de construir mal.
Task<MlResult<PaginationResultInfo<User>>> GetPageAsync(PaginationInfo pagination);

// La llamada es una tupla: legible y sin ceremonia.
var resultado = await repo.GetPageAsync((pageNumber: 2, pageSize: 20));

// Y la respuesta viaja completa: datos + posición + total, en un solo objeto.
```

> 💡 **La idea de fondo**: `Internals` no valida "reglas de negocio", **normaliza**. Un `PaginationInfo` recién construido es siempre coherente, así que ninguna capa posterior necesita volver a comprobarlo. Es el mismo espíritu que los [value objects](../MoralesLarios.OOFP.ValueObjects/README.md): *estados imposibles, imposibles de representar*.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) | Aporta `IntNotNegative`, usado en una de las conversiones implícitas. |
| `System.ComponentModel.DataAnnotations` | Atributos `[Range]` sobre los parámetros del record. |

`GlobalUsings.cs` del proyecto ya expone `MoralesLarios.OOFP.ValueObjects` y `System.ComponentModel.DataAnnotations`, por lo que dentro de esta librería no hacen falta `using` explícitos.

**Namespace de todos los tipos:**

```csharp
using MoralesLarios.OOFP.Internals.Info;
```

> ⚠️ Ojo: el namespace **no** es `MoralesLarios.OOFP.Internals`, sino `MoralesLarios.OOFP.Internals.Info` (los tipos viven en la carpeta `Info/`). Es el error de importación más frecuente al empezar a usar esta librería.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.Internals/
├── GlobalUsings.cs
└── Info/
    ├── PaginationInfo.cs          → PaginationInfo
    └── PaginationResultInfo.cs    → PaginationResultInfo<T>
```

Solo **dos tipos**, ambos `record`. La librería es deliberadamente mínima: cuanto menos contenga, más proyectos pueden depender de ella sin arrastrar peso.

---

## `PaginationInfo` — la petición de página

Representa **lo que pide el cliente**: qué página quiere y de cuántos elementos.

### Firma real

```csharp
namespace MoralesLarios.OOFP.Internals.Info;

public record PaginationInfo([property: Range(0, int.MinValue)] int PageNumber,
                            [property: Range(0, int.MinValue)] int PageSize)
{
    private const int MaxPageSize = 1000;

    public int PageNumber { get; init; } = Math.Max(1, PageNumber);
    public int PageSize   { get; init; } = Math.Clamp(PageSize, 1, MaxPageSize);

    public static implicit operator PaginationInfo((int pageNumber, int pageSize) value)
        => new PaginationInfo(value.pageNumber, value.pageSize);

    public static implicit operator PaginationInfo((IntNotNegative pageNumber, IntNotNegative pageSize) value)
        => new PaginationInfo(value.pageNumber, value.pageSize);
}
```

### Reglas de normalización (esto es lo importante)

El record **redeclara** sus propiedades para no usar los valores crudos del constructor, sino versiones saneadas:

| Propiedad | Regla | Efecto |
|---|---|---|
| `PageNumber` | `Math.Max(1, PageNumber)` | Cualquier valor `≤ 0` (incluidos negativos) se convierte en **1**. La paginación es 1-based. |
| `PageSize` | `Math.Clamp(PageSize, 1, 1000)` | Valores `≤ 0` → **1**. Valores `> 1000` → **1000**. Es un tope de seguridad frente a `?pageSize=999999`. |

**Nunca lanza excepción**: siempre "corrige" en silencio.

```csharp
var p1 = new PaginationInfo(0,     20);      // → PageNumber = 1,   PageSize = 20
var p2 = new PaginationInfo(-7,    20);      // → PageNumber = 1,   PageSize = 20
var p3 = new PaginationInfo(3,      0);      // → PageNumber = 3,   PageSize = 1
var p4 = new PaginationInfo(3,  50_000);     // → PageNumber = 3,   PageSize = 1000  ← tope
var p5 = new PaginationInfo(3,   1000);      // → PageNumber = 3,   PageSize = 1000  ← límite exacto, se respeta
```

> 💡 **`MaxPageSize` es `private const` = 1000** y no es configurable. Si necesitas otro tope, envuelve el tipo en tu propia validación antes de construirlo (por ejemplo con [`EnsureFp`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)).

### Cómo traducirlo a una consulta

La conversión a `Skip`/`Take` es directa, porque `PageNumber` es 1-based:

```csharp
var query = origen.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                  .Take(pagination.PageSize);
```

---

## `PaginationResultInfo<T>` — la respuesta paginada

Representa **lo que se devuelve**: los elementos de esa página más el contexto necesario para que el consumidor pueda pintar un paginador.

### Firma real

```csharp
public record PaginationResultInfo<T>(                         IEnumerable<T> Items,
                                      [Range(0, int.MaxValue)] int            PageNumber,
                                      [Range(0, int.MinValue)] int            PageSize,
                                      [Range(0, int.MinValue)] int            TotalCount)
    : PaginationInfo(PageNumber, PageSize)
{
    public static implicit operator PaginationResultInfo<T>((IEnumerable<T> items,
                                                            int            pageNumber,
                                                            int            pageSize,
                                                            int            totalCount) value)
        => new PaginationResultInfo<T>(value.items, value.pageNumber, value.pageSize, value.totalCount);
}
```

### Miembros

| Miembro | Origen | Notas |
|---|---|---|
| `Items` | Propio | Los elementos **de esta página**, no la colección completa. |
| `TotalCount` | Propio | Total de registros que existen **sin paginar**. Sirve para calcular el número de páginas. |
| `PageNumber` | **Heredado** de `PaginationInfo` | Llega ya normalizado (`Math.Max(1, …)`). |
| `PageSize` | **Heredado** de `PaginationInfo` | Llega ya normalizado (`Math.Clamp(…, 1, 1000)`). |

> 🔑 **Detalle clave**: como `PageNumber` y `PageSize` se pasan al constructor base, `PaginationResultInfo<T>` **hereda gratis la normalización**. No hay dos comportamientos distintos entre la petición y la respuesta.

### Cálculos derivados que tendrás que hacer tú

El tipo es un contenedor puro; no ofrece propiedades calculadas. Si las necesitas, un método de extensión propio es la vía limpia:

```csharp
public static class PaginationResultInfoExtensions
{
    public static int TotalPages<T>(this PaginationResultInfo<T> source)
        => (int)Math.Ceiling(source.TotalCount / (double)source.PageSize);

    public static bool HasPreviousPage<T>(this PaginationResultInfo<T> source)
        => source.PageNumber > 1;

    public static bool HasNextPage<T>(this PaginationResultInfo<T> source)
        => source.PageNumber < source.TotalPages();
}
```

---

## Conversiones implícitas desde tuplas

Las tres conversiones implícitas existen para que **las llamadas queden legibles** sin escribir `new` ni repetir el nombre del tipo.

| Desde | Hacia | Definido en |
|---|---|---|
| `(int pageNumber, int pageSize)` | `PaginationInfo` | `PaginationInfo` |
| `(IntNotNegative pageNumber, IntNotNegative pageSize)` | `PaginationInfo` | `PaginationInfo` |
| `(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)` | `PaginationResultInfo<T>` | `PaginationResultInfo<T>` |

```csharp
// 1) Tupla de enteros → petición
PaginationInfo peticion = (pageNumber: 2, pageSize: 20);

// 2) Tupla de value objects → petición (ya validados en origen)
IntNotNegative pagina  = 2;
IntNotNegative tamanyo = 20;
PaginationInfo peticionVO = (pagina, tamanyo);

// 3) Tupla de 4 elementos → respuesta
PaginationResultInfo<User> respuesta = (usuariosDeLaPagina, 2, 20, 350);
```

> 💡 **Nombra los elementos de la tupla** (`pageNumber:`, `pageSize:`) cuando ambos sean `int`. Es la única defensa real contra el clásico error de invertir el orden: `(20, 2)` compila igual de bien que `(2, 20)`, pero significa algo muy distinto.

---

## ⚠️ Particularidades reales del código fuente

Estas son observaciones sobre el código tal y como está hoy. Conviene conocerlas para no llevarse sorpresas.

### 1. Los atributos `[Range(0, int.MinValue)]` no validan nada

Aparecen escritos como `Range(0, int.MinValue)`, es decir **mínimo 0 y máximo `int.MinValue` (−2.147.483.648)**. Al ser el máximo menor que el mínimo, el rango es vacío: si alguien pasara estos tipos por un validador de DataAnnotations, **ningún valor lo cumpliría**.

> ⚠️ **Consecuencia práctica**: no confíes en los `[Range]` de estos records. **La garantía real la dan `Math.Max` y `Math.Clamp`**, que sí funcionan correctamente y son los que protegen de verdad. Los atributos parecen una errata (probablemente se pretendía `int.MaxValue`).

### 2. `TotalCount` no se normaliza

`PageNumber` y `PageSize` se sanean; **`TotalCount` no**. Se acepta tal cual, incluso negativo:

```csharp
PaginationResultInfo<string> raro = (new[] { "a" }, 1, 20, -5);   // TotalCount = -5, se acepta
```

Si `TotalCount` viene de un `CountAsync()` esto nunca ocurre, pero si lo calculas a mano, asegúralo tú.

### 3. `Items` puede ser `null`

No hay comprobación de nulidad. Para una página vacía **usa una colección vacía, no `null`**:

```csharp
// ✅ Página sin resultados, pero segura de recorrer
PaginationResultInfo<User> vacia = (Enumerable.Empty<User>(), 1, 20, 0);
```

### 4. Igualdad de `record` sobre `IEnumerable<T>`

`PaginationResultInfo<T>` es un `record`, así que su `Equals` compara `Items` **por referencia** (no elemento a elemento, porque `IEnumerable<T>` no implementa igualdad estructural). Dos respuestas con los mismos elementos en listas distintas **no serán iguales**.

---

## ⚠️ Lo que estos tipos NO incluyen

Para evitar buscar miembros que no existen:

> ⚠️ **No existen** en `PaginationInfo` ni en `PaginationResultInfo<T>`: `TotalPages`, `HasNextPage`, `HasPreviousPage`, `IsFirstPage`, `IsLastPage`, `Skip`, `Take`, `Offset`, ni ningún método `Validate()`. Tampoco hay conversión implícita hacia `MlResult<T>`. Son **records de datos**, no objetos con comportamiento.

Si necesitas esas ayudas, créalas como extensiones en tu propio proyecto (ver el ejemplo de la sección anterior).

---

## Ejemplos prácticos

### Ejemplo 1 — Paginar en memoria y devolver un resultado completo

```csharp
using MoralesLarios.OOFP.Internals.Info;

public static PaginationResultInfo<T> Paginar<T>(IEnumerable<T> origen, PaginationInfo pagination)
{
    var lista = origen as IList<T> ?? origen.ToList();

    var pagina = lista.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                      .Take(pagination.PageSize)
                      .ToList();

    // Tupla → PaginationResultInfo<T> por conversión implícita
    return (pagina, pagination.PageNumber, pagination.PageSize, lista.Count);
}

// Uso
var resultado = Paginar(todosLosUsuarios, (pageNumber: 2, pageSize: 20));

Console.WriteLine($"Página {resultado.PageNumber} de {Math.Ceiling(resultado.TotalCount / (double)resultado.PageSize)}");
Console.WriteLine($"Mostrando {resultado.Items.Count()} de {resultado.TotalCount} registros");
```

### Ejemplo 2 — Paginar sobre EF Core devolviendo `MlResult<T>`

Aquí se ve por qué `Internals` es una librería aparte: el repositorio la usa como contrato de entrada y salida, y el núcleo funcional aporta el manejo de errores.

```csharp
using MoralesLarios.OOFP.Internals.Info;
using MoralesLarios.OOFP.Types;

public async Task<MlResult<PaginationResultInfo<UserDto>>> ObtenerPaginaAsync(PaginationInfo pagination)
{
    var total = await _context.Users.CountAsync();

    var items = await _context.Users
                              .OrderBy(u => u.Id)
                              .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                              .Take(pagination.PageSize)
                              .Select(u => new UserDto(u.Id, u.Name))
                              .ToListAsync();

    PaginationResultInfo<UserDto> resultado = (items, pagination.PageNumber, pagination.PageSize, total);

    return resultado.ToMlResultValid();
}
```

### Ejemplo 3 — Endpoint de API con paginación robusta

El caso donde la normalización brilla: la entrada viene de la query string, es decir, de un usuario que puede escribir cualquier cosa.

```csharp
[HttpGet]
public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 20)
{
    // Aunque llegue ?page=-3&size=999999, PaginationInfo lo deja en (1, 1000).
    PaginationInfo pagination = (pageNumber: page, pageSize: size);

    var resultado = await _service.ObtenerPaginaAsync(pagination);

    return resultado.Match(
        valid: r => Ok(new
        {
            r.Items,
            r.PageNumber,
            r.PageSize,
            r.TotalCount,
            TotalPages = (int)Math.Ceiling(r.TotalCount / (double)r.PageSize)
        }),
        fail: errores => BadRequest(errores.ToErrorsMessages()));
}
```

> 💡 Fíjate en que **no hay una sola comprobación defensiva** de `page` ni de `size`. Está toda encapsulada en el tipo, en un único sitio.

### Ejemplo 4 — Encadenar la paginación en una tubería funcional

```csharp
using MoralesLarios.OOFP.Types;

public async Task<MlResult<PaginationResultInfo<UserDto>>> BuscarAsync(string filtro, int page, int size)
    => await EnsureFp.NotNullEmptyOrWhitespace(filtro, "El filtro de búsqueda es obligatorio")
                     .MapAsync(async f =>
                     {
                         PaginationInfo pagination = (pageNumber: page, pageSize: size);
                         return (filtro: f, pagination);
                     })
                     .BindAsync(async x => await _repo.BuscarPaginadoAsync(x.filtro, x.pagination))
                     .ExecSelfIfFailAsync(async errores =>
                         _logger.LogWarning("Búsqueda fallida: {Errores}", errores.ToErrorsDescription()));
```

### Ejemplo 5 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: volver a validar lo que el tipo ya garantiza
PaginationInfo p = (page, size);
if (p.PageNumber < 1)   p = p with { PageNumber = 1 };      // imposible: ya es ≥ 1
if (p.PageSize  > 1000) p = p with { PageSize  = 1000 };    // imposible: ya está limitado

// ✅ BIEN: confiar en la normalización
PaginationInfo p = (pageNumber: page, pageSize: size);


// ❌ MAL: tupla posicional ambigua — fácil invertir los valores
PaginationInfo p = (20, 2);          // ¿página 20 de 2 en 2? Probablemente no era la intención

// ✅ BIEN: tupla con nombres
PaginationInfo p = (pageNumber: 2, pageSize: 20);


// ❌ MAL: devolver null en Items para una página vacía
PaginationResultInfo<User> r = (null!, 1, 20, 0);           // reventará al recorrer

// ✅ BIEN: colección vacía
PaginationResultInfo<User> r = (Enumerable.Empty<User>(), 1, 20, 0);


// ❌ MAL: apoyarse en los atributos [Range] como si validaran
// (Range(0, int.MinValue) define un rango vacío: no valida nada)

// ✅ BIEN: si necesitas reglas propias, usa EnsureFp antes de construir
var validada = EnsureFp.That(size, size <= 100, "El tamaño de página máximo permitido es 100");


// ❌ MAL: importar el namespace equivocado
using MoralesLarios.OOFP.Internals;          // no contiene los tipos

// ✅ BIEN
using MoralesLarios.OOFP.Internals.Info;
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Recibir "qué página quiero" desde una API o un servicio | `PaginationInfo` (o directamente una tupla `(pageNumber:, pageSize:)`) |
| Devolver los elementos de una página **con su contexto** | `PaginationResultInfo<T>` |
| Que un `?pageSize=999999` no tumbe la base de datos | Nada extra: `PaginationInfo` lo limita a 1000 |
| Partir de valores ya validados como no negativos | Conversión implícita desde `(IntNotNegative, IntNotNegative)` |
| `TotalPages`, `HasNextPage`, etc. | Métodos de extensión propios (no existen en la librería) |
| Un tope de página distinto de 1000 | Validación propia con `EnsureFp` antes de construir el `PaginationInfo` |

---

## Mejores prácticas

1. **Importa `MoralesLarios.OOFP.Internals.Info`**, no `MoralesLarios.OOFP.Internals`. Es el fallo de arranque más habitual.
2. **Acepta `PaginationInfo` en las firmas públicas**, no dos `int` sueltos. Así la normalización ocurre una sola vez, en el borde de la aplicación.
3. **Nombra siempre los elementos de la tupla** (`pageNumber:`, `pageSize:`). Evita invertir valores sin que el compilador se queje.
4. **No repitas comprobaciones** de `PageNumber`/`PageSize` aguas abajo: el tipo ya las garantiza.
5. **No confíes en los `[Range]`** de estos records (definen un rango vacío). La garantía real es `Math.Max`/`Math.Clamp`.
6. **Devuelve colecciones vacías, nunca `null`**, en `Items`.
7. **Calcula `TotalCount` con el mismo filtro** que los `Items`, o el paginador de la interfaz mentirá.
8. **Envuelve el resultado en `MlResult<T>`** (`MlResult<PaginationResultInfo<T>>`) para que el error de consulta viaje por el mismo canal que el resto de la aplicación.
9. **Materializa `Items`** (`ToList()`) antes de construir el resultado si la fuente es un `IQueryable` diferido: evitarás que la consulta se ejecute varias veces al leer `Items` más de una vez.

---

## Resumen

`MoralesLarios.OOFP.Internals` es la librería más pequeña del ecosistema y, precisamente por eso, una de las más usadas: define **el vocabulario común de la paginación**.

- **`PaginationInfo`** → la petición. `PageNumber` mínimo 1; `PageSize` acotado entre 1 y 1000. **Nunca lanza; siempre normaliza.**
- **`PaginationResultInfo<T>`** → la respuesta. Añade `Items` y `TotalCount`, y **hereda la normalización** del tipo base.
- **Conversiones implícitas desde tuplas** para que las llamadas queden legibles.
- **Sin lógica de negocio, sin propiedades calculadas, sin dependencias pesadas**: solo el contrato.

La consumen principalmente [`EFCore`](../MoralesLarios.OOFP.EFCore/README.md) (repositorios paginados), [`WebServices`](../MoralesLarios.OOFP.WebServices/README.md) (servicios de aplicación), [`WebControllers`](../MoralesLarios.OOFP.WebControllers/README.md) (endpoints REST genéricos) y [`HttpClients`](../MoralesLarios.OOFP.HttpClients/README.md) (consumo tipado).

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — de aquí sale `IntNotNegative`
- [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) — repositorios con consultas paginadas
- [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — servicios genéricos que propagan la paginación
- [`MoralesLarios.OOFP.WebControllers`](../MoralesLarios.OOFP.WebControllers/README.md) — controladores REST genéricos
- [`MoralesLarios.OOFP.HttpClients`](../MoralesLarios.OOFP.HttpClients/README.md) — clientes tipados que consumen respuestas paginadas

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`EnsureFp` — validaciones previas](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`Map` — transformar el valor](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Match` — salir del mundo `MlResult`](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
