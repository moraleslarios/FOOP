// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Helpers;

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
                                                       SortDirection                     orderBy      = SortDirection.Ascending,
                                                       Expression<Func<T, object>> orderByField = null!)
        => orderByField is null 
                                ? source
                                : orderBy == SortDirection.Ascending
                                    ? source.OrderBy(orderByField)
                                    : source.OrderByDescending(orderByField);


    





}

