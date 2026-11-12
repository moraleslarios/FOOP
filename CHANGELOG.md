# Registro de cambios

Formato basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y versionado
[SemVer](https://semver.org/lang/es/).

Un cambio **rompedor** solo entra en una versión mayor y deja un puente `[Obsolete]` durante un ciclo
cuando es posible. Cada ruptura se describe, con su acción de migración, en
[`oofp-upgrade-2.prompt.md`](src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md).

## [2.0.0] · sin publicar

Versión mayor con renombrado completo de proyectos, tipos y miembros, migración a `net11.0` y
rupturas de contrato registradas.

- **Migración 1.x → 2.x:** [`oofp-upgrade-2.prompt.md`](src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md) y su anexo de renombrados
  [`oofp-upgrade-2.renames.md`](src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.renames.md).
- **Plataforma:** todos los proyectos apuntan a `net11.0` y las dependencias de Microsoft a 11.0.0.
- **Repositorio:** SDK fijado en `global.json`, gestión central de paquetes, flujo de integración continua y
  retirada de credenciales de la configuración de pruebas.
