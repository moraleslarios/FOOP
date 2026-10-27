// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class Key : StringMinLength
{
    public static int MinLength => 1;

    private Key(string value) : base(value, MinLength)
    {
    }

    public static string BuildErrorMessage(string value) => StringMinLength.BuildErrorMessage(value, MinLength);
    public static new bool IsValid(string value, int length) => value.Length >= length;

    public static Key FromString(string value) => new Key(value);

    public static MlResult<Key> ByString(string value, MlErrorsDetails errorsDetails = null!)
        => StringMinLength.ByStringLength(value, MinLength, errorsDetails)
                            .Map(_ => new Key(value));

    public static implicit operator string(Key   valueObject) => valueObject.Value;
    public static implicit operator Key   (string value      ) => new Key(value);
}

