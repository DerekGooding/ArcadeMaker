using Exp.Operations;

namespace Exp.Spans;

internal abstract class ConditionSpan : WordSpan, IContext
{
    public List<Variable> Vars { get; } = [];
    public IVarSystem Parent { get; set; }
    public IVarSystem OuterVarSystem { get; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }
    internal Span[] Condition { get; }
    public IContext Context { get; set; }
    internal bool ConditionWasTrue { get; set; }
    internal override string FullText => Text + ' ' + Condition + "\n{\n\t" + InnerSource + "\n}";

    internal ConditionSpan(string text, Span[] condition, Span[] innerSource, IVarSystem varSystem) : base(text)
    {
        InnerSource = innerSource;
        Condition = condition;
        OuterVarSystem = varSystem;
    }

    internal override Span Container
    {
        get;
        set { field = value; SetContainer(Condition, value); }
    }
}
