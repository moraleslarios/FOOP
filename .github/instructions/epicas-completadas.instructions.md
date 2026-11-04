---
applyTo: 'docs_next/Next_Versions/2.0.0/**'
description: 'Mantener las épicas completadas fuera de Pendientes y sincronizar los índices y enlaces de la planificación 2.0.0.'
---

# Organización de épicas completadas

Esta regla se aplica en cada tarea que implemente o cierre historias de una épica de la versión `2.0.0`.

## Al cerrar una épica

1. Comprueba los criterios de aceptación y la definición de terminado de la épica. Solo se considera completada cuando todos están satisfechos y las verificaciones requeridas pasan.
2. Si está completada, mueve el documento de `Pendientes/` a `Realizados/<Área>/` con `git mv` para conservar el historial. Usa `Core`, `Logging`, `EFCore` u otra carpeta que identifique el proyecto afectado; crea el área si todavía no existe.
3. Si la épica solo está parcialmente realizada, no la muevas. Actualiza su estado en la cabecera y deja explícito qué historias siguen pendientes.
4. Recalcula y corrige todos los enlaces relativos dentro del documento movido y en los documentos que lo enlazan. Busca el nombre del archivo en todo `docs_next/` antes de darlo por cerrado.
5. Actualiza `docs_next/Next_Versions/2.0.0/Pendientes/README.md` y `docs_next/Next_Versions/2.0.0/README.md`: quita la épica del inventario pendiente, incorpórala al índice de realizados y sincroniza los estados, totales y orden de ejecución que hayan cambiado.
6. Mantén un `README.md` índice por área bajo `Realizados/`, con enlaces a las épicas completadas y sus evidencias de verificación.
7. Ejecuta el verificador de anclajes Markdown y revisa el diff para confirmar que no quedan enlaces a la ubicación anterior.

## Criterio de ubicación

`Pendientes/` contiene épicas no iniciadas o parcialmente realizadas. `Realizados/<Área>/` contiene exclusivamente épicas con todas sus historias y su definición de terminado completadas. No dejes una épica completada en `Pendientes/` aunque se haya cerrado en una tarea o commit anterior.
