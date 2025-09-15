# MlResultActionsMapIfFailWithValue - Recuperación de Errores con Valores Preservados

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Concepto de "Detail Value"](#concepto-de-detail-value)
4. [Métodos MapIfFailWithValue Básicos](#métodos-mapiffailwithvalue-básicos)
5. [Métodos con Cambio de Tipo](#métodos-con-cambio-de-tipo)
6. [Métodos TryMapIfFailWithValue](#métodos-trymapiffailwithvalue)
7. [Variantes Asíncronas](#variantes-asíncronas)
8. [Ejemplos Prácticos](#ejemplos-prácticos)
9. [Comparación con Otros Patrones](#comparación-con-otros-patrones)
10. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsMapIfFailWithValue` implementa **recuperación de errores con acceso a valores preservados**. A diferencia de `MapIfFail` que solo recibe información del error, estos métodos pueden acceder a un valor que fue preservado durante la ejecución fallida, permitiendo crear fallbacks más inteligentes basados en datos parciales.

### Propósito Principal

- **Recovered Value Processing**: Procesar valores que fueron preservados durante fallos
- **Partial Success Recovery**: Recuperar de errores usando resultados parciales
- **Context-Aware Fallbacks**: Fallbacks que consideran el estado antes del fallo
- **Value-Based Error Recovery**: Recuperación basada en valores intermedios

### Filosofía de Diseño

```
Valor Válido           → Valor (sin modificación)
Error con Valor        → func(preservedValue) → Valor Recuperado
Error sin Valor        → Error (propagación)
```

La clave está en que el error **contiene un valor preservado** que puede ser utilizado para la recuperación.

---

## Análisis de la Clase

### Patrón de Funcionamiento

`MapIfFailWithValue` funciona cuando:

1. Una operación previa **preservó un valor** antes de fallar
2. El error contiene ese valor como "detail value"
3. La función de recuperación puede usar ese valor preservado
4. Se genera un resultado válido basado en el valor preservado

### Diferencia Clave con MapIfFail

```csharp
// MapIfFail: Solo información del error
var result = GetData()
    .MapIfFail(error => GetDefaultValue());  // Solo mensaje de error disponible

// MapIfFailWithValue: Acceso al valor preservado
var result = GetDataWithPreservation()
    .MapIfFailWithValue(preservedValue => ProcessPreservedValue(preservedValue));
```

### Casos de Uso Típicos

- **Validación con Valores Parciales**: Cuando la validación falla pero el valor parcial es utilizable
- **Parsing con Fallback**: Cuando el parsing falla pero hay una representación alternativa
- **Transformación con Degradación**: Cuando una transformación compleja falla pero la simple funciona
- **Operaciones con Timeout**: Cuando hay timeout pero resultados parciales disponibles

---

## Concepto de "Detail Value"

### ¿Qué es un Detail Value?

Un **Detail Value** es un valor que se preserva en el `MlErrorsDetails` cuando una operación falla. Esto permite que operaciones posteriores accedan a datos parciales o intermedios.

```csharp
// Ejemplo conceptual: Una operación que preserva el valor antes de fallar
public MlResult<ComplexData> ProcessData(RawData raw)
{
    var partiallyProcessed = CreatePartialData(raw);  // Esto siempre funciona
    
    try 
    {
        return CompleteProcessing(partiallyProcessed);  // Esto puede fallar
    }
    catch (Exception ex)
    {
        // Preservar el valor parcial en el error
        return MlResult<ComplexData>.FailWithValue(
            partiallyProcessed,  // Valor preservado
            $"Failed to complete processing: {ex.Message}"
        );
    }
}
```

### Acceso al Detail Value

Los métodos `MapIfFailWithValue` usan `errorsDetails.GetDetailValue<T>()` para extraer el valor preservado:

```csharp
public static MlResult<T> MapIfFailWithValue<T>(this MlResult<T> source,
                                                Func<T, T> func)
    => source.Match(
        fail: errorsDetails => errorsDetails.GetDetailValue<T>().Map(func),
        valid: value => value
    );
```

---

## Métodos MapIfFailWithValue Básicos

### `MapIfFailWithValue<T>()` - Recuperación con Mismo Tipo

**Propósito**: Recupera de un error usando el valor preservado y manteniendo el mismo tipo

```csharp
public static MlResult<T> MapIfFailWithValue<T>(this MlResult<T> source,
                                                Func<T, T> func)
```

**Comportamiento**:
- Si `source` es válido: Retorna el valor sin modificación
- Si `source` es fallido: Extrae el valor preservado y aplica `func(preservedValue)`
- Si no hay valor preservado: Propaga el error

**Ejemplo Básico**:
```csharp
var result = ParseComplexNumber("3.14invalid")
    .MapIfFailWithValue(partialValue => new ComplexNumber 
    { 
        Real = partialValue.Real,  // Usar la parte que se pudo parsear
        Imaginary = 0              // Valor por defecto para la parte que falló
    });
```

### `MapIfFailWithValueAsync<T>()` - Versión Asíncrona

```csharp
public static async Task<MlResult<T>> MapIfFailWithValueAsync<T>(
    this MlResult<T> source,
    Func<T, Task<T>> funcAsync)
```

**Ejemplo**:
```csharp
var result = await ProcessDocumentAsync(document)
    .MapIfFailWithValueAsync(async partialDocument => 
        await SavePartialDocumentAsync(partialDocument));
```

---

## Métodos con Cambio de Tipo

### `MapIfFailWithValue<T, TValue, TReturn>()` - Recuperación con Transformación

**Propósito**: Permite diferentes tipos para el valor de origen, valor preservado y resultado

```csharp
public static MlResult<TReturn> MapIfFailWithValue<T, TValue, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> funcValid,        // Para valores válidos
    Func<TValue, TReturn> funcFail)    // Para valores preservados
```

**Parámetros**:
- `T`: Tipo del valor de origen
- `TValue`: Tipo del valor preservado (puede ser diferente)
- `TReturn`: Tipo del resultado final
- `funcValid`: Función aplicada cuando el resultado es válido
- `funcFail`: Función aplicada al valor preservado cuando hay error

**Ejemplo**:
```csharp
var displayText = ParseUserProfile(jsonString)
    .MapIfFailWithValue<UserProfile, PartialProfile, string>(
        funcValid: profile => $"Welcome, {profile.FullName}!",
        funcFail: partial => $"Welcome, {partial.FirstName ?? "User"}!"
    );
```

### Casos de Uso del Cambio de Tipo

```csharp
// Ejemplo: Parsing con tipos diferentes
public class FullAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public Coordinates Coordinates { get; set; }  // Puede fallar el geocoding
}

public class BasicAddress  // Valor preservado más simple
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
}

var addressResult = ParseFullAddress(addressString)
    .MapIfFailWithValue<FullAddress, BasicAddress, DisplayAddress>(
        funcValid: full => new DisplayAddress 
        { 
            Text = $"{full.Street}, {full.City}, {full.State}",
            HasCoordinates = true,
            Coordinates = full.Coordinates 
        },
        funcFail: basic => new DisplayAddress 
        { 
            Text = $"{basic.Street}, {basic.City}, {basic.State}",
            HasCoordinates = false,
            Coordinates = null 
        }
    );
```

---

## Métodos TryMapIfFailWithValue

### `TryMapIfFailWithValue<T>()` - Versión Segura

**Propósito**: Versión que captura excepciones en la función de recuperación

```csharp
public static MlResult<T> TryMapIfFailWithValue<T>(
    this MlResult<T> source,
    Func<T, T> funcValue,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: Si la función de recuperación lanza una excepción, la captura y convierte en error.

### `TryMapIfFailWithValue<T, TValue, TReturn>()` - Versión Segura con Tipos

```csharp
public static MlResult<TReturn> TryMapIfFailWithValue<T, TValue, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> funcValid,
    Func<TValue, TReturn> funcFail,
    Func<Exception, string> errorMessageBuilder)
```

**Ejemplo**:
```csharp
var result = ProcessConfiguration(configText)
    .TryMapIfFailWithValue<Config, PartialConfig, AppSettings>(
        funcValid: config => CreateFullSettings(config),
        funcFail: partial => CreateMinimalSettings(partial),  // Puede fallar también
        ex => $"Failed to create settings from partial config: {ex.Message}"
    );
```

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Fuente | funcValid | funcFail | Método |
|--------|-----------|----------|---------|
| `MlResult<T>` | `T → U` | `V → U` | `MapIfFailWithValue` |
| `MlResult<T>` | `T → Task<U>` | `V → Task<U>` | `MapIfFailWithValueAsync` |
| `Task<MlResult<T>>` | `T → U` | `V → U` | `MapIfFailWithValueAsync` |
| `Task<MlResult<T>>` | `T → Task<U>` | `V → Task<U>` | `MapIfFailWithValueAsync` |

### Soporte para TryMapIfFailWithValue

Todas las combinaciones están disponibles con:
- Constructor de mensaje de error (`Func<Exception, string>`)
- Mensaje simple (`string`)
- Todas las variantes asíncronas posibles

---

## Ejemplos Prácticos

### Ejemplo 1: Parsing Avanzado con Valores Parciales

```csharp
public class AdvancedParsingService
{
    public MlResult<UserProfile> ParseUserProfile(string jsonString)
    {
        return TryParseCompleteProfile(jsonString)
            .MapIfFailWithValue<UserProfile, BasicUserInfo, UserProfile>(
                funcValid: profile => profile,  // Ya está completo
                funcFail: basicInfo => new UserProfile
                {
                    Id = basicInfo.Id,
                    Name = basicInfo.Name,
                    Email = basicInfo.Email,
                    ProfilePictureUrl = "/images/default-avatar.png",  // Default
                    Preferences = new UserPreferences(),              // Default
                    IsComplete = false,
                    Source = "partial-parsing"
                }
            );
    }

    public MlResult<FinancialReport> GenerateFinancialReport(string dataSource)
    {
        return ProcessCompleteFinancialData(dataSource)
            .MapIfFailWithValue(partialData => new FinancialReport
            {
                BasicMetrics = partialData.BasicMetrics,
                AdvancedAnalytics = null,  // No disponible
                CompletionStatus = "Partial",
                Warning = "Some advanced metrics could not be calculated",
                GeneratedAt = DateTime.UtcNow
            });
    }

    public async Task<MlResult<SearchResults>> PerformAdvancedSearchAsync(SearchQuery query)
    {
        return await ExecuteFullTextSearchAsync(query)
            .MapIfFailWithValueAsync(async basicResults => new SearchResults
            {
                Query = query.Terms,
                Results = basicResults.Results,
                TotalCount = basicResults.Results.Count,
                IsPartial = true,
                SearchType = "basic",
                Message = "Advanced search features unavailable, showing basic results",
                ExecutionTime = basicResults.ExecutionTime
            });
    }

    public MlResult<ConfigurationSet> LoadConfiguration(string configPath)
    {
        return LoadCompleteConfiguration(configPath)
            .TryMapIfFailWithValue<ConfigurationSet, BaseConfiguration, ConfigurationSet>(
                funcValid: config => config,
                funcFail: baseConfig => new ConfigurationSet
                {
                    DatabaseConfig = baseConfig.DatabaseConfig,
                    LoggingConfig = baseConfig.LoggingConfig,
                    CacheConfig = GetDefaultCacheConfig(),    // Default
                    SecurityConfig = GetDefaultSecurityConfig(), // Default
                    IsComplete = false,
                    LoadedFrom = "partial-configuration"
                },
                ex => $"Failed to build configuration from partial data: {ex.Message}"
            );
    }

    public MlResult<ValidationResult> ValidateDocument(Document document)
    {
        return PerformFullValidation(document)
            .MapIfFailWithValue<ValidationResult, PartialValidation, ValidationResult>(
                funcValid: result => result,
                funcFail: partial => new ValidationResult
                {
                    DocumentId = partial.DocumentId,
                    IsValid = false,
                    PassedChecks = partial.PassedChecks,
                    FailedChecks = new List<string> { "Advanced validation failed" },
                    SkippedChecks = partial.PendingChecks,
                    ValidationLevel = "Basic",
                    Message = "Document passed basic validation but failed advanced checks"
                }
            );
    }

    // Métodos auxiliares que preservan valores
    private MlResult<UserProfile> TryParseCompleteProfile(string jsonString)
    {
        try
        {
            var basicInfo = JsonSerializer.Deserialize<BasicUserInfo>(jsonString);
            
            // Intentar parsing completo
            var fullProfile = JsonSerializer.Deserialize<UserProfile>(jsonString);
            return fullProfile;
        }
        catch (JsonException ex) when (ex.Message.Contains("advanced"))
        {
            // Preservar información básica si el parsing avanzado falla
            var basicInfo = JsonSerializer.Deserialize<BasicUserInfo>(jsonString);
            return MlResult<UserProfile>.FailWithValue(
                basicInfo,
                $"Advanced profile parsing failed: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return MlResult<UserProfile>.Fail($"Complete parsing failed: {ex.Message}");
        }
    }

    private MlResult<FinancialReport> ProcessCompleteFinancialData(string dataSource)
    {
        try
        {
            var basicData = ExtractBasicFinancialData(dataSource);
            var advancedData = ExtractAdvancedFinancialData(dataSource); // Puede fallar
            
            return new FinancialReport
            {
                BasicMetrics = basicData,
                AdvancedAnalytics = advancedData,
                CompletionStatus = "Complete"
            };
        }
        catch (Exception ex) when (ex.Message.Contains("advanced"))
        {
            // Preservar datos básicos
            var basicData = ExtractBasicFinancialData(dataSource);
            return MlResult<FinancialReport>.FailWithValue(
                new PartialFinancialData { BasicMetrics = basicData },
                $"Advanced financial processing failed: {ex.Message}"
            );
        }
    }

    private async Task<MlResult<SearchResults>> ExecuteFullTextSearchAsync(SearchQuery query)
    {
        try
        {
            var basicResults = await ExecuteBasicSearchAsync(query);
            var enhancedResults = await EnhanceWithAdvancedFeaturesAsync(basicResults); // Puede fallar
            
            return enhancedResults;
        }
        catch (Exception ex)
        {
            var basicResults = await ExecuteBasicSearchAsync(query);
            return MlResult<SearchResults>.FailWithValue(
                basicResults,
                $"Advanced search features failed: {ex.Message}"
            );
        }
    }
}

// Clases de apoyo
public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public UserPreferences Preferences { get; set; }
    public bool IsComplete { get; set; }
    public string Source { get; set; }
}

public class BasicUserInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class FinancialReport
{
    public BasicFinancialMetrics BasicMetrics { get; set; }
    public AdvancedAnalytics AdvancedAnalytics { get; set; }
    public string CompletionStatus { get; set; }
    public string Warning { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class PartialFinancialData
{
    public BasicFinancialMetrics BasicMetrics { get; set; }
}

public class ConfigurationSet
{
    public DatabaseConfig DatabaseConfig { get; set; }
    public LoggingConfig LoggingConfig { get; set; }
    public CacheConfig CacheConfig { get; set; }
    public SecurityConfig SecurityConfig { get; set; }
    public bool IsComplete { get; set; }
    public string LoadedFrom { get; set; }
}

public class BaseConfiguration
{
    public DatabaseConfig DatabaseConfig { get; set; }
    public LoggingConfig LoggingConfig { get; set; }
}

public class ValidationResult
{
    public string DocumentId { get; set; }
    public bool IsValid { get; set; }
    public List<string> PassedChecks { get; set; }
    public List<string> FailedChecks { get; set; }
    public List<string> SkippedChecks { get; set; }
    public string ValidationLevel { get; set; }
    public string Message { get; set; }
}

public class PartialValidation
{
    public string DocumentId { get; set; }
    public List<string> PassedChecks { get; set; }
    public List<string> PendingChecks { get; set; }
}
```

### Ejemplo 2: Procesamiento de Imágenes con Fallbacks

```csharp
public class ImageProcessingService
{
    public async Task<MlResult<ProcessedImage>> ProcessImageAsync(byte[] imageData)
    {
        return await ProcessHighQualityImageAsync(imageData)
            .MapIfFailWithValueAsync<ProcessedImage, BasicImage, ProcessedImage>(
                funcValidAsync: async image => image,  // Ya procesada completamente
                funcFailAsync: async basicImage => new ProcessedImage
                {
                    Id = basicImage.Id,
                    Width = basicImage.Width,
                    Height = basicImage.Height,
                    Format = basicImage.Format,
                    Data = basicImage.Data,
                    Quality = "Basic",
                    Filters = new List<string>(),  // Sin filtros avanzados
                    Metadata = basicImage.Metadata,
                    ProcessingLevel = "Fallback",
                    Warning = "Advanced processing failed, basic processing applied"
                }
            );
    }

    public MlResult<ThumbnailSet> GenerateThumbnails(ImageFile sourceImage)
    {
        return GenerateAdvancedThumbnails(sourceImage)
            .MapIfFailWithValue<ThumbnailSet, BasicThumbnail, ThumbnailSet>(
                funcValid: thumbnails => thumbnails,
                funcFail: basicThumbnail => new ThumbnailSet
                {
                    SourceImageId = basicThumbnail.SourceImageId,
                    Thumbnails = new List<Thumbnail>
                    {
                        new Thumbnail
                        {
                            Size = "small",
                            Width = basicThumbnail.Width,
                            Height = basicThumbnail.Height,
                            Data = basicThumbnail.Data,
                            Quality = "Standard"
                        }
                    },
                    GenerationMethod = "Basic",
                    IsComplete = false
                }
            );
    }

    public async Task<MlResult<ImageAnalysis>> AnalyzeImageAsync(string imagePath)
    {
        return await PerformAdvancedAnalysisAsync(imagePath)
            .TryMapIfFailWithValueAsync<ImageAnalysis, BasicAnalysis, ImageAnalysis>(
                funcValidAsync: async analysis => analysis,
                funcFailAsync: async basicAnalysis => new ImageAnalysis
                {
                    ImagePath = basicAnalysis.ImagePath,
                    BasicInfo = basicAnalysis.BasicInfo,
                    Colors = basicAnalysis.Colors,
                    Objects = new List<DetectedObject>(),  // Detección avanzada falló
                    Faces = new List<DetectedFace>(),      // Detección facial falló
                    Text = null,                           // OCR falló
                    Quality = basicAnalysis.Quality,
                    AnalysisLevel = "Basic",
                    Confidence = 0.7
                },
                ex => $"Advanced image analysis failed: {ex.Message}"
            );
    }

    public MlResult<CompressedImage> CompressImage(ImageFile source, CompressionOptions options)
    {
        return PerformAdvancedCompression(source, options)
            .MapIfFailWithValue(basicCompressed => new CompressedImage
            {
                OriginalSize = basicCompressed.OriginalSize,
                CompressedSize = basicCompressed.CompressedSize,
                CompressionRatio = basicCompressed.CompressionRatio,
                Data = basicCompressed.Data,
                Quality = "Standard",  // No pudo aplicar compresión óptima
                Algorithm = "Basic",
                Settings = GetDefaultCompressionSettings(),
                Warning = "Advanced compression failed, using standard compression"
            });
    }

    // Métodos que preservan valores parciales
    private async Task<MlResult<ProcessedImage>> ProcessHighQualityImageAsync(byte[] imageData)
    {
        try
        {
            var basicImage = await CreateBasicImageAsync(imageData);
            
            // Intentar procesamiento avanzado
            var enhancedImage = await ApplyAdvancedFiltersAsync(basicImage);
            var finalImage = await OptimizeImageAsync(enhancedImage);
            
            return finalImage;
        }
        catch (Exception ex) when (ex.Message.Contains("advanced") || ex.Message.Contains("optimize"))
        {
            // Preservar imagen básica si el procesamiento avanzado falla
            var basicImage = await CreateBasicImageAsync(imageData);
            return MlResult<ProcessedImage>.FailWithValue(
                basicImage,
                $"Advanced image processing failed: {ex.Message}"
            );
        }
    }

    private MlResult<ThumbnailSet> GenerateAdvancedThumbnails(ImageFile sourceImage)
    {
        try
        {
            var basicThumbnail = GenerateBasicThumbnail(sourceImage);
            
            // Intentar generar múltiples tamaños con algoritmos avanzados
            var advancedThumbnails = GenerateMultipleSizes(basicThumbnail);
            var optimizedThumbnails = ApplySmartCropping(advancedThumbnails);
            
            return new ThumbnailSet
            {
                SourceImageId = sourceImage.Id,
                Thumbnails = optimizedThumbnails,
                GenerationMethod = "Advanced",
                IsComplete = true
            };
        }
        catch (Exception ex)
        {
            var basicThumbnail = GenerateBasicThumbnail(sourceImage);
            return MlResult<ThumbnailSet>.FailWithValue(
                basicThumbnail,
                $"Advanced thumbnail generation failed: {ex.Message}"
            );
        }
    }

    private async Task<MlResult<ImageAnalysis>> PerformAdvancedAnalysisAsync(string imagePath)
    {
        try
        {
            var basicAnalysis = await PerformBasicAnalysisAsync(imagePath);
            
            // Análisis avanzado que puede fallar
            var objects = await DetectObjectsAsync(imagePath);
            var faces = await DetectFacesAsync(imagePath);
            var text = await ExtractTextAsync(imagePath);
            
            return new ImageAnalysis
            {
                ImagePath = imagePath,
                BasicInfo = basicAnalysis.BasicInfo,
                Colors = basicAnalysis.Colors,
                Objects = objects,
                Faces = faces,
                Text = text,
                Quality = basicAnalysis.Quality,
                AnalysisLevel = "Advanced",
                Confidence = 0.95
            };
        }
        catch (Exception ex)
        {
            var basicAnalysis = await PerformBasicAnalysisAsync(imagePath);
            return MlResult<ImageAnalysis>.FailWithValue(
                basicAnalysis,
                $"Advanced analysis failed: {ex.Message}"
            );
        }
    }

    private MlResult<CompressedImage> PerformAdvancedCompression(ImageFile source, CompressionOptions options)
    {
        try
        {
            var basicCompressed = PerformBasicCompression(source);
            
            // Compresión avanzada que puede fallar
            var optimized = ApplyAdvancedCompression(basicCompressed, options);
            
            return optimized;
        }
        catch (Exception ex)
        {
            var basicCompressed = PerformBasicCompression(source);
            return MlResult<CompressedImage>.FailWithValue(
                basicCompressed,
                $"Advanced compression failed: {ex.Message}"
            );
        }
    }

    // Métodos auxiliares
    private async Task<BasicImage> CreateBasicImageAsync(byte[] imageData)
    {
        await Task.Delay(50); // Simular procesamiento
        return new BasicImage
        {
            Id = Guid.NewGuid().ToString(),
            Data = imageData,
            Width = 1920,
            Height = 1080,
            Format = "JPEG",
            Metadata = new Dictionary<string, object>
            {
                ["created"] = DateTime.UtcNow,
                ["source"] = "upload"
            }
        };
    }

    private BasicThumbnail GenerateBasicThumbnail(ImageFile source)
    {
        return new BasicThumbnail
        {
            SourceImageId = source.Id,
            Width = 200,
            Height = 150,
            Data = GenerateSimpleThumbnailData(source),
            Quality = "Standard"
        };
    }

    private byte[] GenerateSimpleThumbnailData(ImageFile source)
    {
        // Simulación de generación de thumbnail básico
        return new byte[1024]; // Thumbnail simplificado
    }

    private CompressionSettings GetDefaultCompressionSettings()
    {
        return new CompressionSettings
        {
            Quality = 80,
            Algorithm = "JPEG",
            Progressive = false,
            OptimizeHuffman = false
        };
    }
}

// Clases de apoyo para imágenes
public class ProcessedImage
{
    public string Id { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Format { get; set; }
    public byte[] Data { get; set; }
    public string Quality { get; set; }
    public List<string> Filters { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    public string ProcessingLevel { get; set; }
    public string Warning { get; set; }
}

public class BasicImage
{
    public string Id { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Format { get; set; }
    public byte[] Data { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class ThumbnailSet
{
    public string SourceImageId { get; set; }
    public List<Thumbnail> Thumbnails { get; set; }
    public string GenerationMethod { get; set; }
    public bool IsComplete { get; set; }
}

public class BasicThumbnail
{
    public string SourceImageId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public byte[] Data { get; set; }
    public string Quality { get; set; }
}

public class ImageAnalysis
{
    public string ImagePath { get; set; }
    public ImageInfo BasicInfo { get; set; }
    public List<ColorInfo> Colors { get; set; }
    public List<DetectedObject> Objects { get; set; }
    public List<DetectedFace> Faces { get; set; }
    public string Text { get; set; }
    public QualityMetrics Quality { get; set; }
    public string AnalysisLevel { get; set; }
    public double Confidence { get; set; }
}

public class BasicAnalysis
{
    public string ImagePath { get; set; }
    public ImageInfo BasicInfo { get; set; }
    public List<ColorInfo> Colors { get; set; }
    public QualityMetrics Quality { get; set; }
}

public class CompressedImage
{
    public long OriginalSize { get; set; }
    public long CompressedSize { get; set; }
    public double CompressionRatio { get; set; }
    public byte[] Data { get; set; }
    public string Quality { get; set; }
    public string Algorithm { get; set; }
    public CompressionSettings Settings { get; set; }
    public string Warning { get; set; }
}
```

### Ejemplo 3: Procesamiento de Datos con Estados Parciales

```csharp
public class DataPipelineService
{
    public async Task<MlResult<ProcessedDataset>> ProcessDatasetAsync(RawDataset dataset)
    {
        return await ExecuteFullPipelineAsync(dataset)
            .MapIfFailWithValueAsync<ProcessedDataset, IntermediateDataset, ProcessedDataset>(
                funcValidAsync: async processed => processed,
                funcFailAsync: async intermediate => new ProcessedDataset
                {
                    Id = intermediate.Id,
                    Name = intermediate.Name,
                    CleanedData = intermediate.CleanedData,
                    ValidationResults = intermediate.ValidationResults,
                    
                    // Campos que no se pudieron completar
                    EnrichedData = null,
                    AnalysisResults = CreateBasicAnalysis(intermediate),
                    QualityScore = CalculateBasicQuality(intermediate),
                    
                    ProcessingStatus = "Partial",
                    ProcessingSteps = intermediate.CompletedSteps,
                    FailedSteps = new List<string> { "Enrichment", "Advanced Analysis" },
                    Warning = "Dataset processed with basic pipeline due to advanced processing failure"
                }
            );
    }

    public MlResult<ReportGeneration> GenerateReport(ReportRequest request)
    {
        return GenerateCompleteReport(request)
            .TryMapIfFailWithValue<ReportGeneration, PartialReport, ReportGeneration>(
                funcValid: report => report,
                funcFail: partial => new ReportGeneration
                {
                    ReportId = partial.ReportId,
                    Title = partial.Title,
                    Sections = partial.CompletedSections,
                    Charts = FilterBasicCharts(partial.AvailableCharts),
                    Tables = partial.BasicTables,
                    
                    // Sin elementos avanzados
                    AdvancedVisualizations = new List<Visualization>(),
                    InteractiveDashboard = null,
                    
                    Status = "Partial",
                    GeneratedAt = DateTime.UtcNow,
                    Warning = "Some advanced report features could not be generated"
                },
                ex => $"Failed to generate report from partial data: {ex.Message}"
            );
    }

    public async Task<MlResult<DataTransformation>> TransformDataAsync(DataTransformRequest request)
    {
        return await ExecuteAdvancedTransformationAsync(request)
            .MapIfFailWithValueAsync(async basicTransform => new DataTransformation
            {
                RequestId = basicTransform.RequestId,
                InputSchema = basicTransform.InputSchema,
                OutputSchema = basicTransform.OutputSchema,
                TransformedRecords = basicTransform.TransformedRecords,
                
                // Transformaciones básicas aplicadas
                BasicMappings = basicTransform.BasicMappings,
                
                // Transformaciones avanzadas omitidas
                AdvancedMappings = new List<AdvancedMapping>(),
                CustomTransformations = new List<CustomTransformation>(),
                
                TransformationType = "Basic",
                PerformanceMetrics = await CalculateBasicMetricsAsync(basicTransform),
                Warning = "Advanced transformations failed, applied basic transformations only"
            });
    }

    public MlResult<ValidationReport> ValidateDataIntegrity(Dataset dataset)
    {
        return PerformComprehensiveValidation(dataset)
            .MapIfFailWithValue<ValidationReport, BasicValidation, ValidationReport>(
                funcValid: report => report,
                funcFail: basic => new ValidationReport
                {
                    DatasetId = basic.DatasetId,
                    ValidationDate = DateTime.UtcNow,
                    
                    // Validaciones básicas completadas
                    SchemaValidation = basic.SchemaValidation,
                    DataTypeValidation = basic.DataTypeValidation,
                    NullValueCheck = basic.NullValueCheck,
                    
                    // Validaciones avanzadas no disponibles
                    ReferentialIntegrityCheck = new ValidationResult 
                    { 
                        Status = "Skipped", 
                        Message = "Advanced validation unavailable" 
                    },
                    BusinessRuleValidation = new ValidationResult 
                    { 
                        Status = "Skipped", 
                        Message = "Business rule validation unavailable" 
                    },
                    CrossDatasetValidation = new ValidationResult 
                    { 
                        Status = "Skipped", 
                        Message = "Cross-dataset validation unavailable" 
                    },
                    
                    OverallStatus = DetermineBasicStatus(basic),
                    ValidationLevel = "Basic",
                    Warning = "Comprehensive validation failed, basic validation completed"
                }
            );
    }

    // Métodos que preservan estados intermedios
    private async Task<MlResult<ProcessedDataset>> ExecuteFullPipelineAsync(RawDataset dataset)
    {
        try
        {
            // Paso 1: Limpieza (casi siempre funciona)
            var cleaned = await CleanDataAsync(dataset);
            
            // Paso 2: Validación básica
            var validated = await ValidateDataAsync(cleaned);
            
            // Estado intermedio preservable
            var intermediate = new IntermediateDataset
            {
                Id = dataset.Id,
                Name = dataset.Name,
                CleanedData = cleaned,
                ValidationResults = validated,
                CompletedSteps = new List<string> { "Cleaning", "Basic Validation" }
            };
            
            // Paso 3: Enriquecimiento (puede fallar)
            var enriched = await EnrichDataAsync(intermediate);
            
            // Paso 4: Análisis avanzado (puede fallar)
            var analyzed = await PerformAdvancedAnalysisAsync(enriched);
            
            return new ProcessedDataset
            {
                Id = dataset.Id,
                Name = dataset.Name,
                CleanedData = intermediate.CleanedData,
                ValidationResults = intermediate.ValidationResults,
                EnrichedData = enriched,
                AnalysisResults = analyzed,
                QualityScore = CalculateAdvancedQuality(analyzed),
                ProcessingStatus = "Complete",
                ProcessingSteps = new List<string> { "Cleaning", "Validation", "Enrichment", "Analysis" }
            };
        }
        catch (Exception ex) when (ex.Message.Contains("enrichment") || ex.Message.Contains("analysis"))
        {
            // Preservar estado intermedio
            var intermediate = new IntermediateDataset
            {
                Id = dataset.Id,
                Name = dataset.Name,
                CleanedData = await CleanDataAsync(dataset),
                ValidationResults = await ValidateDataAsync(await CleanDataAsync(dataset)),
                CompletedSteps = new List<string> { "Cleaning", "Basic Validation" }
            };
            
            return MlResult<ProcessedDataset>.FailWithValue(
                intermediate,
                $"Advanced pipeline steps failed: {ex.Message}"
            );
        }
    }

    private MlResult<ReportGeneration> GenerateCompleteReport(ReportRequest request)
    {
        try
        {
            var basicSections = GenerateBasicSections(request);
            var basicCharts = GenerateBasicCharts(request);
            var basicTables = GenerateBasicTables(request);
            
            // Estado parcial
            var partial = new PartialReport
            {
                ReportId = request.Id,
                Title = request.Title,
                CompletedSections = basicSections,
                AvailableCharts = basicCharts,
                BasicTables = basicTables
            };
            
            // Intentar elementos avanzados
            var advancedVisualizations = GenerateAdvancedVisualizations(request);  // Puede fallar
            var interactiveDashboard = CreateInteractiveDashboard(request);        // Puede fallar
            
            return new ReportGeneration
            {
                ReportId = request.Id,
                Title = request.Title,
                Sections = basicSections,
                Charts = basicCharts,
                Tables = basicTables,
                AdvancedVisualizations = advancedVisualizations,
                InteractiveDashboard = interactiveDashboard,
                Status = "Complete",
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            var partial = new PartialReport
            {
                ReportId = request.Id,
                Title = request.Title,
                CompletedSections = GenerateBasicSections(request),
                AvailableCharts = GenerateBasicCharts(request),
                BasicTables = GenerateBasicTables(request)
            };
            
            return MlResult<ReportGeneration>.FailWithValue(
                partial,
                $"Advanced report generation failed: {ex.Message}"
            );
        }
    }

    // Métodos auxiliares
    private AnalysisResults CreateBasicAnalysis(IntermediateDataset intermediate)
    {
        return new AnalysisResults
        {
            RecordCount = intermediate.CleanedData.Count(),
            FieldStatistics = CalculateBasicStatistics(intermediate.CleanedData),
            QualityMetrics = new QualityMetrics { BasicScore = 0.7 },
            AnalysisType = "Basic"
        };
    }

    private double CalculateBasicQuality(IntermediateDataset intermediate)
    {
        var completenessScore = CalculateCompleteness(intermediate.CleanedData);
        var validityScore = intermediate.ValidationResults.OverallScore;
        return (completenessScore + validityScore) / 2.0;
    }

    private List<Chart> FilterBasicCharts(List<Chart> availableCharts)
    {
        return availableCharts
            .Where(chart => chart.Type == "Bar" || chart.Type == "Line" || chart.Type == "Pie")
            .ToList();
    }

    private async Task<PerformanceMetrics> CalculateBasicMetricsAsync(BasicTransformation basicTransform)
    {
        await Task.Delay(10);
        return new PerformanceMetrics
        {
            ProcessingTime = TimeSpan.FromSeconds(5),
            MemoryUsage = "50MB",
            ThroughputRecordsPerSecond = 1000,
            ErrorRate = 0.05
        };
    }

    private string DetermineBasicStatus(BasicValidation basic)
    {
        var passedChecks = new[] { basic.SchemaValidation, basic.DataTypeValidation, basic.NullValueCheck }
            .Count(check => check.Status == "Passed");
            
        return passedChecks >= 2 ? "Acceptable" : "Needs Review";
    }
}

// Clases de apoyo para el pipeline de datos
public class ProcessedDataset
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<CleanRecord> CleanedData { get; set; }
    public ValidationResults ValidationResults { get; set; }
    public IEnumerable<EnrichedRecord> EnrichedData { get; set; }
    public AnalysisResults AnalysisResults { get; set; }
    public double QualityScore { get; set; }
    public string ProcessingStatus { get; set; }
    public List<string> ProcessingSteps { get; set; }
    public List<string> FailedSteps { get; set; }
    public string Warning { get; set; }
}

public class IntermediateDataset
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<CleanRecord> CleanedData { get; set; }
    public ValidationResults ValidationResults { get; set; }
    public List<string> CompletedSteps { get; set; }
}

public class ReportGeneration
{
    public string ReportId { get; set; }
    public string Title { get; set; }
    public List<ReportSection> Sections { get; set; }
    public List<Chart> Charts { get; set; }
    public List<Table> Tables { get; set; }
    public List<Visualization> AdvancedVisualizations { get; set; }
    public InteractiveDashboard InteractiveDashboard { get; set; }
    public string Status { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string Warning { get; set; }
}

public class PartialReport
{
    public string ReportId { get; set; }
    public string Title { get; set; }
    public List<ReportSection> CompletedSections { get; set; }
    public List<Chart> AvailableCharts { get; set; }
    public List<Table> BasicTables { get; set; }
}
```

---

## Comparación con Otros Patrones

### MapIfFailWithValue vs MapIfFail

```csharp
// MapIfFail: Solo información del error
var result = ProcessData(input)
    .MapIfFail(error => GetDefaultData());  // Solo mensaje de error

// MapIfFailWithValue: Acceso a valor preservado
var result = ProcessDataWithPreservation(input)
    .MapIfFailWithValue(preservedData => EnhancePreservedData(preservedData));
```

### MapIfFailWithValue vs Match

```csharp
// Match: Manejo manual completo
var result = GetDataWithValue(input)
    .Match(
        fail: error => 
        {
            var preserved = error.GetDetailValue<PartialData>();
            return preserved.Map(data => ProcessPartial(data));
        },
        valid: data => ProcessComplete(data)
    );

// MapIfFailWithValue: Patrón específico para valores preservados
var result = GetDataWithValue(input)
    .MapIfFailWithValue(preservedData => ProcessPartial(preservedData));
```

### MapIfFailWithValue vs Bind

```csharp
// Bind: Continuación que puede fallar
var result = GetData()
    .Bind(data => ProcessData(data));  // ProcessData puede retornar error

// MapIfFailWithValue: Recuperación que no falla
var result = GetDataWithPreservation()
    .MapIfFailWithValue(partial => CompleteFromPartial(partial));  // Siempre éxito
```

---

## Mejores Prácticas

### 1. Preservación Inteligente de Valores

```csharp
// ✅ Correcto: Preservar valores que pueden ser útiles para recuperación
public MlResult<CompleteProfile> BuildUserProfile(UserData data)
{
    try
    {
        var basicProfile = CreateBasicProfile(data);  // Siempre funciona
        var enhancedProfile = EnhanceWithExternalData(basicProfile);  // Puede fallar
        return enhancedProfile;
    }
    catch (Exception ex)
    {
        return MlResult<CompleteProfile>.FailWithValue(
            basicProfile,  // Valor útil para recuperación
            $"Profile enhancement failed: {ex.Message}"
        );
    }
}

// ✅ Correcto: Preservar estado intermedio valioso
public MlResult<ProcessedDocument> ProcessDocument(RawDocument doc)
{
    var validated = ValidateDocument(doc);
    var parsed = ParseStructure(validated);  // Estado valioso
    
    try
    {
        return EnrichWithMetadata(parsed);  // Puede fallar
    }
    catch (Exception ex)
    {
        return MlResult<ProcessedDocument>.FailWithValue(
            parsed,  // Documento parseado es útil
            $"Metadata enrichment failed: {ex.Message}"
        );
    }
}

// ❌ Incorrecto: Preservar valores que no son útiles para recuperación
public MlResult<Result> ProcessData(Input input)
{
    try
    {
        return ExpensiveOperation(input);
    }
    catch (Exception ex)
    {
        return MlResult<Result>.FailWithValue(
            input,  // Input original no ayuda en recuperación
            ex.Message
        );
    }
}
```

### 2. Funciones de Recuperación Apropiadas

```csharp
// ✅ Correcto: Función de recuperación que mejora el valor preservado
var result = ParseComplexData(input)
    .MapIfFailWithValue(partialData => new ComplexData
    {
        ValidatedFields = partialData.ValidatedFields,
        ParsedSections = partialData.ParsedSections,
        DefaultFields = GetDefaultValues(),  // Completar campos faltantes
        IsPartial = true,
        Source = "partial-recovery"
    });

// ✅ Correcto: Recuperación que aprovecha lo que se pudo procesar
var result = GenerateReport(request)
    .MapIfFailWithValue(partialReport => new Report
    {
        CompletedSections = partialReport.CompletedSections,
        GeneratedCharts = partialReport.GeneratedCharts,
        MissingElements = GetMissingElementsList(partialReport),
        Warning = "Report generated with partial data"
    });

// ❌ Incorrecto: Función de recuperación que ignora el valor preservado
var badResult = ProcessData(input)
    .MapIfFailWithValue(preservedData => GetCompletelyDifferentData());  // Ignora preserved
```

### 3. Manejo de Tipos Apropiado

```csharp
// ✅ Correcto: Tipos específicos para diferentes contextos
public MlResult<DisplayUser> GetUserForDisplay(int userId)
{
    return GetCompleteUser(userId)
        .MapIfFailWithValue<CompleteUser, BasicUser, DisplayUser>(
            funcValid: complete => new DisplayUser
            {
                Name = complete.FullName,
                Avatar = complete.ProfilePicture,
                Status = complete.OnlineStatus,
                Badges = complete.Achievements
            },
            funcFail: basic => new DisplayUser
            {
                Name = basic.Name ?? "Unknown User",
                Avatar = "/images/default-avatar.png",
                Status = "offline",
                Badges = new List<Badge>()
            }
        );
}

// ✅ Correcto: Preservación de tipo intermedio específico
public class ProcessingState  // Tipo específico para estado intermedio
{
    public List<ValidatedRecord> ProcessedRecords { get; set; }
    public List<string> CompletedSteps { get; set; }
    public Dictionary<string, object> IntermediateResults { get; set; }
}

var result = ProcessRecords(input)
    .MapIfFailWithValue<FinalResult, ProcessingState, FinalResult>(
        funcValid: final => final,
        funcFail: state => CreatePartialResult(state)
    );

// ❌ Incorrecto: Usar tipos genéricos que pierden información
var badResult = ProcessData(input)
    .MapIfFailWithValue<Result, object, Result>(  // object pierde tipo específico
        funcValid: result => result,
        funcFail: obj => CreateFromGeneric(obj)  // Conversión peligrosa
    );
```

### 4. Logging y Monitoreo de Recuperaciones

```csharp
// ✅ Correcto: Log detallado de recuperaciones
var result = ProcessOrder(order)
    .MapIfFailWithValue(partialOrder =>
    {
        _logger.LogWarning("Order processing failed, using partial data. OrderId: {OrderId}, ProcessedItems: {Count}",
                          partialOrder.OrderId, partialOrder.ProcessedItems.Count);
        
        _metrics.IncrementCounter("order.partial_recovery");
        
        return CreatePartialOrderResult(partialOrder);
    });

// ✅ Correcto: Métricas de calidad de recuperación
var result = AnalyzeData(dataset)
    .MapIfFailWithValue(partialAnalysis =>
    {
        var completionRate = (double)partialAnalysis.CompletedAnalyses.Count / partialAnalysis.TotalAnalyses;
        
        _logger.LogInformation("Data analysis partially completed. Completion rate: {Rate:P}",
                              completionRate);
        
        _metrics.RecordValue("analysis.completion_rate", completionRate);
        
        return CreatePartialAnalysisResult(partialAnalysis);
    });

// ❌ Incorrecto: Recuperación silenciosa sin visibilidad
var badResult = ProcessData(input)
    .MapIfFailWithValue(partial => CreateFromPartial(partial));  // Sin logging
```

### 5. Testing de Escenarios de Recuperación

```csharp
// ✅ Correcto: Tests específicos para recuperación con valores
[Test]
public void ProcessDocument_WhenMetadataExtractionFails_ShouldRecoverWithParsedContent()
{
    // Arrange
    var document = CreateTestDocument();
    var mockService = new Mock<IMetadataService>();
    mockService.Setup(s => s.ExtractMetadata(It.IsAny<ParsedDocument>()))
               .Throws(new ExternalServiceException("Metadata service unavailable"));

    // Act
    var result = _documentProcessor.ProcessDocument(document);

    // Assert
    Assert.That(result.IsValid, Is.True);
    Assert.That(result.Value.Content, Is.Not.Null);  // Contenido parseado preservado
    Assert.That(result.Value.Metadata, Is.Empty);    // Metadata por defecto
    Assert.That(result.Value.IsPartial, Is.True);    // Marcado como parcial
}

[Test]
public void GenerateReport_WhenAdvancedChartsFaill_ShouldRecoverWithBasicCharts()
{
    // Arrange
    var request = CreateReportRequest();
    var mockChartService = new Mock<IAdvancedChartService>();
    mockChartService.Setup(s => s.GenerateAdvancedCharts(It.IsAny<ReportData>()))
                    .Throws(new ChartGenerationException("Complex visualization failed"));

    // Act
    var result = _reportGenerator.GenerateReport(request);

    // Assert
    Assert.That(result.IsValid, Is.True);
    Assert.That(result.Value.BasicCharts, Is.Not.Empty);      // Charts básicos funcionan
    Assert.That(result.Value.AdvancedCharts, Is.Empty);       // Charts avanzados fallan
    Assert.That(result.Value.Warning, Contains.Substring("partial"));
}
```

### 6. Composición con Otros Operadores

```csharp
// ✅ Correcto: MapIfFailWithValue en pipeline de recuperación
var result = GetRawData(id)
    .Map(data => ValidateData(data))                    // Validación normal
    .MapIfFailWithValue(partialData => 
        RepairPartialData(partialData))                 // Recuperación con datos parciales
    .MapEnsure(data => data.IsUsable, 
        "Data must be usable after recovery")          // Validación post-recuperación
    .Map(data => ProcessRepairedData(data));            // Procesamiento final

// ✅ Correcto: Cadena de recuperaciones
var result = ProcessLevel1(input)
    .MapIfFailWithValue(partial1 => ProcessLevel2Fallback(partial1))
    .MapIfFailWithValue(partial2 => ProcessLevel3Fallback(partial2))
    .MapIfFail(error => GetUltimateDefault());          // Fallback final sin valor

// ❌ Incorrecto: Uso innecesario cuando no hay valores preservados
var badResult = GetSimpleValue()
    .MapIfFailWithValue(value => ProcessValue(value))   // GetSimpleValue no preserva valores
    .MapIfFail(error => GetDefault());                  // Redundante
```

---

## Consideraciones de Rendimiento

### Costo de Preservación de Valores

- **Memory Overhead**: Preservar valores intermedios consume memoria adicional
- **Serialization Cost**: Si los valores se serializan, considerar el costo
- **Cleanup**: Asegurar que los valores preservados se liberan apropiadamente

### Optimizaciones Específicas

```csharp
// ✅ Optimización: Preservar solo datos esenciales
public class OptimizedPartialResult
{
    public List<int> ProcessedIds { get; set; }      // Solo IDs, no objetos completos
    public Dictionary<string, string> KeyFields { get; set; }  // Solo campos clave
    public int TotalProcessed { get; set; }          // Métricas, no datos
}

// ✅ Optimización: Lazy evaluation de recuperación
var result = ProcessData(input)
    .MapIfFailWithValue(partial => 
    {
        // Solo procesar si realmente es necesario
        if (partial.ProcessedCount > threshold)
            return CreateRichRecovery(partial);
        else
            return CreateMinimalRecovery(partial);
    });

// ❌ Ineficiente: Preservar objetos grandes innecesarios
public class WastefulPartialResult
{
    public byte[] RawData { get; set; }              // Datos raw grandes
    public List<ComplexObject> AllIntermediates { get; set; }  // Todos los intermedios
    public FullProcessingContext Context { get; set; }         // Contexto completo
}
```

---

## Resumen

La clase `MlResultActionsMapIfFailWithValue` implementa **recuperación inteligente de errores**:

- **`MapIfFailWithValue<T>`**: Recuperación usando valores preservados del mismo tipo
- **`MapIfFailWithValue<T,TValue,TReturn>`**: Recuperación con diferentes tipos para entrada, valor preservado y salida
- **`MapIfFailWithValueAsync`**: Soporte completo para operaciones asíncronas
- **`TryMapIfFailWithValue`**: Versiones seguras que capturan excepciones

Estas operaciones son ideales para:

- **Partial Success Recovery**: Recuperar de errores usando resultados parciales exitosos
- **Progressive Degradation**: Fallar gradualmente manteniendo funcionalidad parcial
- **Context-Aware Fallbacks**: Fallbacks que consideran el estado antes del fallo
- **Value-Preserving Operations**: Operaciones que no pierden trabajo parcial realizado

La diferencia clave con `MapIfFail` es el **acceso a valores preservados** durante el fallo, permitiendo recuperaciones más inteligentes y contextuales que aprovechan el trabajo parcial ya realizado.