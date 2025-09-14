# MlResultActionsExecSelfIfFailWithValue - Ejecución Condicional con Valores Preservados

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos ExecSelfIfFailWithValue](#métodos-execselfiffailwithvalue)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryExecSelfIfFailWithValue - Captura de Excepciones](#métodos-tryexecselfiffailwithvalue---captura-de-excepciones)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Otros Métodos](#comparación-con-otros-métodos)

---

## Introducción

Los métodos `ExecSelfIfFailWithValue` son operaciones especializadas que ejecutan acciones **únicamente cuando el `MlResult<T>` es fallido Y contiene valores preservados en los detalles del error**. Estos métodos extraen valores específicos de los detalles del error y los pasan a la acción, permitiendo un manejo contextual avanzado de errores.

### Propósito Principal

- **Recuperación de Contexto**: Acceder a valores originales que causaron el fallo
- **Logging Contextual**: Registrar información específica basada en los valores que fallaron
- **Compensación Inteligente**: Ejecutar acciones de compensación con conocimiento del contexto original
- **Debugging Avanzado**: Analizar valores específicos que llevaron al error
- **Auditoría Detallada**: Registrar información precisa sobre qué valores específicos causaron problemas

---

## Análisis de los Métodos

### Estructura y Filosofía

Los métodos `ExecSelfIfFailWithValue` implementan el patrón de **ejecución condicional con extracción de contexto**:

```
Resultado Exitoso → No acción → Resultado Exitoso (sin cambios)
      ↓                          ↓
Resultado Fallido → Extraer Valor → Ejecutar Acción(valor) → Resultado Fallido (sin cambios)
      ↓                          ↓
Resultado Fallido sin Valor → No acción → Resultado Fallido (sin cambios)
```

### Características Principales

1. **Extracción de Valores**: Utiliza `GetDetailValue<TValue>()` para extraer valores específicos de los detalles del error
2. **Ejecución Condicional Doble**: Solo ejecuta si es fallido AND contiene el valor requerido
3. **Inmutabilidad**: El resultado original nunca se modifica
4. **Tipado Fuerte**: Extracción de valores con tipos específicos
5. **Manejo de Errores en Acciones**: Las versiones `Try*` manejan excepciones y las agregan al error original

---

## Métodos ExecSelfIfFailWithValue

### `ExecSelfIfFailWithValue<T, TValue>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido y contiene un valor específico en los detalles, pasando ese valor a la acción

```csharp
public static MlResult<T> ExecSelfIfFailWithValue<T, TValue>(this MlResult<T> source,
                                                             Action<TValue> actionFailValue)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFailValue`: Acción a ejecutar solo si `source` es fallido y contiene un valor de tipo `TValue`

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFailValue`
- Si `source` es fallido pero no contiene valor de tipo `TValue`: Retorna `source` sin cambios, no ejecuta `actionFailValue`
- Si `source` es fallido y contiene valor de tipo `TValue`: Extrae el valor, ejecuta `actionFailValue(valor)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
// Supongamos que tenemos un resultado que preservó el UserInput original
var result = ValidateUser(userInput)
    .BindSaveValueInDetailsIfFaildFuncResult(user => ProcessUser(user))
    .ExecSelfIfFailWithValue<ProcessedUser, UserInput>(originalInput => 
        _logger.LogError($"Failed to process user with email: {originalInput.Email}"));

// Si ValidateUser o ProcessUser fallan, tendremos acceso al UserInput original
// La acción solo se ejecuta si hay un fallo Y el UserInput está en los detalles
```

**Ejemplo con Compensación Inteligente**:
```csharp
var result = ProcessPayment(paymentData)
    .BindSaveValueInDetailsIfFaildFuncResult(processed => FinalizeTransaction(processed))
    .ExecSelfIfFailWithValue<TransactionResult, PaymentData>(originalPayment => 
    {
        // Tenemos acceso a los datos de pago originales para compensación
        _compensationService.CreateRefundRequest(originalPayment.CustomerId, originalPayment.Amount);
        _auditService.LogFailedPayment(originalPayment.TransactionId, originalPayment.Amount);
    });
```

---

## Variantes Asíncronas

### `ExecSelfIfFailWithValueAsync<T, TValue>()` - Acción Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(
    this MlResult<T> source,
    Func<TValue, Task> actionFailValueAsync)
```

**Comportamiento**: Ejecuta una acción asíncrona solo si el resultado es fallido y contiene el valor especificado

**Ejemplo**:
```csharp
var result = await ProcessOrder(orderRequest)
    .BindSaveValueInDetailsIfFaildFuncResultAsync(async order => await SaveToDatabase(order))
    .ExecSelfIfFailWithValueAsync<SavedOrder, OrderRequest>(async originalRequest => 
    {
        await _notificationService.SendOrderFailureEmailAsync(
            originalRequest.CustomerId, 
            $"Order failed for {originalRequest.Items.Count} items totaling {originalRequest.TotalAmount:C}");
        
        await _metricsService.RecordFailedOrderAsync(originalRequest.CustomerId, originalRequest.TotalAmount);
    });
```

### `ExecSelfIfFailWithValueAsync<T, TValue>()` - Fuente Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(
    this Task<MlResult<T>> sourceAsync,
    Func<TValue, Task> actionFailValueAsync)

public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(
    this Task<MlResult<T>> sourceAsync,
    Action<TValue> actionFailValue)
```

**Ejemplo con Fuente Asíncrona**:
```csharp
var result = await ValidateDocumentAsync(documentData)
    .BindSaveValueInDetailsIfFaildFuncResultAsync(async doc => await ProcessDocumentAsync(doc))
    .ExecSelfIfFailWithValueAsync<ProcessedDocument, DocumentData>(originalDoc => 
        _logger.LogWarning($"Document processing failed for {originalDoc.FileName} ({originalDoc.SizeInBytes} bytes)"));
```

---

## Métodos TryExecSelfIfFailWithValue - Captura de Excepciones

### `TryExecSelfIfFailWithValue<T, TValue>()` - Versión Segura

```csharp
public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(this MlResult<T> source,
                                                                Action<TValue> actionFailValue,
                                                                Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(this MlResult<T> source,
                                                                Action<TValue> actionFailValue,
                                                                string errorMessage = null!)
```

**Comportamiento Especial**: 
- Si la acción lanza una excepción, **las versiones `Try*` agregan el error de la excepción a los errores originales**
- Utiliza `source.ErrorsDetails.Merge(errorDetails)` para combinar errores
- El resultado final contiene tanto los errores originales como cualquier error de la acción

**Ejemplo**:
```csharp
var result = ProcessTransaction(transactionData)
    .BindSaveValueInDetailsIfFaildFuncResult(trans => ValidateTransaction(trans))
    .TryExecSelfIfFailWithValue<ValidatedTransaction, TransactionData>(
        originalTransaction => 
        {
            // Esta acción puede fallar
            _externalAuditService.LogFailedTransaction(originalTransaction); // Puede lanzar excepción
            _riskManagement.AssessRisk(originalTransaction); // Puede lanzar excepción
        },
        ex => $"Failed to execute failure handling for transaction {originalTransaction?.Id}: {ex.Message}"
    );

// Si ProcessTransaction/ValidateTransaction fallan: result contiene esos errores + TransactionData en detalles
// Si además las acciones de fallo lanzan excepciones: result contiene errores originales + errores de las acciones
```

### Versiones Asíncronas de TryExecSelfIfFailWithValue

#### `TryExecSelfIfFailWithValueAsync<T, TValue>()` - Todas las Variantes

```csharp
// Acción asíncrona segura
public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(
    this MlResult<T> source,
    Func<TValue, Task> actionFailValueAsync,
    Func<Exception, string> errorMessageBuilder)

// Fuente asíncrona con acción asíncrona segura
public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(
    this Task<MlResult<T>> sourceAsync,
    Func<TValue, Task> actionFailValueAsync,
    Func<Exception, string> errorMessageBuilder)

// Fuente asíncrona con acción síncrona segura
public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(
    this Task<MlResult<T>> sourceAsync,
    Action<TValue> actionFailValue,
    Func<Exception, string> errorMessageBuilder)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Documentos con Recuperación de Contexto

```csharp
public class DocumentProcessingService
{
    private readonly IDocumentValidator _validator;
    private readonly IDocumentProcessor _processor;
    private readonly IDocumentStorage _storage;
    private readonly INotificationService _notifications;
    private readonly ILogger _logger;
    private readonly IAuditService _audit;
    private readonly IVirusScanner _virusScanner;
    
    public DocumentProcessingService(
        IDocumentValidator validator,
        IDocumentProcessor processor,
        IDocumentStorage storage,
        INotificationService notifications,
        ILogger logger,
        IAuditService audit,
        IVirusScanner virusScanner)
    {
        _validator = validator;
        _processor = processor;
        _storage = storage;
        _notifications = notifications;
        _logger = logger;
        _audit = audit;
        _virusScanner = virusScanner;
    }
    
    public async Task<MlResult<ProcessedDocument>> ProcessDocumentWithContextualRecoveryAsync(DocumentUpload upload)
    {
        var processingId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateDocumentUpload(upload)
            .ExecSelfIfFailWithValue<DocumentUpload, DocumentUpload>(originalUpload => 
            {
                _logger.LogWarning($"Document validation failed for {originalUpload.FileName} " +
                                 $"uploaded by user {originalUpload.UserId}");
                _audit.LogValidationFailure(processingId, originalUpload.UserId, originalUpload.FileName);
            })
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validUpload => 
                await ScanForVirusesAsync(validUpload))
            .TryExecSelfIfFailWithValueAsync<ScannedDocument, DocumentUpload>(
                async originalUpload => 
                {
                    await _notifications.SendVirusDetectionAlertAsync(originalUpload.UserId, originalUpload.FileName);
                    await _audit.LogSecurityIncidentAsync(processingId, originalUpload, "Virus detected");
                    await QuarantineFileAsync(originalUpload.FilePath);
                },
                ex => $"Failed to handle virus detection for document {upload.FileName}: {ex.Message}"
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async scannedDoc => 
                await ProcessDocumentContentAsync(scannedDoc))
            .ExecSelfIfFailWithValueAsync<ProcessedContent, DocumentUpload>(async originalUpload => 
            {
                await _logger.LogErrorAsync($"Document content processing failed for {originalUpload.FileName}");
                await _notifications.SendProcessingFailureNotificationAsync(
                    originalUpload.UserId, 
                    originalUpload.FileName,
                    "Content processing failed");
            })
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async processedContent => 
                await SaveProcessedDocumentAsync(processedContent))
            .TryExecSelfIfFailWithValueAsync<SavedDocument, DocumentUpload>(
                async originalUpload => 
                {
                    // Cleanup en caso de fallo de guardado
                    await CleanupTempFilesAsync(originalUpload.FilePath);
                    await _storage.RollbackPartialSaveAsync(processingId);
                    await _notifications.SendStorageFailureNotificationAsync(originalUpload.UserId, originalUpload.FileName);
                },
                ex => $"Failed to cleanup after storage failure for {upload.FileName}: {ex.Message}"
            )
            .ExecSelfIfFailWithValueAsync<SavedDocument, DocumentUpload>(async originalUpload => 
            {
                var duration = DateTime.UtcNow - startTime;
                await _audit.LogDocumentProcessingFailureAsync(
                    processingId, 
                    originalUpload.UserId, 
                    originalUpload.FileName,
                    originalUpload.SizeInBytes,
                    duration);
            });
    }
    
    private MlResult<DocumentUpload> ValidateDocumentUpload(DocumentUpload upload)
    {
        if (upload == null)
            return MlResult<DocumentUpload>.Fail("Document upload cannot be null");
            
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(upload.FileName))
            errors.Add("File name is required");
        else if (upload.FileName.Length > 255)
            errors.Add("File name is too long (max 255 characters)");
            
        if (upload.SizeInBytes <= 0)
            errors.Add("File size must be positive");
        else if (upload.SizeInBytes > 100 * 1024 * 1024) // 100 MB
            errors.Add("File size exceeds maximum allowed (100 MB)");
            
        if (upload.UserId <= 0)
            errors.Add("Valid user ID is required");
            
        if (string.IsNullOrWhiteSpace(upload.FilePath))
            errors.Add("File path is required");
        else if (!File.Exists(upload.FilePath))
            errors.Add("File does not exist at specified path");
            
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".png" };
        var extension = Path.GetExtension(upload.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            errors.Add($"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
            
        if (errors.Any())
            return MlResult<DocumentUpload>.Fail(errors.ToArray());
            
        return MlResult<DocumentUpload>.Valid(upload);
    }
    
    private async Task<MlResult<ScannedDocument>> ScanForVirusesAsync(DocumentUpload upload)
    {
        try
        {
            var scanResult = await _virusScanner.ScanFileAsync(upload.FilePath);
            
            if (scanResult.VirusDetected)
            {
                return MlResult<ScannedDocument>.Fail($"Virus detected: {scanResult.ThreatName}");
            }
            
            var scannedDoc = new ScannedDocument
            {
                Upload = upload,
                ScanResult = scanResult,
                ScannedAt = DateTime.UtcNow
            };
            
            return MlResult<ScannedDocument>.Valid(scannedDoc);
        }
        catch (Exception ex)
        {
            return MlResult<ScannedDocument>.Fail($"Virus scanning failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ProcessedContent>> ProcessDocumentContentAsync(ScannedDocument scannedDoc)
    {
        try
        {
            var content = await _processor.ExtractContentAsync(scannedDoc.Upload.FilePath);
            var metadata = await _processor.ExtractMetadataAsync(scannedDoc.Upload.FilePath);
            
            var processedContent = new ProcessedContent
            {
                ScannedDocument = scannedDoc,
                Content = content,
                Metadata = metadata,
                ProcessedAt = DateTime.UtcNow
            };
            
            return MlResult<ProcessedContent>.Valid(processedContent);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedContent>.Fail($"Content processing failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<SavedDocument>> SaveProcessedDocumentAsync(ProcessedContent processedContent)
    {
        try
        {
            var documentId = Guid.NewGuid();
            
            var document = new Document
            {
                Id = documentId,
                OriginalFileName = processedContent.ScannedDocument.Upload.FileName,
                UserId = processedContent.ScannedDocument.Upload.UserId,
                Content = processedContent.Content,
                Metadata = processedContent.Metadata,
                SizeInBytes = processedContent.ScannedDocument.Upload.SizeInBytes,
                UploadedAt = DateTime.UtcNow
            };
            
            await _storage.SaveDocumentAsync(document);
            
            var savedDoc = new SavedDocument
            {
                Document = document,
                ProcessedContent = processedContent,
                SavedAt = DateTime.UtcNow
            };
            
            return MlResult<SavedDocument>.Valid(savedDoc);
        }
        catch (Exception ex)
        {
            return MlResult<SavedDocument>.Fail($"Document storage failed: {ex.Message}");
        }
    }
    
    private async Task QuarantineFileAsync(string filePath)
    {
        try
        {
            var quarantinePath = Path.Combine(
                Path.GetTempPath(), 
                "quarantine", 
                $"{Guid.NewGuid()}_{Path.GetFileName(filePath)}");
                
            Directory.CreateDirectory(Path.GetDirectoryName(quarantinePath));
            File.Move(filePath, quarantinePath);
            
            await _logger.LogWarningAsync($"File quarantined: {filePath} -> {quarantinePath}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Failed to quarantine file {filePath}: {ex.Message}");
        }
    }
    
    private async Task CleanupTempFilesAsync(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                await _logger.LogDebugAsync($"Cleaned up temp file: {filePath}");
            }
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Failed to cleanup temp file {filePath}: {ex.Message}");
        }
    }
}

// Clases de apoyo
public class DocumentUpload
{
    public string FileName { get; set; }
    public long SizeInBytes { get; set; }
    public int UserId { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class ScannedDocument
{
    public DocumentUpload Upload { get; set; }
    public VirusScanResult ScanResult { get; set; }
    public DateTime ScannedAt { get; set; }
}

public class ProcessedContent
{
    public ScannedDocument ScannedDocument { get; set; }
    public string Content { get; set; }
    public DocumentMetadata Metadata { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class SavedDocument
{
    public Document Document { get; set; }
    public ProcessedContent ProcessedContent { get; set; }
    public DateTime SavedAt { get; set; }
}

public class ProcessedDocument
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class Document
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public DocumentMetadata Metadata { get; set; }
    public long SizeInBytes { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class VirusScanResult
{
    public bool VirusDetected { get; set; }
    public string ThreatName { get; set; }
    public DateTime ScannedAt { get; set; }
}

public class DocumentMetadata
{
    public string Author { get; set; }
    public DateTime CreatedDate { get; set; }
    public string DocumentType { get; set; }
    public int PageCount { get; set; }
}

// Interfaces de servicios
public interface IDocumentValidator
{
    Task<bool> ValidateAsync(DocumentUpload upload);
}

public interface IDocumentProcessor
{
    Task<string> ExtractContentAsync(string filePath);
    Task<DocumentMetadata> ExtractMetadataAsync(string filePath);
}

public interface IDocumentStorage
{
    Task SaveDocumentAsync(Document document);
    Task RollbackPartialSaveAsync(string processingId);
}

public interface INotificationService
{
    Task SendVirusDetectionAlertAsync(int userId, string fileName);
    Task SendProcessingFailureNotificationAsync(int userId, string fileName, string reason);
    Task SendStorageFailureNotificationAsync(int userId, string fileName);
}

public interface IAuditService
{
    Task LogValidationFailure(string processingId, int userId, string fileName);
    Task LogSecurityIncidentAsync(string processingId, DocumentUpload upload, string incidentType);
    Task LogDocumentProcessingFailureAsync(string processingId, int userId, string fileName, long sizeInBytes, TimeSpan duration);
}

public interface IVirusScanner
{
    Task<VirusScanResult> ScanFileAsync(string filePath);
}
```

### Ejemplo 2: Sistema de E-commerce con Compensación Inteligente

```csharp
public class EcommerceOrderService
{
    private readonly IOrderValidator _validator;
    private readonly IInventoryService _inventory;
    private readonly IPaymentService _payment;
    private readonly IShippingService _shipping;
    private readonly INotificationService _notifications;
    private readonly ICompensationService _compensation;
    private readonly ILogger _logger;
    private readonly IMetricsService _metrics;
    
    public EcommerceOrderService(
        IOrderValidator validator,
        IInventoryService inventory,
        IPaymentService payment,
        IShippingService shipping,
        INotificationService notifications,
        ICompensationService compensation,
        ILogger logger,
        IMetricsService metrics)
    {
        _validator = validator;
        _inventory = inventory;
        _payment = payment;
        _shipping = shipping;
        _notifications = notifications;
        _compensation = compensation;
        _logger = logger;
        _metrics = metrics;
    }
    
    public async Task<MlResult<CompletedOrder>> ProcessOrderWithIntelligentCompensationAsync(OrderRequest orderRequest)
    {
        var orderId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateOrderRequest(orderRequest)
            .ExecSelfIfFailWithValue<OrderRequest, OrderRequest>(originalRequest => 
            {
                _logger.LogWarning($"Order validation failed for customer {originalRequest.CustomerId}");
                _metrics.IncrementCounter("orders.validation_failures");
                // Analizar patrones de fallo por cliente
                AnalyzeCustomerValidationPatterns(originalRequest.CustomerId);
            })
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validOrder => 
                await ReserveInventoryAsync(validOrder))
            .TryExecSelfIfFailWithValueAsync<InventoryReservation, OrderRequest>(
                async originalRequest => 
                {
                    await _notifications.SendInventoryShortageNotificationAsync(originalRequest.CustomerId, originalRequest.Items);
                    await _metrics.RecordInventoryShortageAsync(originalRequest.Items);
                    
                    // Sugerir productos alternativos
                    var alternatives = await FindAlternativeProductsAsync(originalRequest.Items);
                    if (alternatives.Any())
                    {
                        await _notifications.SendAlternativeProductSuggestionsAsync(originalRequest.CustomerId, alternatives);
                    }
                },
                ex => $"Failed to handle inventory shortage for order {orderId}: {ex.Message}"
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async reservation => 
                await ProcessPaymentAsync(reservation))
            .TryExecSelfIfFailWithValueAsync<PaymentResult, OrderRequest>(
                async originalRequest => 
                {
                    // Compensación inteligente basada en el contexto original
                    await _compensation.HandlePaymentFailureAsync(new PaymentFailureContext
                    {
                        OrderId = orderId,
                        CustomerId = originalRequest.CustomerId,
                        Amount = originalRequest.TotalAmount,
                        PaymentMethod = originalRequest.PaymentMethodId,
                        Items = originalRequest.Items,
                        CustomerTier = await GetCustomerTierAsync(originalRequest.CustomerId)
                    });
                    
                    // Liberar inventario reservado
                    await ReleaseInventoryForOrderAsync(orderId);
                    
                    // Notificación personalizada basada en el cliente
                    await SendPersonalizedPaymentFailureNotificationAsync(originalRequest);
                },
                ex => $"Failed to handle payment failure compensation for order {orderId}: {ex.Message}"
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async payment => 
                await ArrangeShippingAsync(payment))
            .TryExecSelfIfFailWithValueAsync<ShippingArrangement, OrderRequest>(
                async originalRequest => 
                {
                    // Fallo en shipping - necesitamos hacer refund del pago
                    await _compensation.InitiatePaymentRefundAsync(new RefundContext
                    {
                        OrderId = orderId,
                        CustomerId = originalRequest.CustomerId,
                        Amount = originalRequest.TotalAmount,
                        Reason = "Shipping arrangement failed",
                        OriginalPaymentMethod = originalRequest.PaymentMethodId
                    });
                    
                    // Liberar inventario
                    await ReleaseInventoryForOrderAsync(orderId);
                    
                    // Notificar al cliente sobre el problema de envío
                    await _notifications.SendShippingFailureNotificationAsync(
                        originalRequest.CustomerId,
                        orderId,
                        originalRequest.ShippingAddress);
                },
                ex => $"Failed to handle shipping failure compensation for order {orderId}: {ex.Message}"
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async shipping => 
                await FinalizeOrderAsync(shipping))
            .ExecSelfIfFailWithValueAsync<CompletedOrder, OrderRequest>(async originalRequest => 
            {
                var duration = DateTime.UtcNow - startTime;
                
                // Log detallado del fallo con contexto original
                await _logger.LogErrorAsync($"Order finalization failed after {duration.TotalMinutes:F2} minutes. " +
                                          $"Customer: {originalRequest.CustomerId}, " +
                                          $"Items: {originalRequest.Items.Count}, " +
                                          $"Amount: {originalRequest.TotalAmount:C}");
                
                // Métricas específicas basadas en el contexto
                await _metrics.RecordOrderFailureAsync(new OrderFailureMetrics
                {
                    OrderId = orderId,
                    CustomerId = originalRequest.CustomerId,
                    CustomerTier = await GetCustomerTierAsync(originalRequest.CustomerId),
                    ItemCount = originalRequest.Items.Count,
                    TotalAmount = originalRequest.TotalAmount,
                    ProcessingDuration = duration,
                    FailureStage = "Finalization"
                });
                
                // Crear tarea de revisión manual para pedidos de alto valor
                if (originalRequest.TotalAmount > 500)
                {
                    await CreateHighValueOrderReviewTaskAsync(orderId, originalRequest);
                }
            });
    }
    
    private MlResult<OrderRequest> ValidateOrderRequest(OrderRequest request)
    {
        if (request == null)
            return MlResult<OrderRequest>.Fail("Order request cannot be null");
            
        var errors = new List<string>();
        
        if (request.CustomerId <= 0)
            errors.Add("Valid customer ID is required");
            
        if (request.Items?.Any() != true)
            errors.Add("Order must contain at least one item");
        else
        {
            foreach (var item in request.Items.Select((item, index) => new { item, index }))
            {
                if (string.IsNullOrWhiteSpace(item.item.ProductId))
                    errors.Add($"Product ID is required for item {item.index + 1}");
                    
                if (item.item.Quantity <= 0)
                    errors.Add($"Quantity must be positive for item {item.index + 1}");
                    
                if (item.item.Price <= 0)
                    errors.Add($"Price must be positive for item {item.index + 1}");
            }
        }
        
        if (request.TotalAmount <= 0)
            errors.Add("Total amount must be positive");
            
        if (string.IsNullOrWhiteSpace(request.PaymentMethodId))
            errors.Add("Payment method is required");
            
        if (request.ShippingAddress == null)
            errors.Add("Shipping address is required");
        else
        {
            if (string.IsNullOrWhiteSpace(request.ShippingAddress.Street))
                errors.Add("Street address is required");
            if (string.IsNullOrWhiteSpace(request.ShippingAddress.City))
                errors.Add("City is required");
            if (string.IsNullOrWhiteSpace(request.ShippingAddress.PostalCode))
                errors.Add("Postal code is required");
        }
        
        if (errors.Any())
            return MlResult<OrderRequest>.Fail(errors.ToArray());
            
        return MlResult<OrderRequest>.Valid(request);
    }
    
    private async Task<MlResult<InventoryReservation>> ReserveInventoryAsync(OrderRequest order)
    {
        try
        {
            var reservations = new List<ItemReservation>();
            
            foreach (var item in order.Items)
            {
                var reservation = await _inventory.ReserveAsync(item.ProductId, item.Quantity);
                if (!reservation.Success)
                {
                    // Liberar reservas anteriores
                    foreach (var prevReservation in reservations)
                    {
                        await _inventory.ReleaseReservationAsync(prevReservation.ReservationId);
                    }
                    
                    return MlResult<InventoryReservation>.Fail(
                        $"Insufficient inventory for product {item.ProductId}. " +
                        $"Requested: {item.Quantity}, Available: {reservation.AvailableQuantity}");
                }
                reservations.Add(reservation);
            }
            
            var inventoryReservation = new InventoryReservation
            {
                OrderRequest = order,
                Reservations = reservations,
                ReservedAt = DateTime.UtcNow
            };
            
            return MlResult<InventoryReservation>.Valid(inventoryReservation);
        }
        catch (Exception ex)
        {
            return MlResult<InventoryReservation>.Fail($"Inventory reservation failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<PaymentResult>> ProcessPaymentAsync(InventoryReservation reservation)
    {
        try
        {
            var paymentRequest = new PaymentRequest
            {
                CustomerId = reservation.OrderRequest.CustomerId,
                Amount = reservation.OrderRequest.TotalAmount,
                PaymentMethodId = reservation.OrderRequest.PaymentMethodId,
                OrderReference = reservation.OrderRequest.Items.Select(i => $"{i.ProductId}x{i.Quantity}").ToList()
            };
            
            var result = await _payment.ProcessPaymentAsync(paymentRequest);
            
            if (!result.Success)
            {
                return MlResult<PaymentResult>.Fail($"Payment failed: {result.ErrorMessage}");
            }
            
            var paymentResult = new PaymentResult
            {
                InventoryReservation = reservation,
                PaymentTransaction = result,
                ProcessedAt = DateTime.UtcNow
            };
            
            return MlResult<PaymentResult>.Valid(paymentResult);
        }
        catch (Exception ex)
        {
            return MlResult<PaymentResult>.Fail($"Payment processing error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ShippingArrangement>> ArrangeShippingAsync(PaymentResult payment)
    {
        try
        {
            var shippingRequest = new ShippingRequest
            {
                CustomerId = payment.InventoryReservation.OrderRequest.CustomerId,
                Items = payment.InventoryReservation.OrderRequest.Items,
                Address = payment.InventoryReservation.OrderRequest.ShippingAddress,
                PaymentReference = payment.PaymentTransaction.TransactionId
            };
            
            var arrangement = await _shipping.ArrangeShippingAsync(shippingRequest);
            
            if (!arrangement.Success)
            {
                return MlResult<ShippingArrangement>.Fail($"Shipping arrangement failed: {arrangement.ErrorMessage}");
            }
            
            var shippingArrangement = new ShippingArrangement
            {
                PaymentResult = payment,
                ShippingDetails = arrangement,
                ArrangedAt = DateTime.UtcNow
            };
            
            return MlResult<ShippingArrangement>.Valid(shippingArrangement);
        }
        catch (Exception ex)
        {
            return MlResult<ShippingArrangement>.Fail($"Shipping arrangement error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<CompletedOrder>> FinalizeOrderAsync(ShippingArrangement shipping)
    {
        try
        {
            var order = new CompletedOrder
            {
                Id = Guid.NewGuid(),
                CustomerId = shipping.PaymentResult.InventoryReservation.OrderRequest.CustomerId,
                Items = shipping.PaymentResult.InventoryReservation.OrderRequest.Items,
                TotalAmount = shipping.PaymentResult.InventoryReservation.OrderRequest.TotalAmount,
                PaymentTransactionId = shipping.PaymentResult.PaymentTransaction.TransactionId,
                ShippingTrackingNumber = shipping.ShippingDetails.TrackingNumber,
                Status = "Completed",
                CompletedAt = DateTime.UtcNow
            };
            
            // Guardar orden en base de datos
            await SaveOrderAsync(order);
            
            return MlResult<CompletedOrder>.Valid(order);
        }
        catch (Exception ex)
        {
            return MlResult<CompletedOrder>.Fail($"Order finalization failed: {ex.Message}");
        }
    }
    
    // Métodos auxiliares de compensación y análisis
    private void AnalyzeCustomerValidationPatterns(int customerId)
    {
        // Implementación de análisis de patrones de validación por cliente
        Task.Run(async () => 
        {
            try
            {
                var recentFailures = await GetRecentValidationFailuresAsync(customerId);
                if (recentFailures > 3)
                {
                    await _notifications.SendCustomerSupportAlertAsync(customerId, 
                        "Customer experiencing repeated validation failures");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to analyze customer validation patterns: {ex.Message}");
            }
        });
    }
    
    private async Task<List<AlternativeProduct>> FindAlternativeProductsAsync(List<OrderItem> unavailableItems)
    {
        var alternatives = new List<AlternativeProduct>();
        
        foreach (var item in unavailableItems)
        {
            try
            {
                var itemAlternatives = await _inventory.FindAlternativesAsync(item.ProductId);
                alternatives.AddRange(itemAlternatives);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to find alternatives for product {item.ProductId}: {ex.Message}");
            }
        }
        
        return alternatives;
    }
    
    private async Task SendPersonalizedPaymentFailureNotificationAsync(OrderRequest originalRequest)
    {
        try
        {
            var customerTier = await GetCustomerTierAsync(originalRequest.CustomerId);
            var paymentHistory = await GetCustomerPaymentHistoryAsync(originalRequest.CustomerId);
            
            var notification = new PersonalizedNotification
            {
                CustomerId = originalRequest.CustomerId,
                CustomerTier = customerTier,
                Message = GeneratePersonalizedPaymentFailureMessage(originalRequest, paymentHistory),
                SuggestedActions = GeneratePaymentFailureSuggestions(originalRequest, paymentHistory),
                Priority = customerTier == "Premium" ? NotificationPriority.High : NotificationPriority.Normal
            };
            
            await _notifications.SendPersonalizedNotificationAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to send personalized payment failure notification: {ex.Message}");
        }
    }
    
    private async Task CreateHighValueOrderReviewTaskAsync(string orderId, OrderRequest originalRequest)
    {
        try
        {
            var reviewTask = new OrderReviewTask
            {
                OrderId = orderId,
                CustomerId = originalRequest.CustomerId,
                Amount = originalRequest.TotalAmount,
                ItemCount = originalRequest.Items.Count,
                FailureReason = "High-value order finalization failed",
                Priority = ReviewPriority.High,
                CreatedAt = DateTime.UtcNow,
                AssignedTo = "order-review-team"
            };
            
            await _compensation.CreateReviewTaskAsync(reviewTask);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create high-value order review task: {ex.Message}");
        }
    }
    
    // Métodos auxiliares simplificados
    private async Task<string> GetCustomerTierAsync(int customerId) => "Standard"; // Implementación simplificada
    private async Task ReleaseInventoryForOrderAsync(string orderId) { } // Implementación simplificada
    private async Task SaveOrderAsync(CompletedOrder order) { } // Implementación simplificada
    private async Task<int> GetRecentValidationFailuresAsync(int customerId) => 0; // Implementación simplificada
    private async Task<PaymentHistory> GetCustomerPaymentHistoryAsync(int customerId) => new PaymentHistory(); // Implementación simplificada
    private string GeneratePersonalizedPaymentFailureMessage(OrderRequest request, PaymentHistory history) => "Payment failed"; // Implementación simplificada
    private List<string> GeneratePaymentFailureSuggestions(OrderRequest request, PaymentHistory history) => new(); // Implementación simplificada
}

// Clases de apoyo adicionales
public class OrderRequest
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethodId { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class ShippingAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}

public class InventoryReservation
{
    public OrderRequest OrderRequest { get; set; }
    public List<ItemReservation> Reservations { get; set; }
    public DateTime ReservedAt { get; set; }
}

public class PaymentResult
{
    public InventoryReservation InventoryReservation { get; set; }
    public PaymentTransaction PaymentTransaction { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ShippingArrangement
{
    public PaymentResult PaymentResult { get; set; }
    public ShippingDetails ShippingDetails { get; set; }
    public DateTime ArrangedAt { get; set; }
}

public class CompletedOrder
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentTransactionId { get; set; }
    public string ShippingTrackingNumber { get; set; }
    public string Status { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class ItemReservation
{
    public string ReservationId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public bool Success { get; set; }
}

public class PaymentRequest
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethodId { get; set; }
    public List<string> OrderReference { get; set; }
}

public class PaymentTransaction
{
    public string TransactionId { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

public class ShippingRequest
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public ShippingAddress Address { get; set; }
    public string PaymentReference { get; set; }
}

public class ShippingDetails
{
    public string TrackingNumber { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

public class PaymentFailureContext
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public List<OrderItem> Items { get; set; }
    public string CustomerTier { get; set; }
}

public class RefundContext
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public string OriginalPaymentMethod { get; set; }
}

public class OrderFailureMetrics
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerTier { get; set; }
    public int ItemCount { get; set; }
    public decimal TotalAmount { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
    public string FailureStage { get; set; }
}

public class AlternativeProduct
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
}

public class PersonalizedNotification
{
    public int CustomerId { get; set; }
    public string CustomerTier { get; set; }
    public string Message { get; set; }
    public List<string> SuggestedActions { get; set; }
    public NotificationPriority Priority { get; set; }
}

public class OrderReviewTask
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public int ItemCount { get; set; }
    public string FailureReason { get; set; }
    public ReviewPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AssignedTo { get; set; }
}

public class PaymentHistory
{
    public List<string> RecentFailures { get; set; } = new();
    public int SuccessfulPayments { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical
}

public enum ReviewPriority
{
    Low,
    Medium,
    High,
    Critical
}

// Interfaces de servicios específicos
public interface IOrderValidator
{
    Task<bool> ValidateAsync(OrderRequest order);
}

public interface IInventoryService
{
    Task<ItemReservation> ReserveAsync(string productId, int quantity);
    Task ReleaseReservationAsync(string reservationId);
    Task<List<AlternativeProduct>> FindAlternativesAsync(string productId);
}

public interface IPaymentService
{
    Task<PaymentTransaction> ProcessPaymentAsync(PaymentRequest request);
}

public interface IShippingService
{
    Task<ShippingDetails> ArrangeShippingAsync(ShippingRequest request);
}

public interface ICompensationService
{
    Task HandlePaymentFailureAsync(PaymentFailureContext context);
    Task InitiatePaymentRefundAsync(RefundContext context);
    Task CreateReviewTaskAsync(OrderReviewTask task);
}

public interface INotificationService
{
    Task SendInventoryShortageNotificationAsync(int customerId, List<OrderItem> items);
    Task SendAlternativeProductSuggestionsAsync(int customerId, List<AlternativeProduct> alternatives);
    Task SendShippingFailureNotificationAsync(int customerId, string orderId, ShippingAddress address);
    Task SendPersonalizedNotificationAsync(PersonalizedNotification notification);
    Task SendCustomerSupportAlertAsync(int customerId, string message);
}

public interface IMetricsService
{
    Task IncrementCounter(string counterName);
    Task RecordInventoryShortageAsync(List<OrderItem> items);
    Task RecordOrderFailureAsync(OrderFailureMetrics metrics);
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar ExecSelfIfFailWithValue

```csharp
// ✅ Correcto: Cuando necesitas acceso al contexto original que causó el fallo
var result = ProcessUser(userData)
    .BindSaveValueInDetailsIfFaildFuncResult(user => ValidateUser(user))
    .ExecSelfIfFailWithValue<ValidatedUser, UserData>(originalData => 
        _logger.LogError($"User processing failed for {originalData.Email}"));

// ✅ Correcto: Para compensación inteligente basada en datos originales
var result = ProcessPayment(paymentData)
    .BindSaveValueInDetailsIfFaildFuncResult(payment => AuthorizePayment(payment))
    .ExecSelfIfFailWithValue<AuthorizedPayment, PaymentData>(originalPayment => 
        _compensationService.HandleFailure(originalPayment.CustomerId, originalPayment.Amount));

// ❌ Incorrecto: Cuando no necesitas el contexto específico
var result = GetUser(userId)
    .ExecSelfIfFailWithValue<User, int>(originalUserId => 
        _logger.LogError($"User {originalUserId} not found")); // Mejor usar ExecSelfIfFail
```

### 2. Combinando con Otros Métodos de Preservación de Contexto

```csharp
// ✅ Correcto: Pipeline completo con preservación y uso de contexto
var result = ValidateInput(inputData)
    .BindSaveValueInDetailsIfFaildFuncResult(input => ProcessStep1(input))
    .ExecSelfIfFailWithValue<Step1Result, InputData>(original => 
        _logger.LogWarning($"Step 1 failed for input: {original.Id}"))
    .BindSaveValueInDetailsIfFaildFuncResult(step1 => ProcessStep2(step1))
    .ExecSelfIfFailWithValue<Step2Result, InputData>(original => 
        _compensationService.CompensateStep1(original))
    .BindSaveValueInDetailsIfFaildFuncResult(step2 => ProcessStep3(step2))
    .ExecSelfIfFailWithValue<Step3Result, InputData>(original => 
        _compensationService.CompensateStep1And2(original));

// En cada paso de fallo, tenemos acceso al InputData original
```

### 3. Manejo de Excepciones en Acciones con Valores

```csharp
// ✅ Correcto: Usar TryExecSelfIfFailWithValue para acciones que pueden fallar
var result = ProcessOrder(orderData)
    .BindSaveValueInDetailsIfFaildFuncResult(order => ValidateOrder(order))
    .TryExecSelfIfFailWithValue<ValidatedOrder, OrderData>(
        originalOrder => 
        {
            // Estas acciones pueden lanzar excepciones
            _externalService.NotifyFailure(originalOrder);
            _auditService.LogOrderFailure(originalOrder.CustomerId, originalOrder.TotalAmount);
        },
        ex => $"Failed to handle order failure for customer {originalOrder?.CustomerId}: {ex.Message}"
    );

// ❌ Incorrecto: No manejar excepciones en acciones críticas
var result = ProcessPayment(payment)
    .ExecSelfIfFailWithValue<PaymentResult, PaymentData>(original => 
        CriticalCompensation(original)); // Si falla, se pierde la excepción
```

### 4. Tipado y Extracción de Valores

```csharp
// ✅ Correcto: Usar tipos específicos para mejor type safety
public class UserRegistrationData { /* ... */ }
public class ProcessedUser { /* ... */ }

var result = RegisterUser(registrationData)
    .BindSaveValueInDetailsIfFaildFuncResult(data => CreateUser(data))
    .ExecSelfIfFailWithValue<ProcessedUser, UserRegistrationData>(originalData => 
    {
        // Acceso type-safe al objeto original
        _notificationService.SendRegistrationFailure(originalData.Email);
        _analytics.RecordFailedRegistration(originalData.Source, originalData.ReferralCode);
    });

// ✅ Correcto: Manejar múltiples tipos de valores preservados
var result = ProcessDocument(documentData)
    .BindSaveValueInDetailsIfFaildFuncResult(doc => ValidateDocument(doc))
    .ExecSelfIfFailWithValue<ValidatedDocument, DocumentData>(original => 
        HandleDocumentValidationFailure(original))
    .BindSaveValueInDetailsIfFaildFuncResult(valid => ProcessContent(valid))
    .ExecSelfIfFailWithValue<ProcessedContent, DocumentData>(original => 
        HandleContentProcessingFailure(original));
```

### 5. Logging Contextual Avanzado

```csharp
// ✅ Correcto: Logging estructurado con contexto completo
var result = ProcessComplexWorkflow(workflowData)
    .BindSaveValueInDetailsIfFaildFuncResult(data => ExecuteWorkflowStep1(data))
    .ExecSelfIfFailWithValue<Step1Result, WorkflowData>(originalData => 
    {
        _logger.LogError("Workflow step 1 failed", new 
        {
            WorkflowId = originalData.Id,
            WorkflowType = originalData.Type,
            CustomerId = originalData.CustomerId,
            StartTime = originalData.StartedAt,
            InputDataSize = originalData.SerializedSize,
            Step = "Step1"
        });
    })
    .BindSaveValueInDetailsIfFaildFuncResult(step1 => ExecuteWorkflowStep2(step1))
    .ExecSelfIfFailWithValue<Step2Result, WorkflowData>(originalData => 
    {
        _logger.LogError("Workflow step 2 failed", new 
        {
            WorkflowId = originalData.Id,
            WorkflowType = originalData.Type,
            CustomerId = originalData.CustomerId,
            StartTime = originalData.StartedAt,
            Step = "Step2",
            Note = "Step 1 was successful"
        });
    });
```

### 6. Análisis de Patrones de Fallo

```csharp
// ✅ Correcto: Análisis de patrones basado en datos originales
public async Task<MlResult<ProcessedData>> ProcessWithPatternAnalysisAsync(InputData inputData)
{
    return await ValidateInput(inputData)
        .BindSaveValueInDetailsIfFaildFuncResultAsync(async input => await ProcessData(input))
        .ExecSelfIfFailWithValueAsync<ProcessedData, InputData>(async originalInput => 
        {
            // Análisis de patrones de fallo basado en el input original
            await AnalyzeFailurePattern(new FailureAnalysisContext
            {
                InputType = originalInput.GetType().Name,
                InputSize = originalInput.EstimatedSize,
                CustomerId = originalInput.CustomerId,
                FailureTime = DateTime.UtcNow,
                InputCharacteristics = ExtractCharacteristics(originalInput)
            });
            
            // Actualizar estadísticas de fallo por tipo de input
            await UpdateFailureStatistics(originalInput.GetType().Name);
            
            // Detectar anomalías basadas en el patrón de input
            if (await IsAnomalousFailure(originalInput))
            {
                await _alertService.SendAnomalyAlert(originalInput);
            }
        });
}

private async Task AnalyzeFailurePattern(FailureAnalysisContext context)
{
    try
    {
        var pattern = new FailurePattern
        {
            InputType = context.InputType,
            InputSize = context.InputSize,
            CustomerId = context.CustomerId,
            FailureTime = context.FailureTime,
            Characteristics = context.InputCharacteristics
        };
        
        await _analyticsService.RecordFailurePatternAsync(pattern);
        
        // Determinar si este patrón indica un problema sistemático
        var recentSimilarFailures = await GetRecentSimilarFailuresAsync(pattern);
        if (recentSimilarFailures > 5)
        {
            await _alertService.SendSystemicIssueAlert(pattern);
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning($"Failed to analyze failure pattern: {ex.Message}");
    }
}
```

---

## Comparación con Otros Métodos

### Tabla Comparativa de Métodos ExecSelf

| Método | Cuándo Ejecuta | Recibe | Uso Principal |
|--------|----------------|--------|---------------|
| `ExecSelf` | Siempre | `T` o `MlErrorsDetails` | Logging/métricas diferenciadas por resultado |
| `ExecSelfIfValid` | Solo si válido | `T` | Efectos secundarios de éxito |
| `ExecSelfIfFail` | Solo si fallido | `MlErrorsDetails` | Manejo de errores genérico |
| `ExecSelfIfFailWithValue` | Solo si fallido Y contiene valor | `TValue` extraído | Manejo de errores con contexto específico |

### Matriz de Decisión

```csharp
// Uso según necesidades:

// ¿Necesitas ejecutar algo siempre?
result.ExecSelf(onSuccess, onFailure);

// ¿Solo en caso de éxito?
result.ExecSelfIfValid(onSuccess);

// ¿Solo en caso de fallo, sin contexto específico?
result.ExecSelfIfFail(onFailure);

// ¿Solo en caso de fallo, CON acceso a valores originales específicos?
result.ExecSelfIfFailWithValue<ResultType, OriginalType>(onFailureWithOriginal);
```

---

## Consideraciones de Rendimiento

### Extracción de Valores

- `ExecSelfIfFailWithValue` tiene overhead adicional por la extracción de valores de los detalles
- Solo ejecuta si hay fallo Y si el valor específico existe
- La extracción usa `GetDetailValue<TValue>()` que puede ser costosa para tipos complejos

### Preservación de Memoria

- Los valores preservados en detalles permanecen en memoria hasta que el resultado se libere
- Considerar el tamaño de los objetos preservados
- Para objetos grandes, considerar preservar solo propiedades esenciales

### Cadenas de Compensación

- Las acciones de compensación pueden ser complejas y costosas
- Considerar ejecutar compensaciones asíncronamente cuando sea posible
- Implementar timeouts para evitar que las compensaciones fallen el sistema

---

## Resumen

Los métodos `ExecSelfIfFailWithValue` proporcionan capacidades avanzadas de manejo de errores con acceso a contexto específico:

**Características principales**:
- **Ejecución condicional doble**: Solo en fallo Y con valor específico disponible
- **Extracción de contexto**: Acceso a valores originales que causaron el fallo
- **Tipado fuerte**: Type-safe access a valores específicos
- **Compensación inteligente**: Acciones de compensación con conocimiento del contexto
- **Manejo de excepciones**: Versiones `Try*` que agregan errores de acciones al resultado original

**Casos de uso ideales**:
- **Compensación basada en contexto**: Revertir acciones basándose en datos originales
- **Logging contextual**: Registrar información específica sobre qué causó el fallo
- **Análisis de patrones**: Estudiar características de inputs que llevan a fallos
- **Notificaciones personalizadas**: Enviar alertas específicas basadas en el contexto
- **Debugging avanzado**: Acceso a valores exactos que causaron problemas

**Ventajas sobre otros métodos**:
- **Precisión**: Acceso exacto a los valores que causaron el fallo
- **Flexibilidad**: Diferentes acciones basadas en diferentes tipos de valores preservados
- **Mantenimiento**: Separación clara entre manejo de errores genérico y específico
- **Debugging**: Información contextual rica para diagnóstico

La clave está en usar `ExecSelfIfFailWithValue` cuando necesites **acceso específico a los valores originales** que causaron el fallo, especialmente para implementar **compensaciones