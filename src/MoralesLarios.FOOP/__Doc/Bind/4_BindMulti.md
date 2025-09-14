# MlResultActionsBind - Operaciones de Binding Múltiple

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Funcionalidad](#análisis-de-la-funcionalidad)
3. [Variantes de BindMulti](#variantes-de-bindmulti)
4. [Patrones de Uso](#patrones-de-uso)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La sección **BindMulti** de `MlResultActionsBind` proporciona operaciones avanzadas para ejecutar múltiples validaciones o transformaciones en paralelo antes de proceder con una función final. Estas operaciones implementan el patrón de "validaciones múltiples" donde todas las funciones deben ser exitosas para que la operación final se ejecute.

### Propósito Principal

- **Validaciones Múltiples**: Ejecutar varias validaciones que deben pasar todas
- **Acumulación de Errores**: Si alguna validación falla, se acumulan todos los errores
- **Procesamiento Condicional**: Solo ejecutar la función final si todas las validaciones pasan
- **Soporte para Agregación**: Pasar los resultados exitosos a la función final

---

## Análisis de la Funcionalidad

### Filosofía de BindMulti

```
Valor → [Validación1, Validación2, Validación3, ...] → ¿Todas exitosas?
                                                         ↓
                                                    Si → Función Final
                                                    No → Errores Acumulados
```

### Comportamiento Base

1. **Si el resultado fuente es fallido**: Propaga el error inmediatamente
2. **Si el resultado fuente es exitoso**: Ejecuta todas las funciones de validación
3. **Si alguna validación falla**: Acumula todos los errores y retorna fallo
4. **Si todas las validaciones pasan**: Ejecuta la función final con el valor original o los resultados

---

## Variantes de BindMulti

### Variante 1: BindMulti Simple

**Propósito**: Ejecuta validaciones que deben pasar todas antes de ejecutar la función final

```csharp
public static MlResult<TReturn> BindMulti<T, TReturn>(this MlResult<T> source,
                                                      Func<T, MlResult<TReturn>> returnFunc,
                                                      params Func<T, MlResult<TReturn>>[] funcs)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `returnFunc`: Función final a ejecutar si todas las validaciones pasan
- `funcs`: Array de funciones de validación que deben ejecutarse

**Comportamiento**:
- Ejecuta todas las funciones en `funcs` con el valor de `source`
- Si alguna falla, acumula los errores y retorna fallo
- Si todas pasan, ejecuta `returnFunc` con el valor original

### Variante 2: BindMulti con Agregación de Resultados

**Propósito**: Similar a la anterior, pero la función final recibe los resultados de las validaciones

```csharp
public static MlResult<TReturn> BindMulti<T, TReturn>(this MlResult<T> source,
                                                      Func<T, IEnumerable<TReturn>, MlResult<TReturn>> returnFunc,
                                                      params Func<T, MlResult<TReturn>>[] funcs)
```

**Comportamiento**:
- Ejecuta todas las funciones de validación
- Si todas pasan, ejecuta `returnFunc` con el valor original Y los resultados de las validaciones

### Variante 3: BindMulti con Tipos Diferentes

**Propósito**: Permite que las funciones de validación retornen un tipo diferente al tipo final

```csharp
public static MlResult<TReturn> BindMulti<T, TReturn, TFuncColec>(this MlResult<T> source,
                                                                  Func<T, IEnumerable<TFuncColec>, MlResult<TReturn>> returnFunc,
                                                                  params Func<T, MlResult<TFuncColec>>[] funcs)
```

**Parámetros**:
- `TFuncColec`: Tipo que retornan las funciones de validación
- `TReturn`: Tipo final que retorna la operación

**Comportamiento**:
- Las funciones de validación retornan `MlResult<TFuncColec>`
- Si todas pasan, `returnFunc` recibe los valores de tipo `TFuncColec`
- Retorna `MlResult<TReturn>`

### Soporte Asíncrono Completo

Cada variante tiene múltiples versiones asíncronas:
- **Función final asíncrona**: `Func<T, Task<MlResult<TReturn>>>`
- **Funciones de validación asíncronas**: `Func<T, Task<MlResult<T>>>`
- **Fuente asíncrona**: `Task<MlResult<T>>`
- **Todas las combinaciones posibles**

---

## Patrones de Uso

### Patrón 1: Validaciones Múltiples Independientes

```csharp
// Múltiples validaciones que deben pasar todas
var result = userData
    .BindMulti(
        returnFunc: validUser => ProcessUser(validUser),
        ValidateAge,
        ValidateEmail,
        ValidatePhoneNumber,
        ValidateAddress
    );
```

### Patrón 2: Validaciones con Agregación de Datos

```csharp
// Recopilar información de múltiples fuentes
var result = userId
    .BindMulti(
        returnFunc: (id, profiles) => CreateCompleteProfile(id, profiles),
        GetUserProfile,
        GetUserPreferences,
        GetUserActivity,
        GetUserSettings
    );
```

### Patrón 3: Validaciones Heterogéneas

```csharp
// Diferentes tipos de validación que contribuyen al resultado final
var result = orderData
    .BindMulti<OrderData, OrderResult, ValidationInfo>(
        returnFunc: (order, validations) => CreateOrderResult(order, validations),
        ValidateCustomerCredit,      // Retorna MlResult<ValidationInfo>
        ValidateInventoryStatus,     // Retorna MlResult<ValidationInfo>
        ValidatePaymentMethod,       // Retorna MlResult<ValidationInfo>
        ValidateShippingAddress      // Retorna MlResult<ValidationInfo>
    );
```

---

## Ejemplos Prácticos

### Ejemplo 1: Validación de Usuario Completa

```csharp
public class UserValidationService
{
    public MlResult<ValidatedUser> ValidateUserCompletely(UserRegistrationData userData)
    {
        return userData.ToMlResult()
            .BindMulti(
                returnFunc: validatedData => CreateValidatedUser(validatedData),
                ValidateBasicInformation,
                ValidateEmailFormat,
                ValidatePasswordStrength,
                ValidatePhoneNumber,
                ValidateAge,
                ValidateTOS
            );
    }
    
    public async Task<MlResult<ValidatedUser>> ValidateUserCompletelyAsync(UserRegistrationData userData)
    {
        return await userData.ToMlResult()
            .BindMultiAsync(
                returnFuncAsync: async validatedData => await CreateValidatedUserAsync(validatedData),
                ValidateBasicInformation,
                ValidateEmailFormat,
                ValidatePasswordStrength,
                ValidatePhoneNumber,
                ValidateAge,
                ValidateTOS
            );
    }
    
    private MlResult<UserRegistrationData> ValidateBasicInformation(UserRegistrationData data)
    {
        if (string.IsNullOrWhiteSpace(data.FirstName))
            return MlResult<UserRegistrationData>.Fail("First name is required");
            
        if (string.IsNullOrWhiteSpace(data.LastName))
            return MlResult<UserRegistrationData>.Fail("Last name is required");
            
        if (data.FirstName.Length < 2)
            return MlResult<UserRegistrationData>.Fail("First name must be at least 2 characters");
            
        return MlResult<UserRegistrationData>.Valid(data);
    }
    
    private MlResult<UserRegistrationData> ValidateEmailFormat(UserRegistrationData data)
    {
        if (string.IsNullOrWhiteSpace(data.Email))
            return MlResult<UserRegistrationData>.Fail("Email is required");
            
        if (!IsValidEmailFormat(data.Email))
            return MlResult<UserRegistrationData>.Fail("Invalid email format");
            
        if (data.Email.Length > 254)
            return MlResult<UserRegistrationData>.Fail("Email is too long");
            
        return MlResult<UserRegistrationData>.Valid(data);
    }
    
    private MlResult<UserRegistrationData> ValidatePasswordStrength(UserRegistrationData data)
    {
        if (string.IsNullOrWhiteSpace(data.Password))
            return MlResult<UserRegistrationData>.Fail("Password is required");
            
        if (data.Password.Length < 8)
            return MlResult<UserRegistrationData>.Fail("Password must be at least 8 characters");
            
        if (!data.Password.Any(char.IsUpper))
            return MlResult<UserRegistrationData>.Fail("Password must contain at least one uppercase letter");
            
        if (!data.Password.Any(char.IsDigit))
            return MlResult<UserRegistrationData>.Fail("Password must contain at least one number");
            
        return MlResult<UserRegistrationData>.Valid(data);
    }
    
    private MlResult<UserRegistrationData> ValidatePhoneNumber(UserRegistrationData data)
    {
        if (string.IsNullOrWhiteSpace(data.PhoneNumber))
            return MlResult<UserRegistrationData>.Fail("Phone number is required");
            
        if (data.PhoneNumber.Length < 10)
            return MlResult<UserRegistrationData>.Fail("Phone number must be at least 10 digits");
            
        if (!data.PhoneNumber.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ' || c == '(' || c == ')'))
            return MlResult<UserRegistrationData>.Fail("Phone number contains invalid characters");
            
        return MlResult<UserRegistrationData>.Valid(data);
    }
    
    private MlResult<UserRegistrationData> ValidateAge(UserRegistrationData data)
    {
        if (data.DateOfBirth == default)
            return MlResult<UserRegistrationData>.Fail("Date of birth is required");
            
        var age = DateTime.Now.Year - data.DateOfBirth.Year;
        if (data.DateOfBirth.Date > DateTime.Now.AddYears(-age))
            age--;
            
        if (age < 13)
            return MlResult<UserRegistrationData>.Fail("User must be at least 13 years old");
            
        if (age > 120)
            return MlResult<UserRegistrationData>.Fail("Invalid age");
            
        return MlResult<UserRegistrationData>.Valid(data);
    }
    
    private MlResult<UserRegistrationData> ValidateTOS(UserRegistrationData data)
    {
        if (!data.AcceptedTermsOfService)
            return MlResult<UserRegistrationData>.Fail("Terms of service must be accepted");
            
        if (!data.AcceptedPrivacyPolicy)
            return MlResult<UserRegistrationData>.Fail("Privacy policy must be accepted");
            
        return MlResult<UserRegistrationData>.Valid(data);
    }
    
    private MlResult<ValidatedUser> CreateValidatedUser(UserRegistrationData data)
    {
        var validatedUser = new ValidatedUser
        {
            Id = Guid.NewGuid(),
            FirstName = data.FirstName.Trim(),
            LastName = data.LastName.Trim(),
            Email = data.Email.ToLower().Trim(),
            PhoneNumber = NormalizePhoneNumber(data.PhoneNumber),
            DateOfBirth = data.DateOfBirth,
            CreatedAt = DateTime.UtcNow,
            IsValidated = true
        };
        
        return MlResult<ValidatedUser>.Valid(validatedUser);
    }
    
    private async Task<MlResult<ValidatedUser>> CreateValidatedUserAsync(UserRegistrationData data)
    {
        await Task.Delay(10); // Simular operación asíncrona
        return CreateValidatedUser(data);
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
    
    private string NormalizePhoneNumber(string phoneNumber)
    {
        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }
}

// Clases de apoyo
public class UserRegistrationData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool AcceptedTermsOfService { get; set; }
    public bool AcceptedPrivacyPolicy { get; set; }
}

public class ValidatedUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsValidated { get; set; }
}
```

### Ejemplo 2: Agregación de Datos de Múltiples Fuentes

```csharp
public class UserProfileAggregationService
{
    public async Task<MlResult<CompleteUserProfile>> AggregateUserProfileAsync(int userId)
    {
        return await userId.ToMlResult()
            .BindMultiAsync(
                returnFuncAsync: async (id, profiles) => await CreateCompleteProfileAsync(id, profiles),
                GetBasicProfileAsync,
                GetPreferencesProfileAsync,
                GetActivityProfileAsync,
                GetSocialProfileAsync
            );
    }
    
    public async Task<MlResult<UserAnalytics>> AnalyzeUserDataAsync(int userId)
    {
        return await userId.ToMlResult()
            .BindMultiAsync<int, UserAnalytics, AnalyticsData>(
                returnFuncAsync: async (id, analyticsData) => await GenerateAnalyticsAsync(id, analyticsData),
                GetUserBehaviorDataAsync,
                GetUserEngagementDataAsync,
                GetUserPerformanceDataAsync,
                GetUserPreferenceDataAsync
            );
    }
    
    private async Task<MlResult<UserProfile>> GetBasicProfileAsync(int userId)
    {
        await Task.Delay(50); // Simular llamada a base de datos
        
        if (userId <= 0)
            return MlResult<UserProfile>.Fail("Invalid user ID for basic profile");
            
        var profile = new UserProfile
        {
            UserId = userId,
            Name = $"User {userId}",
            Email = $"user{userId}@example.com",
            CreatedAt = DateTime.Now.AddDays(-userId)
        };
        
        return MlResult<UserProfile>.Valid(profile);
    }
    
    private async Task<MlResult<UserProfile>> GetPreferencesProfileAsync(int userId)
    {
        await Task.Delay(30);
        
        if (userId % 10 == 0) // Simular algunos fallos
            return MlResult<UserProfile>.Fail($"Preferences not found for user {userId}");
            
        var profile = new UserProfile
        {
            UserId = userId,
            Theme = "Dark",
            Language = "English",
            Notifications = true
        };
        
        return MlResult<UserProfile>.Valid(profile);
    }
    
    private async Task<MlResult<UserProfile>> GetActivityProfileAsync(int userId)
    {
        await Task.Delay(40);
        
        var profile = new UserProfile
        {
            UserId = userId,
            LastLoginDate = DateTime.Now.AddHours(-userId),
            TotalLogins = userId * 10,
            AverageSessionTime = TimeSpan.FromMinutes(userId % 60 + 15)
        };
        
        return MlResult<UserProfile>.Valid(profile);
    }
    
    private async Task<MlResult<UserProfile>> GetSocialProfileAsync(int userId)
    {
        await Task.Delay(35);
        
        if (userId % 15 == 0) // Simular fallos ocasionales
            return MlResult<UserProfile>.Fail($"Social profile unavailable for user {userId}");
            
        var profile = new UserProfile
        {
            UserId = userId,
            FriendsCount = userId % 100,
            FollowersCount = userId % 1000,
            PostsCount = userId % 50
        };
        
        return MlResult<UserProfile>.Valid(profile);
    }
    
    private async Task<MlResult<CompleteUserProfile>> CreateCompleteProfileAsync(int userId, IEnumerable<UserProfile> profiles)
    {
        await Task.Delay(20);
        
        var profileList = profiles.ToList();
        var completeProfile = new CompleteUserProfile
        {
            UserId = userId,
            AggregatedAt = DateTime.UtcNow,
            ProfileCount = profileList.Count,
            CombinedData = profileList.SelectMany(p => p.GetAllProperties()).ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value
            )
        };
        
        return MlResult<CompleteUserProfile>.Valid(completeProfile);
    }
    
    // Métodos para análisis con tipos diferentes
    private async Task<MlResult<AnalyticsData>> GetUserBehaviorDataAsync(int userId)
    {
        await Task.Delay(60);
        
        var data = new AnalyticsData
        {
            DataType = "Behavior",
            UserId = userId,
            Metrics = new Dictionary<string, object>
            {
                ["PageViews"] = userId * 50,
                ["SessionDuration"] = TimeSpan.FromMinutes(userId % 30 + 10),
                ["BounceRate"] = (userId % 100) / 100.0
            }
        };
        
        return MlResult<AnalyticsData>.Valid(data);
    }
    
    private async Task<MlResult<AnalyticsData>> GetUserEngagementDataAsync(int userId)
    {
        await Task.Delay(45);
        
        var data = new AnalyticsData
        {
            DataType = "Engagement",
            UserId = userId,
            Metrics = new Dictionary<string, object>
            {
                ["LikesGiven"] = userId * 5,
                ["CommentsPosted"] = userId * 2,
                ["SharesCount"] = userId % 20
            }
        };
        
        return MlResult<AnalyticsData>.Valid(data);
    }
    
    private async Task<MlResult<AnalyticsData>> GetUserPerformanceDataAsync(int userId)
    {
        await Task.Delay(55);
        
        if (userId % 20 == 0)
            return MlResult<AnalyticsData>.Fail($"Performance data not available for user {userId}");
            
        var data = new AnalyticsData
        {
            DataType = "Performance",
            UserId = userId,
            Metrics = new Dictionary<string, object>
            {
                ["TasksCompleted"] = userId * 3,
                ["AverageResponseTime"] = TimeSpan.FromSeconds(userId % 10 + 1),
                ["SuccessRate"] = Math.Min(0.95, (userId % 100) / 100.0 + 0.5)
            }
        };
        
        return MlResult<AnalyticsData>.Valid(data);
    }
    
    private async Task<MlResult<AnalyticsData>> GetUserPreferenceDataAsync(int userId)
    {
        await Task.Delay(25);
        
        var data = new AnalyticsData
        {
            DataType = "Preferences",
            UserId = userId,
            Metrics = new Dictionary<string, object>
            {
                ["PreferredCategories"] = new[] { "Tech", "Sports", "News" },
                ["NotificationFrequency"] = userId % 5 + 1,
                ["ContentLanguage"] = "English"
            }
        };
        
        return MlResult<AnalyticsData>.Valid(data);
    }
    
    private async Task<MlResult<UserAnalytics>> GenerateAnalyticsAsync(int userId, IEnumerable<AnalyticsData> analyticsData)
    {
        await Task.Delay(30);
        
        var dataList = analyticsData.ToList();
        var analytics = new UserAnalytics
        {
            UserId = userId,
            GeneratedAt = DateTime.UtcNow,
            DataSources = dataList.Select(d => d.DataType).ToList(),
            AggregatedMetrics = dataList.SelectMany(d => d.Metrics).ToDictionary(
                kvp => $"{dataList.First(d => d.Metrics.ContainsKey(kvp.Key)).DataType}_{kvp.Key}",
                kvp => kvp.Value
            ),
            AnalyticsScore = CalculateAnalyticsScore(dataList)
        };
        
        return MlResult<UserAnalytics>.Valid(analytics);
    }
    
    private double CalculateAnalyticsScore(List<AnalyticsData> dataList)
    {
        // Lógica simple de scoring basada en la cantidad y calidad de datos
        var baseScore = dataList.Count * 20; // 20 puntos por fuente de datos
        var metricsCount = dataList.Sum(d => d.Metrics.Count);
        var metricsScore = metricsCount * 5; // 5 puntos por métrica
        
        return Math.Min(100, baseScore + metricsScore);
    }
}

// Clases de apoyo adicionales
public class UserProfile
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Theme { get; set; }
    public string Language { get; set; }
    public bool Notifications { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public int TotalLogins { get; set; }
    public TimeSpan AverageSessionTime { get; set; }
    public int FriendsCount { get; set; }
    public int FollowersCount { get; set; }
    public int PostsCount { get; set; }
    
    public Dictionary<string, object> GetAllProperties()
    {
        return GetType().GetProperties()
            .Where(p => p.GetValue(this) != null)
            .ToDictionary(p => p.Name, p => p.GetValue(this));
    }
}

public class CompleteUserProfile
{
    public int UserId { get; set; }
    public DateTime AggregatedAt { get; set; }
    public int ProfileCount { get; set; }
    public Dictionary<string, object> CombinedData { get; set; }
}

public class AnalyticsData
{
    public string DataType { get; set; }
    public int UserId { get; set; }
    public Dictionary<string, object> Metrics { get; set; }
}

public class UserAnalytics
{
    public int UserId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<string> DataSources { get; set; }
    public Dictionary<string, object> AggregatedMetrics { get; set; }
    public double AnalyticsScore { get; set; }
}
```

### Ejemplo 3: Validación de Pedido con Múltiples Criterios

```csharp
public class OrderValidationService
{
    public async Task<MlResult<ValidatedOrder>> ValidateOrderCompletelyAsync(OrderRequest orderRequest)
    {
        return await orderRequest.ToMlResult()
            .BindMultiAsync<OrderRequest, ValidatedOrder, ValidationResult>(
                returnFuncAsync: async (order, validationResults) => 
                    await CreateValidatedOrderAsync(order, validationResults),
                ValidateCustomerStatusAsync,
                ValidateInventoryAvailabilityAsync,
                ValidatePaymentMethodAsync,
                ValidateShippingAddressAsync,
                ValidatePricingAsync,
                ValidateBusinessRulesAsync
            );
    }
    
    private async Task<MlResult<ValidationResult>> ValidateCustomerStatusAsync(OrderRequest orderRequest)
    {
        await Task.Delay(50);
        
        if (orderRequest.CustomerId <= 0)
            return MlResult<ValidationResult>.Fail("Invalid customer ID");
            
        // Simular verificación de estado del cliente
        var customerStatus = await GetCustomerStatusAsync(orderRequest.CustomerId);
        
        if (!customerStatus.IsActive)
            return MlResult<ValidationResult>.Fail("Customer account is inactive");
            
        if (customerStatus.IsSuspended)
            return MlResult<ValidationResult>.Fail("Customer account is suspended");
            
        var result = new ValidationResult
        {
            ValidationType = "CustomerStatus",
            IsValid = true,
            Details = new Dictionary<string, object>
            {
                ["CustomerId"] = orderRequest.CustomerId,
                ["CustomerTier"] = customerStatus.Tier,
                ["AccountAge"] = customerStatus.AccountAge
            }
        };
        
        return MlResult<ValidationResult>.Valid(result);
    }
    
    private async Task<MlResult<ValidationResult>> ValidateInventoryAvailabilityAsync(OrderRequest orderRequest)
    {
        await Task.Delay(75);
        
        if (orderRequest.Items?.Any() != true)
            return MlResult<ValidationResult>.Fail("Order must contain at least one item");
            
        var unavailableItems = new List<string>();
        var reservationDetails = new Dictionary<string, object>();
        
        foreach (var item in orderRequest.Items)
        {
            var availability = await CheckInventoryAsync(item.ProductId, item.Quantity);
            
            if (!availability.Available)
            {
                unavailableItems.Add($"{item.ProductId} (requested: {item.Quantity}, available: {availability.AvailableQuantity})");
            }
            else
            {
                reservationDetails[item.ProductId] = new
                {
                    RequestedQuantity = item.Quantity,
                    AvailableQuantity = availability.AvailableQuantity,
                    ReservationId = availability.ReservationId
                };
            }
        }
        
        if (unavailableItems.Any())
        {
            return MlResult<ValidationResult>.Fail(
                $"Items not available: {string.Join(", ", unavailableItems)}");
        }
        
        var result = new ValidationResult
        {
            ValidationType = "InventoryAvailability",
            IsValid = true,
            Details = reservationDetails
        };
        
        return MlResult<ValidationResult>.Valid(result);
    }
    
    private async Task<MlResult<ValidationResult>> ValidatePaymentMethodAsync(OrderRequest orderRequest)
    {
        await Task.Delay(40);
        
        if (string.IsNullOrWhiteSpace(orderRequest.PaymentMethodId))
            return MlResult<ValidationResult>.Fail("Payment method is required");
            
        var paymentMethod = await GetPaymentMethodAsync(orderRequest.PaymentMethodId);
        
        if (paymentMethod == null)
            return MlResult<ValidationResult>.Fail("Payment method not found");
            
        if (paymentMethod.IsExpired)
            return MlResult<ValidationResult>.Fail("Payment method has expired");
            
        if (!paymentMethod.IsActive)
            return MlResult<ValidationResult>.Fail("Payment method is inactive");
            
        var totalAmount = orderRequest.Items.Sum(i => i.Quantity * i.UnitPrice);
        
        if (totalAmount > paymentMethod.CreditLimit)
            return MlResult<ValidationResult>.Fail(
                $"Order total ({totalAmount:C}) exceeds credit limit ({paymentMethod.CreditLimit:C})");
        
        var result = new ValidationResult
        {
            ValidationType = "PaymentMethod",
            IsValid = true,
            Details = new Dictionary<string, object>
            {
                ["PaymentMethodId"] = orderRequest.PaymentMethodId,
                ["PaymentType"] = paymentMethod.Type,
                ["CreditLimit"] = paymentMethod.CreditLimit,
                ["OrderTotal"] = totalAmount
            }
        };
        
        return MlResult<ValidationResult>.Valid(result);
    }
    
    private async Task<MlResult<ValidationResult>> ValidateShippingAddressAsync(OrderRequest orderRequest)
    {
        await Task.Delay(30);
        
        if (orderRequest.ShippingAddress == null)
            return MlResult<ValidationResult>.Fail("Shipping address is required");
            
        var address = orderRequest.ShippingAddress;
        var validationErrors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(address.Street))
            validationErrors.Add("Street address is required");
            
        if (string.IsNullOrWhiteSpace(address.City))
            validationErrors.Add("City is required");
            
        if (string.IsNullOrWhiteSpace(address.PostalCode))
            validationErrors.Add("Postal code is required");
            
        if (string.IsNullOrWhiteSpace(address.Country))
            validationErrors.Add("Country is required");
            
        if (validationErrors.Any())
            return MlResult<ValidationResult>.Fail(string.Join("; ", validationErrors));
            
        // Verificar si se puede enviar a esa dirección
        var shippingAvailability = await CheckShippingAvailabilityAsync(address);
        
        if (!shippingAvailability.Available)
            return MlResult<ValidationResult>.Fail(
                $"Shipping not available to {address.City}, {address.Country}");
        
        var result = new ValidationResult
        {
            ValidationType = "ShippingAddress",
            IsValid = true,
            Details = new Dictionary<string, object>
            {
                ["ShippingZone"] = shippingAvailability.Zone,
                ["EstimatedDeliveryDays"] = shippingAvailability.EstimatedDays,
                ["ShippingCost"] = shippingAvailability.Cost
            }
        };
        
        return MlResult<ValidationResult>.Valid(result);
    }
    
    private async Task<MlResult<ValidationResult>> ValidatePricingAsync(OrderRequest orderRequest)
    {
        await Task.Delay(35);
        
        var pricingErrors = new List<string>();
        var calculatedTotal = 0m;
        var itemDetails = new Dictionary<string, object>();
        
        foreach (var item in orderRequest.Items)
        {
            if (item.Quantity <= 0)
                pricingErrors.Add($"Invalid quantity for {item.ProductId}: {item.Quantity}");
                
            if (item.UnitPrice <= 0)
                pricingErrors.Add($"Invalid unit price for {item.ProductId}: {item.UnitPrice}");
                
            // Verificar precio actual del producto
            var currentPrice = await GetCurrentProductPriceAsync(item.ProductId);
            
            if (Math.Abs(item.UnitPrice - currentPrice) > 0.01m)
                pricingErrors.Add(
                    $"Price mismatch for {item.ProductId}: expected {currentPrice:C}, got {item.UnitPrice:C}");
            
            var itemTotal = item.Quantity * item.UnitPrice;
            calculatedTotal += itemTotal;
            
            itemDetails[item.ProductId] = new
            {
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                CurrentPrice = currentPrice,
                ItemTotal = itemTotal
            };
        }
        
        if (pricingErrors.Any())
            return MlResult<ValidationResult>.Fail(string.Join("; ", pricingErrors));
        
        var result = new ValidationResult
        {
            ValidationType = "Pricing",
            IsValid = true,
            Details = new Dictionary<string, object>
            {
                ["CalculatedTotal"] = calculatedTotal,
                ["ItemDetails"] = itemDetails,
                ["ValidatedAt"] = DateTime.UtcNow
            }
        };
        
        return MlResult<ValidationResult>.Valid(result);
    }
    
    private async Task<MlResult<ValidationResult>> ValidateBusinessRulesAsync(OrderRequest orderRequest)
    {
        await Task.Delay(45);
        
        var businessRuleViolations = new List<string>();
        
        // Regla: Pedido mínimo
        var orderTotal = orderRequest.Items.Sum(i => i.Quantity * i.UnitPrice);
        const decimal minimumOrderAmount = 10.00m;
        
        if (orderTotal < minimumOrderAmount)
            businessRuleViolations.Add($"Order total ({orderTotal:C}) is below minimum ({minimumOrderAmount:C})");
        
        // Regla: Límite de cantidad por ítem
        const int maxQuantityPerItem = 100;
        var itemsExceedingLimit = orderRequest.Items
            .Where(i => i.Quantity > maxQuantityPerItem)
            .ToList();
            
        if (itemsExceedingLimit.Any())
        {
            var violations = itemsExceedingLimit
                .Select(i => $"{i.ProductId} (quantity: {i.Quantity})")
                .ToList();
            businessRuleViolations.Add(
                $"Items exceed maximum quantity per item ({maxQuantityPerItem}): {string.Join(", ", violations)}");
        }
        
        // Regla: Verificar productos restringidos para el cliente
        var restrictedProducts = await GetRestrictedProductsForCustomerAsync(orderRequest.CustomerId);
        var restrictedItemsInOrder = orderRequest.Items
            .Where(i => restrictedProducts.Contains(i.ProductId))
            .ToList();
            
        if (restrictedItemsInOrder.Any())
        {
            businessRuleViolations.Add(
                $"Order contains restricted products: {string.Join(", ", restrictedItemsInOrder.Select(i => i.ProductId))}");
        }
        
        if (businessRuleViolations.Any())
            return MlResult<ValidationResult>.Fail(string.Join("; ", businessRuleViolations));
        
        var result = new ValidationResult
        {
            ValidationType = "BusinessRules",
            IsValid = true,
            Details = new Dictionary<string, object>
            {
                ["MinimumOrderMet"] = orderTotal >= minimumOrderAmount,
                ["QuantityLimitsRespected"] = true,
                ["NoRestrictedProducts"] = true,
                ["OrderTotal"] = orderTotal
            }
        };
        
        return MlResult<ValidationResult>.Valid(result);
    }
    
    private async Task<MlResult<ValidatedOrder>> CreateValidatedOrderAsync(
        OrderRequest orderRequest, 
        IEnumerable<ValidationResult> validationResults)
    {
        await Task.Delay(25);
        
        var validationList = validationResults.ToList();
        var validatedOrder = new ValidatedOrder
        {
            OrderId = Guid.NewGuid(),
            OriginalRequest = orderRequest,
            ValidationResults = validationList,
            ValidatedAt = DateTime.UtcNow,
            ValidationSummary = validationList.ToDictionary(
                v => v.ValidationType,
                v => v.Details
            ),
            IsFullyValidated = validationList.All(v => v.IsValid),
            ValidationScore = CalculateValidationScore(validationList)
        };
        
        return MlResult<ValidatedOrder>.Valid(validatedOrder);
    }
    
    // Métodos auxiliares simulados
    private async Task<CustomerStatus> GetCustomerStatusAsync(int customerId)
    {
        await Task.Delay(10);
        return new CustomerStatus
        {
            IsActive = customerId % 10 != 0,
            IsSuspended = customerId % 50 == 0,
            Tier = customerId % 3 == 0 ? "Premium" : "Standard",
            AccountAge = TimeSpan.FromDays(customerId * 30)
        };
    }
    
    private async Task<InventoryAvailability> CheckInventoryAsync(string productId, int requestedQuantity)
    {
        await Task.Delay(5);
        var hash = productId.GetHashCode();
        var availableQuantity = Math.Abs(hash % 200) + 50;
        
        return new InventoryAvailability
        {
            Available = availableQuantity >= requestedQuantity,
            AvailableQuantity = availableQuantity,
            ReservationId = availableQuantity >= requestedQuantity ? Guid.NewGuid().ToString() : null
        };
    }
    
    private async Task<PaymentMethodInfo> GetPaymentMethodAsync(string paymentMethodId)
    {
        await Task.Delay(10);
        var hash = paymentMethodId.GetHashCode();
        
        return new PaymentMethodInfo
        {
            IsActive = Math.Abs(hash % 10) != 0,
            IsExpired = Math.Abs(hash % 20) == 0,
            Type = Math.Abs(hash % 2) == 0 ? "CreditCard" : "DebitCard",
            CreditLimit = Math.Abs(hash % 5000) + 1000
        };
    }
    
    private async Task<ShippingAvailability> CheckShippingAvailabilityAsync(Address address)
    {
        await Task.Delay(15);
        var hash = address.Country.GetHashCode();
        
        return new ShippingAvailability
        {
            Available = Math.Abs(hash % 10) != 0,
            Zone = Math.Abs(hash % 3) switch
            {
                0 => "Local",
                1 => "National",
                _ => "International"
            },
            EstimatedDays = Math.Abs(hash % 7) + 1,
            Cost = Math.Abs(hash % 50) + 5
        };
    }
    
    private async Task<decimal> GetCurrentProductPriceAsync(string productId)
    {
        await Task.Delay(5);
        var hash = productId.GetHashCode();
        return Math.Abs(hash % 10000) / 100m + 1;
    }
    
    private async Task<List<string>> GetRestrictedProductsForCustomerAsync(int customerId)
    {
        await Task.Delay(10);
        return customerId % 7 == 0 ? new List<string> { "RESTRICTED_ITEM_1" } : new List<string>();
    }
    
    private double CalculateValidationScore(List<ValidationResult> validationResults)
    {
        if (!validationResults.Any()) return 0;
        
        var validCount = validationResults.Count(v => v.IsValid);
        return (double)validCount / validationResults.Count * 100;
    }
}

// Clases de apoyo adicionales
public class OrderRequest
{
    public int CustomerId { get; set; }
    public string PaymentMethodId { get; set; }
    public Address ShippingAddress { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}

public class ValidationResult
{
    public string ValidationType { get; set; }
    public bool IsValid { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
}

public class ValidatedOrder
{
    public Guid OrderId { get; set; }
    public OrderRequest OriginalRequest { get; set; }
    public List<ValidationResult> ValidationResults { get; set; }
    public DateTime ValidatedAt { get; set; }
    public Dictionary<string, Dictionary<string, object>> ValidationSummary { get; set; }
    public bool IsFullyValidated { get; set; }
    public double ValidationScore { get; set; }
}

public class CustomerStatus
{
    public bool IsActive { get; set; }
    public bool IsSuspended { get; set; }
    public string Tier { get; set; }
    public TimeSpan AccountAge { get; set; }
}

public class InventoryAvailability
{
    public bool Available { get; set; }
    public int AvailableQuantity { get; set; }
    public string ReservationId { get; set; }
}

public class PaymentMethodInfo
{
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public string Type { get; set; }
    public decimal CreditLimit { get; set; }
}

public class ShippingAvailability
{
    public bool Available { get; set; }
    public string Zone { get; set; }
    public int EstimatedDays { get; set; }
    public decimal Cost { get; set; }
}
```

---

## Mejores Prácticas

### 1. Elección de Variante Apropiada

```csharp
// ✅ Correcto: Usar variante simple cuando solo necesitas validación
var result = userData
    .BindMulti(
        returnFunc: validData => ProcessUser(validData),
        ValidateAge,
        ValidateEmail,
        ValidatePhone
    );

// ✅ Correcto: Usar variante con agregación cuando necesitas los resultados
var result = userId
    .BindMulti(
        returnFunc: (id, profiles) => MergeProfiles(id, profiles),
        GetProfile1,
        GetProfile2,
        GetProfile3
    );

// ✅ Correcto: Usar variante con tipos diferentes cuando las validaciones retornan datos útiles
var result = orderData
    .BindMulti<OrderData, FinalResult, ValidationInfo>(
        returnFunc: (order, validations) => CreateResult(order, validations),
        ValidateCustomer,    // Retorna ValidationInfo
        ValidateInventory,   // Retorna ValidationInfo
        ValidatePayment      // Retorna ValidationInfo
    );
```

### 2. Gestión de Errores Acumulados

```csharp
// ✅ Correcto: Proporcionar mensajes específicos en cada validación
private MlResult<UserData> ValidateEmail(UserData user)
{
    return string.IsNullOrEmpty(user.Email)
        ? MlResult<UserData>.Fail("Email is required")
        : user.Email.Contains("@")
            ? MlResult<UserData>.Valid(user)
            : MlResult<UserData>.Fail("Email format is invalid");
}

private MlResult<UserData> ValidateAge(UserData user)
{
    return user.Age < 18
        ? MlResult<UserData>.Fail($"User must be at least 18 years old (current age: {user.Age})")
        : user.Age > 120
            ? MlResult<UserData>.Fail($"Invalid age: {user.Age}")
            : MlResult<UserData>.Valid(user);
}

// ❌ Incorrecto: Mensajes genéricos que no ayudan a identificar el problema
private MlResult<UserData> ValidateEmail(UserData user)
{
    return IsValidEmail(user.Email)
        ? MlResult<UserData>.Valid(user)
        : MlResult<UserData>.Fail("Invalid data"); // Muy genérico
}
```

### 3. Rendimiento en Validaciones

```csharp
// ✅ Correcto: Poner validaciones rápidas primero para fallo temprano
var result = orderData
    .BindMulti(
        returnFunc: validOrder => ProcessOrder(validOrder),
        ValidateBasicData,           // Rápido: verificaciones locales
        ValidateBusinessRules,       // Medio: lógica de negocio
        ValidateWithExternalAPI,     // Lento: llamadas externas
        ValidateWithDatabase         // Lento: consultas BD
    );

// ✅ Correcto: Para validaciones completamente independientes, considerar paralelismo
public async Task<MlResult<ValidatedData>> ValidateInParallelAsync(InputData data)
{
    var validationTasks = new[]
    {
        ValidateWithService1Async(data),
        ValidateWithService2Async(data),
        ValidateWithService3Async(data)
    };
    
    var results = await Task.WhenAll(validationTasks);
    
    // Usar BindMulti con los resultados
    return data.ToMlResult()
        .BindMulti(
            returnFunc: validData => ProcessValidatedData(validData),
            _ => results[0],
            _ => results[1],
            _ => results[2]
        );
}

// ❌ Incorrecto: Ejecutar validaciones costosas cuando las baratas ya fallaron
```

### 4. Composición con Otros Métodos

```csharp
// ✅ Correcto: Combinar BindMulti con otras operaciones de la librería
var result = await GetUserInput()
    .BindAsync(input => ValidateBasicInput(input))
    .BindMultiAsync(
        returnFuncAsync: async validInput => await ProcessCompletelyAsync(validInput),
        ValidateBusinessRules,
        ValidateWithExternalService,
        ValidateComplianceRules
    )
    .ExecSelfIfFailAsync(errors => LogValidationFailuresAsync(errors))
    .BindAsync(processedData => SaveResultsAsync(processedData));

// ✅ Correcto: Usar BindMulti como parte de pipelines más grandes
public async Task<MlResult<FinalResult>> CompleteWorkflowAsync(InitialData data)
{
    return await data.ToMlResult()
        .BindAsync(d => PreprocessDataAsync(d))
        .BindMultiAsync(
            returnFuncAsync: async preprocessed => await MainProcessingAsync(preprocessed),
            ValidateStep1,
            ValidateStep2,
            ValidateStep3
        )
        .BindAsync(processed => PostProcessAsync(processed))
        .Map(final => EnrichFinalResult(final));
}
```

### 5. Testing de BindMulti

```csharp
// ✅ Correcto: Testing completo de diferentes escenarios
[Test]
public async Task BindMulti_AllValidationsPass_ExecutesReturnFunction()
{
    // Arrange
    var inputData = CreateValidInputData();
    
    // Act
    var result = await inputData.ToMlResult()
        .BindMultiAsync(
            returnFuncAsync: async (data, validations) => await CreateSuccessResultAsync(data, validations),
            CreatePassingValidation1(),
            CreatePassingValidation2(),
            CreatePassingValidation3()
        );
    
    // Assert
    result.Should().BeSuccessful();
    result.Value.Should().NotBeNull();
}

[Test]
public async Task BindMulti_OneValidationFails_AccumulatesErrors()
{
    // Arrange
    var inputData = CreateInputData();
    
    // Act
    var result = await inputData.ToMlResult()
        .BindMultiAsync(
            returnFuncAsync: async (data, validations) => await CreateSuccessResultAsync(data, validations),
            CreatePassingValidation1(),
            CreateFailingValidation("Validation 2 failed"),
            CreatePassingValidation3()
        );
    
    // Assert
    result.Should().BeFailure();
    result.ErrorsDetails.GetMessage().Should().Contain("Validation 2 failed");
}

[Test]
public async Task BindMulti_MultipleValidationsFail_AccumulatesAllErrors()
{
    // Arrange
    var inputData = CreateInputData();
    
    // Act
    var result = await inputData.ToMlResult()
        .BindMultiAsync(
            returnFuncAsync: async (data, validations) => await CreateSuccessResultAsync(data, validations),
            CreateFailingValidation("Error 1"),
            CreatePassingValidation2(),
            CreateFailingValidation("Error 2")
        );
    
    // Assert
    result.Should().BeFailure();
    var errorMessage = result.ErrorsDetails.GetMessage();
    errorMessage.Should().Contain("Error 1");
    errorMessage.Should().Contain("Error 2");
}
```

---

## Consideraciones de Rendimiento

### Ejecución de Validaciones

- **Todas las validaciones se ejecutan**: A diferencia de `Bind` normal, `BindMulti` ejecuta todas las funciones incluso si alguna falla
- **Uso de memoria**: Los resultados se almacenan temporalmente para acumular errores
- **Overhead de agregación**: Costo adicional de fusionar errores múltiples

### Optimizaciones Sugeridas

1. **Orden de validaciones**: Poner validaciones rápidas primero para diagnóstico temprano
2. **Validaciones paralelas**: Para validaciones independientes, considerar paralelismo manual
3. **Lazy evaluation**: Para validaciones costosas, considerar implementar evaluación perezosa

### Patrones de Rendimiento

```csharp
// ✅ Optimizado: Validaciones ordenadas por costo
var result = data
    .BindMulti(
        returnFunc: processData,
        QuickLocalValidation,      // < 1ms
        BusinessRuleValidation,    // ~10ms
        DatabaseValidation,        // ~50ms
        ExternalAPIValidation      // ~200ms
    );

// ✅ Para casos de alto rendimiento: Pre-filtrado
public MlResult<ProcessedData> OptimizedValidation(InputData data)
{
    // Validación rápida primero
    var quickValidation = QuickValidation(data);
    if (quickValidation.IsFailure)
        return MlResult<ProcessedData>.Fail(quickValidation.ErrorsDetails);
    
    // Solo entonces hacer validaciones costosas
    return data.ToMlResult()
        .BindMulti(
            returnFunc: ProcessValidatedData,
            MediumCostValidation,
            HighCostValidation
        );
}
```

---

## Resumen

Las operaciones **BindMulti** proporcionan una funcionalidad poderosa para:

- **Validaciones Múltiples**: Ejecutar varias validaciones que deben pasar todas
- **Acumulación de Errores**: Recopilar todos los errores en lugar de fallar en el primero
- **Agregación de Datos**: Combinar resultados de múltiples fuentes
- **Flexibilidad de Tipos**: Soportar diferentes tipos entre validaciones y resultado final

Las tres variantes principales cubren diferentes escenarios:
1. **Variante Simple**: Solo validación, función final recibe valor original
2. **Variante con Agregación**: Función final recibe valor original + resultados de validaciones
3. **Variante con Tipos Diferentes**: Máxima flexibilidad para tipos heterogéneos

El soporte asíncrono completo permite integrar estas operaciones en aplicaciones modernas con operaciones I/O intensivas, manteniendo la composabilidad y robustez