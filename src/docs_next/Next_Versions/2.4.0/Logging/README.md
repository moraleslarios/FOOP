# Plan de la versión 2.4.0 · `MoralesLarios.OOFP.Logging`

> **Estado global** · ⬜ no iniciada · **las 6 decisiones están abiertas** y son bloqueantes
> **Versión destino** · 2.4.0 (US-41 … US-46)
> **Proyecto** · `src/MoralesLarios.OOFP.Logging`
> **Origen** · [`D-29`](../../2.3.0/Logging/Epica-08-Registro-Estructurado-y-Plantillas.md#5-decisiones-abiertas) opción **(b)** de la épica 8: los ámbitos de registro se desgajan a una épica propia
>
> **Puerta de entrada** · la 2.3.0 cerrada, con **1.502 tests en verde**, **236 avisos** y **640 firmas públicas**

---

## Índice

1. [Las épicas](#1-las-épicas)
2. [Mapa de las historias](#2-mapa-de-las-historias)
3. [Qué cambia para el consumidor](#3-qué-cambia-para-el-consumidor)
4. [Orden recomendado](#4-orden-recomendado)
5. [Decisiones abiertas](#5-decisiones-abiertas)
6. [Qué queda fuera](#6-qué-queda-fuera)
7. [Definición de terminado común](#7-definición-de-terminado-común)
8. [Ver también](#8-ver-también)

---

## 1. Las épicas

| # | Épica | Historias | Esfuerzo | Riesgo | Rompe |
|:-:|-------|:---------:|:--------:|:------:|:-----:|
| 9 | [Ámbitos de registro: unir las trazas de una misma operación](Epica-09-Ambitos-de-Registro.md) | US-41 … US-46 | XL | 🟠 | no previsto |

La 2.3.0 cerró el primero de los dos ceros del estándar de `Microsoft.Extensions.Logging` —la
**plantilla de mensaje**, con 242 firmas nuevas—. Queda el segundo: **`BeginScope` sigue a 0**, y con
él la imposibilidad de correlacionar las trazas de una misma ejecución.

---

## 2. Mapa de las historias

| Historia | Qué entrega | Esfuerzo | Rompe |
|----------|-------------|:--------:|:-----:|
| **US-41** | Que el espía de pruebas capture los ámbitos en vez de devolver `NullScope` | M | no |
| **US-42** | El ámbito mínimo sobre un tramo síncrono de la tubería | L | no |
| **US-43** | El ámbito en la tubería asíncrona, con la promesa medida sobre el `await` | L | no |
| **US-44** | Semántica de la excepción y del fallo dentro del tramo envuelto | M | no |
| **US-45** | Correlación: identificador de ejecución y cruce con los ejes existentes | M | no |
| **US-46** | `README.md`, `Sample` y registro en el prompt de migración | S | no |

---

## 3. Qué cambia para el consumidor

| Elemento | 2.2.0 | 2.3.0 | 2.4.0 (objetivo) |
|----------|------:|------:|-----------------:|
| `ILogger.IsEnabled` | 7 | 7 | 7 |
| `EventId` | 123 | > 123 | sin cambios |
| `ILogger<TCategory>` | 21 | 21 | ver [`D-38`](#5-decisiones-abiertas) |
| Plantilla de mensaje con argumentos | 0 | **242 firmas** | sin cambios |
| **`BeginScope`** | **0** | **0** | **objetivo de esta versión** |
| `LoggerMessage` | 0 | 0 | fuera de alcance |
| Firmas públicas del proyecto | 398 | 640 | *por decidir en `D-38`* |

**Naturaleza prevista: aditiva.** Ninguna firma existente se toca. El único cambio de contrato es que
el `README.md` deja de decir que no hay ámbitos.

---

## 4. Orden recomendado

1. **Cerrar [`D-33`](#5-decisiones-abiertas) y [`D-37`](#5-decisiones-abiertas)** · de la primera
   depende la **forma** de la API y de la segunda su **firma**. Nada de código antes.
2. **US-41** · la red de seguridad: sin un espía que vea los ámbitos no hay nada que afirmar.
3. **US-42** · el ámbito síncrono, que es la pieza de la que cuelgan las demás.
4. **US-43** y **US-44** · en paralelo sobre la US-42: una mide el `await`, la otra la excepción.
5. **US-45** · el cruce con los ejes, al final de lo técnico, como ya enseñó la US-36.
6. **US-46** · la documentación, siempre al final, para que no mienta.

> ⚠️ **Antes de empezar, hacer la medición 1 de la
> [§7 de la épica](Epica-09-Ambitos-de-Registro.md#7-apéndice--lo-que-hay-que-medir-antes-de-decidir)**:
> escribir el caso a mano con `using logger.BeginScope(...)` y comprobar qué estorba de verdad. Si no
> estorba lo suficiente, `D-33` se cierra con la opción (c) y esta versión se cierra **sin código**.

---

## 5. Decisiones abiertas

| Id | Asunto | Historia | Estado |
|----|--------|:--------:|--------|
| [`D-33`](Epica-09-Ambitos-de-Registro.md#5-decisiones-abiertas) | Forma de la API: envolver un tramo o devolver el `IDisposable` | US-42 | ⬜ **recomendada (a)** |
| [`D-34`](Epica-09-Ambitos-de-Registro.md#5-decisiones-abiertas) | Qué pasa si el tramo lanza | US-44 | ⬜ **recomendada (c)** |
| [`D-35`](Epica-09-Ambitos-de-Registro.md#5-decisiones-abiertas) | Cómo se compone con `Bind` y qué registra el ámbito | US-44 | ⬜ **recomendada (a)** |
| [`D-36`](Epica-09-Ambitos-de-Registro.md#5-decisiones-abiertas) | Si el ámbito sobrevive al `await` | US-43 | ⬜ **recomendada (b)** |
| [`D-37`](Epica-09-Ambitos-de-Registro.md#5-decisiones-abiertas) | De dónde salen las propiedades: plantilla o diccionario | US-42 | ⬜ **recomendada (c)** |
| [`D-38`](Epica-09-Ambitos-de-Registro.md#5-decisiones-abiertas) | Hasta dónde se cruza con `EventId`, niveles y `ILogger<TCategory>` | US-45 | ⬜ **recomendada (a)** |

> La numeración continúa desde `D-32`, la última de la 2.3.0. **Las seis bloquean** el código de
> producción: `D-33` y `D-37` fijan la superficie pública, y lo que se publica no se puede cambiar.

---

## 6. Qué queda fuera

| Asunto | Dónde va | Por qué |
|--------|----------|---------|
| `LoggerMessage.Define` y el generador de `[LoggerMessage]` | 3.0.0 | ya descartado en `D-30` (a): la plantilla la trae el consumidor en ejecución |
| Manejador de cadena interpolada (`D-26` (c)) | 3.0.0 | es una capa **sobre** la familia `…Template`, no sobre los ámbitos |
| Tipo de opciones `MlLogOptions` | 3.0.0 | cambia el estilo de la API |
| Pasar la `Exception` al `ILogger` | 3.0.0 | hallazgo independiente del ámbito |
| Retirar los puentes `[Obsolete]` de las épicas 4 y 8 | 3.0.0 | rompen compilación; van en una mayor |

---

## 7. Definición de terminado común

- [ ] Las **seis decisiones** `D-33` … `D-38` resueltas y escritas en el documento de la épica.
- [ ] `dotnet build … -c Release --no-incremental` ⇒ **`EXIT = 0`**, **0 errores**, **0 `CS1591`**,
      **0 `CS0618`** y **sin avisos nuevos** sobre los **236** de la línea base.
- [ ] `dotnet test … -c Release --no-build` ⇒ **`EXIT = 0`**, **0 con error**, **0 omitidos** y el
      total **por encima** de los **1.502** de la 2.3.0.
- [ ] Los dos tests de paridad de `MatrixTests.cs` en verde.
- [ ] El `Sample` se ejecuta con **`EXIT = 0`**.
- [ ] `scripts\check-md-anchors4.ps1` ⇒ **`rotos = 0`**.
- [ ] El `README.md` del proyecto refleja la API **real**.
- [ ] Todo aditivo relevante registrado en los **6 sitios** del prompt de migración.
- [ ] Commits en **Conventional Commits en castellano**, creados con `git commit -F`.
- [ ] **No se publica sin confirmación** del propietario.

---

## 8. Ver también

- [`Epica-09-Ambitos-de-Registro.md`](Epica-09-Ambitos-de-Registro.md) · la épica de esta versión
- [`README.md` de la 2.3.0](../../2.3.0/Logging/README.md) · el plan del que ésta se desgaja
- [`README.md` del proyecto](../../../../MoralesLarios.OOFP.Logging/README.md) · la API tal como está hoy
- [`oofp-upgrade-2.prompt.md`](../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md) · prompt de migración
