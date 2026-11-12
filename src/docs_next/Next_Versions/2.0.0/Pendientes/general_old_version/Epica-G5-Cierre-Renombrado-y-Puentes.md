# Épica G5 · Cierre del renombrado 2.0.0 y política de puentes `[Obsolete]`

> **Versión destino:** 2.0.0 (cierre y decisión) · 3.0.0 (retirada de puentes, si se mantienen)
> **Proyecto:** transversal (documentación, `*.Obsolete.cs` y herramientas de inventario)
> **Origen:** `Plan-Renombrados-2.0.0.md` (4 criterios de cierre) · `Consejos-Nomenclatura.md` · INV-18, 72, 73 · [auditoría](02-Auditoria-Documentos-Complementarios.md#2-consejos-nomenclaturamd-y-plan-renombrados-200md)
> **Estado:** ⬜ No iniciada
> **Esfuerzo:** M · **Riesgo:** 🟡 medio (la decisión de puentes cambia la promesa de migración gradual)
> **Compatibilidad:** Depende de D-G5-1: **retirar** puentes es una ruptura de compilación; **conservarlos** no cambia nada

---

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Historias de usuario](#2-historias-de-usuario)
3. [Orden de ejecución](#3-orden-de-ejecución)
4. [Decisiones abiertas](#4-decisiones-abiertas)
5. [Definición de terminado](#5-definición-de-terminado)

---

## 1. Contexto y problema

El renombrado 2.0.0 está ejecutado: el inventario cifra **904 cambios** (885 de nomenclatura y 19 de contrato) y el
prompt de migración documenta **187 puentes `[Obsolete]`** con el mensaje «Will be removed in 3.0.0». Pero:

- `Plan-Renombrados-2.0.0.md` mantiene **cuatro criterios de cierre sin verificar**.
- `Consejos-Nomenclatura.md` los da por completos, **contradiciendo** al plan y al código.
- Todo el trabajo ocurre **antes de la primera publicación de la 2.x**. Un puente solo protege a quien conoce el
  nombre antiguo: los puentes de **nombres 1.x publicados** son útiles a los consumidores que migran; los que
  sustituyen **nombres intermedios de la propia rama 2.0** (renombrados dos veces durante el desarrollo, nunca
  publicados) no protegen a nadie y solo añaden ruido.
- El prompt promete la «migración gradual» como alternativa a la completa; eso **depende** de que los puentes existan.

## 2. Historias de usuario

### G5-US-01 · Verificar los cuatro criterios de cierre · S

> **Como** mantenedor **quiero** comprobar que el renombrado cumple su propia definición de terminado **para** dar
> por cerrado un trabajo de 904 cambios con evidencia.

**Criterios de aceptación**

- [ ] **A** · `dotnet build -c Release --no-incremental` y recuento de `CS0618` en **código propio** (proyectos y tests): objetivo 0 fuera de los tests de contrato de los puentes.
- [ ] **B** · `dotnet test -c Release`: **1.715 tests** pasados hoy (línea base de este plan). El número y los nombres de los tests no bajan.
- [ ] **C** · `src/scripts/inventario-nomenclatura.ps1` y `check-old-names.ps1`: los nombres antiguos solo aparecen en `*.Obsolete.cs`, en `docs_next/Cambios Realizados/` y en el prompt de migración.
- [ ] **D** · Barrido Markdown de nombres antiguos fuera de las carpetas históricas (`Cambios Realizados`, `Temas Pendientes`, `Realizados`).
- [ ] Los cuatro resultados se anotan en `Plan-Renombrados-2.0.0.md` junto al commit verificado.

**Compatibilidad:** Ninguna.

### G5-US-02 · Política de puentes `[Obsolete]` · M

**Criterios de aceptación**

- [ ] Decisión D-G5-1 por **familia de puentes**: conservar hasta 3.0.0, o retirar antes de publicar.
- [ ] Inventario de los 187 puentes agrupados por proyecto y tipo, distinguiendo los que **protegen un nombre 1.x publicado** de los que sustituyen un nombre intermedio de la rama 2.0 (comprobable con el anexo `oofp-upgrade-2.renames.md` y el historial de git).
- [ ] Si se retiran: el prompt de migración pasa de «migración gradual recomendada» a «migración completa obligatoria» en la Fase 4.1 y se actualizan `README.md` de Copilot y los números de puentes.
- [ ] Si se conservan: cada puente lleva `EditorBrowsable(Never)` y el texto «Will be removed in 3.0.0» (ya vigente) y se crea la lista de retirada para 3.0.0.
- [ ] Casos concretos: INV-72 (`ChangeReturnResultAlwais*`), INV-73 (`Lenght`), INV-18 (puente síncrono bloqueante de lectura de errores) y los `By…` de G4-US-06.

**Compatibilidad:** **Compilación** si se retiran; ninguna si se conservan.

### G5-US-03 · Reconciliar la documentación histórica · S

**Criterios de aceptación**

- [ ] Cada documento de `Temas Pendientes/` recibe un **aviso de histórico** de dos líneas al principio que enlaza con este plan; **el contenido no se reescribe**.
- [ ] `Consejos-Nomenclatura.md` deja de afirmar que todos los niveles están hechos y remite al estado de [`02-Auditoria`](02-Auditoria-Documentos-Complementarios.md).
- [ ] Se corrige la contradicción entre `Epica-Simplificacion-Repositorios-EFCore.md` (L684, retira `BuildServiceProvider`) y `Epica-Unidad-de-Trabajo-y-Transacciones.md` (L59, afirma que se conserva).
- [ ] Las épicas desactualizadas ([lista](01-Auditoria-Inventario-89-Puntos.md#3-correcciones-al-inventario-original)) corrigen las líneas y afirmaciones citadas.
- [x] Se borra `Temas Pendientes/_Seccion-para-README.md` (su contenido ya está en `src/README.md`).

### G5-US-04 · Nombre del repositorio y de la carpeta raíz · N6 · XS

**Criterios de aceptación**

- [ ] Decisión D-G5-2: el repositorio y la carpeta raíz conservan `FOOP` o pasan a `OOFP`.
- [ ] Si cambia: se actualizan scripts, plantilla de PR, workflows y enlaces; si no cambia, se documenta el motivo en `CONTRIBUTING.md`.

**Compatibilidad:** Ninguna para la API; afecta a rutas y herramientas.

### G5-US-05 · Una sola épica por estado · XS

**Criterios de aceptación**

- [ ] `Pendientes/Epica-Limpieza-Codigo-Muerto.md` (sin seguimiento en git, **distinto** del archivado) se compara con `Realizados/Http/Epica-Limpieza-Codigo-Muerto.md`: lo que aporte se integra en la archivada y la copia de `Pendientes/` se elimina **con tu confirmación**.
- [ ] Ninguna épica completada permanece en `Pendientes/` (regla `epicas-completadas.instructions.md`).
- [ ] Los índices de `Pendientes/` y de `2.0.0/` coinciden con el contenido real de las carpetas.

---

## 3. Orden de ejecución

```mermaid
graph LR
    US01["US-01 · Criterios A-D<br/>S"] --> US02["US-02 · Política de puentes<br/>M"]
    US02 --> US03["US-03 · Documentación histórica<br/>S"]
    US04["US-04 · Nombre del repositorio<br/>XS"] --> US03
    US05["US-05 · Una épica por estado<br/>XS"] --> US03
```

| Orden | Historia | Tamaño | Hito |
|---:|---|:---:|---|
| 1 | US-05 · Una épica por estado | XS | |
| 2 | US-01 · Criterios A-D | S | 🎯 Renombrado verificado |
| 3 | US-02 · Política de puentes | M | 🎯 Promesa de migración decidida |
| 4 | US-04 · Nombre del repositorio | XS | |
| 5 | US-03 · Documentación histórica | S | 🎯 Una sola fuente de verdad |

**Va después de [G4](Epica-G4-ValueObjects-Nomenclatura-y-Diseno.md)** salvo US-05 y US-01 (parte A), que se pueden hacer antes.

## 4. Decisiones abiertas

| Id | Decisión | Opciones | Recomendación |
|---|---|---|---|
| D-G5-1 | Puentes `[Obsolete]` | (a) conservar los 187 hasta 3.0.0 · (b) retirar los que sustituyen nombres intermedios nunca publicados · (c) retirarlos todos | **(b)**: se conserva lo que protege a consumidores reales de 1.x y se elimina el ruido |
| D-G5-2 | Nombre de repositorio y carpeta raíz | `FOOP` · `OOFP` | Conservar `FOOP` si renombrar rompe enlaces externos; documentar |

## 5. Definición de terminado

- [ ] Los cuatro criterios A-D cumplidos y registrados en `Plan-Renombrados-2.0.0.md`.
- [ ] Política de puentes decidida, aplicada y reflejada en el prompt de migración (los seis sitios si se retira algo).
- [ ] `Temas Pendientes/` con aviso de histórico; contradicciones documentales corregidas.
- [ ] `check-migration-prompt.ps1`, `check-md-links2.ps1` y `check-md-anchors4.ps1` sin errores.
- [ ] Build Release `--no-incremental` sin errores y tests en verde.
