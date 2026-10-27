// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Http.Tests.Integration.Data;
using System.Threading.Tasks;
using MoralesLarios.OOFP.Http.Requests;
using System.Formats.Asn1;

namespace MoralesLarios.OOFP.Http.Tests.Integration;

public class UnitTest1(IHttpApiInvoker _sut)
{
    //    [Fact]
    //    public async Task Test1()
    //    {
    //        var data2 = await _sut.PostAndReadAsync<IEnumerable<int>, IEnumerable<CustomerDto>>(
    //            ("ByCustomersKeys", "default", [11000, 11001], default));

    //        //var data2 = await _sut.PostAsync<IEnumerable<int>, IEnumerable<CustomerDto>>(null!);
    //    }

    //    [Fact]
    //    public async Task Test2()
    //    {
    //        var data2 = await _sut.PostAndReadPagedAsync<string, CustomerDto>
    //            (new PagedApiCallOptions<string>("MaritalStatusPagination",
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
    //        Assert.True(result.IsSuccess);
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
    //        Assert.True(result.IsSuccess);
    //    }

    //    [Fact]
    //    public async Task Test6()
    //    {

    //        var result = await _sut.DeleteByIdAsync<CustomerDto>("default", "29487", default);
    //        Assert.True(result.IsSuccess);
    //    }




    [Fact]
    public async Task TestPostError()
    {

        //var data = await _sut.PostAsync<>("default", new CustomerDto
        //{
        //    CustomerAlternateKey = "Pakkko-A",
        //    FirstName = "Pakkko-XX",
        //    LastName = $"Tests - {DateTime.Now.ToString()}"
        //}, null!, default);




    }


    [Fact]
    public async Task TextGetAsync()
    {

        var data = await _sut.GetAsync<IEnumerable<VinoDto>>("vinos-api", "not-found", null!, default);


    }


    [Fact]
    public async Task TextGet2Async()
    {

        var data = await _sut.GetAsync<IEnumerable<VinoDto>>("vinos-api", "not-found", new Dictionary<string, string>
                                                      {
                                                          {  "prueba", "Cambiado"   }
                                                      }, default);


    }




    [Fact]
    public async Task TextBadAsync()
    {
        var item = new VinoDto
        {

        };

        var result = await _sut.PostAsync<VinoDto>("vinos-api", item, "bad", default!);

    }




    //[Fact]
    //public async Task TextBad2Async()
    //{
    //    var result = await _sut.PostAndReadAsync<IEnumerable<int>, IEnumerable<string>>(("bad2", 
    //                                                  "vinos-api", 
    //                                                  [1, 2, 3], 
    //                                                  new Dictionary<string, string>
    //                                                  {
    //                                                      {  "prueba", "Cambiado"   }
    //                                                  }  , default!));

    //}


    [Fact]
    public async Task TextBad2_2Async()
    {
        var result = await _sut.PostAndReadAsync<IEnumerable<int>, IEnumerable<string>>(
                                                      "vinos-api",
                                                      [1, 2, 3],
                                                      "bad2",
                                                      new Dictionary<string, string>
                                                      {
                                                          {  "prueba", "Cambiado"   }
                                                      }, default!);

    }



    [Fact]
    public async Task Find_NotFound()
    {
        var result = await _sut.GetAsync<VinoDto>("vinos-api", "2", null!, default);



    }




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
    //        { "nombre", "My MemberName" }, 
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
