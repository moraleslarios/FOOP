// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class IntNonNegative : IntMoreThan
{
    private const int Limit = 0;

    private IntNonNegative(int value) : base(value, Limit - 1)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    public static string FormatErrorMessage(int value) => $"{value} must be More than {Limit}";
    public static bool IsValid(int value) => value >= Limit;


    public static IntNonNegative CreateFromInt(int value) => new IntNonNegative(value);
    public static MlResult<IntNonNegative> FromInt(int value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new IntNonNegative(value));

    public static implicit operator int            (IntNonNegative valueObject) => valueObject.Value;
    public static implicit operator IntNonNegative (int            value      ) => new IntNonNegative(value);

}

