namespace Exp.Spans;

internal class IfConditionSpan : ConditionSpan, IKeyword, IExpItem
{
    public static string Keyword { get; } = "if";
    public static string ItemName { get; } = "if statement";

    internal IfConditionSpan(Span[] condition, Span[] innerSource, IVarSystem varSystem) : base(Keyword, condition, innerSource, varSystem)
    {
    }
}
