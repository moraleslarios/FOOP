// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>Void</c>.
/// </summary>
public class Void : ValueObject<string>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="Void"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected Void(string value) : base(string.Empty)
    {
    }


    /// <summary>
    /// Crea la instancia solicitada.
    /// </summary>
    /// <returns>Valor de tipo <c>Void</c> obtenido por la operación.</returns>
    public static Void Create() => new Void(string.Empty);

}

