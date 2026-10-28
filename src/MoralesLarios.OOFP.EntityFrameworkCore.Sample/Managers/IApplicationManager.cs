// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Sample.Managers;

/// <summary>
/// Contrato de ejemplo de un gestor de aplicación que media entre la entidad de persistencia
/// <typeparamref name="TEntity"/> y el DTO <typeparamref name="TDto"/> expuesto al consumidor.
/// </summary>
/// <remarks>
/// Forma parte del proyecto de ejemplo de <c>MoralesLarios.OOFP.EntityFrameworkCore</c>; no se
/// distribuye como paquete y su única finalidad es ilustrar el uso de la librería.
/// </remarks>
/// <typeparam name="TEntity">Tipo de la entidad de persistencia.</typeparam>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos expuesto.</typeparam>
public interface IApplicationManager<TEntity, TDto>
    where TEntity : class
    where TDto    : class
{
}
