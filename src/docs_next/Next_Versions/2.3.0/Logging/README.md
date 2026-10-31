# Plan de la versión 2.3.0 · `MoralesLarios.OOFP.Logging`

> **Estado global** · ✅ **completada** · las 6 decisiones resueltas (31-10-2026) y las **7 historias** (US-34 … US-40) implementadas · puntos **53 a 59** del prompt de migración
> **Proyectos afectados** · `src/MoralesLarios.OOFP.Logging` · `src/MoralesLarios.OOFP.Logging.Tests.Unit` · `src/MoralesLarios.OOFP.Logging.Sample`
> **Origen** · petición del propietario · hallazgo `D2`, diferido en la [épica 6 de la 2.2.0](../../2.2.0/Logging/Epica-06-Rendimiento-y-Estandar-de-Logging.md)
> **Versiones destino** · 2.3.0 (US-34 … US-40)
> **Esfuerzo total** · **XL** (~3,5 jornadas)
> **Naturaleza** · **aditiva**: no se modifica ninguna firma existente

---

## Índice

1. [Las épicas](#1-las-épicas)
2. [Mapa de las historias](#2-mapa-de-las-historias)
3. [Qué cambia para el consumidor](#3-qué-cambia-para-el-consumidor)
4. [Orden recomendado](#4-orden-recomendado)
5. [Decisiones resueltas](#5-decisiones-resueltas)
6. [Qué queda fuera](#6-qué-queda-fuera)
7. [Definición de terminado común](#7-definición-de-terminado-común)
8. [Ver también](#8-ver-también)

---

## 1. Las épicas

| # | Épica | Superficie que toca | Historias | Esfuerzo | Riesgo | Estado |
|--:|-------|---------------------|-----------|---------:|:------:|:------:|
| 8 | [Registro estructurado y plantillas de mensaje](Epica-08-Registro-Estructurado-y-Plantillas.md) | los 6 ficheros de nivel · `MlResultLogExtensions.cs` · `EventId.cs` · `Categoria.cs` · `README.md` · `Sample` | US-34 … US-40 | XL | 🟠 alto | ✅ |

**Leyenda de esfuerzo:** XS (<2 h) · S (½ jornada) · M (1 jornada) · L (2 jornadas) · XL (3+ jornadas)
**Leyenda de riesgo:** 🟢 bajo · 🟡 medio · 🟠 alto · 🔴 muy alto (rompe compatibilidad)

> **Por qué una sola épica.** Las siete historias comparten **una misma decisión bloqueante**,
> [`D-26`](Epica-08-Registro-Estructurado-y-Plantillas.md#5-decisiones-abiertas), que fija el nombre
> de todo lo que se añade. Partirlas en épicas distintas obligaría a repetir esa decisión en cada
> una, o —peor— a resolverla de forma distinta en cada sitio.

> **Por qué el riesgo es alto siendo una versión aditiva.** Porque la capacidad nueva **convive con
> sobrecargas que aceptan `string`**, y hay un caso —un único argumento de tipo `string`— en el que
> el compilador elige la sobrecarga antigua sin avisar. Está probado y documentado en el
> [punto 30](Epica-08-Registro-Estructurado-y-Plantillas.md#punto-30--la-prueba-de-la-ambigüedad-y-la-captura-silenciosa).

---

## 2. Mapa de las historias

| Historia | Resumen | Esfuerzo | Rompe |
|----------|---------|---------:|:-----:|
| **US-34** | Sobrecargas con plantilla de mensaje y argumentos (familia `…Template`) | L | no |
| **US-35** | Plantilla en la rama de fallo | M | no |
| **US-36** | Combinar la plantilla con `EventId` y con `ILogger<TCategory>` | M | no |
| **US-37** ✅ | Decidir y, si procede, implementar los ámbitos (`BeginScope`) · **sin código**: entrega la [épica 9](../../2.4.0/Logging/Epica-09-Ambitos-de-Registro.md) | L | no |
| **US-38** ✅ | Dejar la superficie limpia ante los analizadores · **241** parámetros anotados, `CA2254 = 0` | M | no |
| **US-39** ✅ | Cerrar las dos erratas que sobrevivieron a la 2.2.0 · `LogCompletedResultAsync` + región de `Trace`, **punto 58** del prompt | XS | **sí** |
| **US-40** ✅ | Documentar la capacidad nueva de cara al consumidor · familia 8 del `README.md`, las **seis** afirmaciones falsificadas y el quinto bloque del `Sample`, **punto 59** del prompt | S | no |

---

## 3. Qué cambia para el consumidor

**Hoy**, con 398 firmas públicas, no hay ninguna por la que pueda entrar un campo estructurado:

```csharp
origen.LogMlResultInformation(logger, validBuildMessage: p => $"Pedido {p.Id} confirmado");
// el sink recibe:  "Pedido 4711 confirmado"   ← una cadena opaca
```

**Tras esta versión:**

```csharp
origen.LogMlResultInformation(logger, "Pedido {PedidoId} del cliente {ClienteId} confirmado",
                              p.Id, p.ClienteId);
// el sink recibe:  "Pedido 4711 del cliente 912 confirmado"
//                + { PedidoId = 4711, ClienteId = 912, OriginalFormat = "Pedido {PedidoId} …" }
```

Es decir: se puede **consultar, agrupar y alertar** por campo, que es para lo que el consumidor
instaló `Serilog` o `OpenTelemetry`.

Estado del estándar de `Microsoft.Extensions.Logging` en el proyecto:

| Elemento | 2.1.1 | 2.2.0 | 2.3.0 (objetivo) |
|----------|------:|------:|-----------------:|
| `ILogger.IsEnabled` | 0 | 7 | 7 |
| `EventId` | 0 | 123 | > 123 |
| `ILogger<TCategory>` | 0 | 21 | > 21 |
| **Plantilla con argumentos** | 0 | **0** | **✅** |
| `BeginScope` | 0 | 0 | sigue a **0** · diferido a la [épica 9](../../2.4.0/Logging/Epica-09-Ambitos-de-Registro.md) |
| `LoggerMessage` | 0 | 0 | ver `D-30` |

---

## 4. Orden recomendado

1. **Cerrar `D-26` y `D-27`.** Bloquean la US-34 por completo: de ellas depende el nombre público.
2. **US-34** · la rama válida, que fija la firma y el método base.
3. **US-35** · la rama de fallo, repitiendo el patrón sobre cuatro familias más.
4. **US-36** · el cruce con `EventId` y con `ILogger<TCategory>`, según `D-28`.
5. **US-38** y **US-39** · medición ante analizadores y las dos erratas, una vez la superficie está
   completa.
6. **US-37** ✅ · en paralelo desde el principio, porque sólo dependía de `D-29`. Cerrada entregando la
   [épica 9](../../2.4.0/Logging/Epica-09-Ambitos-de-Registro.md) y su [plan de versión](../../2.4.0/Logging/README.md).
7. **US-40** · siempre al final: el `README.md` documenta lo que existe.

El diagrama de dependencias está en la
[§4 de la épica](Epica-08-Registro-Estructurado-y-Plantillas.md#4-orden-de-ejecución).

---

## 5. Decisiones resueltas

Seis decisiones, numeradas a continuación de las 25 que se cerraron en la 2.2.0, **todas resueltas el
31-10-2026 con la opción recomendada**. El razonamiento completo, con las opciones descartadas y el
*por qué no* de cada una, está en la
[§5 de la épica](Epica-08-Registro-Estructurado-y-Plantillas.md#5-decisiones-abiertas).

| # | Asunto | Afecta a | Resolución |
|:-:|--------|----------|------------|
| `D-26` | Cómo se evita la **captura silenciosa** | **US-34, US-35, US-36** | ✅ **(a)** nombre distinto, con el sufijo **`Template`** |
| `D-27` | A qué rama se aplica la plantilla | US-34 | ✅ **(c)** sólo la válida; la fallida en la US-35 |
| `D-28` | Hasta dónde se cruzan los ejes | US-36 | ✅ **(b)** genérico + 6 niveles + `EventId`; `ILogger<T>` fuera de alcance |
| `D-29` | Si los **ámbitos** entran aquí | US-37 | ✅ **(b)** se desgajan a la **épica 9**; la US-37 no produce código |
| `D-30` | Alcance de la US-38 | US-38 | ✅ **(a)** sólo anotar y documentar · aplicada con `[StringSyntax]` |
| `D-31` | El puente del `Async` que falta | US-39 | ✅ **(a)** puente `[Obsolete]` en la 2.3.0 |

### Lo que estas resoluciones cambian en el plan

- **La familia nueva se llama `…Template`** · `LogMlResultInformationTemplate`,
  `LogMlResultErrorTemplate`, … y el genérico `LogMlResultTemplate`. Elegido frente a `Format`
  porque *plantilla de mensaje* es el término de la plataforma, mientras que `Format` evocaría los
  marcadores **posicionales y anónimos** de `string.Format` (`{0}`, `{1}`), que es justo la idea
  contraria: aquí el marcador **tiene nombre** y ese nombre es el campo que verá el *sink*.
- **La captura silenciosa desaparece por construcción** · al no compartir nombre con las sobrecargas
  de `string`, no hay resolución de sobrecarga que pueda equivocarse. El riesgo de la épica baja de
  🟠 **alto** a 🟡 **medio** en todo lo que no sea la US-39.
- **La US-37 no produce código** · se cierra con la decisión y con el documento de la épica 9.
- **La US-39 pasa a ser la única que rompe** · el puente `[Obsolete]` genera `CS0618`, así que exige
  la fila en `oofp-upgrade-2.renames.md` además del registro en los 6 sitios del prompt.
- **`ILogger<TCategory>` × plantilla queda fuera de alcance**, y hay que declararlo en el `README.md`
  del proyecto igual que hoy se declara que los dos ejes de la 2.2.0 no se combinan.

---

## 6. Qué queda fuera

| Asunto | Dónde va | Por qué |
|--------|----------|---------|
| `D3` · los `null!` como valor por omisión | 3.0.0 | cambiar los valores por omisión es ruptura binaria |
| `D5` · asincronía de adorno | 3.0.0 | retirar firmas rompe compilación |
| `D6` · `LogMlResult` devolviendo `MlResult<ILogger>` | 3.0.0 | cambiar el tipo de retorno es ruptura binaria |
| `D7` · `Match` devolviendo `null!` | 3.0.0 | afecta a `MoralesLarios.OOFP.Core`, no a `Logging` |
| Retirar los puentes `[Obsolete]` de la 2.2.0 | 3.0.0 | su razón de ser es dar una versión de margen |
| Un tipo de opciones (`MlLogOptions`) que detenga la combinatoria | 3.0.0 | cambia el estilo de la API; ver `D-28` opción (d) |
| `BeginScope` | [**épica 9**, versión 2.4.0](../../2.4.0/Logging/Epica-09-Ambitos-de-Registro.md) | plantea preguntas de diseño propias: la excepción del tramo, la composición con `Bind` y la supervivencia al `await` |

---

## 7. Definición de terminado común

Vale para **todas** las historias de esta versión:

- [ ] Build en Release con `--no-incremental` ⇒ `EXIT = 0`, **0 errores**, **0 `CS1591`**, **0
      `CS0618`**, y **sin avisos nuevos** sobre la línea base de **236**.
- [ ] Tests en Release ⇒ `EXIT = 0`, **0 con error**, **0 omitidos**, total **> 1.397**.
- [ ] Los **dos tests de paridad** de `MatrixTests.cs` actualizados y en verde.
- [ ] El `Sample` se ejecuta con `EXIT = 0`.
- [ ] `scripts\check-md-anchors4.ps1` ⇒ `rotos = 0`.
- [ ] Documentación XML en cada miembro público nuevo.
- [ ] Registro en los **6 sitios** de
      [`oofp-upgrade-2.prompt.md`](../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md),
      continuando la numeración **desde el punto 52**, y en
      [`oofp-upgrade-2.renames.md`](../../../Copilot/.github/prompts/oofp-upgrade-2.renames.md) si hay
      renombrados.
- [ ] Commits con **Conventional Commits en castellano**, con `!` si rompen, creados con
      `git commit -F` desde fichero **UTF-8 sin BOM**.
- [ ] **Sin publicar sin confirmación** del propietario.

---

## 8. Ver también

- [`Epica-08-Registro-Estructurado-y-Plantillas.md`](Epica-08-Registro-Estructurado-y-Plantillas.md)
- [`README.md` de la versión 2.2.0](../../2.2.0/Logging/README.md) · las siete épicas ya cerradas
- [`README.md` de la versión 2.4.0](../../2.4.0/Logging/README.md) · la épica 9, desgajada por la US-37
- [`README.md` del proyecto](../../../../MoralesLarios.OOFP.Logging/README.md) · la API tal como está hoy
- [`oofp-upgrade-2.prompt.md`](../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md) · prompt de migración
