

namespace MoralesLarios.OOFP.Validation.FluentValidations.Helpers;
public static class Extensions
{


    public static MlResult<T> ValidateWitHFluentValidations<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
    {
        var result = MlResult.Empty()
                                .TryMap( _          => Activator.CreateInstance<TValidator>(), $"Problems with automatic create instance of {typeof(TValidator).Name}")
                                .TryMap( validator  => validator.Validate(source))
                                .Map   ( valResults => valResults.Errors.Select(x => x.ErrorMessage))
                                .Bind  ( errors     => errors.Any() ? errors.ToMlResultFail<T>() : source.ToMlResultValid<T>());
        return result;
    }

    public static Task<MlResult<T>> ValidateWitHFluentValidationsAsync<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWitHFluentValidations<T, TValidator>().ToAsync();


}