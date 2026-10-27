// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers.Extensions;
public static class ParallelExtensions
{


    public static Task<T> ToAsync<T>(this T value) => Task.FromResult(value);




}

