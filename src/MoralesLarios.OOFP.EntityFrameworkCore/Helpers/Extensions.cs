// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Helpers;

/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{



    /// <summary>
    /// Ejecuta <c>FormatCompositeKey</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatCompositeKey(this object[] source)
    {
        var result = source.Count() == 1
                     ? source[0].ToString() ?? string.Empty
                     : $"({string.Join(", ", source)})";

        return result;
    }


    /// <summary>
    /// Ejecuta <c>PrivateOrderBy</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static IQueryable<T> PrivateOrderBy<T>(this IQueryable<T>               source,
                                                       SortDirection                     orderBy      = SortDirection.Ascending,
                                                       Expression<Func<T, object>> orderByField = null!)
        => orderByField is null 
                                ? source
                                : orderBy == SortDirection.Ascending
                                    ? source.OrderBy(orderByField)
                                    : source.OrderByDescending(orderByField);


    





}

