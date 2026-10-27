// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem;

public partial class DirectoryPath : RegexValue
{
    public const string SearchPattern = @"^(?:[a-zA-Z]:[\\\/]|\\\\[^\\\/]+[\\\/][^\\\/]+[\\\/]?|\.{1,2}[\\\/])?(?:[^<>:""/\\|?*\x00-\x1F]+[\\\/])*[^<>:""/\\|?*\x00-\x1F]*$";

    protected DirectoryPath(NotEmptyString value) : base(value, SearchPattern) { }

    public static string FormatErrorMessage(string value) => $"{value} is not a valid directory path";
    public static bool IsValid(string value) => RegexValue.IsValid(value, SearchPattern);

    public static DirectoryPath CreateFromPath(string value) => new DirectoryPath(value);

    public static MlResult<DirectoryPath> FromPath(string value, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                            .Map ( _ => new DirectoryPath(value));

    public static implicit operator string     (DirectoryPath valueObject) => valueObject.Value;
    public static implicit operator DirectoryPath(string      value)       => new DirectoryPath(value);
}

