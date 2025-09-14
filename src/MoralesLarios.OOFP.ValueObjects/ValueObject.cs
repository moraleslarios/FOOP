namespace MoralesLarios.OOFP.ValueObjects;

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null) return false;

        return left is null || left.Equals(right!);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right) => ! EqualOperator(left, right);


    protected abstract IEnumerable<object> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;

        var other       = (ValueObject)obj;
        var thisValues  = GetAtomicValues().GetEnumerator();
        var otherValues = other.GetAtomicValues().GetEnumerator();

        while (thisValues.MoveNext() && otherValues.MoveNext())
        {
            if (thisValues.Current is null ^ otherValues.Current is null) return false;

            if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current)) return false;
        }
        return ! thisValues.MoveNext() && ! otherValues.MoveNext();
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
    }


    public static bool operator ==(ValueObject left, ValueObject right) => left.Equals(right);

    public static bool operator !=(ValueObject left, ValueObject right) => ! left.Equals(right);

    public ValueObject GetCopy() => (ValueObject)MemberwiseClone();

}


public class ValueObject<TValue> : ValueObject
{
    protected TValue Value;

    protected ValueObject(TValue value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        Value = value;
    }


    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value!;
    }

    public static implicit operator TValue(ValueObject<TValue> ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator ValueObject<TValue>(TValue value) => new ValueObject<TValue>(value);


    public override string? ToString() => Value?.ToString();

}
