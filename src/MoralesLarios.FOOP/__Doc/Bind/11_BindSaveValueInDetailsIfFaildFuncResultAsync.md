# MlResultActionsBindSaveValueInDetails - Operaciones de Binding con Preservación de Valor

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos BindSaveValueInDetailsIfFaildFuncResult](#métodos-bindsavevalueindetailsiffaildFuncresult)
4. [Métodos TryBindSaveValueInDetailsIfFaildFuncResult](#métodos-trybindsavevalueindetailsiffaildFuncresult)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsBindSaveValueInDetails` contiene operaciones especializadas de **binding con preservación de valor en caso de fallo**. Estos métodos extienden el comportamiento del binding tradicional añadiendo automáticamente el valor original como detalle de error cuando la función de transformación falla, proporcionando **trazabilidad completa** del valor que causó el error.

### Propósito Principal

- **Preservación de Contexto**: Mantener el valor original cuando una transformación falla
- **Debugging Mejorado**: Facilitar la depuración al incluir el valor que causó el error
- **Trazabilidad**: Crear un rastro completo de valores a través del pipeline
- **Análisis de Fallos**: Permitir análisis posterior de los valores que causaron errores
- **Auditoría Detallada**: Registrar información completa para auditorías

---

## Análisis de la Clase

### Estructura y Filosofía

Estos métodos implementan un patrón de **binding con memoria**, donde el valor original se preserva en los detalles del error:

```
Valor Original → Función de Transformación → ¿Éxito?
      ↓                      ↓                 ↓
   Preservar            Retornar Éxito    Añadir Valor a Error
      ↓                                         ↓
   En Error Details ←――――――――――――――――――――――――――――
```

### Características Principales

1. **Binding Condicional**: Solo ejecuta la función si el resultado fuente es válido
2. **Preservación Automática**: Añade el valor original a los detalles de error automáticamente
3. **Transparencia en Éxito**: Comportamiento idéntico al binding normal cuando hay éxito
4. **Compatibilidad Completa**: Versiones síncronas, asíncronas y seguras (Try)

---

## Métodos BindSaveValueInDetailsIfFaildFuncResult

### `BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Ejecuta la función de transformación si el resultado es válido, y añade el valor original a los detalles de error si la función falla

```csharp
public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func)
```

**Parámetros**:
- `source`: El resultado origen a evaluar
- `func`: Función que transforma el valor si `source` es válido

**Comportamiento**:
- Si `source` es fallido: Propaga el error sin ejecutar `func`
- Si `source` es válido y `func` tiene éxito: Retorna el resultado de `func`
- Si `source` es válido y `func` falla: Retorna el error de `func` + valor original en detalles

**Ejemplo Básico**:
```csharp
var userAge = MlResult<int>.Valid(25);

var validationResult = userAge.BindSaveValueInDetailsIfFaildFuncResult(age => 
{
    if (age < 18)
        return MlResult<string>.Fail($"Age {age} is below minimum required");
    if (age > 150)
        return MlResult<string>.Fail($"Age {age} is unrealistic");
        
    return MlResult<string>.Valid($"Valid age: {age}");
});

// Si age = 15, el error incluirá:
// - Mensaje: "Age 15 is below minimum required"
// - Detalles: Incluye el valor original (15) para análisis posterior
```

### Versiones Asíncronas

#### `BindSaveValueInDetailsIfFaildFuncResultAsync()` - Función Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)
```

**Ejemplo**:
```csharp
var emailData = MlResult<EmailRequest>.Valid(new EmailRequest { To = "user@example.com" });

var sendResult = await emailData.BindSaveValueInDetailsIfFaildFuncResultAsync(async email => 
{
    var isValid = await ValidateEmailAsync(email.To);
    if (!isValid)
        return MlResult<EmailResponse>.Fail($"Invalid email format: {email.To}");
        
    return await SendEmailAsync(email);
});

// Si la validación falla, el error incluye el EmailRequest completo en los detalles
```

#### `BindSaveValueInDetailsIfFaildFuncResultAsync()` - Fuente Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)

public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, MlResult<TReturn>> func)
```

---

## Métodos TryBindSaveValueInDetailsIfFaildFuncResult

### `TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Versión segura que captura excepciones, las convierte en errores y preserva el valor original

```csharp
public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    Func<Exception, string> errorMessageBuilder)

public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    string errorMessage = null!)
```

**Comportamiento**:
- Comportamiento idéntico al método básico
- **Además**: Captura excepciones de `func` y las convierte en `MlResult` fallido
- El valor original se añade a los detalles tanto para errores de `func` como para excepciones

**Ejemplo**:
```csharp
var jsonData = MlResult<string>.Valid("""{"name": "John", "age": "invalid"}""");

var parseResult = jsonData.TryBindSaveValueInDetailsIfFaildFuncResult(json => 
{
    // Esta función puede lanzar JsonException
    var document = JsonDocument.Parse(json);
    var person = JsonSerializer.Deserialize<Person>(json);
    
    if (person.Age < 0)
        return MlResult<Person>.Fail("Age cannot be negative");
        
    return MlResult<Person>.Valid(person);
}, ex => $"JSON parsing failed: {ex.Message}");

// Si JsonDocument.Parse lanza excepción:
// - Error: "JSON parsing failed: [exception message]"
// - Detalles: Incluye el JSON original para análisis
```

### Versiones Asíncronas de TryBind

#### Conversión a Asíncrono
```csharp
public static Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    Func<Exception, string> errorMessageBuilder)
```

#### Función Asíncrona Segura
```csharp
public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync,
    Func<Exception, string> errorMessageBuilder)
```

**Ejemplo con función asíncrona**:
```csharp
var filePathResult = GetFilePathAsync();

var processResult = await filePathResult.TryBindSaveValueInDetailsIfFaildFuncResultAsync(
    async filePath => 
    {
        // Puede lanzar FileNotFoundException, UnauthorizedAccessException, etc.
        var content = await File.ReadAllTextAsync(filePath);
        
        if (string.IsNullOrWhiteSpace(content))
            return MlResult<ProcessedFile>.Fail("File is empty");
            
        return await ProcessFileContentAsync(content);
    },
    ex => $"File processing failed: {ex.GetType().Name} - {ex.Message}"
);

// Si hay excepción, el error incluye:
// - Mensaje personalizado basado en la excepción
// - El path del archivo original en los detalles
```

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Fuente | Función | Manejo Excepciones | Método |
|--------|---------|-------------------|---------|
| `MlResult<T>` | `T → MlResult<U>` | No | `BindSaveValueInDetailsIfFaildFuncResult` |
| `MlResult<T>` | `T → MlResult<U>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResult` |
| `MlResult<T>` | `T → Task<MlResult<U>>` | No | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `MlResult<T>` | `T → Task<MlResult<U>>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → MlResult<U>` | No | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → MlResult<U>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | No | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResultAsync` |

---

## Ejemplos Prácticos

### Ejemplo 1: Pipeline de Validación con Trazabilidad

```csharp
public class UserRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;
    
    public UserRegistrationService(
        IUserRepository userRepository, 
        IEmailService emailService, 
        ILogger logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task<MlResult<RegisteredUser>> RegisterUserWithDetailedTrackingAsync(UserRegistrationRequest request)
    {
        return await ValidateRegistrationRequest(request)
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validRequest => await ValidateEmailUniquenessAsync(validRequest))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async emailValidatedRequest => await ValidatePasswordStrengthAsync(emailValidatedRequest))
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async passwordValidatedRequest => await CreateUserAccountAsync(passwordValidatedRequest),
                ex => $"Account creation failed: {ex.Message}")
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async createdUser => await SendWelcomeEmailAsync(createdUser),
                "Welcome email sending failed")
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async emailSentUser => await FinalizeRegistrationAsync(emailSentUser));
    }
    
    private MlResult<UserRegistrationRequest> ValidateRegistrationRequest(UserRegistrationRequest request)
    {
        if (request == null)
            return MlResult<UserRegistrationRequest>.Fail("Registration request cannot be null");
            
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add("Email is required");
        else if (!IsValidEmailFormat(request.Email))
            errors.Add($"Email format is invalid: {request.Email}");
            
        if (string.IsNullOrWhiteSpace(request.Password))
            errors.Add("Password is required");
            
        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required");
            
        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required");
            
        if (request.Age.HasValue && (request.Age < 13 || request.Age > 120))
            errors.Add($"Age must be between 13 and 120, provided: {request.Age}");
            
        return errors.Any() 
            ? MlResult<UserRegistrationRequest>.Fail(errors.ToArray())
            : MlResult<UserRegistrationRequest>.Valid(request);
    }
    
    private async Task<MlResult<UserRegistrationRequest>> ValidateEmailUniquenessAsync(UserRegistrationRequest request)
    {
        try
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            
            if (existingUser != null)
            {
                return MlResult<UserRegistrationRequest>.Fail(
                    $"Email '{request.Email}' is already registered to user ID: {existingUser.Id}");
            }
            
            return MlResult<UserRegistrationRequest>.Valid(request);
        }
        catch (Exception ex)
        {
            return MlResult<UserRegistrationRequest>.Fail($"Email uniqueness validation failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<UserRegistrationRequest>> ValidatePasswordStrengthAsync(UserRegistrationRequest request)
    {
        await Task.Delay(10); // Simular validación asíncrona
        
        var password = request.Password;
        var errors = new List<string>();
        
        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long");
            
        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");
            
        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");
            
        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit");
            
        if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(ch)))
            errors.Add("Password must contain at least one special character");
            
        // Verificar patrones comunes débiles
        var commonWeakPatterns = new[] { "123456", "password", "qwerty", request.FirstName?.ToLower(), request.LastName?.ToLower() };
        
        foreach (var pattern in commonWeakPatterns.Where(p => !string.IsNullOrEmpty(p)))
        {
            if (password.ToLower().Contains(pattern))
                errors.Add($"Password cannot contain common pattern: {pattern}");
        }
        
        return errors.Any()
            ? MlResult<UserRegistrationRequest>.Fail(errors.ToArray())
            : MlResult<UserRegistrationRequest>.Valid(request);
    }
    
    private async Task<MlResult<CreatedUser>> CreateUserAccountAsync(UserRegistrationRequest request)
    {
        try
        {
            var hashedPassword = HashPassword(request.Password);
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLower(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailVerified = false
            };
            
            var savedUser = await _userRepository.CreateUserAsync(user);
            
            if (savedUser == null)
                return MlResult<CreatedUser>.Fail("Failed to save user to database");
                
            var createdUser = new CreatedUser
            {
                User = savedUser,
                OriginalRequest = request,
                CreatedAt = DateTime.UtcNow
            };
            
            return MlResult<CreatedUser>.Valid(createdUser);
        }
        catch (DuplicateEmailException ex)
        {
            return MlResult<CreatedUser>.Fail($"Email already exists: {ex.Email}");
        }
        catch (DatabaseException ex)
        {
            return MlResult<CreatedUser>.Fail($"Database error during user creation: {ex.Message}");
        }
    }
    
    private async Task<MlResult<EmailSentUser>> SendWelcomeEmailAsync(CreatedUser createdUser)
    {
        try
        {
            var emailRequest = new WelcomeEmailRequest
            {
                To = createdUser.User.Email,
                FirstName = createdUser.User.FirstName,
                UserId = createdUser.User.Id,
                VerificationToken = GenerateVerificationToken()
            };
            
            var emailResult = await _emailService.SendWelcomeEmailAsync(emailRequest);
            
            if (!emailResult.Success)
            {
                return MlResult<EmailSentUser>.Fail(
                    $"Failed to send welcome email: {emailResult.ErrorMessage}");
            }
            
            var emailSentUser = new EmailSentUser
            {
                CreatedUser = createdUser,
                EmailSent = true,
                EmailSentAt = DateTime.UtcNow,
                EmailId = emailResult.EmailId
            };
            
            return MlResult<EmailSentUser>.Valid(emailSentUser);
        }
        catch (EmailServiceException ex)
        {
            return MlResult<EmailSentUser>.Fail($"Email service error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Si el email falla, el usuario ya está creado
            _logger.LogWarning($"Welcome email failed for user {createdUser.User.Id}: {ex.Message}");
            
            var emailFailedUser = new EmailSentUser
            {
                CreatedUser = createdUser,
                EmailSent = false,
                EmailSentAt = null,
                EmailId = null
            };
            
            return MlResult<EmailSentUser>.Valid(emailFailedUser);
        }
    }
    
    private async Task<MlResult<RegisteredUser>> FinalizeRegistrationAsync(EmailSentUser emailSentUser)
    {
        try
        {
            // Registrar métricas, logs, etc.
            await _logger.LogUserRegistrationAsync(new UserRegistrationLog
            {
                UserId = emailSentUser.CreatedUser.User.Id,
                Email = emailSentUser.CreatedUser.User.Email,
                RegistrationCompletedAt = DateTime.UtcNow,
                WelcomeEmailSent = emailSentUser.EmailSent
            });
            
            var registeredUser = new RegisteredUser
            {
                User = emailSentUser.CreatedUser.User,
                RegistrationCompletedAt = DateTime.UtcNow,
                WelcomeEmailSent = emailSentUser.EmailSent,
                RequiresEmailVerification = !emailSentUser.EmailSent
            };
            
            return MlResult<RegisteredUser>.Valid(registeredUser);
        }
        catch (Exception ex)
        {
            return MlResult<RegisteredUser>.Fail($"Registration finalization failed: {ex.Message}");
        }
    }
    
    private bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private string HashPassword(string password)
    {
        // Implementación de hashing seguro
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    private string GenerateVerificationToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}

// Clases de apoyo
public class UserRegistrationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? Age { get; set; }
    public string PhoneNumber { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? Age { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
}

public class CreatedUser
{
    public User User { get; set; }
    public UserRegistrationRequest OriginalRequest { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EmailSentUser
{
    public CreatedUser CreatedUser { get; set; }
    public bool EmailSent { get; set; }
    public DateTime? EmailSentAt { get; set; }
    public string EmailId { get; set; }
}

public class RegisteredUser
{
    public User User { get; set; }
    public DateTime RegistrationCompletedAt { get; set; }
    public bool WelcomeEmailSent { get; set; }
    public bool RequiresEmailVerification { get; set; }
}

public class WelcomeEmailRequest
{
    public string To { get; set; }
    public string FirstName { get; set; }
    public Guid UserId { get; set; }
    public string VerificationToken { get; set; }
}

public class UserRegistrationLog
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationCompletedAt { get; set; }
    public bool WelcomeEmailSent { get; set; }
}

// Excepciones personalizadas
public class DuplicateEmailException : Exception
{
    public string Email { get; }
    public DuplicateEmailException(string email) : base($"Email {email} already exists")
    {
        Email = email;
    }
}

public class DatabaseException : Exception
{
    public DatabaseException(string message) : base(message) { }
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}

public class EmailServiceException : Exception
{
    public EmailServiceException(string message) : base(message) { }
    public EmailServiceException(string message, Exception innerException) : base(message, innerException) { }
}
```

### Ejemplo 2: Procesamiento de Archivos con Trazabilidad Completa

```csharp
public class FileProcessingService
{
    private readonly IFileValidator _fileValidator;
    private readonly IFileParser _fileParser;
    private readonly IDataTransformer _dataTransformer;
    private readonly ILogger _logger;
    
    public FileProcessingService(
        IFileValidator fileValidator,
        IFileParser fileParser,
        IDataTransformer dataTransformer,
        ILogger logger)
    {
        _fileValidator = fileValidator;
        _fileParser = fileParser;
        _dataTransformer = dataTransformer;
        _logger = logger;
    }
    
    public async Task<MlResult<ProcessedFileResult>> ProcessFileWithFullTrackingAsync(FileProcessingRequest request)
    {
        var startTime = DateTime.UtcNow;
        var correlationId = Guid.NewGuid().ToString();
        
        _logger.LogInformation($"Starting file processing [CorrelationId: {correlationId}]");
        
        return await ValidateFileRequest(request)
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validRequest => await ValidateFileExistsAsync(validRequest))
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async fileExists => await ReadFileContentAsync(fileExists),
                ex => $"File reading failed: {ex.GetType().Name} - {ex.Message}")
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async fileContent => await ParseFileContentAsync(fileContent),
                ex => $"File parsing failed: {ex.Message}")
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async parsedData => await ValidateDataIntegrityAsync(parsedData))
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async validatedData => await TransformDataAsync(validatedData),
                "Data transformation failed")
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async transformedData => await SaveProcessedDataAsync(transformedData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async savedData => await CreateProcessingResultAsync(savedData, startTime, correlationId));
    }
    
    private MlResult<FileProcessingRequest> ValidateFileRequest(FileProcessingRequest request)
    {
        if (request == null)
            return MlResult<FileProcessingRequest>.Fail("File processing request cannot be null");
            
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(request.FilePath))
            errors.Add("File path is required");
            
        if (string.IsNullOrWhiteSpace(request.OutputDirectory))
            errors.Add("Output directory is required");
            
        if (request.MaxFileSize <= 0)
            errors.Add($"Max file size must be positive, provided: {request.MaxFileSize}");
            
        if (request.AllowedExtensions?.Any() != true)
            errors.Add("At least one allowed extension must be specified");
            
        return errors.Any()
            ? MlResult<FileProcessingRequest>.Fail(errors.ToArray())
            : MlResult<FileProcessingRequest>.Valid(request);
    }
    
    private async Task<MlResult<ValidatedFileInfo>> ValidateFileExistsAsync(FileProcessingRequest request)
    {
        try
        {
            if (!File.Exists(request.FilePath))
                return MlResult<ValidatedFileInfo>.Fail($"File does not exist: {request.FilePath}");
                
            var fileInfo = new FileInfo(request.FilePath);
            
            // Validar tamaño
            if (fileInfo.Length > request.MaxFileSize)
            {
                return MlResult<ValidatedFileInfo>.Fail(
                    $"File size ({fileInfo.Length:N0} bytes) exceeds maximum allowed ({request.MaxFileSize:N0} bytes)");
            }
            
            // Validar extensión
            var extension = fileInfo.Extension.ToLowerInvariant();
            if (!request.AllowedExtensions.Contains(extension))
            {
                return MlResult<ValidatedFileInfo>.Fail(
                    $"File extension '{extension}' is not allowed. Allowed: {string.Join(", ", request.AllowedExtensions)}");
            }
            
            var validatedInfo = new ValidatedFileInfo
            {
                Request = request,
                FileInfo = fileInfo,
                ValidatedAt = DateTime.UtcNow
            };
            
            return MlResult<ValidatedFileInfo>.Valid(validatedInfo);
        }
        catch (Exception ex)
        {
            return MlResult<ValidatedFileInfo>.Fail($"File validation error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<FileContentData>> ReadFileContentAsync(ValidatedFileInfo validatedInfo)
    {
        try
        {
            var content = await File.ReadAllTextAsync(validatedInfo.FileInfo.FullName);
            
            if (string.IsNullOrWhiteSpace(content))
                return MlResult<FileContentData>.Fail("File is empty or contains only whitespace");
                
            var contentData = new FileContentData
            {
                ValidatedInfo = validatedInfo,
                Content = content,
                ContentLength = content.Length,
                ReadAt = DateTime.UtcNow
            };
            
            return MlResult<FileContentData>.Valid(contentData);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new FileAccessException($"Access denied to file: {validatedInfo.FileInfo.FullName}", ex);
        }
        catch (IOException ex)
        {
            throw new FileAccessException($"I/O error reading file: {validatedInfo.FileInfo.FullName}", ex);
        }
    }
    
    private async Task<MlResult<ParsedFileData>> ParseFileContentAsync(FileContentData contentData)
    {
        try
        {
            var parseResult = await _fileParser.ParseAsync(contentData.Content, contentData.ValidatedInfo.FileInfo.Extension);
            
            if (!parseResult.Success)
            {
                return MlResult<ParsedFileData>.Fail(
                    $"File parsing failed: {parseResult.ErrorMessage}. Line: {parseResult.ErrorLine}, Column: {parseResult.ErrorColumn}");
            }
            
            if (parseResult.Data == null || !parseResult.Data.Any())
            {
                return MlResult<ParsedFileData>.Fail("File parsed successfully but contains no data");
            }
            
            var parsedData = new ParsedFileData
            {
                ContentData = contentData,
                ParsedRecords = parseResult.Data,
                RecordCount = parseResult.Data.Count(),
                ParsedAt = DateTime.UtcNow,
                ParsingDuration = parseResult.Duration
            };
            
            return MlResult<ParsedFileData>.Valid(parsedData);
        }
        catch (FormatException ex)
        {
            throw new FileParsingException($"Format error in file: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new FileParsingException($"JSON parsing error: {ex.Message}", ex);
        }
    }
    
    private async Task<MlResult<ValidatedParsedData>> ValidateDataIntegrityAsync(ParsedFileData parsedData)
    {
        try
        {
            var validationResult = await _fileValidator.ValidateDataIntegrityAsync(parsedData.ParsedRecords);
            
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.ValidationErrors.Select(e => 
                    $"Record {e.RecordNumber}: {e.FieldName} - {e.ErrorMessage}").ToArray();
                    
                return MlResult<ValidatedParsedData>.Fail(errorMessages);
            }
            
            var validatedData = new ValidatedParsedData
            {
                ParsedData = parsedData,
                ValidRecords = validationResult.ValidRecords,
                ValidRecordCount = validationResult.ValidRecords.Count(),
                ValidatedAt = DateTime.UtcNow,
                ValidationDuration = validationResult.Duration
            };
            
            return MlResult<ValidatedParsedData>.Valid(validatedData);
        }
        catch (Exception ex)
        {
            return MlResult<ValidatedParsedData>.Fail($"Data validation error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<TransformedData>> TransformDataAsync(ValidatedParsedData validatedData)
    {
        try
        {
            var transformationResult = await _dataTransformer.TransformAsync(validatedData.ValidRecords);
            
            if (!transformationResult.Success)
            {
                return MlResult<TransformedData>.Fail(
                    $"Data transformation failed: {transformationResult.ErrorMessage}");
            }
            
            var transformedData = new TransformedData
            {
                ValidatedData = validatedData,
                TransformedRecords = transformationResult.TransformedRecords,
                TransformedRecordCount = transformationResult.TransformedRecords.Count(),
                TransformedAt = DateTime.UtcNow,
                TransformationDuration = transformationResult.Duration
            };
            
            return MlResult<TransformedData>.Valid(transformedData);
        }
        catch (TransformationException ex)
        {
            throw new DataTransformationException($"Transformation error: {ex.Message}", ex);
        }
    }
    
    private async Task<MlResult<SavedData>> SaveProcessedDataAsync(TransformedData transformedData)
    {
        try
        {
            var outputFileName = GenerateOutputFileName(transformedData.ValidatedData.ParsedData.ContentData.ValidatedInfo);
            var outputPath = Path.Combine(
                transformedData.ValidatedData.ParsedData.ContentData.ValidatedInfo.Request.OutputDirectory,
                outputFileName);
                
            // Crear directorio si no existe
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
                
            await File.WriteAllTextAsync(outputPath, 
                JsonSerializer.Serialize(transformedData.TransformedRecords, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }));
                
            var savedData = new SavedData
            {
                TransformedData = transformedData,
                OutputPath = outputPath,
                SavedAt = DateTime.UtcNow
            };
            
            return MlResult<SavedData>.Valid(savedData);
        }
        catch (IOException ex)
        {
            return MlResult<SavedData>.Fail($"Failed to save processed data: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            return MlResult<SavedData>.Fail($"Access denied when saving data: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ProcessedFileResult>> CreateProcessingResultAsync(
        SavedData savedData, 
        DateTime startTime, 
        string correlationId)
    {
        var endTime = DateTime.UtcNow;
        var totalDuration = endTime - startTime;
        
        var result = new ProcessedFileResult
        {
            CorrelationId = correlationId,
            OriginalFilePath = savedData.TransformedData.ValidatedData.ParsedData.ContentData.ValidatedInfo.FileInfo.FullName,
            OutputFilePath = savedData.OutputPath,
            TotalRecordsProcessed = savedData.TransformedData.TransformedRecordCount,
            StartTime = startTime,
            EndTime = endTime,
            TotalDuration = totalDuration,
            ProcessingSteps = new ProcessingStepsInfo
            {
                ParsingDuration = savedData.TransformedData.ValidatedData.ParsedData.ParsingDuration,
                ValidationDuration = savedData.TransformedData.ValidatedData.ValidationDuration,
                TransformationDuration = savedData.TransformedData.TransformationDuration
            }
        };
        
        _logger.LogInformation($"File processing completed successfully [CorrelationId: {correlationId}] in {totalDuration.TotalSeconds:F2}s");
        
        return MlResult<ProcessedFileResult>.Valid(result);
    }
    
    private string GenerateOutputFileName(ValidatedFileInfo validatedInfo)
    {
        var originalName = Path.GetFileNameWithoutExtension(validatedInfo.FileInfo.Name);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        return $"{originalName}_processed_{timestamp}.json";
    }
}

// Clases de apoyo y modelos de datos
public class FileProcessingRequest
{
    public string FilePath { get; set; }
    public string OutputDirectory { get; set; }
    public long MaxFileSize { get; set; }
    public string[] AllowedExtensions { get; set; }
}

public class ValidatedFileInfo
{
    public FileProcessingRequest Request { get; set; }
    public FileInfo FileInfo { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class FileContentData
{
    public ValidatedFileInfo ValidatedInfo { get; set; }
    public string Content { get; set; }
    public int ContentLength { get; set; }
    public DateTime ReadAt { get; set; }
}

public class ParsedFileData
{
    public FileContentData ContentData { get; set; }
    public IEnumerable<object> ParsedRecords { get; set; }
    public int RecordCount { get; set; }
    public DateTime ParsedAt { get; set; }
    public TimeSpan ParsingDuration { get; set; }
}

public class ValidatedParsedData
{
    public ParsedFileData ParsedData { get; set; }
    public IEnumerable<object> ValidRecords { get; set; }
    public int ValidRecordCount { get; set; }
    public DateTime ValidatedAt { get; set; }
    public TimeSpan ValidationDuration { get; set; }
}

public class TransformedData
{
    public ValidatedParsedData ValidatedData { get; set; }
    public IEnumerable<object> TransformedRecords { get; set; }
    public int TransformedRecordCount { get; set; }
    public DateTime TransformedAt { get; set; }
    public TimeSpan TransformationDuration { get; set; }
}

public class SavedData
{
    public TransformedData TransformedData { get; set; }
    public string OutputPath { get; set; }
    public DateTime SavedAt { get; set; }
}

public class ProcessedFileResult
{
    public string CorrelationId { get; set; }
    public string OriginalFilePath { get; set; }
    public string OutputFilePath { get; set; }
    public int TotalRecordsProcessed { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public ProcessingStepsInfo ProcessingSteps { get; set; }
}

public class ProcessingStepsInfo
{
    public TimeSpan ParsingDuration { get; set; }
    public TimeSpan ValidationDuration { get; set; }
    public TimeSpan TransformationDuration { get; set; }
}

// Excepciones personalizadas
public class FileAccessException : Exception
{
    public FileAccessException(string message) : base(message) { }
    public FileAccessException(string message, Exception innerException) : base(message, innerException) { }
}

public class FileParsingException : Exception
{
    public FileParsingException(string message) : base(message) { }
    public FileParsingException(string message, Exception innerException) : base(message, innerException) { }
}

public class DataTransformationException : Exception
{
    public DataTransformationException(string message) : base(message) { }
    public DataTransformationException(string message, Exception innerException) : base(message, innerException) { }
}
```

### Ejemplo 3: Sistema de Análisis de Errores con Valores Preservados

```csharp
public class ErrorAnalysisService
{
    private readonly ILogger _logger;
    private readonly IErrorRepository _errorRepository;
    
    public ErrorAnalysisService(ILogger logger, IErrorRepository errorRepository)
    {
        _logger = logger;
        _errorRepository = errorRepository;
    }
    
    public async Task<MlResult<ErrorAnalysisReport>> AnalyzeSystemErrorsAsync(ErrorAnalysisRequest request)
    {
        return await ValidateAnalysisRequest(request)
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validRequest => await RetrieveErrorsAsync(validRequest))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async errorData => await CategorizeErrorsAsync(errorData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async categorizedData => await AnalyzeErrorPatternsAsync(categorizedData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async patternData => await GenerateRecommendationsAsync(patternData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async recommendationData => await CreateAnalysisReportAsync(recommendationData));
    }
    
    public MlResult<ErrorDetailsWithContext> AnalyzeSpecificErrorWithContext(ErrorOccurrence error)
    {
        return ValidateErrorOccurrence(error)
            .BindSaveValueInDetailsIfFaildFuncResult(validError => ExtractErrorContext(validError))
            .BindSaveValueInDetailsIfFaildFuncResult(contextData => AnalyzeErrorCause(contextData))
            .BindSaveValueInDetailsIfFaildFuncResult(causeData => DetermineErrorSeverity(causeData))
            .BindSaveValueInDetailsIfFaildFuncResult(severityData => CreateErrorDetailsWithContext(severityData));
    }
    
    private MlResult<ErrorAnalysisRequest> ValidateAnalysisRequest(ErrorAnalysisRequest request)
    {
        if (request == null)
            return MlResult<ErrorAnalysisRequest>.Fail("Analysis request cannot be null");
            
        var errors = new List<string>();
        
        if (request.StartDate >= request.EndDate)
            errors.Add($"Start date ({request.StartDate:yyyy-MM-dd}) must be before end date ({request.EndDate:yyyy-MM-dd})");
            
        if ((request.EndDate - request.StartDate).TotalDays > 365)
            errors.Add($"Analysis period cannot exceed 365 days. Requested: {(request.EndDate - request.StartDate).TotalDays:F0} days");
            
        if (request.MaxErrorCount <= 0)
            errors.Add($"Max error count must be positive. Provided: {request.MaxErrorCount}");
            
        if (request.ErrorSources?.Any() != true)
            errors.Add("At least one error source must be specified");
            
        return errors.Any()
            ? MlResult<ErrorAnalysisRequest>.Fail(errors.ToArray())
            : MlResult<ErrorAnalysisRequest>.Valid(request);
    }
    
    private async Task<MlResult<RetrievedErrorData>> RetrieveErrorsAsync(ErrorAnalysisRequest request)
    {
        try
        {
            var errors = await _errorRepository.GetErrorsAsync(request.StartDate, request.EndDate, request.ErrorSources);
            
            if (!errors.Any())
            {
                return MlResult<RetrievedErrorData>.Fail(
                    $"No errors found for the specified period ({request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}) " +
                    $"and sources: {string.Join(", ", request.ErrorSources)}");
            }
            
            if (errors.Count() > request.MaxErrorCount)
            {
                var limitedErrors = errors.Take(request.MaxErrorCount).ToList();
                _logger.LogWarning($"Error count ({errors.Count():N0}) exceeds maximum ({request.MaxErrorCount:N0}). Limited to first {request.MaxErrorCount:N0} errors.");
                
                var retrievedData = new RetrievedErrorData
                {
                    Request = request,
                    Errors = limitedErrors,
                    TotalErrorsFound = errors.Count(),
                    ErrorsIncluded = limitedErrors.Count,
                    WasTruncated = true,
                    RetrievedAt = DateTime.UtcNow
                };
                
                return MlResult<RetrievedErrorData>.Valid(retrievedData);
            }
            else
            {
                var retrievedData = new RetrievedErrorData
                {
                    Request = request,
                    Errors = errors.ToList(),
                    TotalErrorsFound = errors.Count(),
                    ErrorsIncluded = errors.Count(),
                    WasTruncated = false,
                    RetrievedAt = DateTime.UtcNow
                };
                
                return MlResult<RetrievedErrorData>.Valid(retrievedData);
            }
        }
        catch (Exception ex)
        {
            return MlResult<RetrievedErrorData>.Fail($"Failed to retrieve errors: {ex.Message}");
        }
    }
    
    private async Task<MlResult<CategorizedErrorData>> CategorizeErrorsAsync(RetrievedErrorData errorData)
    {
        try
        {
            var categories = new Dictionary<string, List<ErrorOccurrence>>();
            var uncategorized = new List<ErrorOccurrence>();
            
            foreach (var error in errorData.Errors)
            {
                var category = DetermineErrorCategory(error);
                
                if (string.IsNullOrEmpty(category))
                {
                    uncategorized.Add(error);
                    continue;
                }
                
                if (!categories.ContainsKey(category))
                    categories[category] = new List<ErrorOccurrence>();
                    
                categories[category].Add(error);
            }
            
            if (uncategorized.Count > errorData.ErrorsIncluded * 0.3) // Más del 30% sin categorizar
            {
                return MlResult<CategorizedErrorData>.Fail(
                    $"Too many uncategorized errors ({uncategorized.Count}/{errorData.ErrorsIncluded}). " +
                    "This may indicate unknown error patterns or categorization logic issues.");
            }
            
            var categorizedData = new CategorizedErrorData
            {
                RetrievedData = errorData,
                ErrorCategories = categories,
                UncategorizedErrors = uncategorized,
                CategorizedAt = DateTime.UtcNow,
                CategoryCount = categories.Count
            };
            
            return MlResult<CategorizedErrorData>.Valid(categorizedData);
        }
        catch (Exception ex)
        {
            return MlResult<CategorizedErrorData>.Fail($"Error categorization failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<PatternAnalysisData>> AnalyzeErrorPatternsAsync(CategorizedErrorData categorizedData)
    {
        try
        {
            var patterns = new List<ErrorPattern>();
            
            foreach (var category in categorizedData.ErrorCategories)
            {
                var pattern = await AnalyzeCategoryPatternAsync(category.Key, category.Value);
                if (pattern != null)
                    patterns.Add(pattern);
            }
            
            // Analizar patrones temporales
            var temporalPatterns = AnalyzeTemporalPatterns(categorizedData.RetrievedData.Errors);
            
            // Analizar correlaciones entre categorías
            var correlations = AnalyzeCategoryCorrelations(categorizedData.ErrorCategories);
            
            if (!patterns.Any() && !temporalPatterns.Any())
            {
                return MlResult<PatternAnalysisData>.Fail(
                    "No significant error patterns detected. This could indicate either very random errors " +
                    "or insufficient data for pattern analysis.");
            }
            
            var patternData = new PatternAnalysisData
            {
                CategorizedData = categorizedData,
                ErrorPatterns = patterns,
                TemporalPatterns = temporalPatterns,
                CategoryCorrelations = correlations,
                AnalyzedAt = DateTime.UtcNow,
                SignificantPatternCount = patterns.Count(p => p.Significance > 0.7)
            };
            
            return MlResult<PatternAnalysisData>.Valid(patternData);
        }
        catch (Exception ex)
        {
            return MlResult<PatternAnalysisData>.Fail($"Pattern analysis failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<RecommendationData>> GenerateRecommendationsAsync(PatternAnalysisData patternData)
    {
        try
        {
            var recommendations = new List<ErrorRecommendation>();
            
            // Generar recomendaciones basadas en patrones
            foreach (var pattern in patternData.ErrorPatterns.Where(p => p.Significance > 0.5))
            {
                var recommendation = await GeneratePatternRecommendationAsync(pattern);
                if (recommendation != null)
                    recommendations.Add(recommendation);
            }
            
            // Generar recomendaciones temporales
            foreach (var temporalPattern in patternData.TemporalPatterns)
            {
                var recommendation = GenerateTemporalRecommendation(temporalPattern);
                if (recommendation != null)
                    recommendations.Add(recommendation);
            }
            
            // Priorizar recomendaciones
            var prioritizedRecommendations = PrioritizeRecommendations(recommendations);
            
            if (!prioritizedRecommendations.Any())
            {
                return MlResult<RecommendationData>.Fail(
                    "No actionable recommendations could be generated from the error patterns. " +
                    "This may indicate that the errors are too diverse or the patterns are not significant enough.");
            }
            
            var recommendationData = new RecommendationData
            {
                PatternData = patternData,
                Recommendations = prioritizedRecommendations,
                GeneratedAt = DateTime.UtcNow,
                HighPriorityCount = prioritizedRecommendations.Count(r => r.Priority == RecommendationPriority.High)
            };
            
            return MlResult<RecommendationData>.Valid(recommendationData);
        }
        catch (Exception ex)
        {
            return MlResult<RecommendationData>.Fail($"Recommendation generation failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ErrorAnalysisReport>> CreateAnalysisReportAsync(RecommendationData recommendationData)
    {
        try
        {
            var report = new ErrorAnalysisReport
            {
                AnalysisId = Guid.NewGuid(),
                Request = recommendationData.PatternData.CategorizedData.RetrievedData.Request,
                Summary = CreateAnalysisSummary(recommendationData),
                ErrorCategories = recommendationData.PatternData.CategorizedData.ErrorCategories.Keys.ToList(),
                SignificantPatterns = recommendationData.PatternData.ErrorPatterns.Where(p => p.Significance > 0.7).ToList(),
                Recommendations = recommendationData.Recommendations,
                GeneratedAt = DateTime.UtcNow,
                DataQuality = AssessDataQuality(recommendationData.PatternData.CategorizedData.RetrievedData)
            };
            
            // Guardar reporte para referencia futura
            await _errorRepository.SaveAnalysisReportAsync(report);
            
            return MlResult<ErrorAnalysisReport>.Valid(report);
        }
        catch (Exception ex)
        {
            return MlResult<ErrorAnalysisReport>.Fail($"Report creation failed: {ex.Message}");
        }
    }
    
    // Métodos auxiliares específicos del análisis de errores...
    private string DetermineErrorCategory(ErrorOccurrence error)
    {
        // Lógica para categorizar errores basada en mensaje, stack trace, etc.
        if (error.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
            error.Message.Contains("connection", StringComparison.OrdinalIgnoreCase))
            return "Network/Connectivity";
            
        if (error.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
            error.Message.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
            return "Authentication/Authorization";
            
        if (error.Message.Contains("null", StringComparison.OrdinalIgnoreCase) ||
            error.StackTrace?.Contains("NullReferenceException") == true)
            return "Null Reference";
            
        if (error.Message.Contains("database", StringComparison.OrdinalIgnoreCase) ||
            error.Message.Contains("sql", StringComparison.OrdinalIgnoreCase))
            return "Database";
            
        if (error.Message.Contains("validation", StringComparison.OrdinalIgnoreCase) ||
            error.Message.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            return "Validation";
            
        return "Other";
    }
    
    private List<TemporalPattern> AnalyzeTemporalPatterns(List<ErrorOccurrence> errors)
    {
        var patterns = new List<TemporalPattern>();
        
        // Analizar patrones por hora del día
        var hourlyDistribution = errors.GroupBy(e => e.OccurredAt.Hour)
            .ToDictionary(g => g.Key, g => g.Count());
            
        var avgPerHour = errors.Count / 24.0;
        var significantHours = hourlyDistribution.Where(h => h.Value > avgPerHour * 1.5).ToList();
        
        if (significantHours.Any())
        {
            patterns.Add(new TemporalPattern
            {
                Type = "Hourly Peak",
                Description = $"Significant error peaks at hours: {string.Join(", ", significantHours.Select(h => h.Key))}",
                Significance = 0.8
            });
        }
        
        // Analizar patrones por día de la semana
        var dailyDistribution = errors.GroupBy(e => e.OccurredAt.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Count());
            
        var avgPerDay = errors.Count / 7.0;
        var significantDays = dailyDistribution.Where(d => d.Value > avgPerDay * 1.3).ToList();
        
        if (significantDays.Any())
        {
            patterns.Add(new TemporalPattern
            {
                Type = "Weekly Pattern",
                Description = $"Higher error rates on: {string.Join(", ", significantDays.Select(d => d.Key))}",
                Significance = 0.7
            });
        }
        
        return patterns;
    }
    
    private ErrorAnalysisSummary CreateAnalysisSummary(RecommendationData data)
    {
        return new ErrorAnalysisSummary
        {
            TotalErrorsAnalyzed = data.PatternData.CategorizedData.RetrievedData.ErrorsIncluded,
            CategoriesFound = data.PatternData.CategorizedData.CategoryCount,
            SignificantPatternsFound = data.PatternData.SignificantPatternCount,
            RecommendationsGenerated = data.Recommendations.Count,
            HighPriorityRecommendations = data.HighPriorityCount,
            AnalysisPeriod = $"{data.PatternData.CategorizedData.RetrievedData.Request.StartDate:yyyy-MM-dd} to {data.PatternData.CategorizedData.RetrievedData.Request.EndDate:yyyy-MM-dd}"
        };
    }
    
    // ... más métodos auxiliares
}

// Clases de apoyo para análisis de errores
public class ErrorAnalysisRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string[] ErrorSources { get; set; }
    public int MaxErrorCount { get; set; }
}

public class ErrorOccurrence
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Source { get; set; }
    public DateTime OccurredAt { get; set; }
    public string UserId { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class RetrievedErrorData
{
    public ErrorAnalysisRequest Request { get; set; }
    public List<ErrorOccurrence> Errors { get; set; }
    public int TotalErrorsFound { get; set; }
    public int ErrorsIncluded { get; set; }
    public bool WasTruncated { get; set; }
    public DateTime RetrievedAt { get; set; }
}

public class CategorizedErrorData
{
    public RetrievedErrorData RetrievedData { get; set; }
    public Dictionary<string, List<ErrorOccurrence>> ErrorCategories { get; set; }
    public List<ErrorOccurrence> UncategorizedErrors { get; set; }
    public DateTime CategorizedAt { get; set; }
    public int CategoryCount { get; set; }
}

public class ErrorPattern
{
    public string Category { get; set; }
    public string Description { get; set; }
    public double Significance { get; set; }
    public int OccurrenceCount { get; set; }
    public List<string> CommonElements { get; set; }
}

public class TemporalPattern
{
    public string Type { get; set; }
    public string Description { get; set; }
    public double Significance { get; set; }
}

public class PatternAnalysisData
{
    public CategorizedErrorData CategorizedData { get; set; }
    public List<ErrorPattern> ErrorPatterns { get; set; }
    public List<TemporalPattern> TemporalPatterns { get; set; }
    public Dictionary<string, List<string>> CategoryCorrelations { get; set; }
    public DateTime AnalyzedAt { get; set; }
    public int SignificantPatternCount { get; set; }
}

public class ErrorRecommendation
{
    public string Title { get; set; }
    public string Description { get; set; }
    public RecommendationPriority Priority { get; set; }
    public string Category { get; set; }
    public List<string> ActionItems { get; set; }
    public TimeSpan EstimatedImplementationTime { get; set; }
}

public class RecommendationData
{
    public PatternAnalysisData PatternData { get; set; }
    public List<ErrorRecommendation> Recommendations { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int HighPriorityCount { get; set; }
}

public class ErrorAnalysisReport
{
    public Guid AnalysisId { get; set; }
    public ErrorAnalysisRequest Request { get; set; }
    public ErrorAnalysisSummary Summary { get; set; }
    public List<string> ErrorCategories { get; set; }
    public List<ErrorPattern> SignificantPatterns { get; set; }
    public List<ErrorRecommendation> Recommendations { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DataQualityAssessment DataQuality { get; set; }
}

public class ErrorAnalysisSummary
{
    public int TotalErrorsAnalyzed { get; set; }
    public int CategoriesFound { get; set; }
    public int SignificantPatternsFound { get; set; }
    public int RecommendationsGenerated { get; set; }
    public int HighPriorityRecommendations { get; set; }
    public string AnalysisPeriod { get; set; }
}

public enum RecommendationPriority
{
    Low,
    Medium,
    High,
    Critical
}

public class DataQualityAssessment
{
    public double CompletenessScore { get; set; }
    public double ConsistencyScore { get; set; }
    public double ReliabilityScore { get; set; }
    public List<string> QualityIssues { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar BindSaveValueInDetails

```csharp
// ✅ Correcto: Usar cuando necesites debugging y trazabilidad
var result = ParseUserInput(input)
    .BindSaveValueInDetailsIfFaildFuncResult(parsedData => ValidateBusinessRules(parsedData))
    .BindSaveValueInDetailsIfFaildFuncResult(validData => TransformToDto(validData));

// En caso de error, tendrás acceso al valor original que causó el problema

// ✅ Correcto: Para transformaciones complejas donde el valor original es importante
var result = await GetComplexObject()
    .BindSaveValueInDetailsIfFaildFuncResultAsync(async obj => await ProcessComplexTransformation(obj));

// ❌ Incorrecto: Para transformaciones simples donde el valor no añade información
var result = GetNumber()
    .BindSave// filepath: c:\PakkkoTFS\MoralesLarios\FOOP\MoralesLarios.FOOP\docs\MlResultActionsBindSaveValueInDetails.md
# MlResultActionsBindSaveValueInDetails - Operaciones de Binding con Preservación de Valor

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos BindSaveValueInDetailsIfFaildFuncResult](#métodos-bindsavevalueindetailsiffaildFuncresult)
4. [Métodos TryBindSaveValueInDetailsIfFaildFuncResult](#métodos-trybindsavevalueindetailsiffaildFuncresult)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsBindSaveValueInDetails` contiene operaciones especializadas de **binding con preservación de valor en caso de fallo**. Estos métodos extienden el comportamiento del binding tradicional añadiendo automáticamente el valor original como detalle de error cuando la función de transformación falla, proporcionando **trazabilidad completa** del valor que causó el error.

### Propósito Principal

- **Preservación de Contexto**: Mantener el valor original cuando una transformación falla
- **Debugging Mejorado**: Facilitar la depuración al incluir el valor que causó el error
- **Trazabilidad**: Crear un rastro completo de valores a través del pipeline
- **Análisis de Fallos**: Permitir análisis posterior de los valores que causaron errores
- **Auditoría Detallada**: Registrar información completa para auditorías

---

## Análisis de la Clase

### Estructura y Filosofía

Estos métodos implementan un patrón de **binding con memoria**, donde el valor original se preserva en los detalles del error:

```
Valor Original → Función de Transformación → ¿Éxito?
      ↓                      ↓                 ↓
   Preservar            Retornar Éxito    Añadir Valor a Error
      ↓                                         ↓
   En Error Details ←――――――――――――――――――――――――――――
```

### Características Principales

1. **Binding Condicional**: Solo ejecuta la función si el resultado fuente es válido
2. **Preservación Automática**: Añade el valor original a los detalles de error automáticamente
3. **Transparencia en Éxito**: Comportamiento idéntico al binding normal cuando hay éxito
4. **Compatibilidad Completa**: Versiones síncronas, asíncronas y seguras (Try)

---

## Métodos BindSaveValueInDetailsIfFaildFuncResult

### `BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Ejecuta la función de transformación si el resultado es válido, y añade el valor original a los detalles de error si la función falla

```csharp
public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func)
```

**Parámetros**:
- `source`: El resultado origen a evaluar
- `func`: Función que transforma el valor si `source` es válido

**Comportamiento**:
- Si `source` es fallido: Propaga el error sin ejecutar `func`
- Si `source` es válido y `func` tiene éxito: Retorna el resultado de `func`
- Si `source` es válido y `func` falla: Retorna el error de `func` + valor original en detalles

**Ejemplo Básico**:
```csharp
var userAge = MlResult<int>.Valid(25);

var validationResult = userAge.BindSaveValueInDetailsIfFaildFuncResult(age => 
{
    if (age < 18)
        return MlResult<string>.Fail($"Age {age} is below minimum required");
    if (age > 150)
        return MlResult<string>.Fail($"Age {age} is unrealistic");
        
    return MlResult<string>.Valid($"Valid age: {age}");
});

// Si age = 15, el error incluirá:
// - Mensaje: "Age 15 is below minimum required"
// - Detalles: Incluye el valor original (15) para análisis posterior
```

### Versiones Asíncronas

#### `BindSaveValueInDetailsIfFaildFuncResultAsync()` - Función Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)
```

**Ejemplo**:
```csharp
var emailData = MlResult<EmailRequest>.Valid(new EmailRequest { To = "user@example.com" });

var sendResult = await emailData.BindSaveValueInDetailsIfFaildFuncResultAsync(async email => 
{
    var isValid = await ValidateEmailAsync(email.To);
    if (!isValid)
        return MlResult<EmailResponse>.Fail($"Invalid email format: {email.To}");
        
    return await SendEmailAsync(email);
});

// Si la validación falla, el error incluye el EmailRequest completo en los detalles
```

#### `BindSaveValueInDetailsIfFaildFuncResultAsync()` - Fuente Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)

public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, MlResult<TReturn>> func)
```

---

## Métodos TryBindSaveValueInDetailsIfFaildFuncResult

### `TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Versión segura que captura excepciones, las convierte en errores y preserva el valor original

```csharp
public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    Func<Exception, string> errorMessageBuilder)

public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    string errorMessage = null!)
```

**Comportamiento**:
- Comportamiento idéntico al método básico
- **Además**: Captura excepciones de `func` y las convierte en `MlResult` fallido
- El valor original se añade a los detalles tanto para errores de `func` como para excepciones

**Ejemplo**:
```csharp
var jsonData = MlResult<string>.Valid("""{"name": "John", "age": "invalid"}""");

var parseResult = jsonData.TryBindSaveValueInDetailsIfFaildFuncResult(json => 
{
    // Esta función puede lanzar JsonException
    var document = JsonDocument.Parse(json);
    var person = JsonSerializer.Deserialize<Person>(json);
    
    if (person.Age < 0)
        return MlResult<Person>.Fail("Age cannot be negative");
        
    return MlResult<Person>.Valid(person);
}, ex => $"JSON parsing failed: {ex.Message}");

// Si JsonDocument.Parse lanza excepción:
// - Error: "JSON parsing failed: [exception message]"
// - Detalles: Incluye el JSON original para análisis
```

### Versiones Asíncronas de TryBind

#### Conversión a Asíncrono
```csharp
public static Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    Func<Exception, string> errorMessageBuilder)
```

#### Función Asíncrona Segura
```csharp
public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync,
    Func<Exception, string> errorMessageBuilder)
```

**Ejemplo con función asíncrona**:
```csharp
var filePathResult = GetFilePathAsync();

var processResult = await filePathResult.TryBindSaveValueInDetailsIfFaildFuncResultAsync(
    async filePath => 
    {
        // Puede lanzar FileNotFoundException, UnauthorizedAccessException, etc.
        var content = await File.ReadAllTextAsync(filePath);
        
        if (string.IsNullOrWhiteSpace(content))
            return MlResult<ProcessedFile>.Fail("File is empty");
            
        return await ProcessFileContentAsync(content);
    },
    ex => $"File processing failed: {ex.GetType().Name} - {ex.Message}"
);

// Si hay excepción, el error incluye:
// - Mensaje personalizado basado en la excepción
// - El path del archivo original en los detalles
```

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Fuente | Función | Manejo Excepciones | Método |
|--------|---------|-------------------|---------|
| `MlResult<T>` | `T → MlResult<U>` | No | `BindSaveValueInDetailsIfFaildFuncResult` |
| `MlResult<T>` | `T → MlResult<U>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResult` |
| `MlResult<T>` | `T → Task<MlResult<U>>` | No | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `MlResult<T>` | `T → Task<MlResult<U>>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → MlResult<U>` | No | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → MlResult<U>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | No | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | Sí | `TryBindSaveValueInDetailsIfFaildFuncResultAsync` |

---

## Ejemplos Prácticos

### Ejemplo 1: Pipeline de Validación con Trazabilidad

```csharp
public class UserRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;
    
    public UserRegistrationService(
        IUserRepository userRepository, 
        IEmailService emailService, 
        ILogger logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task<MlResult<RegisteredUser>> RegisterUserWithDetailedTrackingAsync(UserRegistrationRequest request)
    {
        return await ValidateRegistrationRequest(request)
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validRequest => await ValidateEmailUniquenessAsync(validRequest))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async emailValidatedRequest => await ValidatePasswordStrengthAsync(emailValidatedRequest))
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async passwordValidatedRequest => await CreateUserAccountAsync(passwordValidatedRequest),
                ex => $"Account creation failed: {ex.Message}")
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async createdUser => await SendWelcomeEmailAsync(createdUser),
                "Welcome email sending failed")
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async emailSentUser => await FinalizeRegistrationAsync(emailSentUser));
    }
    
    private MlResult<UserRegistrationRequest> ValidateRegistrationRequest(UserRegistrationRequest request)
    {
        if (request == null)
            return MlResult<UserRegistrationRequest>.Fail("Registration request cannot be null");
            
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add("Email is required");
        else if (!IsValidEmailFormat(request.Email))
            errors.Add($"Email format is invalid: {request.Email}");
            
        if (string.IsNullOrWhiteSpace(request.Password))
            errors.Add("Password is required");
            
        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required");
            
        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required");
            
        if (request.Age.HasValue && (request.Age < 13 || request.Age > 120))
            errors.Add($"Age must be between 13 and 120, provided: {request.Age}");
            
        return errors.Any() 
            ? MlResult<UserRegistrationRequest>.Fail(errors.ToArray())
            : MlResult<UserRegistrationRequest>.Valid(request);
    }
    
    private async Task<MlResult<UserRegistrationRequest>> ValidateEmailUniquenessAsync(UserRegistrationRequest request)
    {
        try
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            
            if (existingUser != null)
            {
                return MlResult<UserRegistrationRequest>.Fail(
                    $"Email '{request.Email}' is already registered to user ID: {existingUser.Id}");
            }
            
            return MlResult<UserRegistrationRequest>.Valid(request);
        }
        catch (Exception ex)
        {
            return MlResult<UserRegistrationRequest>.Fail($"Email uniqueness validation failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<UserRegistrationRequest>> ValidatePasswordStrengthAsync(UserRegistrationRequest request)
    {
        await Task.Delay(10); // Simular validación asíncrona
        
        var password = request.Password;
        var errors = new List<string>();
        
        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long");
            
        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");
            
        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");
            
        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit");
            
        if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(ch)))
            errors.Add("Password must contain at least one special character");
            
        // Verificar patrones comunes débiles
        var commonWeakPatterns = new[] { "123456", "password", "qwerty", request.FirstName?.ToLower(), request.LastName?.ToLower() };
        
        foreach (var pattern in commonWeakPatterns.Where(p => !string.IsNullOrEmpty(p)))
        {
            if (password.ToLower().Contains(pattern))
                errors.Add($"Password cannot contain common pattern: {pattern}");
        }
        
        return errors.Any()
            ? MlResult<UserRegistrationRequest>.Fail(errors.ToArray())
            : MlResult<UserRegistrationRequest>.Valid(request);
    }
    
    private async Task<MlResult<CreatedUser>> CreateUserAccountAsync(UserRegistrationRequest request)
    {
        try
        {
            var hashedPassword = HashPassword(request.Password);
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLower(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailVerified = false
            };
            
            var savedUser = await _userRepository.CreateUserAsync(user);
            
            if (savedUser == null)
                return MlResult<CreatedUser>.Fail("Failed to save user to database");
                
            var createdUser = new CreatedUser
            {
                User = savedUser,
                OriginalRequest = request,
                CreatedAt = DateTime.UtcNow
            };
            
            return MlResult<CreatedUser>.Valid(createdUser);
        }
        catch (DuplicateEmailException ex)
        {
            return MlResult<CreatedUser>.Fail($"Email already exists: {ex.Email}");
        }
        catch (DatabaseException ex)
        {
            return MlResult<CreatedUser>.Fail($"Database error during user creation: {ex.Message}");
        }
    }
    
    private async Task<MlResult<EmailSentUser>> SendWelcomeEmailAsync(CreatedUser createdUser)
    {
        try
        {
            var emailRequest = new WelcomeEmailRequest
            {
                To = createdUser.User.Email,
                FirstName = createdUser.User.FirstName,
                UserId = createdUser.User.Id,
                VerificationToken = GenerateVerificationToken()
            };
            
            var emailResult = await _emailService.SendWelcomeEmailAsync(emailRequest);
            
            if (!emailResult.Success)
            {
                return MlResult<EmailSentUser>.Fail(
                    $"Failed to send welcome email: {emailResult.ErrorMessage}");
            }
            
            var emailSentUser = new EmailSentUser
            {
                CreatedUser = createdUser,
                EmailSent = true,
                EmailSentAt = DateTime.UtcNow,
                EmailId = emailResult.EmailId
            };
            
            return MlResult<EmailSentUser>.Valid(emailSentUser);
        }
        catch (EmailServiceException ex)
        {
            return MlResult<EmailSentUser>.Fail($"Email service error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Si el email falla, el usuario ya está creado
            _logger.LogWarning($"Welcome email failed for user {createdUser.User.Id}: {ex.Message}");
            
            var emailFailedUser = new EmailSentUser
            {
                CreatedUser = createdUser,
                EmailSent = false,
                EmailSentAt = null,
                EmailId = null
            };
            
            return MlResult<EmailSentUser>.Valid(emailFailedUser);
        }
    }
    
    private async Task<MlResult<RegisteredUser>> FinalizeRegistrationAsync(EmailSentUser emailSentUser)
    {
        try
        {
            // Registrar métricas, logs, etc.
            await _logger.LogUserRegistrationAsync(new UserRegistrationLog
            {
                UserId = emailSentUser.CreatedUser.User.Id,
                Email = emailSentUser.CreatedUser.User.Email,
                RegistrationCompletedAt = DateTime.UtcNow,
                WelcomeEmailSent = emailSentUser.EmailSent
            });
            
            var registeredUser = new RegisteredUser
            {
                User = emailSentUser.CreatedUser.User,
                RegistrationCompletedAt = DateTime.UtcNow,
                WelcomeEmailSent = emailSentUser.EmailSent,
                RequiresEmailVerification = !emailSentUser.EmailSent
            };
            
            return MlResult<RegisteredUser>.Valid(registeredUser);
        }
        catch (Exception ex)
        {
            return MlResult<RegisteredUser>.Fail($"Registration finalization failed: {ex.Message}");
        }
    }
    
    private bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private string HashPassword(string password)
    {
        // Implementación de hashing seguro
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    private string GenerateVerificationToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}

// Clases de apoyo
public class UserRegistrationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? Age { get; set; }
    public string PhoneNumber { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? Age { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
}

public class CreatedUser
{
    public User User { get; set; }
    public UserRegistrationRequest OriginalRequest { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EmailSentUser
{
    public CreatedUser CreatedUser { get; set; }
    public bool EmailSent { get; set; }
    public DateTime? EmailSentAt { get; set; }
    public string EmailId { get; set; }
}

public class RegisteredUser
{
    public User User { get; set; }
    public DateTime RegistrationCompletedAt { get; set; }
    public bool WelcomeEmailSent { get; set; }
    public bool RequiresEmailVerification { get; set; }
}

public class WelcomeEmailRequest
{
    public string To { get; set; }
    public string FirstName { get; set; }
    public Guid UserId { get; set; }
    public string VerificationToken { get; set; }
}

public class UserRegistrationLog
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationCompletedAt { get; set; }
    public bool WelcomeEmailSent { get; set; }
}

// Excepciones personalizadas
public class DuplicateEmailException : Exception
{
    public string Email { get; }
    public DuplicateEmailException(string email) : base($"Email {email} already exists")
    {
        Email = email;
    }
}

public class DatabaseException : Exception
{
    public DatabaseException(string message) : base(message) { }
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}

public class EmailServiceException : Exception
{
    public EmailServiceException(string message) : base(message) { }
    public EmailServiceException(string message, Exception innerException) : base(message, innerException) { }
}
```

### Ejemplo 2: Procesamiento de Archivos con Trazabilidad Completa

```csharp
public class FileProcessingService
{
    private readonly IFileValidator _fileValidator;
    private readonly IFileParser _fileParser;
    private readonly IDataTransformer _dataTransformer;
    private readonly ILogger _logger;
    
    public FileProcessingService(
        IFileValidator fileValidator,
        IFileParser fileParser,
        IDataTransformer dataTransformer,
        ILogger logger)
    {
        _fileValidator = fileValidator;
        _fileParser = fileParser;
        _dataTransformer = dataTransformer;
        _logger = logger;
    }
    
    public async Task<MlResult<ProcessedFileResult>> ProcessFileWithFullTrackingAsync(FileProcessingRequest request)
    {
        var startTime = DateTime.UtcNow;
        var correlationId = Guid.NewGuid().ToString();
        
        _logger.LogInformation($"Starting file processing [CorrelationId: {correlationId}]");
        
        return await ValidateFileRequest(request)
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validRequest => await ValidateFileExistsAsync(validRequest))
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async fileExists => await ReadFileContentAsync(fileExists),
                ex => $"File reading failed: {ex.GetType().Name} - {ex.Message}")
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async fileContent => await ParseFileContentAsync(fileContent),
                ex => $"File parsing failed: {ex.Message}")
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async parsedData => await ValidateDataIntegrityAsync(parsedData))
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async validatedData => await TransformDataAsync(validatedData),
                "Data transformation failed")
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async transformedData => await SaveProcessedDataAsync(transformedData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async savedData => await CreateProcessingResultAsync(savedData, startTime, correlationId));
    }
    
    private MlResult<FileProcessingRequest> ValidateFileRequest(FileProcessingRequest request)
    {
        if (request == null)
            return MlResult<FileProcessingRequest>.Fail("File processing request cannot be null");
            
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(request.FilePath))
            errors.Add("File path is required");
            
        if (string.IsNullOrWhiteSpace(request.OutputDirectory))
            errors.Add("Output directory is required");
            
        if (request.MaxFileSize <= 0)
            errors.Add($"Max file size must be positive, provided: {request.MaxFileSize}");
            
        if (request.AllowedExtensions?.Any() != true)
            errors.Add("At least one allowed extension must be specified");
            
        return errors.Any()
            ? MlResult<FileProcessingRequest>.Fail(errors.ToArray())
            : MlResult<FileProcessingRequest>.Valid(request);
    }
    
    private async Task<MlResult<ValidatedFileInfo>> ValidateFileExistsAsync(FileProcessingRequest request)
    {
        try
        {
            if (!File.Exists(request.FilePath))
                return MlResult<ValidatedFileInfo>.Fail($"File does not exist: {request.FilePath}");
                
            var fileInfo = new FileInfo(request.FilePath);
            
            // Validar tamaño
            if (fileInfo.Length > request.MaxFileSize)
            {
                return MlResult<ValidatedFileInfo>.Fail(
                    $"File size ({fileInfo.Length:N0} bytes) exceeds maximum allowed ({request.MaxFileSize:N0} bytes)");
            }
            
            // Validar extensión
            var extension = fileInfo.Extension.ToLowerInvariant();
            if (!request.AllowedExtensions.Contains(extension))
            {
                return MlResult<ValidatedFileInfo>.Fail(
                    $"File extension '{extension}' is not allowed. Allowed: {string.Join(", ", request.AllowedExtensions)}");
            }
            
            var validatedInfo = new ValidatedFileInfo
            {
                Request = request,
                FileInfo = fileInfo,
                ValidatedAt = DateTime.UtcNow
            };
            
            return MlResult<ValidatedFileInfo>.Valid(validatedInfo);
        }
        catch (Exception ex)
        {
            return MlResult<ValidatedFileInfo>.Fail($"File validation error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<FileContentData>> ReadFileContentAsync(ValidatedFileInfo validatedInfo)
    {
        try
        {
            var content = await File.ReadAllTextAsync(validatedInfo.FileInfo.FullName);
            
            if (string.IsNullOrWhiteSpace(content))
                return MlResult<FileContentData>.Fail("File is empty or contains only whitespace");
                
            var contentData = new FileContentData
            {
                ValidatedInfo = validatedInfo,
                Content = content,
                ContentLength = content.Length,
                ReadAt = DateTime.UtcNow
            };
            
            return MlResult<FileContentData>.Valid(contentData);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new FileAccessException($"Access denied to file: {validatedInfo.FileInfo.FullName}", ex);
        }
        catch (IOException ex)
        {
            throw new FileAccessException($"I/O error reading file: {validatedInfo.FileInfo.FullName}", ex);
        }
    }
    
    private async Task<MlResult<ParsedFileData>> ParseFileContentAsync(FileContentData contentData)
    {
        try
        {
            var parseResult = await _fileParser.ParseAsync(contentData.Content, contentData.ValidatedInfo.FileInfo.Extension);
            
            if (!parseResult.Success)
            {
                return MlResult<ParsedFileData>.Fail(
                    $"File parsing failed: {parseResult.ErrorMessage}. Line: {parseResult.ErrorLine}, Column: {parseResult.ErrorColumn}");
            }
            
            if (parseResult.Data == null || !parseResult.Data.Any())
            {
                return MlResult<ParsedFileData>.Fail("File parsed successfully but contains no data");
            }
            
            var parsedData = new ParsedFileData
            {
                ContentData = contentData,
                ParsedRecords = parseResult.Data,
                RecordCount = parseResult.Data.Count(),
                ParsedAt = DateTime.UtcNow,
                ParsingDuration = parseResult.Duration
            };
            
            return MlResult<ParsedFileData>.Valid(parsedData);
        }
        catch (FormatException ex)
        {
            throw new FileParsingException($"Format error in file: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new FileParsingException($"JSON parsing error: {ex.Message}", ex);
        }
    }
    
    private async Task<MlResult<ValidatedParsedData>> ValidateDataIntegrityAsync(ParsedFileData parsedData)
    {
        try
        {
            var validationResult = await _fileValidator.ValidateDataIntegrityAsync(parsedData.ParsedRecords);
            
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.ValidationErrors.Select(e => 
                    $"Record {e.RecordNumber}: {e.FieldName} - {e.ErrorMessage}").ToArray();
                    
                return MlResult<ValidatedParsedData>.Fail(errorMessages);
            }
            
            var validatedData = new ValidatedParsedData
            {
                ParsedData = parsedData,
                ValidRecords = validationResult.ValidRecords,
                ValidRecordCount = validationResult.ValidRecords.Count(),
                ValidatedAt = DateTime.UtcNow,
                ValidationDuration = validationResult.Duration
            };
            
            return MlResult<ValidatedParsedData>.Valid(validatedData);
        }
        catch (Exception ex)
        {
            return MlResult<ValidatedParsedData>.Fail($"Data validation error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<TransformedData>> TransformDataAsync(ValidatedParsedData validatedData)
    {
        try
        {
            var transformationResult = await _dataTransformer.TransformAsync(validatedData.ValidRecords);
            
            if (!transformationResult.Success)
            {
                return MlResult<TransformedData>.Fail(
                    $"Data transformation failed: {transformationResult.ErrorMessage}");
            }
            
            var transformedData = new TransformedData
            {
                ValidatedData = validatedData,
                TransformedRecords = transformationResult.TransformedRecords,
                TransformedRecordCount = transformationResult.TransformedRecords.Count(),
                TransformedAt = DateTime.UtcNow,
                TransformationDuration = transformationResult.Duration
            };
            
            return MlResult<TransformedData>.Valid(transformedData);
        }
        catch (TransformationException ex)
        {
            throw new DataTransformationException($"Transformation error: {ex.Message}", ex);
        }
    }
    
    private async Task<MlResult<SavedData>> SaveProcessedDataAsync(TransformedData transformedData)
    {
        try
        {
            var outputFileName = GenerateOutputFileName(transformedData.ValidatedData.ParsedData.ContentData.ValidatedInfo);
            var outputPath = Path.Combine(
                transformedData.ValidatedData.ParsedData.ContentData.ValidatedInfo.Request.OutputDirectory,
                outputFileName);
                
            // Crear directorio si no existe
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
                
            await File.WriteAllTextAsync(outputPath, 
                JsonSerializer.Serialize(transformedData.TransformedRecords, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }));
                
            var savedData = new SavedData
            {
                TransformedData = transformedData,
                OutputPath = outputPath,
                SavedAt = DateTime.UtcNow
            };
            
            return MlResult<SavedData>.Valid(savedData);
        }
        catch (IOException ex)
        {
            return MlResult<SavedData>.Fail($"Failed to save processed data: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            return MlResult<SavedData>.Fail($"Access denied when saving data: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ProcessedFileResult>> CreateProcessingResultAsync(
        SavedData savedData, 
        DateTime startTime, 
        string correlationId)
    {
        var endTime = DateTime.UtcNow;
        var totalDuration = endTime - startTime;
        
        var result = new ProcessedFileResult
        {
            CorrelationId = correlationId,
            OriginalFilePath = savedData.TransformedData.ValidatedData.ParsedData.ContentData.ValidatedInfo.FileInfo.FullName,
            OutputFilePath = savedData.OutputPath,
            TotalRecordsProcessed = savedData.TransformedData.TransformedRecordCount,
            StartTime = startTime,
            EndTime = endTime,
            TotalDuration = totalDuration,
            ProcessingSteps = new ProcessingStepsInfo
            {
                ParsingDuration = savedData.TransformedData.ValidatedData.ParsedData.ParsingDuration,
                ValidationDuration = savedData.TransformedData.ValidatedData.ValidationDuration,
                TransformationDuration = savedData.TransformedData.TransformationDuration
            }
        };
        
        _logger.LogInformation($"File processing completed successfully [CorrelationId: {correlationId}] in {totalDuration.TotalSeconds:F2}s");
        
        return MlResult<ProcessedFileResult>.Valid(result);
    }
    
    private string GenerateOutputFileName(ValidatedFileInfo validatedInfo)
    {
        var originalName = Path.GetFileNameWithoutExtension(validatedInfo.FileInfo.Name);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        return $"{originalName}_processed_{timestamp}.json";
    }
}

// Clases de apoyo y modelos de datos
public class FileProcessingRequest
{
    public string FilePath { get; set; }
    public string OutputDirectory { get; set; }
    public long MaxFileSize { get; set; }
    public string[] AllowedExtensions { get; set; }
}

public class ValidatedFileInfo
{
    public FileProcessingRequest Request { get; set; }
    public FileInfo FileInfo { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class FileContentData
{
    public ValidatedFileInfo ValidatedInfo { get; set; }
    public string Content { get; set; }
    public int ContentLength { get; set; }
    public DateTime ReadAt { get; set; }
}

public class ParsedFileData
{
    public FileContentData ContentData { get; set; }
    public IEnumerable<object> ParsedRecords { get; set; }
    public int RecordCount { get; set; }
    public DateTime ParsedAt { get; set; }
    public TimeSpan ParsingDuration { get; set; }
}

public class ValidatedParsedData
{
    public ParsedFileData ParsedData { get; set; }
    public IEnumerable<object> ValidRecords { get; set; }
    public int ValidRecordCount { get; set; }
    public DateTime ValidatedAt { get; set; }
    public TimeSpan ValidationDuration { get; set; }
}

public class TransformedData
{
    public ValidatedParsedData ValidatedData { get; set; }
    public IEnumerable<object> TransformedRecords { get; set; }
    public int TransformedRecordCount { get; set; }
    public DateTime TransformedAt { get; set; }
    public TimeSpan TransformationDuration { get; set; }
}

public class SavedData
{
    public TransformedData TransformedData { get; set; }
    public string OutputPath { get; set; }
    public DateTime SavedAt { get; set; }
}

public class ProcessedFileResult
{
    public string CorrelationId { get; set; }
    public string OriginalFilePath { get; set; }
    public string OutputFilePath { get; set; }
    public int TotalRecordsProcessed { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public ProcessingStepsInfo ProcessingSteps { get; set; }
}

public class ProcessingStepsInfo
{
    public TimeSpan ParsingDuration { get; set; }
    public TimeSpan ValidationDuration { get; set; }
    public TimeSpan TransformationDuration { get; set; }
}

// Excepciones personalizadas
public class FileAccessException : Exception
{
    public FileAccessException(string message) : base(message) { }
    public FileAccessException(string message, Exception innerException) : base(message, innerException) { }
}

public class FileParsingException : Exception
{
    public FileParsingException(string message) : base(message) { }
    public FileParsingException(string message, Exception innerException) : base(message, innerException) { }
}

public class DataTransformationException : Exception
{
    public DataTransformationException(string message) : base(message) { }
    public DataTransformationException(string message, Exception innerException) : base(message, innerException) { }
}
```

### Ejemplo 3: Sistema de Análisis de Errores con Valores Preservados

```csharp
public class ErrorAnalysisService
{
    private readonly ILogger _logger;
    private readonly IErrorRepository _errorRepository;
    
    public ErrorAnalysisService(ILogger logger, IErrorRepository errorRepository)
    {
        _logger = logger;
        _errorRepository = errorRepository;
    }
    
    public async Task<MlResult<ErrorAnalysisReport>> AnalyzeSystemErrorsAsync(ErrorAnalysisRequest request)
    {
        return await ValidateAnalysisRequest(request)
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validRequest => await RetrieveErrorsAsync(validRequest))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async errorData => await CategorizeErrorsAsync(errorData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async categorizedData => await AnalyzeErrorPatternsAsync(categorizedData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async patternData => await GenerateRecommendationsAsync(patternData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async recommendationData => await CreateAnalysisReportAsync(recommendationData));
    }
    
    public MlResult<ErrorDetailsWithContext> AnalyzeSpecificErrorWithContext(ErrorOccurrence error)
    {
        return ValidateErrorOccurrence(error)
            .BindSaveValueInDetailsIfFaildFuncResult(validError => ExtractErrorContext(validError))
            .BindSaveValueInDetailsIfFaildFuncResult(contextData => AnalyzeErrorCause(contextData))
            .BindSaveValueInDetailsIfFaildFuncResult(causeData => DetermineErrorSeverity(causeData))
            .BindSaveValueInDetailsIfFaildFuncResult(severityData => CreateErrorDetailsWithContext(severityData));
    }
    
    private MlResult<ErrorAnalysisRequest> ValidateAnalysisRequest(ErrorAnalysisRequest request)
    {
        if (request == null)
            return MlResult<ErrorAnalysisRequest>.Fail("Analysis request cannot be null");
            
        var errors = new List<string>();
        
        if (request.StartDate >= request.EndDate)
            errors.Add($"Start date ({request.StartDate:yyyy-MM-dd}) must be before end date ({request.EndDate:yyyy-MM-dd})");
            
        if ((request.EndDate - request.StartDate).TotalDays > 365)
            errors.Add($"Analysis period cannot exceed 365 days. Requested: {(request.EndDate - request.StartDate).TotalDays:F0} days");
            
        if (request.MaxErrorCount <= 0)
            errors.Add($"Max error count must be positive. Provided: {request.MaxErrorCount}");
            
        if (request.ErrorSources?.Any() != true)
            errors.Add("At least one error source must be specified");
            
        return errors.Any()
            ? MlResult<ErrorAnalysisRequest>.Fail(errors.ToArray())
            : MlResult<ErrorAnalysisRequest>.Valid(request);
    }
    
    private async Task<MlResult<RetrievedErrorData>> RetrieveErrorsAsync(ErrorAnalysisRequest request)
    {
        try
        {
            var errors = await _errorRepository.GetErrorsAsync(request.StartDate, request.EndDate, request.ErrorSources);
            
            if (!errors.Any())
            {
                return MlResult<RetrievedErrorData>.Fail(
                    $"No errors found for the specified period ({request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}) " +
                    $"and sources: {string.Join(", ", request.ErrorSources)}");
            }
            
            if (errors.Count() > request.MaxErrorCount)
            {
                var limitedErrors = errors.Take(request.MaxErrorCount).ToList();
                _logger.LogWarning($"Error count ({errors.Count():N0}) exceeds maximum ({request.MaxErrorCount:N0}). Limited to first {request.MaxErrorCount:N0} errors.");
                
                var retrievedData = new RetrievedErrorData
                {
                    Request = request,
                    Errors = limitedErrors,
                    TotalErrorsFound = errors.Count(),
                    ErrorsIncluded = limitedErrors.Count,
                    WasTruncated = true,
                    RetrievedAt = DateTime.UtcNow
                };
                
                return MlResult<RetrievedErrorData>.Valid(retrievedData);
            }
            else
            {
                var retrievedData = new RetrievedErrorData
                {
                    Request = request,
                    Errors = errors.ToList(),
                    TotalErrorsFound = errors.Count(),
                    ErrorsIncluded = errors.Count(),
                    WasTruncated = false,
                    RetrievedAt = DateTime.UtcNow
                };
                
                return MlResult<RetrievedErrorData>.Valid(retrievedData);
            }
        }
        catch (Exception ex)
        {
            return MlResult<RetrievedErrorData>.Fail($"Failed to retrieve errors: {ex.Message}");
        }
    }
    
    private async Task<MlResult<CategorizedErrorData>> CategorizeErrorsAsync(RetrievedErrorData errorData)
    {
        try
        {
            var categories = new Dictionary<string, List<ErrorOccurrence>>();
            var uncategorized = new List<ErrorOccurrence>();
            
            foreach (var error in errorData.Errors)
            {
        …