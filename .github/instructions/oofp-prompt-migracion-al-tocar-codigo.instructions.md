---
applyTo: 'src/**/*.{cs,csproj,props,targets}'
description: 'Recordatorio que se aplica al tocar código, proyectos o propiedades de MoralesLarios.OOFP: si el cambio rompe compatibilidad (compilación, contrato silencioso, binaria o puente Obsolete), el prompt de migración oofp-upgrade-2.prompt.md debe actualizarse en el mismo cambio.'
---

# Al tocar código: ¿rompe algo? Entonces actualiza el prompt de migración

> 🎯 `src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md` debe permitir migrar un proyecto
> 1.x a la 2.x **sin incidencias**. Solo lo consigue si se actualiza **cada vez** que algo rompe
> compatibilidad, **lo haya hecho yo, otra persona o un commit antiguo**.

Antes de dar por terminado este cambio, repasa **en 30 segundos**:

| Si el cambio… | Entonces |
| --- | --- |
| renombra, retira o mueve un tipo, miembro, parámetro, paquete o *namespace* | **ruptura de compilación** → registrar |
| cambia una firma, un tipo de retorno o añade un parámetro (aunque sea opcional) | **ruptura de compilación o binaria** → registrar |
| cambia un texto de error, una URL, una cabecera, el orden o el recuento de una consulta, copia/mutación, igualdad, o hace que un método deje de ser NO-OP | **ruptura silenciosa** (compila igual) → registrar **con su patrón de detección** |
| añade un puente `[Obsolete]` | genera `CS0618` en el consumidor → registrar |
| añade un miembro o convenio nuevo relevante | «sin rupturas», pero se registra para que el consumidor lo conozca |
| es puramente interno (`private`/`internal` sin `InternalsVisibleTo`) y no cambia nada observable | exento |

Si **registrar** aplica, hazlo **ahora**, en los seis sitios, y no en «un commit posterior»:

1. fila en la tabla de la §3.3 · 2. subsección `#### N` · 3. barrido de la Fase 5 (punto 4) ·
4. lista de la Fase 6 (punto 5) · 5. tabla del apéndice de versiones · 6. anexo
`oofp-upgrade-2.renames.md` si hay renombrado.

Y además:

- **Si el cambio no es tuyo** (lo trae una PR, una rama, un *merge* o un parche), comprueba igualmente si
  rompe algo y si el prompt lo recoge. Si no lo recoge, **complétalo o devuélvelo con lo que falta**.
- **Si al leer el código ves una ruptura antigua que nunca se registró**, regístrala o avisa.
- Comprobación automática: `cd src` y
  `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-migration-prompt.ps1 -BaseRef origin/<rama>`.

📄 Regla completa (qué cuenta como ruptura, numeración, edición segura del fichero, cambios pasados y de
otros contribuyentes): [`oofp-registro-rupturas.instructions.md`](oofp-registro-rupturas.instructions.md).
