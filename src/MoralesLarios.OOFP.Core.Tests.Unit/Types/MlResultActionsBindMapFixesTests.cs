namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

/// <summary>
/// Tests que demuestran la correccion de los bugs detectados en la auditoria de
/// <c>MlResultActionsMap</c> y <c>MlResultActionsBind</c>:
///
///  - Recursiones infinitas (StackOverflowException) en los overloads de reenvio.
///  - Doble await de la Task fuente (<c>(await sourceAsync).Value</c>).
///  - Uso de <c>source.Value</c> en vez del valor ya casado por el <c>Match</c>.
///  - Task fuente no observada en <c>BindAlwaysAsync</c>.
///  - Auto-recursion en <c>TryBindAlwaysAsync</c>.
///  - Perdida del filtro <c>TException</c> en <c>TryBindIfFailWithoutExceptionAsync</c>.
///
/// Nota: una recursion infinita produce <see cref="StackOverflowException"/>, que NO se puede
/// capturar en .NET (mata el proceso). Por tanto, el simple hecho de que estos tests
/// terminen y hagan sus asserts ya demuestra que la recursion ha desaparecido.
/// </summary>
public class MlResultActionsBindMapFixesTests
{
    private const string ErrorMsg = "boom";

    private static MlResult<int> ValidSource(int value = 10) => MlResult<int>.Valid(value);

    private static MlResult<int> FailSource(string message = "failed")
        => message.ToMlResultFail<int>();

    private static MlResult<int> FailSourceWithException(Exception ex)
        => MlResult<int>.Fail(MlErrorsDetails.FromErrorMessageWithException(ex.Message, ex));

    /// <summary>
    /// Fuente asincrona que cuenta cuantas veces se ha materializado la Task subyacente.
    /// Permite demostrar que los overloads con <c>Task&lt;MlResult&lt;T&gt;&gt;</c>
    /// solo hacen await de la fuente UNA vez (bugs B1/B2).
    /// </summary>
    private sealed class CountingSource
    {
        private int _awaits;

        public int Awaits => _awaits;

        public Task<MlResult<int>> Create(MlResult<int> result) => CreateCore(result);

        private async Task<MlResult<int>> CreateCore(MlResult<int> result)
        {
            Interlocked.Increment(ref _awaits);
            await Task.Yield();
            return result;
        }
    }


    #region B1 - TryBindAsync(Task source) no debe hacer doble await ni usar source.Value


    [Fact]
    public async Task TryBindAsync_TaskSource_AsyncFunc_ShouldAwaitSourceOnlyOnce()
    {
        var counter = new CountingSource();
        var source  = counter.Create(ValidSource(7));

        var result = await source.TryBindAsync(funcAsync          : value => MlResult<int>.Valid(value * 2).ToAsync(),
                                               errorMessageBuilder: _     => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(14);
        counter.Awaits.Should().Be(1, "la Task fuente solo se debe materializar una vez");
    }


    [Fact]
    public async Task TryBindAsync_TaskSource_AsyncFunc_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = await ValidSource(33).ToAsync()
                                          .TryBindAsync(funcAsync: value =>
                                                        {
                                                            received = value;
                                                            return MlResult<int>.Valid(value).ToAsync();
                                                        },
                                                        errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        received.Should().Be(33, "debe recibir el valor casado por el Match, no source.Value");
    }


    [Fact]
    public async Task TryBindAsync_TaskSource_WhenFail_ShouldNotExecuteFunc()
    {
        var executed = false;

        var result = await FailSource().ToAsync()
                                       .TryBindAsync(funcAsync: _ =>
                                                     {
                                                         executed = true;
                                                         return MlResult<int>.Valid(0).ToAsync();
                                                     },
                                                     errorMessageBuilder: _ => ErrorMsg);

        result.IsFail.Should().BeTrue();
        executed.Should().BeFalse();
    }


    [Fact]
    public async Task TryBindAsync_TaskSource_WhenFuncThrows_ShouldCaptureException()
    {
        Func<int, Task<MlResult<int>>> throwingFunc = _ => throw new InvalidOperationException("inner");

        var result = await ValidSource().ToAsync()
                                        .TryBindAsync(funcAsync          : throwingFunc,
                                                      errorMessageBuilder: ex => $"{ErrorMsg}:{ex.Message}");

        result.IsFail.Should().BeTrue();
    }


    #endregion


    #region B3 - TryBind / TryBindAsync (MlResult source) deben usar el valor casado


    [Fact]
    public void TryBind_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = ValidSource(5).TryBind(func: value =>
                                            {
                                                received = value;
                                                return MlResult<int>.Valid(value + 1);
                                            },
                                            errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(6);
        received.Should().Be(5);
    }


    [Fact]
    public void TryBind_WhenFuncThrows_ShouldCaptureExceptionAsFail()
    {
        Func<int, MlResult<int>> throwingFunc = _ => throw new InvalidOperationException("inner");

        var result = ValidSource().TryBind(func               : throwingFunc,
                                           errorMessageBuilder: ex => $"{ErrorMsg}:{ex.Message}");

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task TryBindAsync_MlResultSource_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = await ValidSource(21).TryBindAsync(funcAsync: value =>
                                                        {
                                                            received = value;
                                                            return MlResult<int>.Valid(value).ToAsync();
                                                        },
                                                        errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        received.Should().Be(21);
    }


    #endregion


    #region B2 - TryMap / TryMapAsync deben usar el valor casado y un solo await


    [Fact]
    public void TryMap_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = ValidSource(4).TryMap(func: value =>
                                           {
                                               received = value;
                                               return value * 3;
                                           },
                                           errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(12);
        received.Should().Be(4);
    }


    [Fact]
    public void TryMap_WhenFuncThrows_ShouldCaptureExceptionAsFail()
    {
        Func<int, int> throwingFunc = _ => throw new InvalidOperationException("inner");

        var result = ValidSource().TryMap(func               : throwingFunc,
                                          errorMessageBuilder: ex => $"{ErrorMsg}:{ex.Message}");

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task TryMapAsync_MlResultSource_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = await ValidSource(9).TryMapAsync(funcAsync: value =>
                                                      {
                                                          received = value;
                                                          return Task.FromResult(value * 2);
                                                      },
                                                      errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(18);
        received.Should().Be(9);
    }


    [Fact]
    public async Task TryMapAsync_TaskSource_ShouldAwaitSourceOnlyOnce()
    {
        var counter = new CountingSource();
        var source  = counter.Create(ValidSource(6));

        var result = await source.TryMapAsync(funcAsync          : value => Task.FromResult(value + 1),
                                              errorMessageBuilder: _     => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(7);
        counter.Awaits.Should().Be(1, "la Task fuente solo se debe materializar una vez");
    }


    #endregion


    #region B4 / B5 - Try*IfFailWithValue deben usar el valor casado en la rama valid


    [Fact]
    public void TryBindIfFailWithValue_WhenValid_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = ValidSource(15).TryBindIfFailWithValue<int, string, int>(
                                        funcValid: value =>
                                        {
                                            received = value;
                                            return MlResult<int>.Valid(value);
                                        },
                                        funcFail : _ => MlResult<int>.Valid(-1),
                                        errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        received.Should().Be(15, "debe recibir el valor casado por el Match, no source.Value");
    }


    [Fact]
    public async Task TryBindIfFailWithValueAsync_WhenValid_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = await ValidSource(16).TryBindIfFailWithValueAsync<int, string, int>(
                                              funcValidAsync: value =>
                                              {
                                                  received = value;
                                                  return MlResult<int>.Valid(value).ToAsync();
                                              },
                                              funcFailAsync : _ => MlResult<int>.Valid(-1).ToAsync(),
                                              errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        received.Should().Be(16);
    }


    [Fact]
    public void TryMapIfFailWithValue_WhenValid_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = ValidSource(17).TryMapIfFailWithValue<int, string, int>(
                                        funcValid: value =>
                                        {
                                            received = value;
                                            return value;
                                        },
                                        funcFail : _ => -1,
                                        errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        received.Should().Be(17, "debe recibir el valor casado por el Match, no source.Value");
    }


    [Fact]
    public async Task TryMapIfFailWithValueAsync_WhenValid_ShouldReceiveMatchedValue()
    {
        int? received = null;

        var result = await ValidSource(18).TryMapIfFailWithValueAsync<int, string, int>(
                                              funcValidAsync: value =>
                                              {
                                                  received = value;
                                                  return Task.FromResult(value);
                                              },
                                              funcFailAsync : _ => Task.FromResult(-1),
                                              errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        received.Should().Be(18);
    }


    #endregion


    #region D7 - BindAlwaysAsync(Task source) debe observar la Task fuente


    [Fact]
    public async Task BindAlwaysAsync_TaskSource_ShouldAwaitSource()
    {
        var counter = new CountingSource();
        var source  = counter.Create(ValidSource());

        var result = await source.BindAlwaysAsync(funcAlwaysAsync: () => MlResult<string>.Valid("ok").ToAsync());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("ok");
        counter.Awaits.Should().Be(1, "la Task fuente debe observarse (antes quedaba sin await)");
    }


    [Fact]
    public async Task BindAlwaysAsync_TaskSource_WhenSourceFails_ShouldStillRunFunc()
    {
        var result = await FailSource().ToAsync()
                                       .BindAlwaysAsync(funcAlwaysAsync: () => MlResult<string>.Valid("always").ToAsync());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("always");
    }


    #endregion


    #region D7 - TryBindAlwaysAsync ya no se auto-invoca (antes: StackOverflowException)


    [Fact]
    public async Task TryBindAlwaysAsync_WithBuilder_ShouldReturnFuncResult()
    {
        var result = await ValidSource().TryBindAlwaysAsync(funcAlwaysAsync    : () => MlResult<string>.Valid("done").ToAsync(),
                                                            errorMessageBuilder: _  => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("done");
    }


    [Fact]
    public async Task TryBindAlwaysAsync_WithBuilder_WhenFuncThrows_ShouldCaptureException()
    {
        Func<Task<MlResult<string>>> throwingFunc = () => throw new InvalidOperationException("inner");

        var result = await ValidSource().TryBindAlwaysAsync(funcAlwaysAsync    : throwingFunc,
                                                            errorMessageBuilder: ex => $"{ErrorMsg}:{ex.Message}");

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task TryBindAlwaysAsync_WithErrorMessage_ShouldReturnFuncResult()
    {
        var result = await ValidSource().TryBindAlwaysAsync(funcAlwaysAsync: () => MlResult<string>.Valid("done").ToAsync(),
                                                            errorMessage   : ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("done");
    }


    #endregion


    #region Recursiones eliminadas - MapIfFail / MapAlways sobre Task source


    [Fact]
    public async Task MapIfFailAsync_TaskSource_ShouldNotRecurseInfinitely()
    {
        var result = await FailSource().ToAsync()
                                       .MapIfFailAsync(funcAsync: _ => Task.FromResult(-1));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(-1);
    }


    [Fact]
    public async Task MapIfFailAsync_TaskSource_WhenValid_ShouldKeepValue()
    {
        var result = await ValidSource(88).ToAsync()
                                          .MapIfFailAsync(funcAsync: _ => Task.FromResult(-1));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(88);
    }


    [Fact]
    public async Task MapAlwaysAsync_TaskSource_ShouldNotRecurseInfinitely()
    {
        var result = await FailSource().ToAsync()
                                       .MapAlwaysAsync(funcAlwaysAsync: () => Task.FromResult("always"));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("always");
    }


    #endregion


    #region C3 - TryBindIfFailWithoutExceptionAsync debe respetar el filtro TException


    [Fact]
    public async Task TryBindIfFailWithoutExceptionAsync_SyncValidFunc_ShouldRespectTExceptionFilter()
    {
        // El fallo SI contiene una InvalidOperationException => la rama fail NO debe ejecutarse.
        var failWithException = FailSourceWithException(new InvalidOperationException("tracked"));
        var failExecuted      = false;

        var result = await failWithException.ToAsync()
                                            .TryBindIfFailWithoutExceptionAsync<int, int, InvalidOperationException>(
                                                funcValid    : value => MlResult<int>.Valid(value),
                                                funcFailAsync: _ =>
                                                {
                                                    failExecuted = true;
                                                    return MlResult<int>.Valid(-1).ToAsync();
                                                },
                                                errorMessageBuilder: _ => ErrorMsg);

        result.IsFail.Should().BeTrue("el filtro TException debe impedir ejecutar funcFail");
        failExecuted.Should().BeFalse("antes del arreglo se perdia el filtro TException y se ejecutaba");
    }


    [Fact]
    public async Task TryBindIfFailWithoutExceptionAsync_SyncValidFunc_WhenNoException_ShouldRunFailFunc()
    {
        // El fallo NO contiene excepcion => la rama fail SI debe ejecutarse.
        var failExecuted = false;

        var result = await FailSource().ToAsync()
                                       .TryBindIfFailWithoutExceptionAsync<int, int, InvalidOperationException>(
                                           funcValid    : value => MlResult<int>.Valid(value),
                                           funcFailAsync: _ =>
                                           {
                                               failExecuted = true;
                                               return MlResult<int>.Valid(-1).ToAsync();
                                           },
                                           errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(-1);
        failExecuted.Should().BeTrue();
    }


    [Fact]
    public async Task TryBindIfFailWithoutExceptionAsync_SyncValidFunc_WhenValid_ShouldRunValidFunc()
    {
        var result = await ValidSource(44).ToAsync()
                                          .TryBindIfFailWithoutExceptionAsync<int, int, InvalidOperationException>(
                                              funcValid    : value => MlResult<int>.Valid(value * 2),
                                              funcFailAsync: _     => MlResult<int>.Valid(-1).ToAsync(),
                                              errorMessageBuilder: _ => ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(88);
    }


    #endregion


    #region D9 - TryBindIfFailWithoutExceptionAsync (antes BindExIfFailWithoutExceptionAsync)


    [Fact]
    public async Task TryBindIfFailWithoutExceptionAsync_TaskSource_WithErrorMessage_ShouldWork()
    {
        var result = await FailSource().ToAsync()
                                       .TryBindIfFailWithoutExceptionAsync<int, InvalidOperationException>(
                                           funcAsync   : _ => MlResult<int>.Valid(-5).ToAsync(),
                                           errorMessage: ErrorMsg);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(-5);
    }


    #endregion
}
