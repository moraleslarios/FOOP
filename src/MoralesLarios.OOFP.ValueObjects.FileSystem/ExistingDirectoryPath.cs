// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem;

public partial class ExistingDirectoryPath : NotEmptyString
{
    protected ExistingDirectoryPath(string directoryStr) : base(directoryStr)
    {
        if (!IsValid(directoryStr)) throw new DirectoryNotFoundException(FormatErrorMessage(directoryStr));
    }


    public new static string FormatErrorMessage(string directoryStr) => $"{directoryStr} not exists";
    public new static bool IsValid(string directoryStr) => Directory.Exists(directoryStr);

    public static ExistingDirectoryPath CreateFromPath(string directoryStr) => new ExistingDirectoryPath(directoryStr);

    public static MlResult<ExistingDirectoryPath> FromPath(string directoryStr, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(directoryStr)
                            .Bind( _ => EnsureFp.That(directoryStr, IsValid(directoryStr), errorsDetails ?? FormatErrorMessage(directoryStr)))
                            .Map ( _ => new ExistingDirectoryPath(directoryStr));

    public static implicit operator string        (ExistingDirectoryPath directoryStrObject) => directoryStrObject.Value;
    public static implicit operator ExistingDirectoryPath(string         directoryStr      ) => new ExistingDirectoryPath(directoryStr);
}
