# MlResult NullToFailed - Conversión de Valores Null a Errores

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos NullToFailed Básicos](#métodos-nulltofailed-básicos)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Mejores Prácticas](#mejores-prácticas)
7. [Comparación con Otros Métodos de Validación](#comparación-con-otros-métodos-de-validación)

---

## Introducción

Los métodos `NullToFailed` proporcionan una forma de **convertir valores null en resultados fallidos**, tratando la ausencia de valor como un error explícito. Estos métodos son fundamentales para eliminar referencias null del flujo de datos y transformarlas en errores manejables dentro del contexto de `MlResult<T>`.

### Propósito Principal

- **Eliminación de Null References**: Convertir nulls en errores explícitos
- **Validación Temprana**: Detectar valores null antes de procesamiento
- **Flujo Seguro de Datos**: Garantizar que los valores no sean null en pipelines
- **Manejo Explícito de Errores**: Tratar null como condición de error específica

---

## Análisis de los Métodos

### Filosofía de NullToFailed

```
T (valor o null) → NullToFailed(error) → MlResult<T>
       ↓                 ↓                    ↓
  Valor válido → MlResult<T>.Valid(valor)
       ↓                 ↓                    ↓
      null → MlResult<T>.Fail(error)
```

### Características Principales

1. **Validación de Null**: Verifica que el valor no sea null
2. **Preservación de Valor**: Si no es null, mantiene el valor original
3. **Flexibilidad de Errores**: Acepta diferentes tipos de mensaje de error
4. **Tipos de Referencia**: Funciona con cualquier tipo que pueda ser null
5. **Soporte Asíncrono**: Variantes para operaciones asíncronas

---

## Métodos NullToFailed Básicos

### `NullToFailed<T>()` - Con MlErrorsDetails

**Propósito**: Convertir valor null en resultado fallido usando `MlErrorsDetails`

```csharp
public static MlResult<T> NullToFailed<T>(this T source,
                                          MlErrorsDetails errorsDetails)
```

**Comportamiento**:
- Si `source` no es null: retorna `MlResult<T>.Valid(source)`
- Si `source` es null: retorna `MlResult<T>.Fail(errorsDetails)`

**Ejemplo Básico**:
```csharp
User user = GetUserFromDatabase(userId);
var result = user.NullToFailed(new MlErrorsDetails(new[] {
    $"User with ID {userId} not found",
    "Please verify the user ID is correct"
}));

// Si user no es null: MlResult válido con el usuario
// Si user es null: MlResult fallido con los errores especificados
```

### `NullToFailed<T>()` - Con MlError

```csharp
public static MlResult<T> NullToFailed<T>(this T source,
                                          MlError error)
```

**Ejemplo**:
```csharp
var config = LoadConfiguration();
var result = config.NullToFailed(MlError.FromErrorMessage("Configuration not loaded"));
```

### `NullToFailed<T>()` - Con String

```csharp
public static MlResult<T> NullToFailed<T>(this T source,
                                          string errorMessage)
```

**Ejemplo**:
```csharp
var document = FindDocument(documentId);
var result = document.NullToFailed($"Document {documentId} not found");
```

### `NullToFailed<T>()` - Con IEnumerable<string>

```csharp
public static MlResult<T> NullToFailed<T>(this T source,
                                          IEnumerable<string> errorsMessage)
```

**Ejemplo**:
```csharp
var product = GetProduct(productId);
var result = product.NullToFailed(new[] {
    $"Product {productId} not found",
    "Product may have been discontinued",
    "Check product catalog for alternatives"
});
```

---

## Variantes Asíncronas

### `NullToFailedAsync<T>()` - Valor Síncrono

```csharp
public static async Task<MlResult<T>> NullToFailedAsync<T>(this T source,
                                                           MlError error)

public static async Task<MlResult<T>> NullToFailedAsync<T>(this T source,
                                                           MlErrorsDetails errorsDetails)

public static async Task<MlResult<T>> NullToFailedAsync<T>(this T source,
                                                           string errorMessage)

public static async Task<MlResult<T>> NullToFailedAsync<T>(this T source,
                                                           IEnumerable<string> errorsMessage)
```

**Ejemplo**:
```csharp
var user = GetUser(userId);
var result = await user.NullToFailedAsync("User not found");
```

### `NullToFailedAsync<T>()` - Valor Asíncrono

```csharp
public static async Task<MlResult<T>> NullToFailedAsync<T>(this Task<T> sourceAsync,
                                                           string errorMessage)

public static async Task<MlResult<T>> NullToFailedAsync<T>(this Task<T> sourceAsync,
                                                           IEnumerable<string> errorsMessage)
```

**Ejemplo**:
```csharp
var result = await GetUserAsync(userId)
    .NullToFailedAsync($"User {userId} not found in database");
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Gestión de Usuarios

```csharp
public class UserService
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthService _authService;
    private readonly ILogger _logger;
    
    public async Task<MlResult<UserProfile>> GetUserProfileAsync(int userId)
    {
        return await GetUserByIdAsync(userId)
            .NullToFailedAsync($"User with ID {userId} not found")
            .BindAsync(async user => await LoadUserProfileAsync(user))
            .MapAsync(async profile => await EnrichProfileDataAsync(profile));
    }
    
    public async Task<MlResult<AuthenticationResult>> AuthenticateUserAsync(
        string username, string password)
    {
        var user = await _userRepo.FindByUsernameAsync(username);
        
        return await user
            .NullToFailedAsync(new[] {
                $"User '{username}' not found",
                "Please check your username",
                "Contact support if you believe this is an error"
            })
            .BindAsync(async validUser => await ValidatePasswordAsync(validUser, password))
            .BindAsync(async authenticatedUser => await GenerateTokenAsync(authenticatedUser))
            .ExecSelfIfValidAsync(async result => 
                await _logger.LogInformationAsync($"User {username} authenticated successfully"))
            .ExecSelfIfFailAsync(async errors => 
                await _logger.LogWarningAsync($"Authentication failed for {username}: {errors.FirstErrorMessage}"));
    }
    
    public async Task<MlResult<UserSettings>> GetUserSettingsAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        var settings = await _userRepo.GetUserSettingsAsync(userId);
        
        return user
            .NullToFailed($"User {userId} not found")
            .Bind(_ => settings.NullToFailed(new[] {
                $"No settings found for user {userId}",
                "Default settings will be created",
                "You can customize your preferences in the settings page"
            }))
            .Map(userSettings => userSettings ?? CreateDefaultSettings(userId));
    }
    
    public async Task<MlResult<ContactInfo>> GetPrimaryContactAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        
        return user
            .NullToFailed($"User {userId} not found")
            .Bind(validUser => 
            {
                var primaryContact = validUser.Contacts?.FirstOrDefault(c => c.IsPrimary);
                return primaryContact.NullToFailed($"User {userId} has no primary contact information");
            });
    }
    
    public async Task<MlResult<PaymentMethod>> GetDefaultPaymentMethodAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        
        return user
            .NullToFailed($"User {userId} not found")
            .Bind(validUser =>
            {
                var defaultPayment = validUser.PaymentMethods?.FirstOrDefault(pm => pm.IsDefault);
                return defaultPayment.NullToFailed(new[] {
                    $"User {userId} has no default payment method",
                    "Please add a payment method in your account settings",
                    "A default payment method is required for purchases"
                });
            });
    }
    
    private UserSettings CreateDefaultSettings(int userId)
    {
        return new UserSettings
        {
            UserId = userId,
            Theme = "light",
            Language = "en",
            NotificationsEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class UserProfile
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public class AuthenticationResult
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserProfile User { get; set; }
}

public class UserSettings
{
    public int UserId { get; set; }
    public string Theme { get; set; }
    public string Language { get; set; }
    public bool NotificationsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContactInfo
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsPrimary { get; set; }
}

public class PaymentMethod
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string LastFourDigits { get; set; }
    public bool IsDefault { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public List<ContactInfo> Contacts { get; set; }
    public List<PaymentMethod> PaymentMethods { get; set; }
}
```

### Ejemplo 2: Sistema de Configuración y Recursos

```csharp
public class ConfigurationService
{
    private readonly IConfigRepository _configRepo;
    private readonly IResourceManager _resourceManager;
    
    public async Task<MlResult<AppConfiguration>> LoadApplicationConfigAsync(string environment)
    {
        var config = await _configRepo.GetConfigurationAsync(environment);
        
        return config
            .NullToFailed(new[] {
                $"Configuration for environment '{environment}' not found",
                "Available environments: development, staging, production",
                "Please check your environment settings"
            })
            .Bind(validConfig => ValidateConfiguration(validConfig))
            .Map(validatedConfig => ApplyDefaultValues(validatedConfig));
    }
    
    public async Task<MlResult<DatabaseConnection>> GetDatabaseConnectionAsync(string connectionName)
    {
        var connectionString = await _configRepo.GetConnectionStringAsync(connectionName);
        
        return connectionString
            .NullToFailed($"Connection string '{connectionName}' not found")
            .Bind(connStr => CreateDatabaseConnection(connStr))
            .BindAsync(async connection => await TestConnectionAsync(connection));
    }
    
    public async Task<MlResult<LocalizedResource>> GetResourceAsync(string key, string culture)
    {
        var resource = await _resourceManager.GetResourceAsync(key, culture);
        
        return resource
            .NullToFailed(new[] {
                $"Resource '{key}' not found for culture '{culture}'",
                $"Falling back to default culture may be required",
                "Consider adding the missing resource to the localization files"
            })
            .Map(validResource => validResource);
    }
    
    public async Task<MlResult<ApiConfiguration>> GetApiConfigurationAsync(string serviceName)
    {
        var config = await _configRepo.GetApiConfigAsync(serviceName);
        
        return config
            .NullToFailed($"API configuration for service '{serviceName}' not found")
            .Bind(apiConfig => ValidateApiEndpoints(apiConfig))
            .Bind(validConfig => ValidateApiCredentials(validConfig));
    }
    
    public MlResult<CacheSettings> GetCacheSettingsForModule(string moduleName)
    {
        var settings = _configRepo.GetCacheSettings(moduleName);
        
        return settings
            .NullToFailed($"Cache settings for module '{moduleName}' not configured")
            .Map(cacheSettings => ApplyCacheDefaults(cacheSettings));
    }
    
    private MlResult<AppConfiguration> ValidateConfiguration(AppConfiguration config)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(config.ApplicationName))
            errors.Add("Application name is required");
        if (config.Port <= 0)
            errors.Add("Valid port number is required");
        if (string.IsNullOrEmpty(config.LogLevel))
            errors.Add("Log level is required");
        
        return errors.Any() 
            ? MlResult<AppConfiguration>.Fail(errors.ToArray())
            : MlResult<AppConfiguration>.Valid(config);
    }
    
    private MlResult<DatabaseConnection> CreateDatabaseConnection(string connectionString)
    {
        try
        {
            var connection = new DatabaseConnection(connectionString);
            return MlResult<DatabaseConnection>.Valid(connection);
        }
        catch (Exception ex)
        {
            return MlResult<DatabaseConnection>.Fail($"Invalid connection string: {ex.Message}");
        }
    }
    
    private async Task<MlResult<DatabaseConnection>> TestConnectionAsync(DatabaseConnection connection)
    {
        try
        {
            await connection.TestConnectionAsync();
            return MlResult<DatabaseConnection>.Valid(connection);
        }
        catch (Exception ex)
        {
            return MlResult<DatabaseConnection>.Fail($"Connection test failed: {ex.Message}");
        }
    }
}

public class AppConfiguration
{
    public string ApplicationName { get; set; }
    public int Port { get; set; }
    public string LogLevel { get; set; }
    public string Environment { get; set; }
    public Dictionary<string, string> AppSettings { get; set; }
}

public class DatabaseConnection
{
    public string ConnectionString { get; private set; }
    
    public DatabaseConnection(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    public async Task TestConnectionAsync()
    {
        // Implementación de test de conexión
        await Task.Delay(100); // Simulación
    }
}

public class LocalizedResource
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Culture { get; set; }
}

public class ApiConfiguration
{
    public string ServiceName { get; set; }
    public string BaseUrl { get; set; }
    public int TimeoutSeconds { get; set; }
    public ApiCredentials Credentials { get; set; }
}

public class ApiCredentials
{
    public string ApiKey { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public class CacheSettings
{
    public string ModuleName { get; set; }
    public int ExpirationMinutes { get; set; }
    public int MaxItems { get; set; }
    public bool Enabled { get; set; }
}
```

### Ejemplo 3: Sistema de Procesamiento de Documentos

```csharp
public class DocumentProcessingService
{
    private readonly IDocumentRepository _docRepo;
    private readonly IFileService _fileService;
    private readonly IValidationService _validator;
    
    public async Task<MlResult<ProcessedDocument>> ProcessDocumentAsync(int documentId)
    {
        return await GetDocumentByIdAsync(documentId)
            .NullToFailedAsync($"Document {documentId} not found")
            .BindAsync(async doc => await LoadDocumentContentAsync(doc))
            .BindAsync(async docWithContent => await ValidateDocumentAsync(docWithContent))
            .BindAsync(async validDoc => await ProcessDocumentContentAsync(validDoc))
            .MapAsync(async processedDoc => await SaveProcessedDocumentAsync(processedDoc));
    }
    
    public async Task<MlResult<DocumentMetadata>> GetDocumentMetadataAsync(int documentId)
    {
        var document = await _docRepo.GetByIdAsync(documentId);
        
        return document
            .NullToFailed($"Document {documentId} not found")
            .Bind(doc =>
            {
                var metadata = doc.Metadata;
                return metadata.NullToFailed(new[] {
                    $"Document {documentId} has no metadata",
                    "Metadata is required for document processing",
                    "Please ensure document was uploaded correctly"
                });
            });
    }
    
    public async Task<MlResult<DocumentTemplate>> GetTemplateForDocumentAsync(Document document)
    {
        var template = await _docRepo.GetTemplateAsync(document.TemplateId);
        
        return template
            .NullToFailed(new[] {
                $"Template {document.TemplateId} not found",
                $"Document {document.Id} cannot be processed without template",
                "Please contact administrator to restore missing template"
            });
    }
    
    public async Task<MlResult<ProcessingRule[]>> GetProcessingRulesAsync(string documentType)
    {
        var rules = await _docRepo.GetProcessingRulesAsync(documentType);
        
        return rules
            .NullToFailed($"No processing rules defined for document type '{documentType}'")
            .Bind(ruleArray => ruleArray.Length == 0 
                ? MlResult<ProcessingRule[]>.Fail("Processing rules array is empty")
                : MlResult<ProcessingRule[]>.Valid(ruleArray));
    }
    
    public MlResult<FileInfo> GetDocumentFile(Document document)
    {
        var file = _fileService.GetFile(document.FilePath);
        
        return file
            .NullToFailed(new[] {
                $"Document file not found at path: {document.FilePath}",
                "File may have been moved or deleted",
                "Document processing cannot continue without the source file"
            });
    }
    
    public async Task<MlResult<ValidationResult>> ValidateDocumentStructureAsync(Document document)
    {
        var schema = await _docRepo.GetValidationSchemaAsync(document.DocumentType);
        
        return schema
            .NullToFailed($"Validation schema not found for document type '{document.DocumentType}'")
            .BindAsync(async validSchema => await _validator.ValidateAgainstSchemaAsync(document, validSchema));
    }
    
    private async Task<Document> GetDocumentByIdAsync(int documentId)
    {
        return await _docRepo.GetByIdAsync(documentId);
    }
    
    private async Task<MlResult<Document>> LoadDocumentContentAsync(Document document)
    {
        try
        {
            var content = await _fileService.ReadFileAsync(document.FilePath);
            document.Content = content;
            return MlResult<Document>.Valid(document);
        }
        catch (Exception ex)
        {
            return MlResult<Document>.Fail($"Failed to load document content: {ex.Message}");
        }
    }
    
    private async Task<MlResult<Document>> ValidateDocumentAsync(Document document)
    {
        var validationResult = await _validator.ValidateDocumentAsync(document);
        return validationResult.IsValid 
            ? MlResult<Document>.Valid(document)
            : MlResult<Document>.Fail(validationResult.Errors);
    }
    
    private async Task<MlResult<ProcessedDocument>> ProcessDocumentContentAsync(Document document)
    {
        try
        {
            var processed = await _fileService.ProcessContentAsync(document.Content);
            return MlResult<ProcessedDocument>.Valid(new ProcessedDocument
            {
                OriginalDocument = document,
                ProcessedContent = processed,
                ProcessedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedDocument>.Fail($"Document processing failed: {ex.Message}");
        }
    }
    
    private async Task<ProcessedDocument> SaveProcessedDocumentAsync(ProcessedDocument processed)
    {
        await _docRepo.SaveProcessedDocumentAsync(processed);
        return processed;
    }
}

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string DocumentType { get; set; }
    public int TemplateId { get; set; }
    public DocumentMetadata Metadata { get; set; }
    public string Content { get; set; }
}

public class DocumentMetadata
{
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime CreatedDate { get; set; }
    public Dictionary<string, string> CustomFields { get; set; }
}

public class DocumentTemplate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Structure { get; set; }
    public ProcessingRule[] Rules { get; set; }
}

public class ProcessingRule
{
    public string Name { get; set; }
    public string Condition { get; set; }
    public string Action { get; set; }
}

public class ProcessedDocument
{
    public Document OriginalDocument { get; set; }
    public string ProcessedContent { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string[] Errors { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar NullToFailed

```csharp
// ✅ Correcto: Validar objetos que no pueden ser null
var user = GetUser(userId)
    .NullToFailed($"User {userId} not found");

// ✅ Correcto: Validar configuraciones críticas
var config = LoadConfiguration()
    .NullToFailed("Application configuration not loaded");

// ✅ Correcto: Validar recursos requeridos
var template = GetTemplate(templateId)
    .NullToFailed($"Template {templateId} required for processing");

// ❌ Incorrecto: Validar valores que pueden ser null legitimamente
var optionalData = GetOptionalData()
    .NullToFailed("Optional data missing"); // Optional puede ser null
```

### 2. Mensajes de Error Informativos

```csharp
// ✅ Correcto: Mensajes específicos con contexto
var user = GetUser(userId)
    .NullToFailed(new[] {
        $"User with ID {userId} not found",
        "Please verify the user ID is correct",
        "Contact support if the user should exist"
    });

// ✅ Correcto: Incluir información sobre consecuencias
var paymentMethod = GetDefaultPaymentMethod(userId)
    .NullToFailed(new[] {
        "No default payment method configured",
        "Payment processing cannot continue",
        "Please add a payment method in settings"
    });

// ❌ Incorrecto: Mensajes genéricos
var data = GetData()
    .NullToFailed("Null"); // Muy poco informativo
```

### 3. Uso en Pipelines de Validación

```csharp
// ✅ Correcto: Validación temprana en pipelines
var result = await GetUserAsync(userId)
    .NullToFailedAsync($"User {userId} not found")          // Validar null
    .BindAsync(async user => await ValidateUserStatusAsync(user))  // Validar estado
    .BindAsync(async validUser => await LoadUserDataAsync(validUser))  // Cargar datos
    .MapAsync(async userData => await TransformUserDataAsync(userData)); // Transformar

// ✅ Correcto: Validación de dependencias
var processResult = GetDocument(docId)
    .NullToFailed($"Document {docId} not found")
    .Bind(doc => GetTemplate(doc.TemplateId)
        .NullToFailed($"Template {doc.TemplateId} not found")
        .Map(template => new { Document = doc, Template = template }))
    .Bind(pair => ProcessDocument(pair.Document, pair.Template));
```

### 4. Diferenciación de Tipos de Error

```csharp
// ✅ Correcto: Errores específicos según criticidad
var criticalConfig = GetCriticalConfig()
    .NullToFailed("CRITICAL: Required configuration missing - application cannot start");

var userPreference = GetUserPreference(userId, "theme")
    .NullToFailed("User theme preference not set, using default");

var optionalFeature = GetOptionalFeature(featureId)
    .NullToFailed($"Optional feature {featureId} not available");

// ✅ Correcto: Múltiples niveles de información
var document = GetDocument(docId)
    .NullToFailed(new[] {
        $"Document {docId} not found",           // Error principal
        "Document may have been deleted",        // Posible causa
        "Check document ID and try again",       // Acción sugerida
        "Contact support if problem persists"    // Escalamiento
    });
```

---

## Comparación con Otros Métodos de Validación

### Tabla Comparativa

| Método | Entrada | Condición de Error | Uso Principal | Salida |
|--------|---------|-------------------|---------------|--------|
| `NullToFailed` | `T` | `value == null` | Validar no-null | `MlResult<T>` |
| `EmptyToFailed` | `IEnumerable<T>` | Colección vacía | Validar no-vacío | `MlResult<IEnumerable<T>>` |
| `Bind` | `MlResult<T>` | Función retorna Fail | Encadenar validaciones | `MlResult<TResult>` |

### Ejemplo Comparativo

```csharp
// NullToFailed: Convierte null en error
var user = GetUser(userId)
    .NullToFailed("User not found");

// EmptyToFailed: Convierte colección vacía en error
var users = GetUsers()
    .EmptyToFailed("No users available");

// Bind: Encadena validaciones que pueden fallar
var result = GetUser(userId)
    .NullToFailed("User not found")
    .Bind(user => ValidateUserStatus(user))
    .Bind(validUser => LoadUserPermissions(validUser));

// Combinación típica
var processedUser = GetUser(userId)
    .NullToFailed("User not found")                    // Validar no-null
    .Bind(user => ValidateUser(user))                  // Validar contenido
    .Map(validUser => TransformUser(validUser));       // Transformar
```

---

## Resumen

Los métodos `NullToFailed` proporcionan **validación de valores no-null**:

- **`NullToFailed`**: Convierte valores null en resultados fallidos
- **`NullToFailedAsync`**: Soporte completo para operaciones asíncronas
- **Flexibilidad de errores**: Acepta `MlError`, `MlErrorsDetails`, `string` o `IEnumerable<string>`

**Casos de uso ideales**:
- **Validación de parámetros** requeridos en métodos
- **Verificación de dependencias** antes del procesamiento
- **Validación de resultados** de búsquedas que deben existir
- **Eliminación de null references** en pipelines de datos

**Ventajas principales**:
- **Eliminación segura de nulls** del flujo de datos
- **Mensajes de error descriptivos** y contextuales
- **Integración perfecta** con pipelines de `MlResult`
- **Detección temprana** de valores faltantes