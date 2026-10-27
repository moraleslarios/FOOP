---
mode: agent
description: Reformatea c�digo C# siguiendo el estilo "FP-aligned" del proyecto MoralesLarios.OOFP (alineaci�n por columnas, cadenas fluent alineadas, named arguments alineados, etc.).
---

# /fp-format � Estilo de formato del proyecto MoralesLarios.OOFP

Cuando el usuario invoque `/fp-format` (o pida expl�citamente "formatea con fp-format" / "aplica el estilo OOFP") sobre una clase, archivo, selecci�n o proyecto, **debes reformatear el c�digo respetando ESTRICTAMENTE las reglas siguientes**.

> ?? Reglas s�lo de formato. **No** cambies nombres, no a�adas/quites c�digo, no modifiques sem�ntica. **No** ejecutes el formateador del IDE: el formateador est�ndar **rompe** estas alineaciones, as� que las reglas se aplican manualmente.
>
> Idioma: comentarios y mensajes existentes se preservan tal cual.

---

## 1. Cabecera de archivo y namespace

- **No a�adas autom�ticamente** la cabecera de copyright. Si ya existe en el archivo, **pres�rvala tal cual** (no la modifiques, no la dupliques). Si falta, **no la a�adas** � ese es trabajo de otra herramienta o del autor humano.
- Usa **namespace file-scoped**: `namespace MoralesLarios.OOFP.X;` seguido (con o sin l�nea en blanco) por la declaraci�n de tipo.
- Es habitual dejar **2 o 3 l�neas en blanco** entre regiones l�gicas, entre miembros de tipo y al inicio del cuerpo de la clase.

## 2. Par�metros de m�todo � alineaci�n en columnas

Cuando un m�todo tenga **m�s de un par�metro**, o el usuario lo pida expl�citamente:

- Cada par�metro va en su propia l�nea.
- **El tipo m�s largo de la lista lleva exactamente UN espacio antes de su nombre** (ni m�s, ni menos). Esa posici�n fija la **columna del nombre** para toda la lista.
- **Los dem�s tipos se rellenan a la derecha con espacios** hasta que su nombre quede exactamente en la misma columna que el del tipo m�s largo. Resultado: **todos los nombres de par�metro alineados verticalmente** y **un �nico espacio entre el tipo m�s largo y su nombre**.
- Las l�neas 2..N de la lista de par�metros se sangran hasta justo despu�s del `(` del m�todo (alineadas con el primer par�metro).
- El par�ntesis de cierre `)` va al final del �ltimo par�metro.
- Si hay valores por defecto, los `=` tambi�n se alinean (regla an�loga: el `=` del valor m�s a la izquierda fija la columna y los dem�s se padean hasta esa columna).

> ? **Antipatr�n frecuente**: dejar 2 � 3 espacios entre el tipo m�s largo y su nombre "para respirar". **Est� prohibido** � debe ser exactamente 1 espacio. Si lo dejas mayor, las cadenas fluent y named-args que se construyen sobre estos par�metros se desalinean.

Ejemplo (tipo m�s largo `Func<T, MlResult<TReturn>>` ? 1 espacio antes de `func`; los dem�s se padean):

```csharp
public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T>                source,
                                                         Func<T, MlResult<TReturn>> func,
                                                         string                     exceptionAditionalMessage = null!)
```

Ejemplo con defaults � tipo m�s largo `Dictionary<string, string>` lleva 1 espacio; nombres y `=` alineados:

```csharp
public virtual Task<MlResult<TDto>> GetByIdAsync(NotEmptyString             idStr,
                                                 Dictionary<string, string> headers = null!,
                                                 CancellationToken          ct      = default)
```

Ejemplo con extension method de 5 par�metros (tipo m�s largo `Func<TSource, TInner, TResult>` con 1 espacio antes de `result`):

```csharp
public static IEnumerable<TResult> FullOuterJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource>           source,
                                                                                      IEnumerable<TInner>            inner,
                                                                                      Func<TSource, TKey>            pk,
                                                                                      Func<TInner, TKey>             fk,
                                                                                      Func<TSource, TInner, TResult> result)
```

### 2.1. Modificadores `this` y `params` � columnas reservadas

Cuando una lista de par�metros contiene los modificadores `this` (extension methods) y/o `params`, **estos modificadores ocupan una columna propia al inicio** de la l�nea de su par�metro. El resto de l�neas de la lista **rellenan con espacios** esa columna para que **el tipo siga alineado verticalmente**:

- `this   ` ocupa **5 espacios** (longitud de `"this "`). Las dem�s l�neas insertan **5 espacios** al inicio (entre el `(` y el tipo).
- `params ` ocupa **7 espacios** (longitud de `"params "`). Las dem�s l�neas insertan **7 espacios** al inicio.
- Si conviven (t�picamente `this` en el primer par�metro y `params` en el �ltimo), se reserva la columna del **modificador m�s ancho** (`params`, 7) para el inicio del tipo, y el `this` queda alineado dentro de esos 7 caracteres.
- Las reglas de alineaci�n de �2 (nombres y `=` por defecto en columnas) se siguen aplicando **despu�s** del prefijo de modificador.

Ejemplo con `params` al final (las l�neas no-`params` insertan 7 espacios al inicio):

```csharp
public Task<MlResult<TDto>> DeleteProblemDetailsAsync(       MlErrorDetails               notFoundErrorDetails,
                                                             CancellationToken             ct                  = default!,
                                                             string                        initialMessage      = null!,
                                                             Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                      params object[]                      pk)
```

Ejemplo con `this` al inicio (las l�neas posteriores insertan 5 espacios):

```csharp
public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                            Func<T, bool>                    condition,
                                                                            Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                            Func<T,      MlResult<TReturn>>  funcFalse,
                                                                            Func<Exception, string>          errorMessageBuilder)
```

Observa adem�s, en este �ltimo ejemplo, el padding **interno** del gen�rico: `Func<T,      MlResult<TReturn>>` lleva espacios extra **detr�s de la coma interna** para que su anchura visual coincida con la del m�s largo (`Func<T, Task<MlResult<TReturn>>>`). Esa t�cnica de "padding intra-gen�rico" es v�lida y preferible cuando padear s�lo el final del tipo no permite alinear el nombre del par�metro.

## 3. Constructor primario, gen�ricos, restricciones (`where`) e implementaci�n de interfaces

Para tipos que usan **constructor primario** (C# 12+) con inyecci�n de dependencias y/o par�metros gen�ricos:

- **Sintaxis del constructor primario**: `public class Foo<TGenerics>(par�metros...)`. **No hay espacio entre `>` y `(`** ni entre el nombre del tipo y `(` cuando no hay gen�ricos.
- **Par�metros del constructor primario (DI)**: aplica �ntegramente �2 (un par�metro por l�nea si hay m�s de uno, tipos padded para alinear nombres). Es habitual prefijar los nombres con `_` (`_repo`, `_logger`, `_httpClientFactoryManager`) porque el constructor primario los expone como campos accesibles desde el cuerpo del tipo.
- **Implementaci�n de interfaces / herencia**: `: IInterface<...>` (o `: BaseClass`) se escribe **al final de la l�nea del �ltimo par�metro**, justo despu�s del `)` de cierre del constructor primario. **No** se pone en una l�nea aparte salvo que la lista de bases sea muy larga.
- **Cl�usulas `where`**: cada una en su propia l�nea, **sangradas un nivel** (4 espacios respecto a la declaraci�n del tipo/m�todo), **antes** de la `{` de apertura del cuerpo. Si hay varias, los `:` se alinean en columna padeando los nombres gen�ricos cortos (`TDto    :` vs `TEntity :`).
- **`{` de apertura**: en su propia l�nea, alineada con el `public class ...` (sin sangr�a adicional).

Ejemplo can�nico (clase con constructor primario + DI + gen�ricos restringidos + interfaz):

```csharp
public class GenServiceFp<TEntity, TDto>(IEFRepoFp<TEntity>                   _repo,
                                         ILogger<GenServiceFp<TEntity, TDto>> _logger) : IGenServiceFp<TEntity, TDto>
    where TEntity : class
    where TDto    : class
{
    // cuerpo: _repo y _logger se usan directamente
}
```

Ejemplo con s�lo DI (sin gen�ricos, sin `where`):

```csharp
public class GenClientFp<TDto>(ILogger<GenClientFp<TDto>> _logger,
                               IHttpClientFactoryManager  _httpClientFactoryManager,
                               Key                        _httpClientFactoryKey) : IGenClientFp<TDto>
{
    // cuerpo
}
```

Notas:
- Si el constructor primario tiene un �nico par�metro corto, **d�jalo en una sola l�nea** con el `: IInterface` al final.
- Si el nombre del tipo + gen�ricos hace que el primer par�metro empiece en una columna profunda, los par�metros 2..N se sangran hasta esa misma columna (regla general de �2).
- No mezcles constructor primario con un constructor expl�cito redundante salvo que el c�digo existente ya lo haga.

## 4. Cadenas fluent � alineaci�n de puntos y par�ntesis

En cadenas tipo LINQ / `Bind` / `Map` / `Match` / `Log...`:

- Cada llamada va en su propia l�nea, comenzando con `.`.
- **Todos los `.` quedan alineados verticalmente** (sangrados a la columna del receptor o del primer `.`).
- **Los nombres de m�todo se rellenan con espacios** hasta la longitud del m�s largo de la cadena, de modo que **todos los `(` tambi�n quedan alineados**.
- Los argumentos lambda (`x =>`, `_ =>`, `errors =>`, `bdData =>`) se padean con espacios para que las flechas `=>` queden alineadas dentro de la cadena.

Ejemplo can�nico:

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

Observa: `.BindAsync  `, `.TryMapAsync`, `.MapAsync   `, `.LogMlResultFinalAsync` � todas terminan con `(` en la misma columna.

### 4.1. Alineaci�n interna del receptor cuando varias lambdas llaman al **mismo m�todo**

Cuando dentro de una cadena fluent **varias lambdas consecutivas** invocan el **mismo m�todo** (o uno muy similar) sobre **receptores distintos**, esos receptores **se padean con espacios** para que el **`.` que precede al m�todo compartido quede alineado verticalmente** en todas las l�neas. Resultado: adem�s de los `.` y `(` de la cadena externa, se forma una **segunda columna** dentro de las lambdas (la del m�todo compartido).

Regla pr�ctica: el receptor m�s largo fija la columna; los dem�s se rellenan a la derecha con espacios hasta esa misma anchura, y a continuaci�n va `.M�todoCompartido(...)` exactamente igual en todas las l�neas.

Ejemplo can�nico (5 llamadas a `RemoveDataAndCreateHistoricalAsync` sobre 5 servicios distintos):

```csharp
var result = await _logger.LogMlResultInformationAsync($"CleanAllWithHisto inicio: ...")
                          .TryBindAsync( _     => _serviceCashFlowScheduleLoader.RemoveDataAndCreateHistoricalAsync(ct))
                          .TryBindAsync( _     => _serviceFeeScheduleLoader     .RemoveDataAndCreateHistoricalAsync(ct))
                          .TryBindAsync( _     => _serviceStrikeScheduleLoader  .RemoveDataAndCreateHistoricalAsync(ct))
                          .TryBindAsync( _     => _serviceDerivadosOtcLoader    .RemoveDataAndCreateHistoricalAsync(ct))
                          .TryBindAsync( _     => _serviceCol_ValLoader         .RemoveDataAndCreateHistoricalAsync(ct))
                          .MapAsync    (items => items)
                          .LogMlResultFinalAsync(logger           : _logger,
                                                 validBuildMessage: items         => $"... ejecutado, {items} registros archivados",
                                                 failBuildMessage : errorsDetails => $"... Error: {errorsDetails.ToErrorsDetailsDescription()}");
```

Observa c�mo `_serviceCashFlowScheduleLoader`, `_serviceFeeScheduleLoader  ` (+5 espacios), `_serviceStrikeScheduleLoader` (+2), etc., quedan padeados para que **todos los `.RemoveDataAndCreateHistoricalAsync(ct)` arranquen en la misma columna**.

Cu�ndo aplicar:
- Hay **2 � m�s** lambdas consecutivas en la cadena que llaman al **mismo m�todo** (id�ntico nombre y misma forma de argumentos), o a un grupo de m�todos casi id�nticos del mismo tipo (`.RemoveX`, `.RemoveY` con misma anchura).
- Los receptores son referencias (campos `_xxx`, propiedades, par�metros) cuya longitud difiere.

Cu�ndo **no** aplicar:
- Si las llamadas son a m�todos distintos sobre el mismo receptor ? usa la regla normal de �4 (alinear los `.` exteriores).
- Si la cadena tiene una sola llamada de ese tipo ? no fuerces columna.
- Si los receptores tienen longitudes muy dispares (>15 caracteres de diferencia) y crear la columna desplazar�a tanto las cortas que rompe la legibilidad ? da prioridad a �4 y deja la columna interna sin alinear, comentando si procede.

## 5. Argumentos con nombre � `:` alineado

Cuando se invoca un m�todo con **dos o m�s** argumentos con nombre en multil�nea, **alinea los `:` en columna**. La regla precisa de espaciado es:

- **Antes del `:`**:
  - El **nombre m�s largo** lleva el `:` **pegado** (sin espacio antes). Ese nombre fija la columna del `:`.
  - Los nombres m�s cortos se **rellenan a la derecha con espacios** hasta esa columna, y entonces va el `:`.
- **Despu�s del `:`**: **siempre exactamente 1 espacio**, despu�s el valor.
- Los **valores** (lo que hay tras `: `) **no se alinean en columna** entre s�, simplemente arrancan tras el espacio.

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

> ? **Antipatr�n**: dejar espacio antes del `:` en el nombre m�s largo (`notFoundErrorDetails :`). El nombre m�s largo siempre lleva el `:` pegado; lo que se padea es el espacio **antes** del `:` en los nombres cortos.

### 5.1. Argumentos con nombre dentro de invocaciones anidadas en lambdas de cadenas fluent

Cuando los argumentos con nombre aparecen en una **invocaci�n multil�nea anidada dentro de una lambda** que vive en una cadena fluent (patr�n t�pico `.BindAsync(_ => receptor.Method(arg1: ..., arg2: ..., ...))`), aplican las mismas reglas de �5 **dentro de esa invocaci�n**, con dos precisiones:

- La **columna de continuaci�n** (donde arranca cada argumento de la l�nea 2..N) se alinea **bajo el `(` del m�todo invocado** (`receptor.Method(`), **no** bajo el `.` de la cadena fluent ni bajo el `_ =>` de la lambda.
- Cada invocaci�n anidada calcula **su propia columna de `:`** seg�n su propio nombre m�s largo. No se intenta unificar columnas de distintas invocaciones.

Ejemplo can�nico (`HttpClientFactoryManager.PostAsync` con 4 argumentos con nombre):

```csharp
var result = await _logger.LogMlResultInformationAsync("Creando un nuevo Col Val.")
                          .BindAsync(_ => _httpClientFactoryManager.PostAsync(httpClientFactoryKey: ApiClientKey,
                                                                              url                 : "api/RegulatoryIntakeEtl/FeeSchedule",
                                                                              itemBody            : item,
                                                                              ct                  : cancellationToken))
                          .LogMlResultFinalDescriptionAsync(_logger, nameof(CreateDataAsync));
```

Observa:

- `httpClientFactoryKey` (nombre m�s largo dentro de `PostAsync`) lleva el `:` pegado.
- `url`, `itemBody`, `ct` se padean con espacios hasta esa misma columna.
- Las l�neas 2..N arrancan bajo el `(` de `PostAsync(`, **no** bajo el `(` de `.BindAsync(`.
- Tras `: ` siempre 1 espacio, luego el valor (que puede ser corto o largo, da igual).

Si el m�todo anidado tiene adem�s **named args en otras invocaciones** de la misma cadena, **cada una** mantiene su propia columna de `:` independientemente. No fuerces una columna global.

## 6. `Match` / `MatchAsync` multil�nea

- El `(` va en su propia l�nea, alineado con el `.Match` (mismo nivel del `=>` o del receptor).
- Las ramas `fail`/`valid` (o `failAsync`/`validAsync`) se alinean en columna; el `:` tambi�n.
- El `)` final en su propia l�nea, alineado con el `(`.

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

Nota c�mo los modificadores (`async`) y los nombres de lambda (`errorsDetails`, `value`) tambi�n se padean para alinear `=>`.

Variante "indentada estilo m�todo" (tambi�n v�lida y usada en el repo):

```csharp
=> source.Match(
                    valid: x      => { actionValid(x)    ; return source; },
                    fail : errors => { actionFail(errors); return source; }
                );
```

Aqu� el `(` queda pegado al m�todo y el contenido se sangra hasta alinear con el primer argumento. Mant�n el estilo que ya use el archivo.

## 7. Sobrecargas est�ticas / factory consecutivas

Cuando hay m�ltiples sobrecargas est�ticas seguidas, **se alinean por columnas** sus modificadores, tipo de retorno, nombre, lista de par�metros y, si son de cuerpo expresi�n, tambi�n el cuerpo tras `=>`:

```csharp
public static MlResult<T> Fail<T>(params MlError[]            errors)        => new(errors);
public static MlResult<T> Fail<T>(       MlErrorDetails      errorsDetails) => new(errorsDetails);
public static MlResult<T> Fail<T>(       IEnumerable<MlError> errors       ) => new(errors);
```

Cuando un par�metro es `params` y otros no, los que no lo son llevan **espacios en lugar de `params`** para mantener la columna del tipo.

## 8. Constructores con asignaci�n por tupla

Cuando varios constructores asignan por deconstrucci�n, **alinea las tuplas izquierda y derecha en columnas**:

```csharp
internal MlResult(T                    t                                                        ) => (Value, ErrorsDetails, IsSuccess) = (t       , new MlErrorDetails()                                    , true );
internal MlResult(MlErrorDetails                                         errorsDetails         ) => (Value, ErrorsDetails, IsSuccess) = (default!, errorsDetails                                            , false);
internal MlResult(IEnumerable<MlError> errors, Dictionary<string, object> details        = null!) => (Value, ErrorsDetails, IsSuccess) = (default!, new MlErrorDetails(errors, details)                     , false);
internal MlResult(MlError              error , Dictionary<string, object> details        = null!) => (Value, ErrorsDetails, IsSuccess) = (default!, new MlErrorDetails(new List<MlError> { error }, details), false);
```

Cada coma, par�ntesis y `=` se alinea verticalmente con los de las otras l�neas.

## 9. Operadores impl�citos / expl�citos

Apilados verticalmente y alineados igual que las sobrecargas:

```csharp
public static implicit operator MlResult<T>(List<MlError> errors) => Fail(errors.AsEnumerable());
public static implicit operator MlResult<T>(MlError[]     errors) => Fail(errors);
public static implicit operator MlResult<T>(MlError       error ) => Fail(error);
```

## 10. Discard `_` en lambdas � espacio tras el par�ntesis

Cuando una lambda use el discard `_` como par�metro y vaya entre par�ntesis, **debe haber un espacio entre el `(` y el `_`** (y, normalmente, tambi�n padding tras el `_` para alinear los `=>` de la cadena).

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

La regla aplica tambi�n a discards en argumentos individuales que no est�n en cadena fluent: `Foo( _ => bar)`.

## 11. Negaci�n `!` � espacios a ambos lados

El operador de negaci�n l�gica `!` se escribe **rodeado de espacios**, tanto del lado del par�ntesis/operando como del propio identificador:

```csharp
if ( ! variableBool)            { ... }
public bool IsFailure => ! IsSuccess;
return condition && ( ! source.IsSuccess);
=> That(value, ! string.IsNullOrWhiteSpace(value), errorMessage);
```

Incorrecto:

```csharp
if (!variableBool) { ... }      // ?
=> ! IsSuccess;                   // ? ok (al inicio de expresi�n tambi�n lleva espacio tras !)
=> !IsSuccess;                    // ?
```

Resumen:
- Tras `(` que precede a un `!`: **espacio** ? `( ! x)`.
- Entre `!` y su operando: **espacio** ? `! x`.
- No aplica a `!=` ni a `null!` / `default!` (postfix `!` del operador null-forgiving va pegado).

## 12. L�nea en blanco antes de `return` cuando la cadena fluent est� muy sangrada

Cuando dentro del cuerpo de un m�todo se construye un resultado mediante una **cadena fluent profundamente sangrada** y luego se hace `return <variable>;`, aplica esta regla:

- Calcula la **columna en la que arrancan los `.` de la cadena fluent** (el `.BindAsync`, `.MapAsync`, etc.) medida en espacios desde el inicio de la l�nea.
- Si esa columna es **mayor que 30 espacios**, **elimina la l�nea en blanco** que pueda existir entre la �ltima l�nea de la cadena fluent (la que termina con `;`) y la sentencia `return result;` (o el `return` correspondiente).
- Si la columna es ? 30, mant�n el estilo existente (no fuerces ni a�adas/quites l�neas en blanco).

Correcto (cadena fluent en columna ~36, **sin** l�nea en blanco antes del `return`):

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

Incorrecto (cadena fluent muy sangrada **con** l�nea en blanco antes del `return`):

```csharp
                        .MapAsync   (bdData => bdData.Adapt<TDto>())
                        .LogMlResultFinalAsync(...);
                                                                          // ? l�nea en blanco sobrante
    return result;
```

Motivo: con sangr�as profundas la l�nea en blanco rompe la lectura visual del bloque "asignaci�n + return" como una sola unidad.

## 13. Otros detalles de estilo

- **Limpieza de `using` no utilizados**: elimina los `using` (de directiva top-level y de `global using` si aplica) que no se est�n usando en el archivo. Mant�n los que s� se usan respetando su orden actual; **no los reordenes** salvo que ya estuvieran en orden alfab�tico (en cuyo caso preserva ese orden tras la limpieza). No toques `using static` ni alias `using X = Y;` salvo que sean claramente no utilizados.
- Espacio antes y despu�s de `=>` en lambdas y de `=` en defaults.
- Comentarios XML (`/// <summary>`) por encima del m�todo p�blico sin alteraciones.
- Bloques `#region` / `#endregion` se conservan; deja **una l�nea en blanco** dentro tras `#region` y antes de `#endregion`.
- Varias l�neas en blanco (2�3) entre miembros l�gicamente separados son intencionales: **no las colapses**.
- `null!` y `default!` se usan tal cual; no los modifiques.
- No metas ramas de `Match` en una sola l�nea si el resto del archivo las pone multilinea.

---

## Procedimiento que debes seguir al ejecutar `/fp-format`

1. Si no hay archivo/selecci�n clara, pregunta a qu� clase o proyecto aplicar.
2. Lee el archivo objetivo entero antes de editar (las alineaciones dependen del par�metro/lambda m�s largo del bloque).
3. Para cada bloque de par�metros, calcula la columna objetivo = longitud del tipo m�s largo + 1 espacio m�nimo. Aplica padding con espacios (nunca tabs).
4. Para cadenas fluent: localiza la longitud m�xima del nombre de m�todo de la cadena, padea el resto con espacios antes del `(`. Despu�s padea los nombres de lambda hasta el m�s largo para alinear los `=>`.
5. Para argumentos con nombre: calcula longitud m�xima del nombre, padea el resto con espacios antes del `:`.
6. Para sobrecargas/constructores apilados consecutivos del mismo "grupo", tr�talos como una matriz: alinea cada token (modificador, tipo, nombre, `(`, cada par�metro, `)`, `=>`, cuerpo, `;`) en su columna.
7. Usa **siempre espacios**, nunca tabuladores.
8. No modifiques nada fuera del formato (ni nombres, ni l�gica). **S�** est� permitido eliminar `using` no utilizados (ver �13). **No** a�adas cabeceras de copyright nuevas (ver �1).
9. Tras formatear, indica brevemente qu� bloques se realinearon.

## C�mo resolver casos ambiguos

- Si un m�todo tiene un �nico par�metro corto, **d�jalo en una sola l�nea**.
- Si una cadena fluent tiene un �nico `.Foo(...)`, no fuerces apilado.
- Si el archivo ya usa una variante (p. ej. `Match(` pegado vs `Match` con `(` en l�nea aparte), **respeta la variante predominante en ese archivo**.
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

Si el c�digo de entrada respeta ya una parte de las reglas, **s�lo corrige lo que falte**; no reescribas todo desde cero.
