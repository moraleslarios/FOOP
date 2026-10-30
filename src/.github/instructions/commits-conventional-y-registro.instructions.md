---
applyTo: '**'
description: 'Regla obligatoria del repositorio MoralesLarios.OOFP: todos los commits siguen Conventional Commits en castellano, y ningún commit se crea sin haber verificado antes que el prompt de migración oofp-upgrade-2.prompt.md registra las rupturas que ese commit introduce.'
---

# Commits con Conventional Commits + verificación del prompt de migración

> 🚨 **ESTA REGLA NO ES OPCIONAL Y NO CADUCA.** Se aplica a **todas** las sesiones de trabajo en este
> repositorio, aunque el usuario no la mencione en su petición.

Esta regla tiene **dos mitades inseparables**:

1. **Cómo** se escribe el mensaje de commit → Conventional Commits, en castellano (§2 … §5).
2. **Qué hay que comprobar antes** de hacer el commit → que el prompt de migración está actualizado
   (§6). Un commit con el mensaje perfecto pero sin el registro de la ruptura es un **commit mal
   hecho** y hay que corregirlo antes de publicarlo.

---

## 1. Por qué las dos cosas van juntas

`MoralesLarios.OOFP` es una **librería pública**. El consumidor no ve nuestro código: ve dos cosas.

- El **historial de commits**, que le dice *qué pasó* y, sobre todo, **si algo rompió** (`!`).
- El **prompt de migración**
  [`docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md`](../../docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md),
  que le dice *qué tiene que tocar* para subir de versión.

Si el commit marca `!` pero el prompt no registra la ruptura, el consumidor **sabe que algo se rompió y
no sabe qué hacer**. Si el prompt la registra pero el commit no la marca, la ruptura **no se ve** al
generar el changelog ni al calcular la siguiente versión. Las dos piezas se escriben **a la vez**.

---

## 2. Formato del mensaje

```
<tipo>(<ámbito>)<!>: <resumen en minúscula, imperativo, sin punto final>

<cuerpo opcional en castellano, explicando el POR QUÉ, no el qué>

<pies opcionales: BREAKING CHANGE, Refs>
```

Reglas duras:

- El **resumen** va en **castellano**, en minúscula inicial, en **imperativo** («normaliza», «retira»,
  «trocea»), **sin punto final**, y cabe en ≤ 72 caracteres.
- El `!` antes de los dos puntos es **obligatorio** si el commit rompe algo (ver §4).
- El **cuerpo** va en castellano y explica la motivación y las consecuencias. Es el sitio donde se
  resume la ruptura y se apunta al punto del prompt.
- Si la historia de usuario o la épica tienen identificador, **cítalo**: `(US-11)`,
  `(epica 04, US-17..US-25)`.

---

## 3. Tipos y ámbitos admitidos

| Tipo | Cuándo |
| --- | --- |
| `feat` | miembro, tipo o capacidad **nueva** |
| `fix` | corrección de un defecto de comportamiento |
| `refactor` | reorganización sin cambio funcional pretendido (incluye renombrados) |
| `docs` | documentación XML, `README`, épicas, prompts de migración |
| `test` | tests nuevos o reparados, sin tocar producción |
| `build` | `.csproj`, paquetes, `FrameworkReference`, versionado |
| `chore` | tareas de mantenimiento sin efecto sobre el consumidor |
| `perf` | mejora de rendimiento sin cambio de contrato |

**Ámbito** = la parte afectada, en minúscula y sin el prefijo `MoralesLarios.OOFP.`:

`core` · `copilot` · `http` · `httpclients` · `efcore` · `aspnetcore` · `validation` · `valueobjects` ·
`utilities` · `logging` · `configuration` · `shared` · `docs`

Ejemplos reales ya existentes en este repositorio (úsalos como plantilla):

```text
refactor(core)!: normalizar Constants, centralizar literales y adelgazar los global using
refactor(core)!: trocear la clase Extensions en cuatro clases con nombre propio (US-11)
fix(core): sanea el contenedor de metadatos de MlErrorDetails (epica 04, US-17..US-25)
docs(core): US-16 convenios de nomenclatura Try* y Async
docs(copilot): completar el prompt de migración con los cambios de la 2.1.1
```

---

## 4. Cuándo es obligatorio el `!` (y el pie `BREAKING CHANGE`)

Lleva `!` **todo** commit que encaje en cualquiera de estos casos — es exactamente la misma lista que
obliga a registrar la ruptura en el prompt:

- **Rompe compilación**: renombrar o retirar un tipo/miembro/parámetro, cambiar una firma, pasar de
  `PackageReference` a `FrameworkReference`.
- **Rompe contrato (silenciosa)** ⚠️: cambia el texto de un mensaje, una URL, una cabecera, el cuerpo
  HTTP, el orden o el recuento de una consulta; pasar de mutar a copiar; pasar de igualdad por
  referencia a estructural; que un método deje de ser NO-OP.
- **Ruptura binaria**: `const` → `static readonly`, cambiar el tipo de retorno, añadir un parámetro
  aunque sea opcional.
- **Marcado `[Obsolete]`**: genera `CS0618` en el consumidor.

Para una ruptura **silenciosa**, además del `!`, el cuerpo **debe** decirlo explícitamente, porque el
compilador del consumidor no lo va a avisar:

```text
fix(core)!: corrige el orden de OrderBy en la paginación imperativa

OrderBy se aplicaba DESPUÉS de Skip/Take, así que el contenido de cada
página era incorrecto. Ahora se aplica antes.

RUPTURA SILENCIOSA: compila igual, pero cambia el contenido de cada
página. Los tests que fijaban el orden antiguo fallarán.
Registrada como punto 37 de oofp-upgrade-2.prompt.md §3.3.

BREAKING CHANGE: el contenido de cada página cambia.
```

---

## 5. Cómo crear el commit (restricción técnica)

Los mensajes llevan **acentos** y suelen ser **multilínea**. En este repositorio ya se ha comprobado que
`git commit -m "..."` desde PowerShell 5.1 **corrompe los acentos** y que los mensajes multilínea se
truncan.

Por tanto, **siempre**:

1. Escribe el mensaje en un fichero temporal **UTF-8 sin BOM**:
   `[System.IO.File]::WriteAllText($p, $msg, (New-Object System.Text.UTF8Encoding($false)))`
2. Haz el commit con `git commit -F <ruta>`.
3. Verifica el resultado con `git log -1 --format=%B` y comprueba que los acentos salen bien.
4. Borra el temporal.

Nunca uses `-m` para mensajes con acentos o con cuerpo.

---

## 6. ⛔ Puerta de control: QUÉ VERIFICAR ANTES DE CADA COMMIT

**Antes** de ejecutar `git commit`, y **antes** de `git push`, recorre esta lista. Si algo falla,
**arréglalo y mételo en el mismo commit**; no lo dejes para después.

### 6.1 Identifica si el cambio rompe algo

Revisa el `git diff` con la lista de la §4 en la mano. Pregúntate en concreto: *¿cambia algún texto de
mensaje, alguna clave, alguna URL, algún orden de consulta, alguna firma?* Las rupturas **silenciosas**
son las que se escapan, precisamente porque el build sigue verde.

### 6.2 Si rompe algo, comprueba que el prompt lo registra — **los 6 sitios**

En [`oofp-upgrade-2.prompt.md`](../../docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md):

1. **Fila en la tabla resumen de la §3.3** (`### 3.3 Cambios de COMPORTAMIENTO…`).
2. **Subsección de detalle `#### N — …`**, en orden numérico, con el antes/después, **qué tiene que
   hacer el consumidor** y, si es silenciosa, **cómo detectarla**.
3. **Barrido de búsqueda en la Fase 5, punto 4** · los patrones `grep` que localizan el código afectado.
4. **Lista del informe final, Fase 6, punto 5** · el número añadido a la enumeración.
5. **Tabla del apéndice «Versiones posteriores a la 2.0.0»** · número y resumen en la fila de su versión.
6. **Anexo de renombrados**
   [`oofp-upgrade-2.renames.md`](../../docs_next/Copilot/.github/prompts/oofp-upgrade-2.renames.md) ·
   obligatorio **siempre que haya un renombrado**, en formato apto para buscar-y-reemplazar.

> ⚠️ Registrar la ruptura **solo** en la tabla de la §3.3 es trabajo **incompleto**. La tabla es un
> índice; el valor está en los otros cinco sitios, que son los que dirigen al agente de migración.

### 6.3 Verifica la coherencia interna del prompt

Es un fichero largo y los huecos pasan desapercibidos. Comprueba que **cada número existe en los cinco
sitios**:

- Último número usado: busca `^#### \d+` y continúa desde ahí. La numeración es una **secuencia global
  creciente** que **nunca se reutiliza ni se reordena**, porque se cita por número desde las Fases 5 y 6,
  el apéndice y las épicas.
- Para cada punto nuevo, busca su número en el fichero y confirma que sale en la tabla, en su
  subsección, en la Fase 5, en la Fase 6 y en el apéndice.

### 6.4 Comprueba que el build y los tests siguen verdes

Desde la raíz del repositorio (`src`):

```powershell
dotnet build MoralesLarios.OOFP.sln -c Release --no-incremental -v q --nologo
dotnet test  MoralesLarios.OOFP.sln -c Release --no-build --nologo
```

Criterio: **EXITCODE=0, 0 errores, 0 `CS1591`**. Usa siempre `--no-incremental`: un build incremental
**oculta avisos** (se han visto 15 avisos en incremental frente a ~119 en build completo).

### 6.5 Actualiza el documento de la épica

El registro en el prompt **no sustituye** al documento de la épica en `docs_next/Next_Versions/<versión>/`:

- la **épica** explica el diseño y la decisión (criterios de aceptación, decisiones `D-NN`);
- el **prompt** explica la migración del consumidor.

Marca los criterios de aceptación cumplidos y el estado de la historia (`⬜` → `✅`).

### 6.6 Checklist final antes de `git commit`

- [ ] El tipo y el ámbito del mensaje son correctos (§3).
- [ ] Lleva `!` si rompe algo (§4), y el cuerpo lo explica.
- [ ] Toda ruptura tiene su fila en la §3.3 **y** su subsección `#### N`.
- [ ] Las rupturas silenciosas tienen patrones `grep` en la Fase 5, punto 4.
- [ ] Los números están en la lista de la Fase 6, punto 5, y en la tabla del apéndice.
- [ ] Los renombrados están en `oofp-upgrade-2.renames.md`.
- [ ] El criterio de aceptación está marcado en el documento de la épica.
- [ ] Build en Release y tests en verde.
- [ ] El mensaje se crea con `git commit -F` desde un fichero UTF-8 sin BOM (§5).

---

## 7. Edición de los ficheros del prompt (restricción técnica)

`oofp-upgrade-2.prompt.md` y `oofp-upgrade-2.renames.md` son ficheros **largos**. En este repositorio ya
se ha comprobado que las herramientas de edición por parcheo **truncan o duplican contenido sin avisar**
en ficheros de este tamaño (una vez se perdieron las Fases 4-6 completas).

- **Verifica siempre el recuento de líneas antes y después** de cualquier edición, y aborta si la
  diferencia no es exactamente la esperada.
- Comprueba el resultado con `git diff -U1` antes de darlo por bueno.
- Prefiere inserciones ancladas a un texto **único** (valida que la ancla aparece **una sola vez**)
  frente a reescrituras de bloques grandes.

---

## 8. `git push`

- **Nunca publiques sin pedir confirmación** al usuario, salvo que te lo haya pedido expresamente en la
  petición en curso.
- Antes de publicar, repasa la §6.6 sobre **todos** los commits que vas a subir, no solo el último:
  `git log --oneline origin/<rama>..HEAD`.
- Si detectas que un commit ya creado incumple la regla y **no** está publicado, corrígelo
  (`git commit --amend` o un commit de arreglo `docs(copilot): …`) antes del push.

---

## 9. Regla relacionada

El detalle completo de **qué cuenta como ruptura**, la numeración y el checklist de registro están en
[`oofp-registro-rupturas.instructions.md`](oofp-registro-rupturas.instructions.md). Este documento
añade el **cómo se escribe el commit** y la **puerta de control previa** a crearlo. Los dos son
obligatorios.
