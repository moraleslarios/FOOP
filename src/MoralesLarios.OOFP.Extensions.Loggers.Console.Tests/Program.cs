// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using MoralesLarios.OOFP.Extensions.Loggers;
using static MoralesLarios.OOFP.Helpers.Constants;

Console.WriteLine("Hello, World!");

using var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        services.AddLogging(logginBuilder =>
                        {
                            var loggerConfriguration = new LoggerConfiguration();
 
                            var logger = loggerConfriguration
                                            .WriteTo.Console()
                                            .CreateLogger();
 
                            logginBuilder.Services.AddSingleton<ILoggerFactory>(provider => new SerilogLoggerFactory(logger, dispose: false));
                        });
                    })
                    .Build();
 
var logger = host.Services.GetService<ILogger<Program>>();
 
logger.LogInformation("Comenzando la ejecución");






//logger.LogInformation($"**************  Probando el método LogMlResultIfFailWithValue<T> ***************");

//MlResult<int> partialResult1 = MlResult<int>.Fail("Error de prueba")
//                                .AddValueDetailIfFail(12);

//partialResult1.LogMlResultIfFailWithValue(logger: logger,
//                                          logLevel: LogLevel.Information,
//                                          failBuildMessage: errorsDetails => "Correcto: La prueba de LogMlResultIfFailWithValue<T> ha sido buena");

//MlResult<int> partialResult2 = MlResult<int>.Fail("Error de prueba")
//                                .AddValueDetailIfFail(DateTime.Today);

//partialResult2.LogMlResultIfFailWithValue<int, DateTime>(logger: logger,
//                                                         logLevel: LogLevel.Information,
//                                                         failBuildMessage: (errorsDetails, value) => $"Correcto: La prueba de LogMlResultIfFailWithValue<T> ha sido buena con el valor {value}");

//MlResult<int> partialResult3 = MlResult<int>.Fail("Error de prueba");


//partialResult3.LogMlResultIfFailWithValue(logger: logger,
//                                          logLevel: LogLevel.Information,
//                                          failBuildMessage: errorsDetails => "Esto no tendría que imprimirse");

//partialResult3.LogMlResultIfFailWithValue<int, DateTime>(logger: logger,
//                                                         logLevel: LogLevel.Information,
//                                                         failBuildMessage: (errorsDetails, value) => $"Esto tampoco");

//MlResult<int> partialResult4 = 1;


//partialResult4.LogMlResultIfFailWithValue(logger: logger,
//                                          logLevel: LogLevel.Information,
//                                          failBuildMessage: errorsDetails => "Esto no tendría que imprimirse - 2");

//partialResult4.LogMlResultIfFailWithValue<int, DateTime>(logger: logger,
//                                                         logLevel: LogLevel.Information,
//                                                         failBuildMessage: (errorsDetails, value) => $"Esto tampoco - 2");



//logger.LogInformation($"************** LogMlResultIfFailWithValue<T> FINALIZADO ***************");




//logger.LogInformation($"**************  Probando el método LogMlResultIfFailWithException ***************");

//MlResult<int> partialResult1Exception = ("miError",
//                                            new Dictionary<string, object>
//                                            {
//                                                { EX_DESC_KEY, new Exception("miException") },
//                                                { "key2", "value2" }
//                                            }
//                                        );

//partialResult1Exception.LogMlResultIfFailWithException(logger: logger,
//                                                      logLevel: LogLevel.Information,
//                                                      failBuildMessage: errorsDetails => "Correcto: La prueba de LogMlResultIfFailWithException ha sido buena");

//partialResult1Exception.LogMlResultIfFailWithException<int>(logger: logger,
//                                                         logLevel: LogLevel.Information,
//                                                         failBuildMessage: (errorsDetails, ex) => $"Correcto: La prueba de LogMlResultIfFailWithException ha sido buena con el message {ex.Message}");

//MlResult<int> partialResult3Exception = MlResult<int>.Fail("Error de prueba");


//partialResult3Exception.LogMlResultIfFailWithException(logger: logger,
//                                          logLevel: LogLevel.Information,
//                                          failBuildMessage: errorsDetails => "Esto no tendría que imprimirse");

////partialResult3Exception.LogMlResultIfFailWithValue<int, DateTime>(logger: logger,
////                                                         logLevel: LogLevel.Information,
////                                                         failBuildMessage: (errorsDetails, value) => $"Esto tampoco");

//MlResult<int> partialResult4Exception = 1;


//partialResult4Exception.LogMlResultIfFailWithException(logger: logger,
//                                          logLevel: LogLevel.Information,
//                                          failBuildMessage: errorsDetails => "Esto no tendría que imprimirse - 2");

//partialResult4Exception.LogMlResultIfFailWithException(logger: logger,
//                                                         logLevel: LogLevel.Information,
//                                                         failBuildMessage: (errorsDetails, ex) => $"Esto tampoco - 2");



//logger.LogInformation($"************** LogMlResultIfFailWithException FINALIZADO ***************");


logger.LogInformation($"**************  Probando el método LogMlResultIfFailWithoutException ***************");

MlResult<int> partialResult1Exception = ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

MlResult<int> partialResultNoException = "Error sin excepción".ToMlResultFail<int>();

partialResultNoException.LogMlResultIfFailWithoutException(logger: logger,
                                                      logLevel: LogLevel.Information,
                                                      failBuildMessage: errorsDetails => "Correcto: La prueba de LogMlResultIfFailWithException ha sido buena");


partialResult1Exception.LogMlResultIfFailWithoutException(logger: logger,
                                          logLevel: LogLevel.Information,
                                          failBuildMessage: errorsDetails => "Esto no tendría que imprimirse");

//partialResult3Exception.LogMlResultIfFailWithValue<int, DateTime>(logger: logger,
//                                                         logLevel: LogLevel.Information,
//                                                         failBuildMessage: (errorsDetails, value) => $"Esto tampoco");

MlResult<int> partialResult4Exception = 1;


partialResult4Exception.LogMlResultIfFailWithoutException(logger: logger,
                                          logLevel: LogLevel.Information,
                                          failBuildMessage: errorsDetails => "Esto no tendría que imprimirse - 2");





logger.LogInformation($"************** LogMlResultIfFailWithException FINALIZADO ***************");






logger.LogInformation($"{Environment.NewLine}{Environment.NewLine}TESTS FINALIZADOS");

