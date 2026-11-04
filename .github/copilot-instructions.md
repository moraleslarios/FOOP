# Copilot Instructions

## Project Guidelines
- El usuario prefiere que las respuestas se den en castellano, y que **todos los documentos** (prompts,
  instrucciones, análisis, épicas) se escriban y traten en castellano.

## 🚨 Archivar épicas completadas

Al cerrar una épica de `docs_next/Next_Versions/2.0.0/`, comprueba sus criterios de aceptación y su
definición de terminado. Mueve las épicas completas de `Pendientes/` a `Realizados/<Área>/` con `git mv`;
mantén las parciales en `Pendientes/`. Actualiza ambos índices de la versión, el índice del área, los
estados y todos los enlaces relativos, y ejecuta el verificador de anclajes Markdown.

📄 Detalle y checklist: `.github/instructions/epicas-completadas.instructions.md`

## 🎯 EL PROMPT DE MIGRACIÓN ES EL ENTREGABLE MÁS IMPORTANTE DE ESTE REPOSITORIO

`src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md` (y su anexo
`oofp-upgrade-2.renames.md`) existe para que **un agente pueda migrar cualquier proyecto que use la
versión 1.x a la 2.x SIN INCIDENCIAS**: sin errores de compilación que el prompt no haya previsto, sin
cambios de comportamiento que el prompt no haya avisado, sin intervención manual inesperada.

**Esa promesa solo se cumple si el prompt está completo y al día.** Por eso:

1. **Se rellena y se actualiza en CADA cambio que rompa compatibilidad**, en la misma tanda de cambios que
   el código, aunque el usuario no lo mencione. Rompe compatibilidad: romper la compilación, cambiar el
   contrato (**incluidos los cambios silenciosos que el compilador no detecta**: textos, URL, orden de
   consultas, copia frente a mutación…), la compatibilidad binaria, o añadir un puente `[Obsolete]`.
2. **Los cambios pasados también cuentan.** Si al trabajar en cualquier zona detectas una ruptura que
   **nunca se registró** (en un commit antiguo, en una rama, en una PR ya fusionada), **regístrala tú**
   antes de seguir con lo que estabas haciendo, o avisa al usuario si no puedes verificarla.
3. **Los cambios futuros de otros contribuyentes también cuentan.** Cuando revises, integres, resuelvas o
   continúes el trabajo de otra persona (PR, rama, merge, *cherry-pick*, parche), **comprueba si su cambio
   rompe algo y si el prompt lo recoge**. Si no lo recoge, **no lo des por bueno**: complétalo tú o
   devuélvelo con la lista exacta de lo que falta.
4. **Prueba de aceptación.** Ante cualquier cambio público pregúntate: *«si un agente aplicara hoy el prompt a
   un proyecto 1.x que usa este miembro, ¿acabaría compilando y comportándose igual sin que nadie le
   avise de nada más?»* Si la respuesta no es un sí rotundo, **el prompt está incompleto**.

### Qué hay que rellenar — SIEMPRE LOS SEIS SITIOS

En `src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md`: tabla §3.3, subsección `#### N` de
detalle, barrido de búsqueda de la Fase 5 (punto 4), lista del informe de la Fase 6 (punto 5) y tabla del
apéndice de versiones; y en `oofp-upgrade-2.renames.md` si hay cualquier renombrado. **Los seis sitios, no
solo la tabla.** No des por terminada ninguna historia de usuario ni ninguna épica sin haberlo hecho.

### Cómo se comprueba

```powershell
cd src
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-migration-prompt.ps1                       # coherencia del prompt
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-migration-prompt.ps1 -BaseRef origin/<rama> # + commits con ruptura
```

Falla (código de salida 1) si un punto de la tabla no tiene subsección, informe final o apéndice, o si un
commit marca `!` / `BREAKING CHANGE` sin tocar el prompt. La misma comprobación corre en la integración
continua (`.github/workflows/prompt-migracion.yml`) y la PR tiene una lista de verificación
(`.github/pull_request_template.md`).

📄 **Detalle completo de la regla, auditoría de cambios pasados, revisión de contribuciones ajenas,
checklist y numeración**: `.github/instructions/oofp-registro-rupturas.instructions.md`

## 🚨 Regla obligatoria — Conventional Commits y verificación previa al commit

Todos los commits usan **Conventional Commits** con el resumen y el cuerpo **en castellano**, en
imperativo y sin punto final, con `!` obligatorio si el cambio rompe compilación, contrato o
compatibilidad binaria:

```
refactor(core)!: trocear la clase Extensions en cuatro clases con nombre propio (US-11)
```

**Antes de CADA `git commit`** hay que verificar que
`src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md` está actualizado y que las rupturas de ese
commit están bien registradas en los **6 sitios**, para que los upgrades desde versiones antiguas sigan
siendo fáciles de aplicar. Mensajes con acentos o multilínea: **siempre `git commit -F`** desde un
fichero UTF-8 sin BOM, nunca `-m`. **Nunca `git push` sin confirmación del usuario.**

📄 **Formato, tipos, ámbitos y puerta de control completa**:
`.github/instructions/commits-conventional-y-registro.instructions.md`