// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>EmptyValue</c>.
/// </summary>
public class EmptyValue : ValueObject<string>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="EmptyValue"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected EmptyValue(string value) : base(string.Empty)
    {
    }


    /// <summary>
    /// Crea la instancia solicitada.
    /// </summary>
    /// <returns>Valor de tipo <c>EmptyValue</c> obtenido por la operación.</returns>
    public static EmptyValue Create() => new EmptyValue(string.Empty);


    /// <summary>
    /// Crea la instancia solicitada. Versión asíncrona.
    /// </summary>
    /// <returns>Tarea que produce el valor de tipo <c>EmptyValue</c> obtenido por la operación.</returns>
    public static Task<EmptyValue> CreateAsync() => Create().ToAsync();


}

