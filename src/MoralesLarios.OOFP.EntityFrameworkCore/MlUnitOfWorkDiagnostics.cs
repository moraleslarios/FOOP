// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore;

/// <summary>Options for diagnosing uncommitted changes when a scope is disposed.</summary>
public sealed class MlUnitOfWorkDiagnostics
{
    /// <summary>
    /// Gets or sets whether to log a warning when the unit of work is disposed with pending changes.
    /// The default value is <see langword="false"/>.
    /// </summary>
    public bool WarnOnUncommittedChanges { get; set; }
}