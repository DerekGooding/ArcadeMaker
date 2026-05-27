using Exp.Operations;

namespace Exp.Spans;

internal class TryWordSpan : WordSpan, IContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "try";
    public static string ItemName { get; } = "try block";
    public IContext Context { get; set; }
    public List<Variable> Vars { get; } = []; public IVarSystem Parent { get; set; }
    public IVarSystem OuterVarSystem { get; }
    internal CatchWordSpan Catch { get; set; }
    internal FinallyWordSpan Finally { get; set; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }

    internal TryWordSpan(Span[] innerSource, IVarSystem vs, CatchWordSpan catc, FinallyWordSpan finaly) : base(Keyword)
    {
        InnerSource = innerSource;
        Catch = catc;
        Finally = finaly;
        OuterVarSystem = vs;
    }

    internal override string FullText => $"{Keyword}\n{{\n\t{InnerSource.ToString(" ")}\n}}";
}
