// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public class EmptyValue : ValueObject<string>
{
    protected EmptyValue(string value) : base(string.Empty)
    {
    }


    public static EmptyValue Create() => new EmptyValue(string.Empty);


    public static Task<EmptyValue> CreateAsync() => Create().ToAsync();


}

