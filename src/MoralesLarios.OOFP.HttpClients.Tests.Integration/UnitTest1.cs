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












    //[Fact]
    //public async Task Test1()
    //{
    //    var data2 = await _sut.GetAsync<IEnumerable<PruebasDto>>("pruebas-api");

    //    //var data2 = await _sut.PostAsync<IEnumerable<int>, IEnumerable<CustomerDto>>(null!);
    //}


    //[Fact]
    //public async Task Test2()
    //{
    //    Dictionary<string, string> headers = new()
    //    {
    //        { "data", "my-data" }
    //    };

    //    var headers2 = new Dictionary<string, string> { { "data", "my-data" } };

    //    var data2 = await _sut.GetAsync<PruebasDto>("pruebas-api", "with-header", headers );

    //    //var data2 = await _sut.PostAsync<IEnumerable<int>, IEnumerable<CustomerDto>>(null!);
    //}


    //[Fact]
    //public async Task Test21()
    //{

    //    var item = new PruebasDto
    //    {
    //        Nombre = "Uno más",
    //        Comentarios = "--- - Descriptión"
    //    };

    //    var result = await _sut.PostAsync<PruebasDto>("pruebas-api", item, null!, null!, default);

    //}


    //[Fact]
    //public async Task Test3()    {
    //    Dictionary<string, string> headers = new()
    //    {
    //        { "nombre", "My Name" }, 
    //        { "comentarios", "Comentarios" }
    //    };

    //    var item = new PruebasDto
    //    {
    //        Nombre = "Miiiiooo",
    //        Comentarios = "****"
    //    };

    //    var result = await _sut.PostAsync<PruebasDto>("pruebas-api", item, "with-header", headers, default);

    //}



    //[Fact]
    //public async Task Test4()
    //{

    //    var item = new PruebasDto
    //    {
    //        Id = 1,
    //        Nombre = "Hola",
    //        Comentarios = "Soy un chico simpático"
    //    };

    //    var result = await _sut.PutAsync<PruebasDto>("pruebas-api", item, null!, null!, default);

    //}

    //[Fact]
    //public async Task Test5()
    //{
    //    Dictionary<string, string> headers = new()
    //    {
    //        { "nombre", "Cambiado" },
    //        { "comentarios", "Cambiado del todo" }
    //    };

    //    var item = new PruebasDto
    //    {
    //        Id = 4,
    //        Nombre = "Pruebas - X",
    //        Comentarios = "Description - X"
    //    };

    //    var result = await _sut.PutAsync<PruebasDto>("pruebas-api", item, "with-header", headers, default);

    //}


    //[Fact]
    //public async Task Test6()
    //{
    //    var item = new PruebasDto
    //    {
    //        Id = 1,
    //        Nombre = "Hola",
    //        Comentarios = "Soy un chico simpático"
    //    };

    //    var result = await _sut.DeleteAsync<PruebasDto>("pruebas-api", item, null!, null!, default);
    //}

    //[Fact]
    //public async Task Test7()
    //{
    //    var result = await _sut.DeleteByIdAsync<PruebasDto>("pruebas-api",  "4", null!, default);
    //}

    //[Fact]
    //public async Task Test8()
    //{
    //    Dictionary<string, string> headers = new()
    //    {
    //        { "prueba", "Cambiado" }
    //    };

    //    var item = new PruebasDto
    //    {
    //        Id = 1,
    //        Nombre = "Hola",
    //        Comentarios = "Soy un chico simpático"
    //    };

    //    var result = await _sut.DeleteAsync<PruebasDto>("pruebas-api", item, "with-header", headers, default);
    //}

    //[Fact]
    //public async Task Test9()
    //{
    //    Dictionary<string, string> headers = new()
    //    {
    //        { "prueba", "Cambiado by id" }
    //    };

    //    var result = await _sut.DeleteByIdAsync<PruebasDto>("pruebas-api", "with-header/8", headers, default);

    //}


}