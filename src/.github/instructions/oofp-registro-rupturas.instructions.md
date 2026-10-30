---
applyTo: '**'
description: 'Regla obligatoria del repositorio MoralesLarios.OOFP: toda ruptura de API o de comportamiento debe quedar registrada en el prompt de migración oofp-upgrade-2.prompt.md y en su anexo de renombrados, en la misma tanda de cambios.'
---

# Registro obligatorio de rupturas en el prompt de migración

> 🚨 **ESTA REGLA NO ES OPCIONAL Y NO CADUCA.** Se aplica a **todas** las sesiones de trabajo en este
> repositorio, aunque el usuario no la mencione en su petición.

## 1. Por qué existe esta regla

El fichero
[`docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md`](../../docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md)
es el **único artefacto** que permite a un proyecto de terceros migrar de una versión de
`MoralesLarios.OOFP` a la siguiente. Es el contrato con el consumidor.

Si se introduce una ruptura en el código y **no** se registra ahí, esa ruptura se vuelve **invisible**:
el consumidor se la encuentra en producción, no en el documento de migración. Por eso el registro es
parte de la definición de terminado de **cualquier** cambio, no una tarea posterior.

## 2. Qué cuenta como «ruptura» y hay que registrar

Hay que registrar **todo** lo siguiente, incluso si el código del consumidor sigue compilando:

| Tipo | Ejemplos |
| --- | --- |
| **Rompe compilación** | renombrar un tipo, método, propiedad, parámetro o paquete; retirar un miembro; cambiar una firma; cambiar una referencia de paquete a `FrameworkReference` |
| **Rompe contrato (silenciosa)** ⚠️ | cambiar el texto de un mensaje de error; cambiar una URL, cabecera o cuerpo HTTP emitido; cambiar el orden o el recuento de una consulta; pasar de mutación a copia; pasar de igualdad por referencia a estructural; que un método deje de ser NO-OP |
| **Ruptura binaria** | cambiar `const` por `static readonly`; cambiar el tipo de retorno; añadir un parámetro aunque sea opcional |
| **Marcado `[Obsolete]`** | añadir un puente `[Obsolete]` (genera `CS0618` en el consumidor) |
| **Aditivo relevante** ✅ | miembros nuevos, convenios nuevos o documentación de semántica no obvia: se registran igualmente, marcados como «sin rupturas», para que el consumidor sepa que existen |

**Único caso exento**: cambios puramente internos (`private`, `internal` sin `InternalsVisibleTo`) que no
alteran ningún comportamiento observable.

## 3. Los 5 sitios que hay que tocar — SIEMPRE LOS CINCO

Por **cada** ruptura nueva, en `oofp-upgrade-2.prompt.md`:

1. **Fila nueva en la tabla resumen de la §3.3** · `### 3.3 Cambios de COMPORTAMIENTO…`
   Columnas: `#` · `Qué cambió` (con la versión entre paréntesis en negrita) · `antes` · `ahora` ·
   `Tipo de ruptura`.
2. **Subsección de detalle `#### N — …`** · situada tras la tabla, en orden numérico. Debe explicar:
   qué cambió y por qué, el antes/después en bloques de código, **qué tiene que hacer el consumidor** y,
   si la ruptura es silenciosa, **cómo detectarla** (no basta con decir que existe).
3. **Barrido de búsqueda en la Fase 5, punto 4** · añadir los patrones de búsqueda (grep) que localizan el
   código afectado en el proyecto del consumidor, y la instrucción de qué hacer con cada coincidencia.
4. **Lista del informe final, Fase 6, punto 5** · añadir el número del punto a la enumeración, para que el
   agente de migración esté obligado a pronunciarse sobre él en su informe.
5. **Tabla del apéndice «Versiones posteriores a la 2.0.0»** · añadir el número a la fila de su versión y
   ampliar el «Resumen de la ruptura». Si es una versión nueva, crear la fila y la nota de salto de versión.

Y, además:

6. **Anexo de renombrados**
   [`oofp-upgrade-2.renames.md`](../../docs_next/Copilot/.github/prompts/oofp-upgrade-2.renames.md) ·
   obligatorio **siempre que haya un renombrado**: tabla `antiguo → nuevo` en la sección del paquete
   correspondiente, en **formato apto para buscar y reemplazar**, con una nota si el nombre es ambiguo
   (`Value`, `Name`, `Key`, `Empty`, `Do`, `Combine`, `OrderBy`…).

> ⚠️ Registrar la ruptura **solo** en la tabla de la §3.3 es un trabajo **incompleto**. La tabla es un
> índice; el valor está en los otros cuatro sitios, que son los que dirigen al agente de migración.

## 4. Cuándo hacerlo

**En la misma tanda de cambios que el código.** Nunca «al final de la épica», nunca «en el commit
siguiente». El commit que introduce la ruptura debe incluir la actualización del prompt.

Antes de dar por terminada una historia de usuario o una épica:

- [ ] ¿Toda ruptura introducida tiene su fila en la §3.3?
- [ ] ¿Toda ruptura tiene su subsección `#### N` con el antes/después y la acción del consumidor?
- [ ] ¿Las rupturas silenciosas tienen patrones de búsqueda en la Fase 5, punto 4?
- [ ] ¿Están los números en la lista de la Fase 6, punto 5?
- [ ] ¿Está la versión y sus puntos en la tabla del apéndice?
- [ ] ¿Están los renombrados en `oofp-upgrade-2.renames.md`?
- [ ] ¿Está marcado el criterio de aceptación correspondiente en el documento de la épica?

## 5. Numeración

- Los puntos de la §3.3 son una **secuencia global creciente** y **no se reutilizan ni se reordenan**,
  porque se citan por número desde la Fase 5, la Fase 6, el apéndice y los documentos de las épicas.
- Antes de añadir, **comprueba el último número usado** buscando `^#### \d+` en el prompt y continúa desde
  ahí.
- Si una ruptura es la continuación natural de otra ya registrada, agrupa los números en una sola
  subsección (`#### 27 y 28 — …`), igual que ya se hace con `#### 35, 36 y 37`.

## 6. Cómo editar estos ficheros (restricción técnica)

`oofp-upgrade-2.prompt.md` y `oofp-upgrade-2.renames.md` son ficheros **largos**. En este repositorio ya
se ha comprobado que las herramientas de edición por parcheo **truncan o duplican contenido** en ficheros
de este tamaño sin avisar.

Por tanto:

- **Verifica siempre el recuento de líneas antes y después** de cualquier edición, y aborta si la
  diferencia no es exactamente la esperada.
- Comprueba el resultado con `git diff -U1` sobre el fichero antes de darlo por bueno.
- Prefiere inserciones ancladas a un texto **único** del fichero (valida que la ancla aparece **una sola
  vez**) frente a reescrituras de bloques grandes.

## 7. Documento de la épica

El registro en el prompt **no sustituye** al documento de la épica en `docs_next/Next_Versions/<versión>/`:

- la épica explica **el diseño y la decisión** (criterios de aceptación, decisiones abiertas `D-NN`);
- el prompt explica **la migración** del consumidor.

Hay que actualizar **los dos**, y marcar en la épica el criterio de aceptación
*«La ruptura está registrada en `oofp-upgrade-2.prompt.md` §3.3 y en el anexo de renombrados»* solo cuando
los **seis** sitios de la §3 estén hechos.
