// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.Reflection;
using MoralesLarios.OOFP.Http.Helpers;
using MoralesLarios.OOFP.Types;
using RichardSzalay.MockHttp;

namespace MoralesLarios.OOFP.Http.Tests.Unit.Helpers;

public class MlHttpRequestExtensionsTests
{

    private readonly MockHttpMessageHandler mockHttp = new();
    private readonly HttpClient client;

    public MlHttpRequestExtensionsTests()
    {
        client = new HttpClient(mockHttp);
    }


    [Fact]
    public async Task AddPaginationHeaders_HttpRequestMessage_KeepsConsecutiveRequestsIsolated()
    {
        mockHttp.Expect(HttpMethod.Get, "https://api.ejemplo.com/first")
                .WithHeaders("X-Page-Number", "123")
                .WithHeaders("X-Page-Size", "10")
                .Respond(HttpStatusCode.OK);
        mockHttp.Expect(HttpMethod.Get, "https://api.ejemplo.com/second")
                .WithHeaders("X-Page-Number", "456")
                .WithHeaders("X-Page-Size", "20")
                .Respond(HttpStatusCode.OK);

        using var first = new HttpRequestMessage(HttpMethod.Get, "https://api.ejemplo.com/first");
        using var second = new HttpRequestMessage(HttpMethod.Get, "https://api.ejemplo.com/second");
        var firstAdded = first.AddPaginationHeaders(
            IntNonNegative.CreateFromInt(123), IntNonNegative.CreateFromInt(10));
        var secondAdded = second.AddPaginationHeaders(
            IntNonNegative.CreateFromInt(456), IntNonNegative.CreateFromInt(20));
        using var firstResponse = await client.SendAsync(first);
        using var secondResponse = await client.SendAsync(second);

        firstAdded.IsSuccess.Should().BeTrue();
        secondAdded.IsSuccess.Should().BeTrue();
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        client.DefaultRequestHeaders.Contains("X-Page-Number").Should().BeFalse();
        client.DefaultRequestHeaders.Contains("X-Page-Size").Should().BeFalse();
        mockHttp.VerifyNoOutstandingExpectation();
    }


    [Fact]
    public void HttpClientHeaderExtensions_AreObsoleteAndHavePerRequestAlternatives()
    {
        var clientMethods = typeof(MlHttpRequestExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name.StartsWith("Add", StringComparison.Ordinal) &&
                             method.GetParameters()[0].ParameterType == typeof(HttpClient))
            .ToArray();

        clientMethods.Should().HaveCount(10);

        foreach (var method in clientMethods)
        {
            var obsolete = method.GetCustomAttribute<ObsoleteAttribute>();
            obsolete.Should().NotBeNull();
            obsolete!.Message.Should().Contain("HttpRequestMessage");

            typeof(MlHttpRequestExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Should().Contain(candidate =>
                    candidate.Name == method.Name &&
                    candidate.GetParameters()[0].ParameterType == typeof(HttpRequestMessage));
        }
    }
}

