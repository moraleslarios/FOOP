# MlResultActionsMapEnsure - Operaciones de Validación Condicional

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos MapEnsure Básicos](#métodos-mapensure-básicos)
4. [Constructores de Errores](#constructores-de-errores)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Comparación con Other Patterns](#comparación-con-otros-patrones)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

MapEnsure es una extensión de `MlResult<T>` que permite realizar validaciones condicionales sobre valores exitosos, transformándolos en errores si no cumplen con ciertas condiciones. A diferencia de `Map` o `Bind`, que transforman o encadenan valores, `MapEnsure` verifica condiciones y mantiene el tipo original.

La clase `MlResultActionsMapEnsure` implementa operaciones de **validación condicional** que convierten un `MlResult<T>` exitoso en fallido si no cumple con una condición específica. Es una especialización del patrón de validación que mantiene el valor original si la condición se cumple, o genera un error detallado si no se cumple.

### Propósito Principal

- **Validación Post-Transformación**: Verificar condiciones después de obtener un valor válido
- **Guard Clauses Funcionales**: Implementar verificaciones de precondiciones de forma funcional
- **Filtrado con Error**: Similar a `Where` en LINQ, pero generando errores en lugar de filtrar
- **Preservación de Tipo**: El tipo permanece inalterado, solo cambia el estado de éxito/fallo

### Filosofía de Diseño

```
Valor Válido + Condición Verdadera  → Valor Válido (sin cambios)
Valor Válido + Condición Falsa      → Error (con mensaje personalizado)
Valor Inválido                      → Error (propagación sin evaluación)
```

---

## Análisis de la Clase

### Patrón de Funcionamiento

`MapEnsure` implementa una validación **assertion-style** donde:

1. Si el `MlResult<T>` es fallido, propaga el error sin evaluación
2. Si el `MlResult<T>` es exitoso, evalúa la condición
3. Si la condición es verdadera, retorna el valor original
4. Si la condición es falsa, convierte a error con mensaje personalizado

### Diferencias con Otros Patrones

```csharp
// MapEnsure: Validación con preservación de tipo
MlResult<T> → (T → bool) → MlResult<T>

// Map: Transformación de valor
MlResult<T> → (T → U) → MlResult<U>

// Bind: Encadenamiento de operaciones
MlResult<T> → (T → MlResult<U>) → MlResult<U>
```

---

## Métodos MapEnsure Básicos

### `MapEnsure<T>()` - Error Details Estático

**Propósito**: Valida una condición con un error predefinido

```csharp
public static MlResult<T> MapEnsure<T>(this MlResult<T> source,
                                       Func<T, bool> ensureFunc,
                                       MlErrorsDetails errorDetailsResult)
```

**Parámetros**:
- `source`: El resultado a validar
- `ensureFunc`: Función que retorna `true` si la condición se cumple
- `errorDetailsResult`: Error a retornar si la condición falla

**Comportamiento**:
- Si `source` es fallido: Propaga el error original
- Si `source` es exitoso y `ensureFunc(value)` es `true`: Retorna el valor
- Si `source` es exitoso y `ensureFunc(value)` es `false`: Retorna el error especificado

### `MapEnsure<T>()` - Constructor de Error Dinámico

**Propósito**: Valida con un constructor de error que recibe el valor

```csharp
public static MlResult<T> MapEnsure<T>(this MlResult<T> source,
                                       Func<T, bool> ensureFunc,
                                       Func<T, MlErrorsDetails> errorDetailsResultBuilder)
```

**Comportamiento**: Permite construir errores dinámicos basados en el valor que falló la validación

### `MapEnsure<T>()` - Mensaje de Error Simple

**Propósito**: Valida con un mensaje de error simple

```csharp
public static MlResult<T> MapEnsure<T>(this MlResult<T> source,
                                       Func<T, bool> ensureFunc,
                                       string errorMessageResult)
```

**Comportamiento**: Convierte automáticamente el string a `MlErrorsDetails`

### `MapEnsure<T>()` - Constructor de Mensaje Dinámico

**Propósito**: Valida con un constructor de mensaje que recibe el valor

```csharp
public static MlResult<T> MapEnsure<T>(this MlResult<T> source,
                                       Func<T, bool> ensureFunc,
                                       Func<T, string> errorMessageResultBuilder)
```

**Comportamiento**: Permite crear mensajes de error específicos basados en el valor

---

## Constructores de Errores

### Jerarquía de Flexibilidad

1. **Error Details Estático**: Máximo rendimiento, error fijo
2. **Mensaje Estático**: Balance entre simplicidad y rendimiento  
3. **Constructor de Mensaje**: Flexibilidad media, mensajes dinámicos
4. **Constructor de Error Details**: Máxima flexibilidad, control total del error

### Ejemplos de Constructores

```csharp
// Error estático
result.MapEnsure(x => x > 0, "Value must be positive");

// Mensaje dinámico
result.MapEnsure(x => x > 0, x => $"Value {x} must be positive");

// Error details dinámico
result.MapEnsure(x => x > 0, x => new MlErrorsDetails
{
    ErrorMessage = $"Invalid value: {x}",
    ErrorCode = "NEGATIVE_VALUE",
    AdditionalInfo = new { ActualValue = x, MinimumRequired = 1 }
});
```

---

## Variantes Asíncronas

### Conversión a Asíncrono

Todas las variantes de `MapEnsure` tienen su correspondiente `MapEnsureAsync` que:

1. **Conversión Simple**: Envuelve el resultado sincrónico en `Task`
2. **Desde Fuente Asíncrona**: Opera sobre `Task<MlResult<T>>`

### Matriz de Combinaciones Asíncronas

| Fuente | Constructor de Error | Método |
|--------|---------------------|---------|
| `MlResult<T>` | `MlErrorsDetails` | `MapEnsureAsync` (conversión) |
| `MlResult<T>` | `Func<T, MlErrorsDetails>` | `MapEnsureAsync` (conversión) |
| `MlResult<T>` | `string` | `MapEnsureAsync` (conversión) |
| `MlResult<T>` | `Func<T, string>` | `MapEnsureAsync` (conversión) |
| `Task<MlResult<T>>` | `MlErrorsDetails` | `MapEnsureAsync` |
| `Task<MlResult<T>>` | `Func<T, MlErrorsDetails>` | `MapEnsureAsync` |
| `Task<MlResult<T>>` | `string` | `MapEnsureAsync` |
| `Task<MlResult<T>>` | `Func<T, string>` | `MapEnsureAsync` |

---

## Ejemplos Prácticos

### Ejemplo 1: Validaciones de Dominio Empresarial

```csharp
public class EmployeeValidationService
{
    public MlResult<Employee> ValidateEmployeeForPromotion(int employeeId)
    {
        return GetEmployee(employeeId)
            .MapEnsure(emp => emp.IsActive, "Employee must be active for promotion")
            .MapEnsure(emp => emp.YearsOfService >= 2, 
                      emp => $"Employee {emp.Name} needs at least 2 years of service. Current: {emp.YearsOfService} years")
            .MapEnsure(emp => emp.PerformanceRating >= 3.5m,
                      emp => new MlErrorsDetails
                      {
                          ErrorMessage = $"Performance rating insufficient for promotion",
                          ErrorCode = "INSUFFICIENT_PERFORMANCE",
                          AdditionalInfo = new 
                          { 
                              EmployeeId = emp.Id,
                              CurrentRating = emp.PerformanceRating,
                              RequiredRating = 3.5m,
                              Gap = 3.5m - emp.PerformanceRating
                          }
                      })
            .MapEnsure(emp => !emp.HasDisciplinaryActions,
                      "Employee cannot have pending disciplinary actions")
            .MapEnsure(emp => emp.Department != null,
                      "Employee must be assigned to a department");
    }
    
    public MlResult<Employee> ValidateEmployeeForRemoteWork(int employeeId)
    {
        return GetEmployee(employeeId)
            .MapEnsure(emp => emp.IsActive, "Only active employees can work remotely")
            .MapEnsure(emp => emp.JobLevel >= JobLevel.Senior,
                      emp => $"Remote work requires Senior level or above. Current level: {emp.JobLevel}")
            .MapEnsure(emp => emp.HasRequiredEquipment,
                      emp => $"Employee {emp.Name} lacks required equipment for remote work")
            .MapEnsure(emp => emp.Manager != null,
                      "Employee must have an assigned manager for remote work approval")
            .MapEnsure(emp => emp.RemoteWorkCertification != null && emp.RemoteWorkCertification > DateTime.UtcNow.AddMonths(-12),
                      emp => $"Remote work certification expired or missing. Last certification: {emp.RemoteWorkCertification?.ToString("dd/MM/yyyy") ?? "Never"}");
    }
    
    public MlResult<Employee> ValidateEmployeeForSalaryIncrease(int employeeId, decimal proposedIncrease)
    {
        return GetEmployee(employeeId)
            .MapEnsure(emp => emp.IsActive, "Employee must be active")
            .MapEnsure(emp => emp.LastSalaryReview.AddMonths(6) <= DateTime.UtcNow,
                      emp => $"Salary can only be reviewed every 6 months. Last review: {emp.LastSalaryReview:dd/MM/yyyy}")
            .MapEnsure(emp => proposedIncrease <= emp.CurrentSalary * 0.25m,
                      emp => new MlErrorsDetails
                      {
                          ErrorMessage = "Proposed increase exceeds maximum allowed",
                          ErrorCode = "EXCESSIVE_INCREASE",
                          AdditionalInfo = new
                          {
                              CurrentSalary = emp.CurrentSalary,
                              ProposedIncrease = proposedIncrease,
                              MaxAllowedIncrease = emp.CurrentSalary * 0.25m,
                              IncreasePercentage = (proposedIncrease / emp.CurrentSalary) * 100
                          }
                      })
            .MapEnsure(emp => emp.BudgetApproval != null,
                      "Budget approval required for salary increases");
    }
    
    private MlResult<Employee> GetEmployee(int employeeId)
    {
        if (employeeId <= 0)
            return MlResult<Employee>.Fail("Invalid employee ID");
            
        var employee = GetMockEmployee(employeeId);
        return employee != null
            ? MlResult<Employee>.Valid(employee)
            : MlResult<Employee>.Fail($"Employee with ID {employeeId} not found");
    }
    
    private Employee GetMockEmployee(int id)
    {
        return new Employee
        {
            Id = id,
            Name = "John Doe",
            IsActive = true,
            YearsOfService = 3,
            PerformanceRating = 4.2m,
            HasDisciplinaryActions = false,
            Department = new Department { Name = "Engineering", Id = 1 },
            JobLevel = JobLevel.Senior,
            HasRequiredEquipment = true,
            Manager = new Employee { Id = 100, Name = "Jane Manager" },
            RemoteWorkCertification = DateTime.UtcNow.AddMonths(-6),
            CurrentSalary = 75000,
            LastSalaryReview = DateTime.UtcNow.AddMonths(-8),
            BudgetApproval = new BudgetApproval { Id = 1, IsApproved = true }
        };
    }
}

// Clases de apoyo
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public int YearsOfService { get; set; }
    public decimal PerformanceRating { get; set; }
    public bool HasDisciplinaryActions { get; set; }
    public Department Department { get; set; }
    public JobLevel JobLevel { get; set; }
    public bool HasRequiredEquipment { get; set; }
    public Employee Manager { get; set; }
    public DateTime? RemoteWorkCertification { get; set; }
    public decimal CurrentSalary { get; set; }
    public DateTime LastSalaryReview { get; set; }
    public BudgetApproval BudgetApproval { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class BudgetApproval
{
    public int Id { get; set; }
    public bool IsApproved { get; set; }
}

public enum JobLevel
{
    Junior = 1,
    Mid = 2,
    Senior = 3,
    Lead = 4,
    Principal = 5
}
```

### Ejemplo 2: Validaciones de Datos y Rangos

```csharp
public class DataValidationService
{
    public MlResult<UserRegistration> ValidateUserRegistration(UserRegistrationRequest request)
    {
        return ValidateBasicRequest(request)
            .MapEnsure(req => !string.IsNullOrWhiteSpace(req.Email), "Email is required")
            .MapEnsure(req => IsValidEmailFormat(req.Email), 
                      req => $"Invalid email format: '{req.Email}'")
            .MapEnsure(req => req.Email.Length <= 254, 
                      req => $"Email too long: {req.Email.Length} characters (max: 254)")
            .MapEnsure(req => !string.IsNullOrWhiteSpace(req.Password), "Password is required")
            .MapEnsure(req => req.Password.Length >= 8,
                      req => $"Password too short: {req.Password.Length} characters (min: 8)")
            .MapEnsure(req => req.Password.Length <= 128,
                      req => $"Password too long: {req.Password.Length} characters (max: 128)")
            .MapEnsure(req => HasUpperCase(req.Password), "Password must contain at least one uppercase letter")
            .MapEnsure(req => HasLowerCase(req.Password), "Password must contain at least one lowercase letter")
            .MapEnsure(req => HasDigit(req.Password), "Password must contain at least one digit")
            .MapEnsure(req => HasSpecialChar(req.Password), "Password must contain at least one special character")
            .MapEnsure(req => req.Age >= 13,
                      req => new MlErrorsDetails
                      {
                          ErrorMessage = "User must be at least 13 years old",
                          ErrorCode = "UNDERAGE_USER",
                          AdditionalInfo = new { ProvidedAge = req.Age, MinimumAge = 13 }
                      })
            .MapEnsure(req => req.Age <= 120,
                      req => $"Invalid age: {req.Age} (maximum: 120)")
            .Map(req => new UserRegistration
            {
                Email = req.Email.ToLower(),
                HashedPassword = HashPassword(req.Password),
                Age = req.Age,
                CreatedAt = DateTime.UtcNow
            });
    }
    
    public async Task<MlResult<BankTransfer>> ValidateBankTransferAsync(BankTransferRequest request)
    {
        return await ValidateTransferRequest(request)
            .MapEnsureAsync(req => req.Amount > 0,
                           req => $"Transfer amount must be positive. Provided: {req.Amount:C}")
            .MapEnsureAsync(req => req.Amount <= 10000,
                           req => new MlErrorsDetails
                           {
                               ErrorMessage = "Transfer amount exceeds daily limit",
                               ErrorCode = "AMOUNT_LIMIT_EXCEEDED",
                               AdditionalInfo = new
                               {
                                   RequestedAmount = req.Amount,
                                   DailyLimit = 10000,
                                   ExcessAmount = req.Amount - 10000
                               }
                           })
            .MapEnsureAsync(async req => await IsAccountActiveAsync(req.FromAccountId),
                           "Source account is not active")
            .MapEnsureAsync(async req => await IsAccountActiveAsync(req.ToAccountId),
                           "Destination account is not active")
            .MapEnsureAsync(async req => await HasSufficientFundsAsync(req.FromAccountId, req.Amount),
                           req => $"Insufficient funds. Required: {req.Amount:C}")
            .MapEnsureAsync(async req => !await IsBlockedAccountAsync(req.FromAccountId),
                           "Source account is blocked")
            .MapEnsureAsync(async req => !await IsBlockedAccountAsync(req.ToAccountId),
                           "Destination account is blocked")
            .MapEnsureAsync(req => req.FromAccountId != req.ToAccountId,
                           "Cannot transfer to the same account")
            .MapAsync(req => new BankTransfer
            {
                Id = Guid.NewGuid(),
                FromAccountId = req.FromAccountId,
                ToAccountId = req.ToAccountId,
                Amount = req.Amount,
                Description = req.Description ?? "Bank transfer",
                CreatedAt = DateTime.UtcNow,
                Status = TransferStatus.Pending
            });
    }
    
    public MlResult<ProductPrice> ValidateProductPricing(ProductPricingRequest request)
    {
        return ValidatePricingRequest(request)
            .MapEnsure(req => req.BasePrice > 0,
                      req => $"Base price must be positive. Provided: {req.BasePrice:C}")
            .MapEnsure(req => req.BasePrice <= 1000000,
                      req => $"Base price too high: {req.BasePrice:C} (max: {1000000:C})")
            .MapEnsure(req => req.DiscountPercentage >= 0,
                      req => $"Discount cannot be negative: {req.DiscountPercentage}%")
            .MapEnsure(req => req.DiscountPercentage <= 90,
                      req => new MlErrorsDetails
                      {
                          ErrorMessage = "Discount percentage too high",
                          ErrorCode = "EXCESSIVE_DISCOUNT",
                          AdditionalInfo = new
                          {
                              RequestedDiscount = req.DiscountPercentage,
                              MaxAllowedDiscount = 90,
                              BasePrice = req.BasePrice,
                              CalculatedPrice = req.BasePrice * (1 - req.DiscountPercentage / 100)
                          }
                      })
            .MapEnsure(req => req.TaxRate >= 0,
                      req => $"Tax rate cannot be negative: {req.TaxRate}%")
            .MapEnsure(req => req.TaxRate <= 50,
                      req => $"Tax rate too high: {req.TaxRate}% (max: 50%)")
            .Map(req => new ProductPrice
            {
                BasePrice = req.BasePrice,
                DiscountAmount = req.BasePrice * (req.DiscountPercentage / 100),
                DiscountedPrice = req.BasePrice * (1 - req.DiscountPercentage / 100),
                TaxAmount = req.BasePrice * (1 - req.DiscountPercentage / 100) * (req.TaxRate / 100),
                FinalPrice = req.BasePrice * (1 - req.DiscountPercentage / 100) * (1 + req.TaxRate / 100),
                Currency = req.Currency ?? "USD",
                CalculatedAt = DateTime.UtcNow
            });
    }
    
    // Métodos auxiliares
    private MlResult<UserRegistrationRequest> ValidateBasicRequest(UserRegistrationRequest request)
    {
        return request != null
            ? MlResult<UserRegistrationRequest>.Valid(request)
            : MlResult<UserRegistrationRequest>.Fail("Registration request cannot be null");
    }
    
    private MlResult<BankTransferRequest> ValidateTransferRequest(BankTransferRequest request)
    {
        return request != null
            ? MlResult<BankTransferRequest>.Valid(request)
            : MlResult<BankTransferRequest>.Fail("Transfer request cannot be null");
    }
    
    private MlResult<ProductPricingRequest> ValidatePricingRequest(ProductPricingRequest request)
    {
        return request != null
            ? MlResult<ProductPricingRequest>.Valid(request)
            : MlResult<ProductPricingRequest>.Fail("Pricing request cannot be null");
    }
    
    private bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private bool HasUpperCase(string str) => str.Any(char.IsUpper);
    private bool HasLowerCase(string str) => str.Any(char.IsLower);
    private bool HasDigit(string str) => str.Any(char.IsDigit);
    private bool HasSpecialChar(string str) => str.Any(c => !char.IsLetterOrDigit(c));
    
    private string HashPassword(string password)
    {
        // Simulación de hash de contraseña
        return $"hashed_{password.GetHashCode():X}";
    }
    
    private async Task<bool> IsAccountActiveAsync(string accountId)
    {
        await Task.Delay(10); // Simulación de consulta asíncrona
        return !string.IsNullOrEmpty(accountId) && accountId != "INACTIVE_ACCOUNT";
    }
    
    private async Task<bool> HasSufficientFundsAsync(string accountId, decimal amount)
    {
        await Task.Delay(10);
        // Simulación: cuenta tiene $5000
        return amount <= 5000;
    }
    
    private async Task<bool> IsBlockedAccountAsync(string accountId)
    {
        await Task.Delay(10);
        return accountId == "BLOCKED_ACCOUNT";
    }
}

// Clases de apoyo
public class UserRegistrationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
}

public class UserRegistration
{
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BankTransferRequest
{
    public string FromAccountId { get; set; }
    public string ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}

public class BankTransfer
{
    public Guid Id { get; set; }
    public string FromAccountId { get; set; }
    public string ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransferStatus Status { get; set; }
}

public class ProductPricingRequest
{
    public decimal BasePrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TaxRate { get; set; }
    public string Currency { get; set; }
}

public class ProductPrice
{
    public decimal BasePrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal FinalPrice { get; set; }
    public string Currency { get; set; }
    public DateTime CalculatedAt { get; set; }
}

public enum TransferStatus
{
    Pending,
    Approved,
    Rejected,
    Completed
}
```

### Ejemplo 3: Validaciones de Configuración y Límites

```csharp
public class SystemConfigurationService
{
    public MlResult<DatabaseConfig> ValidateDatabaseConfiguration(DatabaseConfigRequest request)
    {
        return ValidateConfigRequest(request)
            .MapEnsure(cfg => !string.IsNullOrWhiteSpace(cfg.ConnectionString),
                      "Database connection string is required")
            .MapEnsure(cfg => cfg.ConnectionTimeout > 0,
                      cfg => $"Connection timeout must be positive: {cfg.ConnectionTimeout} seconds")
            .MapEnsure(cfg => cfg.ConnectionTimeout <= 300,
                      cfg => $"Connection timeout too high: {cfg.ConnectionTimeout}s (max: 300s)")
            .MapEnsure(cfg => cfg.MaxPoolSize > 0,
                      "Connection pool size must be positive")
            .MapEnsure(cfg => cfg.MaxPoolSize <= 1000,
                      cfg => new MlErrorsDetails
                      {
                          ErrorMessage = "Connection pool size exceeds recommended maximum",
                          ErrorCode = "POOL_SIZE_TOO_HIGH",
                          AdditionalInfo = new
                          {
                              RequestedSize = cfg.MaxPoolSize,
                              RecommendedMax = 1000,
                              PerformanceImpact = "High pool sizes may impact memory usage"
                          }
                      })
            .MapEnsure(cfg => cfg.CommandTimeout >= cfg.ConnectionTimeout,
                      cfg => $"Command timeout ({cfg.CommandTimeout}s) should be >= connection timeout ({cfg.ConnectionTimeout}s)")
            .MapEnsure(cfg => IsValidConnectionString(cfg.ConnectionString),
                      "Invalid connection string format")
            .Map(cfg => new DatabaseConfig
            {
                ConnectionString = cfg.ConnectionString,
                ConnectionTimeout = cfg.ConnectionTimeout,
                CommandTimeout = cfg.CommandTimeout,
                MaxPoolSize = cfg.MaxPoolSize,
                EnableRetry = cfg.EnableRetry,
                CreatedAt = DateTime.UtcNow
            });
    }
    
    public MlResult<CacheConfig> ValidateCacheConfiguration(CacheConfigRequest request)
    {
        return ValidateCacheRequest(request)
            .MapEnsure(cfg => cfg.MaxMemoryMB > 0,
                      cfg => $"Cache memory must be positive: {cfg.MaxMemoryMB} MB")
            .MapEnsure(cfg => cfg.MaxMemoryMB <= 16384,
                      cfg => $"Cache memory too high: {cfg.MaxMemoryMB} MB (max: 16 GB)")
            .MapEnsure(cfg => cfg.DefaultExpirationMinutes > 0,
                      "Default expiration must be positive")
            .MapEnsure(cfg => cfg.DefaultExpirationMinutes <= 43200,
                      cfg => $"Default expiration too long: {cfg.DefaultExpirationMinutes} minutes (max: 30 days)")
            .MapEnsure(cfg => cfg.MaxItemSize > 0,
                      "Maximum item size must be positive")
            .MapEnsure(cfg => cfg.MaxItemSize <= cfg.MaxMemoryMB * 1024 * 1024 / 10,
                      cfg => new MlErrorsDetails
                      {
                          ErrorMessage = "Maximum item size too large for cache memory",
                          ErrorCode = "ITEM_SIZE_TOO_LARGE",
                          AdditionalInfo = new
                          {
                              MaxItemSize = cfg.MaxItemSize,
                              CacheMemory = cfg.MaxMemoryMB * 1024 * 1024,
                              RecommendedMaxItemSize = cfg.MaxMemoryMB * 1024 * 1024 / 10,
                              Reason = "Item size should not exceed 10% of total cache memory"
                          }
                      })
            .MapEnsure(cfg => cfg.CleanupIntervalMinutes > 0,
                      "Cleanup interval must be positive")
            .MapEnsure(cfg => cfg.CleanupIntervalMinutes <= cfg.DefaultExpirationMinutes / 2,
                      cfg => $"Cleanup interval ({cfg.CleanupIntervalMinutes}m) should be <= half of default expiration ({cfg.DefaultExpirationMinutes / 2}m)")
            .Map(cfg => new CacheConfig
            {
                MaxMemoryMB = cfg.MaxMemoryMB,
                DefaultExpirationMinutes = cfg.DefaultExpirationMinutes,
                MaxItemSize = cfg.MaxItemSize,
                CleanupIntervalMinutes = cfg.CleanupIntervalMinutes,
                EnableCompression = cfg.EnableCompression,
                EnableStatistics = cfg.EnableStatistics,
                ConfiguredAt = DateTime.UtcNow
            });
    }
    
    public MlResult<ApiLimitsConfig> ValidateApiLimitsConfiguration(ApiLimitsConfigRequest request)
    {
        return ValidateApiLimitsRequest(request)
            .MapEnsure(cfg => cfg.RequestsPerMinute > 0,
                      "Requests per minute must be positive")
            .MapEnsure(cfg => cfg.RequestsPerMinute <= 10000,
                      cfg => $"Requests per minute too high: {cfg.RequestsPerMinute} (max: 10,000)")
            .MapEnsure(cfg => cfg.RequestsPerHour >= cfg.RequestsPerMinute,
                      cfg => $"Hourly limit ({cfg.RequestsPerHour}) must be >= minute limit ({cfg.RequestsPerMinute})")
            .MapEnsure(cfg => cfg.RequestsPerDay >= cfg.RequestsPerHour,
                      cfg => $"Daily limit ({cfg.RequestsPerDay}) must be >= hourly limit ({cfg.RequestsPerHour})")
            .MapEnsure(cfg => cfg.MaxRequestSizeMB > 0,
                      "Maximum request size must be positive")
            .MapEnsure(cfg => cfg.MaxRequestSizeMB <= 100,
                      cfg => new MlErrorsDetails
                      {
                          ErrorMessage = "Maximum request size exceeds recommended limit",
                          ErrorCode = "REQUEST_SIZE_TOO_LARGE",
                          AdditionalInfo = new
                          {
                              RequestedSize = cfg.MaxRequestSizeMB,
                              RecommendedMax = 100,
                              SecurityConcern = "Large request sizes may impact server performance"
                          }
                      })
            .MapEnsure(cfg => cfg.MaxConcurrentRequests > 0,
                      "Maximum concurrent requests must be positive")
            .MapEnsure(cfg => cfg.MaxConcurrentRequests <= 1000,
                      cfg => $"Maximum concurrent requests too high: {cfg.MaxConcurrentRequests} (max: 1,000)")
            .MapEnsure(cfg => IsValidTimeWindow(cfg.ThrottleWindowMinutes),
                      cfg => $"Invalid throttle window: {cfg.ThrottleWindowMinutes} minutes")
            .Map(cfg => new ApiLimitsConfig
            {
                RequestsPerMinute = cfg.RequestsPerMinute,
                RequestsPerHour = cfg.RequestsPerHour,
                RequestsPerDay = cfg.RequestsPerDay,
                MaxRequestSizeMB = cfg.MaxRequestSizeMB,
                MaxConcurrentRequests = cfg.MaxConcurrentRequests,
                ThrottleWindowMinutes = cfg.ThrottleWindowMinutes,
                EnableThrottling = cfg.EnableThrottling,
                ConfiguredAt = DateTime.UtcNow
            });
    }
    
    // Métodos auxiliares
    private MlResult<DatabaseConfigRequest> ValidateConfigRequest(DatabaseConfigRequest request)
    {
        return request != null
            ? MlResult<DatabaseConfigRequest>.Valid(request)
            : MlResult<DatabaseConfigRequest>.Fail("Database configuration request cannot be null");
    }
    
    private MlResult<CacheConfigRequest> ValidateCacheRequest(CacheConfigRequest request)
    {
        return request != null
            ? MlResult<CacheConfigRequest>.Valid(request)
            : MlResult<CacheConfigRequest>.Fail("Cache configuration request cannot be null");
    }
    
    private MlResult<ApiLimitsConfigRequest> ValidateApiLimitsRequest(ApiLimitsConfigRequest request)
    {
        return request != null
            ? MlResult<ApiLimitsConfigRequest>.Valid(request)
            : MlResult<ApiLimitsConfigRequest>.Fail("API limits configuration request cannot be null");
    }
    
    private bool IsValidConnectionString(string connectionString)
    {
        try
        {
            var builder = new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };
            return builder.ContainsKey("server") || builder.ContainsKey("data source");
        }
        catch
        {
            return false;
        }
    }
    
    private bool IsValidTimeWindow(int windowMinutes)
    {
        var validWindows = new[] { 1, 5, 10, 15, 30, 60 };
        return validWindows.Contains(windowMinutes);
    }
}

// Clases de apoyo
public class DatabaseConfigRequest
{
    public string ConnectionString { get; set; }
    public int ConnectionTimeout { get; set; }
    public int CommandTimeout { get; set; }
    public int MaxPoolSize { get; set; }
    public bool EnableRetry { get; set; }
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; }
    public int ConnectionTimeout { get; set; }
    public int CommandTimeout { get; set; }
    public int MaxPoolSize { get; set; }
    public bool EnableRetry { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CacheConfigRequest
{
    public int MaxMemoryMB { get; set; }
    public int DefaultExpirationMinutes { get; set; }
    public int MaxItemSize { get; set; }
    public int CleanupIntervalMinutes { get; set; }
    public bool EnableCompression { get; set; }
    public bool EnableStatistics { get; set; }
}

public class CacheConfig
{
    public int MaxMemoryMB { get; set; }
    public int DefaultExpirationMinutes { get; set; }
    public int MaxItemSize { get; set; }
    public int CleanupIntervalMinutes { get; set; }
    public bool EnableCompression { get; set; }
    public bool EnableStatistics { get; set; }
    public DateTime ConfiguredAt { get; set; }
}

public class ApiLimitsConfigRequest
{
    public int RequestsPerMinute { get; set; }
    public int RequestsPerHour { get; set; }
    public int RequestsPerDay { get; set; }
    public int MaxRequestSizeMB { get; set; }
    public int MaxConcurrentRequests { get; set; }
    public int ThrottleWindowMinutes { get; set; }
    public bool EnableThrottling { get; set; }
}

public class ApiLimitsConfig
{
    public int RequestsPerMinute { get; set; }
    public int RequestsPerHour { get; set; }
    public int RequestsPerDay { get; set; }
    public int MaxRequestSizeMB { get; set; }
    public int MaxConcurrentRequests { get; set; }
    public int ThrottleWindowMinutes { get; set; }
    public bool EnableThrottling { get; set; }
    public DateTime ConfiguredAt { get; set; }
}
```

---

## Comparación con Otros Patrones

### MapEnsure vs Map

```csharp
// Map: Transformación de valor
var transformed = GetNumber()
    .Map(n => n * 2);  // Siempre retorna int transformado

// MapEnsure: Validación con preservación
var validated = GetNumber()
    .MapEnsure(n => n > 0, "Number must be positive");  // Retorna el mismo int o error
```

### MapEnsure vs Bind

```csharp
// Bind: Encadenamiento de operaciones que pueden fallar
var processed = GetData()
    .Bind(data => ValidateAndProcess(data));  // ValidateAndProcess retorna MlResult<T>

// MapEnsure: Validación simple con error directo
var validated = GetData()
    .MapEnsure(data => data.IsValid, "Data is not valid");  // Retorna mismo tipo o error
```

### MapEnsure vs Match

```csharp
// Match: Manejo explícito de ambos casos
var result = GetValue()
    .Match(
        fail: errors => HandleError(errors),
        valid: value => value > 0 ? ProcessValue(value) : HandleInvalidValue(value)
    );

// MapEnsure: Validación funcional más concisa
var validated = GetValue()
    .MapEnsure(value => value > 0, "Value must be positive")
    .Map(validValue => ProcessValue(validValue));
```

---

## Mejores Prácticas

### 1. Ordenamiento de Validaciones

```csharp
// ✅ Correcto: Validaciones de más básica a más específica
var result = GetUser()
    .MapEnsure(user => user != null, "User cannot be null")  // Existencia
    .MapEnsure(user => user.IsActive, "User must be active")  // Estado básico
    .MapEnsure(user => user.Email != null, "Email is required")  // Propiedades requeridas
    .MapEnsure(user => IsValidEmail(user.Email), "Invalid email format")  // Formato
    .MapEnsure(user => user.Age >= 18, "User must be 18 or older")  // Reglas de negocio
    .MapEnsure(user => HasPermission(user), "Insufficient permissions");  // Autorización

// ❌ Incorrecto: Orden aleatorio puede causar errores confusos
var badResult = GetUser()
    .MapEnsure(user => HasPermission(user), "Insufficient permissions")  // Puede fallar si user es null
    .MapEnsure(user => user != null, "User cannot be null");
```

### 2. Mensajes de Error Descriptivos

```csharp
// ✅ Correcto: Mensajes específicos con contexto
var result = GetTransaction()
    .MapEnsure(tx => tx.Amount > 0, 
              tx => $"Transaction amount must be positive. Provided: {tx.Amount:C}")
    .MapEnsure(tx => tx.Amount <= GetDailyLimit(tx.AccountId),
              tx => new MlErrorsDetails
              {
                  ErrorMessage = "Transaction exceeds daily limit",
                  ErrorCode = "DAILY_LIMIT_EXCEEDED",
                  AdditionalInfo = new
                  {
                      RequestedAmount = tx.Amount,
                      DailyLimit = GetDailyLimit(tx.AccountId),
                      AccountId = tx.AccountId
                  }
              });

// ❌ Incorrecto: Mensajes genéricos
var badResult = GetTransaction()
    .MapEnsure(tx => tx.Amount > 0, "Invalid amount")
    .MapEnsure(tx => tx.Amount <= GetDailyLimit(tx.AccountId), "Limit exceeded");
```

### 3. Agrupación Lógica de Validaciones

```csharp
// ✅ Correcto: Agrupar validaciones relacionadas
public MlResult<Order> ValidateOrder(OrderRequest request)
{
    return ValidateBasicOrderInfo(request)
        .Bind(order => ValidateOrderItems(order))
        .Bind(order => ValidateOrderLimits(order))
        .Bind(order => ValidateOrderPermissions(order));
}

private MlResult<OrderRequest> ValidateBasicOrderInfo(OrderRequest request)
{
    return request.ToMlResult()
        .MapEnsure(req => req != null, "Order request cannot be null")
        .MapEnsure(req => req.CustomerId > 0, "Valid customer ID required")
        .MapEnsure(req => !string.IsNullOrWhiteSpace(req.OrderNumber), "Order number required");
}

private MlResult<OrderRequest> ValidateOrderItems(OrderRequest request)
{
    return request.ToMlResult()
        .MapEnsure(req => req.Items?.Any() == true, "Order must contain at least one item")
        .MapEnsure(req => req.Items.All(i => i.Quantity > 0), "All items must have positive quantity")
        .MapEnsure(req => req.Items.All(i => i.Price > 0), "All items must have positive price");
}

// ❌ Incorrecto: Todas las validaciones mezcladas
public MlResult<Order> ValidateOrderBad(OrderRequest request)
{
    return request.ToMlResult()
        .MapEnsure(req => req != null, "Order request cannot be null")
        .MapEnsure(req => req.Items?.Any() == true, "Order must contain at least one item")
        .MapEnsure(req => req.CustomerId > 0, "Valid customer ID required")
        .MapEnsure(req => req.Items.All(i => i.Quantity > 0), "All items must have positive quantity")
        .MapEnsure(req => !string.IsNullOrWhiteSpace(req.OrderNumber), "Order number required");
        // ... mezclando diferentes tipos de validaciones
}
```

### 4. Uso de Constructores de Error Dinámicos

```csharp
// ✅ Correcto: Constructor de error con información útil
var result = GetFileUpload()
    .MapEnsure(file => file.Size <= MaxFileSize,
              file => new MlErrorsDetails
              {
                  ErrorMessage = $"File size exceeds maximum allowed",
                  ErrorCode = "FILE_TOO_LARGE",
                  AdditionalInfo = new
                  {
                      FileName = file.Name,
                      FileSize = file.Size,
                      FileSizeFormatted = FormatFileSize(file.Size),
                      MaxAllowedSize = MaxFileSize,
                      MaxAllowedFormatted = FormatFileSize(MaxFileSize),
                      ExcessSize = file.Size - MaxFileSize
                  }
              });

// ✅ Correcto: Constructor de mensaje basado en valor
var priceResult = GetProduct()
    .MapEnsure(product => product.Price >= MinPrice,
              product => $"Product '{product.Name}' price ({product.Price:C}) below minimum ({MinPrice:C})");

// ❌ Incorrecto: Error estático cuando se necesita información dinámica
var badResult = GetFileUpload()
    .MapEnsure(file => file.Size <= MaxFileSize, "File too large");
```

### 5. Manejo de Operaciones Asíncronas

```csharp
// ✅ Correcto: Validaciones asíncronas apropiadas
public async Task<MlResult<User>> ValidateUserAsync(UserRequest request)
{
    return await request.ToMlResult()
        .MapEnsureAsync(req => req.Email != null, "Email required")
        .MapEnsureAsync(async req => !await IsEmailTakenAsync(req.Email),
                       req => $"Email '{req.Email}' is already registered")
        .MapEnsureAsync(async req => await IsValidDomainAsync(GetEmailDomain(req.Email)),
                       req => $"Email domain '{GetEmailDomain(req.Email)}' is not allowed")
        .MapAsync(req => new User { Email = req.Email, CreatedAt = DateTime.UtcNow });
}

// ✅ Correcto: Combinar validaciones síncronas y asíncronas eficientemente
public async Task<MlResult<Account>> ValidateAccountAsync(AccountRequest request)
{
    // Validaciones síncronas primero
    var syncValidation = request.ToMlResult()
        .MapEnsure(req => req != null, "Account request required")
        .MapEnsure(req => req.Balance >= 0, "Initial balance cannot be negative")
        .MapEnsure(req => !string.IsNullOrWhiteSpace(req.AccountNumber), "Account number required");
    
    // Luego validaciones asíncronas
    return await syncValidation
        .MapEnsureAsync(async req => !await AccountExistsAsync(req.AccountNumber),
                       req => $"Account number '{req.AccountNumber}' already exists")
        .MapEnsureAsync(async req => await IsValidBankCodeAsync(req.BankCode),
                       req => $"Invalid bank code: '{req.BankCode}'");
}

// ❌ Incorrecto: Mezclar innecesariamente validaciones síncronas con asíncronas
public async Task<MlResult<User>> ValidateUserBadAsync(UserRequest request)
{
    return await request.ToMlResult()
        .MapEnsureAsync(req => req.Email != null, "Email required")  // Innecesariamente async
        .MapEnsureAsync(async req => !await IsEmailTakenAsync(req.Email), "Email taken")
        .MapEnsureAsync(req => req.Age >= 18, "Must be 18+");  // Innecesariamente async
}
```

### 6. Optimización de Rendimiento

```csharp
// ✅ Correcto: Validaciones rápidas primero para cortocircuito temprano
var result = GetData()
    .MapEnsure(data => data != null, "Data required")  // Validación muy rápida
    .MapEnsure(data => data.Count > 0, "Data cannot be empty")  // Validación rápida
    .MapEnsure(data => data.Count <= 1000, "Too many items")  // Validación rápida
    .MapEnsure(data => data.All(item => item.IsValid), "All items must be valid")  // Validación más costosa
    .MapEnsure(data => IsComplexValidationPassed(data), "Complex validation failed");  // Validación más costosa

// ✅ Correcto: Cachear valores costosos de calcular
var expensiveValue = CalculateExpensiveValue(data);
var result = GetData()
    .MapEnsure(data => expensiveValue > threshold,
              data => $"Calculated value {expensiveValue} below threshold {threshold}");

// ❌ Incorrecto: Calcular valores costosos múltiples veces
var badResult = GetData()
    .MapEnsure(data => CalculateExpensiveValue(data) > threshold, "Below threshold")
    .MapEnsure(data => CalculateExpensiveValue(data) < maxValue, "Above maximum");  // Recalcula innecesariamente
```

---

## Consideraciones de Rendimiento

### Cortocircuito de Validaciones

- **Evaluación Lazy**: Si el resultado inicial es fallido, ninguna validación se ejecuta
- **Orden Estratégico**: Colocar validaciones rápidas y más propensas a fallar primero
- **Caching**: Cachear resultados de validaciones costosas cuando sea apropiado

### Manejo de Memoria

- **Constructor de Errores**: Los constructores de errores solo se ejecutan cuando la validación falla
- **Información Adicional**: Incluir información de debug solo cuando sea necesario
- **String Formatting**: Usar constructores de mensaje para evitar formateo innecesario

### Operaciones Asíncronas

- **Separación de Concerns**: Validaciones síncronas primero, luego asíncronas
- **Batch Validation**: Para múltiples validaciones asíncronas independientes, considerar `Task.WhenAll`
- **Timeout Handling**: Considerar timeouts para validaciones que involucran servicios externos

---

## Resumen

La clase `MlResultActionsMapEnsure` proporciona validaciones condicionales funcionales:

- **`MapEnsure`**: Validación que preserva el tipo y convierte a error si la condición falla
- **`MapEnsureAsync`**: Soporte completo para validaciones asíncronas
- **Constructores de Error Flexibles**: Desde mensajes simples hasta `MlErrorsDetails` complejos
- **Preservación de Tipo**: El tipo `T` permanece inalterado, solo cambia el estado de éxito/fallo

Estas operaciones son ideales para:

- **Validaciones de Dominio**: Reglas de negocio complejas
- **Guard Clauses Funcionales**: Precondiciones en pipelines funcionales
- **Filtrado con Error**: Similar a `Where` pero generando errores descriptivos
- **Validaciones en Cadena**: Múltiples verificaciones ordenadas lógicamente

La diferencia clave con otros patrones es que `MapEnsure` actúa como un **filtro con error**, manteniendo el valor original si pasa la validación o convirtiéndolo en error con información detallada si falla.