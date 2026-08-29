# MoralesLarios.OOFP.IO — Entrada/salida en el raíl funcional

El proyecto **más pequeño de toda la solución**: una interfaz con **tres métodos** que envuelven operaciones de `System.IO` para que **la ruta se valide antes de tocar el disco** y el resultado viaje como [`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md).

Su valor no está en la cantidad de operaciones que cubre —cubre muy pocas—, sino en el **patrón** que demuestra: validar la ruta con un [objeto de valor](../MoralesLarios.OOFP.ValueObjects.IO/README.md) y solo entonces ejecutar la operación.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Registro en el contenedor](#registro-en-el-contenedor)
5. [`IWrapperIO` — los tres métodos](#iwrapperio--los-tres-métodos)
6. [El patrón: validar la ruta, luego actuar](#el-patrón-validar-la-ruta-luego-actuar)
7. [Relación con `ValueObjects.IO`](#relación-con-valueobjectsio)
8. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
9. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
10. [Cómo extenderlo](#cómo-extenderlo)
11. [Ejemplos prácticos](#ejemplos-prácticos)
12. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
13. [Mejores prácticas](#mejores-prácticas)
14. [Resumen](#resumen)
15. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

`System.IO` **comunica los errores lanzando excepciones**, y son muchas y variadas: `FileNotFoundException`, `DirectoryNotFoundException`, `UnauthorizedAccessException`, `PathTooLongException`, `IOException`… Además, el código de `System.IO` **no es testeable sin tocar disco**.

❌ **Con `System.IO` directamente:**

```csharp
public Informe Procesar(string ruta)
{
    var texto = File.ReadAllText(ruta);      // 💥 puede lanzar 5 excepciones distintas
    return Parsear(texto);
}

// En el consumidor:
try
{
    var informe = Procesar(ruta);
}
catch (FileNotFoundException ex)      { /* … */ }
catch (DirectoryNotFoundException ex) { /* … */ }
catch (UnauthorizedAccessException ex){ /* … */ }
catch (IOException ex)                { /* … */ }
```

✅ **Con `IWrapperIO`:**

```csharp
public MlResult<Informe> Procesar(string ruta)
    => _io.ReadAllText(ruta)
          .Map(Parsear);
```

Si el fichero no existe, obtienes un `Fail` con el mensaje `"C:\tmp\x.txt not exists"`. **Sin `try`, sin `catch`, y el error se compone con el resto del pipeline.**

> 💡 **La ventaja añadida es la testabilidad**: al depender de `IWrapperIO` en lugar de los métodos estáticos de `File`, puedes sustituirlo por un doble en los tests unitarios y no tocar el sistema de ficheros.

---

## Instalación y dependencias

| Dependencia | Versión | Para qué |
|---|---|---|
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 9.0.6 | `IServiceCollection` |
| [`MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) | — | `MlResult<T>`, `Map` |
| [`MoralesLarios.OOFP.ValueObjects.IO`](../MoralesLarios.OOFP.ValueObjects.IO/README.md) | — | 🔑 `ExistsFile`, `ExistDirectory` |

Destino: **`net8.0`**. Versión del paquete: **1.0.0**.

```csharp
using MoralesLarios.OOFP.IO;              // IWrapperIO, AddOOFPIO
using MoralesLarios.OOFP.ValueObjects.IO; // ExistsFile (aparece en las firmas)
```

> ⚠️ **Versión 1.0.0 y sin `GeneratePackageOnBuild`**: a diferencia de otros proyectos de la solución, este no genera paquete NuGet al compilar. Es el proyecto menos maduro del conjunto.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.IO/
├── GlobalUsing.cs           ← ⚠️ en singular, no "GlobalUsings.cs"
├── IWrapperIO.cs            → la interfaz (3 métodos)
├── WrapperIO.cs             → la implementación
└── RegisterServices.cs      → AddOOFPIO
```

**Tres ficheros de código y 40 líneas en total.** Es el proyecto más pequeño de la solución.

---

## Registro en el contenedor

```csharp
public static IServiceCollection AddOOFPIO(this IServiceCollection services)
{
    services.AddSingleton<IWrapperIO, WrapperIO>();
    return services;
}
```

```csharp
builder.Services.AddOOFPIO();
```

> 💡 **`Singleton` es la elección correcta**: `WrapperIO` no tiene estado, ni campos, ni constructor. Una única instancia sirve para toda la aplicación.

> ⚠️ **El nombre del método rompe la convención del resto de la solución.** Aquí es `AddOOFPIO()`, mientras que en los demás proyectos es `AddMl…` (`AddMlUtilitiesConfig`, `AddMlEFCore…`). No hay `AddMlIO`.

---

## `IWrapperIO` — los tres métodos

```csharp
public interface IWrapperIO
{
    MlResult<IEnumerable<ExistsFile>> EnumerateFiles(string directoryStr);
    MlResult<string>                  ReadAllText   (string filePathStr);
    MlResult<IEnumerable<string>>     ReadAllLines  (string filePathStr);
}
```

| Método | Valida | Ejecuta | Devuelve |
|---|---|---|---|
| `EnumerateFiles(directorio)` | `ExistDirectory.ByString` | `Directory.EnumerateFiles` | `MlResult<IEnumerable<ExistsFile>>` |
| `ReadAllText(fichero)` | `ExistsFile.ByString` | `File.ReadAllText` | `MlResult<string>` |
| `ReadAllLines(fichero)` | `ExistsFile.ByString` | `File.ReadLines` | `MlResult<IEnumerable<string>>` |

**Toda la superficie pública del proyecto son estos tres métodos.** Son **solo de lectura**: no hay escritura, ni copia, ni borrado.

> 💡 **`EnumerateFiles` devuelve `ExistsFile`, no `string`**: los ficheros vienen ya envueltos en un objeto de valor que garantiza su existencia. Es un buen detalle de diseño, porque el tipo del resultado transporta la garantía.

---

## El patrón: validar la ruta, luego actuar

Los tres métodos siguen exactamente la misma forma:

```csharp
public MlResult<string> ReadAllText(string filePathStr)
{
    var result = ExistsFile.ByString(filePathStr)          // 1️⃣ valida: ¿no vacía y existe?
                           .Map(_ => File.ReadAllText(filePathStr));   // 2️⃣ solo entonces, lee
    return result;
}
```

```csharp
public MlResult<IEnumerable<ExistsFile>> EnumerateFiles(string directoryStr)
{
    var result = ExistDirectory.ByString(directoryStr)
                               .Map(_     => Directory.EnumerateFiles(directoryStr))
                               .Map(files => files.Select(file => ExistsFile.FromString(file)));
    return result;
}
```

| Paso | Qué garantiza |
|---|---|
| `ByString` | La ruta **no es nula ni vacía** (heredado de `NotEmptyString`) **y el fichero/directorio existe** |
| `.Map(…)` | **Solo se ejecuta si la validación pasó.** Si la ruta no existe, `System.IO` nunca se llama |

> 💡 **La clave del patrón**: el `Map` cortocircuita. Es la razón de que no haya que comprobar `File.Exists` a mano ni capturar `FileNotFoundException`: **el objeto de valor ya lo hizo.**

> ⚠️ **Nótese que se descarta el objeto de valor** (`.Map(_ => …)`) y se vuelve a usar la `string` original. Funciona porque son equivalentes, pero significa que el `ExistsFile` se crea solo para validar.

---

## Relación con `ValueObjects.IO`

Los dos objetos de valor que usa (`ExistsFile` y `ExistDirectory`) viven en [`MoralesLarios.OOFP.ValueObjects.IO`](../MoralesLarios.OOFP.ValueObjects.IO/README.md) y ambos heredan de `NotEmptyString`:

```csharp
public class ExistsFile : NotEmptyString
{
    public new static bool   IsValid          (string pathStr) => File.Exists(pathStr);
    public new static string BuildErrorMessage(string pathStr) => $"{pathStr} not exists";

    public new static MlResult<ExistsFile> ByString(string pathStr, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(pathStr)
                         .Bind(_ => EnsureFp.That(pathStr, IsValid(pathStr),
                                                  errorsDetails ?? BuildErrorMessage(pathStr))
                         .Map (_ => new ExistsFile(pathStr)));
}
```

| Aspecto | `ExistsFile` | `ExistDirectory` |
|---|---|---|
| Comprobación | `File.Exists` | `Directory.Exists` |
| Mensaje de error | `"{ruta} not exists"` | `"{ruta} not exists"` |
| `FromString` | Lanza `ArgumentNullException` si no existe | Idem |
| `ByString` | Devuelve `MlResult` | Idem |

> ⚠️ **`FromString` lanza, `ByString` devuelve `MlResult`.** `WrapperIO` usa `ByString` para validar (seguro) pero `FromString` dentro de `EnumerateFiles` (ver [Particularidades](#️-particularidades-reales-del-código-fuente)).

> ⚠️ **El mensaje de error es idéntico** para fichero y directorio (`"X not exists"`), así que **no distingue si esperabas un fichero o una carpeta**. Personalízalo si el mensaje va a llegar a un usuario.

---

## ⚠️ Particularidades reales del código fuente

### 1. ⚠️ `ReadAllLines` usa `File.ReadLines`, que es **perezoso**

```csharp
public MlResult<IEnumerable<string>> ReadAllLines(string filePathStr)
    => ExistsFile.ByString(filePathStr)
                 .Map(_ => File.ReadLines(filePathStr));   // ⚠️ ReadLines, no ReadAllLines
```

| | `File.ReadAllLines` | `File.ReadLines` (el que se usa) |
|---|---|---|
| Devuelve | `string[]` **ya leído** | `IEnumerable<string>` **perezoso** |
| Cuándo lee | Inmediatamente | **Al enumerar** |
| Fichero abierto | No, se cierra al volver | **Se abre al empezar a enumerar** |

**Consecuencias importantes:**

```csharp
var lineas = _io.ReadAllLines(ruta);   // ✅ Valid, pero AÚN NO SE HA LEÍDO NADA

File.Delete(ruta);                     // borramos el fichero

foreach (var l in lineas.Value)        // 💥 IOException AQUÍ, fuera del raíl funcional
    Console.WriteLine(l);
```

> ⚠️ **Una excepción durante la enumeración escapa del `MlResult`.** El `Fail` solo cubre "el fichero no existía en el momento de validar", no los errores de lectura posteriores.
>
> **Solución**: materializa dentro del raíl si necesitas la garantía.
> ```csharp
> var lineas = _io.ReadAllLines(ruta)
>                 .TryMap(l => l.ToList(), "Error al leer las líneas del fichero");
> ```

> 💡 El nombre del método (`ReadAllLines`) **sugiere lectura completa**, pero la implementación es perezosa. Es una discrepancia entre nombre e implementación que conviene tener presente.

### 2. `EnumerateFiles` también es perezoso, y doblemente

```csharp
.Map(_     => Directory.EnumerateFiles(directoryStr))    // perezoso
.Map(files => files.Select(file => ExistsFile.FromString(file)));   // perezoso
```

Ni `EnumerateFiles` ni `Select` se ejecutan hasta que enumeras. **Y `ExistsFile.FromString` lanza `ArgumentNullException` si el fichero ya no existe:**

```csharp
var ficheros = _io.EnumerateFiles(carpeta);    // ✅ Valid

// otro proceso borra un fichero de la carpeta…

foreach (var f in ficheros.Value)              // 💥 ArgumentNullException al llegar a ese fichero
    Procesar(f);
```

> ⚠️ **Esta es una condición de carrera real** en carpetas activas (por ejemplo, una carpeta de entrada que otro proceso vacía). Materializa con `TryMap` si el escenario es concurrente.

### 3. `ReadAllText` sí es inmediato

Es el único de los tres que lee de verdad dentro del `Map`. **Es el más seguro de los tres.**

### 4. ⚠️ Las excepciones de `System.IO` no están capturadas

Ningún `Map` es un `TryMap`. La validación previa cubre **existencia**, pero no:

| Escenario | Qué ocurre |
|---|---|
| Fichero sin permisos de lectura | 💥 `UnauthorizedAccessException` escapa |
| Fichero bloqueado por otro proceso | 💥 `IOException` escapa |
| Ruta demasiado larga | 💥 `PathTooLongException` escapa |
| Fichero borrado entre validar y leer | 💥 `FileNotFoundException` escapa |
| Directorio de red caído | 💥 `IOException` escapa |

> ⚠️ **El proyecto solo convierte en `Fail` el caso "no existe"**. Todos los demás errores de E/S siguen siendo excepciones. Si necesitas cobertura completa, envuelve tú:
> ```csharp
> MlResult.TryMap(() => _io.ReadAllText(ruta), "Error al leer el fichero")
>         .Bind(r => r);
> ```

### 5. Hay una ventana entre validar y ejecutar (TOCTOU)

```csharp
ExistsFile.ByString(filePathStr)              // t0: el fichero existe
          .Map(_ => File.ReadAllText(filePathStr));   // t1: ¿sigue existiendo?
```

Entre `t0` y `t1` el fichero puede desaparecer. Es el clásico problema *time-of-check to time-of-use*, **inevitable** en cualquier diseño de este tipo, pero conviene saber que la validación **no es una garantía absoluta**.

### 6. El fichero se llama `GlobalUsing.cs`, en singular

En el resto de la solución es `GlobalUsings.cs`. Detalle cosmético, sin efecto.

### 7. `WrapperIO` no tiene constructor ni estado

Los tres métodos podrían ser estáticos. La interfaz existe **exclusivamente para permitir la sustitución en tests**, lo cual es una razón perfectamente válida.

### 8. La variable local `result` es innecesaria

```csharp
var result = ExistsFile.ByString(filePathStr).Map(…);
return result;
```

Podría ser un cuerpo de expresión (`=>`). Es solo estilo.

---

## ⚠️ Lo que NO incluye

> ⚠️ **No hay escritura.** No existe `WriteAllText`, `WriteAllLines`, `AppendText`… **El proyecto es de solo lectura.**

> ⚠️ **No hay operaciones de gestión.** Nada de `Copy`, `Move`, `Delete`, `CreateDirectory`, `Exists`.

> ⚠️ **No hay métodos asíncronos.** No existe `ReadAllTextAsync`. Para E/S, que es el caso de uso asíncrono por excelencia, esto es una limitación notable.

> ⚠️ **No hay enumeración de directorios.** `EnumerateFiles` lista ficheros; no hay `EnumerateDirectories`.

> ⚠️ **No hay filtros ni recursividad.** `EnumerateFiles` no acepta `searchPattern` (`"*.txt"`) ni `SearchOption.AllDirectories`. Tendrás que filtrar después con LINQ.

> ⚠️ **No hay control de codificación.** `File.ReadAllText` se llama sin `Encoding`, así que se usa UTF-8 con detección de BOM. Un fichero en `Windows-1252` se leerá mal sin avisar.

> ⚠️ **No hay streams.** No hay forma de procesar un fichero grande sin cargarlo (bueno, `ReadLines` lo permite, pero con los riesgos ya descritos).

> ⚠️ **No hay personalización del mensaje de error.** Los métodos no aceptan `MlErrorsDetails`, aunque `ExistsFile.ByString` sí lo soporta. **La capacidad existe en el objeto de valor pero no se expone en el wrapper.**

> ⚠️ **No hay tests.** No existe un proyecto `MoralesLarios.OOFP.IO.Tests`.

---

## Cómo extenderlo

Como la superficie es tan reducida, es probable que necesites más operaciones. El patrón es trivial de replicar:

```csharp
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.ValueObjects.IO;

public interface IWrapperIOExtendido : IWrapperIO
{
    MlResult<string>              WriteAllText   (string filePathStr, string contenido);
    Task<MlResult<string>>        ReadAllTextAsync(string filePathStr);
    MlResult<IEnumerable<ExistsFile>> EnumerateFiles(string directoryStr, string patron);
}

public class WrapperIOExtendido : WrapperIO, IWrapperIOExtendido
{
    // ✅ Escritura: valida el DIRECTORIO padre, no el fichero (aún no existe)
    public MlResult<string> WriteAllText(string filePathStr, string contenido)
        => ExistDirectory.ByString(Path.GetDirectoryName(filePathStr)!")
                         .TryMap(_ => { File.WriteAllText(filePathStr, contenido); return filePathStr; },
                                 $"Error al escribir en '{filePathStr}'");

    // ✅ Asíncrono
    public Task<MlResult<string>> ReadAllTextAsync(string filePathStr)
        => ExistsFile.ByString(filePathStr)
                     .TryMapAsync(_ => File.ReadAllTextAsync(filePathStr),
                                  $"Error al leer '{filePathStr}'");

    // ✅ Con patrón y materialización inmediata (evita la pereza)
    public MlResult<IEnumerable<ExistsFile>> EnumerateFiles(string directoryStr, string patron)
        => ExistDirectory.ByString(directoryStr)
                         .TryMap(_ => Directory.EnumerateFiles(directoryStr, patron)
                                               .Select(ExistsFile.FromString)
                                               .ToList()
                                               .AsEnumerable(),
                                 $"Error al enumerar '{directoryStr}'");
}
```

> 💡 **Tres mejoras clave** respecto al original: usar `TryMap` en lugar de `Map` (captura excepciones), materializar con `ToList()` (evita la pereza) y validar el **directorio padre** al escribir (el fichero destino aún no existe).

---

## Ejemplos prácticos

### Ejemplo 1 — Leer y parsear un fichero de configuración

```csharp
public class ProcesadorInformes(IWrapperIO _io)
{
    public MlResult<Informe> Procesar(string ruta)
        => _io.ReadAllText(ruta)
              .TryMap(json => JsonSerializer.Deserialize<Informe>(json)!,
                      "El fichero no contiene un informe válido")
              .BindEnsure(i => i.Lineas.Any(), "El informe no tiene líneas");
}
```

### Ejemplo 2 — Procesar todos los ficheros de una carpeta

```csharp
public MlResult<IEnumerable<Registro>> ProcesarCarpeta(string carpeta)
    => _io.EnumerateFiles(carpeta)
          .TryMap(ficheros => ficheros.ToList(),                    // 🔑 materializa YA
                  $"Error al enumerar la carpeta '{carpeta}'")
          .Bind(ficheros => ficheros.Where(f => f.Value.EndsWith(".csv"))
                                    .Select(f => _io.ReadAllText(f).Map(Parsear))
                                    .FusionErrosIfExists());
```

> 💡 El `TryMap` con `ToList()` es **imprescindible aquí**: evita la condición de carrera de la enumeración perezosa.

### Ejemplo 3 — Combinado con `Utilities` para leer la ruta de configuración

```csharp
public class ImportadorFicheros(IMlConfigManager _config, IWrapperIO _io)
{
    public MlResult<IEnumerable<ExistsFile>> ListarPendientes()
        => _config.ReadAppSettingKey<string>("Rutas:RutaFicheros",
                                             "Falta 'Rutas:RutaFicheros' en la configuración")
                  .BindEnsure(r => !string.IsNullOrWhiteSpace(r), "La ruta configurada está vacía")
                  .Bind(ruta => _io.EnumerateFiles(ruta));
}
```

> 💡 El `BindEnsure` es necesario porque `IMlConfigManager` **acepta cadenas vacías como valor válido** (ver [`Utilities`](../MoralesLarios.OOFP.Utilities/README.md)), y una ruta vacía haría fallar `ExistDirectory` con un mensaje menos claro.

### Ejemplo 4 — Con logging

```csharp
using MoralesLarios.OOFP.Extensions.Loggers;

public MlResult<string> LeerConTraza(string ruta)
    => _io.ReadAllText(ruta)
          .LogMlResultDebugIfValid(_logger, t => $"Leído '{ruta}' ({t.Length} caracteres)")
          .LogMlResultErrorIfFail (_logger, e => $"No se pudo leer '{ruta}': {e.ToErrorsDescription()}");
```

### Ejemplo 5 — Sustituir en tests sin tocar disco

```csharp
public class ProcesadorInformesTests
{
    private sealed class WrapperIOFalso(string contenido) : IWrapperIO
    {
        public MlResult<string> ReadAllText(string filePathStr) => contenido;

        public MlResult<IEnumerable<string>> ReadAllLines(string filePathStr)
            => MlResult<IEnumerable<string>>.Valid(contenido.Split(Environment.NewLine));

        public MlResult<IEnumerable<ExistsFile>> EnumerateFiles(string directoryStr)
            => MlResult<IEnumerable<ExistsFile>>.Valid([]);
    }

    [Fact]
    public void Procesar_con_json_invalido_devuelve_fail()
    {
        var sut = new ProcesadorInformes(new WrapperIOFalso("esto no es json"));

        var resultado = sut.Procesar("cualquiera.json");

        resultado.IsFail.Should().BeTrue();
    }
}
```

> 💡 **Este es el motivo principal para usar `IWrapperIO`** en lugar de `File` directamente: el test no toca el sistema de ficheros.

### Ejemplo 6 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: enumerar fuera del raíl → la excepción escapa
var lineas = _io.ReadAllLines(ruta);
foreach (var l in lineas.Value) Procesar(l);      // 💥 IOException posible aquí

// ✅ BIEN: materializar dentro del raíl
var lineas = _io.ReadAllLines(ruta)
                .TryMap(l => l.ToList(), "Error al leer las líneas");


// ❌ MAL: dar por hecho que un Valid significa "leído sin problemas"
if (_io.EnumerateFiles(carpeta).IsValid) { /* nada se ha leído todavía */ }

// ✅ BIEN: materializar y comprobar
var ficheros = _io.EnumerateFiles(carpeta).TryMap(f => f.ToList(), "Error al enumerar");


// ❌ MAL: esperar que un fichero sin permisos devuelva Fail
var texto = _io.ReadAllText(rutaProtegida);       // 💥 UnauthorizedAccessException

// ✅ BIEN: envolver
var texto = MlResult.TryMap(() => _io.ReadAllText(rutaProtegida), "Sin acceso al fichero")
                    .Bind(r => r);
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Leer un fichero completo | `_io.ReadAllText(ruta)` ✅ el más seguro |
| Leer línea a línea | `_io.ReadAllLines(ruta)` + `TryMap(l => l.ToList(), …)` |
| Listar ficheros de una carpeta | `_io.EnumerateFiles(carpeta)` + `TryMap(f => f.ToList(), …)` |
| Filtrar por extensión | `.Map(f => f.Where(x => x.Value.EndsWith(".csv")))` |
| Escribir un fichero | ❌ No disponible: [extiéndelo](#cómo-extenderlo) |
| Copiar, mover, borrar | ❌ No disponible |
| Lectura asíncrona | ❌ No disponible: [extiéndelo](#cómo-extenderlo) |
| Recorrer subcarpetas | ❌ No disponible |
| Una codificación concreta | ❌ Usa `ExistsFile.ByString` + `TryMap` con `File.ReadAllText(ruta, encoding)` |
| Solo validar que existe | `ExistsFile.ByString(ruta)` de [`ValueObjects.IO`](../MoralesLarios.OOFP.ValueObjects.IO/README.md) |
| Cubrir permisos, bloqueos, etc. | Envolver la llamada en `MlResult.TryMap(…)` |

---

## Mejores prácticas

1. **Materializa siempre los `IEnumerable`** que devuelven `ReadAllLines` y `EnumerateFiles`: son perezosos y las excepciones escapan del raíl.
2. **Envuelve en `TryMap` si necesitas cubrir algo más que "no existe"**: permisos, bloqueos y rutas largas siguen lanzando.
3. **`ReadAllText` es el método más seguro**: es el único que lee de forma inmediata.
4. **Depende de `IWrapperIO`, no de `WrapperIO`**: es la razón de ser de la interfaz (tests sin disco).
5. **Extiende con tu propio wrapper** si necesitas escritura o asincronía; el patrón es de tres líneas.
6. **Al escribir, valida el directorio padre** (`ExistDirectory`), no el fichero destino: aún no existe.
7. **Combina con [`Utilities`](../MoralesLarios.OOFP.Utilities/README.md)** para leer las rutas de configuración, y añade `BindEnsure` contra rutas vacías.
8. **Filtra con LINQ tras `EnumerateFiles`**: no hay `searchPattern`.
9. **No asumas UTF-8** si los ficheros vienen de sistemas antiguos: usa `File.ReadAllText(ruta, encoding)` con `ExistsFile.ByString` a mano.
10. **En carpetas con concurrencia, materializa cuanto antes**: la enumeración perezosa combinada con `ExistsFile.FromString` puede lanzar a mitad del recorrido.
11. **Combina con [`Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md)** para trazar qué ruta falló.
12. **No lo uses para ficheros muy grandes** sin pensarlo: `ReadAllText` los carga completos en memoria.

---

## Resumen

- El proyecto **más pequeño de la solución**: tres ficheros y una interfaz, `IWrapperIO`, con **tres métodos de solo lectura**.
- El patrón que aplica: **validar la ruta con un objeto de valor** (`ExistsFile` / `ExistDirectory`) y **solo entonces** ejecutar la operación de `System.IO`, gracias al cortocircuito del `Map`.
- `EnumerateFiles` devuelve **`ExistsFile`**, no `string`: el tipo transporta la garantía de existencia.
- Registro: `services.AddOOFPIO()` como `Singleton`. ⚠️ El nombre **rompe la convención `AddMl…`** del resto de la solución.
- ⚠️ **Dos de los tres métodos son perezosos** (`File.ReadLines` y `Directory.EnumerateFiles`): el `MlResult` es `Valid` **antes de leer nada**, y una excepción durante la enumeración **escapa del raíl**. **Materializa con `TryMap(x => x.ToList(), …)`.**
- ⚠️ **Solo se convierte en `Fail` el caso "no existe"**: permisos, bloqueos, rutas largas y codificaciones siguen lanzando excepciones.
- ⚠️ **No incluye escritura, ni operaciones de gestión, ni métodos asíncronos, ni filtros, ni control de codificación, ni tests.** Es un punto de partida ampliable, no una biblioteca de E/S completa.
- Su mayor valor práctico: **poder sustituirlo por un doble en los tests** y no depender de los métodos estáticos de `File`.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.ValueObjects.IO`](../MoralesLarios.OOFP.ValueObjects.IO/README.md) — 🔑 `ExistsFile` y `ExistDirectory`, la base de la validación
- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — `NotEmptyString`, del que heredan
- [`MoralesLarios.OOFP.Utilities`](../MoralesLarios.OOFP.Utilities/README.md) — leer de configuración las rutas que se pasan aquí
- [`MoralesLarios.OOFP.Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md) — trazar los fallos de E/S
- [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) — otro wrapper de infraestructura con el mismo enfoque

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — mensajes y detalles del error](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`Map` — transformación con cortocircuito](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`TryMap` — capturar excepciones en el raíl](../MoralesLarios.FOOP/__Doc/Map/1_Map.md) 🔑 imprescindible aquí
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`EnsureFp` — validaciones de guarda](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`FusionErrosIfExists` — procesar colecciones acumulando errores](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md)
