using Exp.Operations;

namespace Exp.Spans;

internal class FinallyWordSpan : WordSpan, IContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "finally";
    public static string ItemName { get; } = "finally block";
    public IContext Context { get; set; }
    public List<Variable> Vars { get; set; } = [];
    public IVarSystem Parent { get; set; }
    public IVarSystem OuterVarSystem { get; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }

    internal FinallyWordSpan(Span[] innerSource, IVarSystem vs) : base(Keyword)
    {
        this.InnerSource = innerSource;
        this.OuterVarSystem = vs;
    }

    internal override string FullText => $"{Keyword}\n{{\n\t{InnerSource.ToString(" ")}\n}}";
}
