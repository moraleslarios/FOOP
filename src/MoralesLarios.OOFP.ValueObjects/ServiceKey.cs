// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class ServiceKey : StringMinLength
{
    public static int MinLength => 1;

    private ServiceKey(string value) : base(value, MinLength)
    {
    }

    public static string FormatErrorMessage(string value) => StringMinLength.FormatErrorMessage(value, MinLength);
    public static new bool IsValid(string value, int length) => value.Length >= length;

    public static ServiceKey CreateFromString(string value) => new ServiceKey(value);

    public static MlResult<ServiceKey> FromString(string value, MlErrorDetails errorsDetails = null!)
        => StringMinLength.FromLength(value, MinLength, errorsDetails)
                            .Map(_ => new ServiceKey(value));

    public static implicit operator string(ServiceKey   valueObject) => valueObject.Value;
    public static implicit operator ServiceKey   (string value      ) => new ServiceKey(value);
}

