

namespace MoralesLarios.OOFP.WebApi.Tests.Unit.Helpers;

public class MlRequestWebExtensionsTests
{

    


    [Fact]
    public void GetHeaderInfo_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeaderInfo("X-Custom-Header");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderInfo_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeaderInfo("X-Custom-Header");

        MlResult<NotEmptyString> expected = NotEmptyString.FromString("HeaderValue");

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderInfo_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeaderInfo("X-Custom-Header");

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void GetHeaderInfoAsInt_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "123" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderInfoAsIntNotNegative("X-Custom-Header");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderInfoAsInt_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "123" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderInfoAsIntNotNegative("X-Custom-Header");

        MlResult<IntNotNegative> expected = IntNotNegative.FromInt(123);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderInfoAsInt_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "123" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderInfoAsIntNotNegative("X-Custom-Header");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageNumber_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderPageNumber();

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageNumber_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderPageNumber();

        MlResult<IntNotNegative> expected = IntNotNegative.FromInt(1);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderPageNumber_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "1" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderPageNumber();

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageSize_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Size", "10" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderPageSize();

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageSize_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Size", "10" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderPageSize();

        MlResult<IntNotNegative> expected = IntNotNegative.FromInt(10);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderPageSize_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "10" }
        });

        MlResult<IntNotNegative> result = request.GetHeaderPageSize();

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageInfo_withExistHeaders_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" },
            { "X-Page-Size", "10" }
        });

        MlResult<(IntNotNegative PageNumber, IntNotNegative PageSize)> result = request.GetHeaderPageInfo();

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageInfo_withExistHeaders_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" },
            { "X-Page-Size", "10" }
        });

        MlResult<(IntNotNegative PageNumber, IntNotNegative PageSize)> result = request.GetHeaderPageInfo();

        MlResult<(IntNotNegative PageNumber, IntNotNegative PageSize)> expected = (1, 10);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderPageInfo_withNotExistHeaders_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "1" },
            { "YYY", "10" }
        });

        MlResult<(IntNotNegative PageNumber, IntNotNegative PageSize)> result = request.GetHeaderPageInfo();

        result.IsFail.Should().BeTrue();
    }




    private HttpRequest BuildHttpRequest(Dictionary<string, string> headers)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;
        foreach (var header in headers)
        {
            request.Headers[header.Key] = header.Value;
        }
        return request;
    }



}
