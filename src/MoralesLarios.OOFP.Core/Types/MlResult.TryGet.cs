// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;

/// <summary>
/// Miembros de <see cref="MlResult{T}"/> que exponen el contenido del resultado con la forma
/// clásica de la BCL (<see langword="bool"/> de retorno y parámetro <c>out</c>).
/// </summary>
/// <remarks>
/// Estos son los <b>únicos</b> miembros de la biblioteca con la semántica <c>Try*</c> de la BCL,
/// es decir, la de <c>int.TryParse</c> o <c>IDictionary{TKey,TValue}.TryGetValue</c>. El resto de
/// la familia <c>Try*</c> del núcleo tiene una semántica propia y distinta: envuelve la ejecución
/// en un <c>try/catch</c> y convierte la excepción en un fallo de <see cref="MlResult{T}"/>.
/// Véanse los «Convenios de nomenclatura» del <c>README</c> del núcleo.
/// </remarks>
public partial record MlResult<T>
{

    /// <summary>
    /// Intenta obtener el valor transportado por el resultado.
    /// </summary>
    /// <param name="value">
    /// Cuando este método devuelve <see langword="true"/>, contiene el valor transportado por el
    /// resultado; en caso contrario, contiene el valor predeterminado de
    /// <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> si el resultado es válido y, por tanto,
    /// <paramref name="value"/> contiene el valor transportado; <see langword="false"/> si el
    /// resultado es fallido.
    /// </returns>
    /// <remarks>
    /// Sigue la semántica <c>Try*</c> de la BCL: no lanza nunca y nunca devuelve
    /// <see cref="MlResult{T}"/>. Es la alternativa imperativa a
    /// <c>MlResultMatchExtensions.Match</c> para los casos en que el consumidor prefiere un
    /// <c>if</c> a una función de resolución.
    /// </remarks>
    public bool TryGetValue(out T value)
    {
        value = IsSuccess ? SuccessValue : default!;

        return IsSuccess;
    }

    /// <summary>
    /// Intenta obtener los detalles del error que transporta el resultado.
    /// </summary>
    /// <param name="errorsDetails">
    /// Cuando este método devuelve <see langword="true"/>, contiene los detalles del error; en
    /// caso contrario, contiene <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> si el resultado es fallido y, por tanto,
    /// <paramref name="errorsDetails"/> contiene los detalles del error;
    /// <see langword="false"/> si el resultado es válido.
    /// </returns>
    /// <remarks>
    /// Sigue la semántica <c>Try*</c> de la BCL: no lanza nunca y nunca devuelve
    /// <see cref="MlResult{T}"/>. Es la operación complementaria de
    /// <see cref="TryGetValue(out T)"/>.
    /// </remarks>
    public bool TryGetErrors(out MlErrorDetails errorsDetails)
    {
        errorsDetails = IsFailure ? FailureDetails : null!;

        return IsFailure;
    }

}
