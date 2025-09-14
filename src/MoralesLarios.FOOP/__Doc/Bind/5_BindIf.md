# MlResultActionsBind - Operaciones de Binding Condicional

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Funcionalidad](#análisis-de-la-funcionalidad)
3. [Variantes de BindIf](#variantes-de-bindif)
4. [Variantes de TryBindIf](#variantes-de-trybindif)
5. [Patrones de Uso](#patrones-de-uso)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La sección **BindIf** de `MlResultActionsBind` proporciona operaciones de binding condicional que permiten ejecutar diferentes funciones basándose en una condición evaluada sobre el valor exitoso. Estas operaciones implementan patrones de **ramificación controlada** en pipelines funcionales, permitiendo crear flujos de procesamiento que se adaptan dinámicamente a las características de los datos.

### Propósito Principal

- **Ramificación Condicional**: Ejecutar diferentes funciones según una condición
- **Flujos Adaptativos**: Crear pipelines que se adaptan a las características de los datos
- **Procesamiento Contextual**: Aplicar lógica diferente según el contexto del valor
- **Optimización Selectiva**: Ejecutar procesamientos costosos solo cuando sea necesario

---

## Análisis de la Funcionalidad

### Filosofía de BindIf

```
Valor Exitoso → Evaluar Condición → ¿Verdadera?
                                    ├─ Sí → Función A
                                    └─ No → Función B
                                           ↓
                                    Resultado Final
```

### Comportamiento Base

1. **Si el resultado fuente es fallido**: Propaga el error inmediatamente
2. **Si el resultado fuente es exitoso**: Evalúa la condición sobre el valor
3. **Si la condición es verdadera**: Ejecuta `funcTrue`
4. **Si la condición es falsa**: Ejecuta `funcFalse`

### Tipos de Operaciones

1. **BindIf con Dos Funciones**: Ramificación completa con función para cada rama
2. **BindIf con Una Función**: Procesamiento condicional (solo ejecuta si condición es verdadera)
3. **TryBindIf**: Versiones seguras que capturan excepciones

---

## Variantes de BindIf

### Variante 1: BindIf con Ramificación Completa

**Propósito**: Ejecuta una función u otra dependiendo de la condición

```csharp
public static MlResult<TReturn> BindIf<T, TReturn>(this MlResult<T> source,
                                                   Func<T, bool> condition,
                                                   Func<T, MlResult<TReturn>> funcTrue,
                                                   Func<T, MlResult<TReturn>> funcFalse)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `condition`: Función que evalúa la condición sobre el valor
- `funcTrue`: Función a ejecutar si la condición es verdadera
- `funcFalse`: Función a ejecutar si la condición es falsa

**Comportamiento**:
- Si `source` es fallido: Propaga el error
- Si `source` es exitoso: Evalúa `condition(value)` y ejecuta la función correspondiente

### Variante 2: BindIf con Procesamiento Opcional

**Propósito**: Ejecuta una función solo si la condición se cumple, sino retorna el valor sin cambios

```csharp
public static MlResult<T> BindIf<T>(this MlResult<T> source,
                                    Func<T, bool> condition,
                                    Func<T, MlResult<T>> func)
```

**Comportamiento**:
- Si la condición es verdadera: Ejecuta `func(value)`
- Si la condición es falsa: Retorna `MlResult<T>.Valid(value)` (valor sin procesar)

### Soporte Asíncrono Completo

Cada variante soporta todas las combinaciones asíncronas:
- **Función true asíncrona, función false síncrona**
- **Función true síncrona, función false asíncrona**
- **Ambas funciones asíncronas**
- **Fuente asíncrona con cualquier combinación de funciones**

---

## Variantes de TryBindIf

### TryBindIf con Ramificación Completa

**Propósito**: Versión segura de BindIf que captura excepciones en ambas ramas

```csharp
public static MlResult<TReturn> TryBindIf<T, TReturn>(this MlResult<T> source,
                                                      Func<T, bool> condition,
                                                      Func<T, MlResult<TReturn>> funcTrue,
                                                      Func<T, MlResult<TReturn>> funcFalse,
                                                      Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Captura excepciones tanto en `funcTrue` como en `funcFalse`
- Usa `errorMessageBuilder` para crear mensajes de error contextuales

### TryBindIf con Procesamiento Opcional

**Propósito**: Versión segura de BindIf que solo ejecuta función si condición es verdadera

```csharp
public static MlResult<T> TryBindIf<T>(this MlResult<T> source,
                                       Func<T, bool> condition,
                                       Func<T, MlResult<T>> func,
                                       Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Si la condición es falsa: Retorna el valor sin procesar (sin riesgo de excepción)
- Si la condición es verdadera: Ejecuta `func` de forma segura

### Soporte para Mensajes Simples

Todas las variantes de `TryBindIf` incluyen sobrecargas que aceptan un `string` simple en lugar de `Func<Exception, string>`:

```csharp
public static MlResult<T> TryBindIf<T>(this MlResult<T> source,
                                       Func<T, bool> condition,
                                       Func<T, MlResult<T>> func,
                                       string errorMessage = null!)
```

---

## Patrones de Uso

### Patrón 1: Ramificación por Tipo de Datos

```csharp
// Procesamiento diferente según el tipo de usuario
var result = userData
    .BindIf(
        condition: user => user.IsPremium,
        funcTrue: user => ProcessPremiumUser(user),
        funcFalse: user => ProcessStandardUser(user)
    );
```

### Patrón 2: Optimización Condicional

```csharp
// Solo ejecutar procesamiento costoso si es necesario
var result = imageData
    .BindIf(
        condition: image => image.Size > LARGE_IMAGE_THRESHOLD,
        func: image => CompressImage(image) // Solo si es grande
    );
```

### Patrón 3: Validación Condicional

```csharp
// Validación adicional solo para ciertos casos
var result = orderData
    .BindIf(
        condition: order => order.Amount > HIGH_VALUE_THRESHOLD,
        funcTrue: order => ValidateHighValueOrder(order),
        funcFalse: order => ValidateStandardOrder(order)
    );
```

### Patrón 4: Manejo de Excepciones Contextuales

```csharp
// Procesamiento seguro con mensajes específicos por rama
var result = paymentData
    .TryBindIf(
        condition: payment => payment.IsInternational,
        funcTrue: payment => ProcessInternationalPayment(payment),
        funcFalse: payment => ProcessDomesticPayment(payment),
        errorMessageBuilder: ex => $"Payment processing failed: {ex.Message}"
    );
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Usuarios

```csharp
public class UserProcessingService
{
    public async Task<MlResult<ProcessedUser>> ProcessUserAsync(UserData userData)
    {
        return await userData.ToMlResult()
            .BindIfAsync(
                condition: user => user.AccountType == AccountType.Premium,
                funcTrueAsync: async user => await ProcessPremiumUserAsync(user),
                funcFalse: user => ProcessStandardUser(user)
            )
            .BindIfAsync(
                condition: processedUser => processedUser.RequiresVerification,
                funcAsync: async user => await AddVerificationStepAsync(user)
            )
            .BindIfAsync(
                condition: user => user.IsNewUser,
                funcTrueAsync: async user => await SetupNewUserAsync(user),
                funcFalseAsync: async user => await UpdateExistingUserAsync(user)
            );
    }
    
    public async Task<MlResult<ProcessedUser>> ProcessUserSafelyAsync(UserData userData)
    {
        return await userData.ToMlResult()
            .TryBindIfAsync(
                condition: user => user.AccountType == AccountType.Premium,
                funcTrueAsync: async user => await ProcessPremiumUserWithExternalAPIAsync(user),
                funcFalse: user => ProcessStandardUser(user),
                errorMessageBuilder: ex => $"Failed to process {userData.AccountType} user {userData.Id}: {ex.Message}"
            )
            .TryBindIfAsync(
                condition: user => user.RequiresComplexValidation,
                funcAsync: async user => await PerformComplexValidationAsync(user),
                errorMessage: "Complex validation failed"
            );
    }
    
    private async Task<MlResult<ProcessedUser>> ProcessPremiumUserAsync(UserData user)
    {
        await Task.Delay(100); // Simular procesamiento más complejo
        
        var processedUser = new ProcessedUser
        {
            Id = user.Id,
            Name = user.Name,
            AccountType = user.AccountType,
            PremiumFeatures = new List<string> { "AdvancedAnalytics", "PrioritySupport", "CustomReports" },
            ProcessingLevel = "Premium",
            ProcessedAt = DateTime.UtcNow,
            RequiresVerification = user.Email.EndsWith("@enterprise.com")
        };
        
        return MlResult<ProcessedUser>.Valid(processedUser);
    }
    
    private MlResult<ProcessedUser> ProcessStandardUser(UserData user)
    {
        var processedUser = new ProcessedUser
        {
            Id = user.Id,
            Name = user.Name,
            AccountType = user.AccountType,
            PremiumFeatures = new List<string>(),
            ProcessingLevel = "Standard",
            ProcessedAt = DateTime.UtcNow,
            RequiresVerification = false
        };
        
        return MlResult<ProcessedUser>.Valid(processedUser);
    }
    
    private async Task<MlResult<ProcessedUser>> ProcessPremiumUserWithExternalAPIAsync(UserData user)
    {
        await Task.Delay(150);
        
        // Simular posible excepción en API externa
        if (user.Id % 10 == 0)
            throw new ExternalApiException($"External API failed for user {user.Id}");
        
        var processedUser = new ProcessedUser
        {
            Id = user.Id,
            Name = user.Name,
            AccountType = user.AccountType,
            PremiumFeatures = new List<string> { "AdvancedAnalytics", "PrioritySupport", "ExternalIntegration" },
            ProcessingLevel = "Premium_External",
            ProcessedAt = DateTime.UtcNow,
            ExternalData = $"ExternalData_{user.Id}",
            RequiresVerification = true
        };
        
        return MlResult<ProcessedUser>.Valid(processedUser);
    }
    
    private async Task<MlResult<ProcessedUser>> AddVerificationStepAsync(ProcessedUser user)
    {
        await Task.Delay(50);
        
        user.VerificationToken = Guid.NewGuid().ToString();
        user.VerificationRequired = true;
        user.VerificationAddedAt = DateTime.UtcNow;
        
        return MlResult<ProcessedUser>.Valid(user);
    }
    
    private async Task<MlResult<ProcessedUser>> SetupNewUserAsync(ProcessedUser user)
    {
        await Task.Delay(75);
        
        user.WelcomeEmailSent = true;
        user.OnboardingStepsCreated = true;
        user.InitialPreferencesSet = true;
        user.SetupCompletedAt = DateTime.UtcNow;
        
        return MlResult<ProcessedUser>.Valid(user);
    }
    
    private async Task<MlResult<ProcessedUser>> UpdateExistingUserAsync(ProcessedUser user)
    {
        await Task.Delay(30);
        
        user.LastUpdated = DateTime.UtcNow;
        user.UpdateCount = (user.UpdateCount ?? 0) + 1;
        
        return MlResult<ProcessedUser>.Valid(user);
    }
    
    private async Task<MlResult<ProcessedUser>> PerformComplexValidationAsync(ProcessedUser user)
    {
        await Task.Delay(200); // Validación compleja y costosa
        
        // Simular posible fallo en validación compleja
        if (user.Name.Contains("invalid"))
            throw new ValidationException($"Complex validation failed for user {user.Name}");
        
        user.ComplexValidationPassed = true;
        user.ValidationTimestamp = DateTime.UtcNow;
        
        return MlResult<ProcessedUser>.Valid(user);
    }
}

// Clases de apoyo
public enum AccountType
{
    Standard,
    Premium,
    Enterprise
}

public class UserData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public AccountType AccountType { get; set; }
    public bool IsNewUser { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProcessedUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public AccountType AccountType { get; set; }
    public List<string> PremiumFeatures { get; set; } = new();
    public string ProcessingLevel { get; set; }
    public DateTime ProcessedAt { get; set; }
    public bool RequiresVerification { get; set; }
    public string VerificationToken { get; set; }
    public bool VerificationRequired { get; set; }
    public DateTime? VerificationAddedAt { get; set; }
    public bool WelcomeEmailSent { get; set; }
    public bool OnboardingStepsCreated { get; set; }
    public bool InitialPreferencesSet { get; set; }
    public DateTime? SetupCompletedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
    public int? UpdateCount { get; set; }
    public string ExternalData { get; set; }
    public bool ComplexValidationPassed { get; set; }
    public DateTime? ValidationTimestamp { get; set; }
    public bool IsNewUser => CreatedAt > DateTime.UtcNow.AddDays(-7);
    public bool RequiresComplexValidation => AccountType == AccountType.Enterprise;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddDays(-30);
}

public class ExternalApiException : Exception
{
    public ExternalApiException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
```

### Ejemplo 2: Sistema de Procesamiento de Órdenes

```csharp
public class OrderProcessingService
{
    private const decimal HIGH_VALUE_THRESHOLD = 1000m;
    private const decimal INTERNATIONAL_SHIPPING_THRESHOLD = 500m;
    
    public async Task<MlResult<ProcessedOrder>> ProcessOrderAsync(OrderRequest orderRequest)
    {
        return await orderRequest.ToMlResult()
            .BindIfAsync(
                condition: order => order.TotalAmount >= HIGH_VALUE_THRESHOLD,
                funcTrueAsync: async order => await ProcessHighValueOrderAsync(order),
                funcFalse: order => ProcessStandardOrder(order)
            )
            .BindIfAsync(
                condition: order => order.ShippingAddress.Country != "USA",
                funcTrueAsync: async order => await ProcessInternationalShippingAsync(order),
                funcFalse: order => ProcessDomesticShipping(order)
            )
            .BindIfAsync(
                condition: order => order.HasPerishableItems,
                funcAsync: async order => await AddExpeditedProcessingAsync(order)
            )
            .BindIfAsync(
                condition: order => order.CustomerTier == CustomerTier.VIP,
                funcTrueAsync: async order => await ApplyVIPBenefitsAsync(order),
                funcFalseAsync: async order => await ApplyStandardProcessingAsync(order)
            );
    }
    
    public async Task<MlResult<ProcessedOrder>> ProcessOrderSafelyAsync(OrderRequest orderRequest)
    {
        return await orderRequest.ToMlResult()
            .TryBindIfAsync(
                condition: order => order.PaymentMethod == PaymentMethod.CreditCard,
                funcTrueAsync: async order => await ProcessCreditCardPaymentAsync(order),
                funcFalse: order => ProcessCashPayment(order),
                errorMessageBuilder: ex => $"Payment processing failed for order {orderRequest.Id}: {ex.Message}"
            )
            .TryBindIfAsync(
                condition: order => order.RequiresInventoryCheck,
                funcAsync: async order => await PerformInventoryCheckAsync(order),
                errorMessage: "Inventory check failed"
            )
            .TryBindIfAsync(
                condition: order => order.NeedsThirdPartyValidation,
                funcTrueAsync: async order => await ValidateWithThirdPartyAsync(order),
                funcFalse: order => SkipThirdPartyValidation(order),
                errorMessageBuilder: ex => ex switch
                {
                    TimeoutException => "Third-party validation timed out",
                    HttpRequestException => "Third-party service unavailable",
                    _ => $"Third-party validation error: {ex.Message}"
                }
            );
    }
    
    private async Task<MlResult<ProcessedOrder>> ProcessHighValueOrderAsync(OrderRequest order)
    {
        await Task.Delay(200); // Procesamiento adicional para pedidos de alto valor
        
        var processedOrder = new ProcessedOrder
        {
            Id = Guid.NewGuid(),
            OriginalRequest = order,
            ProcessingType = "HighValue",
            RequiresManagerApproval = true,
            FraudCheckRequired = true,
            InsuranceRequired = true,
            ProcessedAt = DateTime.UtcNow
        };
        
        // Verificaciones adicionales para pedidos de alto valor
        if (order.TotalAmount > 5000m)
        {
            processedOrder.RequiresDirectorApproval = true;
            processedOrder.ExtendedWarrantyIncluded = true;
        }
        
        return MlResult<ProcessedOrder>.Valid(processedOrder);
    }
    
    private MlResult<ProcessedOrder> ProcessStandardOrder(OrderRequest order)
    {
        var processedOrder = new ProcessedOrder
        {
            Id = Guid.NewGuid(),
            OriginalRequest = order,
            ProcessingType = "Standard",
            RequiresManagerApproval = false,
            FraudCheckRequired = order.TotalAmount > 100m,
            InsuranceRequired = false,
            ProcessedAt = DateTime.UtcNow
        };
        
        return MlResult<ProcessedOrder>.Valid(processedOrder);
    }
    
    private async Task<MlResult<ProcessedOrder>> ProcessInternationalShippingAsync(ProcessedOrder order)
    {
        await Task.Delay(150);
        
        order.ShippingType = "International";
        order.CustomsDocumentationRequired = true;
        order.EstimatedDeliveryDays = 7 + (order.OriginalRequest.TotalAmount > INTERNATIONAL_SHIPPING_THRESHOLD ? 2 : 5);
        order.TrackingRequired = true;
        
        // Cálculo de aranceles estimados
        order.EstimatedCustomsDuty = order.OriginalRequest.TotalAmount * 0.1m;
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private MlResult<ProcessedOrder> ProcessDomesticShipping(ProcessedOrder order)
    {
        order.ShippingType = "Domestic";
        order.CustomsDocumentationRequired = false;
        order.EstimatedDeliveryDays = order.OriginalRequest.TotalAmount > 50m ? 2 : 5;
        order.TrackingRequired = order.OriginalRequest.TotalAmount > 25m;
        order.EstimatedCustomsDuty = 0m;
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private async Task<MlResult<ProcessedOrder>> AddExpeditedProcessingAsync(ProcessedOrder order)
    {
        await Task.Delay(75);
        
        order.ExpeditedProcessing = true;
        order.EstimatedDeliveryDays = Math.Max(1, order.EstimatedDeliveryDays - 2);
        order.SpecialHandlingRequired = true;
        order.RefrigerationRequired = order.OriginalRequest.Items.Any(i => i.RequiresRefrigeration);
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private async Task<MlResult<ProcessedOrder>> ApplyVIPBenefitsAsync(ProcessedOrder order)
    {
        await Task.Delay(100);
        
        order.VIPProcessing = true;
        order.FreeShippingApplied = true;
        order.PriorityQueuePosition = 1;
        order.DedicatedSupportAssigned = true;
        order.VIPPackagingIncluded = true;
        
        // Descuento VIP
        order.VIPDiscountApplied = order.OriginalRequest.TotalAmount * 0.05m;
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private async Task<MlResult<ProcessedOrder>> ApplyStandardProcessingAsync(ProcessedOrder order)
    {
        await Task.Delay(50);
        
        order.VIPProcessing = false;
        order.PriorityQueuePosition = DeterminePriorityPosition(order);
        order.DedicatedSupportAssigned = false;
        
        // Verificar si aplica para envío gratis por monto
        order.FreeShippingApplied = order.OriginalRequest.TotalAmount >= 75m;
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private async Task<MlResult<ProcessedOrder>> ProcessCreditCardPaymentAsync(OrderRequest order)
    {
        await Task.Delay(100);
        
        // Simular procesamiento de tarjeta de crédito que puede fallar
        if (order.Id % 15 == 0)
            throw new PaymentProcessingException($"Credit card processing failed for order {order.Id}");
        
        var processedOrder = new ProcessedOrder
        {
            Id = Guid.NewGuid(),
            OriginalRequest = order,
            PaymentProcessed = true,
            PaymentMethod = "CreditCard",
            TransactionId = $"CC_{Guid.NewGuid().ToString("N")[..8]}",
            PaymentProcessedAt = DateTime.UtcNow
        };
        
        return MlResult<ProcessedOrder>.Valid(processedOrder);
    }
    
    private MlResult<ProcessedOrder> ProcessCashPayment(OrderRequest order)
    {
        var processedOrder = new ProcessedOrder
        {
            Id = Guid.NewGuid(),
            OriginalRequest = order,
            PaymentProcessed = true,
            PaymentMethod = "Cash",
            TransactionId = $"CASH_{DateTime.UtcNow:yyyyMMddHHmmss}",
            PaymentProcessedAt = DateTime.UtcNow,
            CashHandlingFeeApplied = order.TotalAmount > 1000m ? 10m : 0m
        };
        
        return MlResult<ProcessedOrder>.Valid(processedOrder);
    }
    
    private async Task<MlResult<ProcessedOrder>> PerformInventoryCheckAsync(ProcessedOrder order)
    {
        await Task.Delay(120);
        
        // Simular verificación de inventario que puede fallar
        if (order.OriginalRequest.Items.Any(i => i.ProductId.Contains("OUTOFSTOCK")))
            throw new InventoryException($"Items out of stock in order {order.Id}");
        
        order.InventoryChecked = true;
        order.InventoryCheckTimestamp = DateTime.UtcNow;
        order.AllItemsAvailable = true;
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private async Task<MlResult<ProcessedOrder>> ValidateWithThirdPartyAsync(ProcessedOrder order)
    {
        await Task.Delay(300); // Simulación de llamada externa lenta
        
        // Simular diferentes tipos de excepciones
        var random = new Random(order.Id);
        var errorType = random.Next(20);
        
        if (errorType == 0)
            throw new TimeoutException("Third-party validation timeout");
        if (errorType == 1)
            throw new HttpRequestException("Third-party service unavailable");
        if (errorType == 2)
            throw new UnauthorizedAccessException("Third-party authentication failed");
        
        order.ThirdPartyValidated = true;
        order.ThirdPartyValidationTimestamp = DateTime.UtcNow;
        order.ValidationScore = random.Next(70, 100);
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private MlResult<ProcessedOrder> SkipThirdPartyValidation(ProcessedOrder order)
    {
        order.ThirdPartyValidated = false;
        order.ValidationSkipped = true;
        order.ValidationSkipReason = "Not required for this order type";
        
        return MlResult<ProcessedOrder>.Valid(order);
    }
    
    private int DeterminePriorityPosition(ProcessedOrder order)
    {
        var basePosition = 100;
        
        // Ajustar posición basada en varios factores
        if (order.OriginalRequest.TotalAmount > 500m) basePosition -= 20;
        if (order.ExpeditedProcessing) basePosition -= 30;
        if (order.OriginalRequest.CustomerTier == CustomerTier.Premium) basePosition -= 15;
        
        return Math.Max(1, basePosition);
    }
}

// Clases de apoyo adicionales
public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    Cash,
    BankTransfer
}

public enum CustomerTier
{
    Standard,
    Premium,
    VIP
}

public class OrderRequest
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public CustomerTier CustomerTier { get; set; }
    public Address ShippingAddress { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public bool HasPerishableItems => Items.Any(i => i.IsPerishable);
    public bool RequiresInventoryCheck => Items.Count > 3 || TotalAmount > 200m;
    public bool NeedsThirdPartyValidation => TotalAmount > 750m || CustomerTier == CustomerTier.VIP;
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public bool IsPerishable { get; set; }
    public bool RequiresRefrigeration { get; set; }
}

public class ProcessedOrder
{
    public Guid Id { get; set; }
    public OrderRequest OriginalRequest { get; set; }
    public string ProcessingType { get; set; }
    public DateTime ProcessedAt { get; set; }
    
    // High Value Properties
    public bool RequiresManagerApproval { get; set; }
    public bool RequiresDirectorApproval { get; set; }
    public bool FraudCheckRequired { get; set; }
    public bool InsuranceRequired { get; set; }
    public bool ExtendedWarrantyIncluded { get; set; }
    
    // Shipping Properties
    public string ShippingType { get; set; }
    public bool CustomsDocumentationRequired { get; set; }
    public int EstimatedDeliveryDays { get; set; }
    public bool TrackingRequired { get; set; }
    public decimal EstimatedCustomsDuty { get; set; }
    
    // Expedited Properties
    public bool ExpeditedProcessing { get; set; }
    public bool SpecialHandlingRequired { get; set; }
    public bool RefrigerationRequired { get; set; }
    
    // VIP Properties
    public bool VIPProcessing { get; set; }
    public bool FreeShippingApplied { get; set; }
    public int PriorityQueuePosition { get; set; }
    public bool DedicatedSupportAssigned { get; set; }
    public bool VIPPackagingIncluded { get; set; }
    public decimal VIPDiscountApplied { get; set; }
    
    // Payment Properties
    public bool PaymentProcessed { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public DateTime? PaymentProcessedAt { get; set; }
    public decimal CashHandlingFeeApplied { get; set; }
    
    // Inventory Properties
    public bool InventoryChecked { get; set; }
    public DateTime? InventoryCheckTimestamp { get; set; }
    public bool AllItemsAvailable { get; set; }
    
    // Third Party Validation Properties
    public bool ThirdPartyValidated { get; set; }
    public DateTime? ThirdPartyValidationTimestamp { get; set; }
    public int ValidationScore { get; set; }
    public bool ValidationSkipped { get; set; }
    public string ValidationSkipReason { get; set; }
}

// Excepciones personalizadas
public class PaymentProcessingException : Exception
{
    public PaymentProcessingException(string message) : base(message) { }
}

public class InventoryException : Exception
{
    public InventoryException(string message) : base(message) { }
}
```

### Ejemplo 3: Sistema de Procesamiento de Imágenes

```csharp
public class ImageProcessingService
{
    private const long LARGE_IMAGE_SIZE = 5_000_000; // 5MB
    private const int HIGH_RESOLUTION_THRESHOLD = 1920;
    
    public async Task<MlResult<ProcessedImage>> ProcessImageAsync(ImageData imageData)
    {
        return await imageData.ToMlResult()
            .BindIfAsync(
                condition: img => img.SizeInBytes > LARGE_IMAGE_SIZE,
                funcAsync: async img => await CompressLargeImageAsync(img)
            )
            .BindIfAsync(
                condition: img => img.Width > HIGH_RESOLUTION_THRESHOLD || img.Height > HIGH_RESOLUTION_THRESHOLD,
                funcTrueAsync: async img => await ProcessHighResolutionImageAsync(img),
                funcFalse: img => ProcessStandardResolutionImage(img)
            )
            .BindIfAsync(
                condition: img => img.Format == ImageFormat.RAW,
                funcAsync: async img => await ConvertFromRAWAsync(img)
            )
            .BindIfAsync(
                condition: img => img.RequiresWatermark,
                funcTrueAsync: async img => await AddWatermarkAsync(img),
                funcFalse: img => SkipWatermark(img)
            );
    }
    
    public async Task<MlResult<ProcessedImage>> ProcessImageSafelyAsync(ImageData imageData)
    {
        return await imageData.ToMlResult()
            .TryBindIfAsync(
                condition: img => img.RequiresColorCorrection,
                funcAsync: async img => await PerformColorCorrectionAsync(img),
                errorMessage: "Color correction failed"
            )
            .TryBindIfAsync(
                condition: img => img.IsPortrait,
                funcTrueAsync: async img => await ProcessPortraitImageAsync(img),
                funcFalse: img => ProcessLandscapeImage(img),
                errorMessageBuilder: ex => $"Portrait/Landscape processing failed: {ex.Message}"
            )
            .TryBindIfAsync(
                condition: img => img.RequiresNoiseReduction,
                funcAsync: async img => await ReduceNoiseAsync(img),
                errorMessageBuilder: ex => ex switch
                {
                    OutOfMemoryException => "Insufficient memory for noise reduction",
                    InvalidOperationException => "Invalid image format for noise reduction",
                    _ => $"Noise reduction failed: {ex.Message}"
                }
            );
    }
    
    private async Task<MlResult<ProcessedImage>> CompressLargeImageAsync(ImageData imageData)
    {
        await Task.Delay(500); // Simulación de compresión que toma tiempo
        
        var compressionRatio = CalculateCompressionRatio(imageData.SizeInBytes);
        
        var processedImage = new ProcessedImage
        {
            OriginalData = imageData,
            SizeInBytes = (long)(imageData.SizeInBytes * compressionRatio),
            Width = imageData.Width,
            Height = imageData.Height,
            Format = imageData.Format,
            CompressionApplied = true,
            CompressionRatio = compressionRatio,
            ProcessingSteps = new List<string> { "LargeImageCompression" }
        };
        
        return MlResult<ProcessedImage>.Valid(processedImage);
    }
    
    private async Task<MlResult<ProcessedImage>> ProcessHighResolutionImageAsync(ProcessedImage image)
    {
        await Task.Delay(300);
        
        // Procesamiento especial para imágenes de alta resolución
        image.HighResolutionProcessing = true;
        image.OptimizedForWeb = true;
        image.ThumbnailGenerated = true;
        image.ProcessingSteps.Add("HighResolutionProcessing");
        
        // Generar múltiples tamaños
        image.GeneratedSizes = new Dictionary<string, (int Width, int Height)>
        {
            ["thumbnail"] = (150, 150),
            ["small"] = (400, 300),
            ["medium"] = (800, 600),
            ["large"] = (1200, 900)
        };
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private MlResult<ProcessedImage> ProcessStandardResolutionImage(ProcessedImage image)
    {
        image.HighResolutionProcessing = false;
        image.OptimizedForWeb = true;
        image.ThumbnailGenerated = true;
        image.ProcessingSteps.Add("StandardResolutionProcessing");
        
        // Solo generar tamaños básicos
        image.GeneratedSizes = new Dictionary<string, (int Width, int Height)>
        {
            ["thumbnail"] = (150, 150),
            ["medium"] = (400, 300)
        };
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private async Task<MlResult<ProcessedImage>> ConvertFromRAWAsync(ProcessedImage image)
    {
        await Task.Delay(800); // Conversión RAW es costosa
        
        image.Format = ImageFormat.JPEG; // Convertir a JPEG
        image.RAWProcessingApplied = true;
        image.ColorSpaceConverted = true;
        image.ProcessingSteps.Add("RAWConversion");
        
        // La conversión RAW puede aumentar el tamaño temporalmente
        image.SizeInBytes = (long)(image.SizeInBytes * 1.2);
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private async Task<MlResult<ProcessedImage>> AddWatermarkAsync(ProcessedImage image)
    {
        await Task.Delay(150);
        
        image.WatermarkApplied = true;
        image.WatermarkPosition = "BottomRight";
        image.WatermarkOpacity = 0.3f;
        image.ProcessingSteps.Add("WatermarkApplication");
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private MlResult<ProcessedImage> SkipWatermark(ProcessedImage image)
    {
        image.WatermarkApplied = false;
        image.ProcessingSteps.Add("WatermarkSkipped");
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private async Task<MlResult<ProcessedImage>> PerformColorCorrectionAsync(ProcessedImage image)
    {
        await Task.Delay(200);
        
        // Simulación de operación que puede fallar
        if (image.OriginalData.Format == ImageFormat.BMP)
            throw new InvalidOperationException("Color correction not supported for BMP format");
        
        image.ColorCorrectionApplied = true;
        image.BrightnessAdjusted = true;
        image.ContrastAdjusted = true;
        image.ProcessingSteps.Add("ColorCorrection");
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private async Task<MlResult<ProcessedImage>> ProcessPortraitImageAsync(ProcessedImage image)
    {
        await Task.Delay(250);
        
        // Simulación de procesamiento que puede fallar por memoria
        if (image.Width * image.Height > 10_000_000) // 10MP
            throw new OutOfMemoryException("Image too large for portrait processing");
        
        image.PortraitProcessingApplied = true;
        image.FaceDetectionApplied = true;
        image.BackgroundBlurApplied = true;
        image.ProcessingSteps.Add("PortraitProcessing");
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private MlResult<ProcessedImage> ProcessLandscapeImage(ProcessedImage image)
    {
        image.LandscapeProcessingApplied = true;
        image.HorizonDetectionApplied = true;
        image.SkyEnhancementApplied = true;
        image.ProcessingSteps.Add("LandscapeProcessing");
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private async Task<MlResult<ProcessedImage>> ReduceNoiseAsync(ProcessedImage image)
    {
        await Task.Delay(400);
        
        // Simulación de operación intensiva que puede fallar
        if (image.Format == ImageFormat.GIF)
            throw new InvalidOperationException("Noise reduction not applicable to GIF images");
        
        if (image.SizeInBytes > 50_000_000) // 50MB
            throw new OutOfMemoryException("Image too large for noise reduction");
        
        image.NoiseReductionApplied = true;
        image.SharpeningApplied = true;
        image.ProcessingSteps.Add("NoiseReduction");
        
        return MlResult<ProcessedImage>.Valid(image);
    }
    
    private double CalculateCompressionRatio(long originalSize)
    {
        // Más compresión para archivos más grandes
        if (originalSize > 20_000_000) return 0.3; // 70% reducción
        if (originalSize > 10_000_000) return 0.5; // 50% reducción
        if (originalSize > 5_000_000) return 0.7;  // 30% reducción
        return 0.8; // 20% reducción
    }
}

// Clases de apoyo para procesamiento de imágenes
public enum ImageFormat
{
    JPEG,
    PNG,
    GIF,
    BMP,
    RAW,
    TIFF
}

public class ImageData
{
    public string FileName { get; set; }
    public long SizeInBytes { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public ImageFormat Format { get;