# MlResultActionsMapIfFailWithoutException - Operaciones de Mapeo Condicional Sin Excepciones

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos MapIfFailWithoutException](#métodos-mapiffailwithoutexception)
4. [Métodos TryMapIfFailWithoutException](#métodos-trymapiffailwithoutexception)
5. [Variantes de Transformación](#variantes-de-transformación)
6. [Variantes Asíncronas](#variantes-asíncronas)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

Los métodos **`MapIfFailWithoutException`** proporcionan capacidades especializadas para el **mapeo condicional** de resultados fallidos que **no contienen excepciones**. Estos métodos permiten aplicar transformaciones de recuperación únicamente cuando el error original no fue causado por una excepción, proporcionando un control granular sobre la estrategia de recuperación.

### Propósito Principal

- **Recuperación Selectiva**: Aplicar lógica de recuperación solo para errores "controlados" (sin excepciones)
- **Preservación de Excepciones**: Mantener intactos los errores causados por excepciones para escalamiento apropiado
- **Transformación de Tipos**: Convertir entre diferentes tipos mientras se respeta la naturaleza del error
- **Mapeo Condicional**: Ejecutar funciones diferentes según el estado del resultado y la ausencia de excepciones

### Diferencia Clave con Map

- **`Map`**: Transforma valores válidos únicamente
- **`MapIfFailWithoutException`**: Transforma errores sin excepción, preserva errores con excepción y opcionalmente transforma valores válidos

---

## Análisis de la Clase

### Estructura y Filosofía

Esta clase implementa cuatro patrones principales:

1. **Mapeo de Recuperación Selectiva**: `MapIfFailWithoutException<T>`
2. **Mapeo Seguro con Captura de Excepciones**: `TryMapIfFailWithoutException<T>`
3. **Transformación de Tipos Condicional**: `MapIfFailWithoutException<T, TReturn>`
4. **Mapeo Dual**: Funciones separadas para casos válidos y fallidos sin excepción

### Características Principales

1. **Detección de Excepciones**: Utiliza `GetDetailException()` para determinar si un error contiene excepciones
2. **Preservación de Estado**: Los errores con excepciones se mantienen inalterados
3. **Flexibilidad de Transformación**: Soporte para cambio de tipos con funciones separadas
4. **Manejo Seguro**: Versiones `Try*` para captura de excepciones en las funciones de mapeo

---

## Métodos MapIfFailWithoutException

### `MapIfFailWithoutException<T>()`

**Propósito**: Transforma un resultado fallido sin excepción en un resultado válido, preservando errores con excepción

```csharp
public static MlResult<T> MapIfFailWithoutException<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, T> func)
```

**Parámetros**:
- `source`: El resultado fuente a evaluar
- `func`: Función que convierte detalles de error en un valor válido

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios
- Si `source` es fallido sin excepción: Ejecuta `func` y retorna el resultado como válido
- Si `source` es fallido con excepción: Retorna `source` sin cambios (preserva la excepción)

**Ejemplo Básico**:
```csharp
var result = GetUserFromCache(userId)
    .MapIfFailWithoutException(errors => 
        new User { Id = userId, Name = "Guest User" }); // Usuario por defecto

// Si el cache falla por timeout (sin excepción): retorna usuario por defecto
// Si el cache falla por excepción de red: preserva el error original
// Si hay usuario en cache: retorna el usuario original
```

### Versiones Asíncronas

#### `MapIfFailWithoutExceptionAsync` - Función Asíncrona
```csharp
public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, Task<T>> funcAsync)

public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<MlErrorsDetails, Task<T>> funcAsync)

public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<MlErrorsDetails, T> func)
```

**Ejemplo Asíncrono**:
```csharp
var result = await GetDataFromPrimarySourceAsync(request)
    .MapIfFailWithoutExceptionAsync(async errors => 
        await GetDataFromFallbackSourceAsync(request));

// Solo usa fallback si el error primario no fue una excepción
```

---

## Métodos TryMapIfFailWithoutException

### `TryMapIfFailWithoutException<T>()` - Mapeo Seguro

**Propósito**: Versión segura que captura excepciones durante la transformación

```csharp
public static MlResult<T> TryMapIfFailWithoutException<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, T> func,
    Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryMapIfFailWithoutException<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, T> func,
    string errorMessage = null!)
```

**Parámetros**:
- `source`: El resultado fuente
- `func`: Función de transformación que puede lanzar excepciones
- `errorMessageBuilder`: Constructor de mensaje de error para excepciones capturadas
- `errorMessage`: Mensaje de error fijo para excepciones

**Comportamiento**:
- Si `func` lanza una excepción: Captura la excepción y retorna un error con el mensaje construido
- Caso contrario: Comportamiento idéntico a `MapIfFailWithoutException`

**Ejemplo con Manejo de Excepciones**:
```csharp
var result = LoadConfiguration(configPath)
    .TryMapIfFailWithoutException(
        errors => ParseDefaultConfiguration(), // Puede lanzar excepción
        ex => $"Failed to load default configuration: {ex.Message}"
    );

// Si ParseDefaultConfiguration() falla, se captura y convierte en error
```

### Versiones Asíncronas de TryMap

```csharp
public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, Task<T>> funcAsync,
    Func<Exception, string> errorMessageBuilder)

// Múltiples sobrecargas para diferentes combinaciones de async/sync
```

---

## Variantes de Transformación

### `MapIfFailWithoutException<T, TReturn>()` - Mapeo con Cambio de Tipo

**Propósito**: Permite transformar tanto valores válidos como errores sin excepción, cambiando el tipo del resultado

```csharp
public static MlResult<TReturn> MapIfFailWithoutException<T, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> funcValid,
    Func<MlErrorsDetails, TReturn> funcFail)
```

**Parámetros**:
- `source`: El resultado fuente de tipo `T`
- `funcValid`: Función para transformar valores válidos de `T` a `TReturn`
- `funcFail`: Función para transformar errores sin excepción a `TReturn`

**Comportamiento**:
- Si `source` es válido: Ejecuta `funcValid` y retorna el resultado como válido
- Si `source` es fallido sin excepción: Ejecuta `funcFail` y retorna el resultado como válido
- Si `source` es fallido con excepción: Retorna el error original (sin cambio de tipo)

**Ejemplo de Transformación de Tipos**:
```csharp
// Transformar MlResult<User> a MlResult<UserDto>
var userDto = GetUser(userId)
    .MapIfFailWithoutException(
        validUser => new UserDto 
        { 
            Id = validUser.Id, 
            Name = validUser.FullName 
        },
        errorDetails => new UserDto 
        { 
            Id = userId, 
            Name = "Unknown User",
            IsDefault = true 
        }
    );

// Si GetUser es exitoso: convierte User a UserDto
// Si GetUser falla sin excepción: crea UserDto por defecto
// Si GetUser falla con excepción: preserva el error original
```

### Versiones Asíncronas de Transformación

#### Todas las Combinaciones de Async
```csharp
// Ambas funciones asíncronas
public static async Task<MlResult<TReturn>> MapIfFailWithoutExceptionAsync<T, TReturn>(
    this MlResult<T> source,
    Func<T, Task<TReturn>> funcValidAsync,
    Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)

// Fuente asíncrona con funciones asíncronas
public static async Task<MlResult<TReturn>> MapIfFailWithoutExceptionAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task<TReturn>> funcValidAsync,
    Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)

// Combinaciones mixtas de sync/async para funcValid y funcFail
// ... (múltiples sobrecargas)
```

### Versiones TryMap con Transformación

```csharp
public static MlResult<TReturn> TryMapIfFailWithoutException<T, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> funcValid,
    Func<MlErrorsDetails, TReturn> funcFail,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: Versión segura de la transformación que captura excepciones en ambas funciones

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Operación | Fuente | Función | Método |
|-----------|---------|---------|---------|
| **MapIfFail** | `MlResult<T>` | `MlErrorsDetails → T` | `MapIfFailWithoutException` |
| **MapIfFail** | `MlResult<T>` | `MlErrorsDetails → Task<T>` | `MapIfFailWithoutExceptionAsync` |
| **MapIfFail** | `Task<MlResult<T>>` | `MlErrorsDetails → Task<T>` | `MapIfFailWithoutExceptionAsync` |
| **MapIfFail** | `Task<MlResult<T>>` | `MlErrorsDetails → T` | `MapIfFailWithoutExceptionAsync` |
| **MapDual** | `MlResult<T>` | `T → U`, `MlErrorsDetails → U` | `MapIfFailWithoutException<T,U>` |
| **MapDual** | `MlResult<T>` | `T → Task<U>`, `MlErrorsDetails → Task<U>` | `MapIfFailWithoutExceptionAsync<T,U>` |

Todas las variantes tienen sus correspondientes versiones `Try*` para manejo seguro de excepciones.

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Cache con Fallback Selectivo

```csharp
public class SmartCacheService
{
    private readonly IPrimaryCache _primaryCache;
    private readonly ISecondaryCache _secondaryCache;
    private readonly IDataSource _dataSource;
    private readonly ILogger _logger;
    
    public SmartCacheService(
        IPrimaryCache primaryCache,
        ISecondaryCache secondaryCache,
        IDataSource dataSource,
        ILogger logger)
    {
        _primaryCache = primaryCache;
        _secondaryCache = secondaryCache;
        _dataSource = dataSource;
        _logger = logger;
    }
    
    public async Task<MlResult<CachedData>> GetDataWithIntelligentFallbackAsync(string key)
    {
        return await GetFromPrimaryCacheAsync(key)
            .MapIfFailWithoutExceptionAsync(async primaryErrors => 
            {
                // Solo intentar cache secundario si el primario falló sin excepción
                _logger.LogInformation($"Primary cache miss for key {key}, trying secondary cache");
                
                var secondaryResult = await GetFromSecondaryCacheAsync(key);
                if (secondaryResult.IsValid)
                {
                    // Actualizar cache primario en background
                    _ = Task.Run(async () => 
                    {
                        try 
                        { 
                            await _primaryCache.SetAsync(key, secondaryResult.Value); 
                        }
                        catch (Exception ex) 
                        { 
                            _logger.LogWarning($"Failed to update primary cache: {ex.Message}"); 
                        }
                    });
                    
                    return secondaryResult.Value;
                }
                
                // Si cache secundario también falla, cargar desde fuente de datos
                _logger.LogInformation($"Secondary cache also failed for key {key}, loading from data source");
                var sourceData = await LoadFromDataSourceAsync(key);
                
                // Actualizar ambos caches en background
                _ = Task.Run(async () => await UpdateCachesAsync(key, sourceData));
                
                return sourceData;
            });
    }
    
    private async Task<MlResult<CachedData>> GetFromPrimaryCacheAsync(string key)
    {
        try
        {
            var data = await _primaryCache.GetAsync<CachedData>(key);
            
            if (data == null)
            {
                // Cache miss sin excepción - candidato para fallback
                return MlResult<CachedData>.Fail($"Data not found in primary cache for key: {key}");
            }
            
            if (data.IsExpired)
            {
                // Datos expirados sin excepción - candidato para fallback
                return MlResult<CachedData>.Fail($"Data expired in primary cache for key: {key}");
            }
            
            return MlResult<CachedData>.Valid(data);
        }
        catch (CacheConnectionException ex)
        {
            // Excepción de conexión - NO debe usar fallback, escalar el error
            return MlResult<CachedData>.FailWithException(
                $"Primary cache connection failed for key {key}", ex);
        }
        catch (CacheTimeoutException ex)
        {
            // Timeout - NO debe usar fallback, puede indicar problema sistémico
            return MlResult<CachedData>.FailWithException(
                $"Primary cache timeout for key {key}", ex);
        }
    }
    
    private async Task<MlResult<CachedData>> GetFromSecondaryCacheAsync(string key)
    {
        try
        {
            var data = await _secondaryCache.GetAsync<CachedData>(key);
            
            return data != null && !data.IsExpired
                ? MlResult<CachedData>.Valid(data)
                : MlResult<CachedData>.Fail($"Data not found or expired in secondary cache for key: {key}");
        }
        catch (Exception ex)
        {
            return MlResult<CachedData>.FailWithException(
                $"Secondary cache error for key {key}", ex);
        }
    }
    
    private async Task<CachedData> LoadFromDataSourceAsync(string key)
    {
        var data = await _dataSource.LoadAsync(key);
        return new CachedData
        {
            Key = key,
            Data = data,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
    
    private async Task UpdateCachesAsync(string key, CachedData data)
    {
        try
        {
            await Task.WhenAll(
                _primaryCache.SetAsync(key, data),
                _secondaryCache.SetAsync(key, data)
            );
            
            _logger.LogInformation($"Updated both caches for key {key}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to update caches for key {key}: {ex.Message}");
        }
    }
}

// Clases de apoyo
public class CachedData
{
    public string Key { get; set; }
    public object Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}

public class CacheConnectionException : Exception
{
    public CacheConnectionException(string message) : base(message) { }
    public CacheConnectionException(string message, Exception innerException) : base(message, innerException) { }
}

public class CacheTimeoutException : Exception
{
    public CacheTimeoutException(string message) : base(message) { }
    public CacheTimeoutException(string message, Exception innerException) : base(message, innerException) { }
}

public interface IPrimaryCache
{
    Task<T> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value) where T : class;
}

public interface ISecondaryCache
{
    Task<T> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value) where T : class;
}

public interface IDataSource
{
    Task<object> LoadAsync(string key);
}
```

### Ejemplo 2: Sistema de Configuración con Degradación Elegante

```csharp
public class ConfigurationService
{
    private readonly IRemoteConfigService _remoteConfig;
    private readonly ILocalConfigService _localConfig;
    private readonly IDefaultConfigProvider _defaultConfig;
    private readonly ILogger _logger;
    
    public ConfigurationService(
        IRemoteConfigService remoteConfig,
        ILocalConfigService localConfig,
        IDefaultConfigProvider defaultConfig,
        ILogger logger)
    {
        _remoteConfig = remoteConfig;
        _localConfig = localConfig;
        _defaultConfig = defaultConfig;
        _logger = logger;
    }
    
    public async Task<MlResult<ApplicationConfig>> LoadConfigurationAsync(string environment)
    {
        return await LoadRemoteConfigurationAsync(environment)
            .TryMapIfFailWithoutExceptionAsync(
                async remoteErrors => 
                {
                    _logger.LogWarning($"Remote config failed for {environment}: {remoteErrors.FirstErrorMessage}");
                    
                    // Intentar configuración local solo si remota falló sin excepción
                    var localResult = await LoadLocalConfigurationAsync(environment);
                    if (localResult.IsValid)
                    {
                        _logger.LogInformation($"Using local configuration for {environment}");
                        return localResult.Value;
                    }
                    
                    // Si local también falla, usar configuración por defecto
                    _logger.LogWarning($"Local config also failed for {environment}, using defaults");
                    var defaultConfig = await LoadDefaultConfigurationAsync(environment);
                    
                    return defaultConfig;
                },
                ex => $"Failed to load fallback configuration: {ex.Message}"
            );
    }
    
    public async Task<MlResult<FeatureFlags>> GetFeatureFlagsAsync(string userId)
    {
        return await GetUserSpecificFlagsAsync(userId)
            .MapIfFailWithoutExceptionAsync(
                userErrors => GetDefaultFeatureFlagsAsync(),
                validFlags => validFlags, // Mantener flags específicos si existen
                ex => $"Failed to get feature flags: {ex.Message}"
            );
    }
    
    private async Task<MlResult<ApplicationConfig>> LoadRemoteConfigurationAsync(string environment)
    {
        try
        {
            var config = await _remoteConfig.GetConfigurationAsync(environment);
            
            if (config == null)
            {
                // No hay configuración para este entorno - fallback permitido
                return MlResult<ApplicationConfig>.Fail(
                    $"No remote configuration found for environment: {environment}");
            }
            
            if (!IsValidConfiguration(config))
            {
                // Configuración inválida - fallback permitido
                return MlResult<ApplicationConfig>.Fail(
                    $"Remote configuration is invalid for environment: {environment}");
            }
            
            return MlResult<ApplicationConfig>.Valid(config);
        }
        catch (RemoteConfigTimeoutException ex)
        {
            // Timeout - NO permitir fallback, puede indicar problema de red
            return MlResult<ApplicationConfig>.FailWithException(
                $"Remote configuration timeout for environment {environment}", ex);
        }
        catch (RemoteConfigAuthenticationException ex)
        {
            // Error de autenticación - NO permitir fallback, requiere intervención
            return MlResult<ApplicationConfig>.FailWithException(
                $"Remote configuration authentication failed for environment {environment}", ex);
        }
        catch (Exception ex)
        {
            // Otros errores - NO permitir fallback por seguridad
            return MlResult<ApplicationConfig>.FailWithException(
                $"Unexpected error loading remote configuration for environment {environment}", ex);
        }
    }
    
    private async Task<MlResult<ApplicationConfig>> LoadLocalConfigurationAsync(string environment)
    {
        try
        {
            var config = await _localConfig.GetConfigurationAsync(environment);
            
            return config != null && IsValidConfiguration(config)
                ? MlResult<ApplicationConfig>.Valid(config)
                : MlResult<ApplicationConfig>.Fail($"Local configuration not found or invalid for environment: {environment}");
        }
        catch (Exception ex)
        {
            return MlResult<ApplicationConfig>.FailWithException(
                $"Error loading local configuration for environment {environment}", ex);
        }
    }
    
    private async Task<ApplicationConfig> LoadDefaultConfigurationAsync(string environment)
    {
        var defaultConfig = await _defaultConfig.GetDefaultConfigurationAsync(environment);
        
        // Marcar como configuración degradada
        defaultConfig.IsDegradedMode = true;
        defaultConfig.DegradedReason = "Using default configuration due to remote and local failures";
        
        return defaultConfig;
    }
    
    private async Task<MlResult<FeatureFlags>> GetUserSpecificFlagsAsync(string userId)
    {
        try
        {
            var flags = await _remoteConfig.GetFeatureFlagsAsync(userId);
            
            return flags != null
                ? MlResult<FeatureFlags>.Valid(flags)
                : MlResult<FeatureFlags>.Fail($"No specific feature flags found for user: {userId}");
        }
        catch (Exception ex)
        {
            return MlResult<FeatureFlags>.FailWithException(
                $"Error getting user-specific feature flags for user {userId}", ex);
        }
    }
    
    private async Task<FeatureFlags> GetDefaultFeatureFlagsAsync()
    {
        return await _defaultConfig.GetDefaultFeatureFlagsAsync();
    }
    
    private bool IsValidConfiguration(ApplicationConfig config)
    {
        return config.DatabaseConnectionString != null &&
               config.ApiEndpoints?.Any() == true &&
               config.TimeoutSettings != null;
    }
}

// Clases de apoyo y excepciones
public class ApplicationConfig
{
    public string DatabaseConnectionString { get; set; }
    public Dictionary<string, string> ApiEndpoints { get; set; }
    public TimeoutSettings TimeoutSettings { get; set; }
    public bool IsDegradedMode { get; set; }
    public string DegradedReason { get; set; }
}

public class FeatureFlags
{
    public Dictionary<string, bool> Flags { get; set; }
    public DateTime LoadedAt { get; set; }
    public string Source { get; set; } // "user-specific", "default", etc.
}

public class TimeoutSettings
{
    public int DatabaseTimeoutSeconds { get; set; }
    public int ApiTimeoutSeconds { get; set; }
    public int CacheTimeoutSeconds { get; set; }
}

public class RemoteConfigTimeoutException : Exception
{
    public RemoteConfigTimeoutException(string message) : base(message) { }
    public RemoteConfigTimeoutException(string message, Exception innerException) : base(message, innerException) { }
}

public class RemoteConfigAuthenticationException : Exception
{
    public RemoteConfigAuthenticationException(string message) : base(message) { }
    public RemoteConfigAuthenticationException(string message, Exception innerException) : base(message, innerException) { }
}

public interface IRemoteConfigService
{
    Task<ApplicationConfig> GetConfigurationAsync(string environment);
    Task<FeatureFlags> GetFeatureFlagsAsync(string userId);
}

public interface ILocalConfigService
{
    Task<ApplicationConfig> GetConfigurationAsync(string environment);
}

public interface IDefaultConfigProvider
{
    Task<ApplicationConfig> GetDefaultConfigurationAsync(string environment);
    Task<FeatureFlags> GetDefaultFeatureFlagsAsync();
}
```

### Ejemplo 3: Sistema de Procesamiento de Datos con Recuperación de Errores

```csharp
public class DataProcessingPipeline
{
    private readonly IDataValidator _validator;
    private readonly IDataProcessor _processor;
    private readonly IDataRepository _repository;
    private readonly IDataSanitizer _sanitizer;
    private readonly ILogger _logger;
    
    public DataProcessingPipeline(
        IDataValidator validator,
        IDataProcessor processor,
        IDataRepository repository,
        IDataSanitizer sanitizer,
        ILogger logger)
    {
        _validator = validator;
        _processor = processor;
        _repository = repository;
        _sanitizer = sanitizer;
        _logger = logger;
    }
    
    public async Task<MlResult<ProcessedData>> ProcessDataWithRecoveryAsync(RawData inputData)
    {
        var processingId = Guid.NewGuid().ToString();
        
        return await ValidateDataAsync(inputData)
            .TryMapIfFailWithoutExceptionAsync(
                async validationErrors => 
                {
                    _logger.LogWarning($"Data validation failed for {processingId}, attempting sanitization");
                    
                    // Solo intentar sanitización si la validación falló sin excepción
                    var sanitizedData = await _sanitizer.SanitizeDataAsync(inputData);
                    var revalidationResult = await ValidateDataAsync(sanitizedData);
                    
                    if (revalidationResult.IsValid)
                    {
                        _logger.LogInformation($"Data sanitization successful for {processingId}");
                        return revalidationResult.Value;
                    }
                    
                    // Si la sanitización no ayuda, crear datos por defecto
                    _logger.LogWarning($"Sanitization failed for {processingId}, using default values");
                    return CreateDefaultValidData(inputData);
                },
                ex => $"Data recovery failed for processing {processingId}: {ex.Message}"
            )
            .BindAsync(async validData => await ProcessValidDataAsync(validData, processingId))
            .TryMapIfFailWithoutExceptionAsync(
                async processingErrors => 
                {
                    _logger.LogWarning($"Data processing failed for {processingId}, attempting simplified processing");
                    
                    // Solo intentar procesamiento simplificado si falló sin excepción
                    var simplifiedResult = await ProcessWithSimplifiedRulesAsync(validData, processingId);
                    return simplifiedResult;
                },
                ex => $"Simplified processing failed for {processingId}: {ex.Message}"
            )
            .BindAsync(async processedData => await SaveProcessedDataAsync(processedData, processingId));
    }
    
    public async Task<MlResult<DataReport>> GenerateReportWithFallbackAsync(ReportRequest request)
    {
        return await GenerateDetailedReportAsync(request)
            .MapIfFailWithoutException(
                successReport => successReport,
                async detailedErrors => 
                {
                    _logger.LogInformation($"Detailed report generation failed, creating summary report");
                    
                    // Solo generar reporte resumido si el detallado falló sin excepción
                    var summaryReport = await GenerateSummaryReportAsync(request);
                    return summaryReport;
                },
                ex => $"Report generation fallback failed: {ex.Message}"
            );
    }
    
    private async Task<MlResult<RawData>> ValidateDataAsync(RawData data)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(data);
            
            if (!validationResult.IsValid)
            {
                // Errores de validación - candidatos para recuperación
                return MlResult<RawData>.Fail(validationResult.Errors.ToArray());
            }
            
            return MlResult<RawData>.Valid(data);
        }
        catch (ValidationServiceException ex)
        {
            // Error del servicio de validación - NO recuperable
            return MlResult<RawData>.FailWithException(
                "Validation service error", ex);
        }
        catch (Exception ex)
        {
            // Errores inesperados - NO recuperables
            return MlResult<RawData>.FailWithException(
                "Unexpected validation error", ex);
        }
    }
    
    private async Task<MlResult<ProcessedData>> ProcessValidDataAsync(RawData validData, string processingId)
    {
        try
        {
            var processedData = await _processor.ProcessAsync(validData);
            
            if (processedData.HasErrors)
            {
                // Errores de procesamiento - pueden ser recuperables
                return MlResult<ProcessedData>.Fail(
                    $"Processing completed with errors for {processingId}",
                    processedData.Errors.ToArray());
            }
            
            return MlResult<ProcessedData>.Valid(processedData);
        }
        catch (ProcessingCapacityException ex)
        {
            // Sistema sobrecargado - NO recuperable inmediatamente
            return MlResult<ProcessedData>.FailWithException(
                $"Processing system overloaded for {processingId}", ex);
        }
        catch (ProcessingTimeoutException ex)
        {
            // Timeout - NO recuperable, puede indicar datos muy complejos
            return MlResult<ProcessedData>.FailWithException(
                $"Processing timeout for {processingId}", ex);
        }
        catch (Exception ex)
        {
            // Errores inesperados - NO recuperables
            return MlResult<ProcessedData>.FailWithException(
                $"Unexpected processing error for {processingId}", ex);
        }
    }
    
    private async Task<ProcessedData> ProcessWithSimplifiedRulesAsync(RawData validData, string processingId)
    {
        // Procesamiento con reglas simplificadas para datos problemáticos
        var simplifiedProcessor = _processor.CreateSimplifiedProcessor();
        var result = await simplifiedProcessor.ProcessAsync(validData);
        
        result.IsSimplified = true;
        result.SimplificationReason = "Used simplified processing due to standard processing failures";
        
        _logger.LogInformation($"Applied simplified processing for {processingId}");
        
        return result;
    }
    
    private RawData CreateDefaultValidData(RawData originalData)
    {
        // Crear datos válidos mínimos basados en los originales
        return new RawData
        {
            Id = originalData.Id,
            Timestamp = originalData.Timestamp,
            Data = _sanitizer.CreateMinimalValidData(originalData.Data),
            Source = originalData.Source,
            IsDefault = true,
            DefaultReason = "Created default data due to validation failures"
        };
    }
    
    private async Task<MlResult<ProcessedData>> SaveProcessedDataAsync(ProcessedData data, string processingId)
    {
        try
        {
            await _repository.SaveAsync(data);
            _logger.LogInformation($"Successfully saved processed data for {processingId}");
            return MlResult<ProcessedData>.Valid(data);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedData>.FailWithException(
                $"Failed to save processed data for {processingId}", ex);
        }
    }
    
    private async Task<MlResult<DataReport>> GenerateDetailedReportAsync(ReportRequest request)
    {
        try
        {
            var report = await _processor.GenerateDetailedReportAsync(request);
            
            return report.IsComplete
                ? MlResult<DataReport>.Valid(report)
                : MlResult<DataReport>.Fail("Detailed report is incomplete");
        }
        catch (Exception ex)
        {
            return MlResult<DataReport>.FailWithException(
                "Detailed report generation failed", ex);
        }
    }
    
    private async Task<DataReport> GenerateSummaryReportAsync(ReportRequest request)
    {
        var summaryData = await _processor.GenerateSummaryDataAsync(request);
        
        return new DataReport
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Summary Report - {request.Title}",
            Data = summaryData,
            IsSummary = true,
            GeneratedAt = DateTime.UtcNow,
            SummaryReason = "Generated summary due to detailed report failure"
        };
    }
}

// Clases de apoyo y excepciones
public class RawData
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    public object Data { get; set; }
    public string Source { get; set; }
    public bool IsDefault { get; set; }
    public string DefaultReason { get; set; }
}

public class ProcessedData
{
    public string Id { get; set; }
    public object ProcessedResult { get; set; }
    public bool HasErrors { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool IsSimplified { get; set; }
    public string SimplificationReason { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class DataReport
{
    public string Id { get; set; }
    public string Title { get; set; }
    public object Data { get; set; }
    public bool IsComplete { get; set; }
    public bool IsSummary { get; set; }
    public string SummaryReason { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class ReportRequest
{
    public string Title { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<string> DataSources { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ValidationServiceException : Exception
{
    public ValidationServiceException(string message) : base(message) { }
    public ValidationServiceException(string message, Exception innerException) : base(message, innerException) { }
}

public class ProcessingCapacityException : Exception
{
    public ProcessingCapacityException(string message) : base(message) { }
    public ProcessingCapacityException(string message, Exception innerException) : base(message, innerException) { }
}

public class ProcessingTimeoutException : Exception
{
    public ProcessingTimeoutException(string message) : base(message) { }
    public ProcessingTimeoutException(string message, Exception innerException) : base(message, innerException) { }
}

// Interfaces de servicios
public interface IDataValidator
{
    Task<ValidationResult> ValidateAsync(RawData data);
}

public interface IDataProcessor
{
    Task<ProcessedData> ProcessAsync(RawData data);
    IDataProcessor CreateSimplifiedProcessor();
    Task<DataReport> GenerateDetailedReportAsync(ReportRequest request);
    Task<object> GenerateSummaryDataAsync(ReportRequest request);
}

public interface IDataRepository
{
    Task SaveAsync(ProcessedData data);
}

public interface IDataSanitizer
{
    Task<RawData> SanitizeDataAsync(RawData data);
    object CreateMinimalValidData(object originalData);
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar MapIfFailWithoutException

```csharp
// ✅ Correcto: Usar para errores recuperables que no indican problemas sistémicos
var result = LoadFromPrimarySource(request)
    .MapIfFailWithoutException(errors => LoadFromBackupSource(request));

// Los errores sin excepción (cache miss, data not found) permiten fallback
// Los errores con excepción (connection timeout, auth failure) se escalán

// ✅ Correcto: Degradación elegante de funcionalidad
var userProfile = GetFullUserProfile(userId)
    .MapIfFailWithoutException(errors => CreateBasicUserProfile(userId));

// Si la carga completa falla sin excepción, usar perfil básico
// Si falla por excepción (DB down), escalar el error

// ❌ Incorrecto: Usar para todos los errores sin discriminar
var result = CriticalDatabaseOperation()
    .MapIfFailWithoutException(errors => IgnoreAndContinue()); // Peligroso!
```

### 2. Diferenciación Entre Errores Recuperables y No Recuperables

```csharp
// ✅ Correcto: Clasificar apropiadamente los errores
public async Task<MlResult<Data>> LoadDataWithFallback(string key)
{
    try
    {
        var data = await _primaryService.LoadAsync(key);
        return data != null 
            ? MlResult<Data>.Valid(data)
            : MlResult<Data>.Fail("Data not found"); // SIN excepción - recuperable
    }
    catch (ServiceTimeoutException ex)
    {
        // CON excepción - NO recuperable
        return MlResult<Data>.FailWithException("Service timeout", ex);
    }
    catch (AuthenticationException ex)
    {
        // CON excepción - NO recuperable
        return MlResult<Data>.FailWithException("Authentication failed", ex);
    }
}

// Uso correcto del método
var result = LoadDataWithFallback(key)
    .MapIfFailWithoutException(errors => LoadFromCache(key)); // Solo para "data not found"
```

### 3. Patrones de Recuperación Gradual

```csharp
// ✅ Correcto: Múltiples niveles de fallback
public async Task<MlResult<ServiceResponse>> CallServiceWithFallback(ServiceRequest request)
{
    return await CallPrimaryService(request)
        .MapIfFailWithoutExceptionAsync(async primaryErrors => 
            await CallSecondaryService(request))
        .MapIfFailWithoutExceptionAsync(async secondaryErrors => 
            await CallLocalService(request))
        .MapIfFailWithoutExceptionAsync(async localErrors => 
            CreateDefaultResponse(request));
}

// Cada nivel solo se ejecuta si el anterior falló sin excepción
```

### 4. Uso de TryMap para Operaciones de Recuperación Riesgosas

```csharp
// ✅ Correcto: Usar TryMap cuando la recuperación puede fallar
var result = LoadConfiguration()
    .TryMapIfFailWithoutException(
        errors => ParseAlternativeConfigFormat(), // Puede lanzar excepción
        ex => $"Failed to parse alternative config: {ex.Message}"
    );

// ✅ Correcto: Logging en recuperación
var result = ProcessData(input)
    .TryMapIfFailWithoutException(
        errors => 
        {
            _logger.LogWarning($"Processing failed, attempting recovery: {errors.FirstErrorMessage}");
            return RecoverFromFailure(input); // Puede fallar
        },
        ex => $"Recovery attempt failed: {ex.Message}"
    );
```

### 5. Transformación de Tipos con Recuperación

```csharp
// ✅ Correcto: Mapeo con diferentes estrategias para éxito y fallo
var dto = GetCompleteUserData(userId)
    .MapIfFailWithoutException(
        validUser => new UserDto 
        { 
            Id = validUser.Id,
            FullName = validUser.FullName,
            IsComplete = true 
        },
        errors => new UserDto 
        { 
            Id = userId,
            FullName = "Unknown User",
            IsComplete = false,
            LoadError = errors.FirstErrorMessage 
        }
    );

// Siempre retorna UserDto, pero con información diferente según el resultado
```

### 6. Gestión de Recursos en Recuperación

```csharp
// ✅ Correcto: Limpieza apropiada en recuperación
public async Task<MlResult<ProcessResult>> ProcessWithRecovery(ProcessInput input)
{
    var primaryResources = new ResourcePool();
    
    return await ProcessWithPrimaryResources(input, primaryResources)
        .TryMapIfFailWithoutExceptionAsync(
            async errors => 
            {
                // Liberar recursos primarios antes de usar alternativos
                await primaryResources.DisposeAsync();
                
                var alternativeResources = new AlternativeResourcePool();
                return await ProcessWithAlternativeResources(input, alternativeResources);
            },
            ex => $"Recovery processing failed: {ex.Message}"
        );
}
```

---

## Consideraciones de Rendimiento

### Evaluación de Excepciones

- `GetDetailException()` tiene overhead mínimo pero se ejecuta en cada evaluación
- Considerar cache de evaluación de excepciones para pipelines muy frecuentes
- Las versiones asíncronas mantienen el contexto apropiadamente

### Estrategias de Fallback

- Los fallbacks pueden introducir latencia adicional significativa
- Considerar timeouts apropiados para operaciones de recuperación
- Implementar circuit breakers para servicios de fallback

### Transformaciones de Tipos

- Las transformaciones que cambian tipos pueden tener overhead de serialización
- Considerar reutilización de objetos en transformaciones frecuentes
- Las versiones `Try*` tienen overhead adicional de manejo de excepciones

---

## Resumen

Los métodos **`MapIfFailWithoutException`** proporcionan capacidades especializadas para:

- **Recuperación Selectiva**: Solo aplicar fallbacks para errores sin excepciones
- **Preservación de Errores Críticos**: Mantener intactos los errores con excepciones
- **Transformación Condicional**: Convertir tipos con estrategias diferentes para éxito y fallo
- **Degradación Elegante**: Implementar niveles de funcionalidad según disponibilidad de recursos

### Métodos Principales

- **`MapIfFailWithoutException<T>`**: Recuperación simple sin cambio de tipo
- **`MapIfFailWithoutException<T, TReturn>`**: Transformación con funciones separadas para éxito y fallo
- **`TryMapIfFailWithoutException`**: Versiones seguras con captura de excepciones
- **Variantes Asíncronas**: Soporte completo para operaciones asíncronas

### Casos de Uso Ideales

- **Sistemas de Cache** con múltiples niveles de fallback
- **Servicios de Configuración** con degradación elegante
- **Pipelines de Datos** con estrategias de recuperación
- **APIs Resilientes** con respuestas alternativas

La clave está en distinguir apropiadamente entre errores recuperables (sin excepción) y errores que requieren escalamiento (con excepción), aplicando estrategias de recuperación únicamente cuando es seguro hacerlo.