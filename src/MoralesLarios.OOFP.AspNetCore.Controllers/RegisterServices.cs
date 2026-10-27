using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.AspNetCore.Controllers;

public static partial class RegisterServices
{

    /// <summary>
    /// No realiza ningun registro: los controladores de esta libreria no necesitan servicios propios.
    /// Se mantiene por compatibilidad y se eliminara en la version 3.0.0.
    /// </summary>
    [global::System.Obsolete("No-op: this library registers no services of its own. Remove the call. Will be removed in 3.0.0.", false)]
    public static IServiceCollection AddMlControllers(this IServiceCollection services) => services;


}
