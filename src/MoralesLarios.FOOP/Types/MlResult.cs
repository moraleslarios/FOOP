using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Types;


public partial record MlResult
{
    public static MlResult _  => new(default!);
    public static MlResult Discard  => _ ;

    private string MyField;
    public static MlResult<T> Valid<T>(T t) => new(t);

    public static MlResult<T> Fail<T>(params MlError[]            errors)        => new(errors);
    public static MlResult<T> Fail<T>(       MlErrorsDetails      errorsDetails) => new(errorsDetails);
    public static MlResult<T> Fail<T>(       IEnumerable<MlError> errors       ) => new(errors);

    public static MlResult<T> Fail<T>(MlError              error                                         ) => new(error);
    public static MlResult<T> Fail<T>(IEnumerable<MlError> errors, Dictionary<string, object> details    ) => new(errors, details);
    public static MlResult<T> Fail<T>(MlError              error , Dictionary<string, object> details    ) => new(error, details);
    public static MlResult<T> Fail<T>(IEnumerable<MlError> errors, string detailsKey, object detailsValue) => new(errors, new Dictionary<string, object> { { detailsKey , detailsValue  } });
    public static MlResult<T> Fail<T>(MlError              error , string detailsKey, object detailsValue) => new(error , new Dictionary<string, object> { { detailsKey , detailsValue  } });
    public static MlResult<T> Fail<T>(IEnumerable<MlError> errors, (string key, object value) details    ) => new(errors, new Dictionary<string, object> { { details.key, details.value } });
    public static MlResult<T> Fail<T>(MlError              error , (string key, object value) details    ) => new(error , new Dictionary<string, object> { { details.key, details.value } });



    public static MlResult<object> Empty() => new object();

    public static Task<MlResult<object>> EmptyAsync() => Empty().ToAsync();


}






public partial record MlResult<T>
{
    internal protected T               Value         { get; init; }
    internal protected MlErrorsDetails ErrorsDetails { get; init; }

    public bool IsValid { get; init; }
    public bool IsFail => ! IsValid;


    public static MlResult<T> _  => new(default(T)!);
    public static MlResult<T> Discard => _ ;


    internal MlResult(T                    t                                                        ) => (Value, ErrorsDetails, IsValid) = (t       , new MlErrorsDetails()                                    , true );
    internal MlResult(MlErrorsDetails                                         errorsDetails         ) => (Value, ErrorsDetails, IsValid) = (default!, errorsDetails                                            , false);
    internal MlResult(IEnumerable<MlError> errors, Dictionary<string, object> details        = null!) => (Value, ErrorsDetails, IsValid) = (default!, new MlErrorsDetails(errors, details)                     , false);
    internal MlResult(MlError              error , Dictionary<string, object> details        = null!) => (Value, ErrorsDetails, IsValid) = (default!, new MlErrorsDetails(new List<MlError> { error }, details), false);



    public static MlResult<T> Valid(T t) => new(t);

    public static MlResult<T> Fail(params MlError[]            errors       ) => new(errors);
    public static MlResult<T> Fail(       IEnumerable<MlError> errors       ) => new(errors);
    public static MlResult<T> Fail(       MlErrorsDetails      errorsDetails) => new(errorsDetails);
    public static MlResult<T> Fail(       MlError              error        ) => new(error);

    public static MlResult<T> Fail(IEnumerable<MlError> errors, Dictionary<string, object> details    ) => new(errors, details);
    public static MlResult<T> Fail(MlError              error , Dictionary<string, object> details    ) => new(error , details);
    public static MlResult<T> Fail(IEnumerable<MlError> errors, string detailsKey, object detailsValue) => new(errors, new Dictionary<string, object> { { detailsKey, detailsValue } });
    public static MlResult<T> Fail(MlError              error , string detailsKey, object detailsValue) => new(error , new Dictionary<string, object> { { detailsKey, detailsValue } });
    public static MlResult<T> Fail(IEnumerable<MlError> errors, (string key, object value) details    ) => new(errors, new Dictionary<string, object> { { details.key, details.value } });
    public static MlResult<T> Fail(MlError              error , (string key, object value) details    ) => new(error , new Dictionary<string, object> { { details.key, details.value } });


    public static Task<MlResult<T>> ValidAsync(T t) => MlResult<T>.Valid(t).ToAsync();

    public static Task<MlResult<T>> FailAsync(params MlError[]            errors       ) => MlResult<T>.Fail(errors       ).ToAsync();
    public static Task<MlResult<T>> FailAsync(       IEnumerable<MlError> errors       ) => MlResult<T>.Fail(errors       ).ToAsync();
    public static Task<MlResult<T>> FailAsync(       MlErrorsDetails      errorsDetails) => MlResult<T>.Fail(errorsDetails).ToAsync();
    public static Task<MlResult<T>> FailAsync(       MlError              error        ) => MlResult<T>.Fail(error        ).ToAsync();

    public static Task<MlResult<T>> FailAsync(IEnumerable<MlError> errors, Dictionary<string, object> details    ) => MlResult<T>.Fail(errors, details).ToAsync();
    public static Task<MlResult<T>> FailAsync(MlError              error , Dictionary<string, object> details    ) => MlResult<T>.Fail(error , details).ToAsync();
    public static Task<MlResult<T>> FailAsync(IEnumerable<MlError> errors, string detailsKey, object detailsValue) => MlResult<T>.Fail(errors, new Dictionary<string, object> { { detailsKey, detailsValue } }  ).ToAsync();
    public static Task<MlResult<T>> FailAsync(MlError              error , string detailsKey, object detailsValue) => MlResult<T>.Fail(error , new Dictionary<string, object> { { detailsKey, detailsValue } }  ).ToAsync();
    public static Task<MlResult<T>> FailAsync(IEnumerable<MlError> errors, (string key, object value) details    ) => MlResult<T>.Fail(errors, new Dictionary<string, object> { { details.key, details.value } }).ToAsync();
    public static Task<MlResult<T>> FailAsync(MlError              error , (string key, object value) details    ) => MlResult<T>.Fail(error , new Dictionary<string, object> { { details.key, details.value } }).ToAsync();



    

    public static implicit operator MlResult<T>(T t) => Valid(t);

    public static implicit operator MlResult<T>(MlErrorsDetails errorsDetails) => Fail(errorsDetails);

    public static implicit operator MlResult<T>(List<MlError> errors) => Fail(errors.AsEnumerable());
    public static implicit operator MlResult<T>(MlError[]     errors) => Fail(errors);
    public static implicit operator MlResult<T>(MlError       error ) => Fail(error);


    public static implicit operator MlResult<T>((IEnumerable<MlError> errors, Dictionary<string, object> details)     parameters) => Fail(parameters.errors, parameters.details);
    public static implicit operator MlResult<T>((MlError              error , Dictionary<string, object> details)     parameters) => Fail(parameters.error , parameters.details);
    public static implicit operator MlResult<T>((IEnumerable<MlError> errors, string detailsKey, object detailsValue) parameters) => Fail(parameters.errors, parameters.detailsKey, parameters.detailsValue);
    public static implicit operator MlResult<T>((MlError              error , string detailsKey, object detailsValue) parameters) => Fail(parameters.error , parameters.detailsKey, parameters.detailsValue);


    public override string ToString() => this.Match(
                                                        fail : errors => errors.ToString(),
                                                        valid: value  => value?.ToString() ?? "Not right value"
                                                   );





}
