// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;

namespace MoralesLarios.OOFP.Logging.Tests.Unit.Support;

/// <summary>
/// Traza capturada por <see cref="StructuredLoggerSpy"/>, conservando el <c>state</c> sin aplanar.
/// </summary>
/// <param name="LogLevel">Nivel de severidad con el que se emitió la traza.</param>
/// <param name="EventId">Identificador del evento.</param>
/// <param name="Message">Mensaje ya formateado por el <c>formatter</c> del llamante.</param>
/// <param name="Exception">Excepción asociada a la traza, si la hubiera.</param>
/// <param name="Fields">Campos del <c>state</c>, o <c>null</c> si el <c>state</c> no es una lista de pares nombre/valor.</param>
/// <param name="Scopes">Ámbitos abiertos en el instante en que se emitió la traza, del más externo al más interno.</param>
public record StructuredEntry(LogLevel                                     LogLevel,
                              EventId                                      EventId,
                              string                                       Message,
                              Exception?                                   Exception,
                              IReadOnlyList<KeyValuePair<string, object?>>? Fields,
                              IReadOnlyList<LoggerScopeRecord>              Scopes)
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

    /// <summary>
    /// Indica si la traza se emitió dentro de algún ámbito.
    /// </summary>
    public bool IsInsideScope => Scopes.Count > 0;

    /// <summary>
    /// Obtiene el valor de una propiedad buscándola en los ámbitos abiertos, del más interno al más
    /// externo, que es el orden en que un <i>sink</i> real resuelve un nombre repetido.
    /// </summary>
    /// <param name="name">Nombre de la propiedad, sin llaves.</param>
    /// <returns>El valor de la propiedad, o <c>null</c> si ningún ámbito abierto la lleva.</returns>
    public object? ScopeProperty(string name)
        => Scopes.Reverse()
                 .Where(x => x.HasProperty(name))
                 .Select(x => x.Property(name))
                 .FirstOrDefault();

    /// <summary>
    /// Indica si alguno de los ámbitos abiertos en el instante de la traza lleva la propiedad
    /// indicada.
    /// </summary>
    /// <param name="name">Nombre de la propiedad, sin llaves.</param>
    /// <returns><c>true</c> si algún ámbito abierto la lleva.</returns>
    public bool HasScopeProperty(string name) => Scopes.Any(x => x.HasProperty(name));
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
    private readonly ConcurrentQueue<StructuredEntry>   entries = new();
    private readonly ConcurrentQueue<LoggerScopeRecord> scopes  = new();

    private readonly AsyncLocal<ImmutableStack<LoggerScopeRecord>> active = new();

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

    /// <summary>
    /// Todos los ámbitos abiertos durante la vida del espía, en el orden en que se abrieron, estén
    /// cerrados o no.
    /// </summary>
    public IReadOnlyList<LoggerScopeRecord> Scopes => scopes.ToArray();

    /// <summary>Último ámbito abierto.</summary>
    /// <exception cref="InvalidOperationException">Si no se ha abierto ningún ámbito.</exception>
    public LoggerScopeRecord LastScope => Scopes.Count > 0
                                             ? Scopes[Scopes.Count - 1]
                                             : throw new InvalidOperationException("El StructuredLoggerSpy no ha abierto ningún ámbito.");

    /// <summary>
    /// Ámbitos que se abrieron y <b>no</b> se han cerrado. Un ámbito filtrado contamina todas las
    /// trazas posteriores del flujo, así que esta lista debería quedar vacía al final de una prueba.
    /// </summary>
    public IReadOnlyList<LoggerScopeRecord> LeakedScopes => Scopes.Where(x => !x.IsClosed).ToList();

    /// <summary>Número de ámbitos abiertos en este instante en el flujo de ejecución actual.</summary>
    public int ActiveScopeCount => (active.Value ?? ImmutableStack<LoggerScopeRecord>.Empty).Count();

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
        while (scopes .TryDequeue(out _)) { }

        active.Value       = ImmutableStack<LoggerScopeRecord>.Empty;
        IsEnabledCallCount = 0;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// A diferencia de <c>LoggerSpy</c>, aquí el ámbito <b>no</b> es un <c>NullScope</c>: se
    /// registra con sus propiedades y se apila en un <see cref="AsyncLocal{T}"/>, que es el mismo
    /// mecanismo que usan los proveedores reales, para que cada traza sepa dentro de qué ámbitos
    /// cayó.
    /// </remarks>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        var pila   = active.Value ?? ImmutableStack<LoggerScopeRecord>.Empty;
        var record = new LoggerScopeRecord(state,
                                           state as IReadOnlyList<KeyValuePair<string, object?>>,
                                           pila.Count(),
                                           Stopwatch.GetTimestamp());

        scopes.Enqueue(record);
        active.Value = pila.Push(record);

        return new Scope(this, record);
    }

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
                                               state as IReadOnlyList<KeyValuePair<string, object?>>,
                                               (active.Value ?? ImmutableStack<LoggerScopeRecord>.Empty).Reverse().ToList()));

    private sealed class Scope(StructuredLoggerSpy owner, LoggerScopeRecord record) : IDisposable
    {
        public void Dispose()
        {
            record.DisposeCallCount++;

            if (record.ClosedAt is null) record.ClosedAt = Stopwatch.GetTimestamp();

            var pila = owner.active.Value ?? ImmutableStack<LoggerScopeRecord>.Empty;

            if (! pila.IsEmpty && ReferenceEquals(pila.Peek(), record)) owner.active.Value = pila.Pop();
            else owner.active.Value = ImmutableStack.CreateRange(pila.Where(x => ! ReferenceEquals(x, record)).Reverse());
        }
    }
}
