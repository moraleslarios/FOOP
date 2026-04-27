# MoralesLarios.OOFP.Validation

Proyecto base para abstracciones de validación funcional.

## Qué hace

Define la clase abstracta `MlValidableFp<T>`, que obliga a exponer una validación que devuelve `MlResult<T>`.

```csharp
public abstract class MlValidableFp<T>
    where T : class
{
    public abstract MlResult<T> Validate();
}
```

## Dependencias

- `MoralesLarios.OOFP` (FOOP core)

## Uso

Se utiliza como contrato base para objetos/DTOs auto-validables.

Ejemplo:

```csharp
public class CreateUserRequest : MlValidableFp<CreateUserRequest>
{
    public string Email { get; init; } = string.Empty;

    public override MlResult<CreateUserRequest> Validate()
        => string.IsNullOrWhiteSpace(Email)
            ? "Email requerido".ToMlResultFail<CreateUserRequest>()
            : this.ToMlResultValid();
}
```

## Relación con otros proyectos

- `MoralesLarios.OOFP.Validation.Dataannotations`: validación con atributos `[Required]`, etc.
- `MoralesLarios.OOFP.Validation.FluentValidations`: validación con `FluentValidation`.
