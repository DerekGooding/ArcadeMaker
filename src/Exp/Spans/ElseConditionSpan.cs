using Exp.Operations;

namespace Exp.Spans;

internal class ElseConditionSpan : WordSpan, IContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "else";
    public static string ItemName { get; } = "else statement";
    public List<Variable> Vars { get; } = []; public IVarSystem Parent { get; set; }
    public IVarSystem OuterVarSystem { get; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }
    public IContext Context { get; set; }

    internal override string FullText => $"{Keyword}\n{{\n\t{InnerSource.ToString(" ")}\n}}";

    internal ElseConditionSpan(Span[] innerSource, IVarSystem varSystem) : base(Keyword)
    {
        InnerSource = innerSource;
        OuterVarSystem = varSystem;
        SetContainer(InnerSource);
    }
}
