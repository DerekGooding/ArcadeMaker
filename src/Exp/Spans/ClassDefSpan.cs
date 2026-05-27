namespace Exp.Spans;

public class ClassDefSpan : WordSpan, IDefinition, IVarSystem, IKeyword, ICanSetAttr, IExpItem
{
    private static ClassDefSpan Create(string name, Property[] props)
    {
        var n = new ClassDefSpan(name, props);
        //n.Props.ForEach(pr => pr.Def = n); now done at constructor
        return n;
    }

    public static ClassDefSpan ExpArrayDef { get; private set; } = new("Array", []);
    public static ClassDefSpan ExpStringDef { get; private set; } = Create("string", [new Property(null, true, "chars", true, true)]);
    public static ClassDefSpan ExpExceptionDef { get; private set; } = new("Exception", []);
    public static ClassDefSpan ExternTypeValueDef { get; private set; } = new("ExternTypeValue", []);
    public static ClassDefSpan ExpTypeDef { get; private set; } = new("Type", []);
    public static ClassDefSpan ExpAttrInfoDef { get; private set; } = new("AttrInfo", []);

    public static string Keyword { get; } = "class";
    public static string ItemName { get; } = "class";
    public string Namespace { get; set; }
    public string Name { get; }
    public List<Variable> Vars { get; } = []; // static props
    public IVarSystem Parent { get; set; }
    public Property[] Props { get; }
    public FuncDefSpan[] Funcs { get; set; }
    internal Property BaseArrayProp => Props.FirstOrDefault(p => p.BaseArray);

    public Instance ExpType
    {
        get
        {
            if (_expType == null)
            {
                _expType = new Instance(ExpTypeDef ?? throw new Exception("Trying to get type instance before Type def was collected."));
                _expType.Vars[0].SetSkippingConstant(Interpreter.StringToExpString(Name));
                _expType.Vars[1].SetSkippingConstant(Interpreter.StringToExpString(Namespace + NamespaceSpecificationSpan.Symbol + Name));
                _expType.Vars[2].SetSkippingConstant(SpecialValue.From(this));
            }
            return _expType;
        }
    }

    private Instance _expType;
    public List<Span[]> TagsCode { get; set; } = [];
    public Instance[] AttrInfo { get; set; }

    internal new FuncDefSpan ToString { get; set; }
    internal new FuncDefSpan Equalizer { get; set; }

    public ClassDefSpan(string name, Property[] props, FuncDefSpan[] funcs = null, Instance[] attr = null) : base(Keyword)
    {
        this.Name = name;
        this.Props = props;
        props.ForEach(p => p.Def = this);
        this.Funcs = funcs;
        this.AttrInfo = attr;

        if (name == "Array")
            ExpArrayDef = this;
        else if (name == "string")
            ExpStringDef = this;
        else if (name == "Exception")
            ExpExceptionDef = this;
        else if (name == "ExternTypeValue")
            ExternTypeValueDef = this;
        else if (name == "Type")
            ExpTypeDef = this;
        else if (name == "AttributeInfo")
            ExpAttrInfoDef = this;
    }

    internal override string FullText
    {
        get
        {
            string propsStr = "", funcsStr = "", staticsStr = "";
            foreach (var a in Props)
                propsStr += (a.Const ? "const " : "") + a.Name + (a.Private ? " private" : "") + " , ";
            if (Props.Length > 0)
                propsStr = propsStr.Substring(0, propsStr.Length - 3);

            foreach (var v in Vars)
                staticsStr += $"static " + (v.Private ? "private " : "") + (v.Const ? "const " : "") + v.Name + "\n";

            foreach (var func in Funcs)
                funcsStr += func.FullText + "\n";
            string s = $"{Keyword} {Name} ( {propsStr} )\n{{\n\t{funcsStr}\n}}";
            return s;
        }
    }
}
