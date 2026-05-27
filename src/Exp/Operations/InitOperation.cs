using Exp.Spans;

namespace Exp.Operations;

internal class InitOperation : IReadingOperation
{
    internal ClassDefSpan Def;
    internal IReadingOperation[] Args;
    internal ConstructorDefSpan Constructor { get; }

    internal InitOperation(ClassDefSpan def, IReadingOperation[] args)
    {
        Def = def ?? throw new ArgumentNullException(nameof(def));
        Args = args ?? throw new ArgumentNullException(nameof(args));

        // find the constructor with the given num of arguments
        var ctors = def.Funcs.Where(f => !f.Static && f.Args.Length == args.Length && f is ConstructorDefSpan);
        if (ctors.Count() == 0 && def.Funcs.OfType<ConstructorDefSpan>().Any())
            Interpreter.Activated.Error($"{((IDefinition)def).FullName} does not contain a constructor taking {args.Length} parameters.");
        if (ctors.Count() >= 2)
            throw new Exception($"More than 1 constructor taking {args.Length} parameters found.");

        Constructor = ctors.FirstOrNull() as ConstructorDefSpan; // first or null if the class has no constructors at all
    }

    public IValue Read()
    {
        Instance inst;
        if (Constructor?.DefinedAt == ClassDefSpan.ExpArrayDef && Constructor.Args.Length == 1) // new Array(len)
        {
            var len = (int)(Args[0].Read()?.Number ?? Interpreter.Activated.ThrowRuntime<int>("Argument value was null.", RuntimeException.INVALID_ARGUMENT)); // null check is critical because the arguments null check wasn't happening yet!
            if (len < 0)
                Interpreter.Activated.ThrowRuntime("Array length cannot have a negative size.", RuntimeException.INVALID_ARGUMENT);
            inst = new Instance(Def, new IValue[len]);
        }
        else
            inst = new Instance(Def);

        if (Constructor != null)
            Interpreter.Activated.FuncCall(inst, Constructor, null, out var _, Args.Select(a => a.Read()));

        return inst;
    }
}
