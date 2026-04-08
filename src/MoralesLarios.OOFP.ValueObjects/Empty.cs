// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public class Empty : ValueObject<string>
{
    protected Empty(string value) : base(string.Empty)
    {
    }


    public static Empty Create() => new Empty(string.Empty);


    public static Task<Empty> CreateAsync() => Create().ToAsync();


}

