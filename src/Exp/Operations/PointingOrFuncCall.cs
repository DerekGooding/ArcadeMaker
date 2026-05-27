using Exp.Spans;

namespace Exp.Operations;

internal class PointingOrFuncCall(bool isOp, string name, IEnumerable<IReadingOperation[]> argLists, int? paramsCounter, IVarSystem vs, Span span, bool readValue, bool first) : IReadingOperation
{
    internal IVarSystem VS { get => vs; set => vs = value; }
    internal PointingOrFuncCall Next { get; set; }
    internal bool NextOrNull { get; set; }
    internal Variable KnownPointer { get; set; }
    internal FuncDefSpan KnownFunc { get; set; }

    internal INamedValue Known
    {
        get => KnownPointer ?? (INamedValue)KnownFunc;
        set
        {
            if (value is Variable p)
                KnownPointer = p;
            else if (value is FuncDefSpan f)
                KnownFunc = f;
            else if (value == null)
            {
                KnownPointer = null;
                KnownFunc = null;
            }
            else
                throw new ArgumentException($"Unknown named value type: {value.GetType().Name}");
        }
    }

    internal string Name => name;
    internal IEnumerable<IReadingOperation[]> ArgLists => argLists;

    public IValue Read()
    {
        //ArgumentNullException.ThrowIfNull(VS);
        IValue value;

        // 'this' keyword
        if (first && Known == null && name == ThisWordSpan.Keyword)
        {
            var thiss = Interpreter.FindParentVarSystem<Instance>(vs);

            if (thiss != null)
            {
                if (Next == null)
                    return thiss;

                Next.VS = thiss;
                return Next.Read();
            }

            Interpreter.Activated.ThrowRuntime($"Keyword 'this' is not valid in a static content.", RuntimeException.INVALID_OPERATION, span);
        }

        // get the current item
        var numOfParams = paramsCounter.HasValue ? paramsCounter.Value : (argLists.FirstOrDefault()?.Length ?? -1);
        var item = Known ?? Interpreter.Activated.GetNamedValueItem(VS, name, span, first, numOfParams);
        if (item == null)
        {
            Interpreter.Activated.ThrowRuntime(numOfParams >= 0 ? $"Unknown function '{(VS as Instance)?.def.GetExpTypeName(false) ?? "(?)"}.{name}(..{numOfParams})'." : $"Unknown item '{(VS as Instance)?.def.GetExpTypeName(false) ?? "(?)"}.{name}'.", RuntimeException.INVALID_SYNTAX, span);
            throw null!;
        }

        // if no more dots, return pointer or call the func and return its value
        if (Next == null)
        {
            if (argLists.Any())
            {
                if (Name == "pause")
                    _ = 1;
                FuncDefSpan? fn = item as FuncDefSpan;
                Instance? inst = VS as Instance;
                if (fn == null && item is Variable v && v.Value?.IsFunc == true) // it's not a function, but it IS a function POINTER
                {
                    fn = v.Value.FuncPntr.Func;
                    inst = v.Value.FuncPntr.Instance;
                }
                return RunFunc(fn, inst);
            }
            else
            {
                if (readValue)
                {
                    if (item.IsVar)
                        return item.Value;
                }

                if (item is FuncDefSpan fn)
                    return new FuncPntr(fn, VS as Instance);

                if (item is IValue v)
                    return v;

                return SpecialValue.From(item);
            }
        }

        // if there are more dots, go to next pointing
        Instance? funcPntrInst = null;
        if (item is Variable pntr)
        {
            if (pntr.Value == null)
            {
                if (NextOrNull)
                    return readValue ? null : SpecialValue.From<INamedValue>(Variable.Futile);
                ThrowNullRef();
            }
            else if (pntr.Value.IsInst)
            {
                var inst = pntr.Value.Inst;
                if (inst.def != ClassDefSpan.ExternTypeValueDef || Next.ArgLists?.Any() == false)
                {
                    Next.VS = inst;
                    value = Next.Read();
                    goto Return;
                }
                else
                {
                    // invoke extern method
                    var csInst = (inst as ExternTypeInstance)?.ExternInstance;
                    var args = Next.ArgLists.FirstOrDefault();
                    value = Interpreter.Activated.FuncCall(null, FuncDefSpan.ExternInvoker, null, out var _, [SpecialValue.From(csInst.GetType()), false.ToExp(), SpecialValue.From(Next.Name), SpecialValue.From(csInst), SpecialValue.From(Next.ArgLists.FirstOrDefault()) /*.Select(.Read()) is done in the builtin func code*/]);
                    goto Return;
                }
            }
            else if (pntr.Value is FuncPntr funcpntr)
            {
                funcPntrInst = funcpntr.Instance;
                item = funcpntr.Func; // so it's like if it was a direct function
            }
            else
            {
                ThrowPremitiveRef();
                throw null;
            }
        }
        if (item is FuncDefSpan func)
        {
            var fresult = RunFunc(func, funcPntrInst); // funcPntrInst is null unless "item" was a Variable which is value was a FuncPntr. in this case, "func" is the value of this FuncPntr "Func" property
            if (fresult == null)
            {
                if (NextOrNull)
                    return readValue ? null : SpecialValue.From<INamedValue>(Variable.Futile);
                ThrowNullRef();
            }
            if (fresult.IsInst)
            {
                Next.VS = fresult.Inst;
                value = Next.Read();
            }
            else
            {
                ThrowPremitiveRef();
                throw null;
            }
        }
        else if (item == null)
        {
            Interpreter.Activated.ThrowRuntime($"Unknown item '{name}'.", RuntimeException.INVALID_SYNTAX, span);
            throw null;
        }
        else
        {
            throw new Exception($"Something went wrong (unexpected INamedValue type: {item} [{item.GetType()}]).");
        }

    Return:
        return value;

        IValue RunFunc(FuncDefSpan? func, Instance? instance)
        {
            if (func == null)
                Interpreter.Activated.ThrowRuntime($"An argument list was read, but the given value was not a function but {Extensions.GetExpTypeName(item, true)} ({name}).", RuntimeException.INVALID_SYNTAX, span);

            if (!argLists.Any())
                Interpreter.Activated.ThrowRuntime("Missing argument list.", RuntimeException.INVALID_SYNTAX, span);

            if (isOp && Next == null && func.ReadOnly)
                Interpreter.Activated.ThrowRuntime($"The value returned by {func.GetExpTypeName(false)} must be used.", RuntimeException.INVALID_OPERATION, span);

            IValue val = null;
            foreach (var ls in argLists)
            {
                instance ??= func == FuncDefSpan.ExternInvoker || func == FuncDefSpan.ExternPropGetSet ? null : (first ? Interpreter.FindParentVarSystem<Instance>(vs) : VS as Instance);
                val = Interpreter.Activated.FuncCall(instance, func, null, out var _, ls.Map(a => a?.Read()));
                func = val as FuncDefSpan;
            }
            return val;
        }

        void ThrowNullRef() => Interpreter.Activated.ThrowRuntime("Object reference not set to an instance of an object.", RuntimeException.NULL_REFERENCE, span);
        void ThrowPremitiveRef() => Interpreter.Activated.ThrowRuntime($"The value of {item.Name} was a premitive type and it cannot be followed by a dot.", RuntimeException.INVALID_OPERATION, span);
    }
}
