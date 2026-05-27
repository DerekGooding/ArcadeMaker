namespace Exp.Spans;

public class ConstructorDefSpan : FuncDefSpan, IKeyword, IExpItem
{
    public static string Keyword { get; } = "constructor";
    public static string ItemName { get; } = "constructor";

    public ConstructorDefSpan(ArgumentSpan[] args, Span[] innerSource, ClassDefSpan definedAt, Interpreter toThrowWith) : base(definedAt == null ? null : $"{definedAt.Name}.ctor", args, innerSource, definedAt, Keyword)
    {
        if (definedAt == null)
            toThrowWith.Error("Constructor must be defined inside a class.");

        base.ReadOnly = true;
    }

    internal override string FullText
    {
        get
        {
            var argsStr = "";
            foreach (var a in Args)
                argsStr += a.ToString() + " , ";
            if (Args.Length > 0)
                argsStr = argsStr.Substring(0, argsStr.Length - 3);
            var s = "";
            s = $"{Keyword} ({argsStr})\n{{\n\t{InnerSource.ToString(" ")}\n}}";
            return s;
        }
    }
}
