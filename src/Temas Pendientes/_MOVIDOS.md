# Documentos movidos a esta carpeta

> 📌 **Qué es este archivo**
> Una nota de trazabilidad. Los dos documentos de trabajo que originalmente vivían en la raíz de
> `src/` se han **consolidado dentro de esta carpeta** con nombres homogéneos y con los enlaces
> relativos ya corregidos.

---

## Correspondencia de archivos

| Ubicación antigua (raíz de `src/`) | Ubicación actual (esta carpeta) | Estado |
|---|---|---|
| `src/MEJORAS_PENDIENTES.md` | [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) + [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) + [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) | Reemplazado y **dividido en tres** |
| `src/CONSEJOS_NOMENCLATURA.md` | [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) | Reemplazado tal cual |

---

## Por qué se dividió el inventario de mejoras

El archivo original acumulaba los 89 puntos en un solo documento y, además, había quedado con
**contenido repetido** (la sección de prioridad media aparecía triplicada al final, cada repetición
precedida por un encabezado de prioridad baja mal etiquetado). Al separarlo por niveles de prioridad:

- cada documento se abre y se navega rápido;
- la numeración global (1 → 89) se mantiene, de modo que cualquier referencia externa del tipo
  «punto 34» sigue siendo válida;
- se puede trabajar por bloques sin arrastrar los puntos que aún no toca abordar.

---

## Los originales se pueden borrar

Los dos archivos de la raíz de `src/` ya **no son la fuente de verdad**. Se han dejado en su sitio
únicamente con una nota puntero para no romper marcadores o enlaces antiguos. Cuando se confirme que
nadie los referencia, se pueden eliminar sin pérdida de información:

```text
src/MEJORAS_PENDIENTES.md
src/CONSEJOS_NOMENCLATURA.md
```

---

## Ver también

- [`README.md`](README.md) — índice y plan de trabajo de esta carpeta.
- [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — puntos 1-37.
- [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) — puntos 38-63.
- [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) — puntos 64-89.
- [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — propuesta de renombrado.
- [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) ·
  [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md)
