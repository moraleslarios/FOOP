# MlResult Transformations - Conversiones y Transformaciones Seguras

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos de Conversión Basic](#métodos-de-conversión-basic)
4. [Métodos Try - Manejo Seguro de Excepciones](#métodos-try---manejo-seguro-de-excepciones)
5. [Conversiones de Estado](#conversiones-de-estado)
6. [Variantes Asíncronas](#variantes-asíncronas)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Comparación con Otros Métodos](#comparación-con-otros-métodos)

---

## Introducción

Los métodos de **Transformations** proporcionan un conjunto completo de herramientas para **convertir funciones, valores y operaciones regulares en el ecosistema MlResult**. Estos métodos son fundamentales para integrar código legacy, manejar excepciones de forma controlada y crear puentes entre el mundo imperativo y el funcional.

### Propósito Principal

- **Integración Legacy**: Convertir funciones existentes en MlResult
- **Manejo Seguro de Excepciones**: Capturar y convertir excepciones en errores tipados
- **Conversiones de Estado**: Transformar entre diferentes estados de MlResult
- **Puentes Funcionales**: Conectar paradigmas imperativos y funcionales

---

## Análisis de los Métodos

### Filosofía de Transformations

```
Función Regular + Try → ToMlResult → MlResult<T>
       ↓             ↓      ↓           ↓
   func(input) → Success → Valid(result)
       ↓             ↓      ↓           ↓
   func(input) → Exception → Fail(error)
```

### Características Principales

1. **Conversión Automática**: Transformar funciones regulares en MlResult
2. **Captura de Excepciones**: Manejo seguro y tipado de errores
3. **Flexibilidad de Tipos**: Soporte para múltiples tipos de entrada y salida
4. **Mensajes Personalizados**: Constructores de mensajes de error flexibles
5. **Soporte Async Completo**: Versiones asíncronas para todas las operaciones

---

## Métodos de Conversión Basic

### 1. ToMlResult - Conversión Simple

**Propósito**: Convertir una función regular en MlResult sin manejo de excepciones

```csharp
public static MlResult<TReturn> ToMlResult<T, TReturn>(
    this Func<T, TReturn> source, 
    T value)
```

**Ejemplo Básico**:
```csharp
Func<string, int> parseLength = s => s.Length;
var result = parseLength.ToMlResult("Hello World");
// Resultado: MlResult<int>.Valid(11)
```

### 2. ToMlResultAsync - Conversión Asíncrona Simple

**Propósito**: Convertir funciones async en MlResult

```csharp
public static async Task<MlResult<TReturn>> ToMlResultAsync<T, TReturn>(
    this Func<T, Task<TReturn>> sourceAsync, 
    T value)
```

### 3. Conversiones de Estado

**Múltiples métodos para crear MlResult desde diferentes tipos**:

```csharp
// Crear Valid
public static MlResult<T> ToMlResultValid<T>(this T source)

// Crear Fail desde diferentes fuentes
public static MlResult<T> ToMlResultFail<T>(this string source)
public static MlResult<T> ToMlResultFail<T>(this MlError source)
public static MlResult<T> ToMlResultFail<T>(this MlErrorsDetails source)
public static MlResult<T> ToMlResultFail<T>(this List<string> source)
public static MlResult<T> ToMlResultFail<T>(this IEnumerable<MlError> source)
```

---

## Métodos Try - Manejo Seguro de Excepciones

### 1. TryToMlResult - Funciones con Parámetros

**Propósito**: Ejecutar funciones con manejo automático de excepciones

```csharp
// Con mensaje de error simple
public static MlResult<TReturn> TryToMlResult<T, TReturn>(
    this Func<T, TReturn> source, 
    T value,
    string exceptionAditionalMessage = null)

// Con constructor de mensaje personalizado
public static MlResult<TReturn> TryToMlResult<T, TReturn>(
    this Func<T, TReturn> source, 
    T value,
    Func<Exception, string> errorMessageBuilder)
```

**Ejemplo**:
```csharp
Func<string, int> parseNumber = s => int.Parse(s);

var result1 = parseNumber.TryToMlResult("123", "Failed to parse number");
// Resultado: MlResult<int>.Valid(123)

var result2 = parseNumber.TryToMlResult("abc", ex => $"Parse error: {ex.Message}");
// Resultado: MlResult<int>.Fail("Parse error: Input string was not in a correct format.")
```

### 2. TryToMlResult - Funciones sin Parámetros

**Propósito**: Ejecutar funciones sin parámetros con protección

```csharp
public static MlResult<T> TryToMlResult<T>(
    this Func<T> source, 
    Func<Exception, string> errorMessageBuilder = null)

public static MlResult<T> TryToMlResult<T>(
    this Func<MlResult<T>> source, 
    Func<Exception, string> errorMessageBuilder = null)
```

### 3. TryToMlResult - Acciones

**Propósito**: Ejecutar acciones y retornar el valor si es exitoso

```csharp
public static MlResult<T> TryToMlResult<T>(
    this Action<T> source, 
    T value,
    Func<Exception, string> messageBuilder = null)
```

**Ejemplo**:
```csharp
Action<List<int>> sortList = list => list.Sort();
var numbers = new List<int> { 3, 1, 4, 1, 5 };

var result = sortList.TryToMlResult(numbers, ex => $"Sort failed: {ex.Message}");
// Resultado: MlResult<List<int>>.Valid([1, 1, 3, 4, 5])
```

---

## Conversiones de Estado

### 1. ToMlResultFail - Conversión de Error de Tipo

**Propósito**: Convertir errores de un tipo MlResult a otro

```csharp
public static MlResult<TReturn> ToMlResultFail<T, TReturn>(this MlResult<T> source)
```

**Funcionamiento**:
```csharp
var userResult = MlResult<User>.Fail("User not found");
var orderResult = userResult.ToMlResultFail<User, Order>();
// Resultado: MlResult<Order>.Fail("User not found")
```

### 2. Conversiones con Contexto de Error

**Soporte para errores con contexto adicional**:

```csharp
// Con tuplas para contexto adicional
public static MlResult<T> ToMlResultFail<T>(
    this (string, Dictionary<string, object>) source)

public static MlResult<T> ToMlResultFail<T>(
    this (MlError, Dictionary<string, object>) source)
```

---

## Variantes Asíncronas

### Soporte Completo Async/Await

```csharp
// TryToMlResultAsync para funciones async
public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(
    this Func<T, Task<TReturn>> sourceAsync, 
    T value,
    string exceptionAditionalMessage = null)

// Para funciones que retornan Task<MlResult<T>>
public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(
    this Func<T, Task<MlResult<TReturn>>> sourceAsync, 
    T value,
    Func<Exception, string> errorMessageBuilder)

// Para acciones async
public static async Task<MlResult<T>> TryToMlResultAsync<T>(
    this Func<T, Task> sourceAsync,
    T returnValue,
    Func<Exception, string> messageBuilder = null)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Integración con APIs Legacy

```csharp
public class LegacyApiIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly ILegacyDatabaseService _legacyDb;
    private readonly IFileSystemService _fileSystem;
    
    public async Task<MlResult<CustomerIntegrationResult>> IntegrateCustomerDataAsync(
        CustomerIntegrationRequest request)
    {
        // 1. Convertir función legacy de validación
        Func<string, bool> validateCustomerId = id => 
            !string.IsNullOrEmpty(id) && id.Length == 10 && id.All(char.IsDigit);
        
        var validationResult = validateCustomerId.TryToMlResult(
            request.CustomerId, 
            ex => $"Customer ID validation failed: {ex.Message}");
        
        if (validationResult.IsFailed)
            return validationResult.ToMlResultFail<bool, CustomerIntegrationResult>();
        
        // 2. Integrar con base de datos legacy usando Try
        var customerDataResult = await TryGetLegacyCustomerDataAsync(request.CustomerId);
        if (customerDataResult.IsFailed)
            return customerDataResult.ToMlResultFail<LegacyCustomerData, CustomerIntegrationResult>();
        
        // 3. Procesar datos con transformaciones seguras
        var processedResult = await ProcessCustomerDataSafelyAsync(customerDataResult.Value);
        if (processedResult.IsFailed)
            return processedResult.ToMlResultFail<ProcessedCustomerData, CustomerIntegrationResult>();
        
        // 4. Integrar con API externa
        var externalApiResult = await IntegrateWithExternalApiAsync(processedResult.Value);
        if (externalApiResult.IsFailed)
            return externalApiResult.ToMlResultFail<ExternalApiResponse, CustomerIntegrationResult>();
        
        // 5. Generar archivos de salida
        var fileGenerationResult = await GenerateOutputFilesAsync(
            processedResult.Value, externalApiResult.Value);
        
        return fileGenerationResult.Match(
            valid: files => MlResult<CustomerIntegrationResult>.Valid(new CustomerIntegrationResult
            {
                CustomerId = request.CustomerId,
                LegacyData = customerDataResult.Value,
                ProcessedData = processedResult.Value,
                ExternalApiResponse = externalApiResult.Value,
                GeneratedFiles = files,
                IntegratedAt = DateTime.UtcNow
            }),
            fail: errors => MlResult<CustomerIntegrationResult>.Fail(errors.AllErrors)
        );
    }
    
    private async Task<MlResult<LegacyCustomerData>> TryGetLegacyCustomerDataAsync(string customerId)
    {
        Func<string, Task<LegacyCustomerData>> legacyCall = async id =>
        {
            // Simulación de llamada a sistema legacy que puede fallar
            await Task.Delay(100);
            
            var data = await _legacyDb.GetCustomerByIdAsync(id);
            if (data == null)
                throw new CustomerNotFoundException($"Customer {id} not found in legacy system");
            
            return data;
        };
        
        return await legacyCall.TryToMlResultAsync(
            customerId, 
            ex => $"Legacy database access failed for customer {customerId}: {ex.Message}");
    }
    
    private async Task<MlResult<ProcessedCustomerData>> ProcessCustomerDataSafelyAsync(
        LegacyCustomerData legacyData)
    {
        Func<LegacyCustomerData, Task<ProcessedCustomerData>> processor = async data =>
        {
            // Procesamiento complejo que puede fallar
            await Task.Delay(50);
            
            if (string.IsNullOrEmpty(data.Email))
                throw new ValidationException("Customer email is required");
            
            if (data.AccountBalance < 0)
                throw new BusinessRuleException("Negative account balance not allowed");
            
            return new ProcessedCustomerData
            {
                CustomerId = data.Id,
                NormalizedEmail = data.Email.ToLowerInvariant(),
                AccountStatus = DetermineAccountStatus(data.AccountBalance),
                ProcessedAt = DateTime.UtcNow,
                ValidationFlags = ValidateCustomerData(data)
            };
        };
        
        return await processor.TryToMlResultAsync(
            legacyData,
            ex => $"Customer data processing failed: {ex.Message}");
    }
    
    private async Task<MlResult<ExternalApiResponse>> IntegrateWithExternalApiAsync(
        ProcessedCustomerData customerData)
    {
        Func<ProcessedCustomerData, Task<ExternalApiResponse>> apiCall = async data =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/customers")
            {
                Content = JsonContent.Create(data)
            };
            
            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ExternalApiException($"API call failed: {response.StatusCode} - {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ExternalApiResponse>(responseContent);
        };
        
        return await apiCall.TryToMlResultAsync(
            customerData,
            ex => $"External API integration failed: {ex.Message}");
    }
    
    private async Task<MlResult<GeneratedFiles>> GenerateOutputFilesAsync(
        ProcessedCustomerData customerData, ExternalApiResponse apiResponse)
    {
        Func<Task<GeneratedFiles>> fileGenerator = async () =>
        {
            var files = new List<GeneratedFile>();
            
            // Generar reporte JSON
            var jsonReport = new
            {
                CustomerData = customerData,
                ApiResponse = apiResponse,
                GeneratedAt = DateTime.UtcNow
            };
            
            var jsonPath = await _fileSystem.WriteJsonFileAsync(
                $"customer_{customerData.CustomerId}_report.json", 
                jsonReport);
            
            files.Add(new GeneratedFile 
            { 
                Path = jsonPath, 
                Type = "JSON", 
                Size = new FileInfo(jsonPath).Length 
            });
            
            // Generar reporte CSV
            var csvData = GenerateCsvReport(customerData, apiResponse);
            var csvPath = await _fileSystem.WriteCsvFileAsync(
                $"customer_{customerData.CustomerId}_summary.csv", 
                csvData);
            
            files.Add(new GeneratedFile 
            { 
                Path = csvPath, 
                Type = "CSV", 
                Size = new FileInfo(csvPath).Length 
            });
            
            return new GeneratedFiles { Files = files.ToArray() };
        };
        
        return await fileGenerator.TryToMlResultAsync(
            ex => $"File generation failed: {ex.Message}");
    }
    
    // Métodos auxiliares con conversiones seguras
    public MlResult<ConfigurationSettings> LoadConfigurationSafely(string configPath)
    {
        Func<string, ConfigurationSettings> configLoader = path =>
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");
            
            var json = File.ReadAllText(path);
            var config = JsonSerializer.Deserialize<ConfigurationSettings>(json);
            
            if (config == null)
                throw new InvalidOperationException("Configuration is null after deserialization");
            
            return config;
        };
        
        return configLoader.TryToMlResult(
            configPath, 
            ex => $"Configuration loading failed from {configPath}: {ex.Message}");
    }
    
    public MlResult<ValidationReport> ValidateIntegrationInputSafely(CustomerIntegrationRequest request)
    {
        Action<CustomerIntegrationRequest> validator = req =>
        {
            if (string.IsNullOrEmpty(req.CustomerId))
                throw new ArgumentException("Customer ID is required");
            
            if (req.IntegrationOptions == null)
                throw new ArgumentException("Integration options are required");
            
            if (req.IntegrationOptions.IncludeHistory && req.DateRange == null)
                throw new ArgumentException("Date range is required when including history");
        };
        
        var validationResult = validator.TryToMlResult(
            request, 
            ex => $"Input validation failed: {ex.Message}");
        
        return validationResult.Map(validRequest => new ValidationReport
        {
            IsValid = true,
            ValidatedAt = DateTime.UtcNow,
            InputHash = CalculateHash(validRequest)
        });
    }
    
    // Conversiones de estado para diferentes contextos
    public MlResult<T> ConvertLegacyErrorToMlResult<T>(LegacySystemError legacyError)
    {
        var errorDetails = new Dictionary<string, object>
        {
            { "LegacyErrorCode", legacyError.Code },
            { "LegacySystem", legacyError.SystemName },
            { "LegacyTimestamp", legacyError.Timestamp }
        };
        
        return (legacyError.Message, errorDetails).ToMlResultFail<T>();
    }
    
    public async Task<MlResult<T>> ConvertLegacyErrorWithContextAsync<T>(
        LegacySystemError legacyError, 
        string additionalContext)
    {
        var errorMessage = $"{additionalContext}: {legacyError.Message}";
        var errorDetails = new Dictionary<string, object>
        {
            { "LegacyErrorCode", legacyError.Code },
            { "LegacySystem", legacyError.SystemName },
            { "AdditionalContext", additionalContext },
            { "ConvertedAt", DateTime.UtcNow }
        };
        
        return await (errorMessage, errorDetails).ToMlResultFailAsync<T>();
    }
    
    // Métodos auxiliares
    private string DetermineAccountStatus(decimal balance) =>
        balance switch
        {
            >= 1000 => "Premium",
            >= 100 => "Standard",
            >= 0 => "Basic",
            _ => "Suspended"
        };
    
    private string[] ValidateCustomerData(LegacyCustomerData data) =>
        new[]
        {
            !string.IsNullOrEmpty(data.Email) ? "EmailValid" : "EmailInvalid",
            data.AccountBalance >= 0 ? "BalanceValid" : "BalanceInvalid",
            data.LastActivityDate > DateTime.Now.AddYears(-1) ? "RecentActivity" : "InactiveAccount"
        };
    
    private string GenerateCsvReport(ProcessedCustomerData customerData, ExternalApiResponse apiResponse) =>
        $"CustomerId,Email,Status,Balance,ProcessedAt\n" +
        $"{customerData.CustomerId},{customerData.NormalizedEmail},{customerData.AccountStatus},{apiResponse.Balance},{customerData.ProcessedAt:yyyy-MM-dd}";
    
    private string CalculateHash(object obj) => 
        obj.GetHashCode().ToString("X");
}

// Clases de apoyo
public class CustomerIntegrationRequest
{
    public string CustomerId { get; set; }
    public IntegrationOptions IntegrationOptions { get; set; }
    public DateRange DateRange { get; set; }
}

public class IntegrationOptions
{
    public bool IncludeHistory { get; set; }
    public bool GenerateReports { get; set; }
    public bool ValidateExternally { get; set; }
}

public class DateRange
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class CustomerIntegrationResult
{
    public string CustomerId { get; set; }
    public LegacyCustomerData LegacyData { get; set; }
    public ProcessedCustomerData ProcessedData { get; set; }
    public ExternalApiResponse ExternalApiResponse { get; set; }
    public GeneratedFiles GeneratedFiles { get; set; }
    public DateTime IntegratedAt { get; set; }
}

public class LegacyCustomerData
{
    public string Id { get; set; }
    public string Email { get; set; }
    public decimal AccountBalance { get; set; }
    public DateTime LastActivityDate { get; set; }
}

public class ProcessedCustomerData
{
    public string CustomerId { get; set; }
    public string NormalizedEmail { get; set; }
    public string AccountStatus { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string[] ValidationFlags { get; set; }
}

public class ExternalApiResponse
{
    public string Status { get; set; }
    public decimal Balance { get; set; }
    public string ExternalId { get; set; }
}

public class GeneratedFiles
{
    public GeneratedFile[] Files { get; set; }
}

public class GeneratedFile
{
    public string Path { get; set; }
    public string Type { get; set; }
    public long Size { get; set; }
}

public class ConfigurationSettings
{
    public string DatabaseConnection { get; set; }
    public string ApiEndpoint { get; set; }
    public int TimeoutSeconds { get; set; }
}

public class ValidationReport
{
    public bool IsValid { get; set; }
    public DateTime ValidatedAt { get; set; }
    public string InputHash { get; set; }
}

public class LegacySystemError
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string SystemName { get; set; }
    public DateTime Timestamp { get; set; }
}

// Excepciones personalizadas
public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

public class ExternalApiException : Exception
{
    public ExternalApiException(string message) : base(message) { }
}
```

### Ejemplo 2: Sistema de Migración de Datos

```csharp
public class DataMigrationService
{
    private readonly ISourceDatabase _sourceDb;
    private readonly ITargetDatabase _targetDb;
    private readonly IValidationService _validator;
    private readonly ITransformationEngine _transformer;
    
    public async Task<MlResult<MigrationResult>> MigrateDataBatchAsync(MigrationBatch batch)
    {
        var migrationId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        
        // 1. Validar configuración de migración
        var configValidation = ValidateMigrationConfigSafely(batch.Configuration);
        if (configValidation.IsFailed)
            return configValidation.ToMlResultFail<MigrationConfiguration, MigrationResult>();
        
        // 2. Extraer datos origen con manejo seguro
        var extractionResult = await ExtractSourceDataSafelyAsync(batch.SourceQuery);
        if (extractionResult.IsFailed)
            return extractionResult.ToMlResultFail<SourceDataSet, MigrationResult>();
        
        // 3. Transformar datos usando conversiones seguras
        var transformationResult = await TransformDataSafelyAsync(
            extractionResult.Value, batch.TransformationRules);
        if (transformationResult.IsFailed)
            return transformationResult.ToMlResultFail<TransformedDataSet, MigrationResult>();
        
        // 4. Validar datos transformados
        var validationResult = await ValidateTransformedDataAsync(transformationResult.Value);
        if (validationResult.IsFailed)
            return validationResult.ToMlResultFail<ValidationResult, MigrationResult>();
        
        // 5. Cargar datos en destino
        var loadResult = await LoadDataToTargetSafelyAsync(transformationResult.Value);
        if (loadResult.IsFailed)
            return loadResult.ToMlResultFail<LoadResult, MigrationResult>();
        
        var endTime = DateTime.UtcNow;
        
        return MlResult<MigrationResult>.Valid(new MigrationResult
        {
            MigrationId = migrationId,
            SourceRecords = extractionResult.Value.Records.Count(),
            TransformedRecords = transformationResult.Value.Records.Count(),
            LoadedRecords = loadResult.Value.ProcessedRecords,
            Duration = endTime - startTime,
            StartTime = startTime,
            EndTime = endTime,
            Status = "Completed"
        });
    }
    
    private MlResult<MigrationConfiguration> ValidateMigrationConfigSafely(MigrationConfiguration config)
    {
        Func<MigrationConfiguration, MigrationConfiguration> validator = cfg =>
        {
            if (string.IsNullOrEmpty(cfg.SourceConnectionString))
                throw new ConfigurationException("Source connection string is required");
            
            if (string.IsNullOrEmpty(cfg.TargetConnectionString))
                throw new ConfigurationException("Target connection string is required");
            
            if (cfg.BatchSize <= 0 || cfg.BatchSize > 10000)
                throw new ConfigurationException("Batch size must be between 1 and 10000");
            
            if (cfg.TransformationRules == null || !cfg.TransformationRules.Any())
                throw new ConfigurationException("At least one transformation rule is required");
            
            return cfg;
        };
        
        return validator.TryToMlResult(
            config, 
            ex => $"Migration configuration validation failed: {ex.Message}");
    }
    
    private async Task<MlResult<SourceDataSet>> ExtractSourceDataSafelyAsync(string sourceQuery)
    {
        Func<string, Task<SourceDataSet>> extractor = async query =>
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Source query cannot be empty");
            
            // Validar sintaxis SQL básica
            if (!IsValidSqlQuery(query))
                throw new SqlSyntaxException("Invalid SQL syntax in source query");
            
            var records = await _sourceDb.ExecuteQueryAsync(query);
            
            if (records == null)
                throw new DataExtractionException("Query returned null result");
            
            return new SourceDataSet
            {
                Query = query,
                Records = records.ToArray(),
                ExtractedAt = DateTime.UtcNow,
                RecordCount = records.Count()
            };
        };
        
        return await extractor.TryToMlResultAsync(
            sourceQuery,
            ex => $"Data extraction failed: {ex.Message}");
    }
    
    private async Task<MlResult<TransformedDataSet>> TransformDataSafelyAsync(
        SourceDataSet sourceData, 
        TransformationRule[] rules)
    {
        Func<SourceDataSet, Task<TransformedDataSet>> transformer = async data =>
        {
            var transformedRecords = new List<TransformedRecord>();
            var errorSummary = new List<string>();
            
            foreach (var record in data.Records)
            {
                try
                {
                    var transformedRecord = await ApplyTransformationRules(record, rules);
                    transformedRecords.Add(transformedRecord);
                }
                catch (Exception ex)
                {
                    errorSummary.Add($"Record {record.Id}: {ex.Message}");
                    
                    // Si hay demasiados errores, fallar la transformación completa
                    if (errorSummary.Count > data.Records.Length * 0.1) // Más del 10% de errores
                    {
                        throw new TransformationException(
                            $"Too many transformation errors. Sample errors: {string.Join("; ", errorSummary.Take(5))}");
                    }
                }
            }
            
            return new TransformedDataSet
            {
                Records = transformedRecords.ToArray(),
                TransformedAt = DateTime.UtcNow,
                SourceRecordCount = data.Records.Length,
                TransformedRecordCount = transformedRecords.Count,
                ErrorSummary = errorSummary.ToArray()
            };
        };
        
        return await transformer.TryToMlResultAsync(
            sourceData,
            ex => $"Data transformation failed: {ex.Message}");
    }
    
    private async Task<MlResult<ValidationResult>> ValidateTransformedDataAsync(TransformedDataSet transformedData)
    {
        Func<TransformedDataSet, Task<ValidationResult>> validator = async data =>
        {
            var validationErrors = new List<string>();
            var validRecords = 0;
            
            foreach (var record in data.Records)
            {
                var recordValidation = await _validator.ValidateRecordAsync(record);
                
                if (recordValidation.IsValid)
                {
                    validRecords++;
                }
                else
                {
                    validationErrors.AddRange(recordValidation.Errors.Select(e => 
                        $"Record {record.Id}: {e}"));
                }
            }
            
            // Permitir hasta 5% de registros inválidos
            var errorThreshold = data.Records.Length * 0.05;
            if (validationErrors.Count > errorThreshold)
            {
                throw new ValidationException(
                    $"Too many validation errors ({validationErrors.Count}). " +
                    $"Threshold: {errorThreshold}. Sample errors: {string.Join("; ", validationErrors.Take(3))}");
            }
            
            return new ValidationResult
            {
                TotalRecords = data.Records.Length,
                ValidRecords = validRecords,
                InvalidRecords = validationErrors.Count,
                ValidationErrors = validationErrors.ToArray(),
                ValidationSuccessRate = (double)validRecords / data.Records.Length,
                ValidatedAt = DateTime.UtcNow
            };
        };
        
        return await validator.TryToMlResultAsync(
            transformedData,
            ex => $"Data validation failed: {ex.Message}");
    }
    
    private async Task<MlResult<LoadResult>> LoadDataToTargetSafelyAsync(TransformedDataSet transformedData)
    {
        Func<TransformedDataSet, Task<LoadResult>> loader = async data =>
        {
            var batchSize = 1000;
            var processedRecords = 0;
            var errorCount = 0;
            var loadErrors = new List<string>();
            
            var batches = data.Records.Batch(batchSize);
            
            foreach (var batch in batches)
            {
                try
                {
                    var batchResult = await _targetDb.InsertBatchAsync(batch);
                    processedRecords += batchResult.ProcessedCount;
                    
                    if (batchResult.Errors.Any())
                    {
                        errorCount += batchResult.Errors.Count();
                        loadErrors.AddRange(batchResult.Errors);
                    }
                }
                catch (Exception ex)
                {
                    errorCount += batch.Count();
                    loadErrors.Add($"Batch error: {ex.Message}");
                    
                    // Si hay demasiados errores de carga, fallar
                    if (errorCount > data.Records.Length * 0.05) // Más del 5% de errores
                    {
                        throw new DataLoadException(
                            $"Too many load errors ({errorCount}). Sample errors: {string.Join("; ", loadErrors.Take(3))}");
                    }
                }
            }
            
            return new LoadResult
            {
                ProcessedRecords = processedRecords,
                ErrorCount = errorCount,
                LoadErrors = loadErrors.ToArray(),
                LoadSuccessRate = (double)processedRecords / data.Records.Length,
                LoadedAt = DateTime.UtcNow
            };
        };
        
        return await loader.TryToMlResultAsync(
            transformedData,
            ex => $"Data loading failed: {ex.Message}");
    }
    
    // Métodos de conversión para errores específicos del dominio
    public MlResult<T> ConvertDatabaseErrorToMlResult<T>(DatabaseException dbEx)
    {
        var errorContext = new Dictionary<string, object>
        {
            { "DatabaseErrorCode", dbEx.ErrorCode },
            { "SqlState", dbEx.SqlState },
            { "Severity", dbEx.Severity },
            { "DatabaseName", dbEx.DatabaseName }
        };
        
        var errorMessage = $"Database error: {dbEx.Message}";
        
        return (errorMessage, errorContext).ToMlResultFail<T>();
    }
    
    public async Task<MlResult<T>> HandleAsyncDatabaseOperationSafely<T>(
        Func<Task<T>> databaseOperation,
        string operationContext)
    {
        return await databaseOperation.TryToMlResultAsync(ex => ex switch
        {
            TimeoutException timeout => $"Database timeout in {operationContext}: {timeout.Message}",
            SqlException sql => $"SQL error in {operationContext}: {sql.Message}",
            ConnectionException conn => $"Connection error in {operationContext}: {conn.Message}",
            _ => $"Database operation failed in {operationContext}: {ex.Message}"
        });
    }
    
    // Métodos auxiliares
    private bool IsValidSqlQuery(string query) =>
        !string.IsNullOrWhiteSpace(query) && 
        query.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) &&
        !query.Contains(";DROP", StringComparison.OrdinalIgnoreCase) &&
        !query.Contains(";DELETE", StringComparison.OrdinalIgnoreCase);
    
    private async Task<TransformedRecord> ApplyTransformationRules(SourceRecord record, TransformationRule[] rules)
    {
        var transformedRecord = new TransformedRecord { Id = record.Id };
        
        foreach (var rule in rules)
        {
            await _transformer.ApplyRuleAsync(record, transformedRecord, rule);
        }
        
        return transformedRecord;
    }
}

// Clases adicionales para el ejemplo
public class MigrationBatch
{
    public MigrationConfiguration Configuration { get; set; }
    public string SourceQuery { get; set; }
    public TransformationRule[] TransformationRules { get; set; }
}

public class MigrationConfiguration
{
    public string SourceConnectionString { get; set; }
    public string TargetConnectionString { get; set; }
    public int BatchSize { get; set; }
    public TransformationRule[] TransformationRules { get; set; }
}

public class MigrationResult
{
    public Guid MigrationId { get; set; }
    public int SourceRecords { get; set; }
    public int TransformedRecords { get; set; }
    public int LoadedRecords { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; }
}

public class SourceDataSet
{
    public string Query { get; set; }
    public SourceRecord[] Records { get; set; }
    public DateTime ExtractedAt { get; set; }
    public int RecordCount { get; set; }
}

public class TransformedDataSet
{
    public TransformedRecord[] Records { get; set; }
    public DateTime TransformedAt { get; set; }
    public int SourceRecordCount { get; set; }
    public int TransformedRecordCount { get; set; }
    public string[] ErrorSummary { get; set; }
}

public class SourceRecord
{
    public string Id { get; set; }
    public Dictionary<string, object> Data { get; set; }
}

public class TransformedRecord
{
    public string Id { get; set; }
    public Dictionary<string, object> Data { get; set; }
}

public class TransformationRule
{
    public string SourceField { get; set; }
    public string TargetField { get; set; }
    public string Transformation { get; set; }
}

public class ValidationResult
{
    public int TotalRecords { get; set; }
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
    public string[] ValidationErrors { get; set; }
    public double ValidationSuccessRate { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class LoadResult
{
    public int ProcessedRecords { get; set; }
    public int ErrorCount { get; set; }
    public string[] LoadErrors { get; set; }
    public double LoadSuccessRate { get; set; }
    public DateTime LoadedAt { get; set; }
}

// Excepciones específicas
public class ConfigurationException : Exception
{
    public ConfigurationException(string message) : base(message) { }
}

public class SqlSyntaxException : Exception
{
    public SqlSyntaxException(string message) : base(message) { }
}

public class DataExtractionException : Exception
{
    public DataExtractionException(string message) : base(message) { }
}

public class TransformationException : Exception
{
    public TransformationException(string message) : base(message) { }
}

public class DataLoadException : Exception
{
    public DataLoadException(string message) : base(message) { }
}

public class DatabaseException : Exception
{
    public string ErrorCode { get; set; }
    public string SqlState { get; set; }
    public string Severity { get; set; }
    public string DatabaseName { get; set; }
    
    public DatabaseException(string message) : base(message) { }
}

public class SqlException : DatabaseException
{
    public SqlException(string message) : base(message) { }
}

public class ConnectionException : DatabaseException
{
    public ConnectionException(string message) : base(message) { }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar ToMlResult vs TryToMlResult

```csharp
// ✅ Correcto: Usar ToMlResult para conversiones simples sin riesgo
Func<string, int> getLength = s => s.Length;
var result1 = getLength.ToMlResult("Hello"); // Seguro

// ✅ Correcto: Usar TryToMlResult para operaciones que pueden fallar
Func<string, int> parseNumber = s => int.Parse(s);
var result2 = parseNumber.TryToMlResult("123", "Parse failed");

// ❌ Incorrecto: Usar ToMlResult para operaciones riesgosas
var result3 = parseNumber.ToMlResult("abc"); // Lanzará excepción no controlada
```

### 2. Constructores de Mensajes de Error Eficaces

```csharp
// ✅ Correcto: Mensajes contextuales específicos
var result = operation.TryToMlResult(input, ex => 
    $"Failed to process customer {input.Id} during {operationName}: {ex.Message}");

// ✅ Correcto: Incluir información relevante del contexto
var result = apiCall.TryToMlResultAsync(request, ex => ex switch
{
    TimeoutException timeout => $"API timeout after {timeout.Data["Duration"]}ms",
    HttpRequestException http => $"HTTP error {http.Data["StatusCode"]}: {http.Message}",
    _ => $"Unexpected API error: {ex.Message}"
});

// ❌ Incorrecto: Mensajes genéricos sin contexto
var result = operation.TryToMlResult(input, ex => "Something went wrong");
```

### 3. Manejo de Recursos y Disposables

```csharp
// ✅ Correcto: Usar using para recursos disposables
public MlResult<FileContent> ReadFileSafely(string filePath)
{
    Func<string, FileContent> reader = path =>
    {
        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);
        
        return new FileContent 
        { 
            Path = path, 
            Content = reader.ReadToEnd() 
        };
    };
    
    return reader.TryToMlResult(filePath, ex => $"Failed to read file {filePath}: {ex.Message}");
}

// ✅ Correcto: Manejar disposables en async
public async Task<MlResult<DatabaseResult>> QueryDatabaseSafelyAsync(string query)
{
    Func<string, Task<DatabaseResult>> dbQuery = async q =>
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(q, connection);
        
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        
        return new DatabaseResult { Value = result };
    };
    
    return await dbQuery.TryToMlResultAsync(query, ex => $"Database query failed: {ex.Message}");
}
```

### 4. Composición de Transformaciones

```csharp
// ✅ Correcto: Componer transformaciones de forma legible
public async Task<MlResult<ProcessedOrder>> ProcessOrderSafelyAsync(OrderRequest request)
{
    return await ValidateOrderRequest(request)
        .BindAsync(async validRequest => await EnrichOrderData(validRequest))
        .BindAsync(async enrichedOrder => await CalculatePricing(enrichedOrder))
        .BindAsync(async pricedOrder => await ApplyDiscounts(pricedOrder))
        .BindAsync(async finalOrder => await SaveOrder(finalOrder));
}

private MlResult<ValidatedOrderRequest> ValidateOrderRequest(OrderRequest request)
{
    Func<OrderRequest, ValidatedOrderRequest> validator = req =>
    {
        if (req.CustomerId <= 0)
            throw new ValidationException("Invalid customer ID");
        
        if (!req.Items.Any())
            throw new ValidationException("Order must have items");
        
        return new ValidatedOrderRequest(req);
    };
    
    return validator.TryToMlResult(request, ex => $"Order validation failed: {ex.Message}");
}

// ❌ Incorrecto: Anidamiento excesivo sin composición
public async Task<MlResult<ProcessedOrder>> ProcessOrderBadAsync(OrderRequest request)
{
    var validationResult = ValidateOrderRequest(request);
    if (validationResult.IsFailed)
    {
        var enrichResult = await EnrichOrderData(validationResult.Value);
        if (enrichResult.IsFailed)
        {
            // ... anidamiento profundo
        }
    }
}
```

---

## Comparación con Otros Métodos

### Tabla Comparativa

| Método | Propósito | Manejo de Excepciones | Cuándo Usar |
|--------|-----------|----------------------|-------------|
| `ToMlResult` | Conversión simple | No (lanza excepciones) | Operaciones seguras |
| `TryToMlResult` | Conversión segura | Sí (convierte a Fail) | Operaciones riesgosas |
| `Map` | Transformación | Depende de función | Transformaciones simples |
| `Bind` | Encadenamiento | Preserva errores MlResult | Operaciones encadenadas |

### Ejemplo Comparativo

```csharp
var input = "123";

// ToMlResult: Conversión directa (sin protección)
Func<string, int> parse = s => int.Parse(s);
var result1 = parse.ToMlResult(input); // Valid(123)

// TryToMlResult: Conversión protegida
var result2 = parse.TryToMlResult(input, "Parse failed"); // Valid(123)
var result3 = parse.TryToMlResult("abc", "Parse failed"); // Fail("Parse failed")

// Map: Para transformaciones en MlResult existente
var mlInput = MlResult<string>.Valid("123");
var result4 = mlInput.Map(s => int.Parse(s)); // Lanza excepción si falla

// Bind: Para operaciones que retornan MlResult
var result5 = mlInput.Bind(s => parse.TryToMlResult(s, "Parse error")); // Seguro
```

---

## Resumen

Los métodos **Transformations** proporcionan **conversión segura entre paradigmas**:

- **`ToMlResult`**: Conversión directa sin protección de excepciones
- **`TryToMlResult`**: Conversión segura con captura automática de excepciones
- **`ToMlResultValid/Fail`**: Creación directa de estados MlResult
- **Variantes Async**: Soporte completo para operaciones asíncronas

**Casos de uso ideales**:
- **Integración con código legacy** que lanza excepciones
- **Conversión de APIs externas** en el ecosistema MlResult
- **Manejo seguro de operaciones** de I/O y parsing
- **Puentes entre paradigmas** imperativos y funcionales

**Ventajas principales**:
- **Captura automática de excepciones** sin bloques try-catch explícitos
- **Mensajes de error personalizables** con contexto específico
- **Integración transparente** con el ecosistema MlResult
- **Soporte async completo** para operaciones modernas