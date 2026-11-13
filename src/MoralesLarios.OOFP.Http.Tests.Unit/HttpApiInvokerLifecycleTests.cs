using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using MoralesLarios.OOFP.Http.Requests;
using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Http.Tests.Unit;

public class HttpApiInvokerLifecycleTests
{
    [Fact]
    public async Task InternalHttpActionAsync_DisposesRequestsAndResponsesAcrossRepeatedSuccessAndFailure()
    {
        var disposedRequests = 0;
        var disposedResponses = 0;
        var handler = new TrackingHandler(index => new HttpResponseMessage(
            index % 2 == 0 ? HttpStatusCode.OK : HttpStatusCode.BadGateway)
        {
            Content = new TrackingContent(
                index % 2 == 0 ? "\"ok\"" : "remote error",
                () => Interlocked.Increment(ref disposedResponses))
        });
        using var client = new HttpClient(handler);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(client);
        var invoker = new TestHttpApiInvoker(clientFactory.Object);

        for (var index = 0; index < 1000; index++)
        {
            var request = new TrackingRequestMessage(
                new Uri("https://example.test/items"),
                () => Interlocked.Increment(ref disposedRequests));

            var result = await invoker.Send(request);

            result.IsSuccess.Should().Be(index % 2 == 0);
        }

        disposedRequests.Should().Be(1000);
        disposedResponses.Should().Be(1000);
    }

    [Fact]
    public async Task SendAndReadAsync_SerializesRequestAndDisposesResponse()
    {
        var disposedResponses = 0;
        var handler = new TrackingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new TrackingContent("\"response\"", () => Interlocked.Increment(ref disposedResponses))
        });
        using var client = new HttpClient(handler);
        var invoker = new TestHttpApiInvoker(Mock.Of<IHttpClientFactory>());
        var options = new ApiCallOptions<string>(
            "https://example.test/items",
            ServiceKey.CreateFromString("lifecycle-test"),
            "request");

        var result = await invoker.SendAndRead(options, client);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be("response");
        disposedResponses.Should().Be(1);
    }

    [Fact]
    public async Task ActionEmptyAsync_DisposesSuccessfulAndFailedResponses()
    {
        var disposedResponses = 0;
        var handler = new TrackingHandler(index => new HttpResponseMessage(
            index == 0 ? HttpStatusCode.NoContent : HttpStatusCode.BadGateway)
        {
            Content = new TrackingContent("remote error", () => Interlocked.Increment(ref disposedResponses))
        });
        using var client = new HttpClient(handler);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(client);
        var invoker = new TestHttpApiInvoker(clientFactory.Object);

        var success = await invoker.Delete(new HttpRequestMessage(HttpMethod.Delete, "https://example.test/items"));
        var failure = await invoker.Delete(new HttpRequestMessage(HttpMethod.Delete, "https://example.test/items"));

        success.IsSuccess.Should().BeTrue();
        failure.IsFailure.Should().BeTrue();
        disposedResponses.Should().Be(2);
    }

    private sealed class TestHttpApiInvoker(IHttpClientFactory clientFactory)
        : HttpApiInvoker(clientFactory, NullLogger<HttpApiInvoker>.Instance)
    {
        public Task<MlResult<string?>> Send(HttpRequestMessage request)
            => InternalHttpActionAsync<HttpRequestMessage, string>(
                ServiceKey.CreateFromString("lifecycle-test"),
                request,
                request.RequestUri!.ToString());

        public Task<MlResult<string?>> SendAndRead(ApiCallOptions<string> options, HttpClient client)
            => SendAndReadAsync<string, string>(options, client, MemberName.CreateFromString("lifecycle"));

        public Task<MlResult<EmptyValue>> Delete(HttpRequestMessage request)
            => ActionEmptyAsync<string>(
                ServiceKey.CreateFromString("lifecycle-test"),
                "Delete",
                request,
                request.RequestUri!.ToString());
    }

    private sealed class TrackingHandler(Func<int, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        private int _calls;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(responseFactory(Interlocked.Increment(ref _calls) - 1));
    }

    private sealed class TrackingContent(string content, Action onDispose)
        : ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content))
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing) onDispose();
            base.Dispose(disposing);
        }
    }

    private sealed class TrackingRequestMessage(Uri requestUri, Action onDispose)
        : HttpRequestMessage(HttpMethod.Get, requestUri)
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing) onDispose();
            base.Dispose(disposing);
        }
    }
}