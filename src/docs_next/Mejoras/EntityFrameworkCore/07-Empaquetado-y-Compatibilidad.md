# 07 · Empaquetado y compatibilidad

> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Versión destino** · 2.x, sujeto a validar dependencias centrales del repositorio
> **Origen** · análisis del commit `2d07c33`, rama `release/2.0.0-nomenclatura`
> **Estado** · propuesta, pendiente de decisión
> **Esfuerzo total** · sin estimar
> **Riesgo** · 🟡 medio

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

El método de registro público recibe y devuelve `IServiceCollection`, pero el proyecto solo declara una referencia directa a `Microsoft.EntityFrameworkCore`. El comentario del README del proyecto también señala que la dependencia de DI llega transitivamente desde EF Core. Una API pública no debería depender accidentalmente de la composición transitiva de otra dependencia sin que el empaquetado lo haga explícito.

| Punto # | Fichero | Líneas | Naturaleza |
|---:|---|---:|---|
| 01 | `RegisterServices.cs` | 30-33 | Superficie pública usa `IServiceCollection` |
| 01 | `MoralesLarios.OOFP.EntityFrameworkCore.csproj` | 10-13 | Solo se declara `Microsoft.EntityFrameworkCore` directamente |
| 01 | `README.md` del proyecto | sección «Instalación y dependencias» | El README reconoce la dependencia transitiva |

### Punto 01 — Dependencia pública de DI implícita

```xml
<!-- MoralesLarios.OOFP.EntityFrameworkCore.csproj:12 -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.3" />
<!-- ← no hay PackageReference explícito de Microsoft.Extensions.DependencyInjection.Abstractions -->
```

```csharp
// RegisterServices.cs:30-33
public static IServiceCollection AddOofpRepositories<T, TContext>(
    this IServiceCollection services) // ← tipo del BCL Extensions DI expuesto públicamente
```

La referencia transitiva permite compilar hoy, pero acopla el contrato de compilación a los dependientes que EF Core elija publicar. No se verificó el `.nuspec` empaquetado ni el grafo restaurado en este análisis.

> **Propuesta** · Revisar el grafo NuGet resultante y declarar directamente `Microsoft.Extensions.DependencyInjection.Abstractions` si la biblioteca usa y expone ese contrato como dependencia propia. Alinear su versión con la política de versiones central del repositorio antes de editar el `.csproj`. Clasificación: **aditiva** para consumidores si la dependencia queda correctamente transitiva y compatible; validar que no se duplique ni entre en conflicto con el paquete existente.

## Alcance

Se revisa la dependencia visible en la API de registro. No se recomienda elevar EF Core ni cambiar `TargetFramework` basándose solo en la versión observada `8.0.3`; eso requiere consultar la política de soporte y compatibilidad del producto.

## Historias de usuario

- Como consumidor de la API de registro, quiero que las dependencias necesarias para compilar sus tipos públicos estén declaradas de forma explícita y estable.
- Como mantenedor del paquete, quiero que las actualizaciones de EF Core no cambien indirectamente el grafo público de dependencias de DI.

## Orden de ejecución

1. Inspeccionar centralización de versiones y el `.nuspec` generado en el flujo de empaquetado.
2. Confirmar si la versión de abstracciones se administra centralmente.
3. Añadir referencia directa solo si no existe una política equivalente que la resuelva.
4. Validar restore/pack/build en el flujo autorizado del repositorio.

## Decisiones abiertas

- D-01 · ¿Hay una política central de paquetes fuera del árbol inspeccionado que ya fije `Microsoft.Extensions.DependencyInjection.Abstractions`?
- D-02 · ¿La biblioteca publica NuGet por un flujo que modifica `IsPackable` o metadatos fuera de este `.csproj`?

## Definición de terminado

- El grafo de dependencias empaquetado declara de manera reproducible las abstracciones usadas por la API pública.
- No se eleva ninguna versión de paquete sin la aprobación correspondiente.
- El paquete restaurado y publicado conserva una única versión compatible de las abstracciones DI.

## Apéndice · código actual y propuesto

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.3" />
  <!-- Propuesta condicionada a la política central de paquetes:
  <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="..." />
  -->
</ItemGroup>
```

## Ver también

- [README del análisis](README.md) · dependencias y proyectos consumidores.
- `src/MoralesLarios.OOFP.EntityFrameworkCore/README.md` · dependencias declaradas en la documentación de instalación.
