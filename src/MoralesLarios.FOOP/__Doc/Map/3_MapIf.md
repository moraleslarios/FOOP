# MlResultActionsMapIf - Operaciones de Transformación Condicional

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos MapIf Básicos](#métodos-mapif-básicos)
4. [Variantes con Mismo Tipo](#variantes-con-mismo-tipo)
5. [Métodos TryMapIf - Captura de Excepciones](#métodos-trymapif---captura-de-excepciones)
6. [Variantes Asíncronas](#variantes-asíncronas)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Comparación con Otros Patrones](#comparación-con-otros-patrones)
9. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsMapIf` implementa operaciones de **transformación condicional** que permiten aplicar diferentes funciones de transformación basadas en una condición evaluada sobre el valor contenido en un `MlResult<T>`. Es una implementación funcional del patrón **if-then-else** que mantiene la estructura monádica.

### Propósito Principal

- **Bifurcación de Transformaciones**: Aplicar diferentes lógicas según condiciones
- **Ternary Operator Funcional**: Implementación funcional de `condition ? funcTrue : funcFalse`
- **Branching sin Match**: Ramificación manteniendo el contexto de `MlResult`
- **Transformaciones Contextuales**: Decidir transformación basada en el valor actual

### Filosofía de Diseño

```
Valor Válido + Condición Verdadera  → funcTrue(valor) → MlResult<TReturn>
Valor Válido + Condición Falsa      → funcFalse(valor) → MlResult<TReturn>
Valor Inválido                      → Error (propagación sin evaluación)
```

---

## Análisis de la Clase

### Patrón de Funcionamiento

`MapIf` implementa una **transformación condicional** donde:

1. Si el `MlResult<T>` es fallido, propaga el error sin evaluación
2. Si el `MlResult<T>` es exitoso, evalúa la condición
3. Aplica `funcTrue` si la condición es verdadera
4. Aplica `funcFalse` si la condición es falsa
5. Ambas funciones devuelven un valor del mismo tipo de retorno

### Estructura Conceptual

```csharp
// Signatura básica
MlResult<T> → (T → bool) → (T → U) → (T → U) → MlResult<U>

// Flujo lógico
if (mlResult.IsValid) {
    var value = mlResult.Value;
    var transformedValue = condition(value) ? funcTrue(value) : funcFalse(value);
    return MlResult<U>.Valid(transformedValue);
}
return MlResult<U>.Fail(mlResult.ErrorsDetails);
```

---

## Métodos MapIf Básicos

### `MapIf<T, TReturn>()` - Transformación Condicional con Cambio de Tipo

**Propósito**: Aplica una de dos funciones de transformación basada en una condición

```csharp
public static MlResult<TReturn> MapIf<T, TReturn>(this MlResult<T> source,
                                                  Func<T, bool> condition,
                                                  Func<T, TReturn> funcTrue,
                                                  Func<T, TReturn> funcFalse)
```

**Parámetros**:
- `source`: El resultado a transformar
- `condition`: Función que evalúa la condición sobre el valor
- `funcTrue`: Función a aplicar si la condición es verdadera
- `funcFalse`: Función a aplicar si la condición es falsa

**Comportamiento**:
- Si `source` es fallido: Propaga el error sin evaluación
- Si `source` es exitoso: Evalúa `condition(value)` y aplica la función correspondiente

### `MapIfAsync<T, TReturn>()` - Conversión a Asíncrono

**Propósito**: Convierte el resultado de `MapIf` a `Task<MlResult<TReturn>>`

```csharp
public static Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T> source,
                                                             Func<T, bool> condition,
                                                             Func<T, TReturn> funcTrue,
                                                             Func<T, TReturn> funcFalse)
```

### Variantes Asíncronas Mixtas

**Una Función Asíncrona (True)**:
```csharp
public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T> source,
                                                                   Func<T, bool> condition,
                                                                   Func<T, Task<TReturn>> funcTrueAsync,
                                                                   Func<T, TReturn> funcFalse)
```

**Una Función Asíncrona (False)**:
```csharp
public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T> source,
                                                                   Func<T, bool> condition,
                                                                   Func<T, TReturn> funcTrue,
                                                                   Func<T, Task<TReturn>> funcFalseAsync)
```

**Ambas Funciones Asíncronas**:
```csharp
public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T> source,
                                                                   Func<T, bool> condition,
                                                                   Func<T, Task<TReturn>> funcTrueAsync,
                                                                   Func<T, Task<TReturn>> funcFalseAsync)
```

---

## Variantes con Mismo Tipo

### `MapIf<T>()` - Transformación Condicional sin Cambio de Tipo

**Propósito**: Aplica transformación solo si la condición es verdadera, manteniendo el tipo

```csharp
public static MlResult<T> MapIf<T>(this MlResult<T> source,
                                   Func<T, bool> condition,
                                   Func<T, T> func)
```

**Comportamiento**:
- Si `condition(value)` es verdadera: Aplica `func(value)`
- Si `condition(value)` es falsa: Retorna el valor original sin transformar

**Casos de Uso**:
- Normalización condicional de datos
- Aplicación de formatos opcionales
- Transformaciones que solo aplican en ciertos casos

### Variantes Asíncronas del Mismo Tipo

```csharp
// Función asíncrona
public static async Task<MlResult<T>> MapIfAsync<T>(this MlResult<T> source,
                                                    Func<T, bool> condition,
                                                    Func<T, Task<T>> funcAsync)

// Desde fuente asíncrona
public static async Task<MlResult<T>> MapIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                    Func<T, bool> condition,
                                                    Func<T, Task<T>> funcAsync)
```

---

## Métodos TryMapIf - Captura de Excepciones

### `TryMapIf<T, TReturn>()` - Transformación Condicional Segura

**Propósito**: Versión segura que captura excepciones en ambas funciones de transformación

```csharp
public static MlResult<TReturn> TryMapIf<T, TReturn>(this MlResult<T> source,
                                                     Func<T, bool> condition,
                                                     Func<T, TReturn> funcTrue,
                                                     Func<T, TReturn> funcFalse,
                                                     Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Captura excepciones tanto en `funcTrue` como en `funcFalse`
- Convierte cualquier excepción en un `MlResult` fallido
- Permite mensajes de error personalizados

### `TryMapIf<T, TReturn>()` - Mensaje Simple

```csharp
public static MlResult<TReturn> TryMapIf<T, TReturn>(this MlResult<T> source,
                                                     Func<T, bool> condition,
                                                     Func<T, TReturn> funcTrue,
                                                     Func<T, TReturn> funcFalse,
                                                     string exceptionAditionalMessage = null!)
```

### Variantes Asíncronas de TryMapIf

Todas las combinaciones de funciones síncronas/asíncronas están disponibles:

- `TryMapIfAsync` con ambas funciones síncronas
- `TryMapIfAsync` con `funcTrue` asíncrona
- `TryMapIfAsync` con `funcFalse` asíncrona  
- `TryMapIfAsync` con ambas funciones asíncronas
- Versiones desde fuente asíncrona (`Task<MlResult<T>>`)

### `TryMapIf<T>()` - Mismo Tipo con Captura de Excepciones

```csharp
public static MlResult<T> TryMapIf<T>(this MlResult<T> source,
                                      Func<T, bool> condition,
                                      Func<T, T> func,
                                      Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: Solo aplica la función si la condición es verdadera, con captura de excepciones

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Fuente | Condición | funcTrue | funcFalse | Método |
|--------|-----------|----------|-----------|---------|
| `MlResult<T>` | `T → bool` | `T → U` | `T → U` | `MapIf` |
| `MlResult<T>` | `T → bool` | `T → U` | `T → U` | `MapIfAsync` (conversión) |
| `MlResult<T>` | `T → bool` | `T → Task<U>` | `T → U` | `MapIfAsync` |
| `MlResult<T>` | `T → bool` | `T → U` | `T → Task<U>` | `MapIfAsync` |
| `MlResult<T>` | `T → bool` | `T → Task<U>` | `T → Task<U>` | `MapIfAsync` |
| `Task<MlResult<T>>` | `T → bool` | `T → U` | `T → U` | `MapIfAsync` |
| `Task<MlResult<T>>` | `T → bool` | `T → Task<U>` | `T → U` | `MapIfAsync` |
| `Task<MlResult<T>>` | `T → bool` | `T → U` | `T → Task<U>` | `MapIfAsync` |
| `Task<MlResult<T>>` | `T → bool` | `T → Task<U>` | `T → Task<U>` | `MapIfAsync` |

### Soporte Completo para TryMapIf

Todas las combinaciones anteriores están disponibles también para `TryMapIf`:

- Con constructor de mensaje (`Func<Exception, string>`)
- Con mensaje simple (`string`)

---

## Ejemplos Prácticos

### Ejemplo 1: Formateo Condicional de Datos

```csharp
public class DataFormattingService
{
    public MlResult<string> FormatUserDisplayName(User user, bool isAdmin)
    {
        return user.ToMlResult()
            .MapIf(u => isAdmin,
                   u => $"👤 ADMIN: {u.FirstName} {u.LastName} ({u.Role})",
                   u => $"{u.FirstName} {u.LastName}");
    }
    
    public MlResult<string> FormatCurrency(decimal amount, string userLocale)
    {
        return amount.ToMlResult()
            .MapIf(amt => userLocale.StartsWith("en-US"),
                   amt => amt.ToString("C", CultureInfo.GetCultureInfo("en-US")),
                   amt => amt.ToString("C", CultureInfo.GetCultureInfo("es-ES")));
    }
    
    public MlResult<Product> ApplyDiscountIfEligible(Product product, Customer customer)
    {
        return product.ToMlResult()
            .MapIf(p => customer.IsPremium && p.Category == "Electronics",
                   p => new Product
                   {
                       Id = p.Id,
                       Name = p.Name,
                       OriginalPrice = p.Price,
                       Price = p.Price * 0.9m, // 10% descuento
                       Category = p.Category,
                       HasDiscount = true,
                       DiscountReason = "Premium customer electronics discount"
                   },
                   p => p); // Sin descuento
    }
    
    public MlResult<string> FormatPhoneNumber(string phoneNumber, string countryCode)
    {
        return phoneNumber.ToMlResult()
            .MapIf(phone => countryCode == "US",
                   phone => FormatUSPhoneNumber(phone),
                   phone => FormatInternationalPhoneNumber(phone, countryCode));
    }
    
    public MlResult<DateTime> ConvertToUserTimezone(DateTime utcDateTime, string userTimezone)
    {
        return utcDateTime.ToMlResult()
            .TryMapIf(dt => !string.IsNullOrEmpty(userTimezone),
                     dt => TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.FindSystemTimeZoneById(userTimezone)),
                     dt => dt, // Mantener UTC si no hay timezone
                     ex => $"Failed to convert timezone '{userTimezone}': {ex.Message}");
    }
    
    // Métodos auxiliares
    private string FormatUSPhoneNumber(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length == 10)
            return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6)}";
        return phone;
    }
    
    private string FormatInternationalPhoneNumber(string phone, string countryCode)
    {
        return $"+{GetCountryPrefix(countryCode)} {phone}";
    }
    
    private string GetCountryPrefix(string countryCode)
    {
        return countryCode switch
        {
            "ES" => "34",
            "FR" => "33",
            "DE" => "49",
            "UK" => "44",
            _ => "1"
        };
    }
}

// Clases de apoyo
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Category { get; set; }
    public bool HasDiscount { get; set; }
    public string DiscountReason { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsPremium { get; set; }
    public DateTime JoinDate { get; set; }
}
```

### Ejemplo 2: Procesamiento de Archivos Condicional

```csharp
public class FileProcessingService
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IDocumentProcessor _documentProcessor;
    private readonly ICompressionService _compressionService;
    
    public FileProcessingService(
        IImageProcessor imageProcessor,
        IDocumentProcessor documentProcessor,
        ICompressionService compressionService)
    {
        _imageProcessor = imageProcessor;
        _documentProcessor = documentProcessor;
        _compressionService = compressionService;
    }
    
    public async Task<MlResult<ProcessedFile>> ProcessFileAsync(UploadedFile file)
    {
        return await file.ToMlResult()
            .MapIfAsync(f => IsImageFile(f.Extension),
                       async f => await ProcessAsImageAsync(f),
                       async f => await ProcessAsDocumentAsync(f))
            .MapIfAsync(async processed => processed.SizeBytes > 1024 * 1024, // > 1MB
                       async large => await CompressFileAsync(large),
                       normal => Task.FromResult(normal))
            .MapIfAsync(processed => processed.IsPublic,
                       async pub => await GeneratePublicUrlAsync(pub),
                       priv => Task.FromResult(priv));
    }
    
    public MlResult<string> GenerateFileName(string originalName, FileProcessingOptions options)
    {
        return originalName.ToMlResult()
            .MapIf(name => options.AddTimestamp,
                   name => $"{Path.GetFileNameWithoutExtension(name)}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(name)}",
                   name => name)
            .MapIf(name => options.MakeUnique,
                   name => $"{Path.GetFileNameWithoutExtension(name)}_{Guid.NewGuid():N}{Path.GetExtension(name)}",
                   name => name)
            .MapIf(name => options.ConvertToLowerCase,
                   name => name.ToLowerInvariant(),
                   name => name);
    }
    
    public async Task<MlResult<FileValidationResult>> ValidateFileAsync(UploadedFile file, ValidationRules rules)
    {
        return await file.ToMlResult()
            .MapIfAsync(f => rules.CheckFileSize,
                       async f => await ValidateFileSizeAsync(f, rules.MaxSizeBytes),
                       f => Task.FromResult(new FileValidationResult { IsValid = true, File = f }))
            .MapIfAsync(async result => rules.CheckVirusScanning && result.IsValid,
                       async r => await ScanForVirusesAsync(r),
                       r => Task.FromResult(r))
            .MapIfAsync(async result => rules.CheckContentType && result.IsValid,
                       async r => await ValidateContentTypeAsync(r, rules.AllowedTypes),
                       r => Task.FromResult(r));
    }
    
    public MlResult<ThumbnailConfig> CreateThumbnailConfig(ImageFile image, ThumbnailOptions options)
    {
        return image.ToMlResult()
            .MapIf(img => img.Width > img.Height, // Landscape
                   img => new ThumbnailConfig
                   {
                       Width = options.MaxSize,
                       Height = (int)(img.Height * ((double)options.MaxSize / img.Width)),
                       Quality = options.Quality,
                       Format = DetermineOptimalFormat(img, true)
                   },
                   img => new ThumbnailConfig // Portrait or Square
                   {
                       Width = (int)(img.Width * ((double)options.MaxSize / img.Height)),
                       Height = options.MaxSize,
                       Quality = options.Quality,
                       Format = DetermineOptimalFormat(img, false)
                   })
            .MapIf(config => options.EnableWebPOptimization && SupportsWebP(),
                   config => config with { Format = "webp", Quality = Math.Min(config.Quality + 10, 100) },
                   config => config);
    }
    
    // Métodos auxiliares asíncronos
    private async Task<ProcessedFile> ProcessAsImageAsync(UploadedFile file)
    {
        var imageInfo = await _imageProcessor.GetImageInfoAsync(file.Stream);
        
        return new ProcessedFile
        {
            OriginalFile = file,
            ProcessedType = "image",
            SizeBytes = file.SizeBytes,
            Metadata = new Dictionary<string, object>
            {
                ["width"] = imageInfo.Width,
                ["height"] = imageInfo.Height,
                ["format"] = imageInfo.Format,
                ["hasAlpha"] = imageInfo.HasAlpha
            },
            ProcessedAt = DateTime.UtcNow
        };
    }
    
    private async Task<ProcessedFile> ProcessAsDocumentAsync(UploadedFile file)
    {
        var docInfo = await _documentProcessor.ExtractMetadataAsync(file.Stream);
        
        return new ProcessedFile
        {
            OriginalFile = file,
            ProcessedType = "document",
            SizeBytes = file.SizeBytes,
            Metadata = new Dictionary<string, object>
            {
                ["pageCount"] = docInfo.PageCount,
                ["author"] = docInfo.Author ?? "Unknown",
                ["createdDate"] = docInfo.CreatedDate,
                ["hasText"] = docInfo.ExtractedText?.Length > 0
            },
            ProcessedAt = DateTime.UtcNow
        };
    }
    
    private async Task<ProcessedFile> CompressFileAsync(ProcessedFile file)
    {
        await Task.Delay(100); // Simular compresión
        
        var compressedSize = (long)(file.SizeBytes * 0.7); // 30% reducción simulada
        
        return file with
        {
            SizeBytes = compressedSize,
            IsCompressed = true,
            CompressionRatio = (double)compressedSize / file.OriginalFile.SizeBytes
        };
    }
    
    private async Task<ProcessedFile> GeneratePublicUrlAsync(ProcessedFile file)
    {
        await Task.Delay(50); // Simular generación de URL
        
        return file with
        {
            PublicUrl = $"https://cdn.example.com/files/{Guid.NewGuid():N}/{file.OriginalFile.Name}",
            UrlGeneratedAt = DateTime.UtcNow
        };
    }
    
    private async Task<FileValidationResult> ValidateFileSizeAsync(UploadedFile file, long maxSize)
    {
        await Task.Delay(10);
        
        var isValid = file.SizeBytes <= maxSize;
        return new FileValidationResult
        {
            IsValid = isValid,
            File = file,
            ValidationErrors = isValid ? new List<string>() : new List<string> { $"File size {file.SizeBytes} exceeds maximum {maxSize}" }
        };
    }
    
    private async Task<FileValidationResult> ScanForVirusesAsync(FileValidationResult result)
    {
        await Task.Delay(200); // Simular escaneo
        
        // Simulación: archivos con "virus" en el nombre fallan
        var hasVirus = result.File.Name.ToLower().Contains("virus");
        
        if (hasVirus)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Virus detected in file");
        }
        
        return result;
    }
    
    private async Task<FileValidationResult> ValidateContentTypeAsync(FileValidationResult result, string[] allowedTypes)
    {
        await Task.Delay(30);
        
        var isAllowed = allowedTypes.Contains(result.File.ContentType);
        if (!isAllowed)
        {
            result.IsValid = false;
            result.ValidationErrors.Add($"Content type {result.File.ContentType} not allowed");
        }
        
        return result;
    }
    
    // Métodos auxiliares síncronos
    private bool IsImageFile(string extension)
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        return imageExtensions.Contains(extension.ToLower());
    }
    
    private string DetermineOptimalFormat(ImageFile image, bool isLandscape)
    {
        if (image.HasAlpha) return "png";
        if (isLandscape && image.Width > 1920) return "webp";
        return "jpg";
    }
    
    private bool SupportsWebP()
    {
        // Simulación de detección de soporte WebP
        return true;
    }
}

// Clases de apoyo e interfaces
public record UploadedFile
{
    public string Name { get; init; }
    public string Extension { get; init; }
    public string ContentType { get; init; }
    public long SizeBytes { get; init; }
    public Stream Stream { get; init; }
}

public record ProcessedFile
{
    public UploadedFile OriginalFile { get; init; }
    public string ProcessedType { get; init; }
    public long SizeBytes { get; init; }
    public bool IsCompressed { get; init; }
    public double? CompressionRatio { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
    public DateTime ProcessedAt { get; init; }
    public bool IsPublic { get; init; }
    public string PublicUrl { get; init; }
    public DateTime? UrlGeneratedAt { get; init; }
}

public class FileProcessingOptions
{
    public bool AddTimestamp { get; set; }
    public bool MakeUnique { get; set; }
    public bool ConvertToLowerCase { get; set; }
}

public class ValidationRules
{
    public bool CheckFileSize { get; set; }
    public long MaxSizeBytes { get; set; }
    public bool CheckVirusScanning { get; set; }
    public bool CheckContentType { get; set; }
    public string[] AllowedTypes { get; set; }
}

public class FileValidationResult
{
    public bool IsValid { get; set; }
    public UploadedFile File { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}

public record ThumbnailConfig
{
    public int Width { get; init; }
    public int Height { get; init; }
    public int Quality { get; init; }
    public string Format { get; init; }
}

public class ThumbnailOptions
{
    public int MaxSize { get; set; } = 200;
    public int Quality { get; set; } = 80;
    public bool EnableWebPOptimization { get; set; } = true;
}

public class ImageFile
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Format { get; set; }
    public bool HasAlpha { get; set; }
}

public interface IImageProcessor
{
    Task<ImageFile> GetImageInfoAsync(Stream stream);
}

public interface IDocumentProcessor
{
    Task<DocumentInfo> ExtractMetadataAsync(Stream stream);
}

public interface ICompressionService
{
    Task<byte[]> CompressAsync(byte[] data);
}

public class DocumentInfo
{
    public int PageCount { get; set; }
    public string Author { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ExtractedText { get; set; }
}
```

### Ejemplo 3: Lógica de Negocio Condicional

```csharp
public class OrderProcessingService
{
    public MlResult<OrderTotal> CalculateOrderTotal(Order order, Customer customer)
    {
        return order.ToMlResult()
            .MapIf(o => customer.IsPremium,
                   o => CalculatePremiumTotal(o, customer),
                   o => CalculateStandardTotal(o))
            .MapIf(total => total.Subtotal > 100,
                   total => ApplyFreeShipping(total),
                   total => total)
            .MapIf(total => customer.HasActivePromoCode,
                   total => ApplyPromoDiscount(total, customer.PromoCode),
                   total => total);
    }
    
    public MlResult<PaymentMethod> SelectPaymentMethod(PaymentRequest request, Customer customer)
    {
        return request.ToMlResult()
            .MapIf(req => customer.CreditScore >= 700 && req.Amount > 1000,
                   req => new PaymentMethod
                   {
                       Type = "CreditLine",
                       Provider = "InternalCredit",
                       InterestRate = 0.05m,
                       TermsMonths = DetermineTerms(req.Amount),
                       IsInstantApproval = true
                   },
                   req => new PaymentMethod
                   {
                       Type = "CreditCard",
                       Provider = req.PreferredProvider ?? "Default",
                       InterestRate = 0,
                       TermsMonths = 1,
                       IsInstantApproval = false
                   });
    }
    
    public async Task<MlResult<ShippingOption>> SelectShippingOptionAsync(Order order, Address shippingAddress)
    {
        return await order.ToMlResult()
            .MapIfAsync(async o => await IsInternationalShippingAsync(shippingAddress),
                       async o => await GetInternationalShippingAsync(o, shippingAddress),
                       o => Task.FromResult(GetDomesticShipping(o, shippingAddress)))
            .MapIfAsync(shipping => order.Items.Any(i => i.IsFragile),
                       s => s with { 
                           HandlingFee = s.HandlingFee + 15m,
                           Notes = s.Notes + " - Fragile item handling" 
                       },
                       s => s)
            .MapIfAsync(async shipping => order.TotalWeight > 50,
                       async s => await CalculateOversizeShippingAsync(s, order.TotalWeight),
                       s => Task.FromResult(s));
    }
    
    public MlResult<InventoryAction> DetermineInventoryAction(Product product, int requestedQuantity, InventoryLevel currentLevel)
    {
        return product.ToMlResult()
            .MapIf(p => currentLevel.AvailableQuantity >= requestedQuantity,
                   p => new InventoryAction
                   {
                       Action = "Reserve",
                       Quantity = requestedQuantity,
                       ProductId = p.Id,
                       IsImmediate = true,
                       EstimatedFulfillment = DateTime.UtcNow
                   },
                   p => currentLevel.ReorderLevel > 0 && currentLevel.AvailableQuantity + currentLevel.IncomingQuantity >= requestedQuantity
                       ? new InventoryAction
                       {
                           Action = "Backorder",
                           Quantity = requestedQuantity,
                           ProductId = p.Id,
                           IsImmediate = false,
                           EstimatedFulfillment = currentLevel.NextDeliveryDate
                       }
                       : new InventoryAction
                       {
                           Action = "Unavailable",
                           Quantity = 0,
                           ProductId = p.Id,
                           IsImmediate = false,
                           EstimatedFulfillment = null
                       });
    }
    
    public MlResult<NotificationStrategy> DetermineNotificationStrategy(Customer customer, OrderEvent orderEvent)
    {
        return customer.ToMlResult()
            .MapIf(c => c.NotificationPreferences.PrefersSMS && HasValidPhoneNumber(c),
                   c => new NotificationStrategy
                   {
                       PrimaryChannel = "SMS",
                       SecondaryChannel = c.NotificationPreferences.PrefersPush ? "Push" : "Email",
                       UrgencyLevel = DetermineUrgencyLevel(orderEvent),
                       ShouldBatch = false
                   },
                   c => new NotificationStrategy
                   {
                       PrimaryChannel = "Email",
                       SecondaryChannel = c.NotificationPreferences.PrefersPush ? "Push" : null,
                       UrgencyLevel = DetermineUrgencyLevel(orderEvent),
                       ShouldBatch = c.NotificationPreferences.AllowBatching
                   })
            .MapIf(strategy => orderEvent.IsUrgent,
                   strategy => strategy with { 
                       ShouldBatch = false,
                       UrgencyLevel = "High",
                       RetryAttempts = 3
                   },
                   strategy => strategy);
    }
    
    // Métodos auxiliares
    private OrderTotal CalculatePremiumTotal(Order order, Customer customer)
    {
        var subtotal = order.Items.Sum(i => i.Price * i.Quantity);
        var premiumDiscount = subtotal * 0.1m; // 10% descuento premium
        
        return new OrderTotal
        {
            Subtotal = subtotal,
            PremiumDiscount = premiumDiscount,
            Tax = (subtotal - premiumDiscount) * 0.08m,
            Shipping = 0, // Envío gratis para premium
            Total = (subtotal - premiumDiscount) * 1.08m
        };
    }
    
    private OrderTotal CalculateStandardTotal(Order order)
    {
        var subtotal = order.Items.Sum(i => i.Price * i.Quantity);
        var shipping = CalculateStandardShipping(subtotal);
        
        return new OrderTotal
        {
            Subtotal = subtotal,
            Tax = subtotal * 0.08m,
            Shipping = shipping,
            Total = subtotal * 1.08m + shipping
        };
    }
    
    private OrderTotal ApplyFreeShipping(OrderTotal total)
    {
        return total with { Shipping = 0, Total = total.Subtotal + total.Tax };
    }
    
    private OrderTotal ApplyPromoDiscount(OrderTotal total, string promoCode)
    {
        var discount = promoCode switch
        {
            "SAVE10" => total.Subtotal * 0.1m,
            "SAVE20" => total.Subtotal * 0.2m,
            "WELCOME15" => total.Subtotal * 0.15m,
            _ => 0m
        };
        
        return total with 
        { 
            PromoDiscount = discount,
            Total = total.Total - discount
        };
    }
    
    private decimal CalculateStandardShipping(decimal subtotal)
    {
        return subtotal switch
        {
            < 50 => 9.99m,
            < 100 => 7.99m,
            _ => 0m
        };
    }
    
    private int DetermineTerms(decimal amount)
    {
        return amount switch
        {
            < 1000 => 6,
            < 5000 => 12,
            < 10000 => 24,
            _ => 36
        };
    }
    
    private async Task<bool> IsInternationalShippingAsync(Address address)
    {
        await Task.Delay(10);
        return address.Country != "US";
    }
    
    private async Task<ShippingOption> GetInternationalShippingAsync(Order order, Address address)
    {
        await Task.Delay(50);
        
        return new ShippingOption
        {
            Method = "International",
            Cost = order.Items.Sum(i => i.Weight * i.Quantity) * 5m + 25m,
            EstimatedDays = 7 + GetCountryShippingDelay(address.Country),
            Tracking = true,
            Insurance = true,
            HandlingFee = 15m,
            Notes = $"International shipping to {address.Country}"
        };
    }
    
    private ShippingOption GetDomesticShipping(Order order, Address address)
    {
        var baseRate = order.Items.Sum(i => i.Weight * i.Quantity) * 1.5m;
        
        return new ShippingOption
        {
            Method = "Domestic",
            Cost = Math.Max(baseRate, 5.99m),
            EstimatedDays = GetDomesticShippingDays(address.ZipCode),
            Tracking = true,
            Insurance = false,
            HandlingFee = 0m,
            Notes = "Standard domestic shipping"
        };
    }
    
    private async Task<ShippingOption> CalculateOversizeShippingAsync(ShippingOption baseShipping, decimal weight)
    {
        await Task.Delay(30);
        
        var oversizeFee = (weight - 50) * 2m; // $2 por libra extra
        
        return baseShipping with
        {
            Cost = baseShipping.Cost + oversizeFee,
            EstimatedDays = baseShipping.EstimatedDays + 1,
            HandlingFee = baseShipping.HandlingFee + 10m,
            Notes = baseShipping.Notes + $" - Oversize package ({weight} lbs)"
        };
    }
    
    private bool HasValidPhoneNumber(Customer customer)
    {
        return !string.IsNullOrWhiteSpace(customer.PhoneNumber) &&
               customer.PhoneNumber.Length >= 10;
    }
    
    private string DetermineUrgencyLevel(OrderEvent orderEvent)
    {
        return orderEvent.Type switch
        {
            "OrderConfirmed" => "Low",
            "PaymentFailed" => "High",
            "Shipped" => "Medium",
            "Delivered" => "Low",
            _ => "Low"
        };
    }
    
    private int GetCountryShippingDelay(string country)
    {
        return country switch
        {
            "CA" => 1,   // Canada
            "MX" => 2,   // Mexico
            "GB" => 3,   // UK
            "EU" => 4,   // Europe
            _ => 7       // Others
        };
    }
    
    private int GetDomesticShippingDays(string zipCode)
    {
        // Simulación basada en código postal
        return zipCode?.Substring(0, 1) switch
        {
            "0" or "1" or "2" => 2, // Costa Este
            "9" => 1,               // Costa Oeste (cerca del centro de distribución)
            _ => 3                  // Centro/Otras áreas
        };
    }
}

// Clases de apoyo
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalWeight { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Weight { get; set; }
    public bool IsFragile { get; set; }
}

public record OrderTotal
{
    public decimal Subtotal { get; init; }
    public decimal PremiumDiscount { get; init; }
    public decimal PromoDiscount { get; init; }
    public decimal Tax { get; init; }
    public decimal Shipping { get; init; }
    public decimal Total { get; init; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsPremium { get; set; }
    public int CreditScore { get; set; }
    public bool HasActivePromoCode { get; set; }
    public string PromoCode { get; set; }
    public string PhoneNumber { get; set; }
    public NotificationPreferences NotificationPreferences { get; set; } = new();
}

public class NotificationPreferences
{
    public bool PrefersSMS { get; set; }
    public bool PrefersPush { get; set; }
    public bool AllowBatching { get; set; }
}

public record PaymentMethod
{
    public string Type { get; init; }
    public string Provider { get; init; }
    public decimal InterestRate { get; init; }
    public int TermsMonths { get; init; }
    public bool IsInstantApproval { get; init; }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string PreferredProvider { get; set; }
}

public class Address
{
    public string