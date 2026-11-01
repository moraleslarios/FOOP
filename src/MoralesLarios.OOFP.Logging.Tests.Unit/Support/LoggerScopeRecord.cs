// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit.Support;

/// <summary>
/// Ámbito de registro capturado por <see cref="StructuredLoggerSpy"/>, con sus propiedades, su
/// profundidad de anidamiento y los instantes de apertura y de cierre.
/// </summary>
/// <remarks>
/// <para>
/// Es la pieza que permite afirmar <b>tres</b> cosas que antes no se podían comprobar: qué
/// propiedades lleva un ámbito, si una traza concreta cayó <b>dentro</b> de él, y si se cerró.
/// </para>
/// <para>
/// El cierre se expone a propósito: un ámbito que no se cierra contamina todas las trazas
/// posteriores del mismo flujo de ejecución, y ése es exactamente el fallo que hay que poder
/// detectar en una prueba.
/// </para>
/// </remarks>
public sealed class LoggerScopeRecord
{
    internal LoggerScopeRecord(object?                                      state,
                               IReadOnlyList<KeyValuePair<string, object?>>? properties,
                               int                                          depth,
                               long                                         openedAt)
    {
        State      = state;
        Properties = properties;
        Depth      = depth;
        OpenedAt   = openedAt;
    }

    /// <summary>Objeto de estado tal y como lo recibió <c>BeginScope</c>, sin aplanar.</summary>
    public object? State { get; }

    /// <summary>
    /// Propiedades del ámbito, o <c>null</c> si el estado no es una lista de pares nombre/valor.
    /// </summary>
    public IReadOnlyList<KeyValuePair<string, object?>>? Properties { get; }

    /// <summary>
    /// Profundidad de anidamiento en el momento de abrirse. El ámbito más externo es el <c>0</c>.
    /// </summary>
    public int Depth { get; }

    /// <summary>Marca de tiempo de apertura, en pulsos de <see cref="System.Diagnostics.Stopwatch"/>.</summary>
    public long OpenedAt { get; }

    /// <summary>
    /// Marca de tiempo de cierre, o <c>null</c> si el ámbito sigue abierto.
    /// </summary>
    public long? ClosedAt { get; internal set; }

    /// <summary>Indica si el ámbito se ha cerrado.</summary>
    public bool IsClosed => ClosedAt is not null;

    /// <summary>Número de veces que se ha llamado a <c>Dispose</c> sobre este ámbito.</summary>
    public int DisposeCallCount { get; internal set; }

    /// <summary>
    /// Plantilla original del ámbito, si su estado la publica bajo la clave <c>{OriginalFormat}</c>.
    /// </summary>
    public string? OriginalFormat => Property("{OriginalFormat}") as string;

    /// <summary>Nombres de las propiedades del ámbito, en orden.</summary>
    public IReadOnlyList<string> PropertyNames
        => Properties is null ? [] : Properties.Select(x => x.Key).ToList();

    /// <summary>Obtiene el valor de la propiedad indicada.</summary>
    /// <param name="name">Nombre de la propiedad, sin llaves.</param>
    /// <returns>El valor de la propiedad, o <c>null</c> si no existe.</returns>
    public object? Property(string name)
        => Properties?.Where(x => x.Key == name).Select(x => x.Value).FirstOrDefault();

    /// <summary>Indica si el ámbito contiene la propiedad indicada.</summary>
    /// <param name="name">Nombre de la propiedad, sin llaves.</param>
    /// <returns><c>true</c> si la propiedad existe.</returns>
    public bool HasProperty(string name)
        => Properties is not null && Properties.Any(x => x.Key == name);

    /// <summary>Representación legible, pensada para los mensajes de fallo de las pruebas.</summary>
    /// <returns>El estado del ámbito, su profundidad y si está cerrado.</returns>
    public override string ToString()
        => $"[profundidad {Depth}] {State} · {(IsClosed ? "cerrado" : "ABIERTO")}";
}
