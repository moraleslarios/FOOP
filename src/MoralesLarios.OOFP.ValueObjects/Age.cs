// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class Age : IntMoreThan
{
    private static int Limit => -1;

    protected Age(int value) : base(value, Limit)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), FormatErrorMessage(value));
    }
    public static string FormatErrorMessage(int value) => $"Age cannot be negative.";
    public static bool IsValid(int value) => Age.IsValid(value, Limit);

    public static Age CreateFromInt(int value) => new Age(value);
    public static MlResult<Age> FromInt(int value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new Age(value));

    public static implicit operator int (Age valueObject) => valueObject.Value;
    public static implicit operator Age (int value) => new Age(value);


}

