# 05 · Superficie de API y arquitectura

> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Versión destino** · 2.x para aclaraciones; cambio funcional sujeto a revisión de compatibilidad
> **Origen** · análisis del commit `2d07c33`, rama `release/2.0.0-nomenclatura`
> **Estado** · propuesta, pendiente de decisión
> **Esfuerzo total** · sin estimar
> **Riesgo** · 🟠 alto en semántica transaccional; 🟡 medio en composición DI

---

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Alcance](#alcance)
3. [Historias de usuario](#historias-de-usuario)
4. [Orden de ejecución](#orden-de-ejecución)
5. [Decisiones abiertas](#decisiones-abiertas)
6. [Definición de terminado](#definición-de-terminado)
7. [Apéndice · código actual y propuesto](#apéndice--código-actual-y-propuesto)
8. [Ver también](#ver-también)

---

## 1. Contexto y problema

La unidad de trabajo respeta la propiedad de una transacción externa en el código, pero la documentación presenta el rollback como incondicional. Además, el registro expone una interfaz no genérica de unidad de trabajo, aunque el proyecto permite registrar repositorios para distintos tipos de `DbContext` en el mismo contenedor.

| Punto # | Fichero | Líneas | Naturaleza |
|---:|---|---:|---|
| 01 | `Repositories/IMlUnitOfWork.cs` | 32-44 | El contrato no explica la propiedad de la transacción |
| 01 | `Repositories/MlUnitOfWork.cs` | 83-118, 146-180 | Solo revierte si la unidad creó la transacción |
| 01 | `Tests.Integration/UnitOfWorkTransactionTests.cs` | 328-344 | Se prueba transacción externa válida, no fallo dentro de ella |
| 02 | `RegisterServices.cs` | 30-40 | Registro repetido del servicio no genérico por contexto |

### Punto 01 — El rollback depende de quién posee la transacción

La documentación dice que un resultado fallido revierte la transacción. En la implementación, cuando `Database.CurrentTransaction` ya existe, `ownsTransaction` vale `false`; si el callback devuelve fallo, el método retorna sin llamar a `Rollback`. La prueba existente cubre la transacción externa exitosa y confirma que la unidad no la confirma, pero no cubre el camino fallido.

```csharp
// Repositories/MlUnitOfWork.cs:83-97
var existingTransaction = _context.Database.CurrentTransaction;
var ownsTransaction = existingTransaction is null;
...
if (result.IsFailure)
{
    if (ownsTransaction) // ← con transacción externa no se revierte
    {
        transaction!.Rollback();
        _context.ChangeTracker.Clear();
    }
    return result;
}
```

No se debe asumir que la implementación está mal por no revertir una transacción que pertenece al llamador: revertirla también podría violar el límite de propiedad externa. El defecto confirmado es que el contrato no documenta esa excepción y puede llevar al consumidor a creer que una operación fallida ya limpió la transacción compartida.

> **Propuesta** · Aclarar que el rollback automático solo ocurre cuando la unidad crea la transacción; con una transacción externa, su propietario decide rollback/commit. Añadir una prueba de callback fallido con transacción externa que fije expresamente el comportamiento. Clasificación: **aditiva** si solo se documenta y prueba el contrato actual; **de comportamiento** si se decide que la unidad debe revertir también la transacción externa.

### Punto 02 — La interfaz no genérica es ambigua con varios contextos

`AddOofpRepositories<T, TContext>` registra `IMlUnitOfWork<TContext>` y también `IMlUnitOfWork` en cada llamada. Si se configuran varios contextos en el mismo `IServiceCollection`, la resolución individual de `IMlUnitOfWork` devuelve el último descriptor registrado; no identifica qué contexto representa. La interfaz genérica sí conserva ese vínculo.

```csharp
// RegisterServices.cs:35-40
services.AddScoped<IMlUnitOfWork<TContext>>(...);
services.AddScoped<IMlUnitOfWork>(serviceProvider =>
    serviceProvider.GetRequiredService<IMlUnitOfWork<TContext>>()); // ← registro por cada TContext
```

> **Propuesta** · Preferir inyección de `IMlUnitOfWork<TContext>` cuando exista más de un contexto y documentar que la interfaz no genérica no es resoluble sin ambigüedad en ese escenario. Si se modifica el registro público, medir el efecto en consumidores antes de retirar el servicio no genérico. Clasificación del problema actual: **de comportamiento**, condicionado a varios contextos; ninguna ruptura se introduce por documentarlo.

## Alcance

Se revisan la propiedad de transacciones y la composición de servicios. El alcance no propone cambiar la unidad de trabajo compartida por agregado ni los ciclos de vida `Scoped`, que están alineados con el contexto EF Core.

## Historias de usuario

- Como consumidor, quiero saber quién debe revertir una transacción externa cuando un callback falla.
- Como aplicación con más de un `DbContext`, quiero resolver sin ambigüedad la unidad de trabajo asociada a cada contexto.

## Orden de ejecución

1. Definir explícitamente el contrato de anidamiento y propiedad de transacciones.
2. Añadir la prueba del callback fallido bajo transacción externa.
3. Documentar el uso de la interfaz genérica para contenedores con varios contextos.
4. Solo entonces decidir si se requiere una modificación del registro DI.

## Decisiones abiertas

- D-01 · ¿Una llamada con transacción externa debe dejar su rollback exclusivamente al propietario?
- D-02 · ¿Se conserva la resolución de `IMlUnitOfWork` no genérica como conveniencia para escenarios de un único contexto?

## Definición de terminado

- El contrato describe qué ramas confirman, revierten o dejan abierta una transacción externa.
- Hay pruebas separadas para transacción propia y externa, con callback exitoso y fallido.
- La guía explica cuándo inyectar `IMlUnitOfWork<TContext>`.

## Apéndice · código actual y propuesto

```csharp
// Contrato recomendado, sin cambiar el comportamiento:
// Si ya existe una transacción, su propietario externo es responsable de confirmarla o revertirla.
```

## Ver también

- `Repositories/IMlUnitOfWork.cs` · contrato actual.
- [08 · Calidad, pruebas y documentación](08-Calidad-Pruebas-y-Documentacion.md).
- [README del análisis](README.md).
