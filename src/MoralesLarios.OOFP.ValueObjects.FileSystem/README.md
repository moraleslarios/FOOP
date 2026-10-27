# MoralesLarios.OOFP.ValueObjects.FileSystem — rutas de ficheros y directorios como tipos

Extensión de [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) especializada en el sistema de archivos. Contiene cuatro value objects que responden a dos preguntas distintas:

- **¿Es una ruta bien formada?** → `FilePath`, `DirectoryPath` (validación **sintáctica**, sin tocar el disco).
- **¿Existe de verdad ahora mismo?** → `ExistingFilePath`, `ExistingDirectoryPath` (validación **física**, consultando el disco).

Distinguir estas dos preguntas es el aporte principal de la librería: permite decidir en cada firma de método si te basta con una ruta válida o necesitas la garantía de que el recurso está ahí.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Validación sintáctica vs. validación física](#validación-sintáctica-vs-validación-física)
5. [`FilePath` — ruta de fichero bien formada](#mlfile--ruta-de-fichero-bien-formada)
6. [`DirectoryPath` — ruta de directorio bien formada](#mldirectory--ruta-de-directorio-bien-formada)
7. [`ExistingFilePath` — fichero que existe en disco](#existsfile--fichero-que-existe-en-disco)
8. [`ExistingDirectoryPath` — directorio que existe en disco](#existdirectory--directorio-que-existe-en-disco)
9. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
10. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
11. [Ejemplos prácticos](#ejemplos-prácticos)
12. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
13. [Mejores prácticas](#mejores-prácticas)
14. [Resumen](#resumen)
15. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

Las rutas son `string`, y eso significa que **nada distingue una ruta de un mensaje de error, ni una ruta válida de una cadena vacía**.

❌ **Sin value objects de ruta:**
```csharp
public string LeerConfiguracion(string ruta)
{
    if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException(nameof(ruta));
    if (ruta.IndexOfAny(Path.GetInvalidPathChars()) >= 0) throw new ArgumentException(nameof(ruta));
    if (! File.Exists(ruta)) throw new FileNotFoundException(ruta);

    return File.ReadAllText(ruta);
}

// Y esto compila sin problema:
LeerConfiguracion("");                       // 💥
LeerConfiguracion(nombreDelUsuario);         // 💥 se pasó el dato equivocado
```

✅ **Con value objects de ruta:**
```csharp
// La firma expresa el contrato: "necesito un fichero que EXISTE"
public string LeerConfiguracion(ExistingFilePath ruta) => File.ReadAllText(ruta);

// La validación se hace una vez, en el borde, y devuelve MlResult
ExistingFilePath.FromPath(rutaDelUsuario, "El fichero de configuración indicado no existe")
          .Map(LeerConfiguracion)
          .Match(valid: texto   => Procesar(texto),
                 fail : errores => Registrar(errores.GetErrorMessages()));
```

> 💡 **La clave**: cuando un método recibe `ExistingFilePath`, ya no puede fallar por *"fichero no encontrado"* en su primera línea. La comprobación ocurrió antes, en el punto donde había contexto para dar un buen mensaje de error.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) | Clases base `RegexValue` y `NotEmptyString` |
| [`MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md) (transitiva) | `MlResult<T>`, `MlErrorDetails`, [`EnsureFp`](../docs/EnsureFp/EnsureFp.md) |
| `System.IO` | `File.Exists`, `Directory.Exists` |

```csharp
using MoralesLarios.OOFP.ValueObjects.FileSystem;
```

> ⚠️ **Atención al namespace**: es `MoralesLarios.OOFP.ValueObjects.FileSystem`, **distinto** del de la librería base (`MoralesLarios.OOFP.ValueObjects`). Si usas ambos tipos, necesitas los dos `using`.

No requiere registro en el contenedor de dependencias.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.ValueObjects.FileSystem/
├── FilePath.cs            → FilePath         : RegexValue      (sintaxis de fichero)
├── DirectoryPath.cs       → DirectoryPath    : RegexValue      (sintaxis de directorio)
├── ExistingFilePath.cs        → ExistingFilePath     : NotEmptyString  (existencia de fichero)
├── ExistingDirectoryPath.cs    → ExistingDirectoryPath : NotEmptyString  (existencia de directorio)
└── GlobalUsings.cs
```

Jerarquía real:

```
NotEmptyString  (de ValueObjects)
├── RegexValue
│   ├── FilePath
│   └── DirectoryPath
├── ExistingFilePath
└── ExistingDirectoryPath
```

> 🔑 **Todos heredan, directa o indirectamente, de `NotEmptyString`.** Eso garantiza por construcción que **nunca habrá una ruta vacía o en blanco**, sin necesidad de comprobarlo.

---

## Validación sintáctica vs. validación física

Esta es la distinción que hay que interiorizar:

| Tipo | Qué valida | Toca el disco | Es estable en el tiempo |
|---|---|---|---|
| `FilePath` | Que el texto **tenga forma** de ruta de fichero | ❌ No | ✅ Sí |
| `DirectoryPath` | Que el texto **tenga forma** de ruta de directorio | ❌ No | ✅ Sí |
| `ExistingFilePath` | Que el fichero **exista** en el momento de crear el VO | ✅ Sí | ❌ **No** |
| `ExistingDirectoryPath` | Que el directorio **exista** en el momento de crear el VO | ✅ Sí | ❌ **No** |

> ⚠️ **Aviso importante sobre `ExistingFilePath` / `ExistingDirectoryPath`**: la comprobación se hace **una sola vez, al construir el objeto**. El objeto **no vuelve a mirar el disco**. Si otro proceso borra el fichero un segundo después, tu `ExistingFilePath` seguirá existiendo como objeto pero apuntará a algo que ya no está. Es un *TOCTOU* clásico (*time-of-check to time-of-use*).
>
> Interpreta estos tipos como **"existía cuando lo validé"**, y sigue protegiendo la lectura real con `TryMap` / `TryBind` o `try/catch`.

**Cuándo usar cada uno:**

- **Salida / creación** (voy a *escribir* un fichero que aún no existe) → `FilePath`, `DirectoryPath`.
- **Entrada / lectura** (voy a *leer* algo que debe estar ahí) → `ExistingFilePath`, `ExistingDirectoryPath`.
- **Configuración validada al arrancar** → `DirectoryPath` para la carpeta base (aún puede no existir y crearse), `ExistingFilePath` para plantillas o certificados que son requisito de arranque.

---

## `FilePath` — ruta de fichero bien formada

```csharp
public class FilePath : RegexValue
{
    public const string SearchPattern = /* patrón de ruta de fichero */;

    protected FilePath(NotEmptyString value) : base(value, SearchPattern) { }

    public static string FormatErrorMessage(string value);          // "{value} is not a valid file path"
    public static bool   IsValid(string value);

    public static FilePath           FromString(string value);                                          // lanza
    public static MlResult<FilePath> ByString  (string value, MlErrorDetails errorsDetails = null!);   // funcional

    public static implicit operator string(FilePath valueObject);
    public static implicit operator FilePath(string value);          // lanza si es inválida
}
```

### Qué acepta el patrón

El patrón (`FilePath.SearchPattern`, constante pública) contempla:

| Forma | Ejemplo | Resultado |
|---|---|---|
| Ruta absoluta Windows | `C:\datos\informe.pdf` | ✅ |
| Ruta absoluta con `/` | `C:/datos/informe.pdf` | ✅ |
| Ruta UNC de red | `\\servidor\compartido\informe.pdf` | ✅ |
| Ruta relativa explícita | `.\salida\informe.pdf`, `..\otro\informe.pdf` | ✅ |
| Ruta relativa simple | `salida/informe.pdf` | ✅ |
| Nombre de fichero suelto | `informe.pdf` | ✅ |
| Termina en separador | `C:\datos\` | ❌ (eso es un directorio) |
| Contiene caracteres inválidos | `C:\da<t>os\a?.txt` | ❌ |
| Vacío o en blanco | `""`, `"   "` | ❌ (lo rechaza `NotEmptyString`) |

```csharp
FilePath.IsValid(@"C:\datos\informe.pdf");     // true
FilePath.IsValid(@"informe.pdf");              // true
FilePath.IsValid(@"C:\datos\");                // false → el último segmento no puede estar vacío
FilePath.IsValid(@"C:\da<tos>\a.txt");         // false
```

### Uso

```csharp
// Ruta de salida de un informe que todavía no existe
MlResult<FilePath> destino = FilePath.FromPath(rutaConfigurada,
                                           $"'{rutaConfigurada}' no es una ruta de fichero válida");

destino.Map(ruta => { File.WriteAllText(ruta, contenido); return ruta; })
       .Match(valid: ruta    => Console.WriteLine($"Informe escrito en {ruta}"),
              fail : errores => Console.WriteLine(errores.GetErrorsSummary()));
```

---

## `DirectoryPath` — ruta de directorio bien formada

```csharp
public class DirectoryPath : RegexValue
{
    public const string SearchPattern = /* patrón de ruta de directorio */;

    protected DirectoryPath(NotEmptyString value) : base(value, SearchPattern) { }

    public static string FormatErrorMessage(string value);          // "{value} is not a valid directory path"
    public static bool   IsValid(string value);

    public static DirectoryPath           FromString(string value);
    public static MlResult<DirectoryPath> ByString  (string value, MlErrorDetails errorsDetails = null!);

    public static implicit operator string     (DirectoryPath valueObject);
    public static implicit operator DirectoryPath(string      value);
}
```

El patrón es **casi idéntico** al de `FilePath`, con una diferencia deliberada: **el último segmento puede estar vacío**, de modo que se admiten rutas terminadas en separador.

| Forma | Ejemplo | `FilePath` | `DirectoryPath` |
|---|---|---|---|
| Sin separador final | `C:\datos` | ✅ | ✅ |
| Con separador final | `C:\datos\` | ❌ | ✅ |
| Solo raíz de unidad | `C:\` | ❌ | ✅ |
| UNC | `\\servidor\compartido\` | ❌ | ✅ |

```csharp
DirectoryPath.IsValid(@"C:\datos\");   // true
DirectoryPath.IsValid(@"C:\datos");    // true
FilePath     .IsValid(@"C:\datos\");   // false
```

> 💡 En la práctica, **`DirectoryPath` acepta todo lo que acepta `FilePath`, y además las rutas con separador final**. No confíes en él para *distinguir* un fichero de un directorio; úsalo para *documentar tu intención* y garantizar que la ruta está bien formada y no vacía.

### Uso: crear el directorio si no existe

```csharp
public static MlResult<DirectoryPath> AsegurarCarpeta(string ruta)
    => DirectoryPath.FromPath(ruta, $"'{ruta}' no es una ruta de carpeta válida")
                  .TryMap(carpeta => { Directory.CreateDirectory(carpeta); return carpeta; },
                          ex => $"No se pudo crear la carpeta '{ruta}': {ex.Message}");
```

---

## `ExistingFilePath` — fichero que existe en disco

```csharp
public class ExistingFilePath : NotEmptyString
{
    protected ExistingFilePath(string pathStr);      // lanza ArgumentNullException si no existe

    public new static string FormatErrorMessage(string pathStr);   // "{pathStr} not exists"
    public new static bool   IsValid(string pathStr);             // File.Exists(pathStr)

    public new static ExistingFilePath           FromString(string pathStr);
    public new static MlResult<ExistingFilePath> ByString  (string pathStr, MlErrorDetails errorsDetails = null!);

    public static implicit operator string    (ExistingFilePath pathStrObject);
    public static implicit operator ExistingFilePath(string     pathStr);
}
```

`IsValid` es literalmente `File.Exists(pathStr)`. Eso implica un comportamiento concreto que conviene tener claro:

| Entrada | `IsValid` | Motivo |
|---|---|---|
| Fichero existente | `true` | — |
| Fichero inexistente | `false` | — |
| **Un directorio existente** | `false` | `File.Exists` devuelve `false` para carpetas |
| Ruta mal formada | `false` | `File.Exists` no lanza, devuelve `false` |
| Sin permisos de lectura sobre la carpeta | `false` | `File.Exists` traga la excepción |

> ⚠️ **Ojo**: un `false` de `ExistingFilePath` significa *"no lo encuentro"*, y eso engloba **"no existe"**, **"la ruta es inválida"** y **"no tengo permisos"**. El mensaje por defecto (`"… not exists"`) no distingue entre los tres casos. Si el diagnóstico importa, combínalo con `FilePath` para separar el error de formato del de existencia (ver [Ejemplo 3](#ejemplo-3--separar-error-de-formato-de-error-de-existencia)).

### Uso

```csharp
public static MlResult<string> LeerPlantilla(string ruta)
    => ExistingFilePath.FromPath(ruta, $"No se encuentra la plantilla '{ruta}'")
                 .TryMap(f => File.ReadAllText(f),
                         ex => $"No se pudo leer la plantilla '{ruta}': {ex.Message}");
```

---

## `ExistingDirectoryPath` — directorio que existe en disco

```csharp
public class ExistingDirectoryPath : NotEmptyString
{
    protected ExistingDirectoryPath(string directoryStr);

    public new static string FormatErrorMessage(string directoryStr);   // "{directoryStr} not exists"
    public new static bool   IsValid(string directoryStr);             // Directory.Exists(directoryStr)

    public new static ExistingDirectoryPath           FromString(string directoryStr);
    public new static MlResult<ExistingDirectoryPath> ByString  (string directoryStr, MlErrorDetails errorsDetails = null!);

    public static implicit operator string        (ExistingDirectoryPath directoryStrObject);
    public static implicit operator ExistingDirectoryPath(string         directoryStr);
}
```

Simétrico a `ExistingFilePath`, con `Directory.Exists`. Un **fichero** existente da `false`, igual que antes en espejo.

> ⚠️ **Nombre sin `s`**: el tipo se llama `ExistingDirectoryPath`, no `ExistsDirectory`. Es una asimetría respecto a `ExistingFilePath` que despista al escribirlo.

### Uso

```csharp
public static MlResult<IEnumerable<string>> ListarFicheros(string carpeta, string patron = "*.*")
    => ExistingDirectoryPath.FromPath(carpeta, $"La carpeta '{carpeta}' no existe")
                     .TryMap(d => Directory.EnumerateFiles(d, patron),
                             ex => $"No se pudo listar '{carpeta}': {ex.Message}");
```

---

## ⚠️ Particularidades reales del código fuente

### 1. La constante del patrón se llama `SearchPattern` en todos los tipos

`FilePath.SearchPattern` y `DirectoryPath.SearchPattern` **no tienen nada que ver con endpoints HTTP**. El nombre viene heredado del patrón de la clase base `RegexValue` en la librería original y se replicó por copia. Es público y `const`, así que puedes reutilizarlo, pero no te fíes del nombre.

### 2. `ExistingFilePath` y `ExistingDirectoryPath` redeclaran los miembros con `new`

```csharp
public new static bool IsValid(string pathStr) => File.Exists(pathStr);
```

Al ocultar (`new`) en lugar de sobrescribir, **la resolución depende del tipo estático de la referencia**:

```csharp
ExistingFilePath.IsValid(@"C:\no-existe.txt");        // false  → usa File.Exists
NotEmptyString.IsValid(@"C:\no-existe.txt");    // true   → solo comprueba "no vacío"
```

Es correcto para uso normal, pero **no esperes polimorfismo**: si tienes una variable declarada como `NotEmptyString` que contiene un `ExistingFilePath`, la llamada estática no se redirige.

### 3. `ByString` de los tipos `Exist*` tiene los paréntesis desplazados

```csharp
public new static MlResult<ExistingFilePath> ByString(string pathStr, MlErrorDetails errorsDetails = null!)
    => NotEmptyString.FromString(pathStr)
                     .Bind( _ => EnsureFp.That(pathStr, IsValid(pathStr), errorsDetails ?? FormatErrorMessage(pathStr))
                     .Map ( _ => new ExistingFilePath(pathStr)));      // ← el Map está DENTRO del Bind
```

El `.Map` está anidado dentro del `.Bind` en lugar de encadenado después. **El resultado es equivalente** (`Bind` recibe un `MlResult<ExistingFilePath>` y lo devuelve tal cual), así que el comportamiento es el correcto; es solo una diferencia de estilo respecto al resto de la solución. En `FilePath` y `DirectoryPath` el encadenado sí es el habitual.

### 4. La validación de existencia se hace **dos veces**

En `ByString`, `EnsureFp.That(..., IsValid(pathStr), ...)` consulta el disco, y acto seguido el constructor de `ExistingFilePath` vuelve a llamar a `IsValid`. Son **dos accesos al sistema de archivos por cada objeto creado**. Irrelevante para casos normales, pero tenlo en cuenta si vas a validar miles de rutas en bucle.

### 5. Los mensajes por defecto están en inglés y son escuetos

`"C:\x.txt not exists"`, `"C:\x.txt is not a valid file path"`. Si el mensaje va a llegar a un usuario, **pasa tu propio `errorsDetails`**.

### 6. `FilePath` no comprueba la longitud máxima de ruta

El patrón no limita la longitud. Una ruta de 400 caracteres puede pasar la validación sintáctica y luego fallar con `PathTooLongException` al usarla. Protege siempre la operación real de E/S con `TryMap` / `TryBind`.

---

## ⚠️ Lo que NO incluye

> ⚠️ **Estos value objects solo validan.** **No hay** métodos para leer, escribir, copiar, mover ni borrar. Para eso está [`MoralesLarios.OOFP.FileSystem`](../MoralesLarios.OOFP.FileSystem/README.md), que envuelve `System.IO` en operaciones que devuelven `MlResult<T>`.

> ⚠️ **No existen** propiedades derivadas de la ruta: nada de `Extension`, `FileName`, `DirectoryName`, `FullPath`, `Length`, `Exists` ni `Parent`. Convierte a `string` (implícitamente) y usa `Path.GetFileName(...)`, `Path.GetExtension(...)`, `new FileInfo(...)`, etc.

> ⚠️ **No existen** variantes asíncronas (`ByStringAsync`), ni `ExistsDirectory` con `s`, ni un tipo para rutas de fichero **con extensión concreta**. Para lo último, usa `RegexValue.FromRegex(ruta, @"\.csv$")` o compón tu propia validación con `EnsureFp.That`.

> ⚠️ **No hay normalización de rutas.** El valor se guarda tal cual se recibe: `C:\datos\a.txt` y `C:/datos/a.txt` producen **dos objetos distintos y no iguales**, aunque apunten al mismo fichero. Si necesitas comparar rutas, normaliza antes con `Path.GetFullPath(...)`.

> ⚠️ **Sobre `MlErrorDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `GetErrorMessages()`, `GetErrorsSummary()`, `Errors.First().Message`, `GetMetadataValue<T>()`, `GetException()`, `GetMetadataSummary()`.

---

## Ejemplos prácticos

### Ejemplo 1 — Validar la configuración de rutas al arrancar la aplicación

Es el caso de uso ideal: **fallar rápido y con un mensaje claro** en el arranque, no a mitad de un proceso nocturno.

```csharp
using MoralesLarios.OOFP.ValueObjects.FileSystem;

public record RutasApp(ExistingDirectoryPath Entrada, DirectoryPath Salida, ExistingFilePath Plantilla);

public static MlResult<RutasApp> ValidarRutas(IConfiguration config)
    => ExistingDirectoryPath.FromPath(config["Rutas:Entrada"]!,  "La carpeta de entrada configurada no existe")
        .Bind(entrada   => DirectoryPath .FromString(config["Rutas:Salida"]!,    "La ruta de salida configurada no es válida")
        .Bind(salida    => ExistingFilePath  .FromString(config["Rutas:Plantilla"]!, "No se encuentra la plantilla de informes")
        .Map (plantilla => new RutasApp(entrada, salida, plantilla))));

// En Program.cs
ValidarRutas(builder.Configuration)
    .Match(valid: rutas   => builder.Services.AddSingleton(rutas),
           fail : errores => throw new InvalidOperationException(
                                 $"Configuración de rutas inválida: {errores.GetErrorsSummary()}"));
```

> 💡 Fíjate en la mezcla intencionada: la carpeta de **entrada** y la **plantilla** deben existir (`Exist*`), pero la de **salida** solo necesita ser una ruta válida, porque la crearemos nosotros.

### Ejemplo 2 — Proceso completo de lectura y escritura

```csharp
public static MlResult<FilePath> ProcesarInforme(string rutaOrigen, string carpetaDestino)
    => ExistingFilePath.FromPath(rutaOrigen, $"No se encuentra el fichero de origen '{rutaOrigen}'")
        .TryBind(origen => DirectoryPath.FromPath(carpetaDestino, $"'{carpetaDestino}' no es una carpeta válida")
                                      .Map(destino => (origen, destino)),
                 ex => $"Error validando rutas: {ex.Message}")
        .TryMap(x =>
        {
            Directory.CreateDirectory(x.destino);

            var contenido = File.ReadAllText(x.origen);
            var salida    = FilePath.CreateFromPath(Path.CombineAll(x.destino, $"procesado_{Path.GetFileName(x.origen)}"));

            File.WriteAllText(salida, contenido.ToUpperInvariant());

            return salida;
        },
        ex => $"Error procesando el informe: {ex.Message}");
```

> 💡 `Path.CombineAll`, `Path.GetFileName` y `File.ReadAllText` reciben los VO **sin cast explícito**, gracias a la conversión implícita a `string`.

### Ejemplo 3 — Separar error de formato de error de existencia

Los tipos `Exist*` no distinguen *"ruta mal escrita"* de *"fichero ausente"*. Encadenando `FilePath` primero, sí:

```csharp
public static MlResult<ExistingFilePath> AbrirFichero(string ruta)
    => FilePath.FromPath(ruta, $"'{ruta}' no tiene el formato de una ruta de fichero. " +
                              "Revisa si hay caracteres no permitidos o si termina en '\\'.")
             .Bind(_ => ExistingFilePath.FromPath(ruta, $"La ruta '{ruta}' es correcta, pero el fichero no existe " +
                                                   "o no tienes permisos para verlo."));

// "C:\da<t>os\a.txt"  → "no tiene el formato de una ruta de fichero…"
// "C:\datos\falta.txt" → "la ruta es correcta, pero el fichero no existe…"
```

> 💡 El cortocircuito de [`Bind`](../docs/Bind/3_Bind.md) hace el trabajo: si el formato falla, nunca se consulta el disco.

### Ejemplo 4 — Filtrar una lista de rutas y quedarse solo con las existentes

```csharp
var rutas = new[] { @"C:\datos\a.txt", @"C:\datos\b.txt", @"C:\da<t>os\c.txt" };

// Solo interesa saber cuáles existen: IsValid evita construir objetos
var existentes = rutas.Where(ExistingFilePath.IsValid).ToList();

// Con diagnóstico por cada ruta
foreach (var r in rutas)
{
    ExistingFilePath.FromPath(r, $"Se omite '{r}': no disponible")
              .Match(valid: f       => Console.WriteLine($"✔ {f}"),
                     fail : errores => Console.WriteLine($"✘ {errores.Errors.First().Message}"));
}
```

### Ejemplo 5 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: usar CreateFrom… con una ruta que viene del usuario
var f = ExistingFilePath.CreateFromPath(Request.Form["ruta"]);       // 💥 ArgumentNullException si no existe

// ✅ BIEN
var f = ExistingFilePath.FromPath(Request.Form["ruta"], "El fichero indicado no está disponible");
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Ruta de un fichero que voy a **crear o escribir** | `FilePath` |
| Ruta de una carpeta que voy a **crear** | `DirectoryPath` |
| Ruta de un fichero que voy a **leer** | `ExistingFilePath` |
| Ruta de una carpeta que voy a **recorrer** | `ExistingDirectoryPath` |
| Solo preguntar, sin construir el objeto | `FilePath.IsValid(...)`, `ExistingFilePath.IsValid(...)` |
| Distinguir "mal formada" de "no existe" | `FilePath.FromPath(...).Bind(_ => ExistingFilePath.FromPath(...))` |
| Extensión, nombre, carpeta padre… | Convertir a `string` y usar `System.IO.Path` |
| **Leer, escribir, copiar, borrar** | [`MoralesLarios.OOFP.FileSystem`](../MoralesLarios.OOFP.FileSystem/README.md) |
| Validar una extensión concreta | `RegexValue.FromRegex(ruta, @"\.csv$")` |

---

## Mejores prácticas

1. **`From…` para rutas que vienen de fuera; `CreateFrom…` solo para literales del código.**
2. **Elige el tipo según la intención**: `Ml*` cuando la ruta puede no existir todavía, `Exist*` cuando su ausencia es un error.
3. **No guardes `ExistingFilePath` / `ExistingDirectoryPath` durante mucho tiempo.** Guarda `FilePath` / `DirectoryPath` y revalida la existencia justo antes de usar el recurso.
4. **Protege siempre la operación real de E/S** con `TryMap` / `TryBind`: aunque el fichero exista, la lectura puede fallar por permisos, bloqueo o disco lleno.
5. **Pasa tu propio `errorsDetails` en español**; los mensajes por defecto son escuetos y en inglés.
6. **Encadena `FilePath` antes de `ExistingFilePath`** cuando quieras mensajes de diagnóstico precisos.
7. **Normaliza con `Path.GetFullPath` antes de construir el VO** si vas a comparar rutas o usarlas como clave.
8. **Valida las rutas de configuración en el arranque**, no en el primer uso.
9. **Usa `IsValid` para consultas masivas**: evita construir objetos y excepciones en bucles y filtros.
10. **Recuerda la asimetría de nombres**: `ExistingFilePath` con `s`, `ExistingDirectoryPath` sin `s`.

---

## Resumen

`MoralesLarios.OOFP.ValueObjects.FileSystem` aporta cuatro value objects de rutas, agrupados en dos parejas con propósitos opuestos:

| | Fichero | Directorio |
|---|---|---|
| **Formato** (no toca el disco) | `FilePath` | `DirectoryPath` |
| **Existencia** (consulta el disco) | `ExistingFilePath` | `ExistingDirectoryPath` |

- Todos heredan de `NotEmptyString`, así que **una ruta vacía nunca llega a construirse**.
- `FilePath` y `DirectoryPath` validan con expresión regular (`SearchPattern`, constante pública) y aceptan rutas absolutas, relativas y UNC; la única diferencia práctica es que `DirectoryPath` admite separador final.
- `ExistingFilePath` y `ExistingDirectoryPath` validan con `File.Exists` / `Directory.Exists` **una sola vez, en la construcción**: son una foto del pasado, no una garantía permanente.
- Todos siguen el patrón de la librería base: `IsValid`, `FormatErrorMessage`, `CreateFrom…` (lanza), `From…` (devuelve `MlResult<T>`) y conversiones implícitas hacia y desde `string`.
- **Solo validan.** Las operaciones de E/S funcionales están en [`MoralesLarios.OOFP.FileSystem`](../MoralesLarios.OOFP.FileSystem/README.md).

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md)
- [Introducción general al núcleo funcional](../docs/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — librería base: `NotEmptyString`, `RegexValue`, `Mail`, numéricos…
- [`MoralesLarios.OOFP.FileSystem`](../MoralesLarios.OOFP.FileSystem/README.md) — operaciones de fichero y directorio que devuelven `MlResult<T>`
- [`MoralesLarios.OOFP.Configuration`](../MoralesLarios.OOFP.Configuration/README.md) — lectura segura de configuración (buen sitio para validar rutas)
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validación funcional de objetos completos

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../docs/core-types/MlResult.md)
- [`MlErrorDetails` — errores y detalles](../docs/core-types/MlResultErrors.md)
- [`EnsureFp` — el motor de validación de los `From…`](../docs/EnsureFp/EnsureFp.md)
- [`Bind` — encadenar formato + existencia](../docs/Bind/3_Bind.md)
- [`Map` y `TryMap` — transformar protegiendo la E/S](../docs/Map/1_Map.md)
- [`Match` — salir del mundo `MlResult`](../docs/Match/1_Match.md)
