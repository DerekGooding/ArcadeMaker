using Exp.Spans;

namespace Exp.Operations;

internal class ExternTypeInitOperation : IReadingOperation
{
    internal ExternClassDefSpan Extern { get; }
    internal IReadingOperation[] Args { get; }

    internal ExternTypeInitOperation(ExternClassDefSpan extrn, IReadingOperation[] args)
    {
        ArgumentNullException.ThrowIfNull(extrn);
        ArgumentNullException.ThrowIfNull(args);

        Extern = extrn;
        Args = args;

        if (!Extern.Type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Any(ctor => ctor.GetParameters().Length == Args.Length))
            Interpreter.Activated.Error($"Extern class '{extrn.GetExpTypeName(false)}' does not contain a constructor taking {Args.Length} parameters.");
    }

    public IValue Read() => Interpreter.NewExtern(Extern.Type, [.. Args.Map(a => a.Read())]);
}
