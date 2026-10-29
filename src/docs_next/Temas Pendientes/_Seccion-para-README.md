# Fragmento listo para pegar en `src/README.md`

> 📌 Este archivo existe solo como **respaldo temporal**. Contiene la sección que enlaza esta carpeta
> desde el `README.md` de la solución. Pégala al final de `src/README.md` (justo después de la
> sección «Nota final») y luego **borra este archivo**.

---

```markdown
---

## 🗂️ Temas pendientes: mejoras, nomenclatura y profesionalización

La carpeta [**`Temas Pendientes`**](./Temas%20Pendientes/README.md) reúne los documentos de trabajo
sobre el estado de la solución: qué hay que arreglar, en qué orden y qué cambiaría para
profesionalizar la biblioteca. Ninguno de ellos modifica código: son inventarios y guías de decisión.

| Documento | Contenido |
|---|---|
| [🗂️ **Índice de la carpeta**](./Temas%20Pendientes/README.md) | Resumen global de los 89 puntos, resumen por proyecto, plan de trabajo por bloques y orden de lectura recomendado. |
| [🔴🟠 Mejoras de prioridad crítica y alta](./Temas%20Pendientes/Mejoras-Prioridad-Critica-y-Alta.md) | Puntos 1-37: bugs que producen resultados incorrectos, seguridad, inyección de dependencias, culturas y contratos rotos. |
| [🟡 Mejoras de prioridad media](./Temas%20Pendientes/Mejoras-Prioridad-Media.md) | Puntos 38-63: rendimiento y acceso a datos, diseño de API y coherencia funcional. |
| [🟢 Mejoras de prioridad baja](./Temas%20Pendientes/Mejoras-Prioridad-Baja.md) | Puntos 64-89: código muerto, erratas en identificadores públicos, mensajes al usuario y documentación. |
| [🔤 Consejos de nomenclatura](./Temas%20Pendientes/Consejos-Nomenclatura.md) | Propuesta de renombrado en 10 niveles (solución, proyectos, carpetas, tipos, métodos y propiedades), con tablas «nombre actual → nombre propuesto» y una estrategia de migración con `[Obsolete]`. |
| [🏗️ Profesionalización (1/2): ingeniería y calidad](./Temas%20Pendientes/Profesionalizacion-Ingenieria-y-Calidad.md) | Higiene del repositorio, `Directory.Build.props`, gestión centralizada de paquetes, analizadores, nulabilidad, estrategia de pruebas, metadatos NuGet, SemVer, CI/CD y seguridad de la cadena de suministro. |
| [🎨 Profesionalización (2/2): diseño de API y producto](./Temas%20Pendientes/Profesionalizacion-Diseno-API-y-Producto.md) | Superficie pública, asincronía y cancelación, modelo de errores tipado, i18n, rendimiento de `MlResult<T>`, arquitectura por capas, observabilidad, documentación como producto, comunidad y hoja de ruta en 4 fases. |

**Por dónde empezar:** el [índice de la carpeta](./Temas%20Pendientes/README.md) → los puntos 🔴 críticos
→ la fase 1 de la hoja de ruta (credibilidad del repositorio, sin cambios de comportamiento).
```

---

## Aviso adicional sugerido

En la sección «README de cada proyecto» de `src/README.md` conviene añadir, tras el último elemento
de la lista:

```markdown
> ⚠️ `MoralesLarios.OOFP.EFCore.WebApi` y `MoralesLarios.OOFP.WebControllers.Cache` son los dos únicos
> proyectos que **todavía no tienen `README.md`** propio (recogido como punto 89 del inventario de mejoras).
```
