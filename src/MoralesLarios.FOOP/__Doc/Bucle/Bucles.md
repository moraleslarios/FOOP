# MlResult Bucles - Completado y Fusión de Datos en Colecciones

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos Bucles](#métodos-Bucles)
4. [Métodos de Fusión de Errores](#métodos-de-fusión-de-errores)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Métodos Similares](#comparación-con-métodos-similares)

---

## Introducción

Los métodos `CompleteData` y las funciones de fusión de errores proporcionan un sistema robusto para **procesar colecciones de datos** donde cada elemento puede requerir transformación o validación que puede fallar. Estos métodos implementan el patrón "**todo o nada**" donde una colección completa es válida solo si todos sus elementos son procesados exitosamente.

### Propósito Principal

- **Completado de Datos**: Transformar/enriquecer elementos de una colección
- **Validación Colectiva**: Asegurar que todos los elementos cumplan criterios
- **Fusión de Errores**: Combinar errores de múltiples operaciones fallidas
- **Procesamiento Seguro**: Manejar fallos individuales sin interrumpir el proceso completo

---

## Análisis de los Métodos

### Filosofía de CompleteData

```
IEnumerable<T> + Transform → CompleteData → MlResult<IEnumerable<T/TResult>>
       ↓              ↓           ↓                ↓
   [item1, item2] + func → [func(item1), func(item2)]
       ↓              ↓           ↓                ↓
   Si todos válidos → Valid([result1, result2])
       ↓              ↓           ↓                ↓
   Si alguno falla → Fail(fusioned_errors)
```

### Características Principales

1. **Aplicación de Función**: Aplica una función de transformación a cada elemento
2. **Validación Todo-o-Nada**: Falla si cualquier elemento falla
3. **Fusión de Errores**: Combina todos los errores en un solo resultado
4. **Preservación de Tipos**: Mantiene tipado fuerte en transformaciones
5. **Soporte Asíncrono**: Versiones completas async/await

---

## Métodos CompleteData

### 1. CompleteData - Transformación del Mismo Tipo

**Propósito**: Procesar elementos manteniendo el tipo original

```csharp
public static MlResult<IEnumerable<T>> CompleteData<T>(
    this IEnumerable<T> source,
    Func<T, MlResult<T>> completeFuncTransform)
```

**Funcionamiento Interno**:
```csharp
var result = source.ToMlResultValid()
    .Bind(x =>
    {
        var partialData = x.Select(completeFuncTransform).ToList();
        
        var result = partialData.Any(x => x.IsFail) ?
                     FusionFailErros(partialData) :
                     MlResult<IEnumerable<T>>.Valid(partialData.Select(x => x.Value));
        
        return result;
    });
```

### 2. CompleteData - Transformación a Tipo Diferente

**Propósito**: Transformar elementos a un tipo diferente

```csharp
public static MlResult<IEnumerable<TResult>> CompleteData<T, TResult>(
    this IEnumerable<T> source,
    Func<T, MlResult<TResult>> completeFuncTransform)
```

### 3. CompleteDataAsync - Variantes Asíncronas

**Múltiples sobrecargas para diferentes escenarios async**:

```csharp
// Función síncrona con resultado async
public static Task<MlResult<IEnumerable<T>>> CompleteDataAsync<T>(
    this IEnumerable<T> source,
    Func<T, MlResult<T>> completeFuncTransform)

// Colección async con función async
public static async Task<MlResult<IEnumerable<T>>> CompleteDataAsync<T>(
    this Task<IEnumerable<T>> sourceAsync,
    Func<T, Task<MlResult<T>>> completeFuncTransformAsync)

// Colección síncrona con función async
public static async Task<MlResult<IEnumerable<T>>> CompleteDataAsync<T>(
    this IEnumerable<T> source,
    Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
```

---

## Métodos de Fusión de Errores

### 1. FusionFailErros

**Propósito**: Fusionar errores de elementos fallidos (requiere al menos un error)

```csharp
public static MlResult<IEnumerable<T>> FusionFailErros<T>(
    this IEnumerable<MlResult<T>> source)
```

**Comportamiento**:
- **Si no hay errores**: Retorna error (requiere elementos fallidos)
- **Si hay errores**: Fusiona todos los errores en un `MlErrorsDetails`

### 2. FusionErrosIfExists

**Propósito**: Fusionar errores si existen, sino retornar valores válidos

```csharp
public static MlResult<IEnumerable<T>> FusionErrosIfExists<T>(
    this IEnumerable<MlResult<T>> source)
```

**Comportamiento**:
- **Si no hay errores**: Retorna `Valid` con todos los valores
- **Si hay errores**: Fusiona errores e ignora valores válidos

---

## Variantes Asíncronas

### Soporte Completo Async/Await

```csharp
// Para FusionFailErros
public static Task<MlResult<IEnumerable<T>>> FusionFailErrosAsync<T>(
    this IEnumerable<MlResult<T>> source)

public static async Task<MlResult<IEnumerable<T>>> FusionFailErrosAsync<T>(
    this Task<IEnumerable<MlResult<T>>> sourceAsync)

// Para FusionErrosIfExists
public static Task<MlResult<IEnumerable<T>>> FusionErrosIfExistsAsync<T>(
    this IEnumerable<MlResult<T>> source)

public static async Task<MlResult<IEnumerable<T>>> FusionErrosIfExistsAsync<T>(
    this Task<IEnumerable<MlResult<T>>> sourceAsync)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Validación de Usuarios en Lote

```csharp
public class UserBatchValidationService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IValidationService _validationService;
    
    public async Task<MlResult<IEnumerable<ValidatedUser>>> ValidateUserBatchAsync(
        IEnumerable<UserRegistrationRequest> requests)
    {
        // Completar datos de usuarios aplicando validaciones completas
        var validationResult = await requests.CompleteDataAsync(async request =>
        {
            // Validar formato de datos
            var formatValidation = await ValidateUserFormatAsync(request);
            if (formatValidation.IsFailed)
                return formatValidation.ToMlResultFail<ValidatedUser>();
            
            // Verificar duplicados
            var duplicateValidation = await CheckDuplicateUserAsync(request.Email);
            if (duplicateValidation.IsFailed)
                return duplicateValidation.ToMlResultFail<ValidatedUser>();
            
            // Validar dominio de email
            var emailDomainValidation = await ValidateEmailDomainAsync(request.Email);
            if (emailDomainValidation.IsFailed)
                return emailDomainValidation.ToMlResultFail<ValidatedUser>();
            
            // Crear usuario validado
            return MlResult<ValidatedUser>.Valid(new ValidatedUser
            {
                Email = request.Email,
                Name = request.Name,
                Phone = request.Phone,
                ValidationId = Guid.NewGuid(),
                ValidatedAt = DateTime.UtcNow,
                Status = "Validated"
            });
        });
        
        return validationResult;
    }
    
    public async Task<MlResult<IEnumerable<EnrichedUserProfile>>> EnrichUserProfilesAsync(
        IEnumerable<int> userIds)
    {
        // Enriquecer perfiles de usuario con datos adicionales
        return await userIds.CompleteDataAsync(async userId =>
        {
            try
            {
                // Obtener datos básicos del usuario
                var basicUser = await _userRepository.GetByIdAsync(userId);
                if (basicUser == null)
                    return MlResult<EnrichedUserProfile>.Fail($"User {userId} not found");
                
                // Enriquecer con datos de perfil
                var profileData = await GetUserProfileDataAsync(userId);
                var preferences = await GetUserPreferencesAsync(userId);
                var activitySummary = await GetUserActivitySummaryAsync(userId);
                var socialConnections = await GetUserSocialConnectionsAsync(userId);
                
                // Combinar todos los datos
                var enrichedProfile = new EnrichedUserProfile
                {
                    UserId = userId,
                    BasicInfo = basicUser,
                    ProfileData = profileData,
                    Preferences = preferences,
                    ActivitySummary = activitySummary,
                    SocialConnections = socialConnections,
                    EnrichmentTimestamp = DateTime.UtcNow,
                    DataSources = new[] { "UserRepo", "ProfileService", "PreferencesService", "ActivityService", "SocialService" }
                };
                
                return MlResult<EnrichedUserProfile>.Valid(enrichedProfile);
            }
            catch (Exception ex)
            {
                return MlResult<EnrichedUserProfile>.Fail($"Failed to enrich profile for user {userId}: {ex.Message}");
            }
        });
    }
    
    public async Task<MlResult<IEnumerable<ProcessedDocument>>> ProcessDocumentsAsync(
        IEnumerable<DocumentUploadRequest> documents)
    {
        // Procesar documentos con validación y transformación
        var processingResult = await documents.CompleteDataAsync(async doc =>
        {
            // Validar tamaño del archivo
            if (doc.FileSize > 10 * 1024 * 1024) // 10MB
                return MlResult<ProcessedDocument>.Fail($"File {doc.FileName} exceeds size limit");
            
            // Validar tipo de archivo
            var allowedTypes = new[] { ".pdf", ".docx", ".txt", ".jpg", ".png" };
            var extension = Path.GetExtension(doc.FileName);
            if (!allowedTypes.Contains(extension.ToLower()))
                return MlResult<ProcessedDocument>.Fail($"File type {extension} not allowed");
            
            // Escanear virus
            var virusScanResult = await ScanForVirusAsync(doc.FileContent);
            if (!virusScanResult.IsClean)
                return MlResult<ProcessedDocument>.Fail($"Virus detected in {doc.FileName}");
            
            // Procesar contenido
            var contentAnalysis = await AnalyzeDocumentContentAsync(doc.FileContent);
            var metadata = await ExtractMetadataAsync(doc.FileContent);
            var thumbnails = await GenerateThumbnailsAsync(doc.FileContent);
            
            // Almacenar en sistema de archivos seguro
            var storageLocation = await StoreDocumentSecurelyAsync(doc);
            
            return MlResult<ProcessedDocument>.Valid(new ProcessedDocument
            {
                OriginalFileName = doc.FileName,
                StorageLocation = storageLocation,
                ContentAnalysis = contentAnalysis,
                Metadata = metadata,
                Thumbnails = thumbnails,
                ProcessedAt = DateTime.UtcNow,
                FileSize = doc.FileSize,
                FileType = extension,
                ProcessingId = Guid.NewGuid()
            });
        });
        
        return processingResult;
    }
    
    public async Task<MlResult<BatchProcessingReport>> ProcessUserDataBatchWithReportAsync(
        IEnumerable<UserDataRequest> requests)
    {
        var batchId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        
        // Procesar datos y capturar tanto éxitos como fallos
        var individualResults = new List<MlResult<ProcessedUserData>>();
        
        foreach (var request in requests)
        {
            var result = await ProcessSingleUserDataAsync(request);
            individualResults.Add(result);
        }
        
        // Separar éxitos y fallos
        var successes = individualResults.Where(r => r.IsValid).ToList();
        var failures = individualResults.Where(r => r.IsFailed).ToList();
        
        // Crear reporte detallado
        var processingTime = DateTime.UtcNow - startTime;
        
        var report = new BatchProcessingReport
        {
            BatchId = batchId,
            TotalRequests = requests.Count(),
            SuccessfulProcessed = successes.Count,
            FailedProcessed = failures.Count,
            SuccessfulData = successes.Select(s => s.Value).ToArray(),
            FailureDetails = failures.Select(f => new FailureDetail
            {
                ErrorMessage = string.Join("; ", f.ErrorsDetails.AllErrors),
                Timestamp = DateTime.UtcNow
            }).ToArray(),
            ProcessingDuration = processingTime,
            ProcessedAt = DateTime.UtcNow
        };
        
        // Si hay fallos, fusionar errores pero incluir reporte parcial
        if (failures.Any())
        {
            var fusionedErrors = await failures.FusionFailErrosAsync();
            return MlResult<BatchProcessingReport>.Fail(
                $"Batch processing partially failed. Report: {JsonSerializer.Serialize(report)}. " +
                $"Errors: {string.Join("; ", fusionedErrors.ErrorsDetails.AllErrors)}");
        }
        
        return MlResult<BatchProcessingReport>.Valid(report);
    }
    
    // Métodos auxiliares
    private async Task<MlResult<UserFormatValidation>> ValidateUserFormatAsync(UserRegistrationRequest request)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            errors.Add("Invalid email format");
        
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 2)
            errors.Add("Name must be at least 2 characters");
        
        if (!string.IsNullOrWhiteSpace(request.Phone) && !IsValidPhone(request.Phone))
            errors.Add("Invalid phone format");
        
        return errors.Any()
            ? MlResult<UserFormatValidation>.Fail(string.Join("; ", errors))
            : MlResult<UserFormatValidation>.Valid(new UserFormatValidation { IsValid = true });
    }
    
    private async Task<MlResult<DuplicateValidation>> CheckDuplicateUserAsync(string email)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        return existingUser != null
            ? MlResult<DuplicateValidation>.Fail($"User with email {email} already exists")
            : MlResult<DuplicateValidation>.Valid(new DuplicateValidation { IsUnique = true });
    }
    
    private async Task<MlResult<EmailDomainValidation>> ValidateEmailDomainAsync(string email)
    {
        var domain = email.Split('@').LastOrDefault();
        var blockedDomains = new[] { "tempmail.com", "10minutemail.com", "throwaway.email" };
        
        return blockedDomains.Contains(domain)
            ? MlResult<EmailDomainValidation>.Fail($"Email domain {domain} is not allowed")
            : MlResult<EmailDomainValidation>.Valid(new EmailDomainValidation { IsValid = true });
    }
    
    private async Task<MlResult<ProcessedUserData>> ProcessSingleUserDataAsync(UserDataRequest request)
    {
        try
        {
            // Simulación de procesamiento complejo
            await Task.Delay(100); // Simular trabajo async
            
            if (request.UserId <= 0)
                return MlResult<ProcessedUserData>.Fail("Invalid user ID");
            
            return MlResult<ProcessedUserData>.Valid(new ProcessedUserData
            {
                UserId = request.UserId,
                ProcessedData = $"Processed data for user {request.UserId}",
                ProcessedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedUserData>.Fail($"Processing failed: {ex.Message}");
        }
    }
    
    private bool IsValidEmail(string email) => email.Contains("@") && email.Contains(".");
    private bool IsValidPhone(string phone) => phone.All(char.IsDigit) && phone.Length >= 10;
}

// Clases de apoyo
public class UserRegistrationRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
}

public class ValidatedUser
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public Guid ValidationId { get; set; }
    public DateTime ValidatedAt { get; set; }
    public string Status { get; set; }
}

public class EnrichedUserProfile
{
    public int UserId { get; set; }
    public object BasicInfo { get; set; }
    public object ProfileData { get; set; }
    public object Preferences { get; set; }
    public object ActivitySummary { get; set; }
    public object SocialConnections { get; set; }
    public DateTime EnrichmentTimestamp { get; set; }
    public string[] DataSources { get; set; }
}

public class DocumentUploadRequest
{
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
    public long FileSize { get; set; }
}

public class ProcessedDocument
{
    public string OriginalFileName { get; set; }
    public string StorageLocation { get; set; }
    public object ContentAnalysis { get; set; }
    public object Metadata { get; set; }
    public object Thumbnails { get; set; }
    public DateTime ProcessedAt { get; set; }
    public long FileSize { get; set; }
    public string FileType { get; set; }
    public Guid ProcessingId { get; set; }
}

public class BatchProcessingReport
{
    public Guid BatchId { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulProcessed { get; set; }
    public int FailedProcessed { get; set; }
    public ProcessedUserData[] SuccessfulData { get; set; }
    public FailureDetail[] FailureDetails { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ProcessedUserData
{
    public int UserId { get; set; }
    public string ProcessedData { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class FailureDetail
{
    public string ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
}

public class UserDataRequest
{
    public int UserId { get; set; }
    public object Data { get; set; }
}

public class UserFormatValidation
{
    public bool IsValid { get; set; }
}

public class DuplicateValidation
{
    public bool IsUnique { get; set; }
}

public class EmailDomainValidation
{
    public bool IsValid { get; set; }
}
```

### Ejemplo 2: Sistema de Procesamiento de Transacciones Financieras

```csharp
public class FinancialTransactionProcessor
{
    private readonly IBankingService _bankingService;
    private readonly IFraudDetectionService _fraudDetection;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    
    public async Task<MlResult<IEnumerable<ProcessedTransaction>>> ProcessTransactionBatchAsync(
        IEnumerable<TransactionRequest> transactions)
    {
        var batchId = Guid.NewGuid();
        var processingStartTime = DateTime.UtcNow;
        
        // Procesar transacciones aplicando validaciones y verificaciones
        var processingResult = await transactions.CompleteDataAsync(async transaction =>
        {
            try
            {
                // 1. Validación básica de la transacción
                var basicValidation = ValidateTransactionBasics(transaction);
                if (basicValidation.IsFailed)
                    return basicValidation.ToMlResultFail<ProcessedTransaction>();
                
                // 2. Verificación de fondos
                var fundsValidation = await VerifyAccountFundsAsync(
                    transaction.FromAccountId, transaction.Amount);
                if (fundsValidation.IsFailed)
                    return fundsValidation.ToMlResultFail<ProcessedTransaction>();
                
                // 3. Detección de fraude
                var fraudCheck = await _fraudDetection.AnalyzeTransactionAsync(transaction);
                if (fraudCheck.IsSuspicious)
                    return MlResult<ProcessedTransaction>.Fail(
                        $"Transaction flagged as suspicious: {fraudCheck.Reason}");
                
                // 4. Verificación de límites
                var limitsCheck = await CheckTransactionLimitsAsync(transaction);
                if (limitsCheck.IsFailed)
                    return limitsCheck.ToMlResultFail<ProcessedTransaction>();
                
                // 5. Procesamiento de la transacción
                var transactionResult = await _bankingService.ProcessTransactionAsync(transaction);
                if (!transactionResult.IsSuccessful)
                    return MlResult<ProcessedTransaction>.Fail(
                        $"Transaction processing failed: {transactionResult.ErrorMessage}");
                
                // 6. Auditoría
                await _auditService.LogTransactionAsync(transaction, transactionResult);
                
                // 7. Crear resultado procesado
                return MlResult<ProcessedTransaction>.Valid(new ProcessedTransaction
                {
                    TransactionId = transactionResult.TransactionId,
                    OriginalRequest = transaction,
                    ProcessedAt = DateTime.UtcNow,
                    BatchId = batchId,
                    Status = "Completed",
                    ProcessingDuration = DateTime.UtcNow - processingStartTime,
                    FraudScore = fraudCheck.Score,
                    BankTransactionId = transactionResult.BankTransactionId
                });
            }
            catch (Exception ex)
            {
                return MlResult<ProcessedTransaction>.Fail(
                    $"Transaction processing exception: {ex.Message}");
            }
        });
        
        // Si el procesamiento del lote falla, enviar notificaciones
        if (processingResult.IsFailed)
        {
            await _notificationService.NotifyBatchProcessingFailedAsync(
                batchId, processingResult.ErrorsDetails);
        }
        else
        {
            await _notificationService.NotifyBatchProcessingCompletedAsync(
                batchId, processingResult.Value.Count());
        }
        
        return processingResult;
    }
    
    public async Task<MlResult<IEnumerable<ValidatedAccount>>> ValidateAccountsForTransferAsync(
        IEnumerable<string> accountNumbers)
    {
        return await accountNumbers.CompleteDataAsync(async accountNumber =>
        {
            // Validar formato de número de cuenta
            if (!IsValidAccountNumberFormat(accountNumber))
                return MlResult<ValidatedAccount>.Fail($"Invalid account number format: {accountNumber}");
            
            // Verificar existencia de la cuenta
            var accountExists = await _bankingService.AccountExistsAsync(accountNumber);
            if (!accountExists)
                return MlResult<ValidatedAccount>.Fail($"Account not found: {accountNumber}");
            
            // Verificar estado de la cuenta
            var accountStatus = await _bankingService.GetAccountStatusAsync(accountNumber);
            if (!accountStatus.IsActive)
                return MlResult<ValidatedAccount>.Fail($"Account is not active: {accountNumber}");
            
            if (accountStatus.IsFrozen)
                return MlResult<ValidatedAccount>.Fail($"Account is frozen: {accountNumber}");
            
            // Obtener información adicional de la cuenta
            var accountInfo = await _bankingService.GetAccountInfoAsync(accountNumber);
            
            return MlResult<ValidatedAccount>.Valid(new ValidatedAccount
            {
                AccountNumber = accountNumber,
                AccountType = accountInfo.AccountType,
                Currency = accountInfo.Currency,
                CurrentBalance = accountInfo.CurrentBalance,
                ValidatedAt = DateTime.UtcNow,
                Status = "Valid"
            });
        });
    }
    
    public async Task<MlResult<IEnumerable<RecurringPaymentSetup>>> SetupRecurringPaymentsAsync(
        IEnumerable<RecurringPaymentRequest> requests)
    {
        return await requests.CompleteDataAsync(async request =>
        {
            // Validar configuración de pago recurrente
            var configValidation = ValidateRecurringPaymentConfig(request);
            if (configValidation.IsFailed)
                return configValidation.ToMlResultFail<RecurringPaymentSetup>();
            
            // Verificar autorización
            var authorizationResult = await VerifyPaymentAuthorizationAsync(request);
            if (authorizationResult.IsFailed)
                return authorizationResult.ToMlResultFail<RecurringPaymentSetup>();
            
            // Configurar pago recurrente
            var setupResult = await _bankingService.SetupRecurringPaymentAsync(request);
            if (!setupResult.IsSuccessful)
                return MlResult<RecurringPaymentSetup>.Fail(
                    $"Failed to setup recurring payment: {setupResult.ErrorMessage}");
            
            // Programar próximo pago
            var nextPaymentDate = CalculateNextPaymentDate(request.Schedule);
            await _bankingService.ScheduleNextPaymentAsync(setupResult.PaymentId, nextPaymentDate);
            
            return MlResult<RecurringPaymentSetup>.Valid(new RecurringPaymentSetup
            {
                PaymentId = setupResult.PaymentId,
                OriginalRequest = request,
                NextPaymentDate = nextPaymentDate,
                SetupAt = DateTime.UtcNow,
                Status = "Active"
            });
        });
    }
    
    // Ejemplo de manejo de errores fusionados
    public async Task<MlResult<BatchValidationReport>> ValidateTransactionBatchAsync(
        IEnumerable<TransactionRequest> transactions)
    {
        var batchId = Guid.NewGuid();
        var validationResults = new List<MlResult<TransactionValidation>>();
        
        // Validar cada transacción individualmente
        foreach (var transaction in transactions)
        {
            var validation = await ValidateSingleTransactionAsync(transaction);
            validationResults.Add(validation);
        }
        
        // Usar FusionErrosIfExists para obtener tanto éxitos como fallos
        var fusionResult = validationResults.FusionErrosIfExists();
        
        var report = new BatchValidationReport
        {
            BatchId = batchId,
            TotalTransactions = transactions.Count(),
            ValidatedAt = DateTime.UtcNow
        };
        
        if (fusionResult.IsValid)
        {
            // Todas las validaciones pasaron
            report.SuccessfulValidations = fusionResult.Value.Count();
            report.FailedValidations = 0;
            report.Status = "AllValid";
            report.ValidTransactions = fusionResult.Value.ToArray();
        }
        else
        {
            // Algunas validaciones fallaron
            var successfulValidations = validationResults.Where(r => r.IsValid).ToList();
            report.SuccessfulValidations = successfulValidations.Count;
            report.FailedValidations = validationResults.Count - successfulValidations.Count;
            report.Status = "PartiallyValid";
            report.ValidTransactions = successfulValidations.Select(r => r.Value).ToArray();
            report.ValidationErrors = fusionResult.ErrorsDetails.AllErrors.ToArray();
        }
        
        return MlResult<BatchValidationReport>.Valid(report);
    }
    
    // Métodos auxiliares
    private MlResult<TransactionBasicValidation> ValidateTransactionBasics(TransactionRequest transaction)
    {
        var errors = new List<string>();
        
        if (transaction.Amount <= 0)
            errors.Add("Transaction amount must be positive");
        
        if (string.IsNullOrEmpty(transaction.FromAccountId))
            errors.Add("Source account ID is required");
        
        if (string.IsNullOrEmpty(transaction.ToAccountId))
            errors.Add("Destination account ID is required");
        
        if (transaction.FromAccountId == transaction.ToAccountId)
            errors.Add("Source and destination accounts cannot be the same");
        
        return errors.Any()
            ? MlResult<TransactionBasicValidation>.Fail(string.Join("; ", errors))
            : MlResult<TransactionBasicValidation>.Valid(new TransactionBasicValidation { IsValid = true });
    }
    
    private async Task<MlResult<FundsValidation>> VerifyAccountFundsAsync(string accountId, decimal amount)
    {
        var balance = await _bankingService.GetAccountBalanceAsync(accountId);
        
        return balance >= amount
            ? MlResult<FundsValidation>.Valid(new FundsValidation { HasSufficientFunds = true })
            : MlResult<FundsValidation>.Fail($"Insufficient funds. Required: {amount}, Available: {balance}");
    }
    
    private bool IsValidAccountNumberFormat(string accountNumber) =>
        !string.IsNullOrEmpty(accountNumber) && accountNumber.Length >= 10 && accountNumber.All(char.IsDigit);
    
    private DateTime CalculateNextPaymentDate(PaymentSchedule schedule) =>
        schedule.Frequency switch
        {
            "Daily" => DateTime.Today.AddDays(1),
            "Weekly" => DateTime.Today.AddDays(7),
            "Monthly" => DateTime.Today.AddMonths(1),
            _ => DateTime.Today.AddMonths(1)
        };
}

// Clases adicionales para el ejemplo
public class TransactionRequest
{
    public string FromAccountId { get; set; }
    public string ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Description { get; set; }
}

public class ProcessedTransaction
{
    public string TransactionId { get; set; }
    public TransactionRequest OriginalRequest { get; set; }
    public DateTime ProcessedAt { get; set; }
    public Guid BatchId { get; set; }
    public string Status { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
    public double FraudScore { get; set; }
    public string BankTransactionId { get; set; }
}

public class ValidatedAccount
{
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public string Currency { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime ValidatedAt { get; set; }
    public string Status { get; set; }
}

public class RecurringPaymentRequest
{
    public string FromAccountId { get; set; }
    public string ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public PaymentSchedule Schedule { get; set; }
}

public class RecurringPaymentSetup
{
    public string PaymentId { get; set; }
    public RecurringPaymentRequest OriginalRequest { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public DateTime SetupAt { get; set; }
    public string Status { get; set; }
}

public class BatchValidationReport
{
    public Guid BatchId { get; set; }
    public int TotalTransactions { get; set; }
    public int SuccessfulValidations { get; set; }
    public int FailedValidations { get; set; }
    public string Status { get; set; }
    public TransactionValidation[] ValidTransactions { get; set; }
    public string[] ValidationErrors { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class TransactionValidation
{
    public string TransactionId { get; set; }
    public bool IsValid { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class PaymentSchedule
{
    public string Frequency { get; set; } // Daily, Weekly, Monthly
    public DateTime StartDate { get; set; }
}

public class TransactionBasicValidation
{
    public bool IsValid { get; set; }
}

public class FundsValidation
{
    public bool HasSufficientFunds { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar CompleteData

```csharp
// ✅ Correcto: Validar y transformar cada elemento
var validatedUsers = await userRequests.CompleteDataAsync(async request =>
{
    var validation = await ValidateUserAsync(request);
    return validation.Map(user => EnrichUserData(user));
});

// ✅ Correcto: Procesar documentos con validación
var processedDocs = await documents.CompleteDataAsync(async doc =>
{
    var scanResult = await ScanDocumentAsync(doc);
    return scanResult.Bind(result => ProcessDocument(doc, result));
});

// ✅ Correcto: Transformar tipos manteniendo validación
var dtoResults = await entities.CompleteDataAsync(async entity =>
{
    var validEntity = await ValidateEntityAsync(entity);
    return validEntity.Map(e => e.ToDto());
});

// ❌ Incorrecto: Para transformaciones simples sin validación
var simpleMapped = await items.CompleteDataAsync(async item =>
    MlResult<string>.Valid(item.ToString())); // Mejor usar Select o Map directo
```

### 2. Manejo de Fusión de Errores

```csharp
// ✅ Correcto: Usar FusionErrosIfExists para reportes
var validationResults = GetValidationResults();
var fusionResult = validationResults.FusionErrosIfExists();

if (fusionResult.IsValid)
{
    ProcessSuccessfulResults(fusionResult.Value);
}
else
{
    var errors = fusionResult.ErrorsDetails.AllErrors;
    LogValidationErrors(errors);
    NotifyAdministrators(errors);
}

// ✅ Correcto: Usar FusionFailErros cuando solo interesan errores
var failedResults = GetOnlyFailedResults();
if (failedResults.Any())
{
    var fusionedErrors = failedResults.FusionFailErros();
    SendErrorReport(fusionedErrors.ErrorsDetails);
}

// ❌ Incorrecto: No manejar el caso donde no hay errores en FusionFailErros
var results = GetMixedResults();
var errorFusion = results.FusionFailErros(); // Puede fallar si no hay errores
```

### 3. Gestión de Recursos en Operaciones Async

```csharp
// ✅ Correcto: Usar using para recursos
public async Task<MlResult<IEnumerable<ProcessedFile>>> ProcessFilesAsync(IEnumerable<string> filePaths)
{
    return await filePaths.CompleteDataAsync(async filePath =>
    {
        try
        {
            using var fileStream = File.OpenRead(filePath);
            using var reader = new StreamReader(fileStream);
            
            var content = await reader.ReadToEndAsync();
            var processed = await ProcessFileContentAsync(content);
            
            return MlResult<ProcessedFile>.Valid(processed);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedFile>.Fail($"Failed to process {filePath}: {ex.Message}");
        }
    });
}

// ✅ Correcto: Manejar timeouts en operaciones largas
public async Task<MlResult<IEnumerable<ApiResponse>>> CallApisAsync(IEnumerable<ApiRequest> requests)
{
    return await requests.CompleteDataAsync(async request =>
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        try
        {
            var response = await _httpClient.SendAsync(request.ToHttpRequest(), cts.Token);
            var apiResponse = await ParseResponseAsync(response);
            return MlResult<ApiResponse>.Valid(apiResponse);
        }
        catch (OperationCanceledException)
        {
            return MlResult<ApiResponse>.Fail($"API call timeout for {request.Endpoint}");
        }
        catch (Exception ex)
        {
            return MlResult<ApiResponse>.Fail($"API call failed for {request.Endpoint}: {ex.Message}");
        }
    });
}
```

### 4. Optimización de Rendimiento

```csharp
// ✅ Correcto: Usar ConfigureAwait(false) en bibliotecas
public async Task<MlResult<IEnumerable<T>>> OptimizedCompleteDataAsync<T>(
    IEnumerable<T> source,
    Func<T, Task<MlResult<T>>> transform)
{
    return await source.CompleteDataAsync(async item =>
    {
        var result = await transform(item).ConfigureAwait(false);
        return result;
    }).ConfigureAwait(false);
}

// ✅ Correcto: Procesar en lotes para grandes volúmenes
public async Task<MlResult<IEnumerable<T>>> ProcessLargeBatchAsync<T>(
    IEnumerable<T> largeCollection,
    Func<T, Task<MlResult<T>>> processor)
{
    const int batchSize = 100;
    var results = new List<T>();
    var errors = new List<string>();
    
    var batches = largeCollection.Batch(batchSize);
    
    foreach (var batch in batches)
    {
        var batchResult = await batch.CompleteDataAsync(processor);
        
        if (batchResult.IsValid)
        {
            results.AddRange(batchResult.Value);
        }
        else
        {
            errors.AddRange(batchResult.ErrorsDetails.AllErrors);
        }
    }
    
    return errors.Any()
        ? MlResult<IEnumerable<T>>.Fail(string.Join("; ", errors))
        : MlResult<IEnumerable<T>>.Valid(results);
}

// ❌ Incorrecto: No considerar el rendimiento con grandes volúmenes
var result = await millionItems.CompleteDataAsync(async item =>
    await ExpensiveOperationAsync(item)); // Puede causar problemas de memoria
```

---

## Comparación con Métodos Similares

### Tabla Comparativa

| Método | Propósito | Comportamiento ante Errores | Resultado | Cuándo Usar |
|--------|-----------|----------------------------|-----------|-------------|
| `CompleteData` | Transformar colección completa | Falla si cualquier elemento falla | Todo o nada | Validaciones críticas |
| `Select` + `Where` | Filtrar y transformar | Ignora elementos problemáticos | Solo éxitos | Procesamiento permisivo |
| `Map` en colección | Transformar cada elemento | Depende de implementación | Transformación simple | Cambios de tipo simples |
| `Bind` secuencial | Encadenar operaciones | Falla en primer error | Operación secuencial | Dependencias entre elementos |

### Ejemplo Comparativo

```csharp
var numbers = new[] { "1", "2", "invalid", "4", "5" };

// CompleteData: Falla completamente si hay un error
var completeResult = numbers.CompleteData(str =>
    int.TryParse(str, out var num) 
        ? MlResult<int>.Valid(num)
        : MlResult<int>.Fail($"Invalid number: {str}"));
// Resultado: Fail("Invalid number: invalid")

// Select + Where: Solo procesa elementos válidos
var selectResult = numbers
    .Select(str => int.TryParse(str, out var num) ? (int?)num : null)
    .Where(x => x.HasValue)
    .Select(x => x.Value);
// Resultado: [1, 2, 4, 5] (ignora "invalid")

// FusionErrosIfExists: Combina errores pero permite resultados parciales
var mixedResults = numbers.Select(str =>
    int.TryParse(str, out var num) 
        ? MlResult<int>.Valid(num)
        : MlResult<int>.Fail($"Invalid: {str}"));

var fusionResult = mixedResults.FusionErrosIfExists();
// Resultado: Fail("Invalid: invalid") - pero se pueden extraer los válidos por separado
```

---

## Resumen

Los métodos `CompleteData` y las funciones de fusión proporcionan **procesamiento seguro de colecciones**:

- **`CompleteData`**: Aplica transformaciones con política "todo o nada"
- **`CompleteDataAsync`**: Soporte completo para operaciones asíncronas
- **`FusionFailErros`**: Combina errores de elementos fallidos
- **`FusionErrosIfExists`**: Fusiona errores o retorna valores válidos

**Casos de uso ideales**:
- **Validación de lotes** donde todos los elementos deben ser válidos
- **Procesamiento crítico** donde un fallo debe detener toda la operación  
- **Transformaciones complejas** que pueden fallar individualmente
- **Fusión de errores** para reportes detallados de fallos

**Ventajas principales**:
- **Seguridad de tipos** en transformaciones
- **Manejo robusto de errores** con fusión automática
- **Flexibilidad async** para operaciones I/O intensivas
- **Política todo-o-nada** para operaciones críticas