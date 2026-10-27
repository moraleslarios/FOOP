// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem;

public partial class ExistingFilePath : NotEmptyString
{
    protected ExistingFilePath(string pathStr) : base(pathStr)
    {
        if ( ! IsValid(pathStr)) throw new FileNotFoundException(FormatErrorMessage(pathStr));
    } 


    public new static string FormatErrorMessage(string pathStr) => $"{pathStr} not exists";
    public new static bool IsValid(string pathStr) => File.Exists(pathStr);

    public static ExistingFilePath CreateFromPath(string pathStr) => new ExistingFilePath(pathStr);

    public static MlResult<ExistingFilePath> FromPath(string pathStr, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(pathStr)
                            .Bind( _ => EnsureFp.That(pathStr, IsValid(pathStr), errorsDetails ?? FormatErrorMessage(pathStr)))
                            .Map ( _ => new ExistingFilePath(pathStr));

    public static implicit operator string    (ExistingFilePath pathStrObject) => pathStrObject.Value;
    public static implicit operator ExistingFilePath(string     pathStr      ) => new ExistingFilePath(pathStr);
}

