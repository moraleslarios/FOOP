// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Microsoft.Extensions.Http;
using MoralesLarios.OOFP.Types;
using RichardSzalay.MockHttp;

namespace MoralesLarios.OOFP.Http.Tests.Unit;

public class RegisterServicesTests
{
    private interface ITestClient { }

    private sealed class TestClient : RestClientFp<string>, ITestClient
    {
        public static ServiceKey? CapturedKey { get; private set; }

        public TestClient(IHttpApiInvoker apiInvoker, ServiceKey clientName)
            : base(apiInvoker, clientName)
        {
            CapturedKey = clientName;
        }
    }

    private sealed class DuplexService : RestClientFp<string, string>, ITestClient
    {
        public static ServiceKey? CapturedKey { get; private set; }

        public DuplexService(IHttpApiInvoker apiInvoker, ServiceKey clientName)
            : base(apiInvoker, clientName)
        {
            CapturedKey = clientName;
        }
    }

    [Fact]
    public void AddRestClient_uses_default_key_when_not_configured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMlHttp();

        services.AddRestClient<ITestClient, TestClient>();
        var provider = services.BuildServiceProvider();

        _ = provider.GetRequiredService<ITestClient>();

        TestClient.CapturedKey.Should().Be(ServiceKey.CreateFromString(nameof(TestClient)));
    }

    [Fact]
    public void AddRestClient_uses_custom_key_when_configured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMlHttp();
        var expected = ServiceKey.CreateFromString("custom-client");

        services.AddRestClient<ITestClient, TestClient>(() => expected);
        var provider = services.BuildServiceProvider();

        _ = provider.GetRequiredService<ITestClient>();

        TestClient.CapturedKey.Should().Be(expected);
    }

    [Fact]
    public void AddCompositeKeyRestClient_uses_custom_key_when_configured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMlHttp();
        var expected = ServiceKey.CreateFromString("complex-client");

        services.AddCompositeKeyRestClient<ITestClient, TestClient, string>(() => expected);
        var provider = services.BuildServiceProvider();

        _ = provider.GetRequiredService<ITestClient>();

        TestClient.CapturedKey.Should().Be(expected);
    }

    [Fact]
    public void AddCompositeKeyRestClient_throws_when_key_is_null()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMlHttp();

        Action act = () => services.AddCompositeKeyRestClient<ITestClient, TestClient, string>(() => null!);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("clientName no puede ser null");
    }

    [Fact]
    public void AddCompositeKeyRestClient_duplex_throws_when_key_is_null()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMlHttp();

        Action act = () => services.AddCompositeKeyRestClient<ITestClient, DuplexService, string, string>(() => null!);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("clientName no puede ser null");
    }


    [Fact]
    public async Task AddCompositeKeyRestClient_GetByIdAsync_SendsRequestThroughConfiguredClient()
    {
        var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://api.ejemplo.com/id-str/42")
               .Respond("application/json", "\"respuesta\"");
        using var provider = CreateHttpProvider(handler);

         var client = (TestClient)provider.GetRequiredService<ITestClient>();
         var result = await client.GetByIdAsync(NotEmptyString.CreateFromString("42"));

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be("respuesta");
        handler.VerifyNoOutstandingExpectation();
    }


    [Fact]
    public async Task GetByIdAsync_IdentifierWithSlash_UsesOneEncodedPathSegment()
    {
        var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://api.ejemplo.com/id-str/uno%252Fdos")
               .Respond("application/json", "\"respuesta\"");
        using var provider = CreateHttpProvider(handler);

         var client = (TestClient)provider.GetRequiredService<ITestClient>();
         var result = await client.GetByIdAsync(NotEmptyString.CreateFromString("uno/dos"));

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be("respuesta");
        handler.VerifyNoOutstandingExpectation();
    }


    private static ServiceProvider CreateHttpProvider(MockHttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpMessageHandlerBuilderFilter>(
            new TestMessageHandlerBuilderFilter(handler));
        services.AddMlHttp();
        services.AddCompositeKeyRestClient<ITestClient, TestClient, string>(
            () => ServiceKey.CreateFromString("request-test"),
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
