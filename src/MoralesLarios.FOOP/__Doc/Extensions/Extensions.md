# MlResult Extensions - Utilidades y Extensiones Auxiliares

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Extensiones de Validación](#extensiones-de-validación)
4. [Utilidades de Tipos](#utilidades-de-tipos)
5. [Extensiones de Flujo](#extensiones-de-flujo)
6. [Conversiones de Funciones](#conversiones-de-funciones)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Integración con MlResult](#integración-con-mlresult)

---

## Introducción

La clase `Extensions` proporciona un conjunto de **utilidades y extensiones auxiliares** que complementan el ecosistema MlResult con funcionalidades adicionales para validación, manipulación de objetos, gestión de errores y conversiones de tipos. Estas extensiones facilitan la integración con .NET estándar y proporcionan herramientas útiles para el desarrollo cotidiano.

### Propósito Principal

- **Validación de Objetos**: Integración con Data Annotations
- **Manipulación Fluida**: Modificación de objetos de forma funcional
- **Gestión de Errores**: Manejo de excepciones y contexto de error
- **Conversiones de Tipos**: Utilidades para transformaciones comunes
- **Interoperabilidad**: Puentes entre paradigmas síncronos y asíncronos

---

## Análisis de los Métodos

### Filosofía de Extensions

```
Objeto + Extensión → Funcionalidad Mejorada
  ↓         ↓              ↓
value + ValidateObject → ValidationResults
  ↓         ↓              ↓
value + With(actions) → Modified Object
  ↓         ↓              ↓
func + ToFuncTask → Async Function
```

### Características Principales

1. **Extensiones No-Invasivas**: Métodos que no modifican la API base
2. **Integración Estándar**: Compatibilidad con .NET System
3. **Flujo Funcional**: Métodos que preservan el estilo funcional
4. **Utilidades Comunes**: Soluciones para problemas frecuentes
5. **Soporte Async**: Conversiones y adaptadores async/await

---

## Extensiones de Validación

### 1. ValidateObject - Validación con Data Annotations

**Propósito**: Validar objetos usando atributos de validación de .NET

```csharp
public static IEnumerable<ValidationResult> ValidateObject(this object source)
```

**Funcionamiento Interno**:
```csharp
var valContext = new ValidationContext(source, null, null);
var resultado = new List<ValidationResult>();
Validator.TryValidateObject(source, valContext, resultado, true);
return resultado;
```

**Ejemplo Básico**:
```csharp
public class UserModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name too long")]
    public string Name { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    
    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int Age { get; set; }
}

var user = new UserModel { Name = "", Email = "invalid-email", Age = 15 };
var validationResults = user.ValidateObject();

foreach (var error in validationResults)
{
    Console.WriteLine(error.ErrorMessage);
}
// Output:
// Name is required
// Invalid email format  
// Age must be between 18 and 120
```

---

## Utilidades de Tipos

### 1. ToNullable - Conversión a Nullable

**Propósito**: Convertir tipos valor a sus equivalentes nullable

```csharp
public static T? ToNullable<T>(this T source) where T : struct
```

**Ejemplo**:
```csharp
int age = 25;
int? nullableAge = age.ToNullable();

DateTime date = DateTime.Now;
DateTime? nullableDate = date.ToNullable();

// Útil en expresiones LINQ y asignaciones condicionales
var result = someCondition ? value.ToNullable() : null;
```

### 2. AppendExDetails - Extensión de Detalles de Excepción

**Propósito**: Agregar excepciones a diccionarios de detalles sin sobrescribir

```csharp
public static Dictionary<string, object> AppendExDetails(
    this Dictionary<string, object> source, 
    Exception ex)
```

**Funcionamiento**:
```csharp
var exKeys = source.Keys.Where(x => x.StartsWith(EX_DESC_KEY)).ToList();
var exKey = exKeys.Any() ? $"{EX_DESC_KEY}{exKeys.Count + 1}" : EX_DESC_KEY;
var result = source.ToDictionary(x => x.Key, x => x.Value);
result.Add(exKey, ex);
return result;
```

**Ejemplo**:
```csharp
var errorDetails = new Dictionary<string, object>
{
    { "UserId", 123 },
    { "Operation", "CreateOrder" }
};

try
{
    // Primera operación que falla
    throw new ArgumentException("Invalid argument");
}
catch (Exception ex1)
{
    errorDetails = errorDetails.AppendExDetails(ex1);
    // errorDetails ahora contiene "Ex" -> ArgumentException
}

try
{
    // Segunda operación que falla
    throw new InvalidOperationException("Invalid state");
}
catch (Exception ex2)
{
    errorDetails = errorDetails.AppendExDetails(ex2);
    // errorDetails ahora contiene "Ex" y "Ex2"
}
```

---

## Extensiones de Flujo

### 1. With - Modificación Fluida de Objetos

**Propósito**: Aplicar múltiples modificaciones a un objeto de forma fluida

```csharp
public static T With<T>(this T source, params Action<T>[] changes) where T : class
```

**Ejemplo Básico**:
```csharp
var user = new User()
    .With(
        u => u.Name = "John Doe",
        u => u.Email = "john@example.com",
        u => u.Age = 30,
        u => u.IsActive = true
    );

// Equivalente a:
// var user = new User();
// user.Name = "John Doe";
// user.Email = "john@example.com";
// user.Age = 30;
// user.IsActive = true;
```

### 2. WithAsync - Versión Asíncrona de With

**Múltiples sobrecargas para diferentes escenarios async**:

```csharp
public static Task<T> WithAsync<T>(this T source, params Action<T>[] changes) where T : class
public static async Task<T> WithAsync<T>(this Task<T> sourceAsync, params Action<T>[] changes) where T : class
```

**Ejemplo**:
```csharp
var user = await GetUserAsync(userId)
    .WithAsync(
        u => u.LastLoginDate = DateTime.Now,
        u => u.LoginCount++,
        u => u.IsOnline = true
    );
```

### 3. VoidToAsync - Conversión de Acciones a Task

**Propósito**: Convertir operaciones void en operaciones async

```csharp
public static Task VoidToAsync<T>(this T source, Action<T> voidAction)
```

**Ejemplo**:
```csharp
var user = new User();
await user.VoidToAsync(u => u.UpdateLastActivity());

// Útil para integrar código síncrono en flujos async
await ProcessUserAsync(user)
    .ContinueWith(async result => await result.Result.VoidToAsync(u => LogUserAction(u)));
```

---

## Conversiones de Funciones

### 1. ToFuncTask - Conversión de Funciones a Async

**Múltiples sobrecargas para diferentes tipos de funciones**:

```csharp
// Func<T, TResult> -> Func<T, Task<TResult>>
public static Func<T, Task<TResult>> ToFuncTask<T, TResult>(this Func<T, TResult> func)

// Func<MlErrorsDetails, TResult> -> Func<MlErrorsDetails, Task<TResult>>
public static Func<MlErrorsDetails, Task<TResult>> ToFuncTask<TResult>(this Func<MlErrorsDetails, TResult> func)

// Action<T> -> Func<T, Task>
public static Func<T, Task> ToFuncTask<T>(this Action<T> action)

// Action<MlErrorsDetails> -> Func<MlErrorsDetails, Task>
public static Func<MlErrorsDetails, Task> ToFuncTask(this Action<MlErrorsDetails> action)
```

**Ejemplo de Uso**:
```csharp
// Función síncrona original
Func<User, string> getUserName = user => user.Name;

// Convertir a async
Func<User, Task<string>> getUserNameAsync = getUserName.ToFuncTask();

// Usar en contexto async
var name = await getUserNameAsync(user);

// Acción síncrona original
Action<User> logUser = user => Console.WriteLine($"User: {user.Name}");

// Convertir a async
Func<User, Task> logUserAsync = logUser.ToFuncTask();

// Usar en contexto async
await logUserAsync(user);
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Validación y Procesamiento de Formularios

```csharp
public class FormProcessingService
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly ILoggerService _loggerService;
    
    public async Task<MlResult<ProcessedForm>> ProcessUserRegistrationFormAsync(UserRegistrationForm form)
    {
        // 1. Validar formulario usando Data Annotations
        var validationResult = await ValidateFormWithDataAnnotationsAsync(form);
        if (validationResult.IsFailed)
            return validationResult.ToMlResultFail<ValidationSummary, ProcessedForm>();
        
        // 2. Procesar y enriquecer datos del formulario
        var enrichedForm = await EnrichFormDataAsync(form);
        if (enrichedForm.IsFailed)
            return enrichedForm.ToMlResultFail<EnrichedForm, ProcessedForm>();
        
        // 3. Crear usuario con modificaciones fluidas
        var userCreationResult = await CreateUserWithFluentModificationsAsync(enrichedForm.Value);
        if (userCreationResult.IsFailed)
            return userCreationResult.ToMlResultFail<User, ProcessedForm>();
        
        // 4. Procesar notificaciones asíncronas
        var notificationResult = await ProcessNotificationsAsync(userCreationResult.Value);
        
        return notificationResult.Match(
            valid: _ => MlResult<ProcessedForm>.Valid(new ProcessedForm
            {
                FormId = form.Id,
                UserId = userCreationResult.Value.Id,
                ProcessedAt = DateTime.UtcNow,
                ValidationSummary = validationResult.Value,
                CreatedUser = userCreationResult.Value,
                NotificationsSent = notificationResult.Value
            }),
            fail: errors => MlResult<ProcessedForm>.Fail(errors.AllErrors)
        );
    }
    
    private async Task<MlResult<ValidationSummary>> ValidateFormWithDataAnnotationsAsync(UserRegistrationForm form)
    {
        return await Task.Run(() =>
        {
            // Usar extensión ValidateObject para validación estándar
            var validationResults = form.ValidateObject().ToList();
            
            // Validaciones personalizadas adicionales
            var customValidations = PerformCustomValidations(form);
            validationResults.AddRange(customValidations);
            
            var summary = new ValidationSummary
            {
                IsValid = !validationResults.Any(),
                ErrorCount = validationResults.Count,
                Errors = validationResults.Select(vr => new ValidationError
                {
                    Field = string.Join(",", vr.MemberNames),
                    Message = vr.ErrorMessage,
                    Severity = DetermineErrorSeverity(vr)
                }).ToArray(),
                ValidatedAt = DateTime.UtcNow
            };
            
            return summary.IsValid
                ? MlResult<ValidationSummary>.Valid(summary)
                : MlResult<ValidationSummary>.Fail($"Form validation failed with {summary.ErrorCount} errors");
        });
    }
    
    private async Task<MlResult<EnrichedForm>> EnrichFormDataAsync(UserRegistrationForm form)
    {
        try
        {
            // Enriquecer datos usando modificaciones fluidas
            var enrichedForm = new EnrichedForm
            {
                OriginalForm = form,
                ProcessingId = Guid.NewGuid(),
                ReceivedAt = DateTime.UtcNow
            }.With(
                ef => ef.NormalizedEmail = form.Email?.ToLowerInvariant()?.Trim(),
                ef => ef.FormattedPhone = FormatPhoneNumber(form.Phone),
                ef => ef.GeolocationData = await GetGeolocationDataAsync(form.IpAddress),
                ef => ef.UserAgent = form.UserAgent,
                ef => ef.ReferralSource = DetermineReferralSource(form.ReferralCode)
            );
            
            // Validaciones adicionales en datos enriquecidos
            return EnsureFp.NotNull(enrichedForm.NormalizedEmail, "Email normalization failed")
                .Bind(_ => EnsureFp.NotNullEmptyOrWhitespace(enrichedForm.FormattedPhone, "Phone formatting failed"))
                .Map(_ => enrichedForm);
        }
        catch (Exception ex)
        {
            var errorDetails = new Dictionary<string, object>
            {
                { "FormId", form.Id },
                { "EnrichmentStep", "DataEnrichment" },
                { "ProcessedAt", DateTime.UtcNow }
            }.AppendExDetails(ex);
            
            return MlResult<EnrichedForm>.Fail(
                new MlErrorsDetails(
                    new List<MlError> { new MlError("Form enrichment failed") },
                    errorDetails));
        }
    }
    
    private async Task<MlResult<User>> CreateUserWithFluentModificationsAsync(EnrichedForm enrichedForm)
    {
        try
        {
            // Crear usuario base
            var baseUser = new User
            {
                Id = 0, // Will be set by database
                Email = enrichedForm.NormalizedEmail,
                CreatedAt = DateTime.UtcNow
            };
            
            // Aplicar modificaciones fluidas basadas en datos del formulario
            var configuredUser = baseUser.With(
                u => u.Name = $"{enrichedForm.OriginalForm.FirstName} {enrichedForm.OriginalForm.LastName}",
                u => u.Phone = enrichedForm.FormattedPhone,
                u => u.DateOfBirth = enrichedForm.OriginalForm.DateOfBirth?.ToNullable(),
                u => u.Country = enrichedForm.OriginalForm.Country,
                u => u.IsEmailVerified = false,
                u => u.IsActive = true,
                u => u.UserType = DetermineUserType(enrichedForm),
                u => u.Preferences = CreateDefaultPreferences(enrichedForm),
                u => u.Metadata = CreateUserMetadata(enrichedForm)
            );
            
            // Guardar usuario en base de datos
            var savedUser = await _userService.CreateAsync(configuredUser);
            
            return EnsureFp.NotNull(savedUser, "User creation failed - database returned null")
                .Bind(user => EnsureFp.That(user.Id, user.Id > 0, "User creation failed - invalid ID assigned"));
        }
        catch (Exception ex)
        {
            var errorDetails = new Dictionary<string, object>
            {
                { "FormId", enrichedForm.OriginalForm.Id },
                { "ProcessingId", enrichedForm.ProcessingId },
                { "Email", enrichedForm.NormalizedEmail }
            }.AppendExDetails(ex);
            
            return MlResult<User>.Fail(
                new MlErrorsDetails(
                    new List<MlError> { new MlError("User creation failed") },
                    errorDetails));
        }
    }
    
    private async Task<MlResult<NotificationResults>> ProcessNotificationsAsync(User user)
    {
        var results = new List<NotificationResult>();
        var errors = new List<string>();
        
        // Convertir acciones síncronas a async usando ToFuncTask
        var emailNotificationFunc = ((Action<User>)(u => 
            results.Add(SendWelcomeEmail(u)))).ToFuncTask();
        
        var smsNotificationFunc = ((Action<User>)(u => 
            results.Add(SendWelcomeSms(u)))).ToFuncTask();
        
        var auditLogFunc = ((Action<User>)(u => 
            _loggerService.LogUserCreation(u))).ToFuncTask();
        
        try
        {
            // Ejecutar notificaciones en paralelo
            await Task.WhenAll(
                emailNotificationFunc(user),
                smsNotificationFunc(user),
                auditLogFunc(user)
            );
            
            // Agregar notificación de sistema usando VoidToAsync
            await user.VoidToAsync(u => 
                results.Add(new NotificationResult 
                { 
                    Type = "System", 
                    Success = true, 
                    Message = "User registered in system" 
                }));
            
            var notificationResults = new NotificationResults
            {
                Results = results.ToArray(),
                TotalSent = results.Count(r => r.Success),
                TotalFailed = results.Count(r => !r.Success),
                ProcessedAt = DateTime.UtcNow
            };
            
            return MlResult<NotificationResults>.Valid(notificationResults);
        }
        catch (Exception ex)
        {
            var errorDetails = new Dictionary<string, object>
            {
                { "UserId", user.Id },
                { "NotificationStep", "ParallelNotifications" }
            }.AppendExDetails(ex);
            
            return MlResult<NotificationResults>.Fail(
                new MlErrorsDetails(
                    new List<MlError> { new MlError("Notification processing failed") },
                    errorDetails));
        }
    }
    
    // Métodos auxiliares que usan las extensiones
    private IEnumerable<ValidationResult> PerformCustomValidations(UserRegistrationForm form)
    {
        var customErrors = new List<ValidationResult>();
        
        // Validación personalizada de edad
        if (form.DateOfBirth.HasValue)
        {
            var age = CalculateAge(form.DateOfBirth.Value);
            if (age < 13)
            {
                customErrors.Add(new ValidationResult(
                    "User must be at least 13 years old",
                    new[] { nameof(form.DateOfBirth) }));
            }
        }
        
        // Validación de unicidad de email (simulada)
        if (!string.IsNullOrEmpty(form.Email) && IsEmailTaken(form.Email))
        {
            customErrors.Add(new ValidationResult(
                "Email address is already registered",
                new[] { nameof(form.Email) }));
        }
        
        return customErrors;
    }
    
    private async Task<GeolocationData> GetGeolocationDataAsync(string ipAddress)
    {
        // Simulación de servicio de geolocalización
        await Task.Delay(100);
        return new GeolocationData 
        { 
            Country = "US", 
            City = "New York", 
            Timezone = "America/New_York" 
        };
    }
    
    private string FormatPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone))
            return phone;
        
        // Formateo básico de teléfono
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        return digits.Length == 10 ? $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6)}" : phone;
    }
    
    private NotificationResult SendWelcomeEmail(User user)
    {
        try
        {
            // Simulación de envío de email
            _notificationService.SendEmail(user.Email, "Welcome!", "Welcome to our platform!");
            return new NotificationResult { Type = "Email", Success = true, Message = "Welcome email sent" };
        }
        catch (Exception ex)
        {
            return new NotificationResult { Type = "Email", Success = false, Message = ex.Message };
        }
    }
    
    private NotificationResult SendWelcomeSms(User user)
    {
        try
        {
            // Simulación de envío de SMS
            if (!string.IsNullOrEmpty(user.Phone))
            {
                _notificationService.SendSms(user.Phone, "Welcome to our platform!");
                return new NotificationResult { Type = "SMS", Success = true, Message = "Welcome SMS sent" };
            }
            return new NotificationResult { Type = "SMS", Success = false, Message = "No phone number provided" };
        }
        catch (Exception ex)
        {
            return new NotificationResult { Type = "SMS", Success = false, Message = ex.Message };
        }
    }
    
    private string DetermineUserType(EnrichedForm form) => "Standard";
    private UserPreferences CreateDefaultPreferences(EnrichedForm form) => new UserPreferences();
    private Dictionary<string, object> CreateUserMetadata(EnrichedForm form) => new Dictionary<string, object>();
    private string DetermineReferralSource(string referralCode) => string.IsNullOrEmpty(referralCode) ? "Direct" : "Referral";
    private ErrorSeverity DetermineErrorSeverity(ValidationResult vr) => ErrorSeverity.High;
    private int CalculateAge(DateTime birthDate) => DateTime.Now.Year - birthDate.Year;
    private bool IsEmailTaken(string email) => false; // Simulación
}

// Clases de apoyo para el ejemplo
public class UserRegistrationForm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; }
    
    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public string ReferralCode { get; set; }
    
    public string IpAddress { get; set; }
    
    public string UserAgent { get; set; }
}

public class EnrichedForm
{
    public UserRegistrationForm OriginalForm { get; set; }
    public Guid ProcessingId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string NormalizedEmail { get; set; }
    public string FormattedPhone { get; set; }
    public GeolocationData GeolocationData { get; set; }
    public string UserAgent { get; set; }
    public string ReferralSource { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Country { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public string UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserPreferences Preferences { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class ValidationSummary
{
    public bool IsValid { get; set; }
    public int ErrorCount { get; set; }
    public ValidationError[] Errors { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class ValidationError
{
    public string Field { get; set; }
    public string Message { get; set; }
    public ErrorSeverity Severity { get; set; }
}

public class ProcessedForm
{
    public Guid FormId { get; set; }
    public int UserId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public ValidationSummary ValidationSummary { get; set; }
    public User CreatedUser { get; set; }
    public NotificationResults NotificationsSent { get; set; }
}

public class NotificationResults
{
    public NotificationResult[] Results { get; set; }
    public int TotalSent { get; set; }
    public int TotalFailed { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class NotificationResult
{
    public string Type { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class GeolocationData
{
    public string Country { get; set; }
    public string City { get; set; }
    public string Timezone { get; set; }
}

public class UserPreferences
{
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "light";
}

public enum ErrorSeverity
{
    Low,
    Medium,
    High,
    Critical
}
```

### Ejemplo 2: Sistema de Configuración y Ajustes

```csharp
public class ConfigurationManagementService
{
    private readonly IConfigRepository _configRepository;
    private readonly ICacheService _cacheService;
    
    public async Task<MlResult<SystemConfiguration>> LoadAndValidateSystemConfigAsync()
    {
        // Cargar configuración base
        var baseConfig = await LoadBaseConfigurationAsync();
        if (baseConfig.IsFailed)
            return baseConfig;
        
        // Aplicar configuraciones específicas del entorno usando With
        var environmentConfig = await ApplyEnvironmentSpecificConfigAsync(baseConfig.Value);
        if (environmentConfig.IsFailed)
            return environmentConfig;
        
        // Validar configuración completa
        var validationResult = await ValidateCompleteConfigurationAsync(environmentConfig.Value);
        if (validationResult.IsFailed)
            return validationResult;
        
        // Aplicar configuraciones dinámicas
        var finalConfig = await ApplyDynamicConfigurationsAsync(validationResult.Value);
        
        return finalConfig;
    }
    
    private async Task<MlResult<SystemConfiguration>> ApplyEnvironmentSpecificConfigAsync(
        SystemConfiguration baseConfig)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        
        try
        {
            var configuredSystem = environment switch
            {
                "Development" => await baseConfig.WithAsync(
                    config => config.DatabaseSettings.ConnectionTimeout = 30,
                    config => config.LoggingSettings.LogLevel = "Debug",
                    config => config.CacheSettings.EnableCache = false,
                    config => config.SecuritySettings.RequireHttps = false,
                    config => config.PerformanceSettings.EnableCompression = false
                ),
                
                "Testing" => await baseConfig.WithAsync(
                    config => config.DatabaseSettings.ConnectionTimeout = 60,
                    config => config.LoggingSettings.LogLevel = "Information",
                    config => config.CacheSettings.EnableCache = true,
                    config => config.CacheSettings.CacheDurationMinutes = 5,
                    config => config.SecuritySettings.RequireHttps = false,
                    config => config.TestingSettings = new TestingSettings 
                    { 
                        MockExternalServices = true,
                        UseInMemoryDatabase = true 
                    }
                ),
                
                "Staging" => await baseConfig.WithAsync(
                    config => config.DatabaseSettings.ConnectionTimeout = 120,
                    config => config.LoggingSettings.LogLevel = "Information",
                    config => config.CacheSettings.EnableCache = true,
                    config => config.CacheSettings.CacheDurationMinutes = 30,
                    config => config.SecuritySettings.RequireHttps = true,
                    config => config.PerformanceSettings.EnableCompression = true,
                    config => config.MonitoringSettings.EnableDetailedMetrics = true
                ),
                
                "Production" => await baseConfig.WithAsync(
                    config => config.DatabaseSettings.ConnectionTimeout = 180,
                    config => config.LoggingSettings.LogLevel = "Warning",
                    config => config.CacheSettings.EnableCache = true,
                    config => config.CacheSettings.CacheDurationMinutes = 60,
                    config => config.SecuritySettings.RequireHttps = true,
                    config => config.SecuritySettings.EnableRateLimiting = true,
                    config => config.PerformanceSettings.EnableCompression = true,
                    config => config.PerformanceSettings.MaxConcurrentRequests = 1000,
                    config => config.MonitoringSettings.EnableDetailedMetrics = true,
                    config => config.MonitoringSettings.EnableAlerts = true
                ),
                
                _ => throw new InvalidOperationException($"Unknown environment: {environment}")
            };
            
            return MlResult<SystemConfiguration>.Valid(configuredSystem);
        }
        catch (Exception ex)
        {
            var errorDetails = new Dictionary<string, object>
            {
                { "Environment", environment },
                { "ConfigurationStep", "EnvironmentSpecific" }
            }.AppendExDetails(ex);
            
            return MlResult<SystemConfiguration>.Fail(
                new MlErrorsDetails(
                    new List<MlError> { new MlError("Environment configuration failed") },
                    errorDetails));
        }
    }
    
    private async Task<MlResult<SystemConfiguration>> ValidateCompleteConfigurationAsync(
        SystemConfiguration config)
    {
        try
        {
            // Usar ValidateObject para validación con Data Annotations
            var validationResults = config.ValidateObject().ToList();
            
            // Agregar validaciones personalizadas
            var customValidations = await PerformCustomConfigValidationsAsync(config);
            validationResults.AddRange(customValidations);
            
            if (validationResults.Any())
            {
                var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
                return MlResult<SystemConfiguration>.Fail(
                    $"Configuration validation failed: {string.Join("; ", errorMessages)}");
            }
            
            return MlResult<SystemConfiguration>.Valid(config);
        }
        catch (Exception ex)
        {
            var errorDetails = new Dictionary<string, object>
            {
                { "ConfigurationStep", "Validation" },
                { "ValidatedAt", DateTime.UtcNow }
            }.AppendExDetails(ex);
            
            return MlResult<SystemConfiguration>.Fail(
                new MlErrorsDetails(
                    new List<MlError> { new MlError("Configuration validation error") },
                    errorDetails));
        }
    }
    
    private async Task<MlResult<SystemConfiguration>> ApplyDynamicConfigurationsAsync(
        SystemConfiguration config)
    {
        try
        {
            // Convertir funciones síncronas a async usando ToFuncTask
            var updateCacheSettingsFunc = ((Func<SystemConfiguration, SystemConfiguration>)(cfg =>
                cfg.With(c => c.CacheSettings.MaxMemoryMB = GetOptimalCacheSize())))
                .ToFuncTask();
            
            var updateConnectionPoolFunc = ((Func<SystemConfiguration, SystemConfiguration>)(cfg =>
                cfg.With(c => c.DatabaseSettings.MaxPoolSize = GetOptimalPoolSize())))
                .ToFuncTask();
            
            var updatePerformanceSettingsFunc = ((Func<SystemConfiguration, SystemConfiguration>)(cfg =>
                cfg.With(
                    c => c.PerformanceSettings.MaxConcurrentRequests = GetMaxConcurrentRequests(),
                    c => c.PerformanceSettings.RequestTimeoutSeconds = GetOptimalTimeout())))
                .ToFuncTask();
            
            // Aplicar configuraciones dinámicas en secuencia
            var updatedConfig = await updateCacheSettingsFunc(config);
            updatedConfig = await updateConnectionPoolFunc(updatedConfig);
            updatedConfig = await updatePerformanceSettingsFunc(updatedConfig);
            
            // Aplicar configuraciones finales usando VoidToAsync
            await updatedConfig.VoidToAsync(cfg =>
            {
                cfg.RuntimeSettings = new RuntimeSettings
                {
                    StartupTime = DateTime.UtcNow,
                    ConfigurationVersion = GenerateConfigVersion(),
                    OptimizationApplied = true
                };
            });
            
            return MlResult<SystemConfiguration>.Valid(updatedConfig);
        }
        catch (Exception ex)
        {
            var errorDetails = new Dictionary<string, object>
            {
                { "ConfigurationStep", "DynamicConfiguration" }
            }.AppendExDetails(ex);
            
            return MlResult<SystemConfiguration>.Fail(
                new MlErrorsDetails(
                    new List<MlError> { new MlError("Dynamic configuration failed") },
                    errorDetails));
        }
    }
    
    // Métodos auxiliares que demuestran el uso de extensiones
    private async Task<List<ValidationResult>> PerformCustomConfigValidationsAsync(SystemConfiguration config)
    {
        var errors = new List<ValidationResult>();
        
        // Validación de rangos de configuración
        if (config.DatabaseSettings?.MaxPoolSize <= 0)
        {
            errors.Add(new ValidationResult(
                "Database max pool size must be positive",
                new[] { nameof(config.DatabaseSettings.MaxPoolSize) }));
        }
        
        // Validación de consistencia entre configuraciones
        if (config.CacheSettings?.EnableCache == true && config.CacheSettings.MaxMemoryMB <= 0)
        {
            errors.Add(new ValidationResult(
                "Cache memory must be positive when cache is enabled",
                new[] { nameof(config.CacheSettings.MaxMemoryMB) }));
        }
        
        // Validación asíncrona de conectividad
        if (!string.IsNullOrEmpty(config.DatabaseSettings?.ConnectionString))
        {
            var connectionValid = await TestDatabaseConnectionAsync(config.DatabaseSettings.ConnectionString);
            if (!connectionValid)
            {
                errors.Add(new ValidationResult(
                    "Database connection string is invalid or unreachable",
                    new[] { nameof(config.DatabaseSettings.ConnectionString) }));
            }
        }
        
        return errors;
    }
    
    private int GetOptimalCacheSize()
    {
        var totalMemory = GC.GetTotalMemory(false);
        return (int)(totalMemory / 1024 / 1024 * 0.1); // 10% of current memory
    }
    
    private int GetOptimalPoolSize()
    {
        var processorCount = Environment.ProcessorCount;
        return Math.Max(10, processorCount * 2);
    }
    
    private int GetMaxConcurrentRequests()
    {
        var processorCount = Environment.ProcessorCount;
        return processorCount * 100;
    }
    
    private int GetOptimalTimeout()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" ? 30 : 60;
    }
    
    private string GenerateConfigVersion() => $"v{DateTime.UtcNow:yyyyMMdd.HHmmss}";
    
    private async Task<bool> TestDatabaseConnectionAsync(string connectionString)
    {
        // Simulación de prueba de conexión
        await Task.Delay(100);
        return !connectionString.Contains("invalid");
    }
}

// Clases de configuración para el ejemplo
public class SystemConfiguration
{
    [Required]
    public DatabaseSettings DatabaseSettings { get; set; }
    
    [Required]
    public CacheSettings CacheSettings { get; set; }
    
    [Required]
    public SecuritySettings SecuritySettings { get; set; }
    
    [Required]
    public LoggingSettings LoggingSettings { get; set; }
    
    public PerformanceSettings PerformanceSettings { get; set; }
    public MonitoringSettings MonitoringSettings { get; set; }
    public TestingSettings TestingSettings { get; set; }
    public RuntimeSettings RuntimeSettings { get; set; }
}

public class DatabaseSettings
{
    [Required]
    public string ConnectionString { get; set; }
    
    [Range(1, 300)]
    public int ConnectionTimeout { get; set; } = 30;
    
    [Range(1, 1000)]
    public int MaxPoolSize { get; set; } = 100;
}

public class CacheSettings
{
    public bool EnableCache { get; set; } = true;
    
    [Range(1, 10000)]
    public int MaxMemoryMB { get; set; } = 512;
    
    [Range(1, 1440)]
    public int CacheDurationMinutes { get; set; } = 60;
}

public class SecuritySettings
{
    public bool RequireHttps { get; set; } = true;
    public bool EnableRateLimiting { get; set; } = false;
    
    [StringLength(256)]
    public string JwtSecret { get; set; }
}

public class LoggingSettings
{
    [Required]
    public string LogLevel { get; set; } = "Information";
    
    public string LogPath { get; set; }
}

public class PerformanceSettings
{
    public bool EnableCompression { get; set; } = true;
    
    [Range(1, 10000)]
    public int MaxConcurrentRequests { get; set; } = 100;
    
    [Range(5, 300)]
    public int RequestTimeoutSeconds { get; set; } = 30;
}

public class MonitoringSettings
{
    public bool EnableDetailedMetrics { get; set; } = false;
    public bool EnableAlerts { get; set; } = false;
}

public class TestingSettings
{
    public bool MockExternalServices { get; set; } = false;
    public bool UseInMemoryDatabase { get; set; } = false;
}

public class RuntimeSettings
{
    public DateTime StartupTime { get; set; }
    public string ConfigurationVersion { get; set; }
    public bool OptimizationApplied { get; set; }
}
```

---

## Mejores Prácticas

### 1. Uso Apropiado de With

```csharp
// ✅ Correcto: Para configuración de objetos con múltiples propiedades
var user = new User()
    .With(
        u => u.Name = "John Doe",
        u => u.Email = "john@example.com",
        u => u.IsActive = true,
        u => u.CreatedAt = DateTime.UtcNow
    );

// ✅ Correcto: Para modificaciones condicionales
var product = baseProduct.With(
    p => p.Price = calculatedPrice,
    p => p.IsOnSale = price < originalPrice,
    p => p.UpdatedAt = DateTime.UtcNow
);

// ❌ Incorrecto: Para una sola propiedad
var user = new User().With(u => u.Name = "John"); // Mejor: user.Name = "John";
```

### 2. Gestión de Errores con AppendExDetails

```csharp
// ✅ Correcto: Acumular errores en operaciones complejas
var errorContext = new Dictionary<string, object>
{
    { "UserId", userId },
    { "Operation", "UserProcessing" }
};

try
{
    await ProcessStep1();
}
catch (Exception ex1)
{
    errorContext = errorContext.AppendExDetails(ex1);
    
    try
    {
        await ProcessStep2();
    }
    catch (Exception ex2)
    {
        errorContext = errorContext.AppendExDetails(ex2);
        // errorContext ahora tiene "Ex" y "Ex2"
        
        return MlResult<T>.Fail(
            new MlErrorsDetails(
                new List<MlError> { new MlError("Multiple failures occurred") },
                errorContext));
    }
}

// ❌ Incorrecto: Sobrescribir excepciones anteriores
var errorContext = new Dictionary<string, object>();
// errorContext["Ex"] = ex1; // Se pierde cuando llega ex2
// errorContext["Ex"] = ex2; // Sobrescribe ex1
```

### 3. Conversiones de Funciones Apropiadas

```csharp
// ✅ Correcto: Convertir funciones para uso en contextos async
Func<User, string> syncFormatter = user => $"{user.Name} ({user.Email})";
Func<User, Task<string>> asyncFormatter = syncFormatter.ToFuncTask();

await users.SelectAsync(asyncFormatter); // Usar en contexto async

// ✅ Correcto: Convertir acciones para integración async
Action<User> syncLogger = user => Console.WriteLine($"User: {user.Name}");
Func<User, Task> asyncLogger = syncLogger.ToFuncTask();

await ProcessUsersAsync(users, asyncLogger);

// ❌ Incorrecto: Convertir funciones ya async
Func<User, Task<string>> alreadyAsync = async user => await FormatUserAsync(user);
var redundant = alreadyAsync.ToFuncTask(); // Innecesario
```

### 4. Validación con ValidateObject

```csharp
// ✅ Correcto: Combinar Data Annotations con validaciones personalizadas
public MlResult<User> ValidateUser(User user)
{
    // Validaciones estándar
    var standardValidations = user.ValidateObject().ToList();
    
    // Validaciones personalizadas
    var customValidations = new List<ValidationResult>();
    
    if (user.Age.HasValue && user.Age < 13)
    {
        customValidations.Add(new ValidationResult(
            "Users must be at least 13 years old",
            new[] { nameof(user.Age) }));
    }
    
    var allValidations = standardValidations.Concat(customValidations).ToList();
    
    return allValidations.Any()
        ? MlResult<User>.Fail($"Validation failed: {string.Join("; ", allValidations.Select(v => v.ErrorMessage))}")
        : MlResult<User>.Valid(user);
}

// ❌ Incorrecto: Solo usar validaciones manuales cuando Data Annotations está disponible
public MlResult<User> ValidateUserManually(User user)
{
    if (string.IsNullOrEmpty(user.Name)) // Mejor usar [Required] attribute
        return MlResult<User>.Fail("Name required");
    
    if (string.IsNullOrEmpty(user.Email)) // Mejor usar [Required] attribute
        return MlResult<User>.Fail("Email required");
    
    // ... más validaciones manuales que podrían ser attributes
}
```

---

## Integración con MlResult

### Combinación con Otros Métodos MlResult

```csharp
// Ejemplo de integración completa
public async Task<MlResult<ProcessedUser>> ProcessUserWithExtensionsAsync(UserInput input)
{
    return await EnsureFp.NotNull(input, "User input is required")
        .BindAsync(async validInput =>
        {
            // Validar con Data Annotations
            var validationErrors = validInput.ValidateObject().ToList();
            if (validationErrors.Any())
            {
                return MlResult<User>.Fail(
                    $"Validation failed: {string.Join("; ", validationErrors.Select(v => v.ErrorMessage))}");
            }
            
            // Crear usuario usando With
            var user = new User().With(
                u => u.Name = validInput.Name,
                u => u.Email = validInput.Email.ToLowerInvariant(),
                u => u.Age = validInput.BirthDate?.ToNullable() != null 
                    ? CalculateAge(validInput.BirthDate.Value) 
                    : (int?)null,
                u => u.CreatedAt = DateTime.UtcNow
            );
            
            return MlResult<User>.Valid(user);
        })
        .BindAsync(async user =>
        {
            // Procesar usando funciones convertidas a async
            var processFunc = ((Func<User, ProcessedUser>)ProcessUserSync).ToFuncTask();
            var processed = await processFunc(user);
            
            return MlResult<ProcessedUser>.Valid(processed);
        })
        .BindAsync(async processed =>
        {
            // Finalizar usando VoidToAsync
            await processed.VoidToAsync(p => LogProcessedUser(p));
            
            return MlResult<ProcessedUser>.Valid(processed);
        });
}
```

---

## Resumen

La clase `Extensions` proporciona **utilidades auxiliares** que enriquecen el ecosistema MlResult:

- **`ValidateObject`**: Integración con Data Annotations para validación estándar
- **`With/WithAsync`**: Modificación fluida de objetos con múltiples cambios
- **`ToFuncTask`**: Conversión de funciones síncronas a asíncronas
- **`AppendExDetails`**: Acumulación segura de excepciones en contexto de error

**Casos de uso ideales**:
- **Validación de modelos** con atributos estándar de .NET
- **Configuración de objetos** con múltiples propiedades
- **Integración legacy** convirtiendo código síncrono a asíncrono
- **Gestión de errores** acumulando contexto de múltiples fallos

**Ventajas principales**:
- **Interoperabilidad** con el ecosistema .NET estándar
- **Flujo funcional** manteniendo inmutabilidad conceptual
- **Flexibilidad async** para adaptar código existente
- **Gestión robusta de errores** con preservación de contexto