using System.Reflection;

namespace Exp.Spans;

internal class ExternClassDefSpan : WordSpan, IDefinition, IKeyword, IExpItem
{
    public static string Keyword { get; } = "extern";
    public static string ItemName { get; } = "extern class";
    public string Name => RefName;
    public string Namespace { get; set; }
    internal string RefName { get; }
    internal Type Type { get; }
    internal MethodInfo[] Methods { get; }
    internal ConstructorInfo[] Constructors { get; }
    internal PropertyInfo[] Props { get; }
    public bool IsEnum { get; }
    public Dictionary<string, IValue> EnumValues { get; } = [];

    internal ExternClassDefSpan(string refName, Type type) : base(Keyword)
    {
        this.RefName = refName;
        this.Type = type;
        this.IsEnum = type.IsEnum;

        if (IsEnum)
        {
            Array vals = type.GetEnumValues();
            type.GetEnumNames().ForEach((name, index) => EnumValues.Add(name.StartWithLowerCase(), Interpreter.CsValToExpVal(vals.GetValue(index))));
        }

        // get all public static methods of the type
        this.Methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
        this.Constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        this.Props = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
    }

    internal override string FullText => $"{Keyword} {RefName} = \"{Type}\"";
}
