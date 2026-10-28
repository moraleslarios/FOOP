// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>ValueObject</c>.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Ejecuta <c>EqualOperator</c>.
    /// </summary>
    /// <param name="left">Valor de <c>left</c>.</param>
    /// <param name="right">Valor de <c>right</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null) return false;

        return left is null || left.Equals(right!);
    }

    /// <summary>
    /// Ejecuta <c>NotEqualOperator</c>.
    /// </summary>
    /// <param name="left">Valor de <c>left</c>.</param>
    /// <param name="right">Valor de <c>right</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right) => ! EqualOperator(left, right);


    /// <summary>
    /// Obtiene el elemento solicitado de <c>AtomicValues</c>.
    /// </summary>
    /// <returns>Colección con los elementos obtenidos.</returns>
    protected abstract IEnumerable<object> GetAtomicValues();

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
    }


    /// <summary>
    /// Implementa el operador <c>==</c>.
    /// </summary>
    /// <param name="left">Valor de <c>left</c>.</param>
    /// <param name="right">Valor de <c>right</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool operator ==(ValueObject left, ValueObject right) => left.Equals(right);

    /// <summary>
    /// Implementa el operador <c>!=</c>.
    /// </summary>
    /// <param name="left">Valor de <c>left</c>.</param>
    /// <param name="right">Valor de <c>right</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool operator !=(ValueObject left, ValueObject right) => ! left.Equals(right);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Copy</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>ValueObject</c> obtenido por la operación.</returns>
    public ValueObject GetCopy() => (ValueObject)MemberwiseClone();

}


/// <summary>
/// Representa <c>ValueObject</c>.
/// </summary>
/// <typeparam name="TValue">Tipo del valor.</typeparam>
public class ValueObject<TValue> : ValueObject
{
    /// <summary>
    /// Valor de <c>Value</c>.
    /// </summary>
    protected TValue Value;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ValueObject{TValue}"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected ValueObject(TValue value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        Value = value;
    }


    /// <inheritdoc/>
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value!;
    }

    /// <summary>
    /// Realiza la conversión implícita a <c>TValue</c>.
    /// </summary>
    /// <param name="ValueObjectFp">Valor de <c>ValueObjectFp</c>.</param>
    /// <returns>Valor de tipo <c>TValue</c> obtenido por la operación.</returns>
    public static implicit operator TValue(ValueObject<TValue> ValueObjectFp) => ValueObjectFp.Value;

    /// <summary>
    /// Realiza la conversión explícita a <c>ValueObject&lt;TValue&gt;</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>ValueObject&lt;TValue&gt;</c> obtenido por la operación.</returns>
    public static explicit operator ValueObject<TValue>(TValue value) => new ValueObject<TValue>(value);


    /// <inheritdoc/>
    public override string? ToString() => Value?.ToString();

}

