// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MoralesLarios.OOFP.EntityFrameworkCore;
/// <summary>
/// Registro de los repositorios de Entity Framework Core en el contenedor de dependencias.
/// </summary>
/// <remarks>
/// Todos los repositorios de la librería se registran con ciclo de vida <c>Scoped</c>. Es el único
/// ciclo de vida admitido, porque un repositorio vive <em>al lado</em> del <c>DbContext</c> que
/// recibe por constructor y ese contexto es <c>Scoped</c>: alinear los dos hace que todos los
/// repositorios de un mismo ámbito compartan contexto y unidad de trabajo, y que su vida termine
/// exactamente cuando termina la del contexto.
/// </remarks>
public static partial class RegisterServices
{

    /// <summary>
    /// Registra, con ciclo de vida <c>Scoped</c>, los repositorios de la entidad indicada sobre
    /// el contexto indicado.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad sobre la que operan los repositorios.</typeparam>
    /// <typeparam name="TContext">Tipo del <c>DbContext</c> que da acceso a la entidad.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>La misma colección de servicios, para permitir el encadenado.</returns>
    public static IServiceCollection AddOofpRepositories<T, TContext>(this IServiceCollection services)
        where T        : class
        where TContext : DbContext
    {
        services.AddOptions<MlUnitOfWorkDiagnostics>();
        services.AddScoped<IMlUnitOfWork<TContext>>(serviceProvider =>
            new MlUnitOfWork<TContext>(serviceProvider.GetRequiredService<TContext>(),
                                       serviceProvider.GetRequiredService<ILogger<MlUnitOfWork<TContext>>>(),
                                       serviceProvider.GetRequiredService<IOptions<MlUnitOfWorkDiagnostics>>()));
        services.AddScoped<IMlUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<IMlUnitOfWork<TContext>>());

        services.AddScoped<IEfRepository<T>>(serviceProvider =>
            new EfCrudRepository<T, TContext>(serviceProvider.GetRequiredService<TContext>(),
                                              serviceProvider.GetRequiredService<IMlUnitOfWork<TContext>>()));
        services.AddScoped<IEfPagedRepository<T>>(serviceProvider =>
            new EfPagedRepository<T, TContext>(serviceProvider.GetRequiredService<TContext>(),
                                               serviceProvider.GetRequiredService<IMlUnitOfWork<TContext>>()));
        services.AddScoped<IEfCreateRepository   <T>, EfCreateRepository   <T, TContext>>();
        services.AddScoped<IEfDeleteRepository   <T>, EfDeleteRepository   <T, TContext>>();
        services.AddScoped<IEfReadRepository     <T>, EfReadRepository     <T, TContext>>();
        services.AddScoped<IEfUpdateRepository   <T>, EfUpdateRepository   <T, TContext>>();
        services.AddScoped<IEfWriteRepository    <T>, EfWriteRepository    <T, TContext>>();
        services.AddScoped<IEfPagedReadRepository<T>, EfPagedReadRepository<T, TContext>>();

        return services;
    }

}
