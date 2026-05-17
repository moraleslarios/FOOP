// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Globalization;

namespace MoralesLarios.OOFP.WebServices.Tests.Unit;

public class UnitTest1
{
    //[Fact]
    //public void Test1()
    //{


    //    var value = new Data {Id = 1, Name = "Test", CreatedAt = DateTime.UtcNow };

    //    var data = new PkComplexOptions<Data>();

    //    var result = data.GetPkValues(new[] { "1", "Test", DateTime.UtcNow.ToString("o") }, d => new object[] { d.Id, d.Name, d.CreatedAt });

    //}
}



//public class Data
//{
//    public long Id { get; set; }
//    public string Name { get; set; }
//    public DateTime CreatedAt { get; set; }
//}



//public class PkComplexOptions<TEntity>
//    where TEntity : class
//{
//    public Func<TEntity, object[]> PkFields { get; set; }



//    public object[] GetPkValues(string[] values, Func<TEntity, object[]> pkFields)
//    {
//        var sample = Activator.CreateInstance<TEntity>();   // requiere new() o reflexión
//        var sampleP = pkFields(sample);                       // todos serán default/null
//        if (values.Length != sampleP.Length) throw new ArgumentException();

//        // ⚠️ Esto sólo funciona si los miembros devueltos son value types con default no-null.
//        // Si la PK es int / long / Guid → ok. Si es string → sampleP[i] será null y NO sabrás el tipo.
//        var result = new object[values.Length];
//        for (int i = 0; i < values.Length; i++)
//        {
//            var t = sampleP[i]?.GetType() ?? typeof(string);
//            result[i] = Convert.ChangeType(values[i], t, CultureInfo.InvariantCulture);
//        }
//        return result;
//    }


//}