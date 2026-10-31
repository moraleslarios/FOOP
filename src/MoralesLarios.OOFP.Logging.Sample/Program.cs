using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.Logging;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;
using Serilog;
using Serilog.Extensions.Logging;
using static MoralesLarios.OOFP.Helpers.Constants;

using var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        services.AddLogging(loggingBuilder =>
                        {
                            var loggerConfiguration = new LoggerConfiguration();

                            var serilogLogger = loggerConfiguration
                                                    .WriteTo.Console()
                                                    .CreateLogger();

                            loggingBuilder.Services.AddSingleton<ILoggerFactory>(provider => new SerilogLoggerFactory(serilogLogger, dispose: false));
                        });
                    })
                    .Build();

ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

if (! logger.IsEnabled(LogLevel.Information))
{
    Console.WriteLine("El nivel Information está apagado: la demostración no registraría ninguna traza.");

    return;
}

logger.LogInformation("**** Comenzando la demostración de MoralesLarios.OOFP.Logging ****");

logger.LogInformation("**** Probando LogMlResultInformation con un registrador tipado ILogger<T> ****");

MlResult<int> calculoValido  = 42;
MlResult<int> calculoFallido = "El cálculo no se ha podido completar".ToMlResultFail<int>();

calculoValido.LogMlResultInformation<int, Program>(logger,
                                                   validBuildMessage: valor         => $"Correcto: camino feliz registrado con el valor {valor}",
                                                   failBuildMessage : errorsDetails => "Esto no tendría que imprimirse: el resultado es válido");

calculoFallido.LogMlResultInformation<int, Program>(logger,
                                                    validBuildMessage: valor         => "Esto no tendría que imprimirse: el resultado es fallido",
                                                    failBuildMessage : errorsDetails => $"Correcto: camino de error registrado con el fallo {errorsDetails.GetErrorsSummary()}");

logger.LogInformation("**** LogMlResultInformation con un registrador tipado FINALIZADO ****");

logger.LogInformation("**** Probando LogMlResultIfFailWithValue ****");

MlResult<int> fallidoConValorEntero = "Error de prueba".ToMlResultFail<int>()
                                            .AddValueDetailIfFail(12);

fallidoConValorEntero.LogMlResultIfFailWithValue(logger          : logger,
                                                 logLevel        : LogLevel.Information,
                                                 failBuildMessage: errorsDetails => "Correcto: LogMlResultIfFailWithValue ha registrado el fallo que arrastra un valor");

MlResult<int> fallidoConValorFecha = "Error de prueba".ToMlResultFail<int>()
                                            .AddValueDetailIfFail(DateTime.Today);

fallidoConValorFecha.LogMlResultIfFailWithValue<int, DateTime>(logger          : logger,
                                                               logLevel        : LogLevel.Information,
                                                               failBuildMessage: (errorsDetails, value) => $"Correcto: LogMlResultIfFailWithValue ha recuperado la fecha {value:yyyy-MM-dd}");

MlResult<int> fallidoSinValor = "Error de prueba".ToMlResultFail<int>();

fallidoSinValor.LogMlResultIfFailWithValue(logger          : logger,
                                           logLevel        : LogLevel.Information,
                                           failBuildMessage: errorsDetails => "Esto no tendría que imprimirse: el fallo no arrastra ningún valor");

fallidoSinValor.LogMlResultIfFailWithValue<int, DateTime>(logger          : logger,
                                                          logLevel        : LogLevel.Information,
                                                          failBuildMessage: (errorsDetails, value) => "Esto no tendría que imprimirse: el fallo no arrastra ninguna fecha");

MlResult<int> validoSinDetalles = 1;

validoSinDetalles.LogMlResultIfFailWithValue(logger          : logger,
                                             logLevel        : LogLevel.Information,
                                             failBuildMessage: errorsDetails => "Esto no tendría que imprimirse: el resultado es válido");

logger.LogInformation("**** LogMlResultIfFailWithValue FINALIZADO ****");

logger.LogInformation("**** Probando LogMlResultIfFailWithException ****");

MlResult<int> fallidoConExcepcion = ("miError",
                                        new Dictionary<string, object>
                                        {
                                            { ExceptionMetadataKey, new Exception("miException") },
                                            { "key2", "value2" }
                                        }
                                    );

fallidoConExcepcion.LogMlResultIfFailWithException(logger          : logger,
                                                   logLevel        : LogLevel.Information,
                                                   failBuildMessage: errorsDetails => "Correcto: LogMlResultIfFailWithException ha registrado el fallo que arrastra una excepción");

fallidoConExcepcion.LogMlResultIfFailWithException<int>(logger          : logger,
                                                        logLevel        : LogLevel.Information,
                                                        failBuildMessage: (errorsDetails, ex) => $"Correcto: LogMlResultIfFailWithException ha recuperado la excepción con el mensaje {ex.Message}");

MlResult<int> fallidoSinExcepcion = "Error sin excepción".ToMlResultFail<int>();

fallidoSinExcepcion.LogMlResultIfFailWithException(logger          : logger,
                                                   logLevel        : LogLevel.Information,
                                                   failBuildMessage: errorsDetails => "Esto no tendría que imprimirse: el fallo no arrastra ninguna excepción");

validoSinDetalles.LogMlResultIfFailWithException(logger          : logger,
                                                 logLevel        : LogLevel.Information,
                                                 failBuildMessage: errorsDetails => "Esto no tendría que imprimirse: el resultado es válido");

logger.LogInformation("**** LogMlResultIfFailWithException FINALIZADO ****");

logger.LogInformation("**** Probando LogMlResultIfFailWithoutException ****");

fallidoSinExcepcion.LogMlResultIfFailWithoutException(logger          : logger,
                                                      logLevel        : LogLevel.Information,
                                                      failBuildMessage: errorsDetails => "Correcto: LogMlResultIfFailWithoutException ha registrado el error sin excepción");

fallidoConExcepcion.LogMlResultIfFailWithoutException(logger          : logger,
                                                      logLevel        : LogLevel.Information,
                                                      failBuildMessage: errorsDetails => "Esto no tendría que imprimirse: el fallo sí arrastra una excepción");

validoSinDetalles.LogMlResultIfFailWithoutException(logger          : logger,
                                                    logLevel        : LogLevel.Information,
                                                    failBuildMessage: errorsDetails => "Esto no tendría que imprimirse: el resultado es válido");

logger.LogInformation("**** LogMlResultIfFailWithoutException FINALIZADO ****");

logger.LogInformation("**** Probando el registro estructurado con plantillas ****");

// La plantilla NO se interpola: llega intacta al registrador, que la publica como {OriginalFormat}
// y expone cada marcador como un campo consultable en el destino.
logger.LogMlResultInformationTemplate("Correcto: plantilla suelta sobre el registrador, a las {Hora}",
                                      DateTime.Now);

calculoValido.LogMlResultInformationTemplate(logger,
                                             "Correcto: plantilla con el valor {Valor} consultado a las {Hora}",
                                             42, DateTime.Now);

logger.LogMlResultTemplate(LogLevel.Information,
                           new EventId(1001, "PlantillaDemo"),
                           "Correcto: plantilla cruzada con el EventId {EventoId}",
                           1001);

// En la rama de fallo, los campos canónicos los aporta MlResultLogErrorFields:
// ErrorMessage · ErrorCount · ExceptionType, en ese orden.
calculoFallido.LogMlResultIfFailTemplate(logger,
                                         LogLevel.Information,
                                         MlResultLogErrorFields.DefaultTemplate,
                                         MlResultLogErrorFields.From);

calculoValido.LogMlResultIfFailTemplate(logger,
                                        LogLevel.Information,
                                        "Esto no tendría que imprimirse: el resultado es válido · {ErrorMessage}",
                                        MlResultLogErrorFields.From);

logger.LogInformation("**** El registro estructurado con plantillas FINALIZADO ****");

logger.LogInformation($"{Environment.NewLine}{Environment.NewLine}DEMOSTRACIÓN FINALIZADA");
