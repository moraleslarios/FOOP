// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore;
/// <summary>
/// Registro de los repositorios de Entity Framework Core en el contenedor de dependencias.
/// </summary>
/// <remarks>
/// Todos los repositorios de la librería se registran con ciclo de vida <c>Transient</c>. Es el
/// único ciclo de vida admitido: un repositorio no guarda estado propio y el <c>DbContext</c> que
/// recibe lo gestiona el contenedor, así que crear una instancia nueva por resolución es la opción
/// correcta y además la única compatible con un <c>DbContext</c> registrado como <c>Scoped</c>.
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
    /// Registra, con ciclo de vida <c>Transient</c>, los repositorios de la entidad indicada sobre
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

        services.AddTransient<IThrowingEfRepository         <T>, ThrowingEfCrudRepository     <T, TContext>>();
        services.AddTransient<IThrowingEfPagedRepository    <T>, ThrowingEfPagedRepository    <T, TContext>>();
        services.AddTransient<IThrowingEfCreateRepository   <T>, ThrowingEfCreateRepository   <T, TContext>>();
        services.AddTransient<IThrowingEfDeleteRepository   <T>, ThrowingEfDeleteRepository   <T, TContext>>();
        services.AddTransient<IThrowingEfReadRepository     <T>, ThrowingEfReadRepository     <T, TContext>>();
        services.AddTransient<IThrowingEfPagedReadRepository<T>, ThrowingEfPagedReadRepository<T, TContext>>();
        services.AddTransient<IThrowingEfUpdateRepository   <T>, ThrowingEfUpdateRepository   <T, TContext>>();
        services.AddTransient<IThrowingEfWriteRepository    <T>, ThrowingEfWriteRepository    <T, TContext>>();

        services.AddTransient<IEfRepository         <T>, EfCrudRepository     <T, TContext>>();
        services.AddTransient<IEfPagedRepository    <T>, EfPagedRepository    <T, TContext>>();
        services.AddTransient<IEfCreateRepository   <T>, EfCreateRepository   <T, TContext>>();
        services.AddTransient<IEfDeleteRepository   <T>, EfDeleteRepository   <T, TContext>>();
        services.AddTransient<IEfReadRepository     <T>, EfReadRepository     <T, TContext>>();
        services.AddTransient<IEfUpdateRepository   <T>, EfUpdateRepository   <T, TContext>>();
        services.AddTransient<IEfWriteRepository    <T>, EfWriteRepository    <T, TContext>>();
        services.AddTransient<IEfPagedReadRepository<T>, EfPagedReadRepository<T, TContext>>();

        return services;
    }

}
