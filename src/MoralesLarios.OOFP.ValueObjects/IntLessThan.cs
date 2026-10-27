// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class IntLessThan : ValueObject<int>
{
    protected IntLessThan(int value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }

    public static string BuildErrorMessage(int value, int length) => $"{value} must be less than {length}";
    public static bool IsValid(int value, int length) => value < length;

    public static IntLessThan FromIntLength(int value, int length) => new IntLessThan(value, length);
    public static MlResult<IntLessThan> ByIntLength(int value, int length, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? BuildErrorMessage(value, length)))
                    .Map ( _ => new IntLessThan(value, length));

    public static implicit operator int (IntLessThan valueObject) => valueObject.Value;
    public static implicit operator IntLessThan ((int value, int length) tupleValues) => new IntLessThan(tupleValues.value, tupleValues.length);



}

