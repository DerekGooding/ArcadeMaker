using Exp.Operations;

namespace Exp.Spans;

internal class CatchWordSpan : WordSpan, IContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "catch";
    public static string ItemName { get; } = "catch block";
    public IContext Context { get; set; }
    public List<Variable> Vars { get; set; } = [];
    public IVarSystem Parent { get; set; }
    public IVarSystem OuterVarSystem { get; }
    internal string VarName { get; }
    internal WhenWordSpan When { get; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }

    internal CatchWordSpan(string varname, WhenWordSpan when, Span[] innerSource, IVarSystem vs) : base(Keyword)
    {
        VarName = varname;
        When = when;
        InnerSource = innerSource;
        OuterVarSystem = vs;
    }

    internal override string FullText
    {
        get
        {
            var s = $"{Keyword} ";
            if (VarName != null)
                s += VarName;
            if (When != null)
                s += When.FullText;
            s += $"\n{{\n\t{InnerSource.ToString(" ")}\n}}";
            return s;
        }
    }
}
