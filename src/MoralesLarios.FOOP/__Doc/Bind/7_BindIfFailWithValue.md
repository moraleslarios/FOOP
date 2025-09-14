# MlResultActionsBind - Operaciones de Binding para Manejo de Fallos con Valor Previo

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Funcionalidad](#análisis-de-la-funcionalidad)
3. [Variantes de BindIfFailWithValue](#variantes-de-bindiffailwithvalue)
4. [Variantes de TryBindIfFailWithValue](#variantes-de-trybindiffailwithvalue)
5. [Patrones de Uso](#patrones-de-uso)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La sección **BindIfFailWithValue** de `MlResultActionsBind` proporciona operaciones especializadas para el manejo de errores que tienen acceso a un **valor previo almacenado** en los detalles del error. Estas operaciones implementan patrones de **recuperación contextual**, donde las funciones de recuperación pueden acceder al último valor válido que se procesó antes del fallo, permitiendo estrategias de recuperación más sofisticadas e informadas.

### Propósito Principal

- **Recuperación Contextual**: Acceso al valor previo para informar la estrategia de recuperación
- **Rollback Inteligente**: Volver a un estado previo conocido y válido
- **Compensación de Errores**: Usar el valor previo para crear compensaciones o alternativas
- **Continuidad de Datos**: Mantener continuidad usando el último valor válido conocido

### Concepto Clave: Valor en Detalles de Error

El sistema `MlResult` permite almacenar valores en los detalles del error cuando ocurre un fallo. Esto significa que cuando una operación falla después de procesar exitosamente un valor, ese valor puede estar disponible para funciones de recuperación posteriores.

---

## Análisis de la Funcionalidad

### Filosofía de BindIfFailWithValue

```
MlResult<T> → ¿Es Fallo?
              ├─ Sí → Extraer Valor Previo → Función de Recuperación(ValorPrevio) → MlResult<T>
              └─ No → Retorna Valor Original (sin cambios)
```

### Flujo de Procesamiento

1. **Si el resultado fuente es exitoso**: Retorna el valor sin cambios
2. **Si el resultado fuente es fallido**: 
   - Extrae el valor previo almacenado usando `errorsDetails.GetDetailValue<T>()`
   - Si hay valor previo válido: Ejecuta la función de recuperación con ese valor
   - Si no hay valor previo: Puede propagar el error o usar valor por defecto

### Tipos de Operaciones

1. **BindIfFailWithValue Simple**: Recuperación usando valor previo del mismo tipo
2. **BindIfFailWithValue con Transformación**: Manejo de éxito y fallo con posible cambio de tipo
3. **TryBindIfFailWithValue**: Versiones seguras que capturan excepciones

---

## Variantes de BindIfFailWithValue

### Variante 1: BindIfFailWithValue Simple

**Propósito**: Recuperación usando el valor previo almacenado en el error

```csharp
public static MlResult<T> BindIfFailWithValue<T>(this MlResult<T> source,
                                                 Func<T, MlResult<T>> funcValue)
```

**Parámetros**:
- `source`: El resultado que puede contener un valor previo en caso de fallo
- `funcValue`: Función que recibe el valor previo y retorna un nuevo resultado

**Comportamiento**:
- Si `source` es exitoso: Retorna el valor sin cambios
- Si `source` es fallido: Extrae valor previo y ejecuta `funcValue(previousValue)`

**Flujo Interno**:
```csharp
source.Match(
    fail: errorsDetails => errorsDetails.GetDetailValue<T>().Bind(funcValue),
    valid: value => value
)
```

### Variante 2: BindIfFailWithValue con Transformación de Tipos

**Propósito**: Manejo completo con acceso a valor previo y posibilidad de cambiar tipos

```csharp
public static MlResult<TReturn> BindIfFailWithValue<T, TValue, TReturn>(
    this MlResult<T> source,
    Func<T, MlResult<TReturn>> funcValid,
    Func<TValue, MlResult<TReturn>> funcFail)
```

**Parámetros**:
- `source`: El resultado fuente
- `funcValid`: Función para manejar valores exitosos
- `funcFail`: Función para manejar fallos usando valor previo de tipo `TValue`

**Comportamiento**:
- Si `source` es exitoso: Ejecuta `funcValid(value)`
- Si `source` es fallido: Extrae valor previo de tipo `TValue` y ejecuta `funcFail(previousValue)`

### Soporte Asíncrono Completo

Ambas variantes incluyen soporte asíncrono completo:
- **Funciones asíncronas**: `Func<T, Task<MlResult<T>>>`
- **Fuente asíncrona**: `Task<MlResult<T>>`
- **Todas las combinaciones**: Función síncrona con fuente asíncrona, etc.

---

## Variantes de TryBindIfFailWithValue

### TryBindIfFailWithValue Simple

**Propósito**: Versión segura que captura excepciones en la función de recuperación

```csharp
public static MlResult<T> TryBindIfFailWithValue<T>(this MlResult<T> source,
                                                    Func<T, MlResult<T>> funcValue,
                                                    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Protege la función de recuperación contra excepciones
- Proporciona contexto específico sobre fallos en la recuperación

### TryBindIfFailWithValue con Transformación

**Propósito**: Versión segura para ambas funciones (éxito y fallo)

```csharp
public static MlResult<TReturn> TryBindIfFailWithValue<T, TValue, TReturn>(
    this MlResult<T> source,
    Func<T, MlResult<TReturn>> funcValid,
    Func<TValue, MlResult<TReturn>> funcFail,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Protege tanto la función de éxito como la de recuperación
- Maneja diferentes tipos para valor actual y valor previo

### Sobrecargas con Mensajes Simples

Todas las variantes incluyen sobrecargas que aceptan strings simples en lugar de `Func<Exception, string>`.

---

## Patrones de Uso

### Patrón 1: Rollback a Estado Previo

```csharp
// Volver al último estado válido conocido
var result = await riskyUpdate
    .BindIfFailWithValueAsync(async previousValue => 
        await RestoreToStateAsync(previousValue));
```

### Patrón 2: Compensación Inteligente

```csharp
// Usar valor previo para crear compensación
var result = await chargePayment
    .BindIfFailWithValueAsync(async previousBalance => 
        await CreateRefundAsync(previousBalance));
```

### Patrón 3: Continuidad de Datos

```csharp
// Continuar usando último valor válido con degradación gradual
var result = await updateCache
    .BindIfFailWithValue(lastValidData => 
        MlResult<CacheData>.Valid(lastValidData.MarkAsStale()));
```

### Patrón 4: Merge de Datos

```csharp
// Combinar valor actual con valor previo
var result = await partialUpdate
    .BindIfFailWithValueAsync<CurrentData, PreviousData, MergedData>(
        validAsync: current => ProcessSuccessfulUpdate(current),
        failAsync: async previous => await MergeWithPartialData(previous)
    );
```

### Patrón 5: Análisis de Diferencias

```csharp
// Analizar qué cambió entre valor previo y fallo actual
var result = await validation
    .BindIfFailWithValueAsync(async previousValue =>
    {
        var differences = AnalyzeDifferences(currentAttempt, previousValue);
        return await CreateRecoveryStrategy(differences);
    });
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Actualización de Configuración con Rollback

```csharp
public class ConfigurationUpdateService
{
    private readonly IConfigurationRepository _repository;
    private readonly IValidationService _validationService;
    private readonly ILogger<ConfigurationUpdateService> _logger;

    public async Task<MlResult<Configuration>> UpdateConfigurationAsync(
        int configId, 
        ConfigurationUpdate update)
    {
        var currentConfig = await GetCurrentConfigurationAsync(configId);
        
        return await currentConfig
            .BindAsync(config => ValidateUpdateAsync(config, update))
            .BindAsync(validatedConfig => ApplyUpdateAsync(validatedConfig, update))
            .BindIfFailWithValueAsync(async previousConfig =>
            {
                await LogUpdateFailureAsync(configId, previousConfig, update);
                return await RollbackToSafeConfigurationAsync(configId, previousConfig);
            });
    }

    public async Task<MlResult<Configuration>> UpdateConfigurationSafelyAsync(
        int configId, 
        ConfigurationUpdate update)
    {
        var currentConfig = await GetCurrentConfigurationAsync(configId);
        
        return await currentConfig
            .BindAsync(config => ValidateUpdateAsync(config, update))
            .TryBindIfFailWithValueAsync(
                funcValueAsync: async previousConfig => await CreateSafeRollbackAsync(configId, previousConfig),
                errorMessageBuilder: ex => $"Failed to rollback configuration {configId}: {ex.Message}"
            )
            .BindAsync(config => ApplyUpdateAsync(config, update))
            .TryBindIfFailWithValueAsync(
                funcValueAsync: async previousConfig => await HandleUpdateFailureAsync(configId, previousConfig, update),
                errorMessage: "Configuration update failed after validation"
            );
    }

    private async Task<MlResult<Configuration>> GetCurrentConfigurationAsync(int configId)
    {
        await Task.Delay(50);
        
        var config = new Configuration
        {
            Id = configId,
            Version = 1,
            Settings = new Dictionary<string, object>
            {
                ["MaxConnections"] = 100,
                ["Timeout"] = 30,
                ["EnableLogging"] = true
            },
            LastModified = DateTime.UtcNow.AddHours(-1),
            IsValid = true
        };
        
        return MlResult<Configuration>.Valid(config);
    }

    private async Task<MlResult<Configuration>> ValidateUpdateAsync(
        Configuration config, 
        ConfigurationUpdate update)
    {
        await Task.Delay(100);
        
        // Simular validación que puede fallar
        if (update.Settings.ContainsKey("MaxConnections") && 
            (int)update.Settings["MaxConnections"] < 0)
        {
            // Almacenar configuración previa en el error para posible rollback
            return MlResult<Configuration>.Fail(
                "Invalid MaxConnections value", 
                detailValue: config);
        }
        
        if (update.Settings.ContainsKey("InvalidSetting"))
        {
            return MlResult<Configuration>.Fail(
                "Unknown configuration setting", 
                detailValue: config);
        }
        
        var updatedConfig = new Configuration
        {
            Id = config.Id,
            Version = config.Version + 1,
            Settings = new Dictionary<string, object>(config.Settings),
            LastModified = DateTime.UtcNow,
            IsValid = true
        };
        
        // Aplicar actualizaciones
        foreach (var kvp in update.Settings)
        {
            updatedConfig.Settings[kvp.Key] = kvp.Value;
        }
        
        return MlResult<Configuration>.Valid(updatedConfig);
    }

    private async Task<MlResult<Configuration>> ApplyUpdateAsync(
        Configuration config, 
        ConfigurationUpdate update)
    {
        await Task.Delay(150);
        
        // Simular fallo en aplicación (por ejemplo, problema de persistencia)
        if (config.Id % 10 == 0)
        {
            return MlResult<Configuration>.Fail(
                "Database update failed", 
                detailValue: config);
        }
        
        // Simular fallo de validación post-aplicación
        if (config.Settings.ContainsKey("Timeout") && 
            (int)config.Settings["Timeout"] > 300)
        {
            return MlResult<Configuration>.Fail(
                "Configuration validation failed after update", 
                detailValue: config);
        }
        
        config.LastModified = DateTime.UtcNow;
        config.IsApplied = true;
        
        return MlResult<Configuration>.Valid(config);
    }

    private async Task<MlResult<Configuration>> RollbackToSafeConfigurationAsync(
        int configId, 
        Configuration previousConfig)
    {
        await Task.Delay(200);
        await LogRollbackActionAsync(configId, previousConfig);
        
        // Crear configuración de rollback basada en la previa
        var rollbackConfig = new Configuration
        {
            Id = previousConfig.Id,
            Version = previousConfig.Version, // Mantener versión previa
            Settings = new Dictionary<string, object>(previousConfig.Settings),
            LastModified = DateTime.UtcNow,
            IsValid = true,
            IsRollback = true,
            RollbackReason = "Update validation failed"
        };
        
        return MlResult<Configuration>.Valid(rollbackConfig);
    }

    private async Task<MlResult<Configuration>> CreateSafeRollbackAsync(
        int configId, 
        Configuration previousConfig)
    {
        await Task.Delay(100);
        
        // Validar que la configuración previa es segura para rollback
        if (previousConfig == null || !previousConfig.IsValid)
        {
            throw new InvalidOperationException($"Previous configuration for {configId} is not safe for rollback");
        }
        
        var safeConfig = new Configuration
        {
            Id = previousConfig.Id,
            Version = previousConfig.Version,
            Settings = CreateSafeSettings(previousConfig.Settings),
            LastModified = DateTime.UtcNow,
            IsValid = true,
            IsRollback = true,
            RollbackReason = "Validation failed, rolled back to safe configuration"
        };
        
        return MlResult<Configuration>.Valid(safeConfig);
    }

    private async Task<MlResult<Configuration>> HandleUpdateFailureAsync(
        int configId, 
        Configuration previousConfig, 
        ConfigurationUpdate update)
    {
        await Task.Delay(120);
        
        // Intentar crear configuración híbrida usando configuración previa como base
        try
        {
            var hybridConfig = new Configuration
            {
                Id = previousConfig.Id,
                Version = previousConfig.Version + 1,
                Settings = new Dictionary<string, object>(previousConfig.Settings),
                LastModified = DateTime.UtcNow,
                IsValid = true,
                IsHybrid = true
            };
            
            // Aplicar solo actualizaciones seguras
            foreach (var kvp in update.Settings)
            {
                if (IsSafeSetting(kvp.Key, kvp.Value))
                {
                    hybridConfig.Settings[kvp.Key] = kvp.Value;
                }
            }
            
            return MlResult<Configuration>.Valid(hybridConfig);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to create hybrid configuration for {configId}", ex);
        }
    }

    private Dictionary<string, object> CreateSafeSettings(Dictionary<string, object> originalSettings)
    {
        var safeSettings = new Dictionary<string, object>();
        
        foreach (var kvp in originalSettings)
        {
            if (IsSafeSetting(kvp.Key, kvp.Value))
            {
                safeSettings[kvp.Key] = kvp.Value;
            }
            else
            {
                // Usar valores por defecto seguros
                safeSettings[kvp.Key] = GetDefaultSafeSetting(kvp.Key);
            }
        }
        
        return safeSettings;
    }

    private bool IsSafeSetting(string key, object value)
    {
        return key switch
        {
            "MaxConnections" => value is int i && i > 0 && i <= 1000,
            "Timeout" => value is int t && t > 0 && t <= 300,
            "EnableLogging" => value is bool,
            _ => false
        };
    }

    private object GetDefaultSafeSetting(string key)
    {
        return key switch
        {
            "MaxConnections" => 50,
            "Timeout" => 30,
            "EnableLogging" => true,
            _ => null
        };
    }

    private async Task LogUpdateFailureAsync(int configId, Configuration previousConfig, ConfigurationUpdate update)
    {
        await Task.Delay(10);
        _logger.LogWarning("Configuration update failed for {ConfigId}. Rolling back to version {Version}", 
            configId, previousConfig.Version);
    }

    private async Task LogRollbackActionAsync(int configId, Configuration previousConfig)
    {
        await Task.Delay(10);
        _logger.LogInformation("Rolling back configuration {ConfigId} to version {Version}", 
            configId, previousConfig.Version);
    }
}

// Clases de apoyo
public class Configuration
{
    public int Id { get; set; }
    public int Version { get; set; }
    public Dictionary<string, object> Settings { get; set; } = new();
    public DateTime LastModified { get; set; }
    public bool IsValid { get; set; }
    public bool IsApplied { get; set; }
    public bool IsRollback { get; set; }
    public bool IsHybrid { get; set; }
    public string RollbackReason { get; set; }
}

public class ConfigurationUpdate
{
    public Dictionary<string, object> Settings { get; set; } = new();
    public string UpdateReason { get; set; }
    public string UpdatedBy { get; set; }
}

public interface IConfigurationRepository { }
public interface IValidationService { }
```

### Ejemplo 2: Sistema de Procesamiento de Transacciones Financieras con Compensación

```csharp
public class FinancialTransactionService
{
    private readonly IAccountService _accountService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAuditService _auditService;

    public async Task<MlResult<TransactionResult>> ProcessTransactionAsync(TransactionRequest request)
    {
        return await ValidateTransactionAsync(request)
            .BindAsync(validRequest => GetSourceAccountAsync(validRequest.SourceAccountId))
            .BindAsync(sourceAccount => DebitSourceAccountAsync(sourceAccount, request.Amount))
            .BindIfFailWithValueAsync(async originalBalance =>
            {
                await LogTransactionFailureAsync(request, originalBalance);
                return await CreateCompensationTransactionAsync(request, originalBalance);
            })
            .BindAsync(debitedAccount => GetDestinationAccountAsync(request.DestinationAccountId))
            .BindAsync(destAccount => CreditDestinationAccountAsync(destAccount, request.Amount))
            .BindIfFailWithValueAsync<Account, Account, TransactionResult>(
                validAsync: async creditedAccount => await CompleteTransactionAsync(request, creditedAccount),
                failAsync: async sourceAccountState => await HandleCreditFailureAsync(request, sourceAccountState)
            );
    }

    public async Task<MlResult<TransactionResult>> ProcessTransactionSafelyAsync(TransactionRequest request)
    {
        return await ValidateTransactionAsync(request)
            .BindAsync(validRequest => GetSourceAccountAsync(validRequest.SourceAccountId))
            .TryBindIfFailWithValueAsync(
                funcValueAsync: async originalAccount => await RestoreAccountStateAsync(originalAccount),
                errorMessage: "Failed to restore account state after validation failure"
            )
            .BindAsync(sourceAccount => DebitSourceAccountAsync(sourceAccount, request.Amount))
            .TryBindIfFailWithValueAsync(
                funcValueAsync: async originalBalance => await CreateSafeCompensationAsync(request, originalBalance),
                errorMessageBuilder: ex => $"Failed to create compensation for transaction {request.Id}: {ex.Message}"
            );
    }

    private async Task<MlResult<TransactionRequest>> ValidateTransactionAsync(TransactionRequest request)
    {
        await Task.Delay(50);

        if (request.Amount <= 0)
            return MlResult<TransactionRequest>.Fail("Invalid transaction amount");

        if (request.SourceAccountId == request.DestinationAccountId)
            return MlResult<TransactionRequest>.Fail("Source and destination accounts cannot be the same");

        return MlResult<TransactionRequest>.Valid(request);
    }

    private async Task<MlResult<Account>> GetSourceAccountAsync(int accountId)
    {
        await Task.Delay(100);

        var account = new Account
        {
            Id = accountId,
            Balance = 1000.00m,
            Currency = "USD",
            Status = AccountStatus.Active,
            LastTransaction = DateTime.UtcNow.AddDays(-1)
        };

        if (accountId % 15 == 0)
            return MlResult<Account>.Fail("Source account not found");

        return MlResult<Account>.Valid(account);
    }

    private async Task<MlResult<Account>> DebitSourceAccountAsync(Account account, decimal amount)
    {
        await Task.Delay(150);

        // Almacenar estado original del account para posible rollback
        var originalAccount = new Account
        {
            Id = account.Id,
            Balance = account.Balance,
            Currency = account.Currency,
            Status = account.Status,
            LastTransaction = account.LastTransaction
        };

        if (account.Balance < amount)
        {
            return MlResult<Account>.Fail(
                "Insufficient funds", 
                detailValue: originalAccount);
        }

        if (account.Status != AccountStatus.Active)
        {
            return MlResult<Account>.Fail(
                "Account is not active", 
                detailValue: originalAccount);
        }

        // Simular fallo en el débito
        if (account.Id % 12 == 0)
        {
            return MlResult<Account>.Fail(
                "Debit operation failed", 
                detailValue: originalAccount);
        }

        // Aplicar débito
        account.Balance -= amount;
        account.LastTransaction = DateTime.UtcNow;

        return MlResult<Account>.Valid(account);
    }

    private async Task<MlResult<Account>> GetDestinationAccountAsync(int accountId)
    {
        await Task.Delay(80);

        var account = new Account
        {
            Id = accountId,
            Balance = 500.00m,
            Currency = "USD",
            Status = AccountStatus.Active,
            LastTransaction = DateTime.UtcNow.AddDays(-2)
        };

        if (accountId % 18 == 0)
            return MlResult<Account>.Fail("Destination account not found");

        return MlResult<Account>.Valid(account);
    }

    private async Task<MlResult<Account>> CreditDestinationAccountAsync(Account account, decimal amount)
    {
        await Task.Delay(120);

        if (account.Status != AccountStatus.Active)
        {
            return MlResult<Account>.Fail(
                "Destination account is not active",
                detailValue: account); // Pasar estado de la cuenta fuente (que ya fue debitada)
        }

        // Simular fallo en el crédito
        if (account.Id % 20 == 0)
        {
            return MlResult<Account>.Fail(
                "Credit operation failed",
                detailValue: account);
        }

        // Aplicar crédito
        account.Balance += amount;
        account.LastTransaction = DateTime.UtcNow;

        return MlResult<Account>.Valid(account);
    }

    private async Task<MlResult<TransactionResult>> CreateCompensationTransactionAsync(
        TransactionRequest originalRequest, 
        Account originalSourceAccount)
    {
        await Task.Delay(200);

        // Crear transacción de compensación para revertir el débito
        var compensationTransaction = new TransactionResult
        {
            Id = Guid.NewGuid(),
            OriginalTransactionId = originalRequest.Id,
            Type = TransactionType.Compensation,
            SourceAccountId = originalRequest.SourceAccountId,
            DestinationAccountId = originalRequest.SourceAccountId, // Mismo account
            Amount = originalRequest.Amount,
            Status = TransactionStatus.Completed,
            ProcessedAt = DateTime.UtcNow,
            CompensationReason = "Original transaction failed",
            RestoredBalance = originalSourceAccount.Balance
        };

        await _auditService.LogCompensationAsync(compensationTransaction);

        return MlResult<TransactionResult>.Valid(compensationTransaction);
    }

    private async Task<MlResult<TransactionResult>> HandleCreditFailureAsync(
        TransactionRequest request, 
        Account sourceAccountAfterDebit)
    {
        await Task.Delay(180);

        // El débito ya se aplicó pero el crédito falló - necesitamos compensar
        var compensationTransaction = new TransactionResult
        {
            Id = Guid.NewGuid(),
            OriginalTransactionId = request.Id,
            Type = TransactionType.Reversal,
            SourceAccountId = request.SourceAccountId,
            DestinationAccountId = request.SourceAccountId,
            Amount = request.Amount,
            Status = TransactionStatus.Completed,
            ProcessedAt = DateTime.UtcNow,
            CompensationReason = "Credit to destination failed, reversing debit",
            RestoredBalance = sourceAccountAfterDebit.Balance + request.Amount // Restaurar balance original
        };

        // Restaurar balance de la cuenta fuente
        await _accountService.CreditAccountAsync(sourceAccountAfterDebit.Id, request.Amount);

        return MlResult<TransactionResult>.Valid(compensationTransaction);
    }

    private async Task<MlResult<TransactionResult>> CompleteTransactionAsync(
        TransactionRequest request, 
        Account creditedAccount)
    {
        await Task.Delay(100);

        var result = new TransactionResult
        {
            Id = Guid.NewGuid(),
            OriginalTransactionId = request.Id,
            Type = TransactionType.Transfer,
            SourceAccountId = request.SourceAccountId,
            DestinationAccountId = request.DestinationAccountId,
            Amount = request.Amount,
            Status = TransactionStatus.Completed,
            ProcessedAt = DateTime.UtcNow,
            FinalDestinationBalance = creditedAccount.Balance
        };

        return MlResult<TransactionResult>.Valid(result);
    }

    private async Task<MlResult<Account>> RestoreAccountStateAsync(Account originalAccount)
    {
        await Task.Delay(75);

        // Simular operación de restauración que puede fallar
        if (originalAccount.Id % 25 == 0)
            throw new InvalidOperationException($"Cannot restore account {originalAccount.Id} - system lock");

        var restoredAccount = new Account
        {
            Id = originalAccount.Id,
            Balance = originalAccount.Balance,
            Currency = originalAccount.Currency,
            Status = originalAccount.Status,
            LastTransaction = originalAccount.LastTransaction,
            IsRestored = true,
            RestoreTimestamp = DateTime.UtcNow
        };

        return MlResult<Account>.Valid(restoredAccount);
    }

    private async Task<MlResult<TransactionResult>> CreateSafeCompensationAsync(
        TransactionRequest request, 
        Account originalAccount)
    {
        await Task.Delay(150);

        // Operación de compensación que puede fallar de forma controlada
        if (originalAccount.Balance < 0)
            throw new InvalidOperationException("Cannot create compensation for negative balance account");

        var safeCompensation = new TransactionResult
        {
            Id = Guid.NewGuid(),
            OriginalTransactionId = request.Id,
            Type = TransactionType.SafeCompensation,
            SourceAccountId = request.SourceAccountId,
            DestinationAccountId = request.SourceAccountId,
            Amount = request.Amount,
            Status = TransactionStatus.Completed,
            ProcessedAt = DateTime.UtcNow,
            CompensationReason = "Safe compensation after controlled failure",
            RestoredBalance = originalAccount.Balance,
            IsSafeOperation = true
        };

        return MlResult<TransactionResult>.Valid(safeCompensation);
    }

    private async Task LogTransactionFailureAsync(TransactionRequest request, Account originalBalance)
    {
        await Task.Delay(25);
        // Log the failure with original balance context
    }
}

// Clases de apoyo para transacciones financieras
public enum AccountStatus
{
    Active,
    Suspended,
    Closed
}

public enum TransactionType
{
    Transfer,
    Compensation,
    Reversal,
    SafeCompensation
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Failed,
    Compensated
}

public class Account
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public AccountStatus Status { get; set; }
    public DateTime LastTransaction { get; set; }
    public bool IsRestored { get; set; }
    public DateTime? RestoreTimestamp { get; set; }
}

public class TransactionRequest
{
    public int Id { get; set; }
    public int SourceAccountId { get; set; }
    public int DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class TransactionResult
{
    public Guid Id { get; set; }
    public int OriginalTransactionId { get; set; }
    public TransactionType Type { get; set; }
    public int SourceAccountId { get; set; }
    public int DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string CompensationReason { get; set; }
    public decimal? RestoredBalance { get; set; }
    public decimal? FinalDestinationBalance { get; set; }
    public bool IsSafeOperation { get; set; }
}

public interface IAccountService
{
    Task CreditAccountAsync(int accountId, decimal amount);
}

public interface ITransactionRepository { }

public interface IAuditService
{
    Task LogCompensationAsync(TransactionResult compensation);
}
```

### Ejemplo 3: Sistema de Migración de Datos con Checkpoint y Rollback

```csharp
public class DataMigrationService
{
    public async Task<MlResult<MigrationResult>> MigrateDataAsync(MigrationRequest request)
    {
        return await PrepareSourceDataAsync(request)
            .BindAsync(sourceData => CreateCheckpointAsync(sourceData))
            .BindAsync(checkpoint => TransformDataAsync(checkpoint))
            .BindIfFailWithValueAsync(async originalCheckpoint =>
            {
                await LogTransformationFailureAsync(request, originalCheckpoint);
                return await RollbackToCheckpointAsync(originalCheckpoint);
            })
            .BindAsync(transformedData => ValidateTransformedDataAsync(transformedData))
            .BindIfFailWithValueAsync<TransformedData, Checkpoint, MigrationResult>(
                validAsync: async data => await CompleteMigrationAsync(data),
                failAsync: async checkpoint => await HandleValidationFailureAsync(checkpoint)
            );
    }

    public async Task<MlResult<MigrationResult>> MigrateDataSafelyAsync(MigrationRequest request)
    {
        return await PrepareSourceDataAsync(request)
            .BindAsync(sourceData => CreateCheckpointAsync(sourceData))
            .TryBindIfFailWithValueAsync(
                funcValueAsync: async originalData => await CreateEmergencyCheckpointAsync(originalData),
                errorMessage: "Failed to create emergency checkpoint"
            )
            .BindAsync(checkpoint => TransformDataAsync(checkpoint))
            .TryBindIfFailWithValueAsync(
                funcValueAsync: async originalCheckpoint => await SafeRollbackAsync(originalCheckpoint),
                errorMessageBuilder: ex => $"Safe rollback failed for migration {request.Id}: {ex.Message}"
            );
    }

    private async Task<MlResult<SourceData>> PrepareSourceDataAsync(MigrationRequest request)
    {
        await Task.Delay(200);

        var sourceData = new SourceData
        {
            MigrationId = request.Id,
            RecordCount = request.RecordCount,
            SourceFormat = request.SourceFormat,
            Data = GenerateSourceData(request.RecordCount),
            PreparedAt = DateTime.UtcNow
        };

        if (request.RecordCount <= 0)
            return MlResult<SourceData>.Fail("Invalid record count");

        return MlResult<SourceData>.Valid(sourceData);
    }

    private async Task<MlResult<Checkpoint>> CreateCheckpointAsync(SourceData sourceData)
    {
        await Task.Delay(150);

        var checkpoint = new Checkpoint
        {
            Id = Guid.NewGuid(),
            MigrationId = sourceData.MigrationId,
            OriginalData = sourceData,
            CreatedAt = DateTime.UtcNow,
            RecordCount = sourceData.RecordCount,
            CheckpointType = CheckpointType.Initial
        };

        // Simular fallo en creación de checkpoint
        if (sourceData.MigrationId % 10 == 0)
        {
            return MlResult<Checkpoint>.Fail(
                "Failed to create checkpoint",
                detailValue: sourceData);
        }

        return MlResult<Checkpoint>.Valid(checkpoint);
    }

    private async Task<MlResult<TransformedData>> TransformDataAsync(Checkpoint checkpoint)
    {
        await Task.Delay(300);

        // Simular transformación que puede fallar
        if (checkpoint.OriginalData.RecordCount > 10000)
        {
            return MlResult<TransformedData>.Fail(
                "Transformation failed - dataset too large",
                detailValue: checkpoint);
        }

        if (checkpoint.OriginalData.SourceFormat == "INVALID")
        {
            return MlResult<TransformedData>.Fail(
                "Unsupported source format",
                detailValue: checkpoint);
        }

        var transformedData = new TransformedData
        {
            MigrationId = checkpoint.MigrationId,
            SourceCheckpoint = checkpoint,
            TransformedRecords = TransformRecords(checkpoint.OriginalData.Data),
            TransformedAt = DateTime.UtcNow,
            TransformationRules = GetTransformationRules(checkpoint.OriginalData.SourceFormat)
        };

        return MlResult<TransformedData>.Valid(transformedData);
    }

    private async Task<MlResult<TransformedData>> ValidateTransformedDataAsync(TransformedData data)
    {
        await Task.Delay(100);

        // Simular validación que puede fallar
        if (data.TransformedRecords.Count() != data.SourceCheckpoint.RecordCount)
        {
            return MlResult<TransformedData>.Fail(
                "Record count mismatch after transformation",
                detailValue: data.SourceCheckpoint);
        }

        if (data.TransformedRecords.Any(r => r.IsCorrupted))
        {
            return MlResult<TransformedData>.Fail(
                "Corrupted records detected",
                detailValue: data.SourceCheckpoint);
        }

        return MlResult<TransformedData>.Valid(data);
    }

    private async Task<MlResult<MigrationResult>> RollbackToCheckpointAsync(Checkpoint originalCheckpoint)
    {
        await Task.Delay(250);

        var rollbackResult = new MigrationResult
        {
            MigrationId = originalCheckpoint.MigrationId,
            Status = MigrationStatus.RolledBack,
            CompletedAt = DateTime.UtcNow,
            RecordsProcessed = 0,
            RecordsRolledBack = originalCheckpoint.RecordCount,
            RollbackReason = "Transformation failed, rolled back to checkpoint",
            CheckpointUsed = originalCheckpoint,
            IsRollback = true
        };

        return MlResult<MigrationResult>.Valid(rollbackResult);
    }

    private async Task<MlResult<MigrationResult>> HandleValidationFailureAsync(Checkpoint checkpoint)
    {
        await Task.Delay(200);

        // Intentar recuperación parcial usando datos del checkpoint
        var partialResult = new MigrationResult
        {
            MigrationId = checkpoint.MigrationId,
            Status = MigrationStatus.PartiallyCompleted,
            CompletedAt = DateTime.UtcNow,
            RecordsProcessed = checkpoint.RecordCount / 2, // Procesar solo la mitad
            RecordsSkipped = checkpoint.RecordCount / 2,
            RollbackReason = "Validation failed, completed partial migration",
            CheckpointUsed = checkpoint,
            IsPartial = true
        };

        return MlResult<MigrationResult>.Valid(partialResult);
    }

    private async Task<MlResult<MigrationResult>> CompleteMigrationAsync(TransformedData data)
    {
        await Task.Delay(400);

        var result = new MigrationResult
        {
            MigrationId = data.MigrationId,
            Status = MigrationStatus.Completed,
            CompletedAt = DateTime.UtcNow,
            RecordsProcessed = data.TransformedRecords.Count(),
            CheckpointUsed = data.SourceCheckpoint,
            TransformationRules = data.TransformationRules
        };

        return MlResult<MigrationResult>.Valid(result);
    }

    private async Task<MlResult<Checkpoint>> CreateEmergencyCheckpointAsync(SourceData originalData)
    {
        await Task.Delay(100);

        // Operación que puede fallar de forma controlada
        if (originalData.RecordCount > 50000)
            throw new OutOfMemoryException("Cannot create emergency checkpoint for large dataset");

        var emergencyCheckpoint = new Checkpoint
        {
            Id = Guid.NewGuid(),
            MigrationId = originalData.MigrationId,
            OriginalData = originalData,
            CreatedAt = DateTime.UtcNow,
            RecordCount = originalData.RecordCount,
            CheckpointType = CheckpointType.Emergency,
            IsEmergency = true
        };

        return MlResult<Checkpoint>.Valid(emergencyCheckpoint);
    }

    private async Task<MlResult<MigrationResult>> SafeRollbackAsync(Checkpoint originalCheckpoint)
    {
        await Task.Delay(180);

        // Rollback seguro que puede fallar
        if (originalCheckpoint.CheckpointType == CheckpointType.Emergency)
            throw new InvalidOperationException("Cannot perform safe rollback on emergency checkpoint");

        var safeRollback = new MigrationResult
        {
            MigrationId = originalCheckpoint.MigrationId,
            Status = MigrationStatus.SafelyRolledBack,
            CompletedAt = DateTime.UtcNow,
            RecordsProcessed = 0,
            RecordsRolledBack = originalCheckpoint.RecordCount,
            RollbackReason = "Safe rollback performed",
            CheckpointUsed = originalCheckpoint,
            IsRollback = true,
            IsSafeRollback = true
        };

        return MlResult<MigrationResult>.Valid(safeRollback);
    }

    private async Task LogTransformationFailureAsync(MigrationRequest request, Checkpoint originalCheckpoint)
    {
        await Task.Delay(20);
        // Log transformation failure with checkpoint context
    }

    // Métodos auxiliares
    private IEnumerable<DataRecord> GenerateSourceData(int count)
    {
        return Enumerable.Range(1, count).Select(i => new DataRecord
        {
            Id = i,
            Data = $"SourceData_{i}",
            IsCorrupted = i % 100 == 0 // 1% de registros corruptos
        });
    }

    private IEnumerable<DataRecord> TransformRecords(IEnumerable<DataRecord> sourceRecords)
    {
        return sourceRecords.Select(r => new DataRecord
        {
            Id = r.Id,
            Data = $"Transformed_{r.Data}",
            IsCorrupted = r.IsCorrupted
        });
    }

    private List<string> GetTransformationRules(string sourceFormat)
    {
        return new List<string> { $"Rule1_For_{sourceFormat}", $"Rule2_For_{sourceFormat}" };
    }
}

// Clases de apoyo para migración de datos
public enum CheckpointType
{
    Initial,
    Intermediate,
    Emergency
}

public enum MigrationStatus
{
    InProgress,
    Completed,
    Failed,
    RolledBack,
    PartiallyCompleted,
    SafelyRolledBack
}

public class MigrationRequest
{
    public int Id { get; set; }
    public int RecordCount { get; set; }
    public string SourceFormat { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class SourceData
{
    public int MigrationId { get; set; }
    public int RecordCount { get; set; }
    public string SourceFormat { get; set; }
    public IEnumerable<DataRecord> Data { get; set; }
    public DateTime PreparedAt { get; set; }
}

public class Checkpoint
{
    public Guid Id { get; set; }
    public int MigrationId { get; set; }
    public SourceData OriginalData { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RecordCount { get; set; }
    public CheckpointType CheckpointType { get; set; }
    public bool IsEmergency { get; set; }
}

public class TransformedData
{
    public int MigrationId { get; set; }
    public Checkpoint SourceCheckpoint { get; set; }
    public IEnumerable<DataRecord> TransformedRecords { get; set; }
    public DateTime TransformedAt { get; set; }
    public List<string> TransformationRules { get; set; }
}

public class MigrationResult
{
    public int MigrationId { get; set; }
    public MigrationStatus Status { get; set; }
    public DateTime CompletedAt { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsRolledBack { get; set; }
    public int RecordsSkipped { get; set; }
    public string RollbackReason { get; set; }
    public Checkpoint CheckpointUsed { get; set; }
    public List<string> TransformationRules { get; set; }
    public bool IsRollback { get; set; }
    public bool IsPartial { get; set; }
    public bool IsSafeRollback { get; set; }
}

public class DataRecord
{
    public int Id { get; set; }
    public string Data { get; set; }
    public bool IsCorrupted { get; set; }
}
```

---

## Mejores Prácticas

### 1. Gestión Efectiva de Valores Previos

```csharp
// ✅ Correcto: Almacenar estado completo necesario para recuperación
var result = await UpdateUserProfile(userId, newProfile)
    .BindIfFailWithValueAsync(async originalProfile =>
    {
        // originalProfile contiene el estado antes del fallo
        await LogProfileUpdateFailure(userId, originalProfile, newProfile);
        return await RestoreUserProfile(userId, originalProfile);
    });

// ✅ Correcto: Verificar existencia de valor previo
var result = await riskyOperation
    .BindIfFailWithValueAsync(async previousValue =>
    {
        if (previousValue == null)
        {
            return await CreateDefaultRecovery();
        }
        
        return await RecoverUsingPreviousValue(previousValue);
    });

// ❌ Incorrecto: Asumir que siempre hay valor previo
var result = await operation
    .BindIfFailWithValue(prevValue => 
        ProcessRecovery(prevValue)); // prevValue puede ser null
```

### 2. Creación de Checkpoints Estratégicos

```csharp
// ✅ Correcto: Crear checkpoints en puntos críticos
public async Task<MlResult<ProcessedData>> ProcessDataWithCheckpointsAsync(RawData data)
{
    return await ValidateRawData(data)
        .BindAsync(validData => CreateInitialCheckpoint(validData)) // Checkpoint después de validación
        .BindAsync(checkpoint => PerformTransformation(checkpoint))
        .BindIfFailWithValueAsync(async originalCheckpoint =>
        {
            await LogTransformationFailure(originalCheckpoint);
            return await RollbackToCheckpoint(originalCheckpoint);
        })
        .BindAsync(transformedData => CreateTransformationCheckpoint(transformedData)) // Checkpoint después de transformación
        .BindAsync(checkpoint => ApplyBusinessRules(checkpoint))
        .BindIfFailWithValueAsync(async transformationCheckpoint =>
        {
            return await RollbackToTransformationState(transformationCheckpoint);
        });
}

// ✅ Correcto: Checkpoints con información contextual
private async Task<MlResult<Checkpoint>> CreateCheckpoint<T>(T data, string phase)
{
    var checkpoint = new Checkpoint
    {
        Data = data,
        Phase = phase,
        Timestamp = DateTime.UtcNow,
        ProcessId = GetCurrentProcessId(),
        ContextInfo = GetCurrentContext()
    };
    
    return await SaveCheckpoint(checkpoint);
}
```

### 3. Estrategias de Compensación Inteligente

```csharp
// ✅ Correcto: Compensación basada en el tipo de operación
public async Task<MlResult<OperationResult>> PerformCompensatableOperationAsync(OperationRequest request)
{
    return await ExecuteOperation(request)
        .BindIfFailWithValueAsync(async previousState =>
        {
            var compensationType = DetermineCompensationType(request, previousState);
            
            return compensationType switch
            {
                CompensationType.FullRollback => await PerformFullRollback(previousState),
                CompensationType.PartialRollback => await PerformPartialRollback(previousState, request),
                CompensationType.ForwardRecovery => await PerformForwardRecovery(previousState, request),
                _ => await CreateCompensationRecord(previousState, request)
            };
        });
}

// ✅ Correcto: Compensación con validación
private async Task<MlResult<CompensationResult>> CreateCompensation(
    OriginalState previousState, 
    OperationRequest failedRequest)
{
    // Validar que la compensación es segura
    var validationResult = await ValidateCompensationSafety(previousState, failedRequest);
    if (!validationResult.IsSuccess)
    {
        return MlResult<CompensationResult>.Fail(
            $"Compensation not safe: {validationResult.ErrorMessage}");
    }
    
    // Crear y aplicar compensación
    var compensation = CreateCompensationFromState(previousState);
    return await ApplyCompensation(compensation);
}
```

### 4. Manejo de Tipos Múltiples

```csharp
// ✅ Correcto: Manejo claro de tipos diferentes para valor actual y previo
public async Task<MlResult<FinalResult>> ProcessWithDifferentTypesAsync(InputData input)
{
    return await ConvertToIntermediate(input)
        .BindAsync(intermediate => ProcessIntermediate(intermediate))
        .BindIfFailWithValueAsync<ProcessedData, IntermediateData, FinalResult>(
            validAsync: async processed => await CreateFinalResult(processed),
            failAsync: async intermediateState => 
            {
                // intermediateState es de tipo IntermediateData
                // processed sería de tipo ProcessedData si hubiera éxito
                return await RecoverFromIntermediateState(intermediateState);
            }
        );
}

// ✅ Correcto: Documentar tipos claramente
/// <summary>
/// Procesa datos con recuperación basada en estado previo
/// </summary>
/// <typeparam name="TInput">Tipo de datos de entrada</typeparam>
/// <typeparam name="TPrevious">Tipo del valor previo almacenado en error</typeparam>
/// <typeparam name="TOutput">Tipo de resultado final</typeparam>
public async Task<MlResult<TOutput>> ProcessWithRecoveryAsync<TInput, TPrevious, TOutput>(
    TInput input,
    Func<TInput, MlResult<TOutput>> processor,
    Func<TPrevious, MlResult<TOutput>> recoveryFunc)
{
    return await processor(input)
        .BindIfFailWithValueAsync<TOutput, TPrevious, TOutput>(
            validAsync: async result => result.ToMlResultValidAsync(),
            failAsync: async previousValue => await recoveryFunc(previousValue).ToAsync()
        );
}
```

### 5. Testing de Recuperación con Valores Previos

```csharp
// ✅ Correcto: Testing completo de scenarios de recuperación
[Test]
public async Task BindIfFailWithValue_HasPreviousValue_UsesForRecovery()
{
    // Arrange
    var originalData = CreateTestData();
    var failingResult = MlResult<ProcessedData>.Fail("Processing failed", detailValue: originalData);
    
    // Act
    var result = await failingResult
        .BindIfFailWithValueAsync(async previousValue =>
        {
            return await CreateRecoveryFromPrevious(previousValue);
        });
    
    // Assert
    result.Should().BeSuccessful();
    result.Value.IsRecovery.Should().BeTrue();
    result.Value.OriginalData.Should().Be(originalData);
}

[Test]
public async Task BindIfFailWithValue_NoPreviousValue_HandlesGracefully()
{
    // Arrange
    var failingResult = MlResult<ProcessedData>.Fail("Processing failed"); // Sin valor previo
    
    // Act
    var result = await failingResult
        .BindIfFailWithValueAsync(async previousValue =>
        {
            // previousValue será null o default
            return await CreateDefaultRecovery();
        });
    
    // Assert
    result.Should().BeSuccessful();
    result.Value.IsDefaultRecovery.Should().BeTrue();
}

[Test]
public async Task TryBindIfFailWithValue_RecoveryThrows_HandlesException()
{
    // Arrange
    var originalData = CreateTestData();
    var failingResult = MlResult<ProcessedData>.Fail("Processing failed", detailValue: originalData);
    
    // Act
    var result = await failingResult
        .TryBindIfFailWithValueAsync(
            funcValueAsync: async previousValue => throw new Exception("Recovery failed"),
            errorMessage: "Recovery operation failed"
        );
    
    // Assert
    result.Should().BeFailure();
    result.ErrorsDetails.GetMessage().Should().Contain("Recovery operation failed");
}

[Test]
public async Task BindIfFailWithValue_WithTypeTransformation_PreservesTypes()
{
    // Arrange
    var inputData = new InputData { Value = "test" };
    var intermediateResult = MlResult<IntermediateData>.Fail("Transform failed", detailValue: inputData);
    
    // Act
    var result = await intermediateResult
        .BindIfFailWithValueAsync<IntermediateData, InputData, FinalData>(
            validAsync: async intermediate => await ProcessIntermediate(intermediate),
            failAsync: async original => await RecoverFromOriginal(original)
        );
    
    // Assert
    result.Should().BeSuccessful();
    result.Value.Should().BeOfType<FinalData>();
    result.Value.SourceType.Should().Be("InputData");
}
```

---

## Consideraciones de Rendimiento y Memoria

### Gestión de Memoria con Valores Previos

```csharp
// ⚠️ Considerar: Valores previos grandes pueden consumir memoria
public class LargeDataProcessor
{
    // ✅ Mejor: Almacenar solo información esencial para recuperación
    public async Task<MlResult<ProcessedLargeData>> ProcessLargeDataAsync(LargeData data)
    {
        // En lugar de almacenar todo el objeto grande
        var essentialInfo = ExtractEssentialInfo(data);
        
        return await ProcessData(data)
            .BindAsync(result => ValidateResult(result, essentialInfo))
            .BindIfFailWithValueAsync(async previousEssentials =>
            {
                // Usar solo la información esencial para recuperación
                return await RecoverUsingEssentials(previousEssentials);
            });
    }
    
    private EssentialInfo ExtractEssentialInfo(LargeData data)
    {
        return new EssentialInfo
        {
            Id = data.Id,
            Version = data.Version,
            Checksum = data.CalculateChecksum(),
            Key = data.Key
        };
    }
}

// ✅ Patrón para datos grandes: Referencias en lugar de valores completos
public class DataReferenceRecovery
{
    public async Task<MlResult<ProcessedData>> ProcessWithReferenceRecoveryAsync(DataId dataId)
    {
        var data = await LoadData(dataId);
        
        return await data
            .BindAsync(loadedData => ProcessData(loadedData))
            .BindIfFailWithValueAsync(async dataReference =>
            {
                // Recargar datos usando referencia en lugar de almacenar datos completos
                var reloadedData = await ReloadData(dataReference);
                return await AttemptRecovery(reloadedData);
            });
    }
}
```

### Optimización de Checkpoints

```csharp
// ✅ Checkpoints optimizados para rendimiento
public class OptimizedCheckpointService
{
    private readonly ICheckpointStorage _storage;
    
    public async Task<MlResult<ProcessedData>> ProcessWithOptimizedCheckpointsAsync(InputData input)
    {
        return await CreateCheckpoint(input)
            .BindAsync(checkpoint => ProcessData(checkpoint))
            .BindIfFailWithValueAsync(async originalCheckpoint =>
            {
                // Cleanup de checkpoints antiguos durante recuperación
                _ = Task.Run(() => CleanupOldCheckpoints(originalCheckpoint.ProcessId));
                
                return await RecoverFromCheckpoint(originalCheckpoint);
            })
            .DoAsync(result => CleanupCurrentCheckpoint(input.ProcessId)); // Cleanup en éxito también
    }
    
    private async Task CleanupOldCheckpoints(string processId)
    {
        var oldCheckpoints = await _storage.GetOldCheckpoints(processId, TimeSpan.FromHours(1));
        await _storage.DeleteCheckpoints(oldCheckpoints);
    }
}
```

---

## Resumen

Las operaciones **BindIfFailWithValue** proporcionan capacidades avanzadas de recuperación contextual:

### Variantes Principales

1. **BindIfFailWithValue Simple**: Recuperación usando valor previo del mismo tipo
2. **BindIfFailWithValue con Transformación**: Manejo completo con soporte para diferentes tipos
3. **TryBindIfFailWithValue**: Versiones seguras que protegen las funciones de recuperación

### Características Clave

- **Acceso a Valor Previo**: Funciones de recuperación reciben el último valor válido conocido
- **Recuperación Contextual**: Estrategias de recuperación informadas por el estado previo
- **Flexibilidad de Tipos**: Soporte para diferentes tipos entre valor actual y valor previo
- **Soporte Asíncrono Completo**: Todas las variaciones asíncronas disponibles

### Casos de Uso Ideales

- **Rollback de Transacciones**: Vuelta a estado previo en operaciones financieras
- **Checkpoints de Procesamiento**: Puntos de recuperación en procesos largos
- **Compensación de Errores**: Uso de estado previo para crear compensaciones
- **Migración de Datos**: Recuperación usando snapshots de datos previos
- **Versionado de Estados**: Manejo de versiones con capacidad de rollback

### Ventajas sobre BindIfFail Simple

- **Información Contextual**: Acceso al valor que causó o precedió al error
- **Recuperación Inteligente**: Decisiones de recuperación basadas en datos reales
- **Continuidad de Operaciones**: Capacidad de continuar desde un punto conocido
- **Auditabilidad**: Mejor trazabilidad de qué se recuperó y cómo

Las operaciones BindIfFailWithValue son especialmente poderosas en sistemas que requieren alta disponibilidad y recuperación automática, proporcionando mecanismos sofisticados para manejar fallos mientras mantienen la