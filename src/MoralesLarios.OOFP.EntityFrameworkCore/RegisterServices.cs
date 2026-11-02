// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

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

    private static IServiceCollection? _services;

    internal static IServiceProvider ServiceProvider
    {
        get => _services?.BuildServiceProvider() ?? throw new ArgumentNullException("The field Provider is null");
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Repository</c>.
    /// </summary>
    /// <typeparam name="TRepo">Tipo del repositorio que se desea resolver.</typeparam>
    /// <returns>Valor de tipo <c>TRepo</c> obtenido por la operación.</returns>
    public static TRepo GetRepository<TRepo>()
            => ServiceProvider.GetService<TRepo>()
                    ?? throw new ArgumentException($"{typeof(TRepo).Name} is not registered in the IServiceCollection. It must be registered individually or using the AddOofpRepositories<T, TContext> method.");


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
        _services = services;

        services.AddScoped<IThrowingEfRepository         <T>, ThrowingEfCrudRepository     <T, TContext>>();
        services.AddScoped<IThrowingEfPagedRepository    <T>, ThrowingEfPagedRepository    <T, TContext>>();
        services.AddScoped<IThrowingEfCreateRepository   <T>, ThrowingEfCreateRepository   <T, TContext>>();
        services.AddScoped<IThrowingEfDeleteRepository   <T>, ThrowingEfDeleteRepository   <T, TContext>>();
        services.AddScoped<IThrowingEfReadRepository     <T>, ThrowingEfReadRepository     <T, TContext>>();
        services.AddScoped<IThrowingEfPagedReadRepository<T>, ThrowingEfPagedReadRepository<T, TContext>>();
        services.AddScoped<IThrowingEfUpdateRepository   <T>, ThrowingEfUpdateRepository   <T, TContext>>();
        services.AddScoped<IThrowingEfWriteRepository    <T>, ThrowingEfWriteRepository    <T, TContext>>();

        services.AddScoped<IEfRepository         <T>, EfCrudRepository     <T, TContext>>();
        services.AddScoped<IEfPagedRepository    <T>, EfPagedRepository    <T, TContext>>();
        services.AddScoped<IEfCreateRepository   <T>, EfCreateRepository   <T, TContext>>();
        services.AddScoped<IEfDeleteRepository   <T>, EfDeleteRepository   <T, TContext>>();
        services.AddScoped<IEfReadRepository     <T>, EfReadRepository     <T, TContext>>();
        services.AddScoped<IEfUpdateRepository   <T>, EfUpdateRepository   <T, TContext>>();
        services.AddScoped<IEfWriteRepository    <T>, EfWriteRepository    <T, TContext>>();
        services.AddScoped<IEfPagedReadRepository<T>, EfPagedReadRepository<T, TContext>>();

        return services;
    }

}
