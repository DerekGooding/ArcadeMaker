namespace Exp.Spans;

internal class AttributeParamSpan : WordSpan, IExpItem
{
    public static string ItemName { get; } = "attribute parameter";
    internal string Name { get; }
    internal Instance ExpType { get; private set; }
    internal DefNameSpan ExpTypeName { get; }
    internal Type Type { get; }

    private AttributeParamSpan(string name) : base(name) => Name = name;

    internal AttributeParamSpan(Instance type, string name) : this(name) => ExpType = type;

    internal AttributeParamSpan(Type type, string name) : this(name) => Type = type;

    internal AttributeParamSpan(DefNameSpan defName, string name) : this(name)
    {
        ArgumentNullException.ThrowIfNull(defName);

        ExpTypeName = defName;
    }

    internal bool ResolveTypeName(IEnumerable<IDefinition> defs)
    {
        if (ExpTypeName != null)
        {
            if (ExpTypeName.Class == null)
                Interpreter.Activated.ThrowRuntime<Instance>("Attribute parameter must be of premitive / non-extern class type.", RuntimeException.INVALID_SYNTAX, this);
            else
                ExpType = ExpTypeName.Class.ExpType;
        }
        return true;
    }
}
