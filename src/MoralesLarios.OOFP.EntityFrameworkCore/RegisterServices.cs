// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MoralesLarios.OOFP.EntityFrameworkCore;
/// <summary>
/// Registers Entity Framework Core repositories in a dependency-injection container.
/// </summary>
/// <remarks>
/// All repositories are registered with the <see cref="ServiceLifetime.Scoped"/> lifetime so that
/// repositories in the same scope share their <see cref="DbContext"/> and unit of work.
/// </remarks>
public static partial class RegisterServices
{

    /// <summary>
    /// Registers the repositories for an entity and its EF Core context with a scoped lifetime.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TContext">The <see cref="DbContext"/> type used to access the entity.</typeparam>
    /// <param name="services">The service collection to add the repository registrations to.</param>
    /// <returns>The same service collection, to allow chained registrations.</returns>
    public static IServiceCollection AddOofpRepositories<TEntity, TContext>(this IServiceCollection services)
        where TEntity        : class
        where TContext : DbContext
    {
        services.AddOptions<MlUnitOfWorkDiagnostics>();
        services.AddScoped<IMlUnitOfWork<TContext>>(serviceProvider =>
            new MlUnitOfWork<TContext>(serviceProvider.GetRequiredService<TContext>(),
                                       serviceProvider.GetRequiredService<ILogger<MlUnitOfWork<TContext>>>(),
                                       serviceProvider.GetRequiredService<IOptions<MlUnitOfWorkDiagnostics>>()));
        services.AddScoped<IMlUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<IMlUnitOfWork<TContext>>());

        services.AddScoped<IEfRepository<TEntity>>(serviceProvider =>
            new EfCrudRepository<TEntity, TContext>(serviceProvider.GetRequiredService<TContext>(),
                                              serviceProvider.GetRequiredService<IMlUnitOfWork<TContext>>()));
        services.AddScoped<IEfPagedRepository<TEntity>>(serviceProvider =>
            new EfPagedRepository<TEntity, TContext>(serviceProvider.GetRequiredService<TContext>(),
                                               serviceProvider.GetRequiredService<IMlUnitOfWork<TContext>>()));
        services.AddScoped<IEfCreateRepository   <TEntity>, EfCreateRepository   <TEntity, TContext>>();
        services.AddScoped<IEfDeleteRepository   <TEntity>, EfDeleteRepository   <TEntity, TContext>>();
        services.AddScoped<IEfReadRepository     <TEntity>, EfReadRepository     <TEntity, TContext>>();
        services.AddScoped<IEfOrderedReadRepository<TEntity>, EfReadRepository<TEntity, TContext>>();
        services.AddScoped<IEfUpdateRepository   <TEntity>, EfUpdateRepository   <TEntity, TContext>>();
        services.AddScoped<IEfWriteRepository    <TEntity>, EfWriteRepository    <TEntity, TContext>>();
        services.AddScoped<IEfPagedReadRepository<TEntity>, EfPagedReadRepository<TEntity, TContext>>();

        return services;
    }

}
