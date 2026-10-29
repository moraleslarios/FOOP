// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace MoralesLarios.OOFP.Types.Errors;


/// <summary>
/// Mensaje de error expresado como tipo propio, para no manejar cadenas de texto desnudas.
/// </summary>
/// <remarks>
/// Tipo heredado sin cometido propio en el modelo de error. Use <see cref="MlError"/> en su lugar.
/// </remarks>
/// <param name="Message">Texto descriptivo del error.</param>
[Obsolete("Use MlError instead. Will be removed in 3.0.0.", false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public record ErrorMessage(string Message);
