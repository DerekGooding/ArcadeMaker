using Exp.Operations;

namespace Exp.Spans;

public class FuncDefSpan : WordSpan, IContext, IDefinition, IKeyword, IClassMember, IExpItem, INamedValue
{
    public static string Keyword { get; } = "func";
    public static string ItemName { get; } = "function";
    public string Namespace { get; set; }
    public List<Variable> Vars { get; } = [];
    public IVarSystem Parent { get; set; } //= Interpreter.Activated.ParentVs;
    public IVarSystem OuterVarSystem { get; set; }
    public Span[] InnerSource
    { get; set { field = value; SetContainer(value); } }
    public IOperation[] Operations { get; set; }
    public IContext Context { get; set; }
    internal bool IsRunning { get; set; }
    public bool ReadOnly { get; internal set; }
    public string Name { get; }
    public bool Private { get; set; }
    public IValue Value => throw new Exception("A function value can only be accessed by calling it.");
    public bool IsVar => false;
    internal bool Static { get; set; }
    internal ArgumentSpan[] Args { get; }
    internal bool Return { get; set; } = false;
    internal IValue? Returns { get; set; }
    internal bool SpanItselfIsReadedAsValue { get; set; }

    internal ClassDefSpan DefinedAt { get; }
    public ClassDefSpan Def => DefinedAt;

    public string TypeName => ItemName;

    public List<Span[]> TagsCode { get; set; } = [];
    public Instance[] AttrInfo { get; set; }

    internal static FuncDefSpan ArrayIndexGetter, ArrayIndexSetter;
    internal static FuncDefSpan ExternInvoker = new("extern.invoker", [new ArgumentSpan("extrn", notNull: true), new ArgumentSpan("statc", notNull: true), new ArgumentSpan("memberName", true), new ArgumentSpan("inst"), new ArgumentSpan("args", notNull: true)], [], null);
    internal static FuncDefSpan ExternPropGetSet = new("extern.pgetset", [new ArgumentSpan("extrn", notNull: true), new ArgumentSpan("pinfo", true), new ArgumentSpan("inst"), new ArgumentSpan("value", notNull: false), new ArgumentSpan("set", notNull: true)], [], null);

    internal Variable[] ParamVariables { get; }

    internal FuncDefSpan(string name, ArgumentSpan[] args, Span[] innerSource, ClassDefSpan definedAt, string text = null) : base(text ?? Keyword)
    {
        this.Name = name;
        this.Args = args;
        this.InnerSource = innerSource;
        this.DefinedAt = definedAt;

        // create variables for parameter
        ParamVariables = new Variable[Args.Length];
        for (int i = 0; i < ParamVariables.Length; i++)
        {
            Variable pv = new(Args[i].Name, null, null, false);
            Vars.Add(pv);
            ParamVariables[i] = pv;
        }

        if (DefinedAt == ClassDefSpan.ExpArrayDef)
        {
            if (name == "get")
            {
                ArrayIndexGetter = this;
                this.Name = "array." + name;
            }
            else if (name == "set")
            {
                ArrayIndexSetter = this;
                this.Name = "array." + name;
            }
        }
    }

    internal override string FullText
    {
        get
        {
            string argsStr = "";
            foreach (var a in Args)
                argsStr += a.ToString() + " , ";
            if (Args.Length > 0)
                argsStr = argsStr.Substring(0, argsStr.Length - 3);
            string s = Static ? "static " : "";
            s += $"{Keyword} {Name} ( {argsStr} )\n{{\n\t{InnerSource.ToString(" ")}\n}}";
            return s;
        }
    }

    string IDefinition.FullName => (DefinedAt != null ? (DefinedAt.GetExpTypeName(false) + ".") : (Namespace == null ? "" : (Namespace + NamespaceSpecificationSpan.Symbol))) + Name + "(.." + Args.Length + ")";

    public override string ToString() => Keyword + " " + ((IDefinition)this).FullName;
}
