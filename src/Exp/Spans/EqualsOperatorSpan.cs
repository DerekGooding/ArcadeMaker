namespace Exp.Spans;

internal class EqualsOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "==";
    internal override bool TwoSides { get; } = true;

    internal EqualsOperatorSpan() : base(Symbol)
    {
    }

    internal override IValue Result(IValue left, IValue right) => GetResult(left, right);

    internal static BoolValue GetResult(IValue left, IValue right)
    {
        if (left == null)
            return right == null;
        else if (right == null)
            return left?.IsInst == true ? left.Equals(right) : left == right;
        else if (left.IsBool && right.IsBool)
            return left.Bool == right.Bool;
        else if (left.IsNumber && right.IsNumber)
            return left.Number == right.Number;
        else if (left.IsChar && right.IsChar)
            return left.Char == right.Char;
        else if (left.IsNumber && right.IsChar)
            return left.Number == right.Char;
        else if (left.IsChar && right.IsNumber)
            return left.Char == right.Number;
        //else if (left.IsInst && right.IsInst)
        //    return (left.Inst.IsString() && right.Inst.IsString()) ? left.Inst.ExpStringEquals(right.Inst) : left.Inst.Equals(right.Inst);
        else if (left.Equals(right)) // for funcs. and for instances that wouldn't work as Instance == Instance operator is overrided
            return true;
        else if (left is SpecialValue<object> l && right is SpecialValue<object> r)
            return l.Value == r.Value;
        return false;
    }
}
