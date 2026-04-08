// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EFCore.Helpers;

public static class Extensions
{



    public static string GetPkValues(this object[] source)
    {
        var result = source.Count() == 1
                     ? source[0].ToString() ?? string.Empty
                     : $"({string.Join(", ", source)})";

        return result;
    }


    public static IQueryable<T> PrivateOrderBy<T>(this IQueryable<T>               source,
                                                       OrderBy                     orderBy      = OrderBy.Ascending,
                                                       Expression<Func<T, object>> orderByField = null!)
        => orderByField is null 
                                ? source
                                : orderBy == OrderBy.Ascending
                                    ? source.OrderBy(orderByField)
                                    : source.OrderByDescending(orderByField);


    





}

