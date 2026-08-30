// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public class IntNotNegative : IntMoreThan
{
    private const int Limit = 0;

    private IntNotNegative(int value) : base(value, Limit - 1)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));
    }

    public static string BuildErrorMessage(int value) => $"{value} must be More than {Limit}";
    public static bool IsValid(int value) => value >= Limit;


    public static IntNotNegative FromInt(int value) => new IntNotNegative(value);
    public static MlResult<IntNotNegative> ByInt(int value, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
                    .Map ( _ => new IntNotNegative(value));

    public static implicit operator int            (IntNotNegative valueObject) => valueObject.Value;
    public static implicit operator IntNotNegative (int            value      ) => new IntNotNegative(value);

}

