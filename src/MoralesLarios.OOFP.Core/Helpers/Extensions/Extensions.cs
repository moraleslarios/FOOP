// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers.Extensions;

/// <summary>
/// Clase de compatibilidad que conserva, en forma de puentes marcados como obsoletos, los
/// métodos que antes vivían aquí y que se han repartido entre
/// <see cref="NullableExtensions"/>, <see cref="ExceptionMetadataExtensions"/>,
/// <see cref="MutationExtensions"/> y <see cref="TaskLiftExtensions"/>.
/// </summary>
/// <remarks>
/// Esta clase ya <b>no define métodos de extensión</b>: la invocación en forma de extensión
/// (<c>valor.ToNullable()</c>, <c>objeto.With(...)</c>, <c>func.ToFuncTask()</c>…) se resuelve
/// contra las clases nuevas sin que el código llamante necesite ningún cambio, porque el espacio
/// de nombres <c>MoralesLarios.OOFP.Helpers.Extensions</c> <b>no ha cambiado</b>.
/// <para>
/// Los puentes viven en el fichero parcial <c>Helpers/Extensions/Extensions.Obsolete.cs</c> y sólo
/// cubren la invocación en forma <b>estática</b> (<c>Extensions.ToNullable(valor)</c>), que sí
/// habría quedado rota. Están marcados como obsoletos con aviso (no error) y
/// <b>desaparecerán en la versión 3.0.0</b>: migre esas llamadas a la clase indicada en cada
/// mensaje, o pásela a la forma de extensión.
/// </para>
/// </remarks>
public static partial class Extensions
{
}
