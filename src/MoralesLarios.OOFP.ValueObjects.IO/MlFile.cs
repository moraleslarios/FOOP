// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.IO;

public class MlFile : RegexValue
{
    public const string EndpointPattern = @"^(?:[a-zA-Z]:[\\\/]|\\\\[^\\\/]+[\\\/][^\\\/]+[\\\/]?|\.{1,2}[\\\/])?(?:[^<>:""/\\|?*\x00-\x1F]+[\\\/])*[^<>:""/\\|?*\\\/\x00-\x1F]+$";

    protected MlFile(NotEmptyString value) : base(value, EndpointPattern) { }

    public static string BuildErrorMessage(string value) => $"{value} is not a valid file path";
    public static bool IsValid(string value) => RegexValue.IsValid(value, EndpointPattern);

    public static MlFile FromString(string value) => new MlFile(value);

    public static MlResult<MlFile> ByString(string value, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
                            .Map ( _ => new MlFile(value));

    public static implicit operator string(MlFile valueObject) => valueObject.Value;
    public static implicit operator MlFile(string value      ) => new MlFile(value);
}

