// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class IntMoreThan : ValueObject<int>
{
    protected IntMoreThan(int value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    public static string FormatErrorMessage(int value, int length) => $"{value} must be More than {length}";
    public static bool IsValid(int value, int length) => value > length;


    public static IntMoreThan CreateFromIntLength(int value, int length) => new IntMoreThan(value, length);
    public static MlResult<IntMoreThan> FromIntLength(int value, int length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                    .Map ( _ => new IntMoreThan(value, length));

    public static implicit operator int (IntMoreThan valueObject) => valueObject.Value;
    public static implicit operator IntMoreThan ((int value, int length) tupleValues) => new IntMoreThan(tupleValues.value, tupleValues.length);



}

