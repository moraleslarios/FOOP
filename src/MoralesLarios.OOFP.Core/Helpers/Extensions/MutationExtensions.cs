// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers.Extensions;

/// <summary>
/// Métodos de extensión que aplican una secuencia de mutaciones sobre un objeto de referencia y
/// lo devuelven, para poder configurar instancias dentro de una expresión en estilo fluido
/// (familia <c>With</c>).
/// </summary>
/// <remarks>
/// Esta clase forma parte del troceado de la antigua clase <c>Extensions</c>, que acumulaba
/// familias de métodos sin relación entre sí. El espacio de nombres
/// <c>MoralesLarios.OOFP.Helpers.Extensions</c> <b>no cambia</b>, de modo que la invocación en
/// forma de extensión (<c>objeto.With(...)</c>) sigue resolviéndose igual: el compilador busca
/// los métodos de extensión en <b>todas</b> las clases estáticas del espacio de nombres
/// importado, no en una clase concreta.
/// <para>
/// Sólo la invocación en forma <b>estática</b> (<c>Extensions.With(objeto, ...)</c>) queda
/// afectada; para ella se conservan puentes <c>[Obsolete]</c> en
/// <c>Helpers/Extensions/Extensions.Obsolete.cs</c>, que desaparecerán en la versión 3.0.0.
/// </para>
/// <para>
/// <b>Importante</b>: estos métodos <b>mutan la instancia recibida</b> y devuelven esa misma
/// referencia; no crean una copia. El nombre <c>With</c> es habitual en escenarios de copia con
/// cambios, pero aquí la semántica es de mutación, y se mantiene así deliberadamente para no
/// alterar en silencio el comportamiento del código existente.
/// </para>
/// </remarks>
public static class MutationExtensions
{

    /// <summary>
    /// Aplica sobre el objeto de origen todas las mutaciones indicadas y lo devuelve.
    /// </summary>
    /// <typeparam name="T">Tipo por referencia del objeto de origen.</typeparam>
    /// <param name="source">Objeto de origen sobre el que se aplican las mutaciones.</param>
    /// <param name="changes">Mutaciones a aplicar, en el orden en que se indican.</param>
    /// <returns>La <b>misma</b> instancia recibida, ya mutada.</returns>
    /// <remarks>
    /// El objeto de origen se modifica; no se devuelve una copia.
    /// </remarks>
    public static T With<T>(this T source, params Action<T>[] changes)
        where T : class
    {
        foreach (var change in changes)
        {
            change(source);
        }

        return source;
    }


    /// <summary>
    /// Aplica sobre el objeto de origen todas las mutaciones indicadas y lo devuelve. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo por referencia del objeto de origen.</typeparam>
    /// <param name="source">Objeto de origen sobre el que se aplican las mutaciones.</param>
    /// <param name="changes">Mutaciones a aplicar, en el orden en que se indican.</param>
    /// <returns>
    /// Tarea que produce la <b>misma</b> instancia recibida, ya mutada.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<T> WithAsync<T>(this T source, params Action<T>[] changes)
        where T : class
            => source.With(changes).ToAsync();


    /// <summary>
    /// Aplica sobre el objeto producido por la tarea de origen todas las mutaciones indicadas y lo devuelve. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo por referencia del objeto de origen.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el objeto de origen.</param>
    /// <param name="changes">Mutaciones a aplicar, en el orden en que se indican.</param>
    /// <returns>Tarea que produce la <b>misma</b> instancia producida por <paramref name="sourceAsync"/>, ya mutada.</returns>
    public static async Task<T> WithAsync<T>(this Task<T> sourceAsync, params Action<T>[] changes)
        where T : class
            => await (await sourceAsync).WithAsync(changes);

}
