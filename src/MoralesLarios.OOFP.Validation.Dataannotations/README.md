# MoralesLarios.OOFP.Validation.Dataannotations

Validación funcional basada en `System.ComponentModel.DataAnnotations`, devolviendo `MlResult<T>`.

## Qué hace

- Valida objetos con atributos de data annotations.
- Soporta objetos individuales y colecciones.
- Exposición síncrona y asíncrona.

## Dependencias

- `MoralesLarios.OOFP.Validation`

## Clases y métodos

## `DataannotationsValidator`

- `Validate<T>(T source)`
- `ValidateAsync<T>(T source)`
- `ValidateAsync<T>(Task<T> sourceAsync)`
- `Validate<T>(IEnumerable<T> source)`
- `ValidateAsync<T>(IEnumerable<T> source)`
- `ValidateAsync<T>(Task<IEnumerable<T>> sourceAsync)`

Ejemplo:

```csharp
var result = DataannotationsValidator.Validate(requestDto);
```

### `Helpers.Extensions`

- `ValidateObject(this object source)` -> `IEnumerable<ValidationResult>`
- `ValidateWithDataannotations<T>(this T source)` -> `MlResult<T>`
- Variantes async.
- Soporte para `IEnumerable<T>` con fusión de errores (`FusionErrosIfExists`).

Ejemplo:

```csharp
public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

MlResult<CreateUserDto> r = new CreateUserDto { Email = "x" }
    .ValidateWithDataannotations();
```

Si hay errores devuelve `Fail` con los mensajes (`ErrorMessage`) de las validaciones.
