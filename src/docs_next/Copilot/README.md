# Carpeta `Copilot` — Herramientas de migración asistida

Esta carpeta contiene **prompts reutilizables de GitHub Copilot** pensados para los **consumidores
externos** de la librería `MoralesLarios.OOFP`.

No forman parte del código de la librería ni se empaquetan en NuGet: son artefactos para copiar y pegar en
el repositorio de quien usa la librería.

---

## 📦 Contenido

```
Copilot/
├── README.md                                   ← este fichero
└── .github/
    └── prompts/
        ├── oofp-upgrade-2.prompt.md            ← el prompt /oofp-upgrade-2
        └── oofp-upgrade-2.renames.md           ← anexo con el mapa completo de renombrados
```

| Fichero | Para qué sirve |
| --- | --- |
| `oofp-upgrade-2.prompt.md` | Procedimiento de migración en 7 fases (paquetes NuGet → namespaces → API → compilación → verificación → informe). |
| `oofp-upgrade-2.renames.md` | Tablas de datos: los **888 cambios** de nomenclatura, agrupados por paquete, con indicación de si rompen la compilación o solo generan aviso `CS0618`. |

> El prompt **referencia** el anexo con un enlace relativo, así que **los dos ficheros deben copiarse juntos
> y en la misma carpeta**.

---

## 🚀 Instalación en tu proyecto

### 1. Copia la carpeta `.github/prompts/` a la raíz de tu solución

Si tu repositorio es, por ejemplo, `C:\MiEmpresa\MiApp`:

```powershell
# Desde la raíz de tu repositorio
New-Item -ItemType Directory -Force -Path '.github\prompts' | Out-Null
Copy-Item -Path '<ruta-a-MoralesLarios.OOFP>\src\Copilot\.github\prompts\oofp-upgrade-2*.md' `
          -Destination '.github\prompts\'
```

En Linux/macOS:

```bash
mkdir -p .github/prompts
cp <ruta-a-MoralesLarios.OOFP>/src/Copilot/.github/prompts/oofp-upgrade-2*.md .github/prompts/
```

La estructura resultante debe ser:

```
MiApp/
├── .github/
│   └── prompts/
│       ├── oofp-upgrade-2.prompt.md
│       └── oofp-upgrade-2.renames.md
├── MiApp.sln
└── src/...
```

### 2. Asegúrate de que VS Code tiene los prompts activados

En `settings.json`:

```jsonc
{
  "chat.promptFiles": true
}
```

> En versiones recientes de VS Code esta opción está habilitada por defecto. Si no ves el prompt en la lista,
> recarga la ventana (`Developer: Reload Window`).

### 3. Invoca el prompt

Abre **Copilot Chat** en **modo Agente** y escribe:

```
/oofp-upgrade-2
```

El agente ejecutará todo el procedimiento: comprobará que tu árbol de git está limpio, actualizará los
paquetes NuGet, reescribirá los `using`, aplicará los renombrados de la API, compilará iterativamente y
te entregará un informe final.

---

## 🧭 Qué hace el prompt, fase a fase

| Fase | Acción |
| --- | --- |
| **0** | Reconocimiento: verifica git limpio, inventaría referencias, captura línea base de build y crea rama `chore/oofp-upgrade-2`. |
| **1** | Paquetes NuGet: `dotnet remove`/`dotnet add` de los 14 paquetes renombrados + subida de versión del resto. Valida que el TFM sea `net8.0`+. |
| **2** | `using` y namespaces: 13 namespaces raíz + 4 internos, aplicados de más largo a más corto. |
| **3** | API: 231 renombrados que rompen + 187 con puente `[Obsolete]`, más 9 bloques de **cambios semánticos** que requieren revisión manual. |
| **4** | Compilación iterativa: resuelve `CS0246` → `CS0234` → `CS0117`/`CS1061` → `CS0618`. |
| **5** | Verificación: build limpio, tests, barrido final de nombres antiguos y revisión del diff. |
| **6** | Informe final en español con todo lo aplicado y lo que queda pendiente. |

---

## ⚠️ Antes de lanzarlo

1. **Confirma tu trabajo en git.** El prompt lo comprueba, pero más vale prevenir: la migración toca
   muchos ficheros.
2. **Requisito de framework**: la versión 2.0.0 exige **`net8.0` o superior**. Si tu proyecto apunta a
   `net6.0`, `net7.0` o `netstandard2.x`, actualiza el TFM primero.
3. **Lee la sección 3.2 del prompt** ("Cambios con SEMÁNTICA distinta"). Hay cambios que **compilan pero se
   comportan de otra forma**:
   - las factorías de value objects **invirtieron** el significado de `From…` / `By…`;
   - `MlSelfValidatable<T>` pasó de clase abstracta a **interfaz**;
   - el parseo de fechas en los controladores usa ahora `CultureInfo.InvariantCulture`;
   - la versión **síncrona** de `ToResponseErrorsDescription` **desapareció**.
4. **Revisa el diff antes de confirmar**. El prompt está diseñado para ser conservador, pero una migración
   automática de 888 cambios merece una lectura humana.

---

## 🐢 Migración gradual (alternativa)

Si no puedes dedicar el tiempo a una migración completa, la versión 2.0.0 mantiene **187 puentes
`[Obsolete]`** que permiten que la mayoría del código 1.x siga compilando con avisos `CS0618`.

Para esa ruta, dile al agente:

```
/oofp-upgrade-2  — hazme solo la migración mínima: paquetes y namespaces, acepto los avisos CS0618
```

> ⚠️ Los puentes están marcados *"Will be removed in 3.0.0"*. La migración gradual es un aplazamiento,
> no una solución.

---

## 📚 Referencias

| Documento | Contenido |
| --- | --- |
| `Cambios Realizados/Renombrados-v2.0.0.md` | Registro exhaustivo de los 888 cambios con su lote, nivel, tipo, proyecto y nota. |
| `CONTRIBUTING.md` | Convenciones de nomenclatura de la 2.0.0: sufijos prohibidos, sufijos con significado fijo, nombres de genéricos y de booleanos. |
| `README.md` | Visión general de la librería y de sus paquetes. |

---

## 🛠️ Para mantenedores de la librería

Si en el futuro se añaden más renombrados (por ejemplo, de cara a una 3.0.0):

1. Añade las filas al registro `Cambios Realizados/_registro.csv`.
2. Regenera el entregable con `scripts/generar-cambios-realizados.ps1`.
3. Actualiza `oofp-upgrade-2.renames.md` con las tablas nuevas y el recuento del resumen de riesgo (sección 16).
4. Si cambian los paquetes, actualiza la tabla de la Fase 1 del prompt.
5. Al liberar la 3.0.0, crea `oofp-upgrade-3.prompt.md` siguiendo la misma estructura y recuerda que esa
   versión **elimina los puentes `[Obsolete]`**, por lo que la migración ya no podrá ser gradual.
