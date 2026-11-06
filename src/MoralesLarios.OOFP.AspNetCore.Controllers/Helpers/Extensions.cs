// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Globalization;

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Helpers;

/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{


    /// <summary>
    /// Convierte el valor al tipo solicitado de <c>ToType</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="property">Valor de <c>property</c>.</param>
    /// <returns>Valor de tipo <c>object</c> obtenido por la operación.</returns>
    public static object ConvertToType(this string value, Type property)
    {

        string fullNameTipe = property.FullName.Split('`')[0];

        object result = null;

        if (value != null)
        {
            switch (fullNameTipe)
            {
                //case "System.DateTime": resultado = DateTime.Parse(value); break;
                case "System.DateTime": result = ParseDateTimeInvariant(value); break;
                case "System.Int32"   : result = int     .Parse(value); break;
                case "System.Int16"   : result = short   .Parse(value); break;
                case "System.Char"    : result = char    .Parse(value); break;
                case "System.Double"  : result = double  .Parse(value); break;
                case "System.Decimal" : result = decimal .Parse(value); break;
                case "System.Int64"   : result = long    .Parse(value); break;
                case "System.SByte"   : result = sbyte   .Parse(value); break;
                case "System.Byte"    : result = byte    .Parse(value); break;
                case "System.UInt16"  : result = ushort  .Parse(value); break;
                case "System.UInt32"  : result = uint    .Parse(value); break;
                case "System.UInt64"  : result = ulong   .Parse(value); break;
                case "System.Boolean" : result = bool    .Parse(value); break;
                case "System.Single"  : result = float   .Parse(value); break;

                case "System.String"  : result = value.ToString(); break;

                case "System.Nullable":
                    string nullableTypeName = property.FullName.Split('`')[1];

                    //if (tipoDelNulable.Contains("System.DateTime")) resultado = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.Parse(value);
                    if (nullableTypeName.Contains("System.DateTime")) result = ParseDateTimeInvariant(value);
                    if (nullableTypeName.Contains("System.Int32"   )) result = string.IsNullOrEmpty(value) ? null : (int?     )int     .Parse(value);
                    if (nullableTypeName.Contains("System.Int16"   )) result = string.IsNullOrEmpty(value) ? null : (short?   )short   .Parse(value);
                    if (nullableTypeName.Contains("System.Char"    )) result = string.IsNullOrEmpty(value) ? null : (char?    )char    .Parse(value);
                    if (nullableTypeName.Contains("System.Double"  )) result = string.IsNullOrEmpty(value) ? null : (double?  )double  .Parse(value);
                    if (nullableTypeName.Contains("System.Decimal" )) result = string.IsNullOrEmpty(value) ? null : (decimal? )decimal .Parse(value);
                    if (nullableTypeName.Contains("System.Int64"   )) result = string.IsNullOrEmpty(value) ? null : (long?    )long    .Parse(value);
                    if (nullableTypeName.Contains("System.SByte"   )) result = string.IsNullOrEmpty(value) ? null : (sbyte?   )sbyte   .Parse(value);
                    if (nullableTypeName.Contains("System.Byte"    )) result = string.IsNullOrEmpty(value) ? null : (byte?    )byte    .Parse(value);
                    if (nullableTypeName.Contains("System.UInt16"  )) result = string.IsNullOrEmpty(value) ? null : (ushort?  )ushort  .Parse(value);
                    if (nullableTypeName.Contains("System.UInt32"  )) result = string.IsNullOrEmpty(value) ? null : (uint?    )uint    .Parse(value);
                    if (nullableTypeName.Contains("System.UInt64"  )) result = string.IsNullOrEmpty(value) ? null : (ulong?   )ulong   .Parse(value);
                    if (nullableTypeName.Contains("System.Boolean" )) result = string.IsNullOrEmpty(value) ? null : (bool?    )bool    .Parse(value);
                    if (nullableTypeName.Contains("System.Single"  )) result = string.IsNullOrEmpty(value) ? null : (float?   )float   .Parse(value);

                    if (nullableTypeName.Contains("System.String")) result = value;

                    break;

                default: throw new FormatException(string.Format("The type {0} is not supported", fullNameTipe));
            }

        }


        return result;
    }


    private static DateTime ParseDateTimeInvariant(string value)
    {
        DateTime result = DateTime.MinValue;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) return result;

        string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                                "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                                "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                                "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                                "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm",
                                "yyyyMMdd", "MMddyyyy", "dd/MM/yyyy", "ddMMyyyy"};

        if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) return result;

        throw new FormatException(string.Format("The type {0} is not supported", typeof(DateTime)));
    }








    /// <summary>
    /// Convierte el texto en el tipo solicitado de <c>KeyValues</c>.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
    /// <param name="ids">Identificadores de las entidades.</param>
    /// <param name="pkFields">Función a aplicar.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static object[] ParseKeyValues<TEntity>(this string ids, Func<TEntity, object[]> pkFields)
        where TEntity : class
    {
        var result = ParseKeyValues(ids.Split(','), pkFields);

        return result;
    }
    /// <summary>
    /// Construye la instancia solicitada de <c>KeyParseErrorMessage</c>.
    /// </summary>
    /// <param name="ex">Excepción capturada.</param>
    /// <param name="ids">Identificadores de las entidades.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string BuildKeyParseErrorMessage(this Exception ex, string ids) => $"The key values '{ids}' could not be parsed as a comma-separated primary key. {ex.Message}";



    /// <summary>
    /// Convierte el texto en el tipo solicitado de <c>KeyValues</c>.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <param name="pkFields">Función a aplicar.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static object[] ParseKeyValues<TEntity>(string[] values, Func<TEntity, object[]> pkFields)
        where TEntity : class
    {
        var sample = Activator.CreateInstance<TEntity>();   // requiere new() o reflexión
        var sampleP = pkFields(sample);                       // todos serán default/null
        if (values.Length != sampleP.Length) throw new ArgumentException($"The number of provided values ({values.Length}) does not match the number of primary key fields ({sampleP.Length}).", nameof(values));

        // ⚠️ Esto sólo funciona si los miembros devueltos son value types con default no-null.
        // Si la PK es int / long / Guid → ok. Si es string → sampleP[i] será null y NO sabrás el tipo.
        var result = new object[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            var t = sampleP[i]?.GetType() ?? typeof(string);
            result[i] = Convert.ChangeType(values[i], t, CultureInfo.InvariantCulture);
        }
        return result;
    }


}

