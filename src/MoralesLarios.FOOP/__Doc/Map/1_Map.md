# MlResultActionsMap - Operaciones de Transformación de Valores

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos Map Básicos](#métodos-map-básicos)
4. [Métodos TryMap - Captura de Excepciones](#métodos-trymap---captura-de-excepciones)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Diferencias con Bind](#diferencias-con-bind)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsMap` contiene las operaciones fundamentales de **mapping** o transformación de valores para `MlResult<T>`. Estas operaciones permiten transformar el valor contenido en un `MlResult` exitoso manteniendo la estructura monádica, implementando el patrón **Functor** de la programación funcional.

### Propósito Principal

- **Transformación de Valores**: Aplicar funciones de transformación al valor contenido
- **Preservación de Estado**: Los errores se propagan sin ejecutar la transformación
- **Manejo Seguro**: Versiones `Try*` que capturan excepciones durante la transformación
- **Soporte Asíncrono Completo**: Todas las combinaciones de operaciones síncronas y asíncronas

### Diferencia Clave con Bind

```
Map:  MlResult<T> + (T → U) → MlResult<U>     // Función devuelve U
Bind: MlResult<T> + (T → MlResult<U>) → MlResult<U>  // Función devuelve MlResult<U>
```

---

## Análisis de la Clase

### Estructura y Filosofía

Map implementa el patrón **Functor**, permitiendo aplicar transformaciones puras a valores encapsulados:

```
Resultado Exitoso(T) → Función(T → U) → Resultado Exitoso(U)
      ↓                                        ↓
  Resultado Fallido ――――――――――――――――――――→ Resultado Fallido
```

### Características Principales

1. **Transformación Pura**: Las funciones map no cambian la estructura del contenedor
2. **Preservación de Errores**: Los errores se propagan sin cambios
3. **Composición Funcional**: Las transformaciones se pueden componer fácilmente
4. **Simplicidad**: Para transformaciones que no pueden fallar conceptualmente

---

## Métodos Map Básicos

### `Map<T, TReturn>()`

**Propósito**: Transforma el valor contenido aplicando una función de transformación pura

```csharp
public static MlResult<TReturn> Map<T, TReturn>(this MlResult<T> source, 
                                                Func<T, TReturn> func)
```

**Parámetros**:
- `source`: El resultado a transformar
- `func`: Función de transformación que convierte `T` en `TReturn`

**Comportamiento**:
- Si `source` es exitoso: Aplica `func(value)` y retorna `MlResult<TReturn>.Valid(result)`
- Si `source` es fallido: Retorna el error sin ejecutar `func`

**Signatura Functorial**: `F<T> → (T → U) → F<U>`

### `MapAsync<T, TReturn>()` - Conversión a Asíncrono

**Propósito**: Convierte el resultado de `Map` a `Task<MlResult<TReturn>>`

```csharp
public static Task<MlResult<TReturn>> MapAsync<T, TReturn>(this MlResult<T> source, 
                                                           Func<T, TReturn> func)
```

**Comportamiento**: Ejecuta `Map` y envuelve el resultado en una `Task`

### `MapAsync<T, TReturn>()` - Función Asíncrona

**Propósito**: Aplica una función de transformación asíncrona

```csharp
public static async Task<MlResult<TReturn>> MapAsync<T, TReturn>(this MlResult<T> source,
                                                                 Func<T, Task<TReturn>> funcAsync)
```

**Comportamiento**:
- Si `source` es exitoso: Ejecuta `await funcAsync(value)`
- Si `source` es fallido: Retorna el error envuelto en `Task`

### `MapAsync<T, TReturn>()` - Fuente Asíncrona con Función Asíncrona

**Propósito**: Transforma desde un `Task<MlResult<T>>` con función asíncrona

```csharp
public static async Task<MlResult<TReturn>> MapAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, Task<TReturn>> funcAsync)
```

### `MapAsync<T, TReturn>()` - Fuente Asíncrona con Función Síncrona

**Propósito**: Transforma desde un `Task<MlResult<T>>` con función síncrona

```csharp
public static async Task<MlResult<TReturn>> MapAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, TReturn> func)
```

---

## Métodos TryMap - Captura de Excepciones

### `TryMap<T, TReturn>()` - Mensaje Simple

**Propósito**: Versión segura de `Map` que captura excepciones durante la transformación

```csharp
public static MlResult<TReturn> TryMap<T, TReturn>(this MlResult<T> source, 
                                                   Func<T, TReturn> func,
                                                   string exceptionAditionalMessage = null!)
```

**Comportamiento**:
- Si `source` es exitoso y `func` no lanza excepción: Retorna `MlResult<TReturn>.Valid(result)`
- Si `source` es exitoso y `func` lanza excepción: Captura la excepción y retorna error
- Si `source` es fallido: Propaga el error original

### `TryMap<T, TReturn>()` - Constructor de Mensaje

**Propósito**: Versión con función constructora de mensaje de error personalizado

```csharp
public static MlResult<TReturn> TryMap<T, TReturn>(this MlResult<T> source, 
                                                   Func<T, TReturn> func,
                                                   Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: Permite construir mensajes de error específicos basados en la excepción capturada

### Versiones Asíncronas de TryMap

Cada variante de `TryMap` tiene sus correspondientes versiones asíncronas:

- `TryMapAsync` con mensaje simple
- `TryMapAsync` con constructor de mensaje
- `TryMapAsync` para funciones asíncronas
- `TryMapAsync` para fuentes asíncronas

---

## Variantes Asíncronas

### Matriz de Combinaciones

| Fuente | Función | Método |
|--------|---------|---------|
| `MlResult<T>` | `T → U` | `Map` |
| `MlResult<T>` | `T → U` | `MapAsync` (conversión) |
| `MlResult<T>` | `T → Task<U>` | `MapAsync` |
| `Task<MlResult<T>>` | `T → Task<U>` | `MapAsync` |
| `Task<MlResult<T>>` | `T → U` | `MapAsync` |

### Soporte Completo para TryMap

Todas las combinaciones anteriores están disponibles también para `TryMap`:

- Con mensaje de excepción simple (`string`)
- Con constructor de mensaje (`Func<Exception, string>`)

---

## Ejemplos Prácticos

### Ejemplo 1: Transformaciones de Datos Básicas

```csharp
public class UserProfileService
{
    public MlResult<UserProfileView> GetUserProfile(int userId)
    {
        return GetUserById(userId)
            .Map(user => new UserProfileView
            {
                Id = user.Id,
                DisplayName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                JoinDate = user.CreatedAt.ToString("MMMM yyyy"),
                IsActive = user.IsActive,
                ProfileImageUrl = GenerateProfileImageUrl(user)
            });
    }
    
    public MlResult<UserSummary> GetUserSummary(int userId)
    {
        return GetUserById(userId)
            .Map(user => user.Email.ToLower())  // Normalizar email
            .Map(email => email.Split('@'))     // Dividir email
            .Map(emailParts => new UserSummary
            {
                Username = emailParts[0],
                Domain = emailParts[1],
                AccountType = DetermineAccountType(emailParts[1])
            });
    }
    
    public MlResult<List<ProductView>> GetUserProducts(int userId)
    {
        return GetUserById(userId)
            .Map(user => user.PurchasedProducts)
            .Map(products => products.Where(p => p.IsActive).ToList())
            .Map(activeProducts => activeProducts.Select(p => new ProductView
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PurchaseDate = p.PurchaseDate.ToString("dd/MM/yyyy"),
                Category = p.Category.Name
            }).ToList());
    }
    
    private MlResult<User> GetUserById(int userId)
    {
        // Simulación de obtención de usuario
        if (userId <= 0)
            return MlResult<User>.Fail("Invalid user ID");
            
        var users = GetMockUsers();
        var user = users.FirstOrDefault(u => u.Id == userId);
        
        return user != null 
            ? MlResult<User>.Valid(user)
            : MlResult<User>.Fail($"User with ID {userId} not found");
    }
    
    private string GenerateProfileImageUrl(User user)
    {
        var initials = $"{user.FirstName[0]}{user.LastName[0]}";
        return $"https://ui-avatars.com/api/?name={initials}&background=random";
    }
    
    private string DetermineAccountType(string domain)
    {
        var corporateDomains = new[] { "company.com", "business.org", "enterprise.net" };
        return corporateDomains.Contains(domain.ToLower()) ? "Corporate" : "Personal";
    }
    
    private List<User> GetMockUsers()
    {
        return new List<User>
        {
            new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@company.com",
                CreatedAt = DateTime.UtcNow.AddMonths(-6),
                IsActive = true,
                PurchasedProducts = new List<Product>
                {
                    new Product { Id = 101, Name = "Laptop Pro", Price = 1299.99m, 
                                IsActive = true, PurchaseDate = DateTime.UtcNow.AddDays(-30),
                                Category = new Category { Name = "Electronics" } },
                    new Product { Id = 102, Name = "Wireless Mouse", Price = 49.99m, 
                                IsActive = true, PurchaseDate = DateTime.UtcNow.AddDays(-15),
                                Category = new Category { Name = "Accessories" } }
                }
            }
        };
    }
}

// Clases de apoyo
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public List<Product> PurchasedProducts { get; set; } = new List<Product>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime PurchaseDate { get; set; }
    public Category Category { get; set; }
}

public class Category
{
    public string Name { get; set; }
}

public class UserProfileView
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string JoinDate { get; set; }
    public bool IsActive { get; set; }
    public string ProfileImageUrl { get; set; }
}

public class UserSummary
{
    public string Username { get; set; }
    public string Domain { get; set; }
    public string AccountType { get; set; }
}

public class ProductView
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string PurchaseDate { get; set; }
    public string Category { get; set; }
}
```

### Ejemplo 2: Transformaciones Asíncronas con Servicios Externos

```csharp
public class DocumentProcessingService
{
    private readonly IFileService _fileService;
    private readonly ITranslationService _translationService;
    private readonly IMetadataService _metadataService;
    
    public DocumentProcessingService(
        IFileService fileService,
        ITranslationService translationService,
        IMetadataService metadataService)
    {
        _fileService = fileService;
        _translationService = translationService;
        _metadataService = metadataService;
    }
    
    public async Task<MlResult<ProcessedDocument>> ProcessDocumentAsync(string documentPath)
    {
        return await LoadDocument(documentPath)
            .MapAsync(async doc => await ExtractTextAsync(doc))
            .MapAsync(async text => await TranslateTextAsync(text))
            .MapAsync(async translatedText => await GenerateMetadataAsync(translatedText))
            .MapAsync(metadata => new ProcessedDocument
            {
                OriginalPath = documentPath,
                ProcessedText = metadata.TranslatedText,
                Metadata = metadata,
                ProcessedAt = DateTime.UtcNow,
                ProcessingDuration = DateTime.UtcNow - metadata.StartTime
            });
    }
    
    public async Task<MlResult<DocumentSummary>> CreateDocumentSummaryAsync(string documentPath)
    {
        return await LoadDocument(documentPath)
            .MapAsync(doc => doc.Name)  // Extraer nombre
            .MapAsync(async name => await _fileService.GetFileSizeAsync(documentPath))  // Obtener tamaño
            .MapAsync(async size => await _metadataService.GetFileTypeAsync(documentPath))  // Obtener tipo
            .MapAsync(metadata => new DocumentSummary
            {
                Name = Path.GetFileNameWithoutExtension(documentPath),
                Extension = Path.GetExtension(documentPath),
                SizeInBytes = metadata.Size,
                SizeFormatted = FormatFileSize(metadata.Size),
                FileType = metadata.Type,
                CreatedAt = DateTime.UtcNow
            });
    }
    
    public async Task<MlResult<List<ProcessedDocument>>> ProcessMultipleDocumentsAsync(string[] documentPaths)
    {
        var tasks = documentPaths.Select(async path => await ProcessDocumentAsync(path));
        var results = await Task.WhenAll(tasks);
        
        // Filtrar solo los resultados exitosos y transformar a lista
        return results
            .Where(r => r.IsValid)
            .Select(r => r.Value)
            .ToList()
            .ToMlResult()
            .Map(docs => docs.OrderBy(d => d.ProcessedAt).ToList());
    }
    
    private MlResult<Document> LoadDocument(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return MlResult<Document>.Fail("Document path cannot be empty");
            
        if (!File.Exists(path))
            return MlResult<Document>.Fail($"Document not found: {path}");
            
        var document = new Document
        {
            Path = path,
            Name = Path.GetFileName(path),
            LoadedAt = DateTime.UtcNow
        };
        
        return MlResult<Document>.Valid(document);
    }
    
    private async Task<string> ExtractTextAsync(Document document)
    {
        // Simulación de extracción de texto
        await Task.Delay(100);
        return $"Extracted text from {document.Name}";
    }
    
    private async Task<string> TranslateTextAsync(string text)
    {
        return await _translationService.TranslateAsync(text, "en", "es");
    }
    
    private async Task<DocumentMetadata> GenerateMetadataAsync(string translatedText)
    {
        var metadata = await _metadataService.GenerateMetadataAsync(translatedText);
        return new DocumentMetadata
        {
            TranslatedText = translatedText,
            WordCount = translatedText.Split(' ').Length,
            CharacterCount = translatedText.Length,
            Language = "es",
            StartTime = DateTime.UtcNow.AddMinutes(-1),  // Simulado
            Size = 0,
            Type = "text"
        };
    }
    
    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        
        return $"{len:0.##} {sizes[order]}";
    }
}

// Clases de apoyo e interfaces
public class Document
{
    public string Path { get; set; }
    public string Name { get; set; }
    public DateTime LoadedAt { get; set; }
}

public class DocumentMetadata
{
    public string TranslatedText { get; set; }
    public int WordCount { get; set; }
    public int CharacterCount { get; set; }
    public string Language { get; set; }
    public DateTime StartTime { get; set; }
    public long Size { get; set; }
    public string Type { get; set; }
}

public class ProcessedDocument
{
    public string OriginalPath { get; set; }
    public string ProcessedText { get; set; }
    public DocumentMetadata Metadata { get; set; }
    public DateTime ProcessedAt { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
}

public class DocumentSummary
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public long SizeInBytes { get; set; }
    public string SizeFormatted { get; set; }
    public string FileType { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface IFileService
{
    Task<long> GetFileSizeAsync(string path);
}

public interface ITranslationService
{
    Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage);
}

public interface IMetadataService
{
    Task<DocumentMetadata> GenerateMetadataAsync(string text);
    Task<FileTypeInfo> GetFileTypeAsync(string path);
}

public class FileTypeInfo
{
    public string Type { get; set; }
    public long Size { get; set; }
}
```

### Ejemplo 3: TryMap para Transformaciones que Pueden Fallar

```csharp
public class DataFormatterService
{
    public MlResult<FormattedData> FormatDataSafely(RawInputData rawData)
    {
        return ValidateRawData(rawData)
            .TryMap(data => ParseDateString(data.DateString), "Failed to parse date")
            .TryMap(date => FormatCurrency(rawData.AmountString), ex => $"Currency formatting failed: {ex.Message}")
            .TryMap(amount => NormalizeText(rawData.TextContent), "Text normalization failed")
            .TryMap(text => new FormattedData
            {
                FormattedDate = rawData.DateString,  // Ya validado
                FormattedAmount = rawData.AmountString,  // Ya validado
                NormalizedText = text,
                ProcessedAt = DateTime.UtcNow
            }, ex => $"Failed to create formatted data: {ex.Message}");
    }
    
    public async Task<MlResult<BatchFormattedData>> FormatDataBatchAsync(List<RawInputData> rawDataList)
    {
        var formattedResults = new List<FormattedData>();
        var errors = new List<string>();
        
        return await rawDataList.ToMlResult()
            .TryMapAsync(async dataList =>
            {
                foreach (var rawData in dataList)
                {
                    var result = FormatDataSafely(rawData);
                    if (result.IsValid)
                        formattedResults.Add(result.Value);
                    else
                        errors.AddRange(result.ErrorsDetails.Select(e => e.ErrorMessage));
                }
                
                return new BatchFormattedData
                {
                    SuccessfulItems = formattedResults,
                    FailedCount = errors.Count,
                    Errors = errors,
                    ProcessedAt = DateTime.UtcNow
                };
            }, ex => $"Batch processing failed: {ex.Message}");
    }
    
    public MlResult<JsonFormattedData> FormatToJsonSafely(object data)
    {
        return data.ToMlResult()
            .TryMap(obj => SerializeToJson(obj), "JSON serialization failed")
            .TryMap(json => CompressJson(json), ex => $"JSON compression failed: {ex.GetType().Name}")
            .TryMap(compressed => ValidateJsonStructure(compressed), "JSON validation failed")
            .TryMap(validJson => new JsonFormattedData
            {
                OriginalSize = JsonSerializer.Serialize(data).Length,
                CompressedJson = validJson,
                CompressedSize = validJson.Length,
                CompressionRatio = (double)validJson.Length / JsonSerializer.Serialize(data).Length,
                CreatedAt = DateTime.UtcNow
            });
    }
    
    private MlResult<RawInputData> ValidateRawData(RawInputData rawData)
    {
        if (rawData == null)
            return MlResult<RawInputData>.Fail("Raw data cannot be null");
            
        if (string.IsNullOrWhiteSpace(rawData.DateString))
            return MlResult<RawInputData>.Fail("Date string is required");
            
        if (string.IsNullOrWhiteSpace(rawData.AmountString))
            return MlResult<RawInputData>.Fail("Amount string is required");
            
        return MlResult<RawInputData>.Valid(rawData);
    }
    
    // Método que puede lanzar FormatException
    private DateTime ParseDateString(string dateString)
    {
        // Intentar varios formatos de fecha
        var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "MM-dd-yyyy", "yyyy/MM/dd" };
        
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, 
                DateTimeStyles.None, out var date))
            {
                return date;
            }
        }
        
        // Si ningún formato funciona, lanzar excepción
        throw new FormatException($"Unable to parse date string '{dateString}' with any known format");
    }
    
    // Método que puede lanzar ArgumentException
    private decimal FormatCurrency(string amountString)
    {
        // Limpiar el string de caracteres no numéricos excepto punto y coma
        var cleanAmount = Regex.Replace(amountString, @"[^\d.,\-]", "");
        
        if (string.IsNullOrWhiteSpace(cleanAmount))
            throw new ArgumentException("Amount string contains no numeric characters");
            
        // Intentar parsear como decimal
        if (decimal.TryParse(cleanAmount, NumberStyles.Currency, CultureInfo.InvariantCulture, out var amount))
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");
                
            return amount;
        }
        
        throw new FormatException($"Unable to parse amount string '{amountString}' as decimal");
    }
    
    // Método que puede lanzar InvalidOperationException
    private string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Text content cannot be empty for normalization");
            
        // Normalización que puede fallar
        try
        {
            var normalized = text.Trim()
                                .ToLowerInvariant()
                                .Replace("  ", " ");  // Reemplazar espacios dobles
            
            if (normalized.Length > 1000)
                throw new InvalidOperationException("Normalized text exceeds maximum length of 1000 characters");
                
            return normalized;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Text normalization failed: {ex.Message}", ex);
        }
    }
    
    // Método que puede lanzar JsonException
    private string SerializeToJson(object obj)
    {
        try
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            throw new JsonException($"Failed to serialize object to JSON: {ex.Message}", ex);
        }
    }
    
    // Método que puede lanzar CompressionException
    private string CompressJson(string json)
    {
        if (json.Length < 100)  // No comprimir strings pequeños
            return json;
            
        try
        {
            // Simulación de compresión - en realidad solo removemos espacios adicionales
            var compressed = Regex.Replace(json, @"\s+", " ").Trim();
            
            if (compressed.Length >= json.Length)
                throw new InvalidOperationException("Compression did not reduce size");
                
            return compressed;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"JSON compression failed: {ex.Message}", ex);
        }
    }
    
    // Método que puede lanzar ValidationException
    private string ValidateJsonStructure(string json)
    {
        try
        {
            // Validar que es JSON válido
            using var document = JsonDocument.Parse(json);
            
            // Validaciones adicionales
            if (json.Length == 0)
                throw new ValidationException("JSON cannot be empty");
                
            if (!json.StartsWith("{") && !json.StartsWith("["))
                throw new ValidationException("JSON must start with