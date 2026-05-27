namespace Exp;

public class SpecialValue<T> : IValue
{
    public string TypeName => typeof(T).Name;
    public T Value { get; set; }
    public override string ToString() => Value?.ToString() ?? "NULL";
    IValue IValue.Pass() => SpecialValue.From(Value);
    object IValue.Object => Value;
}