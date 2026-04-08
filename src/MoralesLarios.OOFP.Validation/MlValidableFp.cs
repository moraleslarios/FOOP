// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Validation;
public abstract class MlValidableFp<T>
    where T : class
{


    public abstract MlResult<T> Validate();





}

