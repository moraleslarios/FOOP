namespace MoralesLarios.OOFP.EFCore;
public static class RegisterServices
{

    private static IServiceCollection? _services;

    internal static IServiceProvider ServiceProvider
    {
        get => _services?.BuildServiceProvider() ?? throw new ArgumentNullException("The fiel Provider is null");
    }

    public static TRepo ResolveRepoFp<TRepo>()
            => ServiceProvider.GetService<TRepo>() 
                    ?? throw new ArgumentException($"{typeof(TRepo).Name} XXX is not registered in the IServiceCollection. It must be registered individually or using one of the AddTransientOOFPRepos<T, TContext>, AddScopedOOFPRepos<T, TContext>, or AddSingletonOOFPRepos<T, TContext> methods.");


    public static IServiceCollection AddTransientOOFPRepos<T, TContext>(this IServiceCollection services)
        where T        : class
        where TContext : DbContext
    {
        _services = services;



        services.AddTransient<IEFRepo                <T>, EFRepo                <T, TContext>>();
        services.AddTransient<IEFRepoPagination      <T>, EFRepoPagination      <T, TContext>>();
        services.AddTransient<IEFRepoAdder           <T>, EFRepoAdder           <T, TContext>>();
        services.AddTransient<IEFRepoDeleter         <T>, EFRepoDeleter         <T, TContext>>();
        services.AddTransient<IEFRepoReader          <T>, EFRepoReader          <T, TContext>>();
        services.AddTransient<IEFRepoReaderPagination<T>, EFRepoReaderPagination<T, TContext>>();
        services.AddTransient<IEFRepoUpdater         <T>, EFRepoUpdater         <T, TContext>>();
        services.AddTransient<IEFRepoWriter          <T>, EFRepoWriter          <T, TContext>>();


        services.AddTransient<IEFRepoFp                <T>, EFRepoFp                 <T, TContext>>();
        services.AddTransient<IEFRepoPaginationFp      <T>, EFRepoPaginationFp       <T, TContext>>();
        services.AddTransient<IEFRepoAdderFp           <T>, EFRepoAdderFp            <T, TContext>>();
        services.AddTransient<IEFRepoDeleterFp         <T>, EFRepoDeleterFp          <T, TContext>>();
        services.AddTransient<IEFRepoReaderFp          <T>, EFRepoReaderFp           <T, TContext>>();
        services.AddTransient<IEFRepoUpdaterFp         <T>, EFRepoUpdaterFp          <T, TContext>>();
        services.AddTransient<IEFRepoWriterFp          <T>, EFRepoWriterFp           <T, TContext>>();
        services.AddTransient<IEFRepoReaderPaginationFp<T>, EFRepoReaderPaginationFp <T, TContext>>();



        return services;
    }


    public static IServiceCollection AddScopedOOFPRepos<T, TContext>(this IServiceCollection services)
        where T        : class
        where TContext : DbContext
    {
        _services = services;



        services.AddScoped<IEFRepo                <T>, EFRepo                <T, TContext>>();
        services.AddScoped<IEFRepoPagination      <T>, EFRepoPagination      <T, TContext>>();
        services.AddScoped<IEFRepoAdder           <T>, EFRepoAdder           <T, TContext>>();
        services.AddScoped<IEFRepoDeleter         <T>, EFRepoDeleter         <T, TContext>>();
        services.AddScoped<IEFRepoReader          <T>, EFRepoReader          <T, TContext>>();
        services.AddScoped<IEFRepoReaderPagination<T>, EFRepoReaderPagination<T, TContext>>();
        services.AddScoped<IEFRepoUpdater         <T>, EFRepoUpdater         <T, TContext>>();
        services.AddScoped<IEFRepoWriter          <T>, EFRepoWriter          <T, TContext>>();


        services.AddScoped<IEFRepoFp                <T>, EFRepoFp                 <T, TContext>>();
        services.AddScoped<IEFRepoPaginationFp      <T>, EFRepoPaginationFp       <T, TContext>>();
        services.AddScoped<IEFRepoAdderFp           <T>, EFRepoAdderFp            <T, TContext>>();
        services.AddScoped<IEFRepoDeleterFp         <T>, EFRepoDeleterFp          <T, TContext>>();
        services.AddScoped<IEFRepoReaderFp          <T>, EFRepoReaderFp           <T, TContext>>();
        services.AddScoped<IEFRepoUpdaterFp         <T>, EFRepoUpdaterFp          <T, TContext>>();
        services.AddScoped<IEFRepoWriterFp          <T>, EFRepoWriterFp           <T, TContext>>();
        services.AddScoped<IEFRepoReaderPaginationFp<T>, EFRepoReaderPaginationFp <T, TContext>>();


        return services;
    }


    public static IServiceCollection AddSingletonOOFPRepos<T, TContext>(this IServiceCollection services)
        where T        : class
        where TContext : DbContext
    {
        _services = services;



        services.AddSingleton<IEFRepo                <T>, EFRepo                <T, TContext>>();
        services.AddSingleton<IEFRepoPagination      <T>, EFRepoPagination      <T, TContext>>();
        services.AddSingleton<IEFRepoAdder           <T>, EFRepoAdder           <T, TContext>>();
        services.AddSingleton<IEFRepoDeleter         <T>, EFRepoDeleter         <T, TContext>>();
        services.AddSingleton<IEFRepoReader          <T>, EFRepoReader          <T, TContext>>();
        services.AddSingleton<IEFRepoReaderPagination<T>, EFRepoReaderPagination<T, TContext>>();
        services.AddSingleton<IEFRepoUpdater         <T>, EFRepoUpdater         <T, TContext>>();
        services.AddSingleton<IEFRepoWriter          <T>, EFRepoWriter          <T, TContext>>();


        services.AddSingleton<IEFRepoFp                <T>, EFRepoFp                 <T, TContext>>();
        services.AddSingleton<IEFRepoPaginationFp      <T>, EFRepoPaginationFp       <T, TContext>>();
        services.AddSingleton<IEFRepoAdderFp           <T>, EFRepoAdderFp            <T, TContext>>();
        services.AddSingleton<IEFRepoDeleterFp         <T>, EFRepoDeleterFp          <T, TContext>>();
        services.AddSingleton<IEFRepoReaderFp          <T>, EFRepoReaderFp           <T, TContext>>();
        services.AddSingleton<IEFRepoUpdaterFp         <T>, EFRepoUpdaterFp          <T, TContext>>();
        services.AddSingleton<IEFRepoWriterFp          <T>, EFRepoWriterFp           <T, TContext>>();
        services.AddSingleton<IEFRepoReaderPaginationFp<T>, EFRepoReaderPaginationFp <T, TContext>>();


        return services;
    }

}
