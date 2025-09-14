using MoralesLarios.OOFP.HttpClients.Tests.Integration.Data;
using System.Threading.Tasks;
using MoralesLarios.OOFP.HttpClients.ParamsInfo;

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration;

public class UnitTest1(IHttpClientFactoryManager _sut)
{
//    [Fact]
//    public async Task Test1()
//    {
//        var data2 = await _sut.PostGetAsync<IEnumerable<int>, IEnumerable<CustomerDto>>(
//            ("ByCustomersKeys", "default", [11000, 11001], default));

//        //var data2 = await _sut.PostAsync<IEnumerable<int>, IEnumerable<CustomerDto>>(null!);
//    }

//    [Fact]
//    public async Task Test2()
//    {
//        var data2 = await _sut.PostGetPaginationAsync<string, CustomerDto>
//            (new CallRequestPaginationParamsInfo<string>("MaritalStatusPagination",
//                                                         "default",
//                                                         "M",
//                                                         1,
//                                                         20,
//                                                         default));
            

//        //var data2 = await _sut.PostAsync<IEnumerable<int>, IEnumerable<CustomerDto>>(null!);
//    }


//    [Fact]
//    public async Task Test3()
//    {
//        var item = new CustomerDto
//        {
//            CustomerAlternateKey = "Pakkko-A",
//            FirstName = "Pakkko-XX",
//            LastName = $"Tests - {DateTime.Now.ToString()}"
//        };


//        var result = await _sut.PostAsync<CustomerDto>("default", item, null!, default);
//    }

//    [Fact]
//    public async Task Test4()
//    {
//        var item = new CustomerDto
//        {
//            CustomerKey = 29489,
//            CustomerAlternateKey = "Pakkko-A",
//            FirstName = "Pakkko-XXY",
//            LastName = $"Tests - {DateTime.Now.ToString()}"
//        };
//        var result = await _sut.PutAsync<CustomerDto>("default", item, null!, default);
//        Assert.True(result.IsValid);
//    }


//    [Fact]
//    public async Task Test5()
//    {
//        var item = new CustomerDto
//        {
//            CustomerKey = 29489,
//            CustomerAlternateKey = "Pakkko-A",
//            FirstName = "Pakkko-XXY",
//            LastName = $"Tests - {DateTime.Now.ToString()}"
//        };
//        var result = await _sut.DeleteAsync<CustomerDto>("default", item, null!, default);
//        Assert.True(result.IsValid);
//    }

//    [Fact]
//    public async Task Test6()
//    {

//        var result = await _sut.DeleteByIdAsync<CustomerDto>("default", "29487", default);
//        Assert.True(result.IsValid);
//    }
}