using System.Globalization;

namespace MoralesLarios.OOFP.WebControllers.Helpers;

public static class Extensions
{


    public static object ConverterTo(this string value, Type property)
    {

        string fullNameTipe = property.FullName.Split('`')[0];

        object resultado = null;

        if (value != null)
        {
            switch (fullNameTipe)
            {
                //case "System.DateTime": resultado = DateTime.Parse(value); break;
                case "System.DateTime": resultado = ConvertDateTime(value); break;
                case "System.Int32"   : resultado = int     .Parse(value); break;
                case "System.Int16"   : resultado = short   .Parse(value); break;
                case "System.Char"    : resultado = char    .Parse(value); break;
                case "System.Double"  : resultado = double  .Parse(value); break;
                case "System.Decimal" : resultado = decimal .Parse(value); break;
                case "System.Int64"   : resultado = long    .Parse(value); break;
                case "System.SByte"   : resultado = sbyte   .Parse(value); break;
                case "System.Byte"    : resultado = byte    .Parse(value); break;
                case "System.UInt16"  : resultado = ushort  .Parse(value); break;
                case "System.UInt32"  : resultado = uint    .Parse(value); break;
                case "System.UInt64"  : resultado = ulong   .Parse(value); break;
                case "System.Boolean" : resultado = bool    .Parse(value); break;
                case "System.Single"  : resultado = float   .Parse(value); break;

                case "System.String"  : resultado = value.ToString(); break;

                case "System.Nullable":
                    string tipoDelNulable = property.FullName.Split('`')[1];

                    //if (tipoDelNulable.Contains("System.DateTime")) resultado = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.Parse(value);
                    if (tipoDelNulable.Contains("System.DateTime")) resultado = ConvertDateTime(value); 
                    if (tipoDelNulable.Contains("System.Int32"   )) resultado = string.IsNullOrEmpty(value) ? null : (int?     )int     .Parse(value);
                    if (tipoDelNulable.Contains("System.Int16"   )) resultado = string.IsNullOrEmpty(value) ? null : (short?   )short   .Parse(value);
                    if (tipoDelNulable.Contains("System.Char"    )) resultado = string.IsNullOrEmpty(value) ? null : (char?    )char    .Parse(value);
                    if (tipoDelNulable.Contains("System.Double"  )) resultado = string.IsNullOrEmpty(value) ? null : (double?  )double  .Parse(value);
                    if (tipoDelNulable.Contains("System.Decimal" )) resultado = string.IsNullOrEmpty(value) ? null : (decimal? )decimal .Parse(value);
                    if (tipoDelNulable.Contains("System.Int64"   )) resultado = string.IsNullOrEmpty(value) ? null : (long?    )long    .Parse(value);
                    if (tipoDelNulable.Contains("System.SByte"   )) resultado = string.IsNullOrEmpty(value) ? null : (sbyte?   )sbyte   .Parse(value);
                    if (tipoDelNulable.Contains("System.Byte"    )) resultado = string.IsNullOrEmpty(value) ? null : (byte?    )byte    .Parse(value);
                    if (tipoDelNulable.Contains("System.UInt16"  )) resultado = string.IsNullOrEmpty(value) ? null : (ushort?  )ushort  .Parse(value);
                    if (tipoDelNulable.Contains("System.UInt32"  )) resultado = string.IsNullOrEmpty(value) ? null : (uint?    )uint    .Parse(value);
                    if (tipoDelNulable.Contains("System.UInt64"  )) resultado = string.IsNullOrEmpty(value) ? null : (ulong?   )ulong   .Parse(value);
                    if (tipoDelNulable.Contains("System.Boolean" )) resultado = string.IsNullOrEmpty(value) ? null : (bool?    )bool    .Parse(value);
                    if (tipoDelNulable.Contains("System.Single"  )) resultado = string.IsNullOrEmpty(value) ? null : (float?   )float   .Parse(value);

                    if (tipoDelNulable.Contains("System.String")) resultado = value;

                    break;

                default: throw new FormatException(string.Format("The type {0} is not soported", fullNameTipe));
            }

        }


        return resultado;
    }


    private static DateTime ConvertDateTime(string value)
    {
        DateTime result = DateTime.MinValue;

        if (DateTime.TryParse(value, out result)) return result;

        var culture = System.Threading.Thread.CurrentThread.CurrentCulture;

        string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                                "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                                "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                                "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                                "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm",
                                "yyyyMMdd", "MMddyyyy", "dd/MM/yyyy", "ddMMyyyy"};

        if (DateTime.TryParseExact(value, formats, culture, DateTimeStyles.None, out result)) return result;
            
        throw new FormatException(string.Format("The type {0} is not soported", typeof(DateTime)));
    }

}
