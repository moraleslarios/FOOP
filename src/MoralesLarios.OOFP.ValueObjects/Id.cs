// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>Id</c>.
/// </summary>
public class Id : DoubleMoreThan
{
	private static double Limit => 0;

	/// <summary>
	/// Inicializa una nueva instancia de <see cref="Id"/>.
	/// </summary>
	/// <param name="value">Valor sobre el que se opera.</param>
	protected Id(double value) : base(value, Limit)
	{
		if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), BuildErrorMessage(value));
	}
	/// <summary>
	/// Construye la instancia solicitada de <c>ErrorMessage</c>.
	/// </summary>
	/// <param name="value">Valor sobre el que se opera.</param>
	/// <returns>Representación textual obtenida.</returns>
	public static string BuildErrorMessage(double value) => $"Id can be mayor 0.";
	/// <summary>
	/// Ejecuta <c>IsValid</c>.
	/// </summary>
	/// <param name="value">Valor sobre el que se opera.</param>
	/// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
	public static bool IsValid(double value) => Id.IsValid(value, Limit);

	/// <summary>
	/// Ejecuta <c>FromDouble</c>.
	/// </summary>
	/// <param name="value">Valor sobre el que se opera.</param>
	/// <returns>Valor de tipo <c>Id</c> obtenido por la operación.</returns>
	public static Id FromDouble(double value) => new Id(value);
	/// <summary>
	/// Ejecuta <c>ByDouble</c>.
	/// </summary>
	/// <param name="value">Valor sobre el que se opera.</param>
	/// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
	/// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
	public static MlResult<Id> ByDouble(double value, MlErrorDetails errorsDetails = null!)
		=> MlResult.Empty()
					.Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
					.Map ( _ => new Id(value));

	/// <summary>
	/// Realiza la conversión implícita a <c>double</c>.
	/// </summary>
	/// <param name="valueObject">Valor de <c>valueObject</c>.</param>
	/// <returns>Valor de tipo <c>double</c> obtenido por la operación.</returns>
	public static implicit operator double(Id     valueObject) => valueObject.Value;
	/// <summary>
	/// Realiza la conversión implícita a <c>Id</c>.
	/// </summary>
	/// <param name="value">Valor sobre el que se opera.</param>
	/// <returns>Valor de tipo <c>Id</c> obtenido por la operación.</returns>
	public static implicit operator		Id(double value		 ) => new Id(value);


}
