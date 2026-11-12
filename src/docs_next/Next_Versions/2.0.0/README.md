# Versión 2.0.0 · Planificación centralizada

> **Estado del producto** · 🔒 **todos los paquetes congelados en `2.0.0`** · nada publicado todavía
> **Objetivo** · dejar cerrada una **única** versión `2.0.0` antes de publicar el primer paquete
> **Regla de versionado** · ⛔ **ninguna versión se sube sin preguntar antes al propietario**

---

## 1. Por qué existe esta carpeta

Durante el desarrollo se fueron creando carpetas de planificación por versión
(`2.1`, `2.1.1`, `2.2.0`, `2.3.0`, `2.4.0`) como si cada tanda de trabajo fuera a publicarse por
separado. **Ninguna de esas versiones llegó a publicarse**: los 17 proyectos que declaran versión
siguen —y seguirán— en `<Version>2.0.0</Version>`.

Mantener esa numeración resultaba engañoso: el historial hablaba de versiones que ningún consumidor
ha visto nunca. Por eso toda la planificación se ha centralizado **aquí**, en una sola carpeta
`2.0.0`, con dos cajones:

| Carpeta | Qué contiene |
| --- | --- |
| [`Pendientes/`](Pendientes/README.md) | épicas y trabajos **por hacer** |
| [`Realizados/`](#3-realizados--trabajo-ya-implementado) | épicas y trabajos **ya implementados** y publicados en la rama |

> 🔑 La reorganización de estos documentos no cambia código por sí sola. Las épicas archivadas sí
> reflejan cambios implementados antes de la primera publicación de los paquetes `2.0.0`.

---

## 2. Pendientes · trabajo por hacer

Plan de ejecución en [`Pendientes/README.md`](Pendientes/README.md) · **7 épicas pendientes, 3 parciales**.

| Épica | Proyecto principal |
| --- | --- |
| [Coherencia de contratos y documentación](Pendientes/Epica-Coherencia-Y-Documentacion.md) | `Shared`, `Http`, `AspNetCore.Controllers` |
| [Contrato REST](Pendientes/Epica-Contrato-Rest.md) | `AspNetCore.Controllers` |
| [Erratas en identificadores públicos](Pendientes/Epica-Erratas-Identificadores-Publicos.md) | transversal |
| [Mensajes, idioma e instrumentación](Pendientes/Epica-Mensajes-Y-Localizacion.md) | transversal |
| [Serialización y errores HTTP](Pendientes/Epica-Serializacion-y-Errores-Http.md) | `Http` |
| [Servicio y paginación](Pendientes/Epica-Servicio-y-Paginacion.md) | `Application` |
| [Validación](Pendientes/Epica-Validacion.md) | `Validation*` |

Las épicas EFCore y su orden independiente se detallan en [`Pendientes/EntityFrameworkCore/README.md`](Pendientes/EntityFrameworkCore/README.md). El hallazgo #7 de su análisis está cubierto por la guía del proyecto y la épica UoW ya completada.

### 2.1 Inventario general antiguo · `general_old_version`

Auditoría (2026-11-12) de `Temas Pendientes/` contra el código y plan para lo que sigue abierto, en
[`Pendientes/general_old_version/README.md`](Pendientes/general_old_version/README.md) · **6 épicas nuevas**.

| Épica | Resultado |
| --- | --- |
| [G1 · Higiene del repositorio, CI y publicación](Pendientes/general_old_version/Epica-G1-Higiene-Repositorio-CI-y-Publicacion.md) | Credenciales fuera del repo, licencia, SDK fijado, CI |
| [G2 · Brechas de Http, EFCore y controladores](Pendientes/general_old_version/Epica-G2-Brechas-Http-EFCore-y-Controladores.md) | 12 puntos del inventario sin historia previa |
| [G3 · Robustez del núcleo `MlResult`](Pendientes/general_old_version/Epica-G3-Nucleo-MlResult-Robustez.md) | Invariantes y excepciones |
| [G4 · `ValueObjects`: nomenclatura y diseño](Pendientes/general_old_version/Epica-G4-ValueObjects-Nomenclatura-y-Diseno.md) | 20 puntos N decididos antes de publicar |
| [G5 · Cierre del renombrado y puentes](Pendientes/general_old_version/Epica-G5-Cierre-Renombrado-y-Puentes.md) | Criterios A-D y política de puentes |
| [G6 · Pruebas, calidad y rendimiento](Pendientes/general_old_version/Epica-G6-Pruebas-Calidad-y-Rendimiento.md) | Red de seguridad, leyes, cobertura, *benchmarks* |

El [plan maestro](Pendientes/general_old_version/03-Plan-Maestro.md) ordena estas seis junto a las siete anteriores.

---

## 3. Realizados · trabajo ya implementado

### 3.1 `MoralesLarios.OOFP.Core`

Plan en [`Realizados/Core/README.md`](Realizados/Core/README.md) · **US-01 … US-25**.

| Épica | Estado |
| --- | --- |
| [1 · Constantes, literales y `using` globales](Realizados/Core/Epica-01-Constantes-Literales-y-Usings.md) | ✅ |
| [2 · Higiene interna, código muerto y documentación XML](Realizados/Core/Epica-02-Higiene-Interna-y-Codigo-Muerto.md) | ✅ |
| [3 · Nomenclatura de la superficie pública](Realizados/Core/Epica-03-Miembros-Publicos-y-Firmas.md) | ✅ *(su tramo; el resto es trabajo de `3.0.0`)* |
| [4 · El contenedor de metadatos de `MlErrorDetails`](Realizados/Core/Epica-04-Modelo-de-Metadatos-de-Error.md) | ✅ |

### 3.2 `MoralesLarios.OOFP.Logging`

Planes en [`Realizados/Logging/README.md`](Realizados/Logging/README.md) (épicas 1-7),
[`README-Epica-08.md`](Realizados/Logging/README-Epica-08.md) y
[`README-Epica-09.md`](Realizados/Logging/README-Epica-09.md) · **US-01 … US-46**.

| Épica | Estado |
| --- | --- |
| [1 · Red de seguridad: el proyecto de tests](Realizados/Logging/Epica-01-Red-de-Seguridad.md) | ✅ |
| [2 · Higiene interna y código muerto](Realizados/Logging/Epica-02-Higiene-Interna-y-Codigo-Muerto.md) | ✅ |
| [3 · Corrección funcional y validación](Realizados/Logging/Epica-03-Correccion-Funcional-y-Validacion.md) | ✅ |
| [4 · Nomenclatura y firmas públicas](Realizados/Logging/Epica-04-Nomenclatura-y-Firmas-Publicas.md) | ✅ |
| [5 · Documentación XML](Realizados/Logging/Epica-05-Documentacion-XML.md) | ✅ |
| [6 · Rendimiento y estándar de logging](Realizados/Logging/Epica-06-Rendimiento-y-Estandar-de-Logging.md) | ✅ |
| [7 · Saneamiento del `Sample`](Realizados/Logging/Epica-07-Saneamiento-del-Sample.md) | ✅ |
| [8 · Registro estructurado y plantillas](Realizados/Logging/Epica-08-Registro-Estructurado-y-Plantillas.md) | ✅ |
| [9 · Ámbitos de registro](Realizados/Logging/Epica-09-Ambitos-de-Registro.md) | ✅ |

### 3.3 `MoralesLarios.OOFP.EntityFrameworkCore`

Plan en [`Realizados/EFCore/README.md`](Realizados/EFCore/README.md) · **siete épicas completadas**.

| Épica | Estado |
|---|---|
| [Simplificación de repositorios EF Core](Realizados/EFCore/Epica-Simplificacion-Repositorios-EFCore.md) | ✅ |
| [Unidad de trabajo y transacciones](Realizados/EFCore/Epica-Unidad-de-Trabajo-y-Transacciones.md) | ✅ |
| [Lecturas funcionales y captura de errores](Realizados/EFCore/Epica-Lecturas-Funcionales-y-Errores.md) | ✅ · US-01…03 |
| [Paginación determinista y asíncrona](Realizados/EFCore/Epica-Paginacion-Determinista-y-Asincrona.md) | ✅ · US-01…04 |
| [Selección eficiente del último resultado](Realizados/EFCore/Epica-Last-Eficiente.md) | ✅ · US-01…02 |
| [Registro DI multicontexto y dependencias](Realizados/EFCore/Epica-Registro-DI-Multicontexto.md) | ✅ · US-01…02 |
| [Nomenclatura y documentación de la API EFCore](Realizados/EFCore/Epica-Nomenclatura-y-Documentacion-API.md) | ✅ · US-01…04 |

### 3.4 `MoralesLarios.OOFP.Http`

Plan en [`Realizados/Http/README.md`](Realizados/Http/README.md).

| Épica | Estado |
|---|---|
| [Limpieza de código muerto en Http y controladores](Realizados/Http/Epica-Limpieza-Codigo-Muerto.md) | ✅ · US-01…06 |

### 3.5 Frameworks

Plan en [`Realizados/Framework/README.md`](Realizados/Framework/README.md).

| Épica | Estado |
|---|---|
| [Migración de todos los proyectos a .NET 11](Realizados/Framework/Epica-Actualizacion-Net11.md) | ✅ |

---

## 4. De dónde viene cada documento

Tabla de equivalencia, para interpretar las referencias históricas que aún aparecen en el cuerpo de
los documentos y en los comentarios del código:

| Carpeta antigua | Carpeta actual | Contenido |
| --- | --- | --- |
| `Next_Versions/2.1/` | [`2.0.0/Pendientes/`](Pendientes/README.md) | las 7 épicas que siguen pendientes |
| `Next_Versions/2.1.1/Core/` | [`2.0.0/Realizados/Core/`](Realizados/Core/README.md) | épicas 1-4 del núcleo |
| `Next_Versions/2.2.0/Logging/` | [`2.0.0/Realizados/Logging/`](Realizados/Logging/README.md) | épicas 1-7 de registro |
| `Next_Versions/2.3.0/Logging/README.md` | [`Realizados/Logging/README-Epica-08.md`](Realizados/Logging/README-Epica-08.md) | plan de la épica 8 |
| `Next_Versions/2.4.0/Logging/README.md` | [`Realizados/Logging/README-Epica-09.md`](Realizados/Logging/README-Epica-09.md) | plan de la épica 9 |
| `Next_Versions/2.0.0/Pendientes/Epica-Simplificacion-Repositorios-EFCore.md` | [`Realizados/EFCore/Epica-Simplificacion-Repositorios-EFCore.md`](Realizados/EFCore/Epica-Simplificacion-Repositorios-EFCore.md) | épica completada y archivada por área |
| `Next_Versions/2.0.0/Pendientes/Epica-Unidad-de-Trabajo-y-Transacciones.md` | [`Realizados/EFCore/Epica-Unidad-de-Trabajo-y-Transacciones.md`](Realizados/EFCore/Epica-Unidad-de-Trabajo-y-Transacciones.md) | épica completada y archivada por área |
| `Next_Versions/2.0.0/Pendientes/EntityFrameworkCore/Epica-Lecturas-Funcionales-y-Errores.md` | [`Realizados/EFCore/Epica-Lecturas-Funcionales-y-Errores.md`](Realizados/EFCore/Epica-Lecturas-Funcionales-y-Errores.md) | épica completada y archivada por área |
| `Next_Versions/2.0.0/Pendientes/EntityFrameworkCore/Epica-Paginacion-Determinista-y-Asincrona.md` | [`Realizados/EFCore/Epica-Paginacion-Determinista-y-Asincrona.md`](Realizados/EFCore/Epica-Paginacion-Determinista-y-Asincrona.md) | épica completada y archivada por área |
| `Next_Versions/2.0.0/Pendientes/EntityFrameworkCore/Epica-Last-Eficiente.md` | [`Realizados/EFCore/Epica-Last-Eficiente.md`](Realizados/EFCore/Epica-Last-Eficiente.md) | épica completada y archivada por área |
| `Next_Versions/2.0.0/Pendientes/EntityFrameworkCore/Epica-Registro-DI-Multicontexto.md` | [`Realizados/EFCore/Epica-Registro-DI-Multicontexto.md`](Realizados/EFCore/Epica-Registro-DI-Multicontexto.md) | épica completada y archivada por área |
| `Next_Versions/2.0.0/Pendientes/EntityFrameworkCore/Epica-Nomenclatura-y-Documentacion-API.md` | [`Realizados/EFCore/Epica-Nomenclatura-y-Documentacion-API.md`](Realizados/EFCore/Epica-Nomenclatura-y-Documentacion-API.md) | épica completada y archivada por área |
| `Next_Versions/2.0.0/Pendientes/Epica-Limpieza-Codigo-Muerto.md` | [`Realizados/Http/Epica-Limpieza-Codigo-Muerto.md`](Realizados/Http/Epica-Limpieza-Codigo-Muerto.md) | épica completada y archivada por área |

> ℹ️ Las **XML-docs y los comentarios del código** siguen citando esas versiones antiguas («*de la
> épica 6 de la versión 2.2.0*», «*puentes `[Obsolete]` de la v2.1.1*»). Se han dejado **a propósito**:
> son **relato histórico** de cuándo se hizo cada cosa y reescribirlos obligaría a tocar ficheros de
> producción sin ninguna ganancia para el consumidor. Usa esta tabla para traducirlos.

---

## 5. Cómo se sube de versión a partir de ahora

1. El producto permanece en **`2.0.0`** mientras se cierran las épicas de [`Pendientes/`](Pendientes/README.md).
2. **⛔ Nadie sube la versión por iniciativa propia.** Antes de modificar cualquier
   `<Version>` hay que **preguntar expresamente al propietario** y esperar su confirmación.
3. Cuando se autorice un salto, se sube **en los 17 proyectos a la vez**, para que el conjunto nunca
   quede descuadrado. No hay `Directory.Build.props`: cada `.csproj` declara su propio `<Version>`.
4. Toda ruptura introducida se registra en
   [`oofp-upgrade-2.prompt.md`](../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md) y, si hay
   renombrados, en [`oofp-upgrade-2.renames.md`](../../Copilot/.github/prompts/oofp-upgrade-2.renames.md),
   **en la misma tanda de cambios**. La regla completa está en
   `.github/instructions/oofp-registro-rupturas.instructions.md`.

---

## 6. Documentos relacionados

- [`docs_next/Mejoras/`](../../Mejoras/) · análisis de mejoras por proyecto, origen de las épicas.
- [`docs_next/Temas Pendientes/`](../../Temas%20Pendientes/) · priorización transversal.
- [`docs_next/Copilot/.github/prompts/`](../../Copilot/.github/prompts/) · prompts de migración para el consumidor.
