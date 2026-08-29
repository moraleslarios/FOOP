# MoralesLarios.OOFP.ValueObjects.IO — rutas de ficheros y directorios como tipos

Extensión de [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) especializada en el sistema de archivos. Contiene cuatro value objects que responden a dos preguntas distintas:

- **¿Es una ruta bien formada?** → `MlFile`, `MlDirectory` (validación **sintáctica**, sin tocar el disco).
- **¿Existe de verdad ahora mismo?** → `ExistsFile`, `ExistDirectory` (validación **física**, consultando el disco).

Distinguir estas dos preguntas es el aporte principal de la librería: permite decidir en cada firma de método si te basta con una ruta válida o necesitas la garantía de que el recurso está ahí.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Validación sintáctica vs. validación física](#validación-sintáctica-vs-validación-física)
5. [`MlFile` — ruta de fichero bien formada](#mlfile--ruta-de-fichero-bien-formada)
6. [`MlDirectory` — ruta de directorio bien formada](#mldirectory--ruta-de-directorio-bien-formada)
7. [`ExistsFile` — fichero que existe en disco](#existsfile--fichero-que-existe-en-disco)
8. [`ExistDirectory` — directorio que existe en disco](#existdirectory--directorio-que-existe-en-disco)
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
public string LeerConfiguracion(ExistsFile ruta) => File.ReadAllText(ruta);

// La validación se hace una vez, en el borde, y devuelve MlResult
ExistsFile.ByString(rutaDelUsuario, "El fichero de configuración indicado no existe")
          .Map(LeerConfiguracion)
          .Match(valid: texto   => Procesar(texto),
                 fail : errores => Registrar(errores.ToErrorsMessages()));
```

> 💡 **La clave**: cuando un método recibe `ExistsFile`, ya no puede fallar por *"fichero no encontrado"* en su primera línea. La comprobación ocurrió antes, en el punto donde había contexto para dar un buen mensaje de error.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) | Clases base `RegexValue` y `NotEmptyString` |
| [`MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) (transitiva) | `MlResult<T>`, `MlErrorsDetails`, [`EnsureFp`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) |
| `System.IO` | `File.Exists`, `Directory.Exists` |

```csharp
using MoralesLarios.OOFP.ValueObjects.IO;
```

> ⚠️ **Atención al namespace**: es `MoralesLarios.OOFP.ValueObjects.IO`, **distinto** del de la librería base (`MoralesLarios.OOFP.ValueObjects`). Si usas ambos tipos, necesitas los dos `using`.

No requiere registro en el contenedor de dependencias.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.ValueObjects.IO/
├── MlFile.cs            → MlFile         : RegexValue      (sintaxis de fichero)
├── MlDirectory.cs       → MlDirectory    : RegexValue      (sintaxis de directorio)
├── ExistsFile.cs        → ExistsFile     : NotEmptyString  (existencia de fichero)
├── ExistDirectory.cs    → ExistDirectory : NotEmptyString  (existencia de directorio)
└── GlobalUsings.cs
```

Jerarquía real:

```
NotEmptyString  (de ValueObjects)
├── RegexValue
│   ├── MlFile
│   └── MlDirectory
├── ExistsFile
└── ExistDirectory
```

> 🔑 **Todos heredan, directa o indirectamente, de `NotEmptyString`.** Eso garantiza por construcción que **nunca habrá una ruta vacía o en blanco**, sin necesidad de comprobarlo.

---

## Validación sintáctica vs. validación física

Esta es la distinción que hay que interiorizar:

| Tipo | Qué valida | Toca el disco | Es estable en el tiempo |
|---|---|---|---|
| `MlFile` | Que el texto **tenga forma** de ruta de fichero | ❌ No | ✅ Sí |
| `MlDirectory` | Que el texto **tenga forma** de ruta de directorio | ❌ No | ✅ Sí |
| `ExistsFile` | Que el fichero **exista** en el momento de crear el VO | ✅ Sí | ❌ **No** |
| `ExistDirectory` | Que el directorio **exista** en el momento de crear el VO | ✅ Sí | ❌ **No** |

> ⚠️ **Aviso importante sobre `ExistsFile` / `ExistDirectory`**: la comprobación se hace **una sola vez, al construir el objeto**. El objeto **no vuelve a mirar el disco**. Si otro proceso borra el fichero un segundo después, tu `ExistsFile` seguirá existiendo como objeto pero apuntará a algo que ya no está. Es un *TOCTOU* clásico (*time-of-check to time-of-use*).
>
> Interpreta estos tipos como **"existía cuando lo validé"**, y sigue protegiendo la lectura real con `TryMap` / `TryBind` o `try/catch`.

**Cuándo usar cada uno:**

- **Salida / creación** (voy a *escribir* un fichero que aún no existe) → `MlFile`, `MlDirectory`.
- **Entrada / lectura** (voy a *leer* algo que debe estar ahí) → `ExistsFile`, `ExistDirectory`.
- **Configuración validada al arrancar** → `MlDirectory` para la carpeta base (aún puede no existir y crearse), `ExistsFile` para plantillas o certificados que son requisito de arranque.

---

## `MlFile` — ruta de fichero bien formada

```csharp
public class MlFile : RegexValue
{
    public const string EndpointPattern = /* patrón de ruta de fichero */;

    protected MlFile(NotEmptyString value) : base(value, EndpointPattern) { }

    public static string BuildErrorMessage(string value);          // "{value} is not a valid file path"
    public static bool   IsValid(string value);

    public static MlFile           FromString(string value);                                          // lanza
    public static MlResult<MlFile> ByString  (string value, MlErrorsDetails errorsDetails = null!);   // funcional

    public static implicit operator string(MlFile valueObject);
    public static implicit operator MlFile(string value);          // lanza si es inválida
}
```

### Qué acepta el patrón

El patrón (`MlFile.EndpointPattern`, constante pública) contempla:

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
MlFile.IsValid(@"C:\datos\informe.pdf");     // true
MlFile.IsValid(@"informe.pdf");              // true
MlFile.IsValid(@"C:\datos\");                // false → el último segmento no puede estar vacío
MlFile.IsValid(@"C:\da<tos>\a.txt");         // false
```

### Uso

```csharp
// Ruta de salida de un informe que todavía no existe
MlResult<MlFile> destino = MlFile.ByString(rutaConfigurada,
                                           $"'{rutaConfigurada}' no es una ruta de fichero válida");

destino.Map(ruta => { File.WriteAllText(ruta, contenido); return ruta; })
       .Match(valid: ruta    => Console.WriteLine($"Informe escrito en {ruta}"),
              fail : errores => Console.WriteLine(errores.ToErrorsDescription()));
```

---

## `MlDirectory` — ruta de directorio bien formada

```csharp
public class MlDirectory : RegexValue
{
    public const string EndpointPattern = /* patrón de ruta de directorio */;

    protected MlDirectory(NotEmptyString value) : base(value, EndpointPattern) { }

    public static string BuildErrorMessage(string value);          // "{value} is not a valid directory path"
    public static bool   IsValid(string value);

    public static MlDirectory           FromString(string value);
    public static MlResult<MlDirectory> ByString  (string value, MlErrorsDetails errorsDetails = null!);

    public static implicit operator string     (MlDirectory valueObject);
    public static implicit operator MlDirectory(string      value);
}
```

El patrón es **casi idéntico** al de `MlFile`, con una diferencia deliberada: **el último segmento puede estar vacío**, de modo que se admiten rutas terminadas en separador.

| Forma | Ejemplo | `MlFile` | `MlDirectory` |
|---|---|---|---|
| Sin separador final | `C:\datos` | ✅ | ✅ |
| Con separador final | `C:\datos\` | ❌ | ✅ |
| Solo raíz de unidad | `C:\` | ❌ | ✅ |
| UNC | `\\servidor\compartido\` | ❌ | ✅ |

```csharp
MlDirectory.IsValid(@"C:\datos\");   // true
MlDirectory.IsValid(@"C:\datos");    // true
MlFile     .IsValid(@"C:\datos\");   // false
```

> 💡 En la práctica, **`MlDirectory` acepta todo lo que acepta `MlFile`, y además las rutas con separador final**. No confíes en él para *distinguir* un fichero de un directorio; úsalo para *documentar tu intención* y garantizar que la ruta está bien formada y no vacía.

### Uso: crear el directorio si no existe

```csharp
public static MlResult<MlDirectory> AsegurarCarpeta(string ruta)
    => MlDirectory.ByString(ruta, $"'{ruta}' no es una ruta de carpeta válida")
                  .TryMap(carpeta => { Directory.CreateDirectory(carpeta); return carpeta; },
                          ex => $"No se pudo crear la carpeta '{ruta}': {ex.Message}");
```

---

## `ExistsFile` — fichero que existe en disco

```csharp
public class ExistsFile : NotEmptyString
{
    protected ExistsFile(string pathStr);      // lanza ArgumentNullException si no existe

    public new static string BuildErrorMessage(string pathStr);   // "{pathStr} not exists"
    public new static bool   IsValid(string pathStr);             // File.Exists(pathStr)

    public new static ExistsFile           FromString(string pathStr);
    public new static MlResult<ExistsFile> ByString  (string pathStr, MlErrorsDetails errorsDetails = null!);

    public static implicit operator string    (ExistsFile pathStrObject);
    public static implicit operator ExistsFile(string     pathStr);
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

> ⚠️ **Ojo**: un `false` de `ExistsFile` significa *"no lo encuentro"*, y eso engloba **"no existe"**, **"la ruta es inválida"** y **"no tengo permisos"**. El mensaje por defecto (`"… not exists"`) no distingue entre los tres casos. Si el diagnóstico importa, combínalo con `MlFile` para separar el error de formato del de existencia (ver [Ejemplo 3](#ejemplo-3--separar-error-de-formato-de-error-de-existencia)).

### Uso

```csharp
public static MlResult<string> LeerPlantilla(string ruta)
    => ExistsFile.ByString(ruta, $"No se encuentra la plantilla '{ruta}'")
                 .TryMap(f => File.ReadAllText(f),
                         ex => $"No se pudo leer la plantilla '{ruta}': {ex.Message}");
```

---

## `ExistDirectory` — directorio que existe en disco

```csharp
public class ExistDirectory : NotEmptyString
{
    protected ExistDirectory(string directoryStr);

    public new static string BuildErrorMessage(string directoryStr);   // "{directoryStr} not exists"
    public new static bool   IsValid(string directoryStr);             // Directory.Exists(directoryStr)

    public new static ExistDirectory           FromString(string directoryStr);
    public new static MlResult<ExistDirectory> ByString  (string directoryStr, MlErrorsDetails errorsDetails = null!);

    public static implicit operator string        (ExistDirectory directoryStrObject);
    public static implicit operator ExistDirectory(string         directoryStr);
}
```

Simétrico a `ExistsFile`, con `Directory.Exists`. Un **fichero** existente da `false`, igual que antes en espejo.

> ⚠️ **Nombre sin `s`**: el tipo se llama `ExistDirectory`, no `ExistsDirectory`. Es una asimetría respecto a `ExistsFile` que despista al escribirlo.

### Uso

```csharp
public static MlResult<IEnumerable<string>> ListarFicheros(string carpeta, string patron = "*.*")
    => ExistDirectory.ByString(carpeta, $"La carpeta '{carpeta}' no existe")
                     .TryMap(d => Directory.EnumerateFiles(d, patron),
                             ex => $"No se pudo listar '{carpeta}': {ex.Message}");
```

---

## ⚠️ Particularidades reales del código fuente

### 1. La constante del patrón se llama `EndpointPattern` en todos los tipos

`MlFile.EndpointPattern` y `MlDirectory.EndpointPattern` **no tienen nada que ver con endpoints HTTP**. El nombre viene heredado del patrón de la clase base `RegexValue` en la librería original y se replicó por copia. Es público y `const`, así que puedes reutilizarlo, pero no te fíes del nombre.

### 2. `ExistsFile` y `ExistDirectory` redeclaran los miembros con `new`

```csharp
public new static bool IsValid(string pathStr) => File.Exists(pathStr);
```

Al ocultar (`new`) en lugar de sobrescribir, **la resolución depende del tipo estático de la referencia**:

```csharp
ExistsFile.IsValid(@"C:\no-existe.txt");        // false  → usa File.Exists
NotEmptyString.IsValid(@"C:\no-existe.txt");    // true   → solo comprueba "no vacío"
```

Es correcto para uso normal, pero **no esperes polimorfismo**: si tienes una variable declarada como `NotEmptyString` que contiene un `ExistsFile`, la llamada estática no se redirige.

### 3. `ByString` de los tipos `Exist*` tiene los paréntesis desplazados

```csharp
public new static MlResult<ExistsFile> ByString(string pathStr, MlErrorsDetails errorsDetails = null!)
    => NotEmptyString.ByString(pathStr)
                     .Bind( _ => EnsureFp.That(pathStr, IsValid(pathStr), errorsDetails ?? BuildErrorMessage(pathStr))
                     .Map ( _ => new ExistsFile(pathStr)));      // ← el Map está DENTRO del Bind
```

El `.Map` está anidado dentro del `.Bind` en lugar de encadenado después. **El resultado es equivalente** (`Bind` recibe un `MlResult<ExistsFile>` y lo devuelve tal cual), así que el comportamiento es el correcto; es solo una diferencia de estilo respecto al resto de la solución. En `MlFile` y `MlDirectory` el encadenado sí es el habitual.

### 4. La validación de existencia se hace **dos veces**

En `ByString`, `EnsureFp.That(..., IsValid(pathStr), ...)` consulta el disco, y acto seguido el constructor de `ExistsFile` vuelve a llamar a `IsValid`. Son **dos accesos al sistema de archivos por cada objeto creado**. Irrelevante para casos normales, pero tenlo en cuenta si vas a validar miles de rutas en bucle.

### 5. Los mensajes por defecto están en inglés y son escuetos

`"C:\x.txt not exists"`, `"C:\x.txt is not a valid file path"`. Si el mensaje va a llegar a un usuario, **pasa tu propio `errorsDetails`**.

### 6. `MlFile` no comprueba la longitud máxima de ruta

El patrón no limita la longitud. Una ruta de 400 caracteres puede pasar la validación sintáctica y luego fallar con `PathTooLongException` al usarla. Protege siempre la operación real de E/S con `TryMap` / `TryBind`.

---

## ⚠️ Lo que NO incluye

> ⚠️ **Estos value objects solo validan.** **No hay** métodos para leer, escribir, copiar, mover ni borrar. Para eso está [`MoralesLarios.OOFP.IO`](../MoralesLarios.OOFP.IO/README.md), que envuelve `System.IO` en operaciones que devuelven `MlResult<T>`.

> ⚠️ **No existen** propiedades derivadas de la ruta: nada de `Extension`, `FileName`, `DirectoryName`, `FullPath`, `Length`, `Exists` ni `Parent`. Convierte a `string` (implícitamente) y usa `Path.GetFileName(...)`, `Path.GetExtension(...)`, `new FileInfo(...)`, etc.

> ⚠️ **No existen** variantes asíncronas (`ByStringAsync`), ni `ExistsDirectory` con `s`, ni un tipo para rutas de fichero **con extensión concreta**. Para lo último, usa `RegexValue.ByRegex(ruta, @"\.csv$")` o compón tu propia validación con `EnsureFp.That`.

> ⚠️ **No hay normalización de rutas.** El valor se guarda tal cual se recibe: `C:\datos\a.txt` y `C:/datos/a.txt` producen **dos objetos distintos y no iguales**, aunque apunten al mismo fichero. Si necesitas comparar rutas, normaliza antes con `Path.GetFullPath(...)`.

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## Ejemplos prácticos

### Ejemplo 1 — Validar la configuración de rutas al arrancar la aplicación

Es el caso de uso ideal: **fallar rápido y con un mensaje claro** en el arranque, no a mitad de un proceso nocturno.

```csharp
using MoralesLarios.OOFP.ValueObjects.IO;

public record RutasApp(ExistDirectory Entrada, MlDirectory Salida, ExistsFile Plantilla);

public static MlResult<RutasApp> ValidarRutas(IConfiguration config)
    => ExistDirectory.ByString(config["Rutas:Entrada"]!,  "La carpeta de entrada configurada no existe")
        .Bind(entrada   => MlDirectory .ByString(config["Rutas:Salida"]!,    "La ruta de salida configurada no es válida")
        .Bind(salida    => ExistsFile  .ByString(config["Rutas:Plantilla"]!, "No se encuentra la plantilla de informes")
        .Map (plantilla => new RutasApp(entrada, salida, plantilla))));

// En Program.cs
ValidarRutas(builder.Configuration)
    .Match(valid: rutas   => builder.Services.AddSingleton(rutas),
           fail : errores => throw new InvalidOperationException(
                                 $"Configuración de rutas inválida: {errores.ToErrorsDescription()}"));
```

> 💡 Fíjate en la mezcla intencionada: la carpeta de **entrada** y la **plantilla** deben existir (`Exist*`), pero la de **salida** solo necesita ser una ruta válida, porque la crearemos nosotros.

### Ejemplo 2 — Proceso completo de lectura y escritura

```csharp
public static MlResult<MlFile> ProcesarInforme(string rutaOrigen, string carpetaDestino)
    => ExistsFile.ByString(rutaOrigen, $"No se encuentra el fichero de origen '{rutaOrigen}'")
        .TryBind(origen => MlDirectory.ByString(carpetaDestino, $"'{carpetaDestino}' no es una carpeta válida")
                                      .Map(destino => (origen, destino)),
                 ex => $"Error validando rutas: {ex.Message}")
        .TryMap(x =>
        {
            Directory.CreateDirectory(x.destino);

            var contenido = File.ReadAllText(x.origen);
            var salida    = MlFile.FromString(Path.Combine(x.destino, $"procesado_{Path.GetFileName(x.origen)}"));

            File.WriteAllText(salida, contenido.ToUpperInvariant());

            return salida;
        },
        ex => $"Error procesando el informe: {ex.Message}");
```

> 💡 `Path.Combine`, `Path.GetFileName` y `File.ReadAllText` reciben los VO **sin cast explícito**, gracias a la conversión implícita a `string`.

### Ejemplo 3 — Separar error de formato de error de existencia

Los tipos `Exist*` no distinguen *"ruta mal escrita"* de *"fichero ausente"*. Encadenando `MlFile` primero, sí:

```csharp
public static MlResult<ExistsFile> AbrirFichero(string ruta)
    => MlFile.ByString(ruta, $"'{ruta}' no tiene el formato de una ruta de fichero. " +
                              "Revisa si hay caracteres no permitidos o si termina en '\\'.")
             .Bind(_ => ExistsFile.ByString(ruta, $"La ruta '{ruta}' es correcta, pero el fichero no existe " +
                                                   "o no tienes permisos para verlo."));

// "C:\da<t>os\a.txt"  → "no tiene el formato de una ruta de fichero…"
// "C:\datos\falta.txt" → "la ruta es correcta, pero el fichero no existe…"
```

> 💡 El cortocircuito de [`Bind`](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) hace el trabajo: si el formato falla, nunca se consulta el disco.

### Ejemplo 4 — Filtrar una lista de rutas y quedarse solo con las existentes

```csharp
var rutas = new[] { @"C:\datos\a.txt", @"C:\datos\b.txt", @"C:\da<t>os\c.txt" };

// Solo interesa saber cuáles existen: IsValid evita construir objetos
var existentes = rutas.Where(ExistsFile.IsValid).ToList();

// Con diagnóstico por cada ruta
foreach (var r in rutas)
{
    ExistsFile.ByString(r, $"Se omite '{r}': no disponible")
              .Match(valid: f       => Console.WriteLine($"✔ {f}"),
                     fail : errores => Console.WriteLine($"✘ {errores.Errors.First().Message}"));
}
```

### Ejemplo 5 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: usar From… con una ruta que viene del usuario
var f = ExistsFile.FromString(Request.Form["ruta"]);       // 💥 ArgumentNullException si no existe

// ✅ BIEN
var f = ExistsFile.ByString(Request.Form["ruta"], "El fichero indicado no está disponible");
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Ruta de un fichero que voy a **crear o escribir** | `MlFile` |
| Ruta de una carpeta que voy a **crear** | `MlDirectory` |
| Ruta de un fichero que voy a **leer** | `ExistsFile` |
| Ruta de una carpeta que voy a **recorrer** | `ExistDirectory` |
| Solo preguntar, sin construir el objeto | `MlFile.IsValid(...)`, `ExistsFile.IsValid(...)` |
| Distinguir "mal formada" de "no existe" | `MlFile.ByString(...).Bind(_ => ExistsFile.ByString(...))` |
| Extensión, nombre, carpeta padre… | Convertir a `string` y usar `System.IO.Path` |
| **Leer, escribir, copiar, borrar** | [`MoralesLarios.OOFP.IO`](../MoralesLarios.OOFP.IO/README.md) |
| Validar una extensión concreta | `RegexValue.ByRegex(ruta, @"\.csv$")` |

---

## Mejores prácticas

1. **`By…` para rutas que vienen de fuera; `From…` solo para literales del código.**
2. **Elige el tipo según la intención**: `Ml*` cuando la ruta puede no existir todavía, `Exist*` cuando su ausencia es un error.
3. **No guardes `ExistsFile` / `ExistDirectory` durante mucho tiempo.** Guarda `MlFile` / `MlDirectory` y revalida la existencia justo antes de usar el recurso.
4. **Protege siempre la operación real de E/S** con `TryMap` / `TryBind`: aunque el fichero exista, la lectura puede fallar por permisos, bloqueo o disco lleno.
5. **Pasa tu propio `errorsDetails` en español**; los mensajes por defecto son escuetos y en inglés.
6. **Encadena `MlFile` antes de `ExistsFile`** cuando quieras mensajes de diagnóstico precisos.
7. **Normaliza con `Path.GetFullPath` antes de construir el VO** si vas a comparar rutas o usarlas como clave.
8. **Valida las rutas de configuración en el arranque**, no en el primer uso.
9. **Usa `IsValid` para consultas masivas**: evita construir objetos y excepciones en bucles y filtros.
10. **Recuerda la asimetría de nombres**: `ExistsFile` con `s`, `ExistDirectory` sin `s`.

---

## Resumen

`MoralesLarios.OOFP.ValueObjects.IO` aporta cuatro value objects de rutas, agrupados en dos parejas con propósitos opuestos:

| | Fichero | Directorio |
|---|---|---|
| **Formato** (no toca el disco) | `MlFile` | `MlDirectory` |
| **Existencia** (consulta el disco) | `ExistsFile` | `ExistDirectory` |

- Todos heredan de `NotEmptyString`, así que **una ruta vacía nunca llega a construirse**.
- `MlFile` y `MlDirectory` validan con expresión regular (`EndpointPattern`, constante pública) y aceptan rutas absolutas, relativas y UNC; la única diferencia práctica es que `MlDirectory` admite separador final.
- `ExistsFile` y `ExistDirectory` validan con `File.Exists` / `Directory.Exists` **una sola vez, en la construcción**: son una foto del pasado, no una garantía permanente.
- Todos siguen el patrón de la librería base: `IsValid`, `BuildErrorMessage`, `From…` (lanza), `By…` (devuelve `MlResult<T>`) y conversiones implícitas hacia y desde `string`.
- **Solo validan.** Las operaciones de E/S funcionales están en [`MoralesLarios.OOFP.IO`](../MoralesLarios.OOFP.IO/README.md).

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — librería base: `NotEmptyString`, `RegexValue`, `Mail`, numéricos…
- [`MoralesLarios.OOFP.IO`](../MoralesLarios.OOFP.IO/README.md) — operaciones de fichero y directorio que devuelven `MlResult<T>`
- [`MoralesLarios.OOFP.Utilities`](../MoralesLarios.OOFP.Utilities/README.md) — lectura segura de configuración (buen sitio para validar rutas)
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validación funcional de objetos completos

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — errores y detalles](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`EnsureFp` — el motor de validación de los `By…`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`Bind` — encadenar formato + existencia](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Map` y `TryMap` — transformar protegiendo la E/S](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Match` — salir del mundo `MlResult`](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
