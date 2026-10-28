// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore;
/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
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
    /// <typeparam name="TRepo">Tipo genérico TRepo.</typeparam>
    /// <returns>Valor de tipo <c>TRepo</c> obtenido por la operación.</returns>
    public static TRepo GetRepository<TRepo>()
            => ServiceProvider.GetService<TRepo>() 
                    ?? throw new ArgumentException($"{typeof(TRepo).Name} is not registered in the IServiceCollection. It must be registered individually or using one of the AddOofpRepositoriesTransient<T, TContext>, AddOofpRepositories<T, TContext>, or AddOofpRepositoriesSingleton<T, TContext> methods.");


    /// <summary>
    /// Añade el elemento indicado de <c>OofpRepositoriesTransient</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddOofpRepositoriesTransient<T, TContext>(this IServiceCollection services)
        where T        : class
        where TContext : DbContext
    {
        _services = services;



        services.AddTransient<IThrowingEfRepository                <T>, ThrowingEfCrudRepository                <T, TContext>>();
        services.AddTransient<IThrowingEfPagedRepository      <T>, ThrowingEfPagedRepository      <T, TContext>>();
        services.AddTransient<IThrowingEfCreateRepository           <T>, ThrowingEfCreateRepository           <T, TContext>>();
        services.AddTransient<IThrowingEfDeleteRepository         <T>, ThrowingEfDeleteRepository         <T, TContext>>();
        services.AddTransient<IThrowingEfReadRepository          <T>, ThrowingEfReadRepository          <T, TContext>>();
        services.AddTransient<IThrowingEfPagedReadRepository<T>, ThrowingEfPagedReadRepository<T, TContext>>();
        services.AddTransient<IThrowingEfUpdateRepository         <T>, ThrowingEfUpdateRepository         <T, TContext>>();
        services.AddTransient<IThrowingEfWriteRepository          <T>, ThrowingEfWriteRepository          <T, TContext>>();


        services.AddTransient<IEfRepository                <T>, EfCrudRepository                 <T, TContext>>();
        services.AddTransient<IEfPagedRepository      <T>, EfPagedRepository       <T, TContext>>();
        services.AddTransient<IEfCreateRepository           <T>, EfCreateRepository            <T, TContext>>();
        services.AddTransient<IEfDeleteRepository         <T>, EfDeleteRepository          <T, TContext>>();
        services.AddTransient<IEfReadRepository          <T>, EfReadRepository           <T, TContext>>();
        services.AddTransient<IEfUpdateRepository         <T>, EfUpdateRepository          <T, TContext>>();
        services.AddTransient<IEfWriteRepository          <T>, EfWriteRepository           <T, TContext>>();
        services.AddTransient<IEfPagedReadRepository<T>, EfPagedReadRepository <T, TContext>>();



        return services;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>OofpRepositories</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddOofpRepositories<T, TContext>(this IServiceCollection services)
        where T        : class
        where TContext : DbContext
    {
        _services = services;



        services.AddScoped<IThrowingEfRepository                <T>, ThrowingEfCrudRepository                <T, TContext>>();
        services.AddScoped<IThrowingEfPagedRepository      <T>, ThrowingEfPagedRepository      <T, TContext>>();
        services.AddScoped<IThrowingEfCreateRepository           <T>, ThrowingEfCreateRepository           <T, TContext>>();
        services.AddScoped<IThrowingEfDeleteRepository         <T>, ThrowingEfDeleteRepository         <T, TContext>>();
        services.AddScoped<IThrowingEfReadRepository          <T>, ThrowingEfReadRepository          <T, TContext>>();
        services.AddScoped<IThrowingEfPagedReadRepository<T>, ThrowingEfPagedReadRepository<T, TContext>>();
        services.AddScoped<IThrowingEfUpdateRepository         <T>, ThrowingEfUpdateRepository         <T, TContext>>();
        services.AddScoped<IThrowingEfWriteRepository          <T>, ThrowingEfWriteRepository          <T, TContext>>();


        services.AddScoped<IEfRepository                <T>, EfCrudRepository                 <T, TContext>>();
        services.AddScoped<IEfPagedRepository      <T>, EfPagedRepository       <T, TContext>>();
        services.AddScoped<IEfCreateRepository           <T>, EfCreateRepository            <T, TContext>>();
        services.AddScoped<IEfDeleteRepository         <T>, EfDeleteRepository          <T, TContext>>();
        services.AddScoped<IEfReadRepository          <T>, EfReadRepository           <T, TContext>>();
        services.AddScoped<IEfUpdateRepository         <T>, EfUpdateRepository          <T, TContext>>();
        services.AddScoped<IEfWriteRepository          <T>, EfWriteRepository           <T, TContext>>();
        services.AddScoped<IEfPagedReadRepository<T>, EfPagedReadRepository <T, TContext>>();


        return services;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>OofpRepositoriesSingleton</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddOofpRepositoriesSingleton<T, TContext>(this IServiceCollection services)
        where T        : class
        where TContext : DbContext
    {
        _services = services;



        services.AddSingleton<IThrowingEfRepository                <T>, ThrowingEfCrudRepository                <T, TContext>>();
        services.AddSingleton<IThrowingEfPagedRepository      <T>, ThrowingEfPagedRepository      <T, TContext>>();
        services.AddSingleton<IThrowingEfCreateRepository           <T>, ThrowingEfCreateRepository           <T, TContext>>();
        services.AddSingleton<IThrowingEfDeleteRepository         <T>, ThrowingEfDeleteRepository         <T, TContext>>();
        services.AddSingleton<IThrowingEfReadRepository          <T>, ThrowingEfReadRepository          <T, TContext>>();
        services.AddSingleton<IThrowingEfPagedReadRepository<T>, ThrowingEfPagedReadRepository<T, TContext>>();
        services.AddSingleton<IThrowingEfUpdateRepository         <T>, ThrowingEfUpdateRepository         <T, TContext>>();
        services.AddSingleton<IThrowingEfWriteRepository          <T>, ThrowingEfWriteRepository          <T, TContext>>();


        services.AddSingleton<IEfRepository                <T>, EfCrudRepository                 <T, TContext>>();
        services.AddSingleton<IEfPagedRepository      <T>, EfPagedRepository       <T, TContext>>();
        services.AddSingleton<IEfCreateRepository           <T>, EfCreateRepository            <T, TContext>>();
        services.AddSingleton<IEfDeleteRepository         <T>, EfDeleteRepository          <T, TContext>>();
        services.AddSingleton<IEfReadRepository          <T>, EfReadRepository           <T, TContext>>();
        services.AddSingleton<IEfUpdateRepository         <T>, EfUpdateRepository          <T, TContext>>();
        services.AddSingleton<IEfWriteRepository          <T>, EfWriteRepository           <T, TContext>>();
        services.AddSingleton<IEfPagedReadRepository<T>, EfPagedReadRepository <T, TContext>>();


        return services;
    }

}

