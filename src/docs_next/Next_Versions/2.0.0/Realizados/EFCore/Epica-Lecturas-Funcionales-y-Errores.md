# Épica · Captura predecible de errores en las lecturas EF Core

> **Versión destino** · `2.0.0`, pendiente de autorización del propietario; planificación no cambia el congelamiento
> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Origen** · análisis [03 · Modelo de error](../../../../Mejoras/EntityFrameworkCore/03-Modelo-de-Error.md) y [08 · Calidad de pruebas](../../../../Mejoras/EntityFrameworkCore/08-Calidad-Pruebas-y-Documentacion.md)
> **Estado** · ✅ Completada · US-01…03 implementadas y probadas; D-02 queda explícitamente diferida fuera del alcance
> **Esfuerzo estimado** · M (confianza baja; supuesto: conservar el API y ajustar materialización/error handling con tests existentes)
> **Riesgo** · 🔴 muy alto · dos rutas cambian comportamiento observable

> ⚠️ **Bloque de comportamiento** · decidir y publicar juntas la frontera de materialización síncrona y la respuesta a claves inválidas. Los consumidores pueden observar el cambio aunque el código compile igual.

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Hallazgos colaterales](#2-hallazgos-colaterales)
3. [Alcance](#3-alcance)
4. [Historias de usuario](#4-historias-de-usuario)
5. [Orden de ejecución](#5-orden-de-ejecución)
6. [Decisiones cerradas](#6-decisiones-cerradas)
7. [Definición de terminado](#7-definición-de-terminado)
8. [Apéndice · comportamiento actual y destino](#8-apéndice--comportamiento-actual-y-destino)

## 1. Contexto y problema

El contrato de `IEfReadRepository<T>` declara que las excepciones se convierten en `MlResult.Fail`, pero `TryGetData` y `TryAll` devuelven secuencias diferidas: el error de SQL puede aparecer cuando el consumidor enumera, fuera del rail protegido. Las variantes asíncronas sí materializan. Además, `TryFind` formatea las claves antes de validar el arreglo; `null` puede lanzar antes de devolver un resultado funcional. La prueba general de `TryAllAsync` comprueba no-nulidad, no el contenido.

| Hallazgo | Evidencia |
|---|---|
| #4 · consulta diferida y captura tardía | `EfReadRepository.cs:188-207,218-236`; [análisis 03](../../../../Mejoras/EntityFrameworkCore/03-Modelo-de-Error.md) |
| #5 · formateo de clave antes de guardas | `EfReadRepository.cs:45-46,73-74`; `Helpers/Extensions.cs:20-23`; rutas hermanas en `EfUpdateRepository.cs:62,129` y `EfDeleteRepository.cs:56,109` |
| #13 · test con oráculo débil | `Tests.Integration/Repos/RepoVinorFpTests.cs:64-73`; [análisis 08](../../../../Mejoras/EntityFrameworkCore/08-Calidad-Pruebas-y-Documentacion.md) |

## 2. Hallazgos colaterales

El hallazgo #13 es de aserción, no de producción, pero comparte la lectura `TryAllAsync` y se incorpora como regresión de contenido en US-02. No se encontraron otros hallazgos colaterales dentro del alcance.

## 3. Alcance

**Dentro**: fijar si las lecturas síncronas materializan dentro de `MlResult`; validar las claves antes de construir mensajes; reforzar el test de `TryAllAsync`; alinear XML docs de contratos implicados.

**Fuera**: modificar `TryGetLast` por rendimiento (épica separada); cambiar los errores de búsqueda de no encontrado que ya tienen contrato; capturar errores que ocurran después de devolver deliberadamente una consulta diferida, si se conserva ese contrato.

## 4. Historias de usuario

### US-01 · Decidir y documentar la frontera de ejecución síncrona · prioridad alta · S

**Título de issue**: Definir si `TryGetData` y `TryAll` devuelven datos materializados o consultas diferidas

**Como / quiero / para**: Como consumidor de los repositorios, quiero conocer cuándo se ejecutan las lecturas síncronas, para entender quién recibe los errores de base de datos y el coste de memoria.

**Contexto**: `EfReadRepository.cs:188-191,218-221` construye secuencias diferidas; la API promete captura funcional y la variante asíncrona materializa.

**Alcance**: documentar y decidir entre (a) materializar dentro de `TryMap`, manteniendo el contrato de captura y aceptando el coste de memoria, o (b) declarar explícitamente el resultado diferido y retirar la promesa de captura para errores de enumeración. **Decisión de esta implementación**: (a), porque alinea con la intención documentada de `Try*`; para conjuntos grandes se recomienda paginación.

**Criterios de aceptación**
- [x] Se registra la decisión de implementación sobre evaluación y materialización.
- [x] Se describe impacto en timing y memoria para colecciones grandes en el README del proyecto.
- [x] El contrato elegido se refleja en interfaces/README sin prometer captura después del retorno.

**Pruebas**: `TryGetData_FalloDeTraduccionSincrona_SeCapturaEnMlResult` demuestra el error dentro del rail; `TryAllAsync_Integration_DevuelveListaDeVinos` fija el contenido devuelto.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: la materialización síncrona es ruptura silenciosa registrada como punto **74**. El consumidor debe revisar memoria y errores que ahora se capturan al llamar, no al enumerar.

**Metadatos propuestos**: EFCore · prioridad alta · S, confianza baja · tramo 1 · etiquetas por confirmar con el catálogo del repositorio.

### US-02 · Capturar errores de lectura y comprobar resultados reales · prioridad alta · M

**Título de issue**: Mantener los errores de lectura dentro de `MlResult` y verificar el contenido

**Como / quiero / para**: Como consumidor de `IEfReadRepository<T>`, quiero que la lectura cumpla el límite de error decidido y devuelva los elementos encontrados, para no recibir una falsa respuesta válida ni una excepción tardía inesperada.

**Contexto**: `TryGetData`/`TryAll` están en `EfReadRepository.cs:188-236`; la prueba de lista solo usa `NotBeNull` en `RepoVinorFpTests.cs:64-73`.

**Alcance**: aplicar D-01 en los métodos síncronos y fortalecer el test con una entidad de identidad única; no cambiar el contrato de lecturas asíncronas.

**Criterios de aceptación**
- [x] El error de traducción durante la ejecución cae dentro del límite y produce `MlResult.Fail`.
- [x] `TryAllAsync` devuelve la entidad preparada por el test; una secuencia vacía hace fallar el test.
- [x] La consulta mantiene `AsNoTracking`.
- [x] Las lecturas síncronas materializan antes de devolver el resultado.
- [x] No se conserva una diferencia de evaluación entre contratos síncronos y asíncronos.

**Pruebas**: éxito con datos conocidos; resultado vacío; error durante enumeración/ejecución; regresión por no materializar; limpieza del dato único.

> **Progreso** · `TryAllAsync_Integration_DevuelveListaDeVinos` inserta una entidad con identidad única, comprueba el contenido y la materialización; `TryGetData_FalloDeTraduccionSincrona_SeCapturaEnMlResult` demuestra la captura de consulta.

**Dependencias**: bloqueada por US-01.

**Compatibilidad/migración**: si se elige materialización, ruptura de comportamiento silenciosa (momento, memoria y excepción). Se registra en seis sitios del prompt, con número asignado al implementar.

**Metadatos propuestos**: EFCore · prioridad alta · M, confianza media · tramo 2 · etiquetas por confirmar.

### US-03 · Validar claves antes de crear el mensaje de no encontrado · prioridad alta · S

**Título de issue**: Devolver un fallo funcional cuando el arreglo de clave sea nulo

**Como / quiero / para**: Como consumidor de las operaciones `TryFind`, `TryUpdate` y `TryRemove`, quiero que las claves inválidas pasen por sus guardas antes de formatearse, para recibir `MlResult.Fail` en lugar de una excepción fuera del rail.

**Contexto**: `EfReadRepository.cs:45-46,73-74` evalúa `FormatCompositeKey` antes de llamar a la sobrecarga con guardas; `FormatCompositeKey` cuenta el arreglo en `Helpers/Extensions.cs:20-23`. `EfUpdateRepository.cs:62,129` y `EfDeleteRepository.cs:56,109` también formatean claves.

**Alcance**: centralizar validación previa a la generación del mensaje en todas las rutas por clave; decidir cómo tratar `null` dentro de una clave compuesta.

**Criterios de aceptación**
- [x] Un arreglo `null` produce fallo funcional sin lanzar en find/update/remove síncronos y asíncronos.
- [x] Un arreglo vacío mantiene un resultado fallido en las seis variantes find/update/remove sync/async.
- [x] Se conserva el formato del mensaje para claves compuestas válidas no encontradas.
- [x] No se accede a `Count`, `ToString` ni interpolación de clave antes de validar.

**Pruebas**: `TryFind_ArregloDeClavesNulo_DevuelveFalloSinLanzar`, `TryFindAsync_ArregloDeClavesNulo_DevuelveFalloSinLanzar`, `TryUpdateYTryRemove_ClavesNulas_DevuelvenFalloSinLanzar`, `Repositorios_ClavesVacias_DevuelvenFallo`, `TryFind_ClaveCompuestaNoEncontrada_MensajeConservaElOrdenDeValores` y pruebas existentes de clave simple/compuesta.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: cambiar de excepción directa a `MlResult.Fail` es ruptura de comportamiento silenciosa registrada como punto **73**; el patrón de búsqueda y la acción del consumidor están en el prompt.

**Metadatos propuestos**: EFCore · prioridad alta · S, confianza media · tramo 2 · etiquetas por confirmar.

## 5. Orden de ejecución

1. US-01 cierra el contrato (bloquea US-02).
2. US-03 puede implementarse en paralelo con US-01, pero debe publicarse con el registro migratorio correspondiente.
3. US-02 añade la regresión de contenido y el escenario de error escogido.

## 6. Decisiones cerradas

- D-01 · **Cerrada**: materializar lecturas síncronas dentro de `MlResult` para cumplir la promesa de captura; advertir el coste de memoria y preferir paginación para conjuntos grandes.
- D-02 · **Diferida explícitamente**: no cambia la política para un componente individual `null` dentro de una clave compuesta. Solo se valida el arreglo completo nulo y se conserva el comportamiento previo de EF para cada componente.

## 7. Definición de terminado

- [x] Todas las entradas de análisis #4, #5 y #13 quedan resueltas o explícitamente diferidas con motivo.
- [x] Las operaciones `Try*` no lanzan al formatear una clave antes de validarla.
- [x] Las pruebas distinguen consulta no ejecutada, ejecución fallida y contenido correcto.
- [x] Los cambios de ejecución se registran en la guía de migración en los puntos 73 y 74.
- [x] Build y suite de integración Release verificados; enlaces Markdown verificados al archivar.

## 8. Apéndice · comportamiento actual y destino

```csharp
// Actual: la excepción de enumeración puede ocurrir después de devolver el resultado.
var result = EnsureFp.NotNull(filter, message)
	.TryMap(_ => (IEnumerable<T>)context.Set<T>().Where(filter));

// Destino recomendado, sujeto a D-01: ejecución dentro del límite funcional.
var result = EnsureFp.NotNull(filter, message)
	.TryMap(_ => (IEnumerable<T>)context.Set<T>().Where(filter).ToList());
```

```csharp
// Validar la clave antes de interpolarla en un mensaje de no encontrado.
if (keyValues is null)
	return EnsureFp.NotNull(keyValues, "The object array pk cannot be null");
```

## Ver también

- [Índice de épicas EFCore](README.md)
- [Análisis fuente · modelo de error](../../../../Mejoras/EntityFrameworkCore/03-Modelo-de-Error.md)
- [Análisis fuente · calidad de pruebas](../../../../Mejoras/EntityFrameworkCore/08-Calidad-Pruebas-y-Documentacion.md)
- [Épica UoW completada](Epica-Unidad-de-Trabajo-y-Transacciones.md)
- [Prompt de migración](../../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md)
