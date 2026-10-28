// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>IdLite</c>.
/// </summary>
public partial class IdLite : IntMoreThan
{
    private static int Limit => 0;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="IdLite"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected IdLite(int value) : base(value, Limit)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), BuildErrorMessIdLite(value));
    }
    /// <summary>
    /// Construye la instancia solicitada de <c>ErrorMessIdLite</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string BuildErrorMessIdLite(int value) => $"IdLite can be mayor 0.";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(int value) => IdLite.IsValid(value, Limit);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromInt</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>IdLite</c> obtenido por la operación.</returns>
    public static IdLite CreateFromInt(int value) => new IdLite(value);
    /// <summary>
    /// Ejecuta <c>FromInt</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IdLite> FromInt(int value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessIdLite(value)))
                    .Map ( _ => new IdLite(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>int</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>int</c> obtenido por la operación.</returns>
    public static implicit operator int   (IdLite valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>IdLite</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>IdLite</c> obtenido por la operación.</returns>
    public static implicit operator IdLite(int    value      ) => new IdLite(value);


}

