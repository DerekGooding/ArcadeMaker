namespace Exp;

public class SpecialValue : SpecialValue<object>
{
    public SpecialValue(object value) => Value = value;

    public static SpecialValue<T> From<T>(T val) => new SpecialValue<T> { Value = val };
}
