using Exp.Spans;

namespace Exp.Operations;

internal class ExternInvocationOperation(ExternClassDefSpan extrn, object inst, string method, IReadingOperation[] args) : IReadingOperation
{
    internal ExternClassDefSpan Extern => extrn ?? throw new ArgumentNullException();
    internal IReadingOperation[] Args => args ?? throw new ArgumentNullException();
    internal string Method => method ?? throw new ArgumentNullException();

    public IValue Read()
    {
        var statc = false ? false : throw new NotImplementedException();
        return Interpreter.InvokeExtern(Extern.Type, statc, inst, method, [.. args.Select(a => a.Read())]);
    }
}
