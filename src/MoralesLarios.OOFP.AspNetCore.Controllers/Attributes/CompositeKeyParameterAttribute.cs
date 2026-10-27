// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Attributes;

/// <summary>
/// Atributo para documentar par�metros de clave primaria (PK) en endpoints REST.
/// Proporciona informaci�n sobre el formato esperado de los valores de PK, especialmente para claves compuestas.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class CompositeKeyParameterAttribute : Attribute
{
    /// <summary>
    /// Descripci�n del formato esperado del par�metro PK.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia del atributo <see cref="CompositeKeyParameterAttribute"/>.
    /// </summary>
    /// <param name="description">Descripci�n del formato. Si es nulo, usa la descripci�n por defecto.</param>
    public CompositeKeyParameterAttribute(string description = null!)
    {
        Description = description ?? "Valores de la clave primaria separados por comas. " +
                                      "Para DateTime usa formato ISO 8601: yyyy-MM-ddTHH:mm:ss.fff " +
                                      "(Ejemplo: '1,2' para PKs compuestas o '2026-05-16T07:34:29.239' para DateTime)";
    }
}
