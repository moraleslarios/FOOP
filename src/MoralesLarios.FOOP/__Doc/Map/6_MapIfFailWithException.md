# MlResultActionsMapIfFailWithException - Recuperación de Errores con Excepciones Preservadas

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Diferencias Clave con MapIfFailWithValue](#diferencias-clave-con-mapiffailwithvalue)
4. [Concepto de "Detail Exception"](#concepto-de-detail-exception)
5. [Métodos MapIfFailWithException Básicos](#métodos-mapiffailwithexception-básicos)
6. [Métodos con Cambio de Tipo](#métodos-con-cambio-de-tipo)
7. [Métodos TryMapIfFailWithException](#métodos-trymapiffailwithexception)
8. [Variantes Asíncronas](#variantes-asíncronas)
9. [Ejemplos Prácticos](#ejemplos-prácticos)
10. [Comparación con Otros Patrones](#comparación-con-otros-patrones)
11. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsMapIfFailWithException` implementa **recuperación de errores con acceso a excepciones preservadas**. Estos métodos permiten acceder a la excepción original que causó el fallo, habilitando estrategias de recuperación específicas basadas en el tipo y contenido de la excepción.

### Propósito Principal

- **Exception-Based Recovery**: Recuperación basada en el tipo específico de excepción
- **Error Context Preservation**: Preservar el contexto de la excepción original
- **Type-Specific Handling**: Manejo diferenciado según el tipo de excepción
- **Exception Information Access**: Acceso completo a la información de la excepción

### Filosofía de Diseño

```
Valor Válido              → Valor (sin modificación)
Error con Excepción       → func(exception) → Valor Recuperado
Error sin Excepción       → Error (propagación sin modificación)
```

**Comportamiento Clave**: Si no hay excepción preservada, **no se añade error adicional**, solo se propaga el error original.

---

## Análisis de la Clase

### Patrón de Funcionamiento

`MapIfFailWithException` funciona cuando:

1. Una operación previa **preservó una excepción** antes de fallar
2. El error contiene esa excepción como "detail exception"
3. La función de recuperación puede usar la excepción para decidir la estrategia
4. Se genera un resultado válido basado en el análisis de la excepción

### Comportamiento Distintivo

```csharp
// Si HAY excepción preservada
var result = source.Match(
    fail: errorsDetails => errorsDetails.GetDetailException().Match(
        fail: _ => errorsDetails.ToMlResultFail<T>(),           // Sin excepción: propagar error original
        valid: ex => funcException(ex).ToMlResultValid()       // Con excepción: recuperar
    ),
    valid: value => value
);
```

**Importante**: Si no hay excepción detail, se retorna el error original **sin modificaciones**.

---

## Diferencias Clave con MapIfFailWithValue

### Comportamiento Diferenciado

| Aspecto | MapIfFailWithValue | MapIfFailWithException |
|---------|-------------------|------------------------|
| **Sin Detail** | Añade nuevo error al existente | Propaga error original sin cambios |
| **Con Detail** | Usa valor preservado para recuperar | Usa excepción preservada para recuperar |
| **Estrategia** | Mejora/completa valores parciales | Analiza excepción para decidir recuperación |
| **Casos de Uso** | Datos parciales utilizables | Errores específicos manejables |

### Ejemplo Comparativo

```csharp
// MapIfFailWithValue: Siempre intenta recuperar
var resultValue = source
    .MapIfFailWithValue(partialData => EnhancePartialData(partialData));
    // Si no hay valor preservado: AÑADE ERROR sobre el existente

// MapIfFailWithException: Solo recupera si hay excepción específica
var resultException = source
    .MapIfFailWithException(ex => HandleSpecificException(ex));
    // Si no hay excepción preservada: PROPAGA ERROR ORIGINAL sin cambios
```

---

## Concepto de "Detail Exception"

### ¿Qué es un Detail Exception?

Un **Detail Exception** es una excepción que se preserva en el `MlErrorsDetails` cuando una operación falla debido a una excepción específica.

```csharp
// Ejemplo conceptual: Una operación que preserva la excepción
public MlResult<ProcessedData> ProcessRiskyData(RawData data)
{
    try 
    {
        return PerformRiskyOperation(data);
    }
    catch (SpecificException ex)
    {
        // Preservar la excepción específica
        return MlResult<ProcessedData>.FailWithException(
            ex,  // Excepción preservada
            $"Processing failed: {ex.Message}"
        );
    }
    catch (Exception ex)
    {
        // Error general sin preservar excepción
        return MlResult<ProcessedData>.Fail($"Unexpected error: {ex.Message}");
    }
}
```

### Acceso al Detail Exception

Los métodos usan `errorsDetails.GetDetailException()` para extraer la excepción:

```csharp
public static MlResult<T> MapIfFailWithException<T>(this MlResult<T> source,
                                                    Func<Exception, T> funcException)
    => source.Match(
        fail: errorsDetails => errorsDetails.GetDetailException().Match(
            fail: exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),  // Sin excepción
            valid: ex => funcException(ex).ToMlResultValid()                // Con excepción
        ),
        valid: value => value
    );
```

---

## Métodos MapIfFailWithException Básicos

### `MapIfFailWithException<T>()` - Recuperación con Mismo Tipo

**Propósito**: Recupera de un error usando la excepción preservada y manteniendo el mismo tipo

```csharp
public static MlResult<T> MapIfFailWithException<T>(this MlResult<T> source,
                                                    Func<Exception, T> funcException)
```

**Comportamiento**:
- Si `source` es válido: Retorna el valor sin modificación
- Si `source` es fallido con excepción: Aplica `funcException(exception)`
- Si `source` es fallido sin excepción: Propaga el error original sin cambios

**Ejemplo Básico**:
```csharp
var result = GetDataFromExternalService()
    .MapIfFailWithException(ex => ex switch
    {
        TimeoutException _ => GetCachedData(),
        UnauthorizedException _ => GetPublicData(),
        NotFoundException _ => GetDefaultData(),
        _ => throw new InvalidOperationException($"Unhandled exception type: {ex.GetType()}")
    });
```

### `MapIfFailWithExceptionAsync<T>()` - Versión Asíncrona

```csharp
public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(
    this MlResult<T> source,
    Func<Exception, Task<T>> funcExceptionAsync)
```

**Ejemplo**:
```csharp
var result = await CallExternalApiAsync()
    .MapIfFailWithExceptionAsync(async ex => ex switch
    {
        HttpRequestException httpEx when httpEx.Message.Contains("503") => 
            await GetDataFromFallbackServiceAsync(),
        
        TaskCanceledException _ => 
            await GetDataFromFastCacheAsync(),
        
        _ => throw new InvalidOperationException($"Cannot recover from {ex.GetType()}")
    });
```

---

## Métodos con Cambio de Tipo

### `MapIfFailWithException<T, TReturn>()` - Recuperación con Transformación

**Propósito**: Permite diferentes tipos para la entrada y salida, con manejo específico de excepciones

```csharp
public static MlResult<TReturn> MapIfFailWithException<T, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> funcValid,           // Para valores válidos
    Func<Exception, TReturn> funcFail)    // Para excepciones preservadas
```

**Ejemplo**:
```csharp
var displayMessage = GetUserProfile(userId)
    .MapIfFailWithException<UserProfile, string>(
        funcValid: profile => $"Welcome, {profile.Name}!",
        funcFail: ex => ex switch
        {
            UnauthorizedException _ => "Please log in to view your profile",
            NotFoundException _ => "Profile not found",
            TimeoutException _ => "Profile temporarily unavailable",
            _ => "Unable to load profile"
        }
    );
```

### Casos de Uso Avanzados

```csharp
// Ejemplo: Conversión de datos con manejo específico de errores de parsing
public class DataConverter
{
    public MlResult<ConvertedData> ConvertToFormat(RawData data, string format)
    {
        return ParseRawData(data)
            .MapIfFailWithException<ParsedData, ConvertedData>(
                funcValid: parsed => ConvertToSpecificFormat(parsed, format),
                funcFail: ex => ex switch
                {
                    // Errores de formato específicos
                    JsonException jsonEx => CreatePartialConvertedData(data, jsonEx),
                    XmlException xmlEx => CreateBasicConvertedData(data),
                    FormatException formatEx => CreateFallbackData(format),
                    
                    // Errores de encoding
                    DecoderFallbackException _ => ConvertWithBasicEncoding(data),
                    
                    // Otros errores
                    _ => throw new InvalidOperationException($"Cannot handle {ex.GetType()}")
                }
            );
    }
}
```

---

## Métodos TryMapIfFailWithException

### `TryMapIfFailWithException<T>()` - Versión Segura

**Propósito**: Versión que captura excepciones en la función de recuperación

```csharp
public static MlResult<T> TryMapIfFailWithException<T>(
    this MlResult<T> source,
    Func<Exception, T> funcException,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento Especial**: Si la función de recuperación falla, **fusiona** el nuevo error con el original usando `MergeErrorsDetailsIfFail`.

**Ejemplo**:
```csharp
var result = ProcessDocument(document)
    .TryMapIfFailWithException(
        funcException: ex => ex switch
        {
            FileNotFoundException _ => LoadBackupDocument(),     // Puede fallar también
            IOException _ => CreateEmptyDocument(),              // Puede fallar también
            UnauthorizedException _ => LoadPublicDocument(),     // Puede fallar también
            _ => throw new NotSupportedException($"Cannot recover from {ex.GetType()}")
        },
        errorMessageBuilder: recoveryEx => 
            $"Recovery failed for {recoveryEx.GetType().Name}: {recoveryEx.Message}"
    );
```

### `TryMapIfFailWithException<T, TReturn>()` - Versión Segura con Tipos

```csharp
public static MlResult<TReturn> TryMapIfFailWithException<T, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> funcValid,
    Func<Exception, TReturn> funcFail,
    Func<Exception, string> errorMessageBuilder)
```

**Ejemplo**:
```csharp
var result = GetConfiguration(configKey)
    .TryMapIfFailWithException<ConfigData, AppConfig>(
        funcValid: config => CreateAppConfig(config),           // Puede fallar
        funcFail: ex => ex switch
        {
            FileNotFoundException _ => CreateDefaultConfig(),    // Puede fallar
            JsonException _ => CreateMinimalConfig(),           // Puede fallar
            _ => throw new ConfigurationException($"Cannot create config from {ex.GetType()}")
        },
        errorMessageBuilder: ex => $"Configuration creation failed: {ex.Message}"
    );
```

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones Asíncronas

| Fuente | funcValid | funcFail | Método |
|--------|-----------|----------|---------|
| `MlResult<T>` | - | `Exception → U` | `MapIfFailWithException` |
| `MlResult<T>` | - | `Exception → Task<U>` | `MapIfFailWithExceptionAsync` |
| `MlResult<T>` | `T → U` | `Exception → U` | `MapIfFailWithException` |
| `MlResult<T>` | `T → Task<U>` | `Exception → Task<U>` | `MapIfFailWithExceptionAsync` |
| `Task<MlResult<T>>` | `T → U` | `Exception → U` | `MapIfFailWithExceptionAsync` |
| `Task<MlResult<T>>` | `T → Task<U>` | `Exception → U` | `MapIfFailWithExceptionAsync` |
| `Task<MlResult<T>>` | `T → U` | `Exception → Task<U>` | `MapIfFailWithExceptionAsync` |
| `Task<MlResult<T>>` | `T → Task<U>` | `Exception → Task<U>` | `MapIfFailWithExceptionAsync` |

### Manejo de Excepciones Específicas

```csharp
public class AsyncExceptionHandler
{
    public async Task<MlResult<ProcessedResult>> ProcessWithFallbackAsync(InputData input)
    {
        return await ProcessPrimaryAsync(input)
            .MapIfFailWithExceptionAsync(async ex => ex switch
            {
                // Timeouts: usar cache
                OperationCanceledException _ => await GetFromCacheAsync(input.Id),
                TimeoutException _ => await GetFromCacheAsync(input.Id),
                
                // Errores de red: usar servicio alternativo
                HttpRequestException httpEx when httpEx.Message.Contains("503") =>
                    await ProcessWithAlternativeServiceAsync(input),
                
                // Errores de autorización: usar datos públicos
                UnauthorizedException _ => await GetPublicDataAsync(input.Id),
                
                // Errores de formato: procesar con parser básico
                JsonException _ => await ProcessWithBasicParserAsync(input),
                
                _ => throw new NotSupportedException($"Cannot recover from {ex.GetType()}")
            });
    }
}
```

---

## Ejemplos Prácticos

### Ejemplo 1: Manejo de APIs Externas con Recuperación Específica

```csharp
public class ExternalApiServiceWithExceptionHandling
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalApiServiceWithExceptionHandling> _logger;
    private readonly ICacheService _cache;

    public async Task<MlResult<WeatherData>> GetWeatherDataAsync(string city)
    {
        return await CallWeatherApiAsync(city)
            .MapIfFailWithExceptionAsync(async ex =>
            {
                _logger.LogWarning("Weather API call failed for {City}: {ExceptionType} - {Message}",
                                  city, ex.GetType().Name, ex.Message);

                return ex switch
                {
                    // Timeout: datos en cache aunque sean viejos
                    TimeoutException _ => await GetCachedWeatherDataAsync(city)
                        ?? CreateTimeoutFallbackData(city),

                    // Rate limiting: esperar y reintentar con datos básicos
                    HttpRequestException httpEx when httpEx.Message.Contains("429") =>
                        await HandleRateLimitingAsync(city),

                    // Service unavailable: usar servicio alternativo
                    HttpRequestException httpEx when httpEx.Message.Contains("503") =>
                        await GetWeatherFromAlternativeSourceAsync(city),

                    // Unauthorized: usar datos públicos limitados
                    UnauthorizedException _ => CreatePublicWeatherData(city),

                    // Not found: datos por defecto para esa ciudad
                    NotFoundException _ => CreateDefaultWeatherData(city),

                    // Network errors: datos offline
                    SocketException _ => await GetOfflineWeatherDataAsync(city),
                    HttpRequestException _ when ex.Message.Contains("network") =>
                        await GetOfflineWeatherDataAsync(city),

                    // JSON parsing errors: intentar parsing básico
                    JsonException jsonEx => await HandleJsonParsingErrorAsync(city, jsonEx),

                    // Otros errores HTTP específicos
                    HttpRequestException httpEx => HandleHttpErrorByStatusCode(httpEx, city),

                    // Fallback final
                    _ => CreateEmergencyWeatherData(city, ex)
                };
            });
    }

    public async Task<MlResult<UserProfile>> GetUserProfileAsync(int userId)
    {
        return await FetchUserProfileAsync(userId)
            .TryMapIfFailWithExceptionAsync(
                funcException: async ex =>
                {
                    _logger.LogError("Profile fetch failed for user {UserId}: {Exception}",
                                    userId, ex);

                    return ex switch
                    {
                        // Database errors: usar cache o datos básicos
                        SqlException sqlEx when sqlEx.Number == 2 => // Timeout
                            await GetProfileFromCacheAsync(userId) ??
                            CreateBasicProfileFromUserId(userId),

                        SqlException sqlEx when sqlEx.Number == 18456 => // Login failed
                            throw new UnauthorizedException("Database access denied"),

                        // Network database errors
                        InvalidOperationException _ when ex.Message.Contains("connection") =>
                            await GetProfileFromLocalCacheAsync(userId),

                        // Serialization errors: reconstruir desde datos básicos
                        JsonException _ => await ReconstructProfileFromBasicDataAsync(userId),
                        
                        // Authorization errors: perfil limitado
                        UnauthorizedException _ => CreateLimitedProfile(userId),

                        // Not found: crear perfil placeholder
                        NotFoundException _ => CreatePlaceholderProfile(userId),

                        // Validation errors: perfil con datos mínimos
                        ValidationException validationEx => 
                            CreateMinimalProfileFromValidationError(userId, validationEx),

                        _ => throw new ProfileException($"Unrecoverable profile error: {ex.Message}", ex)
                    };
                },
                errorMessageBuilder: ex => 
                    $"Failed to recover user profile for {userId}: {ex.Message}"
            );
    }

    public MlResult<ConfigurationData> LoadConfiguration(string configPath)
    {
        return LoadConfigurationFromFile(configPath)
            .MapIfFailWithException<ConfigurationData>(ex =>
            {
                _logger.LogWarning("Configuration loading failed from {ConfigPath}: {Exception}",
                                  configPath, ex);

                return ex switch
                {
                    // File not found: usar configuración por defecto
                    FileNotFoundException _ => GetDefaultConfiguration(),

                    // Access denied: intentar ubicación alternativa
                    UnauthorizedAccessException _ => LoadFromAlternativeLocation(configPath),

                    // JSON parsing errors: intentar parsing tolerante
                    JsonException jsonEx => ParseConfigWithFallback(configPath, jsonEx),

                    // XML parsing errors: convertir a formato simple
                    XmlException xmlEx => ConvertXmlErrorToSimpleConfig(configPath, xmlEx),

                    // IO errors: configuración mínima
                    IOException ioEx when ioEx.Message.Contains("sharing") =>
                        WaitAndRetryOrUseDefault(configPath),

                    IOException _ => GetMinimalConfiguration(),

                    // Format errors: configuración básica
                    FormatException _ => GetBasicConfiguration(),

                    // Encoding errors: reintento con encoding diferente
                    DecoderFallbackException _ => LoadWithAlternativeEncoding(configPath),

                    _ => throw new ConfigurationException(
                        $"Cannot recover from configuration error: {ex.Message}", ex)
                };
            });
    }

    public async Task<MlResult<ProcessedDocument>> ProcessDocumentAsync(DocumentUpload upload)
    {
        return await ProcessDocumentWithAdvancedFeaturesAsync(upload)
            .TryMapIfFailWithExceptionAsync<ProcessedDocument>(
                funcException: async ex =>
                {
                    _logger.LogInformation("Advanced document processing failed, trying fallback: {Exception}",
                                          ex.GetType().Name);

                    return ex switch
                    {
                        // OCR errors: procesar sin OCR
                        OcrException _ => await ProcessDocumentWithoutOcrAsync(upload),

                        // Image processing errors: usar procesamiento básico
                        ImageProcessingException _ => await ProcessWithBasicImageHandlingAsync(upload),

                        // PDF errors: extraer texto básico
                        PdfException pdfEx when pdfEx.Message.Contains("corrupted") =>
                            await ExtractBasicTextFromPdfAsync(upload),

                        // Memory errors: procesar en chunks
                        OutOfMemoryException _ => await ProcessDocumentInChunksAsync(upload),

                        // Format not supported: conversión básica
                        NotSupportedException _ => await ConvertToBasicFormatAsync(upload),

                        // Virus scan errors: procesar sin escaneado
                        SecurityException _ => await ProcessWithoutVirusScanAsync(upload),

                        // Timeout en procesamiento: versión rápida
                        TimeoutException _ => await ProcessDocumentQuicklyAsync(upload),

                        _ => throw new DocumentProcessingException(
                            $"Document processing cannot recover from: {ex.Message}", ex)
                    };
                },
                errorMessage: "Document processing fallback failed"
            );
    }

    // Métodos auxiliares específicos para cada tipo de excepción
    private async Task<WeatherData> HandleRateLimitingAsync(string city)
    {
        await Task.Delay(1000); // Breve espera
        
        return new WeatherData
        {
            City = city,
            Temperature = 20, // Temperatura promedio
            Description = "Data limited due to rate limiting",
            IsLimited = true,
            Source = "rate-limited-fallback",
            LastUpdated = DateTime.UtcNow
        };
    }

    private async Task<WeatherData> HandleJsonParsingErrorAsync(string city, JsonException jsonEx)
    {
        // Intentar extraer datos básicos del JSON malformado
        _logger.LogDebug("Attempting basic JSON parsing for {City}", city);
        
        try
        {
            // Lógica de parsing básico aquí
            return await TryBasicJsonParsingAsync(city);
        }
        catch
        {
            return CreateDefaultWeatherData(city);
        }
    }

    private WeatherData HandleHttpErrorByStatusCode(HttpRequestException httpEx, string city)
    {
        return httpEx.Message switch
        {
            var msg when msg.Contains("404") => CreateNotFoundWeatherData(city),
            var msg when msg.Contains("500") => CreateServerErrorWeatherData(city),
            var msg when msg.Contains("502") || msg.Contains("503") => CreateServiceUnavailableWeatherData(city),
            _ => CreateGeneralErrorWeatherData(city, httpEx)
        };
    }

    private UserProfile CreateMinimalProfileFromValidationError(int userId, ValidationException validationEx)
    {
        return new UserProfile
        {
            UserId = userId,
            Name = $"User{userId}",
            Email = null, // No incluir email si hay errores de validación
            IsComplete = false,
            ValidationErrors = new List<string> { validationEx.Message },
            Source = "validation-error-fallback"
        };
    }

    private ConfigurationData ParseConfigWithFallback(string configPath, JsonException jsonEx)
    {
        try
        {
            // Intentar parsing línea por línea, saltando líneas problemáticas
            return ParseConfigLineByLine(configPath);
        }
        catch
        {
            _logger.LogWarning("Fallback config parsing also failed for {ConfigPath}", configPath);
            return GetDefaultConfiguration();
        }
    }

    private async Task<ProcessedDocument> ProcessDocumentInChunksAsync(DocumentUpload upload)
    {
        _logger.LogInformation("Processing document in chunks due to memory constraints");
        
        var chunks = SplitDocumentIntoChunks(upload);
        var processedChunks = new List<ProcessedChunk>();

        foreach (var chunk in chunks)
        {
            try
            {
                var processedChunk = await ProcessChunkAsync(chunk);
                processedChunks.Add(processedChunk);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Chunk processing failed, skipping: {Exception}", ex.Message);
                // Continuar con otros chunks
            }
        }

        return CombineProcessedChunks(processedChunks, upload);
    }

    private WeatherData CreateEmergencyWeatherData(string city, Exception originalException)
    {
        return new WeatherData
        {
            City = city,
            Temperature = 15, // Temperatura muy conservadora
            Description = "Weather data unavailable",
            IsEmergencyData = true,
            ErrorInfo = $"Original error: {originalException.GetType().Name}",
            Source = "emergency-fallback",
            LastUpdated = DateTime.UtcNow,
            Reliability = 0.1 // Muy baja confiabilidad
        };
    }
}

// Clases de apoyo específicas para el manejo de excepciones
public class WeatherData
{
    public string City { get; set; }
    public double Temperature { get; set; }
    public string Description { get; set; }
    public bool IsLimited { get; set; }
    public bool IsEmergencyData { get; set; }
    public string ErrorInfo { get; set; }
    public string Source { get; set; }
    public DateTime LastUpdated { get; set; }
    public double Reliability { get; set; } = 1.0;
}

public class UserProfile
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsComplete { get; set; }
    public List<string> ValidationErrors { get; set; }
    public string Source { get; set; }
}

public class ProcessedDocument
{
    public string DocumentId { get; set; }
    public string Content { get; set; }
    public List<ProcessedChunk> Chunks { get; set; }
    public bool IsPartiallyProcessed { get; set; }
    public List<string> ProcessingWarnings { get; set; }
    public string ProcessingMethod { get; set; }
}

// Excepciones específicas
public class OcrException : Exception
{
    public OcrException(string message, Exception innerException = null) 
        : base(message, innerException) { }
}

public class ImageProcessingException : Exception
{
    public ImageProcessingException(string message, Exception innerException = null) 
        : base(message, innerException) { }
}

public class PdfException : Exception
{
    public PdfException(string message, Exception innerException = null) 
        : base(message, innerException) { }
}

public class DocumentProcessingException : Exception
{
    public DocumentProcessingException(string message, Exception innerException = null) 
        : base(message, innerException) { }
}

public class ProfileException : Exception
{
    public ProfileException(string message, Exception innerException = null) 
        : base(message, innerException) { }
}

public class ConfigurationException : Exception
{
    public ConfigurationException(string message, Exception innerException = null) 
        : base(message, innerException) { }
}
```

### Ejemplo 2: Sistema de Archivos con Manejo Granular de Excepciones

```csharp
public class FileSystemService
{
    private readonly ILogger<FileSystemService> _logger;

    public MlResult<FileContent> ReadFileWithRecovery(string filePath)
    {
        return ReadFileContent(filePath)
            .MapIfFailWithException(ex =>
            {
                _logger.LogWarning("File read failed for {FilePath}: {ExceptionType}",
                                  filePath, ex.GetType().Name);

                return ex switch
                {
                    // File not found: buscar en ubicaciones alternativas
                    FileNotFoundException _ => SearchInAlternativeLocations(filePath),

                    // Access denied: intentar con permisos elevados o lectura parcial
                    UnauthorizedAccessException _ => AttemptElevatedReadOrPartial(filePath),

                    // File in use: esperar e intentar de nuevo
                    IOException ioEx when ioEx.Message.Contains("being used") =>
                        WaitAndRetryFileRead(filePath),

                    // Path too long: usar nombre corto
                    PathTooLongException _ => ReadFileWithShortPath(filePath),

                    // Directory not found: crear directorios y buscar archivo
                    DirectoryNotFoundException _ => CreateDirectoriesAndSearchFile(filePath),

                    // Drive not ready: intentar en drives alternativos
                    DriveNotFoundException _ => SearchFileInAvailableDrives(filePath),

                    // Disk full durante lectura: lectura parcial
                    IOException ioEx when ioEx.Message.Contains("disk full") =>
                        ReadFilePartially(filePath),

                    // Encoding errors: intentar diferentes encodings
                    DecoderFallbackException _ => ReadFileWithAlternativeEncoding(filePath),

                    // Network path errors: usar cache local
                    IOException ioEx when ioEx.Message.Contains("network") =>
                        GetFileFromLocalCache(filePath),

                    _ => throw new FileOperationException(
                        $"Cannot recover from file read error: {ex.Message}", ex)
                };
            });
    }

    public async Task<MlResult<SaveResult>> SaveFileWithRecoveryAsync(string filePath, byte[] content)
    {
        return await SaveFileContentAsync(filePath, content)
            .TryMapIfFailWithExceptionAsync(
                funcException: async ex =>
                {
                    _logger.LogError("File save failed for {FilePath}: {Exception}",
                                    filePath, ex);

                    return ex switch
                    {
                        // Disk full: comprimir o guardar en ubicación alternativa
                        IOException ioEx when ioEx.Message.Contains("disk full") =>
                            await SaveFileWithCompressionOrAlternativeLocationAsync(filePath, content),

                        // Access denied: cambiar permisos o ubicación
                        UnauthorizedException _ => 
                            await SaveFileWithPermissionHandlingAsync(filePath, content),

                        // Path too long: usar nombres más cortos
                        PathTooLongException _ => 
                            await SaveFileWithShortenedPathAsync(filePath, content),

                        // Directory not found: crear directorios
                        DirectoryNotFoundException _ => 
                            await CreateDirectoriesAndSaveAsync(filePath, content),

                        // File in use: esperar o usar nombre temporal
                        IOException ioEx when ioEx.Message.Contains("being used") =>
                            await SaveFileWithTemporaryNameAsync(filePath, content),

                        // Network errors: guardar localmente y sincronizar después
                        IOException ioEx when ioEx.Message.Contains("network") =>
                            await SaveFileLocallyForLaterSyncAsync(filePath, content),

                        // Security errors: guardar en zona segura
                        SecurityException _ => 
                            await SaveFileInSecureLocationAsync(filePath, content),

                        _ => throw new FileOperationException(
                            $"Cannot recover from file save error: {ex.Message}", ex)
                    };
                },
                errorMessageBuilder: ex => 
                    $"File save recovery failed for {filePath}: {ex.Message}"
            );
    }

    public MlResult<DirectoryListing> ListDirectoryWithRecovery(string directoryPath)
    {
        return ListDirectoryContent(directoryPath)
            .MapIfFailWithException(ex =>
            {
                _logger.LogInformation("Directory listing failed for {DirectoryPath}: {Exception}",
                                      directoryPath, ex.GetType().Name);

                return ex switch
                {
                    // Directory not found: listar directorios padre o similares
                    DirectoryNotFoundException _ => ListSimilarDirectories(directoryPath),

                    // Access denied: listar contenido público o accesible
                    UnauthorizedException _ => ListAccessibleContent(directoryPath),

                    // Path too long: usar rutas cortas
                    PathTooLongException _ => ListDirectoryWithShortPath(directoryPath),

                    // Network errors: usar cache de directorio
                    IOException ioEx when ioEx.Message.Contains("network") =>
                        GetDirectoryListingFromCache(directoryPath),

                    // Drive not ready: listar drives disponibles
                    DriveNotFoundException _ => ListAvailableDrives(),

                    _ => CreateEmptyDirectoryListing(directoryPath, ex)
                };
            });
    }

    // Métodos auxiliares específicos para cada tipo de recuperación
    private FileContent SearchInAlternativeLocations(string originalPath)
    {
        var alternativeLocations = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Path.GetFileName(originalPath)),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.GetFileName(originalPath)),
            Path.Combine(Path.GetTempPath(), Path.GetFileName(originalPath)),
            Path.Combine(@"C:\Backup", Path.GetFileName(originalPath))
        };

        foreach (var location in alternativeLocations)
        {
            try
            {
                if (File.Exists(location))
                {
                    _logger.LogInformation("Found file in alternative location: {Location}", location);
                    return new FileContent
                    {
                        OriginalPath = originalPath,
                        ActualPath = location,
                        Content = File.ReadAllBytes(location),
                        IsFromAlternativeLocation = true
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Failed to read from alternative location {Location}: {Exception}",
                               location, ex.Message);
            }
        }

        throw new FileNotFoundException($"File not found in any alternative location: {originalPath}");
    }

    private FileContent WaitAndRetryFileRead(string filePath)
    {
        const int maxRetries = 3;
        const int delayMs = 1000;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                Thread.Sleep(delayMs * (i + 1)); // Incrementar delay
                
                if (File.Exists(filePath))
                {
                    return new FileContent
                    {
                        OriginalPath = filePath,
                        ActualPath = filePath,
                        Content = File.ReadAllBytes(filePath),
                        WasRetried = true,
                        RetryCount = i + 1
                    };
                }
            }
            catch (IOException ex) when (ex.Message.Contains("being used"))
            {
                if (i == maxRetries - 1) throw; // Re-throw en último intento
                
                _logger.LogDebug("File still in use, retry {Retry}/{MaxRetries} for {FilePath}",
                               i + 1, maxRetries, filePath);
            }
        }

        throw new IOException($"File remains in use after {maxRetries} retries: {filePath}");
    }

    private FileContent ReadFileWithAlternativeEncoding(string filePath)
    {
        var encodings = new[] 
        { 
            Encoding.UTF8, 
            Encoding.ASCII, 
            Encoding.Unicode, 
            Encoding.BigEndianUnicode,
            Encoding.Latin1 
        };

        foreach (var encoding in encodings)
        {
            try
            {
                var text = File.ReadAllText(filePath, encoding);
                var content = encoding.GetBytes(text);
                
                return new FileContent
                {
                    OriginalPath = filePath,
                    ActualPath = filePath,
                    Content = content,
                    UsedAlternativeEncoding = true,
                    EncodingUsed = encoding.EncodingName
                };
            }
            catch (DecoderFallbackException)
            {
                // Continuar con siguiente encoding
                continue;
            }
        }

        throw new DecoderFallbackException($"Could not decode file with any supported encoding: {filePath}");
    }

    private async Task<SaveResult> SaveFileWithCompressionOrAlternativeLocationAsync(string filePath, byte[] content)
    {
        try
        {
            // Intentar comprimir el contenido primero
            var compressedContent = CompressContent(content);
            
            if (compressedContent.Length < content.Length * 0.8) // Si compresión es significativa
            {
                await File.WriteAllBytesAsync(filePath + ".compressed", compressedContent);
                
                return new SaveResult
                {
                    OriginalPath = filePath,
                    ActualPath = filePath + ".compressed",
                    WasCompressed = true,
                    OriginalSize = content.Length,
                    FinalSize = compressedContent.Length,
                    CompressionRatio = (double)compressedContent.Length / content.Length
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Compression failed, trying alternative location: {Exception}", ex.Message);
        }

        // Si compresión no funciona, intentar ubicación alternativa
        var alternativePath = GetAlternativeSaveLocation(filePath);
        await File.WriteAllBytesAsync(alternativePath, content);

        return new SaveResult
        {
            OriginalPath = filePath,
            ActualPath = alternativePath,
            WasMovedToAlternativeLocation = true,
            OriginalSize = content.Length,
            FinalSize = content.Length
        };
    }

    private DirectoryListing ListSimilarDirectories(string originalPath)
    {
        try
        {
            var parentDirectory = Path.GetDirectoryName(originalPath);
            if (Directory.Exists(parentDirectory))
            {
                var similarDirectories = Directory.GetDirectories(parentDirectory)
                    .Where(dir => Path.GetFileName(dir).Contains(Path.GetFileName(originalPath), StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return new DirectoryListing
                {
                    RequestedPath = originalPath,
                    ActualPath = parentDirectory,
                    Directories = similarDirectories,
                    Files = new List<string>(),
                    IsSimilarMatch = true,
                    Message = $"Found {similarDirectories.Count} similar directories"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Failed to list similar directories: {Exception}", ex.Message);
        }

        return CreateEmptyDirectoryListing(originalPath, 
            new DirectoryNotFoundException($"No similar directories found for: {originalPath}"));
    }

    private DirectoryListing CreateEmptyDirectoryListing(string path, Exception originalException)
    {
        return new DirectoryListing
        {
            RequestedPath = path,
            ActualPath = null,
            Directories = new List<string>(),
            Files = new List<string>(),
            IsEmpty = true,
            ErrorInfo = $"Original error: {originalException.GetType().Name} - {originalException.Message}"
        };
    }

    private byte[] CompressContent(byte[] content)
    {
        using var memoryStream = new MemoryStream();
        using var gzipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Compress);
        gzipStream.Write(content, 0, content.Length);
        gzipStream.Close();
        return memoryStream.ToArray();
    }

    private string GetAlternativeSaveLocation(string originalPath)
    {
        var fileName = Path.GetFileName(originalPath);
        var alternativeLocations = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName),
            Path.Combine(Path.GetTempPath(), fileName),
            Path.Combine(@"C:\Backup", fileName)
        };

        foreach (var location in alternativeLocations)
        {
            try
            {
                var directory = Path.GetDirectoryName(location);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                
                return location;
            }
            catch
            {
                continue;
            }
        }

        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "_" + fileName);
    }
}

// Clases de apoyo para el sistema de archivos
public class FileContent
{
    public string OriginalPath { get; set; }
    public string ActualPath { get; set; }
    public byte[] Content { get; set; }
    public bool IsFromAlternativeLocation { get; set; }
    public bool WasRetried { get; set; }
    public int RetryCount { get; set; }
    public bool UsedAlternativeEncoding { get; set; }
    public string EncodingUsed { get; set; }
}

public class SaveResult
{
    public string OriginalPath { get; set; }
    public string ActualPath { get; set; }
    public bool WasCompressed { get; set; }
    public bool WasMovedToAlternativeLocation { get; set; }
    public long OriginalSize { get; set; }
    public long FinalSize { get; set; }
    public double CompressionRatio { get; set; }
}

public class DirectoryListing
{
    public string RequestedPath { get; set; }
    public string ActualPath { get; set; }
    public List<string> Directories { get; set; }
    public List<string> Files { get; set; }
    public bool IsSimilarMatch { get; set; }
    public bool IsEmpty { get; set; }
    public string Message { get; set; }
    public string ErrorInfo { get; set; }
}

public class FileOperationException : Exception
{
    public FileOperationException(string message, Exception innerException = null) 
        : base(message, innerException) { }
}
```

---

## Comparación con Otros Patrones

### MapIfFailWithException vs MapIfFail

```csharp
// MapIfFail: Solo información del mensaje de error
var result = ProcessData(input)
    .MapIfFail(error => GetDefaultData());  // Solo MlErrorsDetails disponible

// MapIfFailWithException: Acceso a la excepción específica
var result = ProcessDataWithException(input)
    .MapIfFailWithException(ex => ex switch
    {
        TimeoutException _ => GetCachedData(),
        FileNotFoundException _ => GetDefaultData(),
        _ => throw new InvalidOperationException("Unhandled exception")
    });
```

### MapIfFailWithException vs MapIfFailWithValue

```csharp
// MapIfFailWithValue: Usa valores preservados
var result = ProcessWithPreservation(input)
    .MapIfFailWithValue(partialData => CompletePartialData(partialData));

// MapIfFailWithException: Usa excepciones preservadas
var result = ProcessWithExceptionCapture(input)
    .MapIfFailWithException(ex => HandleSpecificException(ex));
```

### MapIfFailWithException vs Try-Catch Tradicional

```csharp
// Try-Catch tradicional
try 
{
    var result = RiskyOperation();
    return ProcessResult(result);
}
catch (SpecificException ex)
{
    return HandleSpecificException(ex);
}
catch (Exception ex)
{
    return HandleGeneralException(ex);
}

// MapIfFailWithException: Enfoque funcional
var result = TryRiskyOperation()
    .MapIfFailWithException(ex => ex switch
    {
        SpecificException specific => HandleSpecificException(specific),
        _ => HandleGeneralException(ex)
    });
```

---

## Mejores Prácticas

### 1. Manejo Específico por Tipo de Excepción

```csharp
// ✅ Correcto: Manejo específico y granular
var result = ProcessData(input)
    .MapIfFailWithException(ex => ex switch
    {
        TimeoutException timeout => HandleTimeout(timeout.Timeout),
        FileNotFoundException fileNotFound => HandleMissingFile(fileNotFound.FileName),
        UnauthorizedException unauthorized => HandleUnauthorized(unauthorized.RequiredPermission),
        ValidationException validation => HandleValidation(validation.ValidationErrors),
        _ => throw new UnrecoverableException($"Cannot handle {ex.GetType()}", ex)
    });

// ✅ Correcto: Usar información específica de la excepción
var result = ConnectToDatabase(connectionString)
    .MapIfFailWithException(ex => ex switch
    {
        SqlException sqlEx when sqlEx.Number == 2 => // Connection timeout
            ConnectToBackupDatabase(),
        
        SqlException sqlEx when sqlEx.Number == 18456 => // Login failed
            ConnectWithDifferentCredentials(),
        
        SqlException sqlEx when sqlEx.Number == 40613 => // Database unavailable
            UseOfflineMode(),
        
        _ => throw new DatabaseException("Unrecoverable database error", ex)
    });

// ❌ Incorrecto: Manejo genérico que ignora el tipo específico
var badResult = ProcessData(input)
    .MapIfFailWithException(ex => GetGenericDefault());  // Ignora información valiosa
```

### 2. Preservación Apropiada de Excepciones

```csharp
// ✅ Correcto: Preservar excepciones que contienen información útil
public MlResult<ProcessedData> ProcessRiskyData(RawData data)
{
    try
    {
        return PerformComplexOperation(data);
    }
    catch (ValidationException validationEx)
    {
        // Preservar excepción con información detallada de validación
        return MlResult<ProcessedData>.FailWithException(
            validationEx,
            $"Data validation failed: {validationEx.Message}"
        );
    }
    catch (TimeoutException timeoutEx)
    {
        // Preservar excepción con información de timeout
        return MlResult<ProcessedData>.FailWithException(
            timeoutEx,
            $"Operation timed out after {timeoutEx.Timeout}"
        );
    }
    catch (IOException ioEx)
    {
        // Preservar excepción con información de I/O
        return MlResult<ProcessedData>.FailWithException(
            ioEx,
            $"I/O error during processing: {ioEx.Message}"
        );
    }
    catch (Exception ex)
    {
        // Error general sin preservar excepción (menos útil para recuperación)
        return MlResult<ProcessedData>.Fail($"Unexpected error: {ex.Message}");
    }
}

// ❌ Incorrecto: Preservar excepciones que no aportan valor para recuperación
public MlResult<Data> BadExample(Input input)
{
    try
    {
        return ProcessInput(input);
    }
    catch (Exception ex)
    {
        // Preservar cualquier excepción sin discriminar
        return MlResult<Data>.FailWithException(ex, "Something failed");
    }
}
```

### 3. Logging Adecuado de Recuperaciones por Excepción

```csharp
// ✅ Correcto: Log detallado con contexto de la excepción
var result = LoadConfiguration(configPath)
    .MapIfFailWithException(ex =>
    {
        _logger.LogWarning("Configuration loading failed, applying recovery strategy. " +
                          "ExceptionType: {ExceptionType}, Message: {Message}, FilePath: {FilePath}",
                          ex.GetType().Name, ex.Message, configPath);

        var recoveredConfig = ex switch
        {
            FileNotFoundException _ => 
            {
                _logger.LogInformation("Creating default configuration due to missing file");
                _metrics.IncrementCounter("config.file_not_found_recovery");
                return GetDefaultConfiguration();
            }
            
            JsonException jsonEx => 
            {
                _logger.LogWarning("JSON parsing failed at line {Line}, using fallback parser",
                                  jsonEx.LineNumber);
                _metrics.IncrementCounter("config.json_parse_recovery");
                return ParseConfigWithFallback(configPath);
            }
            
            UnauthorizedAccessException _ =>
            {
                _logger.LogError("Access denied to config file, using embedded defaults");
                _metrics.IncrementCounter("config.access_denied_recovery");
                return GetEmbeddedConfiguration();
            }
            
            _ => throw new ConfigurationException($"Cannot recover from {ex.GetType()}", ex)
        };

        _logger.LogInformation("Configuration recovery successful using strategy: {Strategy}",
                              recoveredConfig.Source);
        
        return recoveredConfig;
    });

// ❌ Incorrecto: Recuperación silenciosa sin visibilidad
var badResult = LoadData(path)
    .MapIfFailWithException(ex => GetDefaultData());  // Sin logging de la estrategia
```

### 4. Composición con Otros Operadores

```csharp
// ✅ Correcto: MapIfFailWithException en pipeline de recuperación
var result = GetRawData(id)
    .Map(data => ValidateData(data))                    // Validación normal
    .MapIfFailWithException(ex => ex switch             // Recuperación específica por excepción
    {
        ValidationException validationEx => RepairValidationErrors(validationEx),
        FormatException formatEx => ConvertToAlternativeFormat(formatEx),
        _ => throw new UnrecoverableDataException("Cannot repair data", ex)
    })
    .MapEnsure(data => data.IsUsable, 
        "Data must be usable after recovery")          // Validación post-recuperación
    .Map(data => ProcessRepairedData(data));            // Procesamiento final

// ✅ Correcto: Cadena de recuperaciones específicas
var result = ProcessLevel1(input)
    .MapIfFailWithException(ex => ex switch
    {
        TimeoutException _ => ProcessLevel1WithTimeout(input),
        _ => throw ex  // Re-throw si no se puede manejar
    })
    .MapIfFailWithException(ex => ex switch
    {
        FileNotFoundException _ => ProcessLevel1FromCache(input),
        _ => throw ex
    })
    .MapIfFail(error => GetUltimateDefault());          // Fallback final genérico

// ❌ Incorrecto: Uso cuando no hay excepciones preservadas
var badResult = GetSimpleValue()
    .MapIfFailWithException(ex => ProcessException(ex))  // GetSimpleValue no preserva excepciones
    .MapIfFail(error => GetDefault());                   // Redundante
```

### 5. Testing de Recuperación por Excepciones

```csharp
// ✅ Correcto: Tests específicos para cada tipo de excepción
[Test]
public void ProcessDocument_WhenTimeoutOccurs_ShouldUseQuickProcessing()
{
    // Arrange
    var document = CreateTestDocument();
    var mockProcessor = new Mock<IDocumentProcessor>();
    mockProcessor.Setup(p => p.ProcessAdvanced(It.IsAny<Document>()))
                 .Throws(new TimeoutException("Processing timeout", TimeSpan.FromSeconds(30)));

    // Act
    var result = _service.ProcessDocumentWithRecovery(document);

    // Assert
    Assert.That(result.IsValid, Is.True);
    Assert.That(result.Value.ProcessingMethod, Is.EqualTo("Quick"));
    Assert.That(result.Value.Warning, Contains.Substring("timeout"));
}

[Test]
public void LoadConfiguration_WhenJsonParsingFails_ShouldUseFallbackParser()
{
    // Arrange
    var configPath = CreateMalformedJsonFile();
    
    // Act
    var result = _configService.LoadConfigurationWithRecovery(configPath);

    // Assert
    Assert.That(result.IsValid, Is.True);
    Assert.That(result.Value.Source, Is.EqualTo("fallback-parser"));
    Assert.That(result.Value.IsComplete, Is.False);
}

[Test]
public void ConnectToDatabase_WhenLoginFails_ShouldTryAlternativeCredentials()
{
    // Arrange
    var connectionString = "Server=test;Database=testdb;";
    var mockConnection = new Mock<IDbConnection>();
    mockConnection.Setup(c => c.Open())
                  .Throws(new SqlException("Login failed for user", 18456));

    // Act
    var result = _databaseService.ConnectWithRecovery(connectionString);

    // Assert
    Assert.That(result.IsValid, Is.True);
    Assert.That(result.Value.ConnectionType, Is.EqualTo("Alternative"));
    Assert.That(result.Value.UserUsed, Is.Not.EqualTo("original-user"));
}
```

### 6. Manejo de Excepciones Anidadas

```csharp
// ✅ Correcto: Consideración de excepciones internas
var result = ProcessComplexOperation(input)
    .MapIfFailWithException(ex =>
    {
        // Analizar la excepción y sus excepciones internas
        var rootCause = GetRootCause(ex);
        
        return rootCause switch
        {
            SqlException sqlEx => HandleDatabaseError(sqlEx),
            HttpRequestException httpEx => HandleNetworkError(httpEx),
            JsonException jsonEx => HandleSerializationError(jsonEx),
            _ => ex switch
            {
                AggregateException aggEx => HandleAggregateException(aggEx),
                TargetInvocationException invokeEx => HandleReflectionException(invokeEx),
                _ => throw new UnhandledException($"Cannot handle {ex.GetType()}", ex)
            }
        };
    });

// Método auxiliar para encontrar la causa raíz
private Exception GetRootCause(Exception exception)
{
    var current = exception;
    while (current.InnerException != null)
    {
        current = current.InnerException;
    }
    return current;
}

// ✅ Correcto: Manejo específico de AggregateException
var result = ProcessParallelOperations(inputs)
    .MapIfFailWithException(ex =>
    {
        if (ex is AggregateException aggEx)
        {
            var innerExceptions = aggEx.InnerExceptions;
            
            // Si todas son del mismo tipo manejable
            if (innerExceptions.All(e => e is TimeoutException))
            {
                return ProcessWithExtendedTimeout(inputs);
            }
            
            // Si la mayoría son exitosas
            if (innerExceptions.Count <= inputs.Count * 0.3) // Menos del 30% fallaron
            {
                return ProcessPartialResults(inputs, innerExceptions);
            }
        }
        
        throw new UnrecoverableException("Too many failures in parallel processing", ex);
    });
```

---

## Consideraciones de Rendimiento

### Costo de Preservación de Excepciones

- **Memory Overhead**: Las excepciones contienen stack traces que consumen memoria
- **Exception Creation Cost**: Crear excepciones tiene costo computacional
- **GC Impact**: Excepciones pueden impactar el garbage collection

### Optimizaciones Específicas

```csharp
// ✅ Optimización: Preservar solo excepciones útiles para recuperación
public MlResult<Data> OptimizedProcessing(Input input)
{
    try
    {
        return ExpensiveOperation(input);
    }
    catch (Exception ex) when (IsRecoverableException(ex))
    {
        // Solo preservar si la excepción es útil para recuperación
        return MlResult<Data>.FailWithException(ex, "Recoverable error occurred");
    }
    catch (Exception ex)
    {
        // No preservar excepciones no recuperables
        return MlResult<Data>.Fail($"Non-recoverable error: {ex.Message}");
    }
}

private bool IsRecoverableException(Exception ex)
{
    return ex is TimeoutException ||
           ex is FileNotFoundException ||
           ex is UnauthorizedException ||
           ex is ValidationException ||
           ex is HttpRequestException;
}

// ✅ Optimización: Pattern matching eficiente
var result = source.MapIfFailWithException(ex => ex switch
{
    TimeoutException => GetCachedResult(),          // Rápido
    FileNotFoundException => GetDefaultResult(),    // Rápido
    _ when IsNetworkException(ex) => GetOfflineResult(), // Check adicional solo si es necesario
    _ => throw new UnhandledException("Cannot recover", ex)
});
```

---

## Resumen

La clase `MlResultActionsMapIfFailWithException` implementa **recuperación inteligente basada en excepciones**:

- **`MapIfFailWithException<T>`**: Recuperación usando excepciones preservadas del mismo tipo
- **`MapIfFailWithException<T,TReturn>`**: Recuperación con transformación de tipos
- **`MapIfFailWithExceptionAsync`**: Soporte completo para operaciones asíncronas
- **`TryMapIfFailWithException`**: Versiones seguras que capturan excepciones en recuperación

**Características clave**:

- **Comportamiento Conservador**: Si no hay excepción preservada, propaga error sin cambios
- **Type-Specific Recovery**: Permite estrategias diferentes según el tipo de excepción
- **Exception Context Access**: Acceso completo a la información de la excepción
- **Intelligent Fallbacks**: Decisiones de recuperación basadas en análisis de excepciones

Estas operaciones son ideales para:

- **Robust Error Handling**: Manejo robusto de errores con estrategias específicas
- **Exception-Driven Recovery**: Recuperación basada en análisis de excepciones
- **Fault Tolerance**: Tolerancia a fallos con degradación inteligente
- **Context-Aware Fallbacks**: Fallbacks que consideran el contexto específico del error

La diferencia principal con otros patrones de recuperación es el **acceso directo a la excepción original**, permitiendo análisis granular del tipo y contenido del error para implementar estrategias de recuperación altamente