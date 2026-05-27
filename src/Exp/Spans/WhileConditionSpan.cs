namespace Exp.Spans;

internal class WhileConditionSpan : ConditionSpan, ILoopContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "while";
    public static string ItemName { get; } = "while loop";
    public bool Break { get; set; }
    public bool Continue { get; set; }
    public string Counter { get; set; }
    public string Id { get; set; }

    internal override string FullText
    {
        get
        {
            var s = $"{Keyword} {Condition}";
            if (Id != null || Counter != null)
                s += " : ";
            if (Id != null)
                s += "id " + Id;
            if (Id != null && Counter != null)
                s += " , ";
            if (Counter != null)
                s += "counter " + Counter;
            s += $"\n{{\n\t{InnerSource.ToString(" ")}\n}}";
            return s;
        }
    }

    internal WhileConditionSpan(Span[] condition, Span[] innerSource, IVarSystem varSystem) : base(Keyword, condition, innerSource, varSystem)
    {
    }
}
