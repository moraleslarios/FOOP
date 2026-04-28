---
mode: agent
description: Reformatea código C# siguiendo el estilo "FP-aligned" del proyecto MoralesLarios.OOFP (alineación por columnas, cadenas fluent alineadas, named arguments alineados, etc.).
---

# /fp-format — Estilo de formato del proyecto MoralesLarios.OOFP

Cuando el usuario invoque `/fp-format` (o pida explícitamente "formatea con fp-format" / "aplica el estilo OOFP") sobre una clase, archivo, selección o proyecto, **debes reformatear el código respetando ESTRICTAMENTE las reglas siguientes**.

> ?? Reglas sólo de formato. **No** cambies nombres, no añadas/quites código, no modifiques semántica. **No** ejecutes el formateador del IDE: el formateador estándar **rompe** estas alineaciones, así que las reglas se aplican manualmente.
>
> Idioma: comentarios y mensajes existentes se preservan tal cual.

---

## 1. Cabecera de archivo y namespace

- Mantén siempre la cabecera de copyright si ya existe; si falta y el archivo está bajo `MoralesLarios.OOFP.*`, añádela:

  ```csharp
  // Copyright (c) 2023 Juan Francisco Morales Larios
  // moraleslarios@gmail.com
  // Licensed under the Apache License, Version 2.0
  ```

- Usa **namespace file-scoped**: `namespace MoralesLarios.OOFP.X;` seguido (con o sin línea en blanco) por la declaración de tipo.
- Es habitual dejar **2 o 3 líneas en blanco** entre regiones lógicas, entre miembros de tipo y al inicio del cuerpo de la clase.

## 2. Parámetros de método — alineación en columnas

Cuando un método tenga **más de un parámetro**, o el usuario lo pida explícitamente:

- Cada parámetro va en su propia línea.
- **El tipo de cada parámetro se rellena a la derecha con espacios** hasta que el nombre del parámetro de la línea más larga determine la columna; **todos los nombres de parámetro deben quedar alineados verticalmente en la misma columna**.
- Las líneas 2..N de la lista de parámetros se sangran hasta justo después del `(` del método (alineadas con el primer parámetro).
- El paréntesis de cierre `)` va al final del último parámetro.
- Si hay valores por defecto, los `=` también suelen quedar alineados.

Ejemplo:

```csharp
public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T>                source,
                                                         Func<T, MlResult<TReturn>> func,
                                                         string                     exceptionAditionalMessage = null!)
```

```csharp
public virtual Task<MlResult<TDto>> GetByIdAsync(NotEmptyString             idStr,
                                                 Dictionary<string, string> headers = null!,
                                                 CancellationToken          ct      = default)
```

### 2.1. Modificadores `this` y `params` — columnas reservadas

Cuando una lista de parámetros contiene los modificadores `this` (extension methods) y/o `params`, **estos modificadores ocupan una columna propia al inicio** de la línea de su parámetro. El resto de líneas de la lista **rellenan con espacios** esa columna para que **el tipo siga alineado verticalmente**:

- `this   ` ocupa **5 espacios** (longitud de `"this "`). Las demás líneas insertan **5 espacios** al inicio (entre el `(` y el tipo).
- `params ` ocupa **7 espacios** (longitud de `"params "`). Las demás líneas insertan **7 espacios** al inicio.
- Si conviven (típicamente `this` en el primer parámetro y `params` en el último), se reserva la columna del **modificador más ancho** (`params`, 7) para el inicio del tipo, y el `this` queda alineado dentro de esos 7 caracteres.
- Las reglas de alineación de §2 (nombres y `=` por defecto en columnas) se siguen aplicando **después** del prefijo de modificador.

Ejemplo con `params` al final (las líneas no-`params` insertan 7 espacios al inicio):

```csharp
public Task<MlResult<TDto>> DeleteProblemDetailsAsync(       MlErrorsDetails               notFoundErrorDetails,
                                                             CancellationToken             ct                  = default!,
                                                             string                        initialMessage      = null!,
                                                             Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                      params object[]                      pk)
```

Ejemplo con `this` al inicio (las líneas posteriores insertan 5 espacios):

```csharp
public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                            Func<T, bool>                    condition,
                                                                            Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                            Func<T,      MlResult<TReturn>>  funcFalse,
                                                                            Func<Exception, string>          errorMessageBuilder)
```

Observa además, en este último ejemplo, el padding **interno** del genérico: `Func<T,      MlResult<TReturn>>` lleva espacios extra **detrás de la coma interna** para que su anchura visual coincida con la del más largo (`Func<T, Task<MlResult<TReturn>>>`). Esa técnica de "padding intra-genérico" es válida y preferible cuando padear sólo el final del tipo no permite alinear el nombre del parámetro.

## 3. Constructor primario, genéricos, restricciones (`where`) e implementación de interfaces

Para tipos que usan **constructor primario** (C# 12+) con inyección de dependencias y/o parámetros genéricos:

- **Sintaxis del constructor primario**: `public class Foo<TGenerics>(parámetros...)`. **No hay espacio entre `>` y `(`** ni entre el nombre del tipo y `(` cuando no hay genéricos.
- **Parámetros del constructor primario (DI)**: aplica íntegramente §2 (un parámetro por línea si hay más de uno, tipos padded para alinear nombres). Es habitual prefijar los nombres con `_` (`_repo`, `_logger`, `_httpClientFactoryManager`) porque el constructor primario los expone como campos accesibles desde el cuerpo del tipo.
- **Implementación de interfaces / herencia**: `: IInterface<...>` (o `: BaseClass`) se escribe **al final de la línea del último parámetro**, justo después del `)` de cierre del constructor primario. **No** se pone en una línea aparte salvo que la lista de bases sea muy larga.
- **Cláusulas `where`**: cada una en su propia línea, **sangradas un nivel** (4 espacios respecto a la declaración del tipo/método), **antes** de la `{` de apertura del cuerpo. Si hay varias, los `:` se alinean en columna padeando los nombres genéricos cortos (`TDto    :` vs `TEntity :`).
- **`{` de apertura**: en su propia línea, alineada con el `public class ...` (sin sangría adicional).

Ejemplo canónico (clase con constructor primario + DI + genéricos restringidos + interfaz):

```csharp
public class GenServiceFp<TEntity, TDto>(IEFRepoFp<TEntity>                   _repo,
                                         ILogger<GenServiceFp<TEntity, TDto>> _logger) : IGenServiceFp<TEntity, TDto>
    where TEntity : class
    where TDto    : class
{
    // cuerpo: _repo y _logger se usan directamente
}
```

Ejemplo con sólo DI (sin genéricos, sin `where`):

```csharp
public class GenClientFp<TDto>(ILogger<GenClientFp<TDto>> _logger,
                               IHttpClientFactoryManager  _httpClientFactoryManager,
                               Key                        _httpClientFactoryKey) : IGenClientFp<TDto>
{
    // cuerpo
}
```

Notas:
- Si el constructor primario tiene un único parámetro corto, **déjalo en una sola línea** con el `: IInterface` al final.
- Si el nombre del tipo + genéricos hace que el primer parámetro empiece en una columna profunda, los parámetros 2..N se sangran hasta esa misma columna (regla general de §2).
- No mezcles constructor primario con un constructor explícito redundante salvo que el código existente ya lo haga.

## 4. Cadenas fluent — alineación de puntos y paréntesis

En cadenas tipo LINQ / `Bind` / `Map` / `Match` / `Log...`:

- Cada llamada va en su propia línea, comenzando con `.`.
- **Todos los `.` quedan alineados verticalmente** (sangrados a la columna del receptor o del primer `.`).
- **Los nombres de método se rellenan con espacios** hasta la longitud del más largo de la cadena, de modo que **todos los `(` también quedan alineados**.
- Los argumentos lambda (`x =>`, `_ =>`, `errors =>`, `bdData =>`) se padean con espacios para que las flechas `=>` queden alineadas dentro de la cadena.

Ejemplo canónico:

```csharp
var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"...")
                    .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                    .TryMapAsync( _     => dto.Adapt<TEntity>())
                    .BindAsync  (bdData => _repo.TryAddAsync(bdData, token: ct))
                    .MapAsync   (bdData => bdData.Adapt<TDto>())
                    .LogMlResultFinalAsync(logger           : _logger,
                                           validBuildMessage: x      => ...,
                                           failBuildMessage : errors => ...);
```

Observa: `.BindAsync  `, `.TryMapAsync`, `.MapAsync   `, `.LogMlResultFinalAsync` — todas terminan con `(` en la misma columna.

## 5. Argumentos con nombre — `:` alineado

Cuando se invoca un método con argumentos con nombre, **alinea los `:`** en columna y deja un único espacio antes y después:

```csharp
.LogMlResultFinalAsync(logger           : _logger,
                       validBuildMessage: x      => ...,
                       failBuildMessage : errors => ...);
```

```csharp
=> FindByIdProblemsDetailsAsync(ct                  : ct,
                                initialMessage      : initialMessage,
                                notFoundErrorDetails: BuildNotFoundPkError(...),
                                validMessageBuilder : validMessageBuilder,
                                failMessageBuilder  : failMessageBuilder,
                                pk                  : pk);
```

Regla práctica: si el nombre `x` es más corto que la columna objetivo, se pone `x  :` (espacios antes del `:`); si encaja justo, `xxxxxxxx:`. Lo importante es que **todos los `:` queden en la misma columna** y los **valores también** (después de `: `).

## 6. `Match` / `MatchAsync` multilínea

- El `(` va en su propia línea, alineado con el `.Match` (mismo nivel del `=>` o del receptor).
- Las ramas `fail`/`valid` (o `failAsync`/`validAsync`) se alinean en columna; el `:` también.
- El `)` final en su propia línea, alineado con el `(`.

```csharp
=> source.Match
(
    fail : MlResult<TReturn>.Fail,
    valid: value => func(value)
);
```

```csharp
=> await source.MatchAsync
(
    failAsync :       errorsDetails =>       Task.FromResult(MlResult<TReturn>.Fail(errorsDetails)),
    validAsync: async value         => await funcAsync(value)
);
```

Nota cómo los modificadores (`async`) y los nombres de lambda (`errorsDetails`, `value`) también se padean para alinear `=>`.

Variante "indentada estilo método" (también válida y usada en el repo):

```csharp
=> source.Match(
                    valid: x      => { actionValid(x)    ; return source; },
                    fail : errors => { actionFail(errors); return source; }
                );
```

Aquí el `(` queda pegado al método y el contenido se sangra hasta alinear con el primer argumento. Mantén el estilo que ya use el archivo.

## 7. Sobrecargas estáticas / factory consecutivas

Cuando hay múltiples sobrecargas estáticas seguidas, **se alinean por columnas** sus modificadores, tipo de retorno, nombre, lista de parámetros y, si son de cuerpo expresión, también el cuerpo tras `=>`:

```csharp
public static MlResult<T> Fail<T>(params MlError[]            errors)        => new(errors);
public static MlResult<T> Fail<T>(       MlErrorsDetails      errorsDetails) => new(errorsDetails);
public static MlResult<T> Fail<T>(       IEnumerable<MlError> errors       ) => new(errors);
```

Cuando un parámetro es `params` y otros no, los que no lo son llevan **espacios en lugar de `params`** para mantener la columna del tipo.

## 8. Constructores con asignación por tupla

Cuando varios constructores asignan por deconstrucción, **alinea las tuplas izquierda y derecha en columnas**:

```csharp
internal MlResult(T                    t                                                        ) => (Value, ErrorsDetails, IsValid) = (t       , new MlErrorsDetails()                                    , true );
internal MlResult(MlErrorsDetails                                         errorsDetails         ) => (Value, ErrorsDetails, IsValid) = (default!, errorsDetails                                            , false);
internal MlResult(IEnumerable<MlError> errors, Dictionary<string, object> details        = null!) => (Value, ErrorsDetails, IsValid) = (default!, new MlErrorsDetails(errors, details)                     , false);
internal MlResult(MlError              error , Dictionary<string, object> details        = null!) => (Value, ErrorsDetails, IsValid) = (default!, new MlErrorsDetails(new List<MlError> { error }, details), false);
```

Cada coma, paréntesis y `=` se alinea verticalmente con los de las otras líneas.

## 9. Operadores implícitos / explícitos

Apilados verticalmente y alineados igual que las sobrecargas:

```csharp
public static implicit operator MlResult<T>(List<MlError> errors) => Fail(errors.AsEnumerable());
public static implicit operator MlResult<T>(MlError[]     errors) => Fail(errors);
public static implicit operator MlResult<T>(MlError       error ) => Fail(error);
```

## 10. Discard `_` en lambdas — espacio tras el paréntesis

Cuando una lambda use el discard `_` como parámetro y vaya entre paréntesis, **debe haber un espacio entre el `(` y el `_`** (y, normalmente, también padding tras el `_` para alinear los `=>` de la cadena).

Correcto:

```csharp
.BindAsync  ( _     => EnsureFp.NotNull(dto, "..."))
.TryMapAsync( _     => dto.Adapt<TEntity>())
.Map        ( _     => "ok")
```

Incorrecto:

```csharp
.BindAsync  (_ => EnsureFp.NotNull(dto, "..."))   // ? falta el espacio tras '('
.BindAsync  (_     => EnsureFp.NotNull(dto, "...")) // ? falta el espacio tras '('
```

La regla aplica también a discards en argumentos individuales que no están en cadena fluent: `Foo( _ => bar)`.

## 11. Negación `!` — espacios a ambos lados

El operador de negación lógica `!` se escribe **rodeado de espacios**, tanto del lado del paréntesis/operando como del propio identificador:

```csharp
if ( ! variableBool)            { ... }
public bool IsFail => ! IsValid;
return condition && ( ! source.IsValid);
=> That(value, ! string.IsNullOrWhiteSpace(value), errorMessage);
```

Incorrecto:

```csharp
if (!variableBool) { ... }      // ?
=> ! IsValid;                   // ? ok (al inicio de expresión también lleva espacio tras !)
=> !IsValid;                    // ?
```

Resumen:
- Tras `(` que precede a un `!`: **espacio** ? `( ! x)`.
- Entre `!` y su operando: **espacio** ? `! x`.
- No aplica a `!=` ni a `null!` / `default!` (postfix `!` del operador null-forgiving va pegado).

## 12. Línea en blanco antes de `return` cuando la cadena fluent está muy sangrada

Cuando dentro del cuerpo de un método se construye un resultado mediante una **cadena fluent profundamente sangrada** y luego se hace `return <variable>;`, aplica esta regla:

- Calcula la **columna en la que arrancan los `.` de la cadena fluent** (el `.BindAsync`, `.MapAsync`, etc.) medida en espacios desde el inicio de la línea.
- Si esa columna es **mayor que 30 espacios**, **elimina la línea en blanco** que pueda existir entre la última línea de la cadena fluent (la que termina con `;`) y la sentencia `return result;` (o el `return` correspondiente).
- Si la columna es ? 30, mantén el estilo existente (no fuerces ni añadas/quites líneas en blanco).

Correcto (cadena fluent en columna ~36, **sin** línea en blanco antes del `return`):

```csharp
public Task<MlResult<TDto>> CreateAsync(TDto                          dto,
                                        CancellationToken             ct                  = default!,
                                        ...)
{
    var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"...")
                        .BindAsync  ( _     => EnsureFp.NotNull(dto, $"..."))
                        .TryMapAsync( _     => dto.Adapt<TEntity>())
                        .BindAsync  (bdData => _repo.TryAddAsync(bdData, token: ct))
                        .MapAsync   (bdData => bdData.Adapt<TDto>())
                        .LogMlResultFinalAsync(logger           : _logger,
                                               validBuildMessage: x      => ...,
                                               failBuildMessage : errors => ...);
    return result;
}
```

Incorrecto (cadena fluent muy sangrada **con** línea en blanco antes del `return`):

```csharp
                        .MapAsync   (bdData => bdData.Adapt<TDto>())
                        .LogMlResultFinalAsync(...);
                                                                          // ? línea en blanco sobrante
    return result;
```

Motivo: con sangrías profundas la línea en blanco rompe la lectura visual del bloque "asignación + return" como una sola unidad.

## 13. Otros detalles de estilo

- Espacio antes y después de `=>` en lambdas y de `=` en defaults.
- Comentarios XML (`/// <summary>`) por encima del método público sin alteraciones.
- Bloques `#region` / `#endregion` se conservan; deja **una línea en blanco** dentro tras `#region` y antes de `#endregion`.
- Varias líneas en blanco (2–3) entre miembros lógicamente separados son intencionales: **no las colapses**.
- `null!` y `default!` se usan tal cual; no los modifiques.
- No metas ramas de `Match` en una sola línea si el resto del archivo las pone multilinea.

---

## Procedimiento que debes seguir al ejecutar `/fp-format`

1. Si no hay archivo/selección clara, pregunta a qué clase o proyecto aplicar.
2. Lee el archivo objetivo entero antes de editar (las alineaciones dependen del parámetro/lambda más largo del bloque).
3. Para cada bloque de parámetros, calcula la columna objetivo = longitud del tipo más largo + 1 espacio mínimo. Aplica padding con espacios (nunca tabs).
4. Para cadenas fluent: localiza la longitud máxima del nombre de método de la cadena, padea el resto con espacios antes del `(`. Después padea los nombres de lambda hasta el más largo para alinear los `=>`.
5. Para argumentos con nombre: calcula longitud máxima del nombre, padea el resto con espacios antes del `:`.
6. Para sobrecargas/constructores apilados consecutivos del mismo "grupo", trátalos como una matriz: alinea cada token (modificador, tipo, nombre, `(`, cada parámetro, `)`, `=>`, cuerpo, `;`) en su columna.
7. Usa **siempre espacios**, nunca tabuladores.
8. No modifiques nada fuera del formato (ni `using`s salvo orden alfabético si ya estaba así, ni nombres, ni lógica).
9. Tras formatear, indica brevemente qué bloques se realinearon.

## Cómo resolver casos ambiguos

- Si un método tiene un único parámetro corto, **déjalo en una sola línea**.
- Si una cadena fluent tiene un único `.Foo(...)`, no fuerces apilado.
- Si el archivo ya usa una variante (p. ej. `Match(` pegado vs `Match` con `(` en línea aparte), **respeta la variante predominante en ese archivo**.
- Ante conflicto entre dos reglas, prioriza la **legibilidad por columnas** (que las cosas equivalentes se vean en la misma vertical).

---

## Ejemplo completo de salida esperada

```csharp
public static async Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this MlResult<T>                      source,
                                                                          Func<T, Task<MlResult<TReturn>>> funcAsync,
                                                                          Func<Exception, string>          errorMessageBuilder)
    => await source.MatchAsync
    (
        failAsync :       errorsDetails =>       MlResult<TReturn>.Fail(errorsDetails).ToAsync(),
        validAsync: async value         => await funcAsync.TryToMlResultAsync(source.Value, errorMessageBuilder)
    );
```

Si el código de entrada respeta ya una parte de las reglas, **sólo corrige lo que falte**; no reescribas todo desde cero.
