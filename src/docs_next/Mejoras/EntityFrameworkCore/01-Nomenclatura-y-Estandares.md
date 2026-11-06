# 01 · Nomenclatura y estándares de .NET

> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Versión destino** · 2.x para documentación; versión mayor si se renombra una clase pública
> **Origen** · análisis del commit `2d07c33`, rama `release/2.0.0-nomenclatura`
> **Estado** · propuesta, pendiente de decisión
> **Esfuerzo total** · sin estimar
> **Riesgo** · 🟠 alto por el renombrado público; 🟢 bajo para documentación

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

El proyecto está habilitado para nullable reference types y contiene documentación XML, pero parte de la nomenclatura y los comentarios no siguen las reglas expresas de `src/CONTRIBUTING.md`. Se midieron 232 etiquetas `<summary>` en 28 ficheros C#; la métrica cuenta etiquetas, no calidad semántica.

| Punto # | Fichero | Líneas | Naturaleza |
|---:|---|---:|---|
| 01 | `Repositories/IEfReadRepository.cs` | 13 | Convención: parámetro genérico de entidad `T` |
| 02 | `Helpers/Extensions.cs` | 8-10 | API pública con nombre genérico y `<c>` vacío; ruptura si se renombra |
| 03 | `Repositories/IEfReadRepository.cs` | 9-13 | Comentarios XML en español, contra la regla del repositorio |

### Punto 01 — El parámetro genérico de entidad se llama `T`

La guía de contribución asigna `TEntity` a entidades de dominio/persistencia y prohíbe `T` en ese rol. La interfaz principal usa `T`, al igual que sus interfaces hermanas y las implementaciones.

```csharp
// Repositories/IEfReadRepository.cs:13
public interface IEfReadRepository<T> where T : class // ← T representa una entidad persistida
```

> **Propuesta** · Adoptar `TEntity` al tocar la API por otra razón o mediante una edición documental/de código coordinada. El nombre del parámetro genérico no forma parte de la firma binaria ni de la sintaxis posicional de consumo, por lo que su renombre no se clasifica como ruptura de compatibilidad. Decisión D-01: conservar la convención explícita del repositorio frente a la costumbre general de usar `T` en interfaces genéricas.

### Punto 02 — Tipo público `Extensions` y etiqueta XML vacía

El tipo no expresa qué extensiones contiene, y su documentación no aporta contenido porque deja vacía la etiqueta `<c>`.

```csharp
// Helpers/Extensions.cs:8-10
/// Métodos de extension de <c></c>. // ← contenido vacío y comentario en español
public static partial class Extensions // ← nombre de API sin dominio
```

La clase es pública y tiene un miembro puente en `Extensions.Obsolete.cs`; renombrarla sin preservar la clase original rompería referencias de consumidor. Conservar ambas clases con los mismos métodos de extensión también podría crear ambigüedad cuando ambos espacios de nombres están importados.

> **Propuesta** · Corregir el `<summary>` en una versión compatible. Para renombrar, definir antes un puente compilable que no duplique candidatas de extensión, o posponer la retirada a una versión mayor. Registrar el renombrado en el inventario del prompt de migración. Clasificación: **rupturista** si se elimina o renombra el tipo existente.

### Punto 03 — Comentarios XML de producción en español

`CONTRIBUTING.md` fija el inglés para comentarios XML, pero los contratos públicos usan resúmenes y descripciones en español, por ejemplo `IEfReadRepository<T>` y `IEfPaginator<T>`.

```csharp
// Repositories/IEfReadRepository.cs:9-11
/// <summary>
/// Contrato de <c>IEfReadRepository</c>. // ← comentario XML en español
/// </summary>
```

> **Propuesta** · Normalizar el texto XML al inglés al editar cada contrato, manteniendo los documentos Markdown en español. Es una mejora documental aditiva: no cambia la firma ni el texto de ejecución.

## Alcance

Incluye identificadores y comentarios XML de la API de EntityFrameworkCore. No propone renombrar automáticamente todos los parámetros genéricos ni rehacer la documentación del README.

## Historias de usuario

- Como consumidor, quiero que los nombres de tipos y los comentarios XML comuniquen el rol de la API con las convenciones del repositorio.
- Como mantenedor, quiero preservar referencias externas si se aprueba renombrar una clase pública.

## Orden de ejecución

1. Decidir el destino del tipo público `Extensions` y el plan de compatibilidad.
2. Corregir el resumen vacío y normalizar comentarios XML en cambios acotados.
3. Aplicar `TEntity` a parámetros genéricos de entidad cuando se editen las declaraciones relacionadas.

## Decisiones abiertas

- D-01 · ¿Se exige `TEntity` en esta API genérica aunque `T` sea una convención común en C#?
- D-02 · ¿Qué nombre específico sustituiría `Extensions` y cómo evitar ambigüedad entre extensiones antiguas y nuevas?

## Definición de terminado

- Cada renombrado público conserva un puente o se asigna a una versión mayor.
- Los comentarios XML añadidos o modificados están en inglés y las etiquetas `<c>` contienen texto real.
- No se cambia comportamiento ni firma como parte de una tarea puramente documental.

## Apéndice · código actual y propuesto

```csharp
// Actual
public static partial class Extensions
{
    public static string FormatCompositeKey(this object[] source) => /* ... */;
}

// Propuesto: nombre específico aún por decidir; no retirar el tipo antiguo sin puente.
```

## Ver también

- `src/CONTRIBUTING.md` · nomenclatura y reglas de comentarios XML.
- [README del análisis](README.md) · prioridades y métricas del proyecto.
