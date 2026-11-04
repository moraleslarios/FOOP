// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore;

/// <summary>Opciones para diagnosticar cambios que quedan sin confirmar al cerrar un ámbito.</summary>
public sealed class MlUnitOfWorkDiagnostics
{
    /// <summary>
    /// Obtiene o establece si se emite un aviso al cerrar la unidad de trabajo con cambios pendientes.
    /// El valor predeterminado es <see langword="false"/>.
    /// </summary>
    public bool WarnOnUncommittedChanges { get; set; }
}