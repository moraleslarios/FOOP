// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public class Void : ValueObject<string>
{
    protected Void(string value) : base(string.Empty)
    {
    }


    public static Void Create() => new Void(string.Empty);

}

