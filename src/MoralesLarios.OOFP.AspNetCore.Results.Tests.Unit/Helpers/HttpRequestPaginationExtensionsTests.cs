// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers;

public class HttpRequestPaginationExtensionsTests
{

    


    [Fact]
    public void GetHeader_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeader("X-Custom-Header");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetHeader_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeader("X-Custom-Header");

        MlResult<NotEmptyString> expected = NotEmptyString.CreateFromString("HeaderValue");

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeader_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeader("X-Custom-Header");

        result.IsFailure.Should().BeTrue();
    }


    [Fact]
    public void GetIntHeader_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "123" }
        });

        MlResult<IntNonNegative> result = request.GetIntHeader("X-Custom-Header");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetIntHeader_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "123" }
        });

        MlResult<IntNonNegative> result = request.GetIntHeader("X-Custom-Header");

        MlResult<IntNonNegative> expected = IntNonNegative.CreateFromInt(123);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetIntHeader_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "123" }
        });

        MlResult<IntNonNegative> result = request.GetIntHeader("X-Custom-Header");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetPageNumberHeader_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" }
        });

        MlResult<IntNonNegative> result = request.GetPageNumberHeader();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetPageNumberHeader_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" }
        });

        MlResult<IntNonNegative> result = request.GetPageNumberHeader();

        MlResult<IntNonNegative> expected = IntNonNegative.CreateFromInt(1);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetPageNumberHeader_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "1" }
        });

        MlResult<IntNonNegative> result = request.GetPageNumberHeader();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetPageSizeHeader_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Size", "10" }
        });

        MlResult<IntNonNegative> result = request.GetPageSizeHeader();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetPageSizeHeader_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Size", "10" }
        });

        MlResult<IntNonNegative> result = request.GetPageSizeHeader();

        MlResult<IntNonNegative> expected = IntNonNegative.CreateFromInt(10);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetPageSizeHeader_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "10" }
        });

        MlResult<IntNonNegative> result = request.GetPageSizeHeader();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetPaginationHeaders_withExistHeaders_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" },
            { "X-Page-Size", "10" }
        });

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> result = request.GetPaginationHeaders();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetPaginationHeaders_withExistHeaders_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" },
            { "X-Page-Size", "10" }
        });

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> result = request.GetPaginationHeaders();

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> expected = (1, 10);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetPaginationHeaders_withNotExistHeaders_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "1" },
            { "YYY", "10" }
        });

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> result = request.GetPaginationHeaders();

        result.IsFailure.Should().BeTrue();
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

