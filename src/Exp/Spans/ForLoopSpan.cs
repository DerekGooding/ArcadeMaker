namespace Exp.Spans;

internal class ForLoopSpan : ConditionSpan, ILoopContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "for";
    public static string ItemName { get; } = "for loop";
    public bool Break { get; set; }
    public bool Continue { get; set; }
    public string Counter { get; set; }
    internal Span[] InitExe { get; set; }
    internal Span[] StepExe { get; }
    public string Id { get; set; }

    internal override string FullText
    {
        get
        {
            var s = $"{Keyword} ({InitExe}; {Condition}; {StepExe})";
            if (Id != null || Counter != null)
                s += " : ";
            if (Id != null)
                s += "id " + Id;
            if (Id != null && Counter != null)
                s += ", ";
            if (Counter != null)
                s += "counter " + Counter;
            s += $"\n{{\n\t{InnerSource.ToString(" ")}\n}}";
            return s;
        }
    }

    internal ForLoopSpan(Span[] initExe, Span[] condition, Span[] stepExe, Span[] innerSource, IVarSystem varSystem) : base(Keyword, condition, innerSource, varSystem)
    {
        InitExe = initExe;
        StepExe = stepExe;
        SetContainer(InitExe);
        SetContainer(StepExe);
        SetContainer(condition);
    }

    internal override Span Container
    {
        get;
        set;//{ field = value; SetContainer(InitExe, value); SetContainer(StepExe, value); base.Container = value; }
    }
}
