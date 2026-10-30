// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;

namespace MoralesLarios.OOFP.Logging.Tests.Unit.Support;

/// <summary>
/// Traza capturada por <see cref="LoggerSpy"/>.
/// </summary>
/// <param name="LogLevel">Nivel de severidad con el que se emitió la traza.</param>
/// <param name="EventId">Identificador del evento.</param>
/// <param name="Message">Mensaje ya formateado por el <c>formatter</c> del llamante.</param>
/// <param name="Exception">Excepción asociada a la traza, si la hubiera.</param>
public record LoggedEntry(LogLevel LogLevel, EventId EventId, string Message, Exception? Exception);

/// <summary>
/// Doble de <see cref="ILogger"/> escrito a mano que captura en memoria el nivel, el mensaje ya
/// formateado, el <see cref="EventId"/> y la excepción de cada traza emitida.
/// </summary>
/// <remarks>
/// Permite además controlar la respuesta de <see cref="IsEnabled(LogLevel)"/> mediante
/// <see cref="MinimumLevel"/> y <see cref="Enabled"/>, para poder verificar en épicas posteriores
/// que la construcción del mensaje se evita cuando el nivel está deshabilitado.
/// </remarks>
public sealed class LoggerSpy : ILogger
{
    private readonly ConcurrentQueue<LoggedEntry> entries = new();

    /// <summary>
    /// Nivel mínimo a partir del cual <see cref="IsEnabled(LogLevel)"/> devuelve <c>true</c>.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Trace;

    /// <summary>
    /// Interruptor general. Cuando es <c>false</c>, <see cref="IsEnabled(LogLevel)"/> devuelve
    /// siempre <c>false</c>, independientemente de <see cref="MinimumLevel"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Número de veces que se ha consultado <see cref="IsEnabled(LogLevel)"/>.
    /// </summary>
    public int IsEnabledCallCount { get; private set; }

    /// <summary>
    /// Trazas capturadas, en el orden en que se emitieron.
    /// </summary>
    public IReadOnlyList<LoggedEntry> Entries => entries.ToArray();

    /// <summary>
    /// Última traza capturada.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si no se ha capturado ninguna traza.</exception>
    public LoggedEntry Last => Entries.Count > 0
                                   ? Entries[Entries.Count - 1]
                                   : throw new InvalidOperationException("El LoggerSpy no ha capturado ninguna traza.");

    /// <summary>
    /// Mensajes capturados, en el orden en que se emitieron.
    /// </summary>
    public IReadOnlyList<string> Messages => Entries.Select(x => x.Message).ToList();

    /// <summary>
    /// Elimina todas las trazas capturadas y reinicia los contadores.
    /// </summary>
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
        => entries.Enqueue(new LoggedEntry(logLevel, eventId, formatter(state, exception), exception));

    private sealed class NullScope : IDisposable
    {
        internal static readonly NullScope Instance = new();

        private NullScope() { }

        public void Dispose() { }
    }
}
