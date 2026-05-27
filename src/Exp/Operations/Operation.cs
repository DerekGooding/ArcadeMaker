namespace Exp.Operations;

internal class Operation(Action action) : IOperation
{
    public void Make() => action();

    internal static IOperation Custom(Action action) => new Operation(action);

    internal static IOperation Error => new Operation(() => Interpreter.Activated.ThrowRuntime("Execution reached a build error.", "EXE_REACHED_BUILD_ERR"));
}
