using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using MoralesLarios.OOFP.Types;
using Moq;
using RichardSzalay.MockHttp;

namespace MoralesLarios.OOFP.Http.Tests.Unit;

public class RestClientPathEncodingTests
{
    private interface ITestClient
    {
    }

    private sealed class TestClient(IHttpApiInvoker apiInvoker, ServiceKey clientName)
        : RestClientFp<string>(apiInvoker, clientName), ITestClient;

    private sealed class OverridingTestClient(IHttpApiInvoker apiInvoker, ServiceKey clientName)
        : RestClientFp<string>(apiInvoker, clientName)
    {
        public string? CapturedId { get; private set; }

        public override Task<MlResult<string>> GetByIdAsync(
            NotEmptyString idStr,
            Dictionary<string, string> headers = null!,
            CancellationToken ct = default)
        {
            CapturedId = idStr.ToString();
            return Task.FromResult(MlResult<string>.Valid("respuesta"));
        }
    }

    [Theory]
    [InlineData("uno/dos", "uno%252Fdos")]
    [InlineData("a?b", "a%253Fb")]
    [InlineData("a#b", "a%2523b")]
    [InlineData("a b", "a%2520b")]
    [InlineData("a%b", "a%2525b")]
    [InlineData("a+b", "a%252Bb")]
    [InlineData("a,b", "a%252Cb")]
    [InlineData("café", "caf%25C3%25A9")]
    public async Task GetByIdAsync_EncodesReservedIdentifierCharacters(string identifier, string encodedSegment)
    {
        var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, $"https://api.ejemplo.com/id-str/{encodedSegment}")
               .Respond("application/json", "\"respuesta\"");
        using var provider = CreateHttpProvider(handler);
        var client = (TestClient)provider.GetRequiredService<ITestClient>();

        var result = await client.GetByIdAsync(NotEmptyString.CreateFromString(identifier));

        Assert.True(result.IsSuccess);
        handler.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPutAndDeleteByIdAsync_UseOneEncodedPathSegment()
    {
        const string encodedSegment = "uno%252Fdos";
        var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, $"https://api.ejemplo.com/id-str/{encodedSegment}")
               .Respond("application/json", "\"respuesta\"");
        handler.Expect(HttpMethod.Put, $"https://api.ejemplo.com/{encodedSegment}")
               .Respond(HttpStatusCode.NoContent);
        handler.Expect(HttpMethod.Delete, $"https://api.ejemplo.com/{encodedSegment}")
               .Respond(HttpStatusCode.NoContent);
        using var provider = CreateHttpProvider(handler);
        var client = (TestClient)provider.GetRequiredService<ITestClient>();
        var identifier = NotEmptyString.CreateFromString("uno/dos");

        var getResult = await client.GetByIdAsync(identifier);
        var putResult = await client.PutByIdAsync(identifier, "request");
        var deleteResult = await client.DeleteByIdAsync(identifier);

        Assert.True(getResult.IsSuccess);
        Assert.True(putResult.IsSuccess);
        Assert.True(deleteResult.IsSuccess);
        handler.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CompositeKeyClient_EncodesComponentsAndPreservesCommaSeparatorForAllVerbs()
    {
        const string encodedCompositeKey = "ACME%252CSL%2Cwest%252Feast%2C42";
        var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, $"https://api.ejemplo.com/id-str/{encodedCompositeKey}")
               .Respond("application/json", "\"respuesta\"");
        handler.Expect(HttpMethod.Put, $"https://api.ejemplo.com/{encodedCompositeKey}")
               .Respond(HttpStatusCode.NoContent);
        handler.Expect(HttpMethod.Delete, $"https://api.ejemplo.com/{encodedCompositeKey}")
               .Respond(HttpStatusCode.NoContent);
        using var provider = CreateHttpProvider(handler);
        var client = new RestClientFp<string>(
            provider.GetRequiredService<IHttpApiInvoker>(),
            ServiceKey.CreateFromString("path-encoding-test"));
        var compositeClient = new CompositeKeyRestClientFp<string>(client);
        object[] keyValues = ["ACME,SL", "west/east", 42];

        var getResult = await compositeClient.GetByIdAsync(keyValues);
        var putResult = await compositeClient.PutByIdAsync(keyValues, "request");
        var deleteResult = await compositeClient.DeleteByIdAsync(keyValues);

        Assert.True(getResult.IsSuccess);
        Assert.True(putResult.IsSuccess);
        Assert.True(deleteResult.IsSuccess);
        handler.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CompositeKeyClient_PreservesAnEmptyComponentBoundary()
    {
        var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://api.ejemplo.com/id-str/%2C42")
               .Respond("application/json", "\"respuesta\"");
        using var provider = CreateHttpProvider(handler);
        var client = new RestClientFp<string>(
            provider.GetRequiredService<IHttpApiInvoker>(),
            ServiceKey.CreateFromString("path-encoding-test"));
        var compositeClient = new CompositeKeyRestClientFp<string>(client);

        var result = await compositeClient.GetByIdAsync(new object[] { string.Empty, 42 });

        Assert.True(result.IsSuccess);
        handler.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CompositeKeyClient_PreservesOverriddenRestClientMethods()
    {
        var client = new OverridingTestClient(
            Mock.Of<IHttpApiInvoker>(),
            ServiceKey.CreateFromString("override-test"));
        var compositeClient = new CompositeKeyRestClientFp<string>(client);

        var result = await compositeClient.GetByIdAsync(new object[] { "ACME,SL", 42 });

        Assert.True(result.IsSuccess);
        Assert.Equal("ACME,SL,42", client.CapturedId);
    }

    private static ServiceProvider CreateHttpProvider(HttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpMessageHandlerBuilderFilter>(new TestMessageHandlerBuilderFilter(handler));
        services.AddMlHttp();
        services.AddCompositeKeyRestClient<ITestClient, TestClient, string>(
            () => ServiceKey.CreateFromString("path-encoding-test"),
            client => client.BaseAddress = new Uri("https://api.ejemplo.com/"));
        return services.BuildServiceProvider();
    }

    private sealed class TestMessageHandlerBuilderFilter(HttpMessageHandler handler)
        : IHttpMessageHandlerBuilderFilter
    {
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
            => builder =>
            {
                next(builder);
                builder.PrimaryHandler = handler;
            };
    }
}
