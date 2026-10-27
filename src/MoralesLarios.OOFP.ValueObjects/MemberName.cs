// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class MemberName : StringMinLength

{
    public static int MinLength => 3;

    private MemberName(string value) : base(value, MinLength)
    {
    }


    public static string FormatErrorMessage(string value) => StringMinLength.FormatErrorMessage(value, MinLength);
    public static new bool IsValid(string value, int length) => value.Length >= length;

    public static MemberName CreateFromString(string value) => new MemberName(value);

    public static MlResult<MemberName> FromString(string value, MlErrorDetails errorsDetails = null!)
        => StringMinLength.FromLength(value, MinLength, errorsDetails)  
                            .Map ( _ => new MemberName(value));

    public static implicit operator string(MemberName   valueObject) => valueObject.Value;
    public static implicit operator MemberName  (string value      ) => new MemberName(value);



}

