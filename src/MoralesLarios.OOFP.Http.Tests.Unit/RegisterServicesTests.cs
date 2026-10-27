// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace MoralesLarios.OOFP.Http.Tests.Unit;

public class RegisterServicesTests
{
    private interface ITestClient { }

    private sealed class TestClient : RestClientFp<string>, ITestClient
    {
        public static ServiceKey? CapturedKey { get; private set; }

        public TestClient(ILogger<RestClientFp<string>> logger, IHttpApiInvoker httpClientFactoryManager, ServiceKey httpClientFactoryKey)
            : base(logger, httpClientFactoryManager, httpClientFactoryKey)
        {
            CapturedKey = httpClientFactoryKey;
        }
    }

    private sealed class DuplexService : RestClientFp<string, string>, ITestClient
    {
        public static ServiceKey? CapturedKey { get; private set; }

        public DuplexService(ILogger<RestClientFp<string, string>> logger, IHttpApiInvoker httpClientFactoryManager, ServiceKey httpClientFactoryKey)
            : base(logger, httpClientFactoryManager, httpClientFactoryKey)
        {
            CapturedKey = httpClientFactoryKey;
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
            .WithMessage("httpClientFactoryKey no puede ser null");
    }

    [Fact]
    public void AddCompositeKeyRestClient_duplex_throws_when_key_is_null()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMlHttp();

        Action act = () => services.AddCompositeKeyRestClient<ITestClient, DuplexService, string, string>(() => null!);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("httpClientFactoryKey no puede ser null");
    }
}
