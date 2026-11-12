# Política de seguridad

## Versiones con soporte

| Versión | Soporte |
|---|---|
| 2.0.x | Correcciones de seguridad |
| 1.x | Sin soporte: migra con [`oofp-upgrade-2.prompt.md`](src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md) |

## Cómo informar de una vulnerabilidad

No abras una *issue* pública. Usa el aviso privado de seguridad de GitHub del repositorio
(*Security → Report a vulnerability*) e incluye:

- el paquete y la versión afectados;
- los pasos para reproducirlo o un ejemplo mínimo;
- el impacto que has observado.

Se acusará recibo del informe y se publicará la corrección junto con un aviso en el
[`CHANGELOG.md`](CHANGELOG.md) cuando esté disponible.

## Alcance

Las bibliotecas no almacenan ni transmiten secretos por sí mismas. Aun así, `Http` y `Configuration`
trabajan con direcciones, cabeceras y cadenas de conexión: **no incluyas credenciales reales en un
informe público ni en una prueba** (ver *Secretos y configuración local* en
[`src/CONTRIBUTING.md`](src/CONTRIBUTING.md)).
