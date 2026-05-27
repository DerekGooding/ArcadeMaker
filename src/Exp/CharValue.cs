namespace Exp;

public class CharValue(char value) : IValue
{
    public string TypeName => ValueHelper.tchar;
    bool IValue.IsChar => true;
    char IValue.Char => value;

    public static implicit operator CharValue(char val) => new(val);

    public override string ToString() => value.ToString();
}
