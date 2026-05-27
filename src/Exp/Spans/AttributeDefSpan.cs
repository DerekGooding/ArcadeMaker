namespace Exp.Spans;

internal class AttributeDefSpan : WordSpan, IDefinition, IKeyword, ICanSetAttr, IExpItem
{
    internal new static AttributeDefSpan ToString = new("Translator", [])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = true, AllowFor_Property = false, AllowFor_Attr = false, LimitTo1InCls = true, Func_StaticRequirement = StaticRequirement.NonStatic, Func_ParamsCountRequirement = 0 };

    internal new static AttributeDefSpan EqualizerAttr = new("Equalizer", [])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = true, AllowFor_Property = false, AllowFor_Attr = false, LimitTo1InCls = true, Func_StaticRequirement = StaticRequirement.NonStatic, Func_ParamsCountRequirement = 1 };

    internal static AttributeDefSpan AllowFor = new("AllowFor",
        [new(typeof(BoolValue), "class"),
        new(typeof(BoolValue), "property"),
        new(typeof(BoolValue), "func"),
        new(typeof(BoolValue), "ctor"),
        new(typeof(BoolValue), "attr")])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = false, AllowFor_Property = false };

    internal static AttributeDefSpan AllowMultipleAttr = new("AllowMultiple", [])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = false, AllowFor_Property = false };

    internal static AttributeDefSpan LimitTo1InClsAttr = new("OneInClass", [])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = false, AllowFor_Property = false };

    internal static AttributeDefSpan FuncRequirementsAttr = new("FuncRequirements",
        [
            new(typeof(NumberValue), "stat"),
            new(typeof(NumberValue), "params")
        ])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = false, AllowFor_Property = false };

    internal static AttributeDefSpan ExpectFuncAttr;
    /*= new("ExpectFunc",
        [new(ClassDefSpan.ExpStringDef, "name"),
        new(typeof(NumberValue), "paramsCount"),
        new(typeof(BoolValue), "static")])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = false, AllowFor_Property = false, AllowMultiple = true };
    */

    internal static AttributeDefSpan ReadOnlyAttr = new("ReadOnly", [])
    { AllowFor_Class = false, AllowFor_Constructor = false, AllowFor_Func = true, AllowFor_Property = false, AllowFor_Attr = false };

    internal static AttributeDefSpan IteratableAttr /*= new("Iteratable", [])
    { AllowFor_Class = true, AllowFor_Constructor = false, AllowFor_Func = false, AllowFor_Property = false, AllowFor_Attr = false }*/;

    public static string Keyword { get; } = "attribute";
    public static string ItemName { get; } = "attribute";
    public string Name { get; }
    public string Namespace { get; set; }
    internal AttributeParamSpan[] Params { get; }
    public List<Span[]> TagsCode { get; set; } = [];
    public Instance[] AttrInfo { get; set; }
    internal bool AllowFor_Class { get; set; } = true;
    internal bool AllowFor_Property { get; set; } = true;
    internal bool AllowFor_Func { get; set; } = true;
    internal bool AllowFor_Constructor { get; set; } = true;
    internal bool AllowFor_Attr { get; set; } = true;
    internal bool AllowMultiple { get; set; }
    internal bool LimitTo1InCls { get; set; }

    internal enum StaticRequirement
    {
        Static,
        NonStatic,
        Nevermind
    }

    internal StaticRequirement Func_StaticRequirement { get; set; } = StaticRequirement.Nevermind;
    internal int Func_ParamsCountRequirement = -1;

    internal Instance ExpType
    {
        get
        {
            if (_expType == null)
            {
                _expType = new Instance(ClassDefSpan.ExpTypeDef ?? throw new Exception("Trying to get type instance before Type def was collected."));
                _expType.Vars[0].SetSkippingConstant(Interpreter.StringToExpString(Name));
                _expType.Vars[1].SetSkippingConstant(Interpreter.StringToExpString(Namespace + NamespaceSpecificationSpan.Symbol + Name));
                _expType.Vars[2].SetSkippingConstant(SpecialValue.From(this));
            }
            return _expType;
        }
    }

    private Instance _expType;

    internal AttributeDefSpan(string name, AttributeParamSpan[] param) : base(Keyword)
    {
        this.Name = name;
        this.Params = param;
        if (name == "ExpectFunc")
            ExpectFuncAttr ??= this;
        else if (name == "Iteratable")
            IteratableAttr ??= this;
    }
}
