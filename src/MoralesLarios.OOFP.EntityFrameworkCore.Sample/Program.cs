using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.EntityFrameworkCore;
using MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;

await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var services = new ServiceCollection();
services.AddLogging();
services.AddSingleton(connection);
services.AddDbContext<SampleDbContext>((serviceProvider, options) =>
    options.UseSqlite(serviceProvider.GetRequiredService<SqliteConnection>()));
services.AddOofpRepositories<SampleOrder, SampleDbContext>();
services.AddOofpRepositories<SampleOrderLine, SampleDbContext>();

await using var provider = services.BuildServiceProvider();
await using var scope = provider.CreateAsyncScope();
var context = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
await context.Database.EnsureCreatedAsync();

var orders = scope.ServiceProvider.GetRequiredService<IEfRepository<SampleOrder>>();
var lines = scope.ServiceProvider.GetRequiredService<IEfCreateRepository<SampleOrderLine>>();
var unitOfWork = orders.UnitOfWork;
var order = new SampleOrder { Reference = "ORD-001" };
var line = new SampleOrderLine { OrderId = order.Id, Description = "Producto de ejemplo" };

var committed = await unitOfWork.TryExecuteInTransactionAsync<string>(async token =>
{
    var orderResult = await orders.TryAddAsync(order, token);
    if (orderResult.IsFailure)
        return orderResult.GetFailureDetailsOrThrow().ToMlResultFail<string>();

    var lineResult = await lines.TryAddAsync(line, token);
    if (lineResult.IsFailure)
        return lineResult.GetFailureDetailsOrThrow().ToMlResultFail<string>();

    return $"Pedido {order.Reference} creado con detalle.".ToMlResultValid();
});

var failedOrder = new SampleOrder { Reference = "ORD-ROLLBACK" };
var failedLine = new SampleOrderLine { OrderId = failedOrder.Id, Description = "Detalle revertido" };
var rolledBack = await unitOfWork.TryExecuteInTransactionAsync<string>(async token =>
{
    var orderResult = await orders.TryAddAsync(failedOrder, token);
    if (orderResult.IsFailure)
        return orderResult.GetFailureDetailsOrThrow().ToMlResultFail<string>();

    var lineResult = await lines.TryAddAsync(failedLine, token);
    if (lineResult.IsFailure)
        return lineResult.GetFailureDetailsOrThrow().ToMlResultFail<string>();

    return "Fallo intencional: la transacción debe revertirse.".ToMlResultFail<string>();
});

var orderCount = await context.Orders.AsNoTracking().CountAsync();
var lineCount = await context.OrderLines.AsNoTracking().CountAsync();
Console.WriteLine($"Transacción correcta: {committed.IsSuccess}");
Console.WriteLine($"Rollback del caso fallido: {rolledBack.IsFailure}");
Console.WriteLine($"Filas persistidas: pedidos={orderCount}, detalles={lineCount}");

return committed.IsSuccess && rolledBack.IsFailure && orderCount == 1 && lineCount == 1 ? 0 : 1;

internal sealed class SampleDbContext(DbContextOptions<SampleDbContext> options) : DbContext(options)
{
    public DbSet<SampleOrder> Orders => Set<SampleOrder>();
    public DbSet<SampleOrderLine> OrderLines => Set<SampleOrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SampleOrder>().HasKey(order => order.Id);
        modelBuilder.Entity<SampleOrderLine>().HasKey(line => line.Id);
        modelBuilder.Entity<SampleOrderLine>()
                    .HasOne<SampleOrder>()
                    .WithMany()
                    .HasForeignKey(line => line.OrderId);
    }
}

internal sealed class SampleOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty;
}

internal sealed class SampleOrderLine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public string Description { get; set; } = string.Empty;
}