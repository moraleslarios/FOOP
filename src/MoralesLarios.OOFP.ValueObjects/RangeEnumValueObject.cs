//using System;

//namespace MoralesLarios.OOFP.ValueObjects;

//public class RangeEnumValueObject<TEnum> : ValueObject<string>
//    where TEnum : Enum
//{

//    protected RangeEnumValueObject(string value) : base(value)
//        => EvaluateValueByEnum(value);
//    public static RangeEnumValueObject<TEnum> FromEnum(TEnum enumValue) => FromString(enumValue.ToString());
//    public static RangeEnumValueObject<TEnum> FromString(string value) => new RangeEnumValueObject<TEnum>(value);


//    public static RangeEnumValueObject<TEnum> ByString(string enumStringValue)
//    {
//        var result = MlResult.Empty()
//                                .Map( _ => GetEnumStringValues())
//                                .MapEnsure( ensureFunc               : stringValues => stringValues.Any(x => string.Compare(x, enumStringValue, ignoreCase: true) == 0),
//                                            errorMessageResultBuilder: _ =>  $"{enumStringValue} no es un valor válido para el enum {typeof(TEnum).Name}")
//                                .Map( GetEnumStringValues)
//    }




//    public TEnum GetEnumValue()
//    {
//        var stringValues = GetEnumStringValues(); 

//        var enumValue = stringValues.FirstOrDefault(x => string.Compare(x, Value, ignoreCase: true) == 0);

//        var result = (TEnum)Enum.Parse(typeof(TEnum), enumValue);
//        return result;
//    }

//    protected virtual void EvaluateValueByEnum(string value)
//    {
//        var stringValues = GetEnumStringValues();    

//        bool isCorrect = stringValues.Any(x => string.Compare(x, value, ignoreCase: true) == 0);

//        if ( ! isCorrect)
//        {
//            var enumValuesStr = string.Join(",", stringValues);

//            throw new ArgumentException($"El valor {value}, no está dentro de los valores permitidos {enumValuesStr}, referentes al enumerado {enumType.Name}");
//        }
//    }


//    private static IEnumerable<string?> GetEnumStringValues()
//    {
//        var enumType = typeof(TEnum);

//        var result = Enum.GetValues(enumType).OfType<object>().Select(x => x.ToString());

//        return result;
//    }



//    public static implicit operator string(RangeEnumValueObject<TEnum> valueObject) => valueObject.Value;
//    public static implicit operator TEnum (RangeEnumValueObject<TEnum> valueObject) => valueObject.GetEnumValue();

//    public static implicit operator RangeEnumValueObject<TEnum>(string value) => FromString(value);
//    public static implicit operator RangeEnumValueObject<TEnum>(TEnum value ) => FromEnum  (value);

//}
