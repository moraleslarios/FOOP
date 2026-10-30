// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers.Extensions;

/// <summary>
/// Métodos de extensión que elevan un tipo por valor a su equivalente anulable
/// (<c>T</c> → <c>T?</c>), para poder representar la ausencia de valor sin recurrir a valores
/// centinela.
/// </summary>
/// <remarks>
/// Esta clase forma parte del troceado de la antigua clase <c>Extensions</c>, que acumulaba
/// familias de métodos sin relación entre sí. El espacio de nombres
/// <c>MoralesLarios.OOFP.Helpers.Extensions</c> <b>no cambia</b>, de modo que la invocación en
/// forma de extensión (<c>valor.ToNullable()</c>) sigue resolviéndose igual: el compilador busca
/// los métodos de extensión en <b>todas</b> las clases estáticas del espacio de nombres
/// importado, no en una clase concreta.
/// <para>
/// Sólo la invocación en forma <b>estática</b> (<c>Extensions.ToNullable(valor)</c>) queda
/// afectada; para ella se conservan puentes <c>[Obsolete]</c> en
/// <c>Helpers/Extensions/Extensions.Obsolete.cs</c>, que desaparecerán en la versión 3.0.0.
/// </para>
/// </remarks>
public static class NullableExtensions
{

    /// <summary>
    /// Eleva un valor de un tipo por valor a su equivalente anulable.
    /// </summary>
    /// <typeparam name="T">Tipo por valor del valor de origen.</typeparam>
    /// <param name="source">Valor de origen sobre el que se aplica la operación.</param>
    /// <returns>El mismo valor, expuesto como <c>T?</c>.</returns>
    /// <remarks>
    /// La conversión es implícita en C#; este método existe para poder encadenarla con el resto
    /// de combinadores en estilo fluido.
    /// </remarks>
    public static T? ToNullable<T>(this T source)
        where T : struct
            => source;

}
