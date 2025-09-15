# MlResult EnsureFp - Validaciones y Precondiciones Funcionales

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos de Validación Específica](#métodos-de-validación-específica)
4. [Método Base That](#método-base-that)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Assert y Guard](#comparación-con-assert-y-guard)

---

## Introducción

La clase `EnsureFp` proporciona un conjunto de métodos para **validaciones y precondiciones funcionales** que siguen el patrón de programación funcional. Estos métodos permiten validar condiciones y retornar `MlResult<T>` en lugar de lanzar excepciones, manteniendo la cadena funcional intacta y proporcionando un control de flujo más predecible.

### Propósito Principal

- **Validaciones No-Excepcionales**: Validar condiciones sin lanzar excepciones
- **Precondiciones Funcionales**: Establecer contratos de entrada de forma funcional
- **Flujo de Control Predecible**: Mantener el patrón MlResult en validaciones
- **Composición de Validaciones**: Facilitar la combinación de múltiples validaciones

---

## Análisis de los Métodos

### Filosofía de EnsureFp

```
Valor + Condición → EnsureFp → MlResult<T>
  ↓        ↓          ↓           ↓
value + condition → Valid(value) si condition = true
  ↓        ↓          ↓           ↓
value + condition → Fail(error) si condition = false
```

### Características Principales

1. **Validación Funcional**: Sin efectos secundarios ni excepciones
2. **Preservación de Valor**: El valor original se mantiene si es válido
3. **Flexibilidad de Errores**: Soporte para mensajes simples y errores complejos
4. **Composabilidad**: Fácil integración con cadenas funcionales
5. **Soporte Async**: Versiones asíncronas para todos los métodos

---

## Métodos de Validación Específica

### 1. NotNull - Validación de Nulos

**Propósito**: Verificar que un valor no sea null

```csharp
public static MlResult<T> NotNull<T>(T value, string errorMessage)
public static MlResult<T> NotNull<T>(T value, MlErrorsDetails errorsDetails)
```

**Ejemplo Básico**:
```csharp
User user = GetUser(userId);
var validUser = EnsureFp.NotNull(user, "User not found");

// Si user != null: MlResult<User>.Valid(user)
// Si user == null: MlResult<User>.Fail("User not found")
```

### 2. NotEmpty - Validación de Colecciones Vacías

**Propósito**: Verificar que una colección no esté vacía ni sea null

```csharp
public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, string message)
public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails)
```

**Ejemplo**:
```csharp
var items = GetOrderItems(orderId);
var validItems = EnsureFp.NotEmpty(items, "Order must contain at least one item");

// Si items tiene elementos: MlResult<IEnumerable<Item>>.Valid(items)
// Si items es null o vacío: MlResult<IEnumerable<Item>>.Fail("Order must contain at least one item")
```

### 3. NotNullEmptyOrWhitespace - Validación de Strings

**Propósito**: Verificar que un string no sea null, vacío o solo espacios en blanco

```csharp
public static MlResult<string> NotNullEmptyOrWhitespace(string value, string errorMessage)
public static MlResult<string> NotNullEmptyOrWhitespace(string value, MlErrorsDetails errorsDetails)
```

**Ejemplo**:
```csharp
string customerName = GetCustomerName();
var validName = EnsureFp.NotNullEmptyOrWhitespace(customerName, "Customer name is required");

// Si customerName tiene contenido: MlResult<string>.Valid(customerName)
// Si customerName es null/""/espacios: MlResult<string>.Fail("Customer name is required")
```

---

## Método Base That

### Validación Condicional Genérica

**Propósito**: Método base para validaciones personalizadas con cualquier condición

```csharp
public static MlResult<T> That<T>(T value, bool condition, string errorMessage)
public static MlResult<T> That<T>(T value, bool condition, MlErrorsDetails errorsDetails)
```

**Ejemplo de Uso Avanzado**:
```csharp
// Validación de edad
var age = 25;
var validAge = EnsureFp.That(age, age >= 18 && age <= 120, "Age must be between 18 and 120");

// Validación de email
var email = "user@example.com";
var validEmail = EnsureFp.That(email, IsValidEmailFormat(email), "Invalid email format");

// Validación de rango de fechas
var date = DateTime.Now;
var validDate = EnsureFp.That(date, date > DateTime.Now.AddDays(-30), "Date cannot be older than 30 days");
```

---

## Variantes Asíncronas

### Soporte Async Completo

**Todos los métodos tienen equivalentes asíncronos**:

```csharp
// Versiones async de todos los métodos
public static Task<MlResult<T>> NotNullAsync<T>(T value, string errorMessage)
public static Task<MlResult<T>> NotNullAsync<T>(T value, MlErrorsDetails errorsDetails)

public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, string message)
public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails)

public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, string errorMessage)
public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, MlErrorsDetails errorsDetails)

public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, string errorMessage)
public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, MlErrorsDetails errorsDetails)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Validación de Pedidos

```csharp
public class OrderValidationService
{
    private readonly ICustomerService _customerService;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    
    public async Task<MlResult<ValidatedOrder>> ValidateOrderAsync(OrderRequest request)
    {
        // Validación en cadena usando EnsureFp
        var validationResult = await ValidateOrderRequestAsync(request)
            .BindAsync(async validRequest => await ValidateCustomerAsync(validRequest))
            .BindAsync(async orderWithCustomer => await ValidateOrderItemsAsync(orderWithCustomer))
            .BindAsync(async orderWithItems => await ValidatePaymentInfoAsync(orderWithItems))
            .BindAsync(async completeOrder => await ValidateBusinessRulesAsync(completeOrder));
        
        return validationResult.Match(
            valid: validOrder => MlResult<ValidatedOrder>.Valid(new ValidatedOrder
            {
                OrderId = Guid.NewGuid(),
                Request = validOrder.Request,
                Customer = validOrder.Customer,
                ValidatedItems = validOrder.Items,
                PaymentInfo = validOrder.PaymentInfo,
                ValidatedAt = DateTime.UtcNow,
                ValidationId = Guid.NewGuid()
            }),
            fail: errors => MlResult<ValidatedOrder>.Fail(errors.AllErrors)
        );
    }
    
    private async Task<MlResult<ValidatedOrderRequest>> ValidateOrderRequestAsync(OrderRequest request)
    {
        // Usar EnsureFp para validaciones básicas
        var requestValidation = EnsureFp.NotNull(request, "Order request is required");
        
        return await requestValidation.BindAsync(async validRequest =>
        {
            // Validar ID del cliente
            var customerIdValidation = EnsureFp.That(
                validRequest.CustomerId,
                validRequest.CustomerId > 0,
                "Customer ID must be positive");
            
            // Validar items del pedido
            var itemsValidation = EnsureFp.NotEmpty(
                validRequest.Items,
                "Order must contain at least one item");
            
            // Validar información de envío
            var shippingValidation = EnsureFp.NotNull(
                validRequest.ShippingAddress,
                "Shipping address is required");
            
            // Validar dirección de facturación
            var billingValidation = EnsureFp.NotNull(
                validRequest.BillingAddress,
                "Billing address is required");
            
            // Combinar todas las validaciones
            return customerIdValidation
                .Bind(_ => itemsValidation)
                .Bind(_ => shippingValidation)
                .Bind(_ => billingValidation)
                .Map(_ => new ValidatedOrderRequest { Request = validRequest });
        });
    }
    
    private async Task<MlResult<OrderWithCustomer>> ValidateCustomerAsync(ValidatedOrderRequest validRequest)
    {
        var customer = await _customerService.GetByIdAsync(validRequest.Request.CustomerId);
        
        return await EnsureFp.NotNullAsync(customer, "Customer not found")
            .BindAsync(async validCustomer =>
            {
                // Validar estado del cliente
                var activeValidation = EnsureFp.That(
                    validCustomer,
                    validCustomer.IsActive,
                    "Customer account is not active");
                
                // Validar que no esté suspendido
                var suspensionValidation = EnsureFp.That(
                    validCustomer,
                    !validCustomer.IsSuspended,
                    "Customer account is suspended");
                
                // Validar límite de crédito
                var creditValidation = EnsureFp.That(
                    validCustomer,
                    validCustomer.CreditLimit > 0,
                    "Customer has no available credit");
                
                return activeValidation
                    .Bind(_ => suspensionValidation)
                    .Bind(_ => creditValidation)
                    .Map(_ => new OrderWithCustomer
                    {
                        Request = validRequest.Request,
                        Customer = validCustomer
                    });
            });
    }
    
    private async Task<MlResult<OrderWithItems>> ValidateOrderItemsAsync(OrderWithCustomer orderWithCustomer)
    {
        var validatedItems = new List<ValidatedOrderItem>();
        var validationErrors = new List<string>();
        
        foreach (var item in orderWithCustomer.Request.Items)
        {
            // Validar cada item individualmente
            var itemValidation = await ValidateSingleOrderItemAsync(item);
            
            if (itemValidation.IsValid)
            {
                validatedItems.Add(itemValidation.Value);
            }
            else
            {
                validationErrors.AddRange(itemValidation.ErrorsDetails.AllErrors);
            }
        }
        
        // Verificar que al menos un item sea válido
        return EnsureFp.NotEmpty(validatedItems, "No valid items found in order")
            .Bind(items => validationErrors.Any()
                ? MlResult<OrderWithItems>.Fail($"Some items failed validation: {string.Join("; ", validationErrors)}")
                : MlResult<OrderWithItems>.Valid(new OrderWithItems
                {
                    Request = orderWithCustomer.Request,
                    Customer = orderWithCustomer.Customer,
                    Items = items
                }));
    }
    
    private async Task<MlResult<ValidatedOrderItem>> ValidateSingleOrderItemAsync(OrderItem item)
    {
        // Validar ID del producto
        var productIdValidation = EnsureFp.That(
            item.ProductId,
            item.ProductId > 0,
            $"Invalid product ID: {item.ProductId}");
        
        // Validar cantidad
        var quantityValidation = EnsureFp.That(
            item.Quantity,
            item.Quantity > 0,
            $"Quantity must be positive for product {item.ProductId}");
        
        // Validar precio
        var priceValidation = EnsureFp.That(
            item.Price,
            item.Price > 0,
            $"Price must be positive for product {item.ProductId}");
        
        // Combinar validaciones básicas
        var basicValidation = productIdValidation
            .Bind(_ => quantityValidation)
            .Bind(_ => priceValidation);
        
        if (basicValidation.IsFailed)
            return basicValidation.ToMlResultFail<OrderItem, ValidatedOrderItem>();
        
        // Verificar disponibilidad en inventario
        var availability = await _inventoryService.CheckAvailabilityAsync(item.ProductId, item.Quantity);
        
        return EnsureFp.That(
            availability,
            availability.IsAvailable,
            $"Product {item.ProductId} not available in requested quantity")
        .Map(_ => new ValidatedOrderItem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Price,
            AvailabilityInfo = availability,
            ValidatedAt = DateTime.UtcNow
        });
    }
    
    private async Task<MlResult<OrderWithPayment>> ValidatePaymentInfoAsync(OrderWithItems orderWithItems)
    {
        var paymentInfo = orderWithItems.Request.PaymentInfo;
        
        // Validar información de pago
        var paymentValidation = EnsureFp.NotNull(paymentInfo, "Payment information is required");
        
        return await paymentValidation.BindAsync(async validPaymentInfo =>
        {
            // Validar método de pago
            var methodValidation = EnsureFp.NotNullEmptyOrWhitespace(
                validPaymentInfo.PaymentMethod,
                "Payment method is required");
            
            // Validar que el método sea soportado
            var supportedMethods = new[] { "CREDIT_CARD", "DEBIT_CARD", "BANK_TRANSFER", "DIGITAL_WALLET" };
            var methodSupportValidation = EnsureFp.That(
                validPaymentInfo.PaymentMethod,
                supportedMethods.Contains(validPaymentInfo.PaymentMethod),
                $"Payment method '{validPaymentInfo.PaymentMethod}' is not supported");
            
            // Para tarjetas, validar información adicional
            var cardValidation = validPaymentInfo.PaymentMethod.Contains("CARD")
                ? ValidateCreditCardInfo(validPaymentInfo)
                : MlResult<PaymentInfo>.Valid(validPaymentInfo);
            
            return methodValidation
                .Bind(_ => methodSupportValidation)
                .Bind(_ => cardValidation)
                .Map(_ => new OrderWithPayment
                {
                    Request = orderWithItems.Request,
                    Customer = orderWithItems.Customer,
                    Items = orderWithItems.Items,
                    PaymentInfo = validPaymentInfo
                });
        });
    }
    
    private MlResult<PaymentInfo> ValidateCreditCardInfo(PaymentInfo paymentInfo)
    {
        // Validar número de tarjeta
        var cardNumberValidation = EnsureFp.NotNullEmptyOrWhitespace(
            paymentInfo.CardNumber,
            "Card number is required for card payments");
        
        // Validar formato de número de tarjeta
        var cardFormatValidation = EnsureFp.That(
            paymentInfo.CardNumber,
            !string.IsNullOrEmpty(paymentInfo.CardNumber) && paymentInfo.CardNumber.Length >= 13,
            "Card number format is invalid");
        
        // Validar código de seguridad
        var securityCodeValidation = EnsureFp.NotNullEmptyOrWhitespace(
            paymentInfo.SecurityCode,
            "Security code is required for card payments");
        
        // Validar fecha de expiración
        var expirationValidation = EnsureFp.That(
            paymentInfo.ExpirationDate,
            paymentInfo.ExpirationDate > DateTime.Now,
            "Card has expired");
        
        return cardNumberValidation
            .Bind(_ => cardFormatValidation)
            .Bind(_ => securityCodeValidation)
            .Bind(_ => expirationValidation)
            .Map(_ => paymentInfo);
    }
    
    private async Task<MlResult<CompleteValidatedOrder>> ValidateBusinessRulesAsync(OrderWithPayment orderWithPayment)
    {
        var totalAmount = orderWithPayment.Items.Sum(i => i.Price * i.Quantity);
        
        // Validar límite mínimo de pedido
        var minimumOrderValidation = EnsureFp.That(
            totalAmount,
            totalAmount >= 10.00m, // Mínimo $10
            "Order total must be at least $10.00");
        
        // Validar límite de crédito del cliente
        var creditLimitValidation = EnsureFp.That(
            totalAmount,
            totalAmount <= orderWithPayment.Customer.CreditLimit,
            $"Order total ({totalAmount:C}) exceeds customer credit limit ({orderWithPayment.Customer.CreditLimit:C})");
        
        // Validar límites por tipo de cliente
        var customerTypeValidation = ValidateCustomerTypeLimits(orderWithPayment.Customer, totalAmount);
        
        // Validar restricciones geográficas
        var geographicValidation = ValidateGeographicRestrictions(
            orderWithPayment.Customer.Country,
            orderWithPayment.Request.ShippingAddress.Country);
        
        return minimumOrderValidation
            .Bind(_ => creditLimitValidation)
            .Bind(_ => customerTypeValidation)
            .Bind(_ => geographicValidation)
            .Map(_ => new CompleteValidatedOrder
            {
                Request = orderWithPayment.Request,
                Customer = orderWithPayment.Customer,
                Items = orderWithPayment.Items,
                PaymentInfo = orderWithPayment.PaymentInfo,
                TotalAmount = totalAmount,
                BusinessRulesValidatedAt = DateTime.UtcNow
            });
    }
    
    private MlResult<decimal> ValidateCustomerTypeLimits(Customer customer, decimal orderTotal)
    {
        var limits = customer.CustomerType switch
        {
            "BRONZE" => 1000m,
            "SILVER" => 5000m,
            "GOLD" => 25000m,
            "PLATINUM" => decimal.MaxValue,
            _ => 500m // Default limit
        };
        
        return EnsureFp.That(
            orderTotal,
            orderTotal <= limits,
            $"Order total ({orderTotal:C}) exceeds limit for {customer.CustomerType} customers ({limits:C})");
    }
    
    private MlResult<bool> ValidateGeographicRestrictions(string customerCountry, string shippingCountry)
    {
        // Validar que el país del cliente esté permitido
        var allowedCountries = new[] { "US", "CA", "MX", "GB", "FR", "DE", "ES", "IT" };
        var customerCountryValidation = EnsureFp.That(
            customerCountry,
            allowedCountries.Contains(customerCountry),
            $"Orders not allowed from country: {customerCountry}");
        
        // Validar que el envío esté permitido
        var shippingCountryValidation = EnsureFp.That(
            shippingCountry,
            allowedCountries.Contains(shippingCountry),
            $"Shipping not available to country: {shippingCountry}");
        
        return customerCountryValidation
            .Bind(_ => shippingCountryValidation)
            .Map(_ => true);
    }
}

// Clases de apoyo para el ejemplo
public class OrderRequest
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
    public PaymentInfo PaymentInfo { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class ValidatedOrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public object AvailabilityInfo { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuspended { get; set; }
    public decimal CreditLimit { get; set; }
    public string CustomerType { get; set; }
    public string Country { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
}

public class PaymentInfo
{
    public string PaymentMethod { get; set; }
    public string CardNumber { get; set; }
    public string SecurityCode { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class ValidatedOrderRequest
{
    public OrderRequest Request { get; set; }
}

public class OrderWithCustomer
{
    public OrderRequest Request { get; set; }
    public Customer Customer { get; set; }
}

public class OrderWithItems
{
    public OrderRequest Request { get; set; }
    public Customer Customer { get; set; }
    public List<ValidatedOrderItem> Items { get; set; }
}

public class OrderWithPayment
{
    public OrderRequest Request { get; set; }
    public Customer Customer { get; set; }
    public List<ValidatedOrderItem> Items { get; set; }
    public PaymentInfo PaymentInfo { get; set; }
}

public class CompleteValidatedOrder
{
    public OrderRequest Request { get; set; }
    public Customer Customer { get; set; }
    public List<ValidatedOrderItem> Items { get; set; }
    public PaymentInfo PaymentInfo { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime BusinessRulesValidatedAt { get; set; }
}

public class ValidatedOrder
{
    public Guid OrderId { get; set; }
    public OrderRequest Request { get; set; }
    public Customer Customer { get; set; }
    public List<ValidatedOrderItem> ValidatedItems { get; set; }
    public PaymentInfo PaymentInfo { get; set; }
    public DateTime ValidatedAt { get; set; }
    public Guid ValidationId { get; set; }
}
```

### Ejemplo 2: Sistema de Validación de Configuración

```csharp
public class ConfigurationValidationService
{
    public async Task<MlResult<ValidatedApplicationConfig>> ValidateApplicationConfigAsync(
        ApplicationConfig config)
    {
        return await ValidateBasicConfigStructureAsync(config)
            .BindAsync(async validConfig => await ValidateDatabaseConfigAsync(validConfig))
            .BindAsync(async configWithDb => await ValidateApiConfigAsync(configWithDb))
            .BindAsync(async configWithApi => await ValidateSecurityConfigAsync(configWithApi))
            .BindAsync(async configWithSecurity => await ValidateLoggingConfigAsync(configWithSecurity))
            .BindAsync(async completeConfig => await ValidateEnvironmentSpecificConfigAsync(completeConfig));
    }
    
    private async Task<MlResult<ApplicationConfig>> ValidateBasicConfigStructureAsync(ApplicationConfig config)
    {
        // Validar estructura básica
        var configValidation = EnsureFp.NotNull(config, "Configuration object is required");
        
        return await configValidation.BindAsync(async validConfig =>
        {
            // Validar nombre de aplicación
            var appNameValidation = EnsureFp.NotNullEmptyOrWhitespace(
                validConfig.ApplicationName,
                "Application name is required");
            
            // Validar versión
            var versionValidation = EnsureFp.NotNullEmptyOrWhitespace(
                validConfig.Version,
                "Application version is required");
            
            // Validar formato de versión
            var versionFormatValidation = EnsureFp.That(
                validConfig.Version,
                IsValidVersionFormat(validConfig.Version),
                "Version format must be in semantic versioning format (e.g., 1.2.3)");
            
            // Validar entorno
            var environmentValidation = EnsureFp.NotNullEmptyOrWhitespace(
                validConfig.Environment,
                "Environment is required");
            
            // Validar que el entorno sea válido
            var validEnvironments = new[] { "Development", "Testing", "Staging", "Production" };
            var environmentValueValidation = EnsureFp.That(
                validConfig.Environment,
                validEnvironments.Contains(validConfig.Environment),
                $"Environment must be one of: {string.Join(", ", validEnvironments)}");
            
            return appNameValidation
                .Bind(_ => versionValidation)
                .Bind(_ => versionFormatValidation)
                .Bind(_ => environmentValidation)
                .Bind(_ => environmentValueValidation)
                .Map(_ => validConfig);
        });
    }
    
    private async Task<MlResult<ApplicationConfig>> ValidateDatabaseConfigAsync(ApplicationConfig config)
    {
        var dbConfig = config.DatabaseConfig;
        
        // Validar configuración de base de datos
        var dbConfigValidation = EnsureFp.NotNull(dbConfig, "Database configuration is required");
        
        return await dbConfigValidation.BindAsync(async validDbConfig =>
        {
            // Validar cadena de conexión
            var connectionStringValidation = EnsureFp.NotNullEmptyOrWhitespace(
                validDbConfig.ConnectionString,
                "Database connection string is required");
            
            // Validar timeout
            var timeoutValidation = EnsureFp.That(
                validDbConfig.CommandTimeout,
                validDbConfig.CommandTimeout > 0 && validDbConfig.CommandTimeout <= 300,
                "Database command timeout must be between 1 and 300 seconds");
            
            // Validar pool size
            var poolSizeValidation = EnsureFp.That(
                validDbConfig.MaxPoolSize,
                validDbConfig.MaxPoolSize > 0 && validDbConfig.MaxPoolSize <= 1000,
                "Database max pool size must be between 1 and 1000");
            
            // Validar configuración de retry
            var retryConfigValidation = ValidateRetryConfig(validDbConfig.RetryConfig);
            
            // Validar configuración específica por entorno
            var environmentSpecificValidation = ValidateDatabaseEnvironmentConfig(config.Environment, validDbConfig);
            
            return connectionStringValidation
                .Bind(_ => timeoutValidation)
                .Bind(_ => poolSizeValidation)
                .Bind(_ => retryConfigValidation)
                .Bind(_ => environmentSpecificValidation)
                .Map(_ => config);
        });
    }
    
    private async Task<MlResult<ApplicationConfig>> ValidateApiConfigAsync(ApplicationConfig config)
    {
        var apiConfig = config.ApiConfig;
        
        var apiConfigValidation = EnsureFp.NotNull(apiConfig, "API configuration is required");
        
        return await apiConfigValidation.BindAsync(async validApiConfig =>
        {
            // Validar URL base
            var baseUrlValidation = EnsureFp.NotNullEmptyOrWhitespace(
                validApiConfig.BaseUrl,
                "API base URL is required");
            
            // Validar formato de URL
            var urlFormatValidation = EnsureFp.That(
                validApiConfig.BaseUrl,
                Uri.TryCreate(validApiConfig.BaseUrl, UriKind.Absolute, out _),
                "API base URL must be a valid absolute URL");
            
            // Validar puerto
            var portValidation = EnsureFp.That(
                validApiConfig.Port,
                validApiConfig.Port > 0 && validApiConfig.Port <= 65535,
                "API port must be between 1 and 65535");
            
            // Validar timeout
            var timeoutValidation = EnsureFp.That(
                validApiConfig.TimeoutSeconds,
                validApiConfig.TimeoutSeconds > 0 && validApiConfig.TimeoutSeconds <= 300,
                "API timeout must be between 1 and 300 seconds");
            
            // Validar configuración de CORS
            var corsValidation = ValidateCorsConfig(validApiConfig.CorsConfig);
            
            // Validar configuración de rate limiting
            var rateLimitValidation = ValidateRateLimitConfig(validApiConfig.RateLimitConfig);
            
            return baseUrlValidation
                .Bind(_ => urlFormatValidation)
                .Bind(_ => portValidation)
                .Bind(_ => timeoutValidation)
                .Bind(_ => corsValidation)
                .Bind(_ => rateLimitValidation)
                .Map(_ => config);
        });
    }
    
    private MlResult<RetryConfig> ValidateRetryConfig(RetryConfig retryConfig)
    {
        var retryValidation = EnsureFp.NotNull(retryConfig, "Retry configuration is required");
        
        return retryValidation.Bind(validRetryConfig =>
        {
            // Validar número máximo de reintentos
            var maxRetriesValidation = EnsureFp.That(
                validRetryConfig.MaxRetries,
                validRetryConfig.MaxRetries >= 0 && validRetryConfig.MaxRetries <= 10,
                "Max retries must be between 0 and 10");
            
            // Validar delay base
            var baseDelayValidation = EnsureFp.That(
                validRetryConfig.BaseDelayMilliseconds,
                validRetryConfig.BaseDelayMilliseconds > 0 && validRetryConfig.BaseDelayMilliseconds <= 60000,
                "Base delay must be between 1 and 60000 milliseconds");
            
            // Validar multiplicador de backoff
            var backoffMultiplierValidation = EnsureFp.That(
                validRetryConfig.BackoffMultiplier,
                validRetryConfig.BackoffMultiplier >= 1.0 && validRetryConfig.BackoffMultiplier <= 10.0,
                "Backoff multiplier must be between 1.0 and 10.0");
            
            return maxRetriesValidation
                .Bind(_ => baseDelayValidation)
                .Bind(_ => backoffMultiplierValidation)
                .Map(_ => validRetryConfig);
        });
    }
    
    private MlResult<CorsConfig> ValidateCorsConfig(CorsConfig corsConfig)
    {
        var corsValidation = EnsureFp.NotNull(corsConfig, "CORS configuration is required");
        
        return corsValidation.Bind(validCorsConfig =>
        {
            // Si CORS está habilitado, validar configuración
            if (!validCorsConfig.Enabled)
                return MlResult<CorsConfig>.Valid(validCorsConfig);
            
            // Validar orígenes permitidos
            var allowedOriginsValidation = EnsureFp.NotEmpty(
                validCorsConfig.AllowedOrigins,
                "Allowed origins must be specified when CORS is enabled");
            
            // Validar que los orígenes sean URLs válidas
            var originsFormatValidation = validCorsConfig.AllowedOrigins.All(origin =>
                origin == "*" || Uri.TryCreate(origin, UriKind.Absolute, out _))
                ? MlResult<bool>.Valid(true)
                : MlResult<bool>.Fail("All allowed origins must be valid URLs or '*'");
            
            // Validar métodos permitidos
            var allowedMethodsValidation = EnsureFp.NotEmpty(
                validCorsConfig.AllowedMethods,
                "Allowed methods must be specified when CORS is enabled");
            
            return allowedOriginsValidation
                .Bind(_ => originsFormatValidation)
                .Bind(_ => allowedMethodsValidation)
                .Map(_ => validCorsConfig);
        });
    }
    
    private MlResult<RateLimitConfig> ValidateRateLimitConfig(RateLimitConfig rateLimitConfig)
    {
        var rateLimitValidation = EnsureFp.NotNull(rateLimitConfig, "Rate limit configuration is required");
        
        return rateLimitValidation.Bind(validRateLimitConfig =>
        {
            if (!validRateLimitConfig.Enabled)
                return MlResult<RateLimitConfig>.Valid(validRateLimitConfig);
            
            // Validar límite de requests
            var requestLimitValidation = EnsureFp.That(
                validRateLimitConfig.RequestsPerMinute,
                validRateLimitConfig.RequestsPerMinute > 0 && validRateLimitConfig.RequestsPerMinute <= 10000,
                "Requests per minute must be between 1 and 10000");
            
            // Validar ventana de tiempo
            var windowSizeValidation = EnsureFp.That(
                validRateLimitConfig.WindowSizeMinutes,
                validRateLimitConfig.WindowSizeMinutes > 0 && validRateLimitConfig.WindowSizeMinutes <= 60,
                "Window size must be between 1 and 60 minutes");
            
            return requestLimitValidation
                .Bind(_ => windowSizeValidation)
                .Map(_ => validRateLimitConfig);
        });
    }
    
    private MlResult<DatabaseConfig> ValidateDatabaseEnvironmentConfig(string environment, DatabaseConfig dbConfig)
    {
        return environment switch
        {
            "Production" => ValidateProductionDatabaseConfig(dbConfig),
            "Staging" => ValidateStagingDatabaseConfig(dbConfig),
            _ => MlResult<DatabaseConfig>.Valid(dbConfig) // Development y Testing menos restrictivos
        };
    }
    
    private MlResult<DatabaseConfig> ValidateProductionDatabaseConfig(DatabaseConfig dbConfig)
    {
        // En producción, requerir configuraciones más estrictas
        var encryptionValidation = EnsureFp.That(
            dbConfig.ConnectionString,
            dbConfig.ConnectionString.Contains("Encrypt=True", StringComparison.OrdinalIgnoreCase),
            "Database encryption must be enabled in production");
        
        var poolSizeValidation = EnsureFp.That(
            dbConfig.MaxPoolSize,
            dbConfig.MaxPoolSize >= 10,
            "Production database pool size should be at least 10");
        
        var backupValidation = EnsureFp.That(
            dbConfig.BackupEnabled,
            dbConfig.BackupEnabled,
            "Database backups must be enabled in production");
        
        return encryptionValidation
            .Bind(_ => poolSizeValidation)
            .Bind(_ => backupValidation)
            .Map(_ => dbConfig);
    }
    
    private MlResult<DatabaseConfig> ValidateStagingDatabaseConfig(DatabaseConfig dbConfig)
    {
        // En staging, configuraciones moderadas
        var poolSizeValidation = EnsureFp.That(
            dbConfig.MaxPoolSize,
            dbConfig.MaxPoolSize >= 5,
            "Staging database pool size should be at least 5");
        
        return poolSizeValidation.Map(_ => dbConfig);
    }
    
    // Métodos auxiliares
    private bool IsValidVersionFormat(string version)
    {
        if (string.IsNullOrEmpty(version))
            return false;
        
        var parts = version.Split('.');
        return parts.Length == 3 && parts.All(part => int.TryParse(part, out _));
    }
}

// Clases de configuración para el ejemplo
public class ApplicationConfig
{
    public string ApplicationName { get; set; }
    public string Version { get; set; }
    public string Environment { get; set; }
    public DatabaseConfig DatabaseConfig { get; set; }
    public ApiConfig ApiConfig { get; set; }
    public SecurityConfig SecurityConfig { get; set; }
    public LoggingConfig LoggingConfig { get; set; }
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; }
    public int CommandTimeout { get; set; }
    public int MaxPoolSize { get; set; }
    public RetryConfig RetryConfig { get; set; }
    public bool BackupEnabled { get; set; }
}

public class ApiConfig
{
    public string BaseUrl { get; set; }
    public int Port { get; set; }
    public int TimeoutSeconds { get; set; }
    public CorsConfig CorsConfig { get; set; }
    public RateLimitConfig RateLimitConfig { get; set; }
}

public class RetryConfig
{
    public int MaxRetries { get; set; }
    public int BaseDelayMilliseconds { get; set; }
    public double BackoffMultiplier { get; set; }
}

public class CorsConfig
{
    public bool Enabled { get; set; }
    public List<string> AllowedOrigins { get; set; } = new();
    public List<string> AllowedMethods { get; set; } = new();
}

public class RateLimitConfig
{
    public bool Enabled { get; set; }
    public int RequestsPerMinute { get; set; }
    public int WindowSizeMinutes { get; set; }
}

public class SecurityConfig
{
    public string JwtSecret { get; set; }
    public int JwtExpirationMinutes { get; set; }
}

public class LoggingConfig
{
    public string LogLevel { get; set; }
    public string LogPath { get; set; }
}

public class ValidatedApplicationConfig
{
    public ApplicationConfig Config { get; set; }
    public DateTime ValidatedAt { get; set; }
    public Guid ValidationId { get; set; }
}
```

---

## Mejores Prácticas

### 1. Composición de Validaciones

```csharp
// ✅ Correcto: Componer validaciones de forma legible
public MlResult<User> ValidateUser(User user)
{
    return EnsureFp.NotNull(user, "User is required")
        .Bind(u => EnsureFp.NotNullEmptyOrWhitespace(u.Email, "Email is required"))
        .Bind(u => EnsureFp.That(u.Age, u.Age >= 18, "User must be 18 or older"))
        .Bind(u => EnsureFp.That(u.Country, IsValidCountry(u.Country), "Invalid country"));
}

// ✅ Correcto: Usar validaciones específicas cuando sea apropiado
public MlResult<OrderItems> ValidateOrderItems(List<OrderItem> items)
{
    return EnsureFp.NotEmpty(items, "Order must contain items")
        .Bind(validItems => ValidateEachItem(validItems));
}

// ❌ Incorrecto: Validaciones redundantes o inconsistentes
public MlResult<User> ValidateUserBad(User user)
{
    if (user == null) // Usar EnsureFp.NotNull en su lugar
        return MlResult<User>.Fail("User is null");
    
    return EnsureFp.That(user, user != null, "User is required"); // Redundante
}
```

### 2. Mensajes de Error Descriptivos

```csharp
// ✅ Correcto: Mensajes específicos y accionables
var validAge = EnsureFp.That(age, age >= 18 && age <= 120, 
    $"Age {age} is invalid. Must be between 18 and 120.");

var validEmail = EnsureFp.NotNullEmptyOrWhitespace(email, 
    "Email address is required for account creation.");

var validItems = EnsureFp.NotEmpty(orderItems, 
    "Order must contain at least one item. Please add items to your cart.");

// ❌ Incorrecto: Mensajes genéricos
var validAge = EnsureFp.That(age, age >= 18, "Invalid age");
var validEmail = EnsureFp.NotNull(email, "Error");
```

### 3. Uso de Errores Complejos

```csharp
// ✅ Correcto: Usar MlErrorsDetails para errores con contexto
public MlResult<PaymentInfo> ValidatePaymentWithContext(PaymentInfo payment, string orderId)
{
    var errorDetails = new MlErrorsDetails(
        new List<MlError> { new MlError("Payment validation failed") },
        new Dictionary<string, object>
        {
            { "OrderId", orderId },
            { "PaymentMethod", payment?.PaymentMethod },
            { "ValidationTimestamp", DateTime.UtcNow }
        });
    
    return EnsureFp.NotNull(payment, errorDetails);
}

// ✅ Correcto: Combinar múltiples validaciones con contexto
public async Task<MlResult<CompleteOrder>> ValidateCompleteOrderAsync(Order order)
{
    var validationContext = new Dictionary<string, object>
    {
        { "OrderId", order?.Id },
        { "ValidatedBy", "OrderValidationService" },
        { "ValidationStartTime", DateTime.UtcNow }
    };
    
    var baseValidation = EnsureFp.NotNull(order, 
        new MlErrorsDetails(
            new List<MlError> { new MlError("Order object is required") },
            validationContext));
    
    return await baseValidation.BindAsync(async validOrder => 
        await ValidateOrderDetailsWithContextAsync(validOrder, validationContext));
}
```

### 4. Validaciones Async Apropiadas

```csharp
// ✅ Correcto: Usar async solo cuando sea necesario
public async Task<MlResult<User>> ValidateUserAsync(int userId)
{
    // Validación síncrona primero
    var userIdValidation = EnsureFp.That(userId, userId > 0, "User ID must be positive");
    
    if (userIdValidation.IsFailed)
        return userIdValidation.ToMlResultFail<int, User>();
    
    // Luego operaciones async si son necesarias
    var user = await GetUserFromDatabaseAsync(userId);
    return await EnsureFp.NotNullAsync(user, $"User {userId} not found");
}

// ❌ Incorrecto: Async innecesario
public async Task<MlResult<string>> ValidateStringAsync(string input)
{
    return await EnsureFp.NotNullEmptyOrWhitespaceAsync(input, "String required");
    // Mejor usar la versión síncrona: EnsureFp.NotNullEmptyOrWhitespace
}
```

---

## Comparación con Assert y Guard

### Tabla Comparativa

| Método | Comportamiento ante Falla | Retorno | Uso Típico |
|--------|-------------------------|---------|------------|
| `EnsureFp.That` | Retorna `MlResult.Fail` | `MlResult<T>` | Validaciones funcionales |
| `Assert.That` | Lanza excepción | `void` | Pruebas unitarias |
| `Guard.Against` | Lanza excepción | `void` | Validación de parámetros |
| `Contract.Requires` | Lanza excepción | `void` | Contratos de código |

### Ejemplo Comparativo

```csharp
// EnsureFp: Flujo funcional sin excepciones
var result = EnsureFp.NotNull(user, "User required")
    .Bind(u => ProcessUser(u))
    .Map(processed => processed.ToDto());

// Guard: Validación imperativa con excepciones
Guard.Against.Null(user, nameof(user));
var processed = ProcessUser(user); // Puede fallar sin control
var dto = processed.ToDto();

// Assert: Solo para pruebas
Assert.That(user, Is.Not.Null); // Solo en tests

// Uso combinado apropiado
public MlResult<ProcessedUser> ProcessUserSafely(User user)
{
    // Usar EnsureFp para validación funcional
    return EnsureFp.NotNull(user, "User is required")
        .Bind(validUser => 
        {
            // Guard para validaciones internas críticas
            Guard.Against.NullOrEmpty(validUser.Email, nameof(validUser.Email));
            return ProcessUserInternal(validUser);
        });
}
```

---

## Resumen

La clase `EnsureFp` proporciona **validaciones funcionales sin excepciones**:

- **`NotNull`**: Validación de valores null con preservación de tipo
- **`NotEmpty`**: Validación de colecciones vacías
- **`NotNullEmptyOrWhitespace`**: Validación completa de strings
- **`That`**: Validación genérica para cualquier condición boolean

**Casos de uso ideales**:
- **Validaciones de entrada** en métodos públicos
- **Precondiciones funcionales** que no deben lanzar excepciones
- **Composición de validaciones** en cadenas funcionales
- **Validación de configuraciones** y datos estructurados

**Ventajas principales**:
- **Sin efectos secundarios** (no lanza excepciones)
- **Preservación de tipos** y valores válidos
- **Composabilidad** con otros métodos MlResult
- **Mensajes de error flexibles** con soporte para contexto adicional