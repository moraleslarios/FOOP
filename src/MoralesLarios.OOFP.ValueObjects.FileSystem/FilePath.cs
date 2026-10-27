// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem;

public partial class FilePath : RegexValue
{
    public const string SearchPattern = @"^(?:[a-zA-Z]:[\\\/]|\\\\[^\\\/]+[\\\/][^\\\/]+[\\\/]?|\.{1,2}[\\\/])?(?:[^<>:""/\\|?*\x00-\x1F]+[\\\/])*[^<>:""/\\|?*\\\/\x00-\x1F]+$";

    protected FilePath(NotEmptyString value) : base(value, SearchPattern) { }

    public static string FormatErrorMessage(string value) => $"{value} is not a valid file path";
    public static bool IsValid(string value) => RegexValue.IsValid(value, SearchPattern);

    public static FilePath CreateFromPath(string value) => new FilePath(value);

    public static MlResult<FilePath> FromPath(string value, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                            .Map ( _ => new FilePath(value));

    public static implicit operator string(FilePath valueObject) => valueObject.Value;
    public static implicit operator FilePath(string value      ) => new FilePath(value);
}

