namespace Exp;

public readonly struct NumberValue(double value) : IValue
{
    public string TypeName => ValueHelper.tnum;
    bool IValue.IsNumber => true;
    double IValue.Number => value;

    public static implicit operator NumberValue(double val) => new(val);

    public static implicit operator double(NumberValue val) => ((IValue)val).Number;

    public override string ToString() => value.ToString();
}
