# Copilot Instructions

## Project Guidelines
- El usuario prefiere que las respuestas se den en castellano.

## 🚨 Regla obligatoria — Registrar TODA ruptura en el prompt de migración

Este repositorio es una **librería pública**. Por cada cambio que rompa la compilación, el contrato o la
compatibilidad binaria de un consumidor —**incluidos los cambios silenciosos que el compilador no
detecta**— es **obligatorio** actualizar, **en la misma tanda de cambios**:

- `docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md` → tabla §3.3, subsección `#### N` de
  detalle, barrido de búsqueda de la Fase 5 (punto 4), lista del informe de la Fase 6 (punto 5) y tabla
  del apéndice de versiones. **Los cinco sitios, no solo la tabla.**
- `docs_next/Copilot/.github/prompts/oofp-upgrade-2.renames.md` → si hay cualquier renombrado.

No des por terminada ninguna historia de usuario ni ninguna épica sin haberlo hecho.

📄 **Detalle completo de la regla, checklist y numeración**:
`.github/instructions/oofp-registro-rupturas.instructions.md`

## 🚨 Regla obligatoria — Conventional Commits y verificación previa al commit

Todos los commits usan **Conventional Commits** con el resumen y el cuerpo **en castellano**, en
imperativo y sin punto final, con `!` obligatorio si el cambio rompe compilación, contrato o
compatibilidad binaria:

```
refactor(core)!: trocear la clase Extensions en cuatro clases con nombre propio (US-11)
```

**Antes de CADA `git commit`** hay que verificar que
`docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md` está actualizado y que las rupturas de ese
commit están bien registradas en los **6 sitios**, para que los upgrades desde versiones antiguas sigan
siendo fáciles de aplicar. Mensajes con acentos o multilínea: **siempre `git commit -F`** desde un
fichero UTF-8 sin BOM, nunca `-m`. **Nunca `git push` sin confirmación del usuario.**

📄 **Formato, tipos, ámbitos y puerta de control completa**:
`.github/instructions/commits-conventional-y-registro.instructions.md`