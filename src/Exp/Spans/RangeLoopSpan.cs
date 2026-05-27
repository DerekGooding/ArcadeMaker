using Exp.Operations;

namespace Exp.Spans;

internal class RangeLoopSpan : WordSpan, ILoopContext, IKeyword, IExpItem
{
    public static string Keyword { get; } = "foreach";
    public static string ItemName { get; } = "foreach loop";
    public List<Variable> Vars { get; } = [];
    public IVarSystem Parent { get; set; }
    public IVarSystem OuterVarSystem { get; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }
    public IContext Context { get; set; }
    public bool Break { get; set; }
    public bool Continue { get; set; }
    public string Counter { get; set; }
    internal Span[] FromReadText { get; }
    internal Span[] ToReadText { get; }
    internal string VarName { get; }
    internal bool ConstVar { get; }
    public string Id { get; set; }

    internal override string FullText
    {
        get
        {
            var s = $"{Keyword} {VarName} from {FromReadText} to {ToReadText}";
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

    internal RangeLoopSpan(string varName, bool constVar, Span[] fromReadText, Span[] toReadText, Span[] innerSource, IVarSystem varSystem) : base(Keyword)
    {
        VarName = varName;
        ConstVar = constVar;
        FromReadText = fromReadText;
        ToReadText = toReadText;
        InnerSource = innerSource;
        OuterVarSystem = varSystem;
    }

    internal override Span Container
    {
        get;
        set { field = value; SetContainer(FromReadText, value); SetContainer(ToReadText, value); }
    }
}
