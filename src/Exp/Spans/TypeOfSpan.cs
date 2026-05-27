namespace Exp.Spans;

internal class TypeOfSpan : WordSpan
{
    internal Instance Value { get; set; }

    private TypeOfSpan(Instance value) : base("<>") => this.Value = value;

    internal TypeOfSpan(ExternClassDefSpan ext) : this(ext.Type.AsExtern())
    {
    }

    internal TypeOfSpan(ClassDefSpan cls) : this(cls.ExpType)
    {
    }

    internal TypeOfSpan(AttributeDefSpan attr) : this(attr.ExpType)
    {
    }

    internal TypeOfSpan() : this(value: null)
    {
    }

    internal override string FullText => LowerThanOperatorSpan.Symbol + Value?.def.Namespace + NamespaceSpecificationSpan.Symbol + Value.Vars.Find(v => v.Name == "name").Value.Inst.ToString() + GreaterThanOperatorSpan.Symbol;
}
