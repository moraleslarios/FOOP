


********************************************************************************************************************************************
********************************************************       PENDING TASKS       *********************************************************
********************************************************************************************************************************************


                                                    1.- Hacer métodos Ex para los métodos 
	                                                    - BindSaveValueInDetailsIfFaildFuncResult
	                                                    - BindIfFail
	                                                    - BindIfFailWithPreviousValue
	                                                    - MapIfFail

                                                    2.- Hacer logMlFishish... para los métodos con tuplas de parámetros.
	
                                                        public static async Task<MlResult<T>> LogMlResultFinishMethodAsync<T>(this Task<MlResult<T>>                                                   sourceAsync,
                                                                                                                                 ILogger                                                             logger,
                                                                                                                                 (string validMessage                           , LogLevel logLevel) validMessageWithLevel, 
                                                                                                                                 (Func<MlErrorsDetails, string> failBuildMessage, LogLevel logLevel) failBuildMessageWhitLevel)
                                                            => await sourceAsync.LogMlResultAsync(logger, validMessageWithLevel    .logLevel,                   validMessageWithLevel    .validMessage)
                                                                                .LogMlResultAsync(logger, failBuildMessageWhitLevel.logLevel, failBuildMessage: failBuildMessageWhitLevel.failBuildMessage);


                                                         Hacerlos para que la tuplas, no tengan parámetros string, sino builders Func<T, string>


                                                    3.- Crear un método que haga un Map o un Bind que en caso de error, tracee el error diretamente.

                                                                var result = await BuildKeyFpAsync(entidad, clase, tipo)
                                                                                            .LogMlResultErrorAsync(_logger, failBuildMessage : errorDetals => errorDetals.ToErrorsDescription())

                                                            En este caso es más complicado, porque lo tendrá que hacer al crearlo directamente


                                                    4.- La parte de concatenar resultads en tuplas, solo funciona para Map, para Bind no es válido ya que devuelve
                                                        directamente un MlResult<T>. Habría que estudiar la forma de poder hacer un método extensor 'AddResult' o 
                                                        algo así, que nos permitiera ir completando resultados y que deolviera un MlResult<(tupla)>.
                                                        Valorar también que pasaría en caso de que fuera Invalid.

                                                            - Métodos CompleteWithValue,
                                                                      CompleteWithDetailsValue,
                                                                      CompleteWithResultValue




                                                    5.- Crear un metodo extensor para MlErrorDetails, que diga si tiene Excepción.

                                                    6.- Hacer u nmétodo para Bind llamado BindSelf, para devolver el mismo dato que entro en la llamada al Bind
                                                        Esto es interesante, por si no te interesa lo que devuelve la función dentro del Bind y si mantener el parámetro

                                                    - Metodos de HasException
                                                                 HasValue

                                                    - Método de ReplaceValue en ErrorsDetails (Si ya tiene Value, lo sobreescribe, sino lo añade)


- Hacer métodos BindIfFailWithValue
                MapIfFailWithValue
                ExecIfFailWithValue

                BindIfException
                MapIfException
                ExecIfException

                BindIf --> Ya existen
                MapIfE --> Ya existen
                ExecIf --> Ya existen

                BindIfFailWithoutException
                MapIfFailWithoutException
                ExecIfFailWithoutException


- Hacer métodos BoolToMlResult 
                                    .MapExAsync(x => _redisDb.StringSetAsync(key, value, _localTlsSeconds.ToMinutes()),
                                                $"There was a problem executing the method {nameof(_redisDb.StringSetAsync)}").LogGeneralErrorIfFailAsync(_logger)
                                    .BindAsync ( setSuccesly => EnsureFp.ThatAsync(value       : setSuccesly, 
                                                                                   condition   : setSuccesly, 
                                                                                   errorMessage: $"The key {key} with value {value} NOT SET IN REDIS SUCCESLY"))


- Hacer también los métodos de Trazas IfExcepcion
                                      IfFailWithoutException
                                      IfFail --> Ya tendría que estar hecho



                                                    - Intentar hacer todo lo que se pueda async de esta clase MlErrorsDetailsActions

- ExecSelf, con un solo método, que sea valido para Valid y Fail. Tener en cuenta que Valid, recibe el su lambda el Value y Fail el ErrorsDetails

- Cambiar los métodos de CompleteWithValue, CompleteWithDetailsValue, CompleteWithResultValue por TransformData.......

- Crear métodos AddValue
                AddValueIfFail
                AddValueIfValid


- Tener en cuenta acciones con posibles condicionales MapIf, BindIf, ExcecSelfIf, etc ...

- Unificar CompleteWithDetailsValueIfFailAsync - 

- Hacer métodos Map



- Dar la capacidad de poder introducir mensajes personalizados o incluso poder añadir MlErrorsDetails en los métodos de los RepoFp

            public MlResult<T> TryFind(params object[] pk)
            {
                var result = EnsureFp.NotNull(pk, "The object arrya pk cannot be null")
                                .TryMap (x => EnsureFp.NotEmpty(pk, "The object array pk cannot be empty"))
                                .TryMap (x => _repoReader!.Find(pk))
                                .TryBind(x => x.NullToFailed($"{pk.GetPkValues()} values not found data in {typeName}"));

                return result;
            }


        - En este ejemplo, deberíamos de añadir una sobre carga para poder incluir en vez de un string 'msg' un MlErrorsDetails

                    public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, string errorMessage)
                        => condition ? MlResult<T>.Valid(value).ToAsync() : MlResult<T>.Fail(errorMessage).ToAsync();



- Buscar la posibilidad de realizar validaciones de FluentValidation con FP. Trasladar los ValidationsResult a un MlResult.





- ********************* REVISAR TODOS LOS POSIBLES xxx-ChangeValueReturn como hemos hecho en MakeMap

- Hacer MakeBind y todos sus métodos y tests