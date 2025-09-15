# MlResult Match - Pattern Matching Funcional

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos Match Básicos](#métodos-match-básicos)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryMatch - Captura de Excepciones](#métodos-trymatch---captura-de-excepciones)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Bind y Map](#comparación-con-bind-y-map)

---

## Introducción

Los métodos `Match` implementan el patrón de **pattern matching funcional** sobre `MlResult<T>`, permitiendo transformar el resultado en un tipo completamente diferente (`TReturn`) con lógica específica para casos de éxito y fallo.

### Propósito Principal

- **Transformación Condicional**: Convertir `MlResult<T>` a cualquier tipo `TReturn`
- **Manejo Bifurcado**: Lógica diferente para éxito vs fallo
- **Finalización de Cadenas**: Terminar pipelines de `MlResult` extrayendo valores
- **Mapeo a Tipos de Respuesta**: Convertir a DTOs, responses HTTP, etc.

---

## Análisis de los Métodos

### Filosofía del Pattern Matching

```
MlResult<T> → Match(validFunc, failFunc) → TReturn
   ↓                     ↓                    ↓
Éxito → validFunc(value) → TReturn
Fallo → failFunc(errors) → TReturn
```

### Características Principales

1. **Transformación Total**: Siempre produce un `TReturn`, nunca `MlResult`
2. **Bifurcación Funcional**: Dos funciones para dos caminos
3. **Finalización**: Extrae valores del contexto `MlResult`
4. **Soporte Asíncrono Completo**: Todas las combinaciones async/sync
5. **Versiones Seguras**: TryMatch captura excepciones

---

## Métodos Match Básicos

### `Match<T, TReturn>()`

**Propósito**: Transformar un `MlResult<T>` en un `TReturn` con lógica específica para éxito y fallo

```csharp
public static TReturn Match<T, TReturn>(this MlResult<T> source,
                                        Func<T, TReturn> valid,
                                        Func<MlErrorsDetails, TReturn> fail)
```

**Comportamiento**:
- Si `source` es válido: ejecuta `valid(value)` y retorna el resultado
- Si `source` es fallido: ejecuta `fail(errors)` y retorna el resultado

**Ejemplo Básico**:
```csharp
var user = GetUser(userId);
var response = user.Match(
    valid: u => new ApiResponse<User> { Success = true, Data = u },
    fail: errors => new ApiResponse<User> { Success = false, Error = errors.FirstErrorMessage }
);
```

**Ejemplo con Diferentes Tipos de Retorno**:
```csharp
var result = ProcessPayment(paymentData);
var statusCode = result.Match(
    valid: payment => 200,
    fail: errors => errors.AllErrors.Any(e => e.Contains("fraud")) ? 403 : 400
);
```

---

## Variantes Asíncronas

### `MatchAsync<T, TReturn>()` - Ambas Funciones Asíncronas

```csharp
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this MlResult<T> source,
    Func<T, Task<TReturn>> validAsync,
    Func<MlErrorsDetails, Task<TReturn>> failAsync)
```

**Ejemplo**:
```csharp
var result = await GetOrderAsync(orderId);
var notification = await result.MatchAsync(
    validAsync: async order => await _emailService.SendOrderConfirmationAsync(order),
    failAsync: async errors => await _emailService.SendErrorNotificationAsync(errors)
);
```

### `MatchAsync<T, TReturn>()` - Solo Función de Fallo Asíncrona

```csharp
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> valid,
    Func<MlErrorsDetails, Task<TReturn>> failAsync)
```

### `MatchAsync<T, TReturn>()` - Solo Función de Éxito Asíncrona

```csharp
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this MlResult<T> source,
    Func<T, Task<TReturn>> validAsync,
    Func<MlErrorsDetails, TReturn> fail)
```

### Variantes con Fuente Asíncrona

```csharp
// Task<MlResult<T>> con funciones asíncronas
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task<TReturn>> validAsync,
    Func<MlErrorsDetails, Task<TReturn>> failAsync)

// Task<MlResult<T>> con funciones síncronas
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, TReturn> valid,
    Func<MlErrorsDetails, TReturn> fail)
```

---

## Métodos TryMatch - Captura de Excepciones

### `TryMatch<T, TResult>()` - Versión Segura

```csharp
public static MlResult<TResult> TryMatch<T, TResult>(
    this MlResult<T> source, 
    Func<T, TResult> valid,
    Func<MlErrorsDetails, TResult> fail,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: 
- Ejecuta Match normalmente
- Si cualquiera de las funciones lanza excepción, retorna `MlResult<TResult>` fallido
- Convierte excepciones en errores manejables

**Ejemplo**:
```csharp
var result = ProcessData(data);
var safeResponse = result.TryMatch(
    valid: d => TransformToDto(d), // Puede lanzar excepción
    fail: errors => new ErrorDto { Message = errors.FirstErrorMessage },
    ex => $"Transform failed: {ex.Message}"
);
```

### Versiones Asíncronas de TryMatch

```csharp
// Funciones asíncronas
public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(
    this MlResult<T> source, 
    Func<T, Task<TResult>> validAsync,
    Func<MlErrorsDetails, Task<TResult>> failAsync,
    Func<Exception, string> errorMessageBuilder)

// Con Task<MlResult<T>>
public static async Task<MlResult<TResult>> TryMatchAsync<T, TResult>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<TResult>> validAsync,
    Func<MlErrorsDetails, Task<TResult>> failAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Ejemplos Prácticos

### Ejemplo 1: API Response Builder

```csharp
public class ApiResponseBuilder
{
    public static ApiResponse<T> BuildResponse<T>(MlResult<T> result)
    {
        return result.Match(
            valid: data => new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Timestamp = DateTime.UtcNow
            },
            fail: errors => new ApiResponse<T>
            {
                Success = false,
                Errors = errors.AllErrors,
                ErrorCode = DetermineErrorCode(errors),
                Timestamp = DateTime.UtcNow
            }
        );
    }
    
    public static async Task<IActionResult> BuildActionResultAsync<T>(
        Task<MlResult<T>> resultAsync)
    {
        return await resultAsync.MatchAsync(
            validAsync: async data => 
            {
                await LogSuccessAsync(data);
                return new OkObjectResult(data);
            },
            failAsync: async errors => 
            {
                await LogErrorsAsync(errors);
                var statusCode = DetermineHttpStatusCode(errors);
                return new ObjectResult(errors.AllErrors) { StatusCode = statusCode };
            }
        );
    }
    
    private static string DetermineErrorCode(MlErrorsDetails errors)
    {
        if (errors.AllErrors.Any(e => e.Contains("validation")))
            return "VALIDATION_ERROR";
        if (errors.AllErrors.Any(e => e.Contains("not found")))
            return "NOT_FOUND";
        return "GENERAL_ERROR";
    }
    
    private static int DetermineHttpStatusCode(MlErrorsDetails errors)
    {
        if (errors.AllErrors.Any(e => e.Contains("not found")))
            return 404;
        if (errors.AllErrors.Any(e => e.Contains("validation")))
            return 400;
        if (errors.AllErrors.Any(e => e.Contains("unauthorized")))
            return 401;
        return 500;
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string[] Errors { get; set; }
    public string ErrorCode { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### Ejemplo 2: Sistema de Notificaciones Condicionales

```csharp
public class NotificationService
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger _logger;
    
    public async Task<NotificationResult> ProcessOrderWithNotificationsAsync(
        OrderRequest request)
    {
        return await ValidateOrder(request)
            .BindAsync(async validOrder => await ProcessPaymentAsync(validOrder))
            .BindAsync(async paidOrder => await CreateOrderAsync(paidOrder))
            .TryMatchAsync(
                validAsync: async order => await HandleSuccessfulOrderAsync(order),
                failAsync: async errors => await HandleFailedOrderAsync(request, errors),
                ex => $"Notification processing failed: {ex.Message}"
            );
    }
    
    private async Task<NotificationResult> HandleSuccessfulOrderAsync(Order order)
    {
        var notifications = new List<string>();
        
        // Email de confirmación
        var emailResult = await _emailService.SendOrderConfirmationAsync(
            order.CustomerId, order);
        if (emailResult)
            notifications.Add($"Email sent to customer {order.CustomerId}");
        
        // SMS si es pedido urgente
        if (order.IsUrgent)
        {
            var smsResult = await _smsService.SendUrgentOrderSmsAsync(
                order.CustomerPhone, order.Id);
            if (smsResult)
                notifications.Add($"Urgent SMS sent to {order.CustomerPhone}");
        }
        
        // Notificación al vendedor si es pedido grande
        if (order.TotalAmount > 1000)
        {
            await _emailService.SendHighValueOrderNotificationAsync(
                order.SalesRepEmail, order);
            notifications.Add($"High-value alert sent to sales rep");
        }
        
        return new NotificationResult
        {
            Success = true,
            OrderId = order.Id,
            NotificationsSent = notifications.ToArray(),
            Message = $"Order {order.Id} processed successfully"
        };
    }
    
    private async Task<NotificationResult> HandleFailedOrderAsync(
        OrderRequest request, MlErrorsDetails errors)
    {
        var notifications = new List<string>();
        
        // Email de error al cliente
        if (!string.IsNullOrEmpty(request.CustomerEmail))
        {
            await _emailService.SendOrderErrorNotificationAsync(
                request.CustomerEmail, errors);
            notifications.Add($"Error notification sent to {request.CustomerEmail}");
        }
        
        // Alerta al soporte si es error crítico
        if (IsCriticalError(errors))
        {
            await _emailService.SendCriticalErrorAlertAsync(
                "support@company.com", request, errors);
            notifications.Add("Critical error alert sent to support");
        }
        
        // Log para análisis
        await _logger.LogErrorAsync($"Order processing failed", new
        {
            CustomerId = request.CustomerId,
            Errors = errors.AllErrors,
            RequestData = request
        });
        
        return new NotificationResult
        {
            Success = false,
            ErrorCode = DetermineErrorCode(errors),
            ErrorMessage = errors.FirstErrorMessage,
            NotificationsSent = notifications.ToArray(),
            Message = "Order processing failed, notifications sent"
        };
    }
    
    private bool IsCriticalError(MlErrorsDetails errors)
    {
        return errors.AllErrors.Any(e => 
            e.Contains("payment gateway") || 
            e.Contains("database") || 
            e.Contains("critical"));
    }
}

public class NotificationResult
{
    public bool Success { get; set; }
    public Guid? OrderId { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string[] NotificationsSent { get; set; }
    public string Message { get; set; }
}
```

### Ejemplo 3: Transform Pipeline con Match Final

```csharp
public class DataTransformationService
{
    public async Task<ProcessingReport> ProcessDataFileAsync(string filePath)
    {
        var startTime = DateTime.UtcNow;
        
        return await ReadFileAsync(filePath)
            .BindAsync(async content => await ValidateFormatAsync(content))
            .BindAsync(async validData => await TransformDataAsync(validData))
            .BindAsync(async transformed => await SaveToDestinationAsync(transformed))
            .TryMatchAsync(
                validAsync: async result => await CreateSuccessReportAsync(result, startTime),
                failAsync: async errors => await CreateFailureReportAsync(errors, startTime, filePath),
                ex => $"Report generation failed: {ex.Message}"
            );
    }
    
    private async Task<ProcessingReport> CreateSuccessReportAsync(
        SaveResult result, DateTime startTime)
    {
        var duration = DateTime.UtcNow - startTime;
        
        return new ProcessingReport
        {
            Success = true,
            ProcessingTime = duration,
            RecordsProcessed = result.RecordCount,
            RecordsSuccessful = result.RecordCount,
            RecordsFailed = 0,
            OutputPath = result.OutputPath,
            Summary = $"Successfully processed {result.RecordCount} records in {duration.TotalSeconds:F2} seconds",
            Details = new[]
            {
                $"Input validation: OK",
                $"Data transformation: OK", 
                $"Output generation: OK",
                $"Throughput: {result.RecordCount / duration.TotalSeconds:F2} records/sec"
            }
        };
    }
    
    private async Task<ProcessingReport> CreateFailureReportAsync(
        MlErrorsDetails errors, DateTime startTime, string filePath)
    {
        var duration = DateTime.UtcNow - startTime;
        var errorAnalysis = AnalyzeErrors(errors);
        
        // Log para troubleshooting
        await LogProcessingFailureAsync(filePath, errors, duration);
        
        return new ProcessingReport
        {
            Success = false,
            ProcessingTime = duration,
            ErrorStage = errorAnalysis.Stage,
            ErrorCategory = errorAnalysis.Category,
            ErrorMessage = errors.FirstErrorMessage,
            AllErrors = errors.AllErrors,
            Summary = $"Processing failed at {errorAnalysis.Stage} stage after {duration.TotalSeconds:F2} seconds",
            Details = new[]
            {
                $"File: {Path.GetFileName(filePath)}",
                $"Stage: {errorAnalysis.Stage}",
                $"Category: {errorAnalysis.Category}",
                $"Error count: {errors.AllErrors.Length}"
            },
            Recommendations = GenerateRecommendations(errorAnalysis)
        };
    }
    
    private ErrorAnalysis AnalyzeErrors(MlErrorsDetails errors)
    {
        foreach (var error in errors.AllErrors)
        {
            if (error.Contains("file") || error.Contains("read"))
                return new ErrorAnalysis { Stage = "FileReading", Category = "IO" };
            if (error.Contains("format") || error.Contains("validation"))
                return new ErrorAnalysis { Stage = "Validation", Category = "Format" };
            if (error.Contains("transform"))
                return new ErrorAnalysis { Stage = "Transformation", Category = "Logic" };
            if (error.Contains("save") || error.Contains("output"))
                return new ErrorAnalysis { Stage = "Output", Category = "IO" };
        }
        
        return new ErrorAnalysis { Stage = "Unknown", Category = "General" };
    }
    
    private string[] GenerateRecommendations(ErrorAnalysis analysis)
    {
        return analysis.Category switch
        {
            "IO" => new[] 
            {
                "Check file permissions and paths",
                "Verify disk space availability",
                "Ensure network connectivity if using remote paths"
            },
            "Format" => new[]
            {
                "Validate input file format",
                "Check for required headers/fields",
                "Verify data type compatibility"
            },
            "Logic" => new[]
            {
                "Review transformation rules",
                "Check for edge cases in data",
                "Validate business logic constraints"
            },
            _ => new[] { "Review error details and contact support" }
        };
    }
}

public class ProcessingReport
{
    public bool Success { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsSuccessful { get; set; }
    public int RecordsFailed { get; set; }
    public string OutputPath { get; set; }
    public string ErrorStage { get; set; }
    public string ErrorCategory { get; set; }
    public string ErrorMessage { get; set; }
    public string[] AllErrors { get; set; }
    public string Summary { get; set; }
    public string[] Details { get; set; }
    public string[] Recommendations { get; set; }
}

public class ErrorAnalysis
{
    public string Stage { get; set; }
    public string Category { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar Match vs Bind/Map

```csharp
// ✅ Correcto: Usar Match para finalizar cadenas y cambiar de tipo
var response = ProcessOrder(order)
    .Bind(CalculateShipping)
    .Bind(ApplyDiscounts)
    .Match(
        valid: finalOrder => new ApiResponse { Data = finalOrder, Success = true },
        fail: errors => new ApiResponse { Errors = errors.AllErrors, Success = false }
    );

// ✅ Correcto: Usar Match para extraer valores específicos
var statusCode = ValidateUser(userData)
    .Match(
        valid: user => 200,
        fail: errors => errors.AllErrors.Any(e => e.Contains("not found")) ? 404 : 400
    );

// ❌ Incorrecto: Usar Match cuando podrías continuar la cadena
var result = ProcessData(data)
    .Match(
        valid: d => MlResult<TransformedData>.Valid(Transform(d)),
        fail: errors => MlResult<TransformedData>.Fail(errors.AllErrors)
    ); // Mejor usar Map o Bind
```

### 2. Manejo de Excepciones

```csharp
// ✅ Correcto: Usar TryMatch para operaciones que pueden fallar
var result = GetUserData(userId)
    .TryMatch(
        valid: user => JsonSerializer.Serialize(user), // Puede lanzar excepción
        fail: errors => $"{{\"error\": \"{errors.FirstErrorMessage}\"}}",
        ex => $"Serialization failed: {ex.Message}"
    );

// ✅ Correcto: Manejo defensivo en funciones complejas
var response = ProcessComplexData(data)
    .TryMatch(
        valid: d => ComplexTransformation(d),
        fail: errors => CreateErrorResponse(errors),
        ex => $"Complex transformation failed: {ex.Message}"
    );

// ❌ Incorrecto: No manejar excepciones en operaciones riesgosas
var result = GetData(id)
    .Match(
        valid: data => RiskyOperation(data), // Puede lanzar excepción no controlada
        fail: errors => DefaultValue()
    );
```

### 3. Funciones Asíncronas

```csharp
// ✅ Correcto: Usar versión asíncrona apropiada
var notification = await ProcessOrderAsync(order)
    .MatchAsync(
        validAsync: async o => await SendSuccessEmailAsync(o.CustomerId),
        failAsync: async errors => await SendErrorEmailAsync(errors)
    );

// ✅ Correcto: Combinar sync/async según necesidad
var result = await GetDataAsync(id)
    .MatchAsync(
        validAsync: async data => await ProcessAsync(data),
        fail: errors => ProcessErrors(errors) // Síncrono, más rápido
    );

// ❌ Incorrecto: Hacer operaciones síncronas en versiones asíncronas innecesariamente
var result = await GetDataAsync(id)
    .MatchAsync(
        validAsync: async data => await Task.FromResult(SimpleTransform(data)),
        failAsync: async errors => await Task.FromResult(CreateError(errors))
    ); // Mejor usar versión síncrona
```

---

## Comparación con Bind y Map

### Tabla Comparativa

| Método | Entrada | Salida | Propósito | Cuándo Usar |
|--------|---------|--------|-----------|-------------|
| `Map` | `MlResult<T>` | `MlResult<TResult>` | Transformar valor manteniendo contexto | Transformaciones que continúan la cadena |
| `Bind` | `MlResult<T>` | `MlResult<TResult>` | Encadenar operaciones que pueden fallar | Operaciones secuenciales con validación |
| `Match` | `MlResult<T>` | `TReturn` | Extraer/transformar finalizando contexto | Finalizar cadenas, cambiar de tipo |

### Ejemplo Comparativo

```csharp
// Escenario: Procesar usuario y retornar response HTTP
var userId = 123;

// Con Map + Match (recomendado para transformaciones + finalización)
var response1 = GetUser(userId)
    .Map(user => user.ToDto())
    .Match(
        valid: dto => Ok(dto),
        fail: errors => BadRequest(errors.AllErrors)
    );

// Con Bind + Match (recomendado para validaciones + finalización)
var response2 = GetUser(userId)
    .Bind(user => ValidateUserPermissions(user))
    .Match(
        valid: validUser => Ok(validUser.ToDto()),
        fail: errors => Unauthorized(errors.AllErrors)
    );

// Solo Match (recomendado para casos simples)
var response3 = GetUser(userId)
    .Match(
        valid: user => Ok(user.ToDto()),
        fail: errors => NotFound(errors.AllErrors)
    );
```

---

## Resumen

Los métodos `Match` proporcionan **pattern matching funcional** para `MlResult<T>`:

- **`Match`**: Transforma a cualquier tipo con lógica diferenciada
- **`MatchAsync`**: Soporte completo para operaciones asíncronas  
- **`TryMatch`**: Versiones seguras que capturan excepciones

**Casos de uso ideales**:
- **Finalizar cadenas de `MlResult`** extrayendo valores
- **Crear responses HTTP** con lógica diferente para éxito/fallo
- **Transformar a DTOs o tipos externos** 
- **Generar reportes o logs** basados en el resultado

**Ventajas principales**:
- **Finalización limpia** de pipelines funcionales
- **Bifurcación clara** entre lógica de éxito y fallo
- **Flexibilidad total** en tipos de retorno
- **Soporte asíncrono completo** para todas las combinaciones