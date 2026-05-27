using Exp.Operations;

namespace Exp.Spans;

internal class SectionWordSpan : WordSpan, IContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "section";
    public static string ItemName { get; } = "section block";
    public IContext Context { get; set; }
    public List<Variable> Vars { get; } = []; public IVarSystem Parent { get; set; }
    public IVarSystem OuterVarSystem { get; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }

    internal SectionWordSpan(Span[] innerSource, IVarSystem vs) : base(Keyword)
    {
        this.InnerSource = innerSource;
        this.OuterVarSystem = vs;
    }

    internal override string FullText => $"{Keyword}\n{{\n\t{InnerSource.ToString(" ")}\n}}";
}
