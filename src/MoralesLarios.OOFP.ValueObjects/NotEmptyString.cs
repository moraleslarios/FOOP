// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.ValueObjects;

public partial class NotEmptyString : ValueObject<string>
{
    protected NotEmptyString(string value) : base(value)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} cannot be null or empty";
    public static bool IsValid(string value) => ! string.IsNullOrWhiteSpace(value);

    public static NotEmptyString CreateFromString(string value) => new NotEmptyString(value);

    public static MlResult<NotEmptyString> FromString(string value, MlErrorDetails errorsDetails = null!)
        => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value))
                    .Map(_ => new NotEmptyString(value));

    public static implicit operator string        (NotEmptyString valueObject) => valueObject.Value;
    public static implicit operator NotEmptyString(string         value      ) => new NotEmptyString(value);
}

