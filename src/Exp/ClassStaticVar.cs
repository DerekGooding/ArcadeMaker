using Exp.Spans;

namespace Exp;

internal class ClassStaticVar : Variable, IClassMember, IExpItem
{
    public new static string ItemName { get; } = "class static variable";
    public List<Span[]> TagsCode { get; set; }
    public Instance[] AttrInfo { get; set; }
    public ClassDefSpan Def { get; set; }
    internal Span[] InitValueCode { get; set; }

    internal ClassStaticVar(string name, IValue value, ClassDefSpan def, Span settingSpan, bool prvt = false, bool cons = false) : base(name, value, settingSpan, prvt, cons)
    {
        Def = def;
    }
}
