// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class Endpoint : RegexValue
{
    public const string ValidationPattern = @"^https://[A-Za-z0-9.-]+(?::[1-9][0-9]{0,4})?$"; // host + optional port

    protected Endpoint(NotEmptyString value) : base(value, ValidationPattern) { }

    public static string FormatErrorMessage(string value) => $"{value} is not a valid endpoint";
    public static bool   IsValid(string value)          => RegexValue.IsValid(value, ValidationPattern);

    public static Endpoint CreateFromString(string value) => new Endpoint(value);

    public static MlResult<Endpoint> FromString(string value, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                            .Map (_ => new Endpoint(value));

    public static implicit operator string  (Endpoint valueObject) => valueObject.Value;
    public static implicit operator Endpoint(string   value      ) => new Endpoint(value);
}
        
