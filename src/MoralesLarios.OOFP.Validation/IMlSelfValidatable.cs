// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Validation;

/// <summary>
/// Contrato de autovalidacion: el objeto sabe validarse a si mismo y devuelve un <see cref="MlResult{T}"/>.
/// Se usa con el patron autorreferencial <c>class X : IMlSelfValidatable&lt;X&gt;</c>.
/// </summary>
/// <typeparam name="T">El propio tipo que se valida.</typeparam>
public interface IMlSelfValidatable<T>
    where T : class
{

    /// <summary>
    /// Valida el objeto y devuelve <c>this</c> si es valido, o los errores acumulados si no lo es.
    /// </summary>
    MlResult<T> Validate();

}
