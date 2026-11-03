<!-- Plantilla de PR · MoralesLarios.OOFP -->

## Qué cambia y por qué

<!-- Explica el cambio en castellano. Si hay historia de usuario o épica, cítala: (US-11), (epica 04, US-17..US-25). -->

## 🎯 Prompt de migración 1.x → 2.x (OBLIGATORIO)

El prompt [`oofp-upgrade-2.prompt.md`](../src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md) debe poder
migrar **cualquier** proyecto 1.x a la 2.x **sin incidencias**. Solo funciona si se actualiza con **cada** cambio
que rompa compatibilidad. Marca una opción:

- [ ] **Este cambio NO rompe nada** (es puramente interno, o es una corrección que no altera ningún comportamiento
      observable). Lo he comprobado revisando: firmas, nombres, textos de mensajes, URL y cabeceras, orden/recuento de
      consultas, copia frente a mutación, igualdad.
- [ ] **Este cambio rompe compatibilidad** (compilación, contrato **silencioso**, binaria o puente `[Obsolete]`), y está
      registrado **en este mismo PR** en los **seis sitios**:
  - [ ] Fila en la tabla de la §3.3.
  - [ ] Subsección `#### N` con el antes/después, **qué debe hacer el consumidor** y, si es silenciosa, **cómo detectarla**.
  - [ ] Barrido de búsqueda de la Fase 5 (punto 4).
  - [ ] Lista del informe final de la Fase 6 (punto 5).
  - [ ] Tabla del apéndice «Versiones posteriores a la 2.0.0».
  - [ ] Anexo [`oofp-upgrade-2.renames.md`](../src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.renames.md) si hay renombrados.

### Cambios que no son míos o que vienen de antes

- [ ] Si este PR integra, continúa o resuelve el trabajo de **otra persona**, he comprobado que sus rupturas están en el prompt.
- [ ] Si he visto una ruptura **antigua** sin registrar, la he registrado aquí o la he anotado en la descripción.

### Comprobación automática

```powershell
cd src
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-migration-prompt.ps1 -BaseRef origin/<rama-destino>
```

- [ ] El script termina con **0 errores**.

## Resto de la lista

- [ ] Commits con **Conventional Commits** en castellano; `!` si hay ruptura.
- [ ] `dotnet build MoralesLarios.OOFP.sln -c Release --no-incremental` → **0 errores** y **0 `CS1591`**.
- [ ] `dotnet test MoralesLarios.OOFP.sln -c Release --no-build` en verde.
- [ ] Si hay épica: criterios de aceptación marcados y estado actualizado.
