# 08 · Calidad, pruebas y documentación

> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Versión destino** · 2.x, como refuerzo de cobertura sin cambiar producción
> **Origen** · análisis del commit `2d07c33`, rama `release/2.0.0-nomenclatura`
> **Estado** · propuesta, pendiente de decisión
> **Esfuerzo total** · sin estimar
> **Riesgo** · 🟡 medio

---

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Alcance](#alcance)
3. [Historias de usuario](#historias-de-usuario)
4. [Orden de ejecución](#orden-de-ejecución)
5. [Decisiones abiertas](#decisiones-abiertas)
6. [Definición de terminado](#definición-de-terminado)
7. [Apéndice · código actual y propuesto](#apéndice--código-actual-y-propuesto)
8. [Ver también](#ver-también)

---

## 1. Contexto y problema

El árbol de integración contiene cuatro ficheros C# con 33 métodos `[Fact]`/`[Theory]` y 914 líneas. Las pruebas ejercitan operaciones CRUD, claves compuestas, paginación y transacciones con SQLite en memoria. No existe un proyecto `MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit`. El hallazgo se limita a una aserción concreta que no verifica el comportamiento anunciado por el nombre del test.

| Punto # | Fichero | Líneas | Naturaleza |
|---:|---|---:|---|
| 01 | `Tests.Integration/Repos/RepoVinorFpTests.cs` | 64-73 | Oráculo insuficiente para el contenido de `TryAllAsync` |

### Punto 01 — `TryAllAsync` solo comprueba no-nulidad

El método de prueba promete que la operación devuelve una lista, pero solo verifica que el `IEnumerable<T>` no sea nulo. Una secuencia vacía satisface esa aserción, así que el test no prueba que la lectura devuelva las entidades esperadas.

```csharp
// RepoVinorFpTests.cs:64-73
var result = await _sut.TryAllAsync();
result.IsFailure.Should().BeFalse();
var vinos = result.GetValueOrThrow();
vinos.Should().NotBeNull(); // ← pasa también si la secuencia no contiene los registros esperados
```

> **Propuesta** · Preparar una entidad de identidad única en el test, confirmar los cambios, ejecutar `TryAllAsync` y afirmar que el elemento aparece en la secuencia. Limpiar los datos al final siguiendo el patrón existente. No elevar la prioridad por la ausencia de cobertura: los tests de integración existentes sí comprueban resultados y estado persistido en varias rutas.

## Alcance

Revisión de calidad de aserciones y del alcance actual de pruebas. Los 33 métodos de test se contaron, pero no se midieron cobertura de líneas/ramas ni se ejecutó el suite. No se declaran como defectos los caminos no cubiertos que no tengan una aserción débil asociada.

## Historias de usuario

- Como mantenedor, quiero que la prueba de lectura general falle si la operación omite datos, no solo si devuelve `null`.
- Como consumidor, quiero que los contratos de lectura y paginación sigan protegidos por pruebas de integración observables.

## Orden de ejecución

1. Mejorar la aserción de `TryAllAsync` con datos de prueba aislados.
2. Añadir, al implementar los hallazgos relacionados, regresiones para enumeración fallida, claves nulas, orden paginado y callback fallido con transacción externa.
3. Ejecutar el suite autorizado y registrar el resultado antes de cerrar las propuestas.

## Decisiones abiertas

- D-01 · ¿Se añade un proyecto unitario para lógica independiente de EF o se mantiene la cobertura integrada en SQLite?
- D-02 · ¿Qué política de aislamiento/limpieza se aplica al preparar datos para `TryAllAsync`?

## Definición de terminado

- La prueba de `TryAllAsync` comprueba contenido conocido, no solo no-nulidad.
- Las nuevas regresiones asociadas a cambios de comportamiento prueban el observable y su alternativa anterior falla.
- No se atribuye cobertura de líneas ni ejecución satisfactoria sin haberla medido.

## Apéndice · código actual y propuesto

```csharp
// Propuesta: identidad única y comprobación del resultado observado.
var expected = await CreateAndCommitUniqueVinoAsync();
var result = await repository.TryAllAsync();
result.GetValueOrThrow().Should().Contain(item => item.Id == expected.Id);
```

## Ver también

- [03 · Modelo de error](03-Modelo-de-Error.md) · excepciones durante enumeración.
- [05 · Superficie de API y arquitectura](05-Superficie-de-API-y-Arquitectura.md) · rutas transaccionales sin regresión.
- [README del análisis](README.md).
