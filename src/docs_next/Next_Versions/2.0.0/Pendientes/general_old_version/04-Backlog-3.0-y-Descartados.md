# 04 · Backlog 3.0.0 y descartes

> **Qué es:** lo que **no** entra en la 2.0.0, con el motivo. Cada línea tiene una razón de aplazamiento o de
> descarte; si la razón desaparece, la línea vuelve a un plan.
> **Origen:** `Profesionalizacion-Diseno-API-y-Producto.md`, `consejos_mejora_mlresult.md`, `PendingTasks-legacy.md`
> y los consejos de nomenclatura. Auditoría: [`02`](02-Auditoria-Documentos-Complementarios.md).

## Índice

1. [Aplazado a 3.0.0 o posterior](#1-aplazado-a-300-o-posterior)
2. [Descartado o a rediseñar](#2-descartado-o-a-rediseñar)
3. [Qué haría falta para recuperar cada línea](#3-qué-haría-falta-para-recuperar-cada-línea)

---

## 1. Aplazado a 3.0.0 o posterior

| Línea | Motivo del aplazamiento | Prerrequisito |
|---|---|---|
| Errores tipados (`Code`, `Severity`) | Cambia el contrato de `MlError` y de la conversión HTTP; la épica de Validación ya añade `Target` | Validación US-01 y Serialización US-04/05 cerradas |
| Proveedor de mensajes y recursos `.resx` | La infraestructura mínima se hace en Mensajes US-01; el resto exige política de idioma | Decisión de idioma del propietario |
| `MlErrorDetails` inmutable o `record`; `Errors` como lista; `Metadata` no mutable (MR7, MR10, MR17) | Rompe la compilación y la serialización de los consumidores | Una versión mayor y una medición de uso |
| Contrato JSON de `MlResult` (MR20) | Debe evitar serializar excepciones en bruto | Errores tipados |
| `ValueTask`, `IAsyncEnumerable` y rediseño asíncrono | Sin medición no hay justificación | G6-US-05 con línea base |
| Generador de sobrecargas | Reduciría una superficie enorme, pero cambia el modelo de mantenimiento | Decidir qué sobrecargas son canónicas |
| Núcleo mínimo e IntelliSense «avanzado» | Requiere clasificar toda la API | Inventario de uso |
| Observabilidad (`ActivitySource`, `Meter`, `[LoggerMessage]`) | Convenciones de telemetría y redacción de datos sensibles | Decisión de convenciones |
| `IOptions` general y `ValidateOnStart`; opciones neutras | Mueve literales públicos a configuración | Catálogo de opciones por paquete |
| Capa web moderna (`IExceptionHandler`, `IEndpointFilter`, Minimal API, ETag/If-Match) | Una API paralela a la actual | REST y errores tipados cerrados |
| Validadores y *value objects* con `static abstract` | Rediseño amplio | Spike de G4-US-05 |
| Sitio DocFX, plantilla `dotnet new`, ejemplos compilados en CI | Producto, no código | G1-US-04 |
| Retirada de puentes `[Obsolete]` conservados | Acordado «Will be removed in 3.0.0» | G5-US-02 |
| `Id` de 64 bits (`Int64Id`) | No hay API publicada que migrar ni caso de uso | Caso de uso real |

## 2. Descartado o a rediseñar

| Idea | Origen | Veredicto | Motivo |
|---|---|---|---|
| `TransformData` como renombrado de `CompleteWith*` | PendingTasks | **Descartar** | Renombrar sin ganancia semántica rompe llamadas públicas |
| `MakeBind` y `MakeMap`/`ChangeValueReturn` | PendingTasks | **Descartar** hasta definir la diferencia | Ya existen `Bind`/`TryBind` y `ChangeReturnResult`; otra familia duplica |
| `BindSelf` | PendingTasks | **Descartar** | Se compone con `Bind` + `Map`; añadir alias multiplica la matriz de sobrecargas |
| `*IfException` como alias | PendingTasks | **Descartar** | La nomenclatura vigente es `*IfFailWithException` |
| `AddValueIfValid` | PendingTasks | **Rediseñar** | Falta definir qué dato se guarda y dónde |
| Asincronía artificial de las funciones puras de `MlErrorDetailsActions` | PendingTasks | **Descartar** | Aporta ruido; la composición ya es asíncrona donde importa |
| Registrar al crear el `MlResult` | PendingTasks | **Descartar** | Acoplaría `Core` a `Logging`; se compone con `.LogMlResult…` |
| `LogMlFinish` con tuplas y *builders* | PendingTasks | **Descartar** | `LogMlResult` con tuplas y `LogCompletedResult` ya lo cubren |
| Unificar `CompleteWithDetailsValueIfFailAsync` | PendingTasks | **Descartar** salvo duplicación concreta | Sin duplicación demostrada no hay qué unificar |
| `TreatWarningsAsErrors` de golpe con `NoWarn` amplio | Profesionalización | **Descartar** | Oculta deuda; se activa por proyecto con línea base (G1-US-03) |
| Testcontainers para todas las pruebas de integración | Profesionalización | **Rediseñar** | SQLite en memoria cubre la mayoría; se usa donde SQLite no alcance (G6-US-04) |
| «Alinear a `net8.0`» y retirar `Mvc.Core 2.1.0` | Profesionalización | **Obsoleto** | El destino es `net11.0` y MVC llega por `FrameworkReference` |
| «No hay `CancellationToken`» | Profesionalización | **Obsoleto** | 115 declaraciones ya lo reciben; basta auditar la propagación |
| Mover tipos `StringAs*` a genéricos sin spike | Nomenclatura | **Rediseñar** | Exige demostrar equivalencia de límites (G4-US-05) |
| `;` como separador de clave compuesta | Inventario INV-78 | **Obsoleto** | El parser vigente usa `,` y REST US-02 lo trata |

## 3. Qué haría falta para recuperar cada línea

Una línea del backlog **vuelve a un plan** cuando se cumpla su prerrequisito **y** el propietario la priorice. El
procedimiento es el mismo que el de las épicas existentes: historia con criterios verificables, decisión escrita,
test de regresión y registro de la ruptura en los seis sitios del prompt de migración.
