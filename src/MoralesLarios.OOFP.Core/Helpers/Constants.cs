// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers;
/// <summary>
/// Expone las constantes del nucleo: el mensaje de error por defecto y las claves convencionales de
/// metadatos que utiliza <see cref="MoralesLarios.OOFP.Types.Errors.MlErrorDetails"/>.
/// </summary>
/// <remarks>
/// <para>
/// Todos los miembros son <c>static readonly</c> y no <c>const</c> de forma deliberada (decision D-01
/// de la epica 1 de la version 2.1.1): un <c>const</c> se incrusta en el ensamblado del consumidor en
/// tiempo de compilacion, por lo que una correccion futura del valor no le llegaria hasta que
/// recompilase. Con <c>static readonly</c> el valor se resuelve en ejecucion y el coste es
/// despreciable para claves de diccionario.
/// </para>
/// <para>
/// Esta clase no declara metodos: los constructores de mensajes viven en <see cref="EnsureFpMessages"/>.
/// </para>
/// </remarks>
public static partial class Constants
{
    /// <summary>
    /// Mensaje de error utilizado cuando se construye un <see cref="MoralesLarios.OOFP.Types.Errors.MlError"/>
    /// sin proporcionar ningun mensaje.
    /// </summary>
    public static readonly string DefaultErrorMessage = "Without custom error message. For more info, view the 'Ex' details exceptions.";

    /// <summary>
    /// Clave de metadatos bajo la que se almacena la excepcion capturada por la familia <c>Try*</c>.
    /// </summary>
    public static readonly string ExceptionMetadataKey = "Ex";

    /// <summary>
    /// Clave de metadatos bajo la que se almacena el valor que provoco el error.
    /// </summary>
    public static readonly string ValueMetadataKey = "Value";

    /// <summary>
    /// Clave de metadatos que indica que el elemento buscado no se encontro.
    /// </summary>
    public static readonly string NotFoundKey = "NotFound";

    /// <summary>
    /// Clave de metadatos con el nombre del parametro que incumplio la guardia.
    /// </summary>
    public static readonly string ParameterNameKey = "ParamName";

    /// <summary>
    /// Clave de metadatos con los indices de los elementos que fallaron.
    /// </summary>
    public static readonly string FailedIndexesKey = "FailedIndexes";

    /// <summary>
    /// Clave de metadatos con el valor esperado por la guardia.
    /// </summary>
    public static readonly string ExpectedKey = "Expected";
}

