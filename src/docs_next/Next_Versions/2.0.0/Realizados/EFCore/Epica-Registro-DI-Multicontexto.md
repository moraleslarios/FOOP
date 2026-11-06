# Épica · Registro DI explícito para EntityFrameworkCore

> **Versión destino** · `2.0.0`, pendiente de autorización del propietario
> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Origen** · [análisis 05 · arquitectura](../../../../Mejoras/EntityFrameworkCore/05-Superficie-de-API-y-Arquitectura.md) y [07 · empaquetado](../../../../Mejoras/EntityFrameworkCore/07-Empaquetado-y-Compatibilidad.md)
> **Estado** · ✅ Completada · resolución multicontexto y consumo del `.nupkg` verificados
> **Esfuerzo estimado** · S (confianza baja; supuesto: mantener el API de registro actual y añadir prueba/documentación más dependencia directa)
> **Riesgo** · 🟡 medio · el riesgo de contexto incorrecto solo aparece con más de un `DbContext`

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Hallazgos colaterales](#2-hallazgos-colaterales)
3. [Alcance](#3-alcance)
4. [Historias de usuario](#4-historias-de-usuario)
5. [Orden de ejecución](#5-orden-de-ejecución)
6. [Decisiones cerradas](#6-decisiones-cerradas)
7. [Definición de terminado](#7-definición-de-terminado)
8. [Apéndice · registro actual y destino](#8-apéndice--registro-actual-y-destino)

## 1. Contexto y problema

El método público `AddOofpRepositories<T,TContext>` registra la forma genérica `IMlUnitOfWork<TContext>` y la alias no genérica `IMlUnitOfWork` por cada llamada. Si la aplicación registra varios tipos de contexto, la resolución no genérica representa el último descriptor registrado y no permite identificar el contexto asociado. Además, `RegisterServices.cs` expone `IServiceCollection` pero el proyecto solo declara directamente `Microsoft.EntityFrameworkCore`.

| Hallazgo | Evidencia |
|---|---|
| #8 · alias no genérico ambiguo con varios contextos | `RegisterServices.cs:30-40`; `Repositories/IMlUnitOfWork.Context.cs:6-11` |
| #12 · dependencia DI implícita | `RegisterServices.cs:30-33`; `MoralesLarios.OOFP.EntityFrameworkCore.csproj:10-13`; README del proyecto, sección «Instalación y dependencias» |

## 2. Hallazgos colaterales

La épica de unidad de trabajo está completada y ya establece propiedad externa de transacción; este análisis no reabre ese resultado. El alcance se limita a resolución multicontexto y dependencia pública de DI.

## 3. Alcance

**Dentro**: fijar el uso de `IMlUnitOfWork<TContext>` en aplicaciones con varios contextos, probar resolución por ámbito y declarar la dependencia directa de DI según la versión ya resuelta por el repositorio.

**Fuera**: retirar interfaces públicas, cambiar los ciclos de vida `Scoped`, subir la versión de EF Core o elegir una nueva versión de paquete sin aprobación.

## 4. Historias de usuario

### US-01 · Resolver la unidad de trabajo del contexto correcto · prioridad media · S

**Título de issue**: Documentar y probar el registro de repositorios con varios `DbContext`

**Como / quiero / para**: Como aplicación que usa más de un contexto EF Core, quiero resolver `IMlUnitOfWork<TContext>` para el contexto concreto, para confirmar cambios en el tracker correcto y no depender del orden de registro.

**Contexto**: `RegisterServices.cs:35-40` añade una alias no genérica en cada llamada; los repositorios reciben la unidad genérica de su propio contexto. La épica UoW ya completada acredita la intención de mantener el contexto específico.

**Alcance**: hacer explícita la interfaz genérica para escenarios multicontexto y fijar por prueba que cada tipo mantiene su propio UoW/DbContext. Decidir qué promesa ofrece la alias no genérica; no retirarla de forma implícita.

**Criterios de aceptación**
- [x] Un test registra dos tipos de contexto y resuelve cada `IMlUnitOfWork<TContext>` dentro del scope.
- [x] Cada unidad se comparte por referencia con los repositorios que usan su contexto.
- [x] La documentación recomienda inyectar la interfaz genérica cuando se registran varios contextos.
- [x] La interfaz no genérica se documenta como conveniencia de un contexto o se acuerda una estrategia inequívoca para varios.

**Pruebas**: dos contextos distintos; dos entidades en un mismo contexto; resolución repetida por scope; confirmar que no se intercambian contextos.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: documentar el uso genérico es aditivo. Cambiar o retirar el registro de `IMlUnitOfWork` no genérico sería ruptura de compilación/comportamiento para consumidores y requiere decisión y registro migratorio.

**Metadatos propuestos**: EFCore · prioridad media · S, confianza baja · tramo 1 · etiquetas por confirmar.

### US-02 · Declarar directamente las abstracciones de DI usadas por la API · prioridad baja · XS

**Título de issue**: Declarar la dependencia directa de `Microsoft.Extensions.DependencyInjection.Abstractions`

**Como / quiero / para**: Como consumidor del paquete, quiero que la abstracción necesaria para compilar `AddOofpRepositories` forme parte explícita del grafo de dependencias, para no depender accidentalmente de una transitiva de EF Core.

**Contexto**: el método usa `IServiceCollection` (`RegisterServices.cs:30-33`), mientras que el `.csproj` solo declara `Microsoft.EntityFrameworkCore` (`MoralesLarios.OOFP.EntityFrameworkCore.csproj:10-13`). El README actual ya reconoce que DI llega transitivamente.

**Alcance**: inspeccionar el grafo restaurado y declarar `Microsoft.Extensions.DependencyInjection.Abstractions` directamente con la versión compatible ya usada por la solución. No subir EF Core ni cambiar versiones por esta historia.

**Criterios de aceptación**
- [x] La versión propuesta coincide con la versión compatible ya resuelta: `8.0.0`.
- [x] El `.nuspec` empaquetado expone la dependencia directa `8.0.0`, coherente con la versión restaurada.
- [x] El README de instalación enumera la dependencia como directa.
- [x] Un consumidor mínimo restaura el `.nupkg` local y compila `AddOofpRepositories` sin declarar directamente DI.

**Pruebas**: restore/pack/build del proyecto y compilación mínima de un consumidor que use `IServiceCollection` y `AddOofpRepositories`.

> **Verificación** · `DosContextos_ResuelvenLaUnidadDeTrabajoGenericaCorrecta` pasa; el `.nuspec` incluye DI Abstractions `8.0.0`; un consumidor temporal con `TreatWarningsAsErrors=true`, que solo referencia `MoralesLarios.OOFP.EntityFrameworkCore` 2.0.0, restauró desde el feed local y compiló `AddOofpRepositories`.

**Dependencias**: Sin dependencias; coordinación recomendada con US-01 si se cambia el registro.

**Compatibilidad/migración**: Aditiva si se conserva la versión compatible. Cualquier cambio de versión de paquete espera aprobación; no modifica la versión de `MoralesLarios.OOFP.EntityFrameworkCore`.

**Metadatos propuestos**: EFCore · prioridad baja · XS, confianza baja · tramo 1 · etiquetas por confirmar.

## 5. Orden de ejecución

US-01 y US-02 son independientes y pueden implementarse en paralelo. Resolver primero US-01 si se decide alterar el registro de servicios.

## 6. Decisiones cerradas

- D-01 · **Se conserva `IMlUnitOfWork` no genérica** como alias de conveniencia para un contexto. En multicontexto se recomienda `IMlUnitOfWork<TContext>`.
- D-02 · **Se declara `Microsoft.Extensions.DependencyInjection.Abstractions` 8.0.0**, ya presente en el grafo restaurado; no se cambia la versión del paquete OOFP.

**Pendiente de cierre:** US-02 requiere un smoke test de consumidor que restaure el `.nupkg` local y compile `AddOofpRepositories`.

## 7. Definición de terminado

- [x] La guía explica con claridad la resolución genérica para varios contextos.
- [x] Las pruebas demuestran que cada repositorio y unidad de trabajo quedan ligados a su contexto.
- [x] La referencia de DI usada por la API pública es explícita y compatible con el grafo real.
- [x] Un consumidor del `.nupkg` compila sin referencia DI directa.
- [x] No se cambia ni se publica versión de paquete por completar la planificación.

## 8. Apéndice · registro actual y destino

```csharp
// La API genérica identifica el contexto del contrato.
IMlUnitOfWork<TContext> unitOfWork = provider.GetRequiredService<IMlUnitOfWork<TContext>>();

// La alias no genérica no identifica cuál contexto representa si hay varios registrados.
IMlUnitOfWork unitOfWork = provider.GetRequiredService<IMlUnitOfWork>();
```

```xml
<!-- Agregar dependencia directa solo con versión compatible confirmada en el grafo existente. -->
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="..." />
```

## Ver también

- [Índice de épicas EFCore](README.md)
- [Análisis de arquitectura](../../../../Mejoras/EntityFrameworkCore/05-Superficie-de-API-y-Arquitectura.md)
- [Análisis de empaquetado](../../../../Mejoras/EntityFrameworkCore/07-Empaquetado-y-Compatibilidad.md)
- [Épica de unidad de trabajo completada](../../Realizados/EFCore/Epica-Unidad-de-Trabajo-y-Transacciones.md)
