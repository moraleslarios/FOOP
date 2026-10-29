// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;


/// <summary>
/// Mensaje de error expresado como tipo propio, para no manejar cadenas de texto desnudas.
/// </summary>
/// <param name="Message">Valor de <c>Message</c>.</param>
public record ErrorMessage(string Message);
