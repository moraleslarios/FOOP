// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers;

public class HttpRequestPaginationExtensionsTests
{

    


    [Fact]
    public void GetHeaderInfo_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeaderInfo("X-Custom-Header");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderInfo_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "HeaderValue" }
        });

        MlResult<NotEmptyString> result = request.GetHeaderInfo("X-Custom-Header");

        MlResult<NotEmptyString> expected = NotEmptyString.CreateFromString("HeaderValue");

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

        result.IsFailure.Should().BeTrue();
    }


    [Fact]
    public void GetHeaderInfoAsInt_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "123" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderInfoAsIntNotNegative("X-Custom-Header");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderInfoAsInt_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Custom-Header", "123" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderInfoAsIntNotNegative("X-Custom-Header");

        MlResult<IntNonNegative> expected = IntNonNegative.CreateFromInt(123);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderInfoAsInt_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "123" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderInfoAsIntNotNegative("X-Custom-Header");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageNumber_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderPageNumber();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageNumber_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderPageNumber();

        MlResult<IntNonNegative> expected = IntNonNegative.CreateFromInt(1);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderPageNumber_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "1" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderPageNumber();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageSize_withExistHeader_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Size", "10" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderPageSize();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageSize_withExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Size", "10" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderPageSize();

        MlResult<IntNonNegative> expected = IntNonNegative.CreateFromInt(10);

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void GetHeaderPageSize_withNotExistHeader_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "XXX", "10" }
        });

        MlResult<IntNonNegative> result = request.GetHeaderPageSize();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageInfo_withExistHeaders_returnValid()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" },
            { "X-Page-Size", "10" }
        });

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> result = request.GetHeaderPageInfo();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetHeaderPageInfo_withExistHeaders_returnValidHeaderValue()
    {
        var request = BuildHttpRequest(new Dictionary<string, string>
        {
            { "X-Page-Number", "1" },
            { "X-Page-Size", "10" }
        });

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> result = request.GetHeaderPageInfo();

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> expected = (1, 10);

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

        MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> result = request.GetHeaderPageInfo();

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

