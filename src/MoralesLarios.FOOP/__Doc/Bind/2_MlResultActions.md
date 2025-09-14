# MlResultActions - Operaciones de Enriquecimiento y Composición

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos de Enriquecimiento de Errores](#métodos-de-enriquecimiento-de-errores)
4. [Métodos de Completado con Datos](#métodos-de-completado-con-datos)
5. [Métodos de Acceso Seguro](#métodos-de-acceso-seguro)
6. [Métodos de Composición de Resultados](#métodos-de-composición-de-resultados)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActions` proporciona operaciones avanzadas para trabajar con `MlResult<T>`, incluyendo enriquecimiento de errores, completado de datos, acceso seguro a valores y composición de múltiples resultados. Estas operaciones permiten crear pipelines funcionales más robustos y expresivos.

### Propósito Principal

- **Enriquecimiento de Errores**: Añadir información contextual a errores
- **Completado de Datos**: Combinar resultados con información adicional
- **Acceso Seguro**: Extraer valores garantizando el estado del resultado
- **Composición**: Combinar múltiples resultados en estructuras complejas

---

## Análisis de la Clase

### Estructura y Filosofía

```csharp
public static class MlResultActions
{
    // Métodos de extensión para operaciones avanzadas
    // Soporte completo para operaciones síncronas y asíncronas
    // Enfoque en composición y enriquecimiento de información
}
```

### Características Principales

1. **Métodos de Extensión**: Operaciones fluidas sobre `MlResult<T>`
2. **Soporte Asíncrono**: Versiones `Async` para todas las operaciones
3. **Enriquecimiento Contextual**: Añadir información a errores
4. **Composición Segura**: Combinar resultados preservando errores

---

## Métodos de Enriquecimiento de Errores

### `AddMlErrorDetailIfFail<T>()`

**Propósito**: Añade un detalle clave-valor a los errores si el resultado es fallido

```csharp
public static MlResult<T> AddMlErrorDetailIfFail<T>(this MlResult<T> source, 
                                                    string errorKey, 
                                                    object errorValue)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `errorKey`: Clave del detalle a añadir
- `errorValue`: Valor del detalle a añadir

**Comportamiento**:
- Si el resultado es exitoso: Retorna el resultado sin modificaciones
- Si el resultado es fallido: Añade el detalle a los errores y retorna el resultado modificado

### `AddValueDetailIfFail<T>()`

**Propósito**: Añade un valor específico bajo la clave `VALUE_KEY` si el resultado es fallido

```csharp
public static MlResult<T> AddValueDetailIfFail<T>(this MlResult<T> source, 
                                                  object errorValue)
```

**Comportamiento**:
- Utiliza internamente `AddMlErrorDetailIfFail` con `VALUE_KEY` como clave
- Simplifica la adición de valores contextuales sin especificar clave

### Versiones Asíncronas

- `AddMlErrorDetailIfFailAsync<T>()`: Versión asíncrona para `MlResult<T>`
- `AddMlErrorDetailIfFailAsync<T>()`: Versión asíncrona para `Task<MlResult<T>>`
- `AddValueDetailIfFailAsync<T>()`: Versiones asíncronas para valor con clave por defecto

---

## Métodos de Completado con Datos

### `CompleteWithDataValueIfValid<T, TReturn>()`

**Propósito**: Transforma el valor si el resultado es exitoso, usando una función de completado

```csharp
public static MlResult<TReturn> CompleteWithDataValueIfValid<T, TReturn>(this MlResult<T> source,
                                                                         Func<T, TReturn> completeFunc)
```

**Comportamiento**:
- Si el resultado es exitoso: Aplica `completeFunc` al valor y retorna nuevo resultado
- Si el resultado es fallido: Propaga los errores con el nuevo tipo

### `CompleteWithDetailsValueIfFail<T, TValue>()`

**Propósito**: Añade un valor a los detalles del error si el resultado es fallido

```csharp
public static MlResult<T> CompleteWithDetailsValueIfFail<T, TValue>(this MlResult<T> source, 
                                                                    TValue value)
```

**Comportamiento**:
- Si el resultado es exitoso: Retorna el resultado sin modificaciones
- Si el resultado es fallido: Añade el valor a los detalles del error

### `CompleteWithDataValue<T, TValue, TReturn>()`

**Propósito**: Combina ambas operaciones anteriores - completa si es válido, enriquece si es fallido

```csharp
public static MlResult<TReturn> CompleteWithDataValue<T, TValue, TReturn>(this MlResult<T> source, 
                                                                          TValue value,
                                                                          Func<T, TReturn> completeFunc)
```

**Comportamiento**:
- Si el resultado es exitoso: Aplica `completeFunc` y retorna el nuevo resultado
- Si el resultado es fallido: Añade `value` a los detalles y propaga el error

---

## Métodos de Acceso Seguro

### `SecureValidValue<T>()`

**Propósito**: Extrae el valor de un resultado exitoso, lanza excepción si es fallido

```csharp
public static T SecureValidValue<T>(this MlResult<T> source, 
                                   string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state")
```

**Comportamiento**:
- Si el resultado es exitoso: Retorna el valor
- Si el resultado es fallido: Lanza `InvalidProgramException`

**⚠️ Advertencia**: Solo usar cuando se esté seguro de que el resultado es exitoso

### `SecureFailErrorsDetails<T>()`

**Propósito**: Extrae los detalles de error de un resultado fallido, lanza excepción si es exitoso

```csharp
public static MlErrorsDetails SecureFailErrorsDetails<T>(this MlResult<T> source, 
                                                        string exceptionMessage = "Cannot obtain the MlErrorsDetails from MlResult in Valid state")
```

**Comportamiento**:
- Si el resultado es fallido: Retorna los detalles del error
- Si el resultado es exitoso: Lanza `InvalidProgramException`

### Versiones Asíncronas

- `SecureValidValueAsync<T>()`: Versiones asíncronas para ambos métodos
- `SecureFailErrorsDetailsAsync<T>()`: Soporte completo para `Task<MlResult<T>>`

---

## Métodos de Composición de Resultados

### `CreateCompleteMlResult<T1, T2>()` - Dos Resultados

**Propósito**: Combina dos `MlResult<T>` en un `MlResult<(T1, T2)>`

```csharp
public static MlResult<(T1, T2)> CreateCompleteMlResult<T1, T2>(this MlResult<T1> source1,
                                                                MlResult<T2> source2)
```

**Comportamiento**:
- Si ambos son exitosos: Retorna tupla con ambos valores
- Si cualquiera es fallido: Fusiona los errores y retorna resultado fallido

### `CreateCompleteMlResult<T1, T2, T3>()` - Tres Resultados

**Propósito**: Combina tres `MlResult<T>` en un `MlResult<(T1, T2, T3)>`

```csharp
public static MlResult<(T1, T2, T3)> CreateCompleteMlResult<T1, T2, T3>(this MlResult<T1> source1,
                                                                        MlResult<T2> source2,
                                                                        MlResult<T3> source3)
```

### `CreateCompleteMlResult<T1, T2>()` - Objeto + Resultado

**Propósito**: Combina un valor directo con un `MlResult<T>`

```csharp
public static MlResult<(T1, T2)> CreateCompleteMlResult<T1, T2>(this T1 source1,
                                                                MlResult<T2> source2)
```

**Comportamiento**:
- Si `source2` es exitoso: Retorna tupla con ambos valores
- Si `source2` es fallido: Propaga el error

### Versiones Asíncronas Completas

Todas las operaciones de composición tienen versiones asíncronas que soportan:
- `MlResult<T>` + `Task<MlResult<T>>`
- `Task<MlResult<T>>` + `MlResult<T>`
- `Task<MlResult<T>>` + `Task<MlResult<T>>`

---

## Ejemplos Prácticos

### Ejemplo 1: Enriquecimiento de Errores

```csharp
public class UserService
{
    public async Task<MlResult<User>> GetUserWithContextAsync(int userId, string requestId)
    {
        return await GetUserFromDatabaseAsync(userId)
            .AddMlErrorDetailIfFailAsync("UserId", userId)
            .AddMlErrorDetailIfFailAsync("RequestId", requestId)
            .AddMlErrorDetailIfFailAsync("Timestamp", DateTime.UtcNow)
            .AddMlErrorDetailIfFailAsync("Source", "UserService.GetUserWithContext");
    }
    
    public async Task<MlResult<UserProfile>> GetUserProfileAsync(int userId)
    {
        var user = await GetUserAsync(userId);
        
        return user
            .AddValueDetailIfFail(new { UserId = userId, Context = "ProfileRetrieval" })
            .BindAsync(async validUser => await BuildUserProfileAsync(validUser));
    }
    
    private async Task<MlResult<User>> GetUserAsync(int userId)
    {
        // Simulación de obtención de usuario
        await Task.Delay(100);
        
        return userId > 0 
            ? MlResult<User>.Valid(new User { Id = userId, Name = $"User{userId}" })
            : MlResult<User>.Fail("Invalid user ID");
    }
    
    private async Task<MlResult<UserProfile>> BuildUserProfileAsync(User user)
    {
        await Task.Delay(50);
        return MlResult<UserProfile>.Valid(new UserProfile 
        { 
            User = user, 
            LastLoginDate = DateTime.Now.AddDays(-1) 
        });
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class UserProfile
{
    public User User { get; set; }
    public DateTime LastLoginDate { get; set; }
}
```

### Ejemplo 2: Completado con Datos

```csharp
public class OrderProcessor
{
    public async Task<MlResult<OrderSummary>> ProcessOrderAsync(OrderRequest request)
    {
        var validationContext = new { RequestId = Guid.NewGuid(), Timestamp = DateTime.UtcNow };
        
        return await ValidateOrderRequestAsync(request)
            .CompleteWithDetailsValueIfFailAsync(validationContext)
            .CompleteWithDataValueIfValidAsync(validatedRequest => 
                new OrderSummary 
                { 
                    OrderId = Guid.NewGuid(),
                    Request = validatedRequest,
                    ProcessedAt = DateTime.UtcNow 
                });
    }
    
    public async Task<MlResult<EnrichedOrder>> EnrichOrderAsync(Order order, Customer customer)
    {
        var enrichmentData = new { CustomerId = customer.Id, OrderValue = order.TotalAmount };
        
        return await ProcessOrderEnrichmentAsync(order, customer)
            .CompleteWithDataValueAsync(
                enrichmentData,
                async processedOrder => await CreateEnrichedOrderAsync(processedOrder, customer)
            );
    }
    
    private async Task<MlResult<OrderRequest>> ValidateOrderRequestAsync(OrderRequest request)
    {
        await Task.Delay(50);
        
        if (request == null)
            return MlResult<OrderRequest>.Fail("Order request cannot be null");
            
        if (request.Items?.Any() != true)
            return MlResult<OrderRequest>.Fail("Order must contain items");
            
        return MlResult<OrderRequest>.Valid(request);
    }
    
    private async Task<MlResult<Order>> ProcessOrderEnrichmentAsync(Order order, Customer customer)
    {
        await Task.Delay(100);
        
        if (order.TotalAmount > customer.CreditLimit)
            return MlResult<Order>.Fail("Order exceeds customer credit limit");
            
        return MlResult<Order>.Valid(order);
    }
    
    private async Task<EnrichedOrder> CreateEnrichedOrderAsync(Order order, Customer customer)
    {
        await Task.Delay(25);
        return new EnrichedOrder 
        { 
            Order = order, 
            Customer = customer, 
            EnrichmentDate = DateTime.UtcNow 
        };
    }
}

public class OrderRequest
{
    public List<OrderItem> Items { get; set; }
    public int CustomerId { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public string Id { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal CreditLimit { get; set; }
}

public class OrderSummary
{
    public Guid OrderId { get; set; }
    public OrderRequest Request { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class EnrichedOrder
{
    public Order Order { get; set; }
    public Customer Customer { get; set; }
    public DateTime EnrichmentDate { get; set; }
}
```

### Ejemplo 3: Acceso Seguro a Valores

```csharp
public class ConfigurationService
{
    private readonly Dictionary<string, MlResult<string>> _configurations = new();
    
    public void InitializeConfigurations()
    {
        _configurations["DatabaseConnectionString"] = MlResult<string>.Valid("Server=localhost;Database=MyDb");
        _configurations["ApiKey"] = MlResult<string>.Valid("abc123def456");
        _configurations["InvalidConfig"] = MlResult<string>.Fail("Configuration not found");
    }
    
    // Uso seguro cuando sabemos que la configuración existe
    public string GetRequiredConfiguration(string key)
    {
        var configResult = _configurations.GetValueOrDefault(key, MlResult<string>.Fail("Key not found"));
        
        // Solo usar SecureValidValue cuando estemos seguros del resultado
        return configResult.SecureValidValue($"Required configuration '{key}' is not available");
    }
    
    // Manejo de errores de configuración
    public void LogConfigurationErrors()
    {
        foreach (var config in _configurations.Where(c => c.Value.IsFail))
        {
            var errorDetails = config.Value.SecureFailErrorsDetails(
                $"Expected configuration '{config.Key}' to be in fail state");
                
            Console.WriteLine($"Configuration error for '{config.Key}': {errorDetails.GetMessage()}");
        }
    }
    
    // Uso asíncrono seguro
    public async Task<string> GetRequiredConfigurationAsync(string key)
    {
        var configResult = await LoadConfigurationAsync(key);
        
        return await configResult.SecureValidValueAsync(
            $"Failed to load required configuration '{key}'");
    }
    
    private async Task<MlResult<string>> LoadConfigurationAsync(string key)
    {
        await Task.Delay(10); // Simulación de carga
        return _configurations.GetValueOrDefault(key, MlResult<string>.Fail("Configuration not found"));
    }
}
```

### Ejemplo 4: Composición de Resultados

```csharp
public class DataAggregationService
{
    public async Task<MlResult<AggregatedData>> AggregateUserDataAsync(int userId)
    {
        // Cargar datos de diferentes fuentes en paralelo
        var userTask = GetUserDataAsync(userId);
        var preferencesTask = GetUserPreferencesAsync(userId);
        var activityTask = GetUserActivityAsync(userId);
        
        // Esperar a que todas las tareas terminen
        await Task.WhenAll(userTask, preferencesTask, activityTask);
        
        // Componer los resultados
        var userResult = await userTask;
        var preferencesResult = await preferencesTask;
        var activityResult = await activityTask;
        
        return userResult
            .CreateCompleteMlResult(preferencesResult, activityResult)
            .CompleteWithDataValueIfValid(tuple =>
            {
                var (user, preferences, activity) = tuple;
                return new AggregatedData
                {
                    User = user,
                    Preferences = preferences,
                    Activity = activity,
                    AggregatedAt = DateTime.UtcNow
                };
            });
    }
    
    public async Task<MlResult<UserReport>> GenerateUserReportAsync(int userId, DateTime reportDate)
    {
        var userData = await GetUserDataAsync(userId);
        var reportMetadata = new ReportMetadata { GeneratedAt = DateTime.UtcNow, ReportDate = reportDate };
        
        // Combinar valor directo con resultado
        return await reportMetadata
            .CreateCompleteMlResultAsync(userData)
            .CompleteWithDataValueIfValidAsync(tuple =>
            {
                var (metadata, user) = tuple;
                return Task.FromResult(new UserReport
                {
                    User = user,
                    Metadata = metadata,
                    Summary = $"Report for {user.Name} generated on {metadata.GeneratedAt}"
                });
            });
    }
    
    public async Task<MlResult<ValidationResult>> ValidateMultipleInputsAsync(
        string email, 
        int age, 
        string phoneNumber)
    {
        var emailValidation = ValidateEmailAsync(email);
        var ageValidation = ValidateAge(age);
        var phoneValidation = ValidatePhoneNumberAsync(phoneNumber);
        
        // Componer todas las validaciones
        return await emailValidation
            .CreateCompleteMlResultAsync(ageValidation, phoneValidation)
            .CompleteWithDataValueIfValidAsync(tuple =>
            {
                var (emailResult, ageResult, phoneResult) = tuple;
                return Task.FromResult(new ValidationResult
                {
                    IsValid = true,
                    ValidatedAt = DateTime.UtcNow,
                    Fields = new Dictionary<string, bool>
                    {
                        ["Email"] = emailResult,
                        ["Age"] = ageResult,
                        ["Phone"] = phoneResult
                    }
                });
            });
    }
    
    private async Task<MlResult<UserData>> GetUserDataAsync(int userId)
    {
        await Task.Delay(100);
        return userId > 0 
            ? MlResult<UserData>.Valid(new UserData { Id = userId, Name = $"User{userId}" })
            : MlResult<UserData>.Fail("Invalid user ID");
    }
    
    private async Task<MlResult<UserPreferences>> GetUserPreferencesAsync(int userId)
    {
        await Task.Delay(80);
        return MlResult<UserPreferences>.Valid(new UserPreferences { UserId = userId, Theme = "Dark" });
    }
    
    private async Task<MlResult<UserActivity>> GetUserActivityAsync(int userId)
    {
        await Task.Delay(120);
        return MlResult<UserActivity>.Valid(new UserActivity { UserId = userId, LastLoginDate = DateTime.Now.AddDays(-1) });
    }
    
    private async Task<MlResult<bool>> ValidateEmailAsync(string email)
    {
        await Task.Delay(50);
        return (!string.IsNullOrEmpty(email) && email.Contains("@"))
            ? MlResult<bool>.Valid(true)
            : MlResult<bool>.Fail("Invalid email format");
    }
    
    private MlResult<bool> ValidateAge(int age)
    {
        return (age >= 18 && age <= 120)
            ? MlResult<bool>.Valid(true)
            : MlResult<bool>.Fail("Age must be between 18 and 120");
    }
    
    private async Task<MlResult<bool>> ValidatePhoneNumberAsync(string phoneNumber)
    {
        await Task.Delay(30);
        return (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length >= 10)
            ? MlResult<bool>.Valid(true)
            : MlResult<bool>.Fail("Phone number must be at least 10 digits");
    }
}

// Clases de apoyo para los ejemplos
public class UserData
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class UserPreferences
{
    public int UserId { get; set; }
    public string Theme { get; set; }
}

public class UserActivity
{
    public int UserId { get; set; }
    public DateTime LastLoginDate { get; set; }
}

public class AggregatedData
{
    public UserData User { get; set; }
    public UserPreferences Preferences { get; set; }
    public UserActivity Activity { get; set; }
    public DateTime AggregatedAt { get; set; }
}

public class ReportMetadata
{
    public DateTime GeneratedAt { get; set; }
    public DateTime ReportDate { get; set; }
}

public class UserReport
{
    public UserData User { get; set; }
    public ReportMetadata Metadata { get; set; }
    public string Summary { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public DateTime ValidatedAt { get; set; }
    public Dictionary<string, bool> Fields { get; set; }
}
```

---

## Mejores Prácticas

### 1. Enriquecimiento de Errores

```csharp
// ✅ Correcto: Añadir contexto relevante a los errores
var result = await ProcessDataAsync(data)
    .AddMlErrorDetailIfFailAsync("UserId", userId)
    .AddMlErrorDetailIfFailAsync("OperationId", operationId)
    .AddMlErrorDetailIfFailAsync("Timestamp", DateTime.UtcNow);

// ❌ Incorrecto: Añadir información innecesaria o sensible
var result = await ProcessDataAsync(data)
    .AddMlErrorDetailIfFailAsync("Password", user.Password) // ¡Nunca!
    .AddMlErrorDetailIfFailAsync("RandomData", Guid.NewGuid()); // Innecesario
```

### 2. Uso de Acceso Seguro

```csharp
// ✅ Correcto: Usar SecureValidValue solo cuando esté garantizado el éxito
public string GetConfigurationValue(string key)
{
    var result = LoadConfiguration(key);
    
    // Solo si sabemos que la configuración debe existir
    if (IsRequiredConfiguration(key))
    {
        return result.SecureValidValue($"Required configuration '{key}' is missing");
    }
    
    // Para configuraciones opcionales, usar Match
    return result.Match(
        valid: value => value,
        fail: _ => GetDefaultValue(key)
    );
}

// ❌ Incorrecto: Usar SecureValidValue sin estar seguro del resultado
public string GetUserName(int userId)
{
    var userResult = GetUser(userId); // Puede fallar
    return userResult.SecureValidValue(); // ¡Puede lanzar excepción!
}
```

### 3. Composición de Resultados

```csharp
// ✅ Correcto: Usar CreateCompleteMlResult para validaciones que deben pasar todas
var validationResult = emailValidation
    .CreateCompleteMlResult(ageValidation, phoneValidation);

// ✅ Correcto: Combinar valor directo con resultado cuando sea apropiado
var reportResult = reportMetadata
    .CreateCompleteMlResult(userData);

// ❌ Incorrecto: Usar composición cuando solo necesitas una validación
var result = validation1.CreateCompleteMlResult(validation2); // Si validation1 es suficiente
```

### 4. Completado con Datos

```csharp
// ✅ Correcto: Usar CompleteWithDataValue para enriquecer en ambas ramas
var result = await ProcessDataAsync(input)
    .CompleteWithDataValueAsync(
        value: new { Context = "DataProcessing", UserId = userId },
        completeFuncAsync: async processedData => await FinalizeDataAsync(processedData)
    );

// ✅ Correcto: Usar CompleteWithDataValueIfValid solo para transformación en éxito
var result = ValidateInput(input)
    .CompleteWithDataValueIfValid(validInput => new ProcessedInput(validInput));

// ❌ Incorrecto: Usar CompleteWithDataValueIfValid cuando necesitas enriquecer errores también
```

### 5. Manejo de Operaciones Asíncronas

```csharp
// ✅ Correcto: Usar las versiones async apropiadas
var result = await GetDataAsync()
    .AddMlErrorDetailIfFailAsync("Context", context)
    .CompleteWithDataValueIfValidAsync(data => TransformDataAsync(data));

// ✅ Correcto: Composición asíncrona
var result = await GetUserAsync(userId)
    .CreateCompleteMlResultAsync(GetPreferencesAsync(userId));

// ❌ Incorrecto: Mezclar sync y async incorrectamente
var result = GetDataAsync() // Task<MlResult<T>>
    .AddMlErrorDetailIfFail("Context", context); // ¡Error! Necesita Async
```

---

## Consideraciones de Rendimiento

### Enriquecimiento de Errores

- Los métodos de enriquecimiento solo ejecutan lógica si el resultado es fallido
- El costo es mínimo para resultados exitosos
- Considera el tamaño de los objetos añadidos como valores

### Composición de Resultados

- `CreateCompleteMlResult` evalúa todos los resultados antes de componer
- Para operaciones costosas, considera evaluar condiciones tempranas
- Las tuplas se optimizan automáticamente por el compilador

### Acceso Seguro

- `SecureValidValue` y `SecureFailErrorsDetails` son operaciones O(1)
- El costo de las excepciones es alto, úsalos solo cuando sea seguro
- Prefiere `Match` para casos donde el estado es incierto

---

## Resumen

La clase `MlResultActions` proporciona operaciones avanzadas esenciales para:

- **Enriquecimiento Contextual**: `AddMlErrorDetailIfFail`, `AddValueDetailIfFail`
- **Completado Inteligente**: `CompleteWithDataValue`, `CompleteWithDataValueIfValid`, `CompleteWithDetailsValueIfFail`
- **Acceso Garantizado**: `SecureValidValue`, `SecureFailErrorsDetails`
- **Composición Robusta**: `CreateCompleteMlResult` para 2 y 3 resultados, combinaciones con valores directos

Estas operaciones permiten crear pipelines funcionales expresivos y robustos, manteniendo la seguridad de tipos y la composabilidad que caracterizan a la