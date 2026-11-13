using FsCheck;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

public class MlResultLawsTests
{
    [Fact]
    public void FsCheck_Validates_MapAndBindLaws()
        => Check.QuickThrowOnFailureAll(typeof(MlResultLaws));

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public async Task TryMap_SyncAsync_ValidValueMatches(int value)
    {
        var source = MlResult<int>.Valid(value);
        var sync = source.TryMap(static current => unchecked(current + 1), static _ => "delegate failed");
        var async = await source.TryMapAsync<int, int>(
            static current => Task.FromResult(unchecked(current + 1)),
            static _ => "delegate failed");

        SameResult(async, sync).Should().BeTrue();
    }

    [Fact]
    public async Task TryMap_SyncAsync_FailurePreservesSourceAndSkipsDelegate()
    {
        var source = Failure("source failure");
        var syncCalls = 0;
        var asyncCalls = 0;
        var sync = source.TryMap(current => { syncCalls++; return current + 1; }, static _ => "delegate failed");
        var async = await source.TryMapAsync<int, int>(current =>
        {
            asyncCalls++;
            return Task.FromResult(current + 1);
        }, static _ => "delegate failed");

        SameResult(sync, source).Should().BeTrue();
        SameResult(async, source).Should().BeTrue();
        syncCalls.Should().Be(0);
        asyncCalls.Should().Be(0);
    }

    [Fact]
    public async Task TryMap_SyncAsync_CapturesDelegateExceptionEqually()
    {
        var source = MlResult<int>.Valid(42);
        var sync = source.TryMap<int, int>(static _ => throw new InvalidOperationException("boom"), static _ => "delegate failed");
        var async = await source.TryMapAsync<int, int>(
            static _ => Task.FromException<int>(new InvalidOperationException("boom")),
            static _ => "delegate failed");

        SameResult(sync, async).Should().BeTrue();
        FailureMessages(sync).Should().ContainSingle().Which.Should().Be("delegate failed");
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public async Task TryBind_SyncAsync_ValidValueMatches(int value)
    {
        var source = MlResult<int>.Valid(value);
        var sync = source.TryBind(AddOne, static _ => "delegate failed");
        var async = await source.TryBindAsync<int, int>(
            static current => Task.FromResult(AddOne(current)),
            static _ => "delegate failed");

        SameResult(async, sync).Should().BeTrue();
    }

    [Fact]
    public async Task TryBind_SyncAsync_FailurePreservesSourceAndSkipsDelegate()
    {
        var source = Failure("source failure");
        var syncCalls = 0;
        var asyncCalls = 0;
        var sync = source.TryBind(current => { syncCalls++; return AddOne(current); }, static _ => "delegate failed");
        var async = await source.TryBindAsync<int, int>(current =>
        {
            asyncCalls++;
            return Task.FromResult(AddOne(current));
        }, static _ => "delegate failed");

        SameResult(sync, source).Should().BeTrue();
        SameResult(async, source).Should().BeTrue();
        syncCalls.Should().Be(0);
        asyncCalls.Should().Be(0);
    }

    [Fact]
    public async Task TryBind_SyncAsync_CapturesDelegateExceptionEqually()
    {
        var source = MlResult<int>.Valid(42);
        var sync = source.TryBind<int, int>(static _ => throw new InvalidOperationException("boom"), static _ => "delegate failed");
        var async = await source.TryBindAsync<int, int>(
            static _ => Task.FromException<MlResult<int>>(new InvalidOperationException("boom")),
            static _ => "delegate failed");

        SameResult(sync, async).Should().BeTrue();
        FailureMessages(sync).Should().ContainSingle().Which.Should().Be("delegate failed");
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public async Task TryExecSelf_SyncAsync_ValidValueAndEffectMatch(int value)
    {
        var source = MlResult<int>.Valid(value);
        var syncObserved = int.MinValue;
        var asyncObserved = int.MinValue;
        var sync = source.TryExecSelf(current => syncObserved = current, static _ => { }, static _ => "delegate failed");
        var async = await source.TryExecSelfAsync(
            current => { asyncObserved = current; return Task.CompletedTask; },
            static _ => Task.CompletedTask,
            static _ => "delegate failed");

        SameResult(async, sync).Should().BeTrue();
        syncObserved.Should().Be(value);
        asyncObserved.Should().Be(value);
    }

    [Fact]
    public async Task TryExecSelf_SyncAsync_FailureInvokesFailureBranchAndPreservesSource()
    {
        var source = Failure("source failure");
        var syncCalls = 0;
        var asyncCalls = 0;
        var sync = source.TryExecSelf(static _ => { }, _ => syncCalls++, static _ => "delegate failed");
        var async = await source.TryExecSelfAsync(
            static _ => Task.CompletedTask,
            _ => { asyncCalls++; return Task.CompletedTask; },
            static _ => "delegate failed");

        SameResult(sync, source).Should().BeTrue();
        SameResult(async, source).Should().BeTrue();
        syncCalls.Should().Be(1);
        asyncCalls.Should().Be(1);
    }

    [Fact]
    public async Task TryExecSelf_SyncAsync_CapturesDelegateExceptionEqually()
    {
        var source = MlResult<int>.Valid(42);
        var sync = source.TryExecSelf<int>(
            static _ => throw new InvalidOperationException("boom"),
            static _ => { },
            static _ => "delegate failed");
        var async = await source.TryExecSelfAsync<int>(
            static _ => Task.FromException(new InvalidOperationException("boom")),
            static _ => Task.CompletedTask,
            static _ => "delegate failed");

        SameResult(sync, async).Should().BeTrue();
        FailureMessages(sync).Should().ContainSingle().Which.Should().Be("delegate failed");
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public async Task Map_SyncAsync_ValidValueMatches(int value)
    {
        var source = MlResult<int>.Valid(value);
        var sync = source.Map(static current => unchecked(current + 1));
        var async = await source.MapAsync<int, int>(static current => Task.FromResult(unchecked(current + 1)));

        SameResult(sync, async).Should().BeTrue();
    }

    [Fact]
    public async Task Map_SyncAsync_FailurePreservesSourceAndSkipsDelegate()
    {
        var source = Failure("source failure");
        var syncCalls = 0;
        var asyncCalls = 0;
        var sync = source.Map(current => { syncCalls++; return current + 1; });
        var async = await source.MapAsync<int, int>(current =>
        {
            asyncCalls++;
            return Task.FromResult(current + 1);
        });

        SameResult(sync, source).Should().BeTrue();
        SameResult(async, source).Should().BeTrue();
        syncCalls.Should().Be(0);
        asyncCalls.Should().Be(0);
    }

    [Fact]
    public async Task Map_SyncAsync_PropagatesDelegateExceptionEqually()
    {
        var source = MlResult<int>.Valid(42);
        var sync = Record.Exception(() => source.Map<int, int>(static _ => throw new InvalidOperationException("boom")));
        var async = await Record.ExceptionAsync(async () =>
            await source.MapAsync<int, int>(static _ => Task.FromException<int>(new InvalidOperationException("boom"))));

        sync.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("boom");
        async.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("boom");
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public async Task Bind_SyncAsync_ValidValueMatches(int value)
    {
        var source = MlResult<int>.Valid(value);
        var sync = source.Bind(AddOne);
        var async = await source.BindAsync<int, int>(current => Task.FromResult(AddOne(current)));

        SameResult(sync, async).Should().BeTrue();
    }

    [Fact]
    public async Task Bind_SyncAsync_FailurePreservesSourceAndSkipsDelegate()
    {
        var source = Failure("source failure");
        var syncCalls = 0;
        var asyncCalls = 0;
        var sync = source.Bind(current => { syncCalls++; return AddOne(current); });
        var async = await source.BindAsync<int, int>(current =>
        {
            asyncCalls++;
            return Task.FromResult(AddOne(current));
        });

        SameResult(sync, source).Should().BeTrue();
        SameResult(async, source).Should().BeTrue();
        syncCalls.Should().Be(0);
        asyncCalls.Should().Be(0);
    }

    [Fact]
    public async Task Bind_SyncAsync_PropagatesDelegateExceptionEqually()
    {
        var source = MlResult<int>.Valid(42);
        var sync = Record.Exception(() => source.Bind<int, int>(static _ => throw new InvalidOperationException("boom")));
        var async = await Record.ExceptionAsync(async () =>
            await source.BindAsync<int, int>(static _ => Task.FromException<MlResult<int>>(new InvalidOperationException("boom"))));

        sync.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("boom");
        async.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("boom");
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public async Task ExecSelf_SyncAsync_ValidValueAndEffectMatch(int value)
    {
        var source = MlResult<int>.Valid(value);
        var syncObserved = int.MinValue;
        var asyncObserved = int.MinValue;
        var sync = source.ExecSelf(current => syncObserved = current, static _ => { });
        var async = await source.ExecSelfAsync(
            current => { asyncObserved = current; return Task.CompletedTask; },
            static _ => Task.CompletedTask);

        SameResult(sync, async).Should().BeTrue();
        syncObserved.Should().Be(value);
        asyncObserved.Should().Be(value);
    }

    [Fact]
    public async Task ExecSelf_SyncAsync_FailureInvokesFailureBranchAndPreservesSource()
    {
        var source = Failure("source failure");
        var syncCalls = 0;
        var asyncCalls = 0;
        var sync = source.ExecSelf(static _ => { }, _ => syncCalls++);
        var async = await source.ExecSelfAsync(
            static _ => Task.CompletedTask,
            _ => { asyncCalls++; return Task.CompletedTask; });

        SameResult(sync, source).Should().BeTrue();
        SameResult(async, source).Should().BeTrue();
        syncCalls.Should().Be(1);
        asyncCalls.Should().Be(1);
    }

    [Fact]
    public async Task ExecSelf_SyncAsync_PropagatesDelegateExceptionEqually()
    {
        var source = MlResult<int>.Valid(42);
        var sync = Record.Exception(() => source.ExecSelf<int>(
            static _ => throw new InvalidOperationException("boom"), static _ => { }));
        var async = await Record.ExceptionAsync(async () =>
            await source.ExecSelfAsync<int>(
                static _ => Task.FromException(new InvalidOperationException("boom")),
                static _ => Task.CompletedTask));

        sync.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("boom");
        async.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("boom");
    }

    internal static MlResult<int> AddOne(int value)
        => MlResult<int>.Valid(unchecked(value + 1));

    internal static MlResult<int> Double(int value)
        => MlResult<int>.Valid(unchecked(value * 2));

    private static MlResult<int> Failure(string message)
        => MlResult<int>.Fail(MlError.FromErrorMessage(message));

    private static IEnumerable<string> FailureMessages(MlResult<int> result)
        => result.GetFailureDetailsOrThrow().Errors.Select(static error => error.Message);

    internal static bool SameResult(MlResult<int> left, MlResult<int> right)
    {
        if (left.IsSuccess != right.IsSuccess)
            return false;

        return left.IsSuccess
            ? left.GetValueOrThrow() == right.GetValueOrThrow()
            : FailureMessages(left).SequenceEqual(FailureMessages(right));
    }
}

public static class MlResultLaws
{
    public static bool Map_Identity(int value)
    {
        var source = MlResult<int>.Valid(value);
        return MlResultLawsTests.SameResult(source.Map(static current => current), source);
    }

    public static bool Map_Composition(int value)
    {
        var source = MlResult<int>.Valid(value);
        var composed = source.Map(static current => unchecked(current + 3))
                             .Map(static current => unchecked(current * 2));
        var single = source.Map(static current => unchecked((current + 3) * 2));
        return MlResultLawsTests.SameResult(composed, single);
    }

    public static bool Bind_LeftIdentity(int value)
        => MlResultLawsTests.SameResult(MlResult<int>.Valid(value).Bind(MlResultLawsTests.AddOne),
                                        MlResultLawsTests.AddOne(value));

    public static bool Bind_RightIdentity(int value)
    {
        var source = MlResult<int>.Valid(value);
        return MlResultLawsTests.SameResult(source.Bind(static current => MlResult<int>.Valid(current)), source);
    }

    public static bool Bind_Associativity(int value)
    {
        var source = MlResult<int>.Valid(value);
        var left = source.Bind(MlResultLawsTests.AddOne).Bind(MlResultLawsTests.Double);
        var right = source.Bind(static current => MlResultLawsTests.AddOne(current).Bind(MlResultLawsTests.Double));
        return MlResultLawsTests.SameResult(left, right);
    }
}
