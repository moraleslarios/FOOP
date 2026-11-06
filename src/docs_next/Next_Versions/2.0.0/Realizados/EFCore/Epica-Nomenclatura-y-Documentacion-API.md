# Épica · Nomenclatura y documentación de la API EFCore

> **Versión destino** · `2.0.0` para alineación documental; renombrado público sujeto a compatibilidad y autorización
> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Origen** · [análisis 01 · Nomenclatura y estándares](../../../../Mejoras/EntityFrameworkCore/01-Nomenclatura-y-Estandares.md)
> **Estado** · ✅ Completada · transición de `Extensions` decidida, `TEntity` aplicado y XML público verificado
> **Esfuerzo estimado** · M (confianza baja; supuesto: homogeneizar interfaces, docs XML y un tipo público de extensiones)
> **Riesgo** · 🟢 bajo · se conserva `Extensions`; el cambio genérico solo altera el nombre observable por reflexión

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Hallazgos colaterales](#2-hallazgos-colaterales)
3. [Alcance](#3-alcance)
4. [Historias de usuario](#4-historias-de-usuario)
5. [Orden de ejecución](#5-orden-de-ejecución)
6. [Decisiones cerradas y trabajo pendiente](#6-decisiones-cerradas-y-trabajo-pendiente)
7. [Definición de terminado](#7-definición-de-terminado)
8. [Apéndice · transición de la clase pública](#8-apéndice--transición-de-la-clase-pública)

## 1. Contexto y problema

La guía de contribución asigna `TEntity` a tipos genéricos que representan entidades persistidas y exige inglés en comentarios XML. Los contratos y APIs de EFCore ya usan `TEntity`; el tipo público `Extensions` se conserva y su documentación está en inglés. Renombrarlo o retirarlo sin un puente podría romper referencias directas de consumidores.

| Hallazgo | Evidencia |
|---|---|
| #1 · genérico de entidad con nombre `T` | `Repositories/IEfReadRepository.cs:13` antes del rename; ahora `TEntity`; [análisis 01](../../../../Mejoras/EntityFrameworkCore/01-Nomenclatura-y-Estandares.md) |
| #2 · helper público con documentación XML vacía | `Helpers/Extensions.cs` antes del cambio; summary corregido y tipo `Extensions` conservado |
| #3 · comentarios XML en español | `Repositories/IEfReadRepository.cs:9-13`; `Repositories/IEfPaginator.cs:9-15` |

## 2. Hallazgos colaterales

`Extensions.Obsolete.cs:6-18` ya contiene el puente `GetPkValues` → `FormatCompositeKey`. La estrategia de la clase parcial debe conservar ese puente o actualizarlo junto con el anexo de renombrados; no se renumeran ni se borran aliases existentes desde la planificación.

## 3. Alcance

**Dentro**: alinear nombres de parámetros genéricos de entidad; corregir/XML-docs en inglés; definir y aplicar estrategia para el contenedor público `Extensions` y sus miembros de extensión.

**Fuera**: cambiar nombres de métodos de los consumidores, modificar mensajes de ejecución o renombrar APIs ajenas a EntityFrameworkCore.

## 4. Historias de usuario

### US-01 · Elegir una transición compatible para `Extensions` · prioridad alta · S

**Título de issue**: Decidir cómo especializar el tipo público `Extensions` sin romper consumidores

**Como / quiero / para**: Como mantenedor de la biblioteca, quiero definir un nombre de tipo que comunique el dominio de las extensiones y una transición viable, para mejorar IntelliSense sin introducir ambigüedad ni una ruptura imprevista.

**Contexto**: `Extensions` es público y parcial (`Helpers/Extensions.cs:10`); `Extensions.Obsolete.cs` contiene el puente de `GetPkValues`. La clase reúne `FormatCompositeKey` y `PrivateOrderBy`.

**Decisión D-01**: elegir (a) en 2.0.0 y conservar el tipo público y sus métodos. No se crea una segunda clase ni se añade un puente nuevo; revisar ese diseño requeriría una versión futura y una prueba de consumidor.

**Criterios de aceptación**
- [x] Se inventariaron las referencias internas: `FormatCompositeKey` se invoca como extensión; no hay llamadas estáticas a `Extensions` dentro del repositorio.
- [x] Se conserva la llamada de extensión y el puente `GetPkValues`; no se añade una clase duplicada.
- [x] No se añade `[Obsolete]` a `Extensions` ni a sus miembros; el puente existente conserva su atributo y destino.
- [x] El build y las pruebas de integración compilan y ejecutan el uso existente de las extensiones.

**Pruebas**: compilar consumidor con `source.FormatCompositeKey()`, llamada estática actual, llamada al nombre nuevo y uso del método antiguo de `GetPkValues`.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: una retirada/renombrado rompe compilación; un puente `[Obsolete]` también produce advertencias al consumidor según la política del repositorio. Registrar decisión y número antes de implementar.

**Metadatos propuestos**: EFCore · prioridad alta · S, confianza baja · tramo 1 (decisión) · etiquetas por confirmar.

### US-02 · Mantener el contenedor público de extensiones en 2.0.0 · prioridad alta · M

**Título de issue**: Aplicar el nombre y la fachada acordados para las extensiones EFCore

**Como / quiero / para**: Como consumidor de la API, quiero conservar las llamadas públicas actuales mientras la biblioteca permanece en 2.0.0, para no introducir una migración solo nominal.

**Contexto**: `Helpers/Extensions.cs` declara el tipo público `Extensions` y los métodos `FormatCompositeKey` y `PrivateOrderBy`; el análisis proponía especializar el nombre del contenedor.

**Resultado**: no se añade un tipo nuevo. D-01 elige conservar `Extensions` y todas sus firmas; esta historia no requiere una implementación de clase/fachada adicional.

**Criterios de aceptación**
- [x] D-01 conserva el tipo y los métodos existentes; no hay una clase nueva que pueda duplicar candidatos de extensión.
- [x] Las llamadas de extensión existentes mantienen su resolución y no se añade ninguna sobrecarga homónima.
- [x] `GetPkValues` conserva su puente `[Obsolete]` preexistente a `FormatCompositeKey`.
- [x] No se renombra `Extensions`; el cambio de parámetro genérico `T` a `TEntity` se documenta como punto 77 y en el anexo de renombrados.

**Pruebas**: la decisión conserva las llamadas existentes; el build y la suite de integración verifican el uso actual de las extensiones y el puente `GetPkValues` permanece sin cambios.

**Dependencias**: bloqueada por US-01.

**Compatibilidad/migración**: no se retira ni renombra el tipo o sus miembros en 2.0.0. El rename genérico independiente se registra en el punto 77 y no cambia llamadas C#.

**Metadatos propuestos**: EFCore · prioridad alta · M, confianza baja · tramo 3 (renombrado) · etiquetas por confirmar.

### US-03 · Usar `TEntity` para entidades en los contratos genéricos · prioridad baja · XS

**Título de issue**: Alinear el nombre del parámetro genérico de entidad con la guía del repositorio

**Como / quiero / para**: Como consumidor y mantenedor, quiero que las interfaces de entidad usen `TEntity`, para distinguirlas de parámetros genéricos sin rol de dominio.

**Contexto**: los contratos y métodos genéricos públicos de EntityFrameworkCore usaban `T` para representar entidades, mientras que la guía local prescribe `TEntity`.

**Alcance**: renombrar el parámetro genérico en las declaraciones relacionadas y mantener el orden y las restricciones del tipo.

**Criterios de aceptación**
- [x] Los contratos e implementaciones de entidades, `AddOofpRepositories` y `PrivateOrderBy` usan `TEntity`.
- [x] No se cambia la aridad, las restricciones ni el orden de los parámetros genéricos.
- [x] Las llamadas posicionales y las pruebas de consumidor compilan sin cambios.

**Pruebas**: build de la solución, revisión de XML generado y `ContratosGenericosDeEntidad_ExponenNombreTEntityEnMetadatos`.

**Dependencias**: Sin dependencias; conviene ejecutarla junto a US-04.

**Compatibilidad/migración**: no cambia la compilación ni la firma binaria. El nombre genérico se observa por reflexión; no hay usos internos que dependan de `T`, y el punto 77 indica cómo revisar consumidores que sí inspeccionen ese metadato.

**Metadatos propuestos**: EFCore · prioridad baja · XS, confianza media · tramo 1 · etiquetas por confirmar.

### US-04 · Documentar la API pública en inglés · prioridad media · S

**Título de issue**: Normalizar a inglés los comentarios XML de la API EntityFrameworkCore

**Como / quiero / para**: Como consumidor de la biblioteca, quiero leer descripciones XML coherentes con las convenciones del repositorio, para entender los contratos desde IntelliSense y el XML generado.

**Contexto**: las interfaces de lectura y paginación tenían summaries/descripciones en español, contra `src/CONTRIBUTING.md`.

**Alcance**: traducir comentarios XML de tipos y miembros públicos; completar el `<summary>` vacío de `Extensions`; no traducir documentación Markdown ni cambiar comportamiento.

**Criterios de aceptación**
- [x] Los comentarios XML de la superficie pública están en inglés y describen contratos; las clases delegadoras heredan de sus interfaces.
- [x] No quedan etiquetas `<c>` vacías en la superficie modificada.
- [x] Los contratos XML traducidos mantienen documentación de parámetros y retornos.
- [x] El build de documentación no añade avisos `CS1591`.

**Pruebas**: build con generación XML y revisión de diagnósticos/documentación emitida.

> **Verificación** · Las interfaces, los helpers públicos y los métodos de las clases concretas `Ef*` tienen XML en inglés. `dotnet build` Release genera la documentación sin avisos `CS1591`.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: Aditiva; no cambia firma ni texto de ejecución.

**Metadatos propuestos**: EFCore · prioridad media · S, confianza baja · tramo 1 · etiquetas por confirmar.

## 5. Orden de ejecución

1. US-01 cierra estrategia y compatibilidad.
2. US-03 y US-04 pueden ejecutarse en paralelo como alineación compatible.
3. US-02 se ejecuta en el tramo de API/migración y depende de US-01.

## 6. Decisiones cerradas y trabajo pendiente

- D-01 · **Se conserva el tipo público `Extensions` sin renombrarlo en 2.0.0.** Se encontraron llamadas de extensión, no llamadas estáticas al tipo.
- D-02 · **No se añade `[Obsolete]` a `Extensions` ni a sus miembros.** No hay transición de nombre; se conserva el puente existente `GetPkValues` → `FormatCompositeKey`.
- D-03 · **Los parámetros genéricos de entidad se renombran a `TEntity`.** La aridad y la firma binaria permanecen iguales; el punto 77 documenta el cambio visible para reflexión.

**Pendiente de cierre:** completar el XML en inglés de los métodos públicos de las clases concretas `Ef*`.

## 7. Definición de terminado

- [x] Los nombres genéricos de entidad siguen `CONTRIBUTING.md`; `Extensions` se conserva por decisión D-01.
- [x] El build y las pruebas existentes verifican que las llamadas de extensión no cambian de resolución.
- [x] El cambio genérico visible por reflexión aparece en el punto 77 y el anexo de renombrados.
- [x] El XML de todos los miembros públicos está en inglés; los métodos delegados heredan la documentación de sus interfaces.
- [x] No se cambia la versión del paquete ni se declara aprobada una ruptura de compilación.

## 8. Apéndice · transición de la clase pública

```csharp
// Actual
public static partial class Extensions
{
	public static string FormatCompositeKey(this object[] source) => /* ... */;
}

// D-01: no se añade otro contenedor en 2.0.0; Extensions conserva sus miembros públicos.
public static partial class Extensions
{
	public static string FormatCompositeKey(this object[] source) => /* implementación actual */;
}
```

## Ver también

- [Índice de épicas EFCore](README.md)
- [Análisis de nomenclatura](../../../../Mejoras/EntityFrameworkCore/01-Nomenclatura-y-Estandares.md)
- [Guía de contribución](../../../../../CONTRIBUTING.md)
- [Prompt de migración](../../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md)
