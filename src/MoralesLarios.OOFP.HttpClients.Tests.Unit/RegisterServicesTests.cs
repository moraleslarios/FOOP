// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace MoralesLarios.OOFP.HttpClients.Tests.Unit;

public class RegisterServicesTests
{
    private interface ITestClient { }

    private sealed class TestClient : GenClientFp<string>, ITestClient
    {
        public static Key? CapturedKey { get; private set; }

        public TestClient(ILogger<GenClientFp<string>> logger, IHttpClientFactoryManager httpClientFactoryManager, Key httpClientFactoryKey)
            : base(logger, httpClientFactoryManager, httpClientFactoryKey)
        {
            CapturedKey = httpClientFactoryKey;
        }
    }

    private sealed class DuplexService : GenClientFp<string, string>, ITestClient
    {
        public static Key? CapturedKey { get; private set; }

        public DuplexService(ILogger<GenClientFp<string, string>> logger, IHttpClientFactoryManager httpClientFactoryManager, Key httpClientFactoryKey)
            : base(logger, httpClientFactoryManager, httpClientFactoryKey)
        {
            CapturedKey = httpClientFactoryKey;
        }
    }

    [Fact]
    public void AddGenClientFp_uses_default_key_when_not_configured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClientsFp();

        services.AddGenClientFp<ITestClient, TestClient>();
        var provider = services.BuildServiceProvider();

        _ = provider.GetRequiredService<ITestClient>();

        TestClient.CapturedKey.Should().Be(Key.FromString(nameof(TestClient)));
    }

    [Fact]
    public void AddGenClientFp_uses_custom_key_when_configured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClientsFp();
        var expected = Key.FromString("custom-client");

        services.AddGenClientFp<ITestClient, TestClient>(() => expected);
        var provider = services.BuildServiceProvider();

        _ = provider.GetRequiredService<ITestClient>();

        TestClient.CapturedKey.Should().Be(expected);
    }

    [Fact]
    public void AddGenClientComplexFp_uses_custom_key_when_configured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClientsFp();
        var expected = Key.FromString("complex-client");

        services.AddGenClientComplexFp<ITestClient, TestClient, string>(() => expected);
        var provider = services.BuildServiceProvider();

        _ = provider.GetRequiredService<ITestClient>();

        TestClient.CapturedKey.Should().Be(expected);
    }

    [Fact]
    public void AddGenClientComplexFp_throws_when_key_is_null()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClientsFp();

        Action act = () => services.AddGenClientComplexFp<ITestClient, TestClient, string>(() => null!);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("httpClientFactoryKey no puede ser null");
    }

    [Fact]
    public void AddGenClientDuplexComplexFp_throws_when_key_is_null()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClientsFp();

        Action act = () => services.AddGenClientDuplexComplexFp<ITestClient, DuplexService, string, string>(() => null!);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("httpClientFactoryKey no puede ser null");
    }
}
