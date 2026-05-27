namespace Exp;

public class BoolValue(bool value) : IValue
{
    public string TypeName => ValueHelper.tbool;
    bool IValue.IsBool => true;
    bool IValue.Bool => value;
    public bool Bool => value;

    public static implicit operator BoolValue(bool val) => new(val);

    public static implicit operator bool(BoolValue val) => val.Bool;

    public override string ToString() => value ? "true" : "false";
}
