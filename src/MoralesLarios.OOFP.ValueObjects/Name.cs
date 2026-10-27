// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class Name : StringMinLength

{
    public static int MinLength => 3;

    private Name(string value) : base(value, MinLength)
    {
    }


    public static string BuildErrorMessage(string value) => StringMinLength.BuildErrorMessage(value, MinLength);
    public static new bool IsValid(string value, int length) => value.Length >= length;

    public static Name FromString(string value) => new Name(value);

    public static MlResult<Name> ByString(string value, MlErrorDetails errorsDetails = null!)
        => StringMinLength.ByStringLength(value, MinLength, errorsDetails)  
                            .Map ( _ => new Name(value));

    public static implicit operator string(Name   valueObject) => valueObject.Value;
    public static implicit operator Name  (string value      ) => new Name(value);



}

