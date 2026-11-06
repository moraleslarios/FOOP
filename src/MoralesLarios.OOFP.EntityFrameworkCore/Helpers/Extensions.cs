// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Helpers;

/// <summary>
/// Provides extensions for composite-key formatting and EF Core query ordering.
/// </summary>
public static partial class Extensions
{



    /// <summary>
    /// Formats primary-key values as a string.
    /// </summary>
    /// <param name="source">The primary-key values to format.</param>
    /// <returns>A string representation of the values.</returns>
    public static string FormatCompositeKey(this object[] source)
    {
        var result = source.Count() == 1
                     ? source[0].ToString() ?? string.Empty
                     : $"({string.Join(", ", source)})";

        return result;
    }


    /// <summary>
    /// Applies the requested ordering when a selector is provided; otherwise, returns the source unchanged.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="orderBy">The direction used to order the query.</param>
    /// <param name="orderByField">An optional ordering selector.</param>
    /// <returns>The ordered query, or the unchanged source when no selector is supplied.</returns>
    public static IQueryable<TEntity> PrivateOrderBy<TEntity>(this IQueryable<TEntity>               source,
                                                       SortDirection                     orderBy      = SortDirection.Ascending,
                                                       Expression<Func<TEntity, object>> orderByField = null!)
        => orderByField is null 
                                ? source
                                : orderBy == SortDirection.Ascending
                                    ? source.OrderBy(orderByField)
                                    : source.OrderByDescending(orderByField);

    internal static IQueryable<T> OrderByStableForPagination<T>(this IQueryable<T> source,
                                                                 DbContext dbContext,
                                                                 SortDirection orderBy,
                                                                 Expression<Func<T, object>>? orderByField)
        where T : class
    {
        var primaryKey = dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
        if (primaryKey is null)
            throw new InvalidOperationException(
                $"Entity type {typeof(T).Name} has no primary key; stable ordering requires an EF primary key.");

        var orderedQuery = source;
        var hasOrdering = false;

        if (orderByField is not null)
        {
            orderedQuery = orderBy == SortDirection.Ascending
                ? source.OrderBy(orderByField)
                : source.OrderByDescending(orderByField);
            hasOrdering = true;
        }

        if (primaryKey is null)
            return orderedQuery;

        foreach (var keyProperty in primaryKey.Properties)
        {
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            var propertyAccess = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [keyProperty.ClrType],
                entityParameter,
                Expression.Constant(keyProperty.Name));
            var keySelector = Expression.Lambda(propertyAccess, entityParameter);
            var methodName = (orderBy, hasOrdering) switch
            {
                (SortDirection.Ascending, false) => nameof(Queryable.OrderBy),
                (SortDirection.Ascending, true) => nameof(Queryable.ThenBy),
                (_, false) => nameof(Queryable.OrderByDescending),
                _ => nameof(Queryable.ThenByDescending)
            };
            var orderedExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(T), keyProperty.ClrType],
                orderedQuery.Expression,
                Expression.Quote(keySelector));

            orderedQuery = orderedQuery.Provider.CreateQuery<T>(orderedExpression);
            hasOrdering = true;
        }

        return orderedQuery;
    }


    





}

