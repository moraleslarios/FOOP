# Épica · Migración de todos los proyectos a .NET 11

> **Versión destino:** 2.0.0
> **Proyecto:** solución completa MoralesLarios.OOFP
> **Estado:** ✅ Completada
> **Riesgo:** 🔴 alto · ruptura de compatibilidad para consumidores con TFM anteriores a `net11.0`

## Resultado

Los 34 proyectos `.csproj` del repositorio apuntan a `net11.0`. Se actualizaron las referencias directas `Microsoft.Extensions.*` y `Microsoft.EntityFrameworkCore.*` a `11.0.0`, y los proyectos de tests usan `Microsoft.NET.Test.Sdk 18.12.0`. Las herramientas `MlDoc` y `MlRename`, que no pertenecen a la solución, también compilan en Release.

El inventario de partida era 22 proyectos `net8.0` y 11 proyectos `net9.0`; el nuevo proyecto `MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit` partía de `net8.0`. Los 34 terminan en `net11.0`.

## Compatibilidad

La publicación de OOFP 2.0.0 ofrece activos `net11.0` únicamente. Los consumidores que sigan en `net8.0`, `net9.0` o `net10.0` no podrán restaurar los paquetes (`NU1202`); deben actualizar todos sus proyectos .NET, no solo el que referencia OOFP. La ruptura está registrada como punto **84** en `oofp-upgrade-2.prompt.md`.

No se encontró `global.json`, workflow de CI, imagen Docker ni otro archivo de build que fijara un SDK o runtime anterior. ASP.NET Core continúa llegando por `FrameworkReference Include="Microsoft.AspNetCore.App"`.

## Verificación

- [x] Los 34 proyectos apuntan a `net11.0`.
- [x] Las referencias directas Microsoft usan versiones 11.x; los test SDK usan 18.12.0.
- [x] `dotnet restore src/MoralesLarios.OOFP.sln --nologo` termina con código 0.
- [x] `dotnet build src/MoralesLarios.OOFP.sln -c Release --no-incremental -v q --nologo` termina con código 0.
- [x] `dotnet test src/MoralesLarios.OOFP.sln -c Release --no-build --nologo` pasa 1715 tests.
- [x] `MlDoc` y `MlRename` compilan en Release.
- [x] El prompt registra el TFM nuevo en la tabla, el detalle, el barrido, el informe final y el apéndice.
