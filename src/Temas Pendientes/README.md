# Temas Pendientes

> 📌 **Qué es esta carpeta**
> Todo el trabajo de análisis de la solución **MoralesLarios.OOFP**: qué hay que arreglar, en qué orden,
> qué renombrar y qué cambiar para dejar la biblioteca a nivel profesional.
> Son documentos **de trabajo**: se leen, se van marcando las casillas y se van vaciando.

---

## Documentos

| Documento | Contenido | Cuándo leerlo |
|---|---|---|
| [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) | **Puntos 1-37.** Bugs que producen resultados incorrectos, problemas de seguridad, fugas de información, inyección de dependencias mal resuelta, culturas y URLs | **Primero.** Es lo que hay que arreglar ya |
| [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) | **Puntos 38-63.** Rendimiento y acceso a datos, diseño de la API, contratos HTTP, coherencia funcional | Después de cerrar 🔴 y 🟠 |
| [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) | **Puntos 64-89.** Código muerto, erratas en identificadores públicos, mensajes al usuario, documentación incoherente | En paralelo, poco a poco |
| [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) | Tabla de **nombre actual → nombre propuesto** para clases, propiedades, métodos, parámetros genéricos y proyectos, con la estrategia de migración sin romper a nadie | Antes de tocar cualquier nombre público |
| [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) | Higiene del repositorio, `Directory.Build.props`, gestión central de paquetes, analizadores, estrategia de pruebas (incluidas las **leyes monádicas**), API pública congelada, metadatos NuGet, SemVer y CI/CD | Al empezar: es lo que menos cuesta y más se nota |
| [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md) | Tamaño de la superficie pública, asincronía, cancelación, modelo de errores, rendimiento de `MlResult<T>`, interoperabilidad con .NET moderno, capas, observabilidad, capa web, documentación como producto y comunidad | Para decidir el rumbo de la **versión 2.0** |

---

## Cómo están escritos los inventarios de mejoras

Cada punto sigue siempre la misma plantilla, para que se pueda atacar sin volver a investigar:

```text
- [ ] **N. Título breve del problema**
    - Proyecto:        en qué proyecto de la solución está
    - Archivo / clase: dónde exactamente
    - Miembro:         método o propiedad concretos (si aplica)
    - Problema:        qué hace mal el código, de forma objetiva
    - Impacto:         qué consecuencia real tiene
    - Propuesta:       cómo arreglarlo
```

La casilla `- [ ]` se marca como `- [x]` al cerrarlo. Así el propio documento es el seguimiento.

### Criterios de prioridad

| Prioridad | Criterio |
|---|---|
| 🔴 **Crítica** | Produce **resultados incorrectos**, cuelga, o el tipo es imposible de usar. Se arregla antes de cualquier otra cosa |
| 🟠 **Alta** | Riesgo de **seguridad o privacidad**, bloqueos de hilos, contratos rotos entre capas, dependencias mal registradas |
| 🟡 **Media** | Se paga **cuando crece el volumen** o hace la API difícil de usar correctamente |
| 🟢 **Baja** | Limpieza, erratas, mensajes y documentación. No rompe nada, pero es lo que da la sensación de calidad |

---

## Resumen global

| Prioridad | Puntos | Numeración |
|---|---:|---|
| 🔴 Crítica | 16 | 1-16 |
| 🟠 Alta | 21 | 17-37 |
| 🟡 Media | 26 | 38-63 |
| 🟢 Baja | 26 | 64-89 |
| **Total** | **89** | |

### Por proyecto (orientativo)

| Proyecto | 🔴 | 🟠 | 🟡 | 🟢 |
|---|---:|---:|---:|---:|
| `MoralesLarios.OOFP` (núcleo) | 2 | 0 | 1 | 3 |
| `MoralesLarios.OOFP.ValueObjects` | 4 | 1 | 2 | 3 |
| `MoralesLarios.OOFP.ValueObjects.IO` | 2 | 1 | 0 | 1 |
| `MoralesLarios.OOFP.Validation(.*)` | 2 | 1 | 4 | 2 |
| `MoralesLarios.OOFP.EFCore` | 1 | 4 | 3 | 2 |
| `MoralesLarios.OOFP.WebServices` | 1 | 3 | 2 | 2 |
| `MoralesLarios.OOFP.WebApi` | 1 | 3 | 0 | 2 |
| `MoralesLarios.OOFP.WebControllers` | 1 | 5 | 6 | 4 |
| `MoralesLarios.OOFP.HttpClients` | 2 | 3 | 8 | 6 |
| `MoralesLarios.OOFP.Internals` | 0 | 0 | 0 | 1 |

---

## Plan de trabajo sugerido

La idea es que **cada bloque se pueda publicar**, sin quedarse a medias.

### Bloque 0 — Preparar el terreno (1-2 días, sin tocar comportamiento)

1. `LICENSE` en la raíz (**hoy falta**, y sin él nadie puede adoptar la biblioteca).
2. Borrar `MoralesLarios.OOFP - copia.sln`, los `bin/` y `obj/` versionados y los archivos vacíos.
3. `Directory.Build.props` + `Directory.Packages.props` (ver el documento de ingeniería).
4. Analizadores en **modo advertencia**, `.editorconfig` y `nullable enable` en todos los proyectos.
5. CI mínima: compilar y ejecutar pruebas en Linux y Windows.

> Nada de esto cambia el comportamiento, y a partir de aquí **el compilador ayuda** en todo lo demás.

### Bloque 1 — Corrección (🔴, versión de parche)

Los 16 puntos críticos, **cada uno con su prueba de regresión**. Empezar por los dos del núcleo
(el `return` que falta en la fusión de errores y el `Async` que no espera la tarea), porque afectan a
todo lo que se apoya en ellos. Después los *value objects* imposibles de construir y las validaciones
que se saltan.

Añadir aquí las **pruebas de leyes monádicas** y la **paridad sync/async**: habrían detectado dos de
estos bugs solos.

### Bloque 2 — Fiabilidad (🟠, versión de parche o menor)

Primero seguridad y privacidad (dejar de volcar cuerpos de respuesta en los mensajes, eliminar los
`.Result` bloqueantes), después inyección de dependencias (fuera el `BuildServiceProvider()`, el
`DbContext` que se libera solo y la dependencia cautiva), y por último culturas, formatos y URLs.

Cerrar con un test que construya el `ServiceProvider` con `ValidateScopes` y `ValidateOnBuild`: es una
sola prueba que cubre varios de estos puntos a la vez.

### Bloque 3 — Rendimiento y contratos (🟡, versión menor)

El acceso a datos primero (`TryLast*`, el conflicto de seguimiento de EF Core, la paginación
obligatoria y la unidad de trabajo), porque es donde el usuario nota la diferencia. Después los
contratos HTTP: rutas coherentes, `[ProducesResponseType]`, el estado HTTP como dato y no como texto.

### Bloque 4 — Nomenclatura y diseño (versión **2.0**)

Aquí van los cambios rompedores, todos juntos y documentados en un `CHANGELOG.md` con guía de
migración: los renombrados de `Consejos-Nomenclatura.md`, `CancellationToken` en toda la API
asíncrona, códigos de error tipados, opciones configurables en lugar de literales, y la capa web
modernizada.

Regla de oro: **nada rompedor fuera de una versión mayor**, y siempre con `[Obsolete]` durante un
ciclo completo antes de retirar nada.

### Continuo — Limpieza y producto (🟢)

Los puntos de prioridad baja se van cerrando en cualquier momento, idealmente aprovechando cada vez
que se toca un archivo por otro motivo. En paralelo, lo que hace que la biblioteca **se encuentre y se
entienda**: sitio de documentación publicado, ejemplos que compilen en CI, plantilla `dotnet new` y
`README.md` con un ejemplo de quince líneas.

---

## Orden de lectura recomendado

1. Este `README.md`.
2. [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) — el Bloque 0 sale de aquí.
3. [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — a trabajar.
4. [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) y [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) — según vaya cerrando.
5. [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) y [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md) — para planificar la 2.0.

---

## Ver también

- [`../README.md`](../README.md) — visión general de la solución y sus proyectos.
- [`../MoralesLarios.FOOP/README.md`](../MoralesLarios.FOOP/README.md) — biblioteca núcleo.
- [`../MoralesLarios.FOOP/__Doc/1_Intro.md`](../MoralesLarios.FOOP/__Doc/1_Intro.md) — documentación funcional completa.
