// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class Mail : RegexValue
{
    public const string ValidationPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

    protected Mail(NotEmptyString value) : base(value, ValidationPattern) { }

    public static string FormatErrorMessage(string value) => $"{value} is not a valid mail";
    public static bool IsValid(string value) => RegexValue.IsValid(value, ValidationPattern);

    public static Mail CreateFromString(string value) => new Mail(value);

    public static MlResult<Mail> FromString(string value, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                            .Map (_ => new Mail(value));

    public static implicit operator string(Mail valueObject) => valueObject.Value;
    public static implicit operator Mail(string value) => new Mail(value);
}
