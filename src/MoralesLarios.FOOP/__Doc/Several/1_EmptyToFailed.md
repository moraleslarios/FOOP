# MlResult EmptyToFailed - Conversión de Colecciones Vacías a Errores

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos EmptyToFailed Básicos](#métodos-emptytoFailed-básicos)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Mejores Prácticas](#mejores-prácticas)
7. [Comparación con Otros Métodos de Validación](#comparación-con-otros-métodos-de-validación)

---

## Introducción

Los métodos `EmptyToFailed` proporcionan una forma de **convertir colecciones vacías en resultados fallidos**, tratando la ausencia de elementos como un error. Estos métodos son útiles cuando se espera que una colección contenga al menos un elemento y su vacuidad indica un problema en el flujo de datos.

### Propósito Principal

- **Validación de Colecciones**: Asegurar que las colecciones no estén vacías
- **Conversión a MlResult**: Transformar `IEnumerable<T>` en `MlResult<IEnumerable<T>>`
- **Manejo de Casos de Negocio**: Tratar colecciones vacías como errores de negocio
- **Early Validation**: Detectar problemas de datos temprano en el pipeline

---

## Análisis de los Métodos

### Filosofía de EmptyToFailed

```
IEnumerable<T> → EmptyToFailed(error) → MlResult<IEnumerable<T>>
       ↓                 ↓                         ↓
 Con elementos → MlResult<IEnumerable<T>>.Valid(items)
       ↓                 ↓                         ↓
 Vacía/null → MlResult<IEnumerable<T>>.Fail(error)
```

### Características Principales

1. **Validación de Contenido**: Verifica que la colección tenga elementos
2. **Manejo de null**: Trata colecciones null como vacías (error)
3. **Preservación de Datos**: Si hay elementos, los mantiene intactos
4. **Flexibilidad de Errores**: Acepta diferentes tipos de error
5. **Soporte Asíncrono**: Variantes para operaciones asíncronas

---

## Métodos EmptyToFailed Básicos

### `EmptyToFailed<T>()` - Con MlError

**Propósito**: Convertir colección vacía en resultado fallido usando un `MlError`

```csharp
public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, 
                                                         MlError error)
```

**Comportamiento**:
- Si `items` tiene elementos: retorna `MlResult<IEnumerable<T>>.Valid(items)`
- Si `items` es null o vacía: retorna `MlResult<IEnumerable<T>>.Fail(error)`

**Ejemplo Básico**:
```csharp
var users = GetActiveUsers();
var result = users.EmptyToFailed(MlError.FromErrorMessage("No active users found"));

// Si users tiene elementos: MlResult válido con los usuarios
// Si users está vacía: MlResult fallido con el error especificado
```

### `EmptyToFailed<T>()` - Con MlErrorsDetails

```csharp
public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, 
                                                         MlErrorsDetails errorsDetails)
```

**Ejemplo**:
```csharp
var orders = GetPendingOrders(customerId);
var errorDetails = new MlErrorsDetails(new[] { "No pending orders", "Customer may not exist" });
var result = orders.EmptyToFailed(errorDetails);
```

### `EmptyToFailed<T>()` - Con String

```csharp
public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, 
                                                         string messageError)
```

**Ejemplo**:
```csharp
var products = SearchProducts(criteria);
var result = products.EmptyToFailed("No products match the search criteria");
```

---

## Variantes Asíncronas

### `EmptyToFailedAsync<T>()` - Colección Síncrona

```csharp
public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(
    this IEnumerable<T> items, 
    MlError error)

public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(
    this IEnumerable<T> items, 
    MlErrorsDetails errorsDetails)

public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(
    this IEnumerable<T> items, 
    string messageError)
```

**Ejemplo**:
```csharp
var result = await GetUsers()
    .EmptyToFailedAsync("No users available");
```

### `EmptyToFailedAsync<T>()` - Colección Asíncrona

```csharp
public static async Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(
    this Task<IEnumerable<T>> itemsAsync, 
    MlError error)

public static async Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(
    this Task<IEnumerable<T>> itemsAsync, 
    MlErrorsDetails errorsDetails)

public static async Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(
    this Task<IEnumerable<T>> itemsAsync, 
    string messageError)
```

**Ejemplo**:
```csharp
var result = await GetUsersAsync()
    .EmptyToFailedAsync("No users found in database");
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Búsqueda y Filtrado

```csharp
public class ProductSearchService
{
    private readonly IProductRepository _productRepo;
    private readonly ILogger _logger;
    
    public async Task<MlResult<SearchResponse<Product>>> SearchProductsAsync(SearchCriteria criteria)
    {
        return await ValidateSearchCriteria(criteria)
            .BindAsync(async validCriteria => await ExecuteSearchAsync(validCriteria))
            .BindAsync(async products => await ApplyUserFiltersAsync(products, criteria.UserId))
            .BindAsync(async filteredProducts => await products
                .EmptyToFailedAsync("No products match your search criteria"))
            .MapAsync(async products => await BuildSearchResponseAsync(products, criteria));
    }
    
    public async Task<MlResult<IEnumerable<Product>>> GetUserFavoritesAsync(int userId)
    {
        var favorites = await _productRepo.GetUserFavoritesAsync(userId);
        
        return await favorites
            .EmptyToFailedAsync($"User {userId} has no favorite products yet");
    }
    
    public async Task<MlResult<IEnumerable<Order>>> GetRecentOrdersAsync(int customerId, int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        var orders = await _productRepo.GetOrdersSinceAsync(customerId, cutoffDate);
        
        return await orders
            .EmptyToFailedAsync(MlError.FromErrorMessage(
                $"No orders found for customer {customerId} in the last {days} days"));
    }
    
    public async Task<MlResult<CategoryResponse>> GetCategoriesWithProductsAsync()
    {
        var allCategories = await _productRepo.GetAllCategoriesAsync();
        
        var categoriesWithProducts = new List<CategoryWithProducts>();
        
        foreach (var category in allCategories)
        {
            var products = await _productRepo.GetProductsByCategoryAsync(category.Id);
            var productsResult = products.EmptyToFailed($"Category {category.Name} has no products");
            
            if (productsResult.IsValid)
            {
                categoriesWithProducts.Add(new CategoryWithProducts
                {
                    Category = category,
                    Products = productsResult.Value.ToList(),
                    ProductCount = productsResult.Value.Count()
                });
            }
            else
            {
                await _logger.LogWarningAsync($"Skipping empty category: {category.Name}");
            }
        }
        
        return categoriesWithProducts
            .EmptyToFailed("No categories with products available")
            .Map(categories => new CategoryResponse
            {
                Categories = categories.ToArray(),
                TotalCategories = categories.Count(),
                Timestamp = DateTime.UtcNow
            });
    }
}

public class SearchCriteria
{
    public string Query { get; set; }
    public string Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int UserId { get; set; }
}

public class SearchResponse<T>
{
    public IEnumerable<T> Results { get; set; }
    public int TotalCount { get; set; }
    public SearchCriteria AppliedCriteria { get; set; }
    public DateTime SearchTime { get; set; }
}

public class CategoryWithProducts
{
    public Category Category { get; set; }
    public List<Product> Products { get; set; }
    public int ProductCount { get; set; }
}

public class CategoryResponse
{
    public CategoryWithProducts[] Categories { get; set; }
    public int TotalCategories { get; set; }
    public DateTime Timestamp { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}
```

### Ejemplo 2: Sistema de Reportes y Analytics

```csharp
public class ReportingService
{
    private readonly IDataRepository _dataRepo;
    private readonly IAnalyticsEngine _analytics;
    
    public async Task<MlResult<SalesReport>> GenerateSalesReportAsync(ReportPeriod period)
    {
        return await GetSalesDataAsync(period)
            .BindAsync(async sales => await sales
                .EmptyToFailedAsync($"No sales data available for period {period.Start:yyyy-MM-dd} to {period.End:yyyy-MM-dd}"))
            .BindAsync(async sales => await CalculateMetricsAsync(sales))
            .MapAsync(async metrics => await GenerateReportAsync(metrics, period));
    }
    
    public async Task<MlResult<PerformanceReport>> GeneratePerformanceReportAsync(int[] employeeIds)
    {
        var performanceData = new List<EmployeePerformance>();
        
        foreach (var employeeId in employeeIds)
        {
            var tasks = await _dataRepo.GetCompletedTasksAsync(employeeId);
            var tasksResult = tasks.EmptyToFailed($"Employee {employeeId} has no completed tasks");
            
            if (tasksResult.IsValid)
            {
                var performance = await _analytics.CalculatePerformanceAsync(tasksResult.Value);
                performanceData.Add(new EmployeePerformance
                {
                    EmployeeId = employeeId,
                    CompletedTasks = tasksResult.Value.Count(),
                    PerformanceScore = performance.Score,
                    Efficiency = performance.Efficiency
                });
            }
        }
        
        return performanceData
            .EmptyToFailed("No performance data available for any of the specified employees")
            .Map(data => new PerformanceReport
            {
                EmployeePerformances = data.ToArray(),
                GeneratedAt = DateTime.UtcNow,
                Period = GetCurrentPeriod(),
                AverageScore = data.Average(p => p.PerformanceScore)
            });
    }
    
    public async Task<MlResult<TrendAnalysis>> AnalyzeTrendsAsync(string metric, TimeSpan period)
    {
        var dataPoints = await _dataRepo.GetMetricDataAsync(metric, period);
        
        return await dataPoints
            .EmptyToFailedAsync($"Insufficient data points for trend analysis of metric '{metric}'")
            .BindAsync(async points => await ValidateDataQualityAsync(points))
            .MapAsync(async validPoints => await _analytics.AnalyzeTrendsAsync(validPoints));
    }
    
    private async Task<MlResult<IEnumerable<DataPoint>>> ValidateDataQualityAsync(
        IEnumerable<DataPoint> dataPoints)
    {
        var validPoints = dataPoints.Where(dp => dp.IsValid && dp.Value.HasValue).ToList();
        
        return validPoints
            .EmptyToFailed("No valid data points remain after quality validation")
            .ToMlResult();
    }
}

public class ReportPeriod
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class SalesReport
{
    public decimal TotalSales { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public ReportPeriod Period { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class EmployeePerformance
{
    public int EmployeeId { get; set; }
    public int CompletedTasks { get; set; }
    public double PerformanceScore { get; set; }
    public double Efficiency { get; set; }
}

public class PerformanceReport
{
    public EmployeePerformance[] EmployeePerformances { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string Period { get; set; }
    public double AverageScore { get; set; }
}

public class TrendAnalysis
{
    public string Metric { get; set; }
    public string TrendDirection { get; set; }
    public double TrendStrength { get; set; }
    public DataPoint[] DataPoints { get; set; }
    public DateTime AnalyzedAt { get; set; }
}

public class DataPoint
{
    public DateTime Timestamp { get; set; }
    public double? Value { get; set; }
    public bool IsValid { get; set; }
}
```

### Ejemplo 3: Sistema de Validación de Datos de Entrada

```csharp
public class DataValidationService
{
    public async Task<MlResult<ProcessedBatch<T>>> ValidateAndProcessBatchAsync<T>(
        IEnumerable<T> items,
        Func<T, Task<ValidationResult>> validator,
        Func<T, Task<T>> processor) where T : class
    {
        // Validar que hay elementos para procesar
        var itemsResult = items.EmptyToFailed("Batch cannot be empty");
        if (itemsResult.IsFailed)
            return itemsResult.Errors.ToMlResultFail<ProcessedBatch<T>>();
        
        var validItems = new List<T>();
        var invalidItems = new List<ValidationError<T>>();
        
        // Validar cada elemento
        foreach (var item in itemsResult.Value)
        {
            var validationResult = await validator(item);
            if (validationResult.IsValid)
            {
                validItems.Add(item);
            }
            else
            {
                invalidItems.Add(new ValidationError<T>
                {
                    Item = item,
                    Errors = validationResult.Errors
                });
            }
        }
        
        // Verificar que hay elementos válidos para procesar
        var validItemsResult = validItems.EmptyToFailed("No valid items found in batch");
        if (validItemsResult.IsFailed)
        {
            return MlResult<ProcessedBatch<T>>.Fail(
                "Batch validation failed: all items are invalid");
        }
        
        // Procesar elementos válidos
        var processedItems = new List<T>();
        var processingErrors = new List<ProcessingError<T>>();
        
        foreach (var validItem in validItemsResult.Value)
        {
            try
            {
                var processedItem = await processor(validItem);
                processedItems.Add(processedItem);
            }
            catch (Exception ex)
            {
                processingErrors.Add(new ProcessingError<T>
                {
                    Item = validItem,
                    Exception = ex,
                    ErrorMessage = $"Processing failed: {ex.Message}"
                });
            }
        }
        
        // Verificar que hay elementos procesados exitosamente
        var processedResult = processedItems.EmptyToFailed("No items were processed successfully");
        if (processedResult.IsFailed)
        {
            return MlResult<ProcessedBatch<T>>.Fail(
                "Batch processing failed: no items could be processed");
        }
        
        return MlResult<ProcessedBatch<T>>.Valid(new ProcessedBatch<T>
        {
            SuccessfulItems = processedResult.Value.ToArray(),
            ValidationErrors = invalidItems.ToArray(),
            ProcessingErrors = processingErrors.ToArray(),
            TotalItems = items.Count(),
            SuccessfulCount = processedResult.Value.Count(),
            ProcessedAt = DateTime.UtcNow
        });
    }
    
    public MlResult<ContactList> ValidateContactListAsync(IEnumerable<ContactInfo> contacts)
    {
        var contactsResult = contacts.EmptyToFailed("Contact list cannot be empty");
        if (contactsResult.IsFailed)
            return contactsResult.Errors.ToMlResultFail<ContactList>();
        
        var validContacts = contactsResult.Value
            .Where(c => !string.IsNullOrEmpty(c.Email) && !string.IsNullOrEmpty(c.Name))
            .ToList();
        
        var validContactsResult = validContacts.EmptyToFailed("No valid contacts found");
        if (validContactsResult.IsFailed)
            return validContactsResult.Errors.ToMlResultFail<ContactList>();
        
        var duplicateEmails = validContactsResult.Value
            .GroupBy(c => c.Email.ToLower())
            .Where(g => g.Count() > 1)
            .ToList();
        
        var uniqueContacts = validContactsResult.Value
            .GroupBy(c => c.Email.ToLower())
            .Select(g => g.First())
            .ToList();
        
        var uniqueContactsResult = uniqueContacts.EmptyToFailed(
            "No unique contacts remain after duplicate removal");
        if (uniqueContactsResult.IsFailed)
            return uniqueContactsResult.Errors.ToMlResultFail<ContactList>();
        
        return MlResult<ContactList>.Valid(new ContactList
        {
            Contacts = uniqueContactsResult.Value.ToArray(),
            TotalOriginal = contacts.Count(),
            TotalValid = validContacts.Count,
            TotalUnique = uniqueContactsResult.Value.Count(),
            DuplicatesRemoved = duplicateEmails.Sum(g => g.Count() - 1)
        });
    }
}

public class ProcessedBatch<T>
{
    public T[] SuccessfulItems { get; set; }
    public ValidationError<T>[] ValidationErrors { get; set; }
    public ProcessingError<T>[] ProcessingErrors { get; set; }
    public int TotalItems { get; set; }
    public int SuccessfulCount { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ValidationError<T>
{
    public T Item { get; set; }
    public string[] Errors { get; set; }
}

public class ProcessingError<T>
{
    public T Item { get; set; }
    public Exception Exception { get; set; }
    public string ErrorMessage { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string[] Errors { get; set; }
}

public class ContactInfo
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class ContactList
{
    public ContactInfo[] Contacts { get; set; }
    public int TotalOriginal { get; set; }
    public int TotalValid { get; set; }
    public int TotalUnique { get; set; }
    public int DuplicatesRemoved { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar EmptyToFailed

```csharp
// ✅ Correcto: Cuando se espera al menos un elemento
var users = GetActiveUsers()
    .EmptyToFailed("No active users available for processing");

// ✅ Correcto: Validación de resultados de búsqueda
var searchResults = SearchProducts(criteria)
    .EmptyToFailed("No products match your search criteria");

// ✅ Correcto: Validación de datos de entrada críticos
var requiredItems = GetRequiredConfigItems()
    .EmptyToFailed("Required configuration items missing");

// ❌ Incorrecto: Cuando colecciones vacías son válidas
var optionalItems = GetOptionalSettings()
    .EmptyToFailed("No optional settings"); // Las opcionales pueden estar vacías
```

### 2. Mensajes de Error Descriptivos

```csharp
// ✅ Correcto: Mensajes específicos y útiles
var orders = GetPendingOrders(customerId)
    .EmptyToFailed($"Customer {customerId} has no pending orders");

var products = SearchByCategory(categoryId)
    .EmptyToFailed($"Category {categoryId} contains no available products");

// ❌ Incorrecto: Mensajes genéricos poco útiles
var items = GetItems()
    .EmptyToFailed("Empty"); // Muy genérico

var data = GetData()
    .EmptyToFailed("Error"); // No describe el problema
```

### 3. Combinación con Otros Métodos

```csharp
// ✅ Correcto: Usar en pipelines de validación
var result = await GetRawDataAsync()
    .BindAsync(async data => await ValidateDataAsync(data))
    .BindAsync(async validData => await FilterDataAsync(validData))
    .BindAsync(async filtered => await filtered
        .EmptyToFailedAsync("No data remains after filtering"))
    .MapAsync(async finalData => await ProcessDataAsync(finalData));

// ✅ Correcto: Validación temprana en el pipeline
var processedUsers = GetUsers()
    .EmptyToFailed("No users to process")
    .Bind(users => ValidateUsers(users))
    .Map(validUsers => TransformUsers(validUsers));
```

### 4. Manejo de Diferentes Tipos de Error

```csharp
// ✅ Correcto: Usar tipos de error apropiados según el contexto
var criticalData = GetCriticalSystemData()
    .EmptyToFailed(MlError.FromErrorMessage("CRITICAL: No system data available"));

var userPreferences = GetUserPreferences(userId)
    .EmptyToFailed(new MlErrorsDetails(new[] {
        $"User {userId} has no preferences set",
        "Default preferences will be applied",
        "Consider prompting user to set preferences"
    }));

var simpleList = GetItems()
    .EmptyToFailed("No items found"); // String simple para casos básicos
```

---

## Comparación con Otros Métodos de Validación

### Tabla Comparativa

| Método | Entrada | Salida | Cuándo Usar | Propósito |
|--------|---------|--------|-------------|-----------|
| `EmptyToFailed` | `IEnumerable<T>` | `MlResult<IEnumerable<T>>` | Colecciones que deben tener elementos | Validar no-vacuidad |
| `Bind` | `MlResult<T>` | `MlResult<TResult>` | Operaciones que pueden fallar | Encadenar validaciones |
| `Map` | `MlResult<T>` | `MlResult<TResult>` | Transformaciones seguras | Transformar valores |

### Ejemplo Comparativo

```csharp
// EmptyToFailed: Convierte colección vacía en error
var users = GetUsers()
    .EmptyToFailed("No users available");

// Bind: Encadena validaciones que pueden fallar
var result = GetUserIds()
    .Bind(ids => ValidateUserIds(ids))
    .Bind(validIds => LoadUsers(validIds));

// Map: Transforma valores sin validación adicional
var userNames = GetUsers()
    .Map(users => users.Select(u => u.Name));

// Combinación típica
var processedUsers = GetUsers()
    .EmptyToFailed("No users to process")           // Validar no-vacuidad
    .Bind(users => ValidateUsers(users))            // Validar contenido
    .Map(validUsers => TransformUsers(validUsers)); // Transformar resultado
```

---

## Resumen

Los métodos `EmptyToFailed` proporcionan **validación de colecciones no vacías**:

- **`EmptyToFailed`**: Convierte colecciones vacías en resultados fallidos
- **`EmptyToFailedAsync`**: Soporte completo para operaciones asíncronas
- **Flexibilidad de errores**: Acepta `MlError`, `MlErrorsDetails` o `string`

**Casos de uso ideales**:
- **Validación de datos de entrada** que requieren elementos
- **Resultados de búsqueda** donde vacuidad es problemática
- **Configuraciones críticas** que deben contener valores
- **Pipelines de procesamiento** que necesitan datos para funcionar

**Ventajas principales**:
- **Validación temprana** de requisitos de datos
- **Mensajes de error claros** y específicos
- **Integración perfecta** con pipelines de `MlResult`
- **Soporte asíncrono completo** para todas las variantes