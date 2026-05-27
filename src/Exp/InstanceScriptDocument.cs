using Exp.Spans;

namespace Exp;

public class InstanceScriptDocument(string name, ClassDefSpan def, string script, params string[] args) : ScriptDocument(script, name)
{
    public ClassDefSpan Def { get; set; } = def;
    internal FuncDefSpan Runner { get; private set; }
    public string[] Args => args;

    public override bool TryPrepare(Interpreter compiler, out ExpError[] errors)
    {
        ArgumentNullException.ThrowIfNull(compiler);

        var errorsBefore = compiler.Errors.ToArray();

        CodeSpans = compiler.GetCodeSpans(TextSpans);
        var argSpans = args.Map(a => new ArgumentSpan(a));
        Runner = new FuncDefSpan(Name + ".runner", [.. argSpans], CodeSpans, Def) { Static = false, Document = this };
        Def.Funcs = Def.Funcs.Append(Runner).ToArray();
        Runner.Operations = compiler.ReadOperations(CodeSpans, Runner);

        errors = compiler.Errors.ToArray().Remove(err => errorsBefore.Contains(err)).ToArray();
        return errors.Length == 0;
    }

    public void Run(Interpreter compiler, Exp.Instance inst, params IValue?[] args)
    {
        if (Runner == null)
        {
            TryPrepare(compiler, out var errors);
            if (errors.Length >= 1)
                throw new BuildFailureException(errors);
        }

        compiler.RunOpsRunning = true;
        compiler.FuncCall(inst, Runner!, null, out var _, args);
        compiler.RunOpsRunning = false;
    }
}