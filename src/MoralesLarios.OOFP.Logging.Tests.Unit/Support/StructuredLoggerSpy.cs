// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;

namespace MoralesLarios.OOFP.Logging.Tests.Unit.Support;

/// <summary>
/// Traza capturada por <see cref="StructuredLoggerSpy"/>, conservando el <c>state</c> sin aplanar.
/// </summary>
/// <param name="LogLevel">Nivel de severidad con el que se emitió la traza.</param>
/// <param name="EventId">Identificador del evento.</param>
/// <param name="Message">Mensaje ya formateado por el <c>formatter</c> del llamante.</param>
/// <param name="Exception">Excepción asociada a la traza, si la hubiera.</param>
/// <param name="Fields">Campos del <c>state</c>, o <c>null</c> si el <c>state</c> no es una lista de pares nombre/valor.</param>
public record StructuredEntry(LogLevel                                     LogLevel,
                              EventId                                      EventId,
                              string                                       Message,
                              Exception?                                   Exception,
                              IReadOnlyList<KeyValuePair<string, object?>>? Fields)
{
    /// <summary>
    /// Obtiene el valor del campo indicado.
    /// </summary>
    /// <param name="name">Nombre del marcador de la plantilla, sin llaves.</param>
    /// <returns>El valor del campo, o <c>null</c> si no existe.</returns>
    public object? Field(string name)
        => Fields?.Where(x => x.Key == name).Select(x => x.Value).FirstOrDefault();

    /// <summary>
    /// Indica si el <c>state</c> contiene el campo indicado.
    /// </summary>
    /// <param name="name">Nombre del marcador de la plantilla, sin llaves.</param>
    /// <returns><c>true</c> si el campo existe.</returns>
    public bool HasField(string name)
        => Fields is not null && Fields.Any(x => x.Key == name);

    /// <summary>
    /// Plantilla original, tal y como la publica <c>Microsoft.Extensions.Logging</c> bajo la clave
    /// <c>{OriginalFormat}</c>.
    /// </summary>
    public string? OriginalFormat => Field("{OriginalFormat}") as string;

    /// <summary>Nombres de los campos del <c>state</c>, en orden.</summary>
    public IReadOnlyList<string> FieldNames
        => Fields is null ? [] : Fields.Select(x => x.Key).ToList();
}

/// <summary>
/// Doble de <see cref="ILogger"/> que, a diferencia de <see cref="LoggerSpy"/>, conserva el
/// <c>state</c> recibido para poder comprobar que el registro es realmente <b>estructurado</b> y
/// que la plantilla llega <b>sin interpolar</b>.
/// </summary>
/// <remarks>
/// Es la pieza que demuestra la épica 8 de la versión 2.3.0: sin ella nada prueba que los
/// marcadores de la plantilla se publiquen como campos consultables.
/// </remarks>
public sealed class StructuredLoggerSpy : ILogger
{
    private readonly ConcurrentQueue<StructuredEntry> entries = new();

    /// <summary>
    /// Nivel mínimo a partir del cual <see cref="IsEnabled(LogLevel)"/> devuelve <c>true</c>.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Trace;

    /// <summary>
    /// Interruptor general. Cuando es <c>false</c>, <see cref="IsEnabled(LogLevel)"/> devuelve
    /// siempre <c>false</c>.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Número de veces que se ha consultado <see cref="IsEnabled(LogLevel)"/>.</summary>
    public int IsEnabledCallCount { get; private set; }

    /// <summary>Trazas capturadas, en el orden en que se emitieron.</summary>
    public IReadOnlyList<StructuredEntry> Entries => entries.ToArray();

    /// <summary>Última traza capturada.</summary>
    /// <exception cref="InvalidOperationException">Si no se ha capturado ninguna traza.</exception>
    public StructuredEntry Last => Entries.Count > 0
                                       ? Entries[Entries.Count - 1]
                                       : throw new InvalidOperationException("El StructuredLoggerSpy no ha capturado ninguna traza.");

    /// <summary>Elimina todas las trazas capturadas y reinicia los contadores.</summary>
    public void Clear()
    {
        while (entries.TryDequeue(out _)) { }

        IsEnabledCallCount = 0;
    }

    /// <inheritdoc/>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
    {
        IsEnabledCallCount++;

        return Enabled && logLevel != LogLevel.None && logLevel >= MinimumLevel;
    }

    /// <inheritdoc/>
    public void Log<TState>(LogLevel                         logLevel,
                            EventId                          eventId,
                            TState                           state,
                            Exception?                       exception,
                            Func<TState, Exception?, string> formatter)
        => entries.Enqueue(new StructuredEntry(logLevel,
                                               eventId,
                                               formatter(state, exception),
                                               exception,
                                               state as IReadOnlyList<KeyValuePair<string, object?>>));

    private sealed class NullScope : IDisposable
    {
        internal static readonly NullScope Instance = new();

        private NullScope() { }

        public void Dispose() { }
    }
}
