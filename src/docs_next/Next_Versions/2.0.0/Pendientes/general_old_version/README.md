# general_old_version · Plan para cerrar lo que falta del inventario general antiguo

> **Origen:** [`Next_Versions/2.0.0/Realizados/Temas Pendientes/`](../../Realizados/Temas%20Pendientes/README.md) · el inventario general escrito **antes** del renombrado 2.0.0
> **Auditado contra:** commit `dab05b4` (rama `release/2.0.0-nomenclatura`, 2026-11-12) · auditoría **estática** (lectura y búsqueda en código y tests; los tests de regresión citados existen, no se volvieron a ejecutar en esta auditoría)
> **Versión destino:** 2.0.0 (nada publicado todavía) · el 3.0.0 solo recibe lo explícitamente diferido
> **Estado:** ⬜ Plan creado · 0 de 6 épicas nuevas iniciadas

---

## 1. Qué es esta carpeta y por qué existe

`Temas Pendientes/` reúne el análisis original: 89 puntos de mejora, la propuesta de nomenclatura, el
análisis de `ValueObjects`, los consejos sobre `MlResult` y dos documentos de profesionalización. Se
escribió con los nombres 1.x (`EFCore`, `WebServices`, `HttpClients`…) y **ya no refleja el código**:

- el renombrado 2.0.0 cambió proyectos, clases y miembros;
- varias épicas de `Next_Versions/2.0.0/` ya resolvieron parte del inventario;
- casi ninguna casilla `- [ ]` del original se actualizó (aparecen sin marcar puntos que ya están hechos).

Esta carpeta **verifica el estado real** y convierte lo que queda en un plan ejecutable **sin duplicar** las
siete épicas pendientes que ya existen en [`Pendientes/`](../README.md).

## 2. Resultado de la auditoría

### 2.1 Inventario de 89 puntos

| Bloque | Puntos | ✅ Hecho | 🟨 Parcial | ⬜ Pendiente |
|---|---:|---:|---:|---:|
| 🔴 Crítica | 1-16 | 14 | 2 | 0 |
| 🟠 Alta | 17-37 | 9 | 7 | 5 |
| 🟡 Media | 38-63 | 4 | 5 | 17 |
| 🟢 Baja | 64-89 | 15 | 10 | 1 |
| **Total** | **89** | **42** | **24** | **23** |

Detalle punto a punto, con evidencia y destino: [`01-Auditoria-Inventario-89-Puntos.md`](01-Auditoria-Inventario-89-Puntos.md).

### 2.2 Documentos complementarios

| Documento original | Tareas | ✅ | 🟨 | ⬜ | Dónde se trata lo pendiente |
|---|---:|---:|---:|---:|---|
| `Analisis-Nomenclatura-ValueObjects.md` (N1-N21) | 21 | 1 | 10 | 10 | [G4](Epica-G4-ValueObjects-Nomenclatura-y-Diseno.md) |
| `consejos_mejora_mlresult.md` (MR1-MR23) | 23 | 7 | 10 | 6 | [G3](Epica-G3-Nucleo-MlResult-Robustez.md) |
| `Revision-Map-Bind-ExecSelf.md` | 16 | 12 | 3 | 1 | [G3](Epica-G3-Nucleo-MlResult-Robustez.md) |
| `PendingTasks-legacy.md` | 24 ideas | 14 | 7 | 3 | [Backlog y descartes](04-Backlog-3.0-y-Descartados.md) |
| `Consejos-Nomenclatura.md` + `Plan-Renombrados-2.0.0.md` | 4 criterios de cierre | 0 | 0 | 4 | [G5](Epica-G5-Cierre-Renombrado-y-Puentes.md) |
| `Profesionalizacion-Ingenieria-y-Calidad.md` | 17 tareas | 3 | 7 | 7 | [G1](Epica-G1-Higiene-Repositorio-CI-y-Publicacion.md) · [G6](Epica-G6-Pruebas-Calidad-y-Rendimiento.md) |
| `Profesionalizacion-Diseno-API-y-Producto.md` | 20 líneas de trabajo | 0 | 12 | 8 | [Backlog 3.0](04-Backlog-3.0-y-Descartados.md) |

Detalle: [`02-Auditoria-Documentos-Complementarios.md`](02-Auditoria-Documentos-Complementarios.md).

### 2.2.1 Cobertura de los 15 documentos de `Temas Pendientes/`

| Documento | Tratamiento |
|---|---|
| `Mejoras-Prioridad-Critica-y-Alta.md` (1-37) | Auditado punto a punto en [`01`](01-Auditoria-Inventario-89-Puntos.md) §2.1-2.2 |
| `Mejoras-Prioridad-Media.md` (38-63) | Auditado en [`01`](01-Auditoria-Inventario-89-Puntos.md) §2.3 |
| `Mejoras-Prioridad-Baja.md` (64-89) | Auditado en [`01`](01-Auditoria-Inventario-89-Puntos.md) §2.4 |
| `Plan-Ejecucion-Prioridad-Media.md` | Su auditoría previa (2 ✅, 5 🟨, 19 ⬜, commit `9ffd376`) **se reauditó**: hoy hay 4 ✅, 5 🟨 y 17 ⬜, porque las épicas de EFCore cerraron los puntos 39 y 40. Sus fases 1-5 y su orden ya están absorbidos por las épicas de [`Pendientes/`](../README.md) y por el [plan maestro](03-Plan-Maestro.md). Es histórico |
| `Analisis-Nomenclatura-ValueObjects.md` | [`02`](02-Auditoria-Documentos-Complementarios.md) §1 → [G4](Epica-G4-ValueObjects-Nomenclatura-y-Diseno.md) |
| `Consejos-Nomenclatura.md`, `Plan-Renombrados-2.0.0.md` | [`02`](02-Auditoria-Documentos-Complementarios.md) §2 → [G5](Epica-G5-Cierre-Renombrado-y-Puentes.md) |
| `consejos_mejora_mlresult.md` | [`02`](02-Auditoria-Documentos-Complementarios.md) §3 → [G3](Epica-G3-Nucleo-MlResult-Robustez.md) |
| `Revision-Map-Bind-ExecSelf.md` | [`02`](02-Auditoria-Documentos-Complementarios.md) §4 → [G3](Epica-G3-Nucleo-MlResult-Robustez.md) |
| `PendingTasks-legacy.md` | [`02`](02-Auditoria-Documentos-Complementarios.md) → [Backlog](04-Backlog-3.0-y-Descartados.md) |
| `Profesionalizacion-Ingenieria-y-Calidad.md`, `Profesionalizacion-Diseno-API-y-Producto.md` | [`02`](02-Auditoria-Documentos-Complementarios.md) → [G1](Epica-G1-Higiene-Repositorio-CI-y-Publicacion.md), [G6](Epica-G6-Pruebas-Calidad-y-Rendimiento.md), [Backlog](04-Backlog-3.0-y-Descartados.md) |
| `README.md` | Índice de la carpeta; su resumen de 89 puntos es la fuente que se auditó |
| `_MOVIDOS.md` | Nota de trazabilidad sin tareas. Sigue siendo correcta |
| `_Seccion-para-README.md` | Sin tareas: era un fragmento temporal y su sección **ya está pegada** en `src/README.md` (L264). Es un resto que debe borrarse ([G5-US-03](Epica-G5-Cierre-Renombrado-y-Puentes.md)) |

### 2.3 Estado de las épicas que ya existían

| Épica pendiente | US | Criterios hechos | Cubre inventario |
|---|---:|---:|---|
| [Contrato REST](../Epica-Contrato-Rest.md) | 11 | 0 / 74 | INV-43, 48-52, 60, 61 |
| [Serialización y errores HTTP](../Epica-Serializacion-y-Errores-Http.md) | 11 | 0 / 61 | INV-42, 44-47, 63 |
| [Servicio y paginación](../Epica-Servicio-y-Paginacion.md) | 9 | 0 / 48 | INV-41, 53, 54 |
| [Validación](../Epica-Validacion.md) | 10 | 0 / 60 | INV-56-59 |
| [Erratas públicas](../Epica-Erratas-Identificadores-Publicos.md) | 7 | 31 / 44 | INV-71-76 |
| [Mensajes y localización](../Epica-Mensajes-Y-Localizacion.md) | 7 | 9 / 55 | INV-77-80 |
| [Coherencia y documentación](../Epica-Coherencia-Y-Documentacion.md) | 7 | 14 / 51 | INV-81-89 |

**Conclusión:** el trabajo de fondo que sigue abierto (contrato REST, serialización/errores HTTP, servicio y
paginación, validación) ya está planificado, pero con **0 criterios marcados**. Lo que **no** estaba planificado
en ninguna parte, y por eso nace este plan, son 12 puntos de prioridad alta/media, los 20 puntos de nomenclatura
de `ValueObjects`, la robustez del núcleo `MlResult`, el cierre formal del renombrado y toda la infraestructura
de ingeniería (CI, licencia, secretos, publicación).

## 3. Las seis épicas nuevas

| Épica | Resultado verificable | Tamaño | Riesgo | Rompe compatibilidad |
|---|---|:---:|:---:|---|
| [G1 · Higiene del repositorio, CI y publicación](Epica-G1-Higiene-Repositorio-CI-y-Publicacion.md) | Credenciales fuera del repo, `LICENSE`, `global.json`, CI de build y tests, metadatos NuGet | L | 🟢 | No |
| [G2 · Brechas de Http, EFCore y controladores](Epica-G2-Brechas-Http-EFCore-y-Controladores.md) | 12 puntos 17-38 sin historia en ninguna épica quedan resueltos | L | 🟠 | Sí, varias silenciosas |
| [G3 · Robustez del núcleo `MlResult`](Epica-G3-Nucleo-MlResult-Robustez.md) | Invariantes de `MlErrorDetails`, excepciones y ambigüedades cubiertas por tests | M | 🟠 | Sí, silenciosa y de compilación |
| [G4 · `ValueObjects`: nomenclatura y diseño](Epica-G4-ValueObjects-Nomenclatura-y-Diseno.md) | Los 20 puntos N pendientes decididos y aplicados **antes** de publicar | XL | 🔴 | Sí, de compilación |
| [G5 · Cierre del renombrado y puentes `[Obsolete]`](Epica-G5-Cierre-Renombrado-y-Puentes.md) | Criterios A-D del plan de renombrados cumplidos y política de puentes decidida | M | 🟡 | Depende de la decisión |
| [G6 · Pruebas, calidad y rendimiento](Epica-G6-Pruebas-Calidad-y-Rendimiento.md) | Red de seguridad de regresión, leyes monádicas, cobertura y *benchmarks* | L | 🟢 | No |

Complementos: [`03-Plan-Maestro.md`](03-Plan-Maestro.md) (orden, dependencias, solapes, definición de terminado y
decisiones del propietario) y [`04-Backlog-3.0-y-Descartados.md`](04-Backlog-3.0-y-Descartados.md).

> **Leyenda de tamaño:** XS (<2 h) · S (½ jornada) · M (1 jornada) · L (2 jornadas) · XL (3+ jornadas)
> **Leyenda de riesgo:** 🟢 bajo · 🟡 medio · 🟠 alto · 🔴 muy alto (rompe compatibilidad)

## 4. Convenciones de este plan (mejoras sobre el original)

1. **Dos numeraciones, dos prefijos.** El inventario antiguo y el prompt de migración usan números que
   **colisionan** (el «punto 78» del inventario es *mensajes de controladores*; el «punto 78» del prompt es *el
   logger REST*). Aquí: **`INV-nn`** = punto del inventario original; **`MIG-nn`** = punto del prompt
   [`oofp-upgrade-2.prompt.md`](../../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md) §3.3.
2. **Estado verificable, no declarado.** Cada fila cita fichero y línea actuales; «hecho» significa que el
   defecto descrito ya no existe en el código, no que una épica lo diga.
3. **Un punto, un destino.** Todo punto no resuelto apunta a **una** historia (existente o nueva). Nada queda
   sin dueño ni duplicado.
4. **Primero la red de seguridad.** Lo que va a cambiar de comportamiento recibe antes su prueba de regresión
   (G6), para que el cambio se vea en el diff de tests.
5. **La ventana de rotura es ahora.** Nada se ha publicado: cambiar nombres o contratos cuesta lo mismo hoy que
   en la 3.0.0 **más** una migración extra para los consumidores. Por eso las decisiones de nombres (G4, G5)
   van antes de la primera publicación, que sigue siendo decisión del propietario.
6. **Los números de migración no se reservan.** Cada ruptura recibirá su número `MIG-nn` **al implementarse**
   (el último usado hoy es el 84). Las épicas solo indican *qué* hay que registrar en los seis sitios.
7. **Las referencias antiguas se traducen.** Tabla de equivalencias en
   [`01-Auditoria-Inventario-89-Puntos.md`](01-Auditoria-Inventario-89-Puntos.md#4-equivalencias-de-nombres).

## 5. Qué NO hace este plan

- No toca código ni versiones; no autoriza publicar.
- No modifica los documentos de `Temas Pendientes/` (se proponen *avisos de histórico* en G5-US-03).
- No sustituye a las siete épicas existentes: las **ordena** junto a las nuevas en el
  [plan maestro](03-Plan-Maestro.md).
- No es una auditoría dinámica: no se midió rendimiento, cobertura ni se ejecutó análisis de seguridad.

## 6. Hallazgos que requieren atención inmediata

| Hallazgo | Evidencia | Destino |
|---|---|---|
| Contraseñas no vacías en configuración de pruebas **versionada** | `appsettings.test.json` de `Configuration.Tests.Unit` y de `EntityFrameworkCore.Tests.Integration` (3 coincidencias) además de nombres de equipo personales | [G1-US-01](Epica-G1-Higiene-Repositorio-CI-y-Publicacion.md) |
| No hay `LICENSE`, `global.json`, `CHANGELOG.md`, `SECURITY.md` ni `Directory.Packages.props` | `git ls-files` sin resultados | G1 |
| El único *workflow* no compila ni ejecuta tests | `.github/workflows/prompt-migracion.yml` | G1-US-04 |
| 60 enlaces Markdown relativos y 1 ancla rotos en la documentación existente | `src/scripts/check-md-links2.ps1` y `check-md-anchors4.ps1` | G1-US-08 |
| Un borrador de épica «completada» sigue en `Pendientes/` y difiere del archivado | `Epica-Limpieza-Codigo-Muerto.md` (sin seguimiento en git) frente a `Realizados/Http/` | G5-US-05 |
